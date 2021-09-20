using System;
using System.ComponentModel;
using Android.Content;
using Android.Gms.Extensions;
using Android.Hardware.Camera2;
using Android.Util;
using AndroidX.Camera.Camera2.InterOp;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using Google.Common.Util.Concurrent;
using Java.Lang;
using Java.Util.Concurrent;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Common;
using Exception = Java.Lang.Exception;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Renderer
{
    internal class CameraViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<CameraView, PreviewView>
    {

        private bool _isDisposed;

        private IListenableFuture _cameraFuture;
        private IExecutorService _cameraExecutor;

        private ICamera _camera;


        public static void Init() { }

        public CameraViewRenderer(Context context) : base(context)
        {
            _cameraExecutor = Executors.NewSingleThreadExecutor();
            _cameraFuture = ProcessCameraProvider.GetInstance(context);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null) return;
            if (Control == null)
            {
                // Instantiate the native control and assign it to the Control property with
                // the SetNativeControl method
                SetNativeControl(CreateNativeControl());
            }
            // Configure the control and subscribe to event handlers
            _cameraFuture.AddListener(new Runnable(CameraCallback), ContextCompat.GetMainExecutor(Context));

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CameraView.TorchOnProperty.PropertyName)
                HandleTorch();
        }

        protected override PreviewView CreateNativeControl() => new PreviewView(Context);

        private void CameraCallback()
        {
            // Used to bind the lifecycle of cameras to the lifecycle owner
            var cameraProvider = (ProcessCameraProvider)_cameraFuture.Get();

            if (cameraProvider == null)
                return;

            // Preview
            var preview = new Preview.Builder().Build();
            preview.SetSurfaceProvider(Control.SurfaceProvider);
            
            // Frame by frame analyze
            var imageAnalyzerBuilder = new ImageAnalysis.Builder();
            if (Element.RequestedFPS.HasValue)
            {
                Camera2Interop.Extender ext = new Camera2Interop.Extender(imageAnalyzerBuilder);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeMode, 0);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeTargetFpsRange, new Android.Util.Range((int)Element.RequestedFPS.Value, (int)Element.RequestedFPS.Value));
            }
            var imageAnalyzer = imageAnalyzerBuilder.Build();
            imageAnalyzer.SetAnalyzer(_cameraExecutor, new BarcodeAnalyzer(this));

            // Select back camera as a default, or front camera otherwise
            CameraSelector cameraSelector = null;
            if (cameraProvider.HasCamera(CameraSelector.DefaultBackCamera))
                cameraSelector = CameraSelector.DefaultBackCamera;
            else if (cameraProvider.HasCamera(CameraSelector.DefaultFrontCamera))
                cameraSelector = CameraSelector.DefaultFrontCamera;
            else
                throw new System.Exception("Camera not found");

            try
            {
                // Unbind use cases before rebinding
                cameraProvider.UnbindAll();
                // Bind use cases to camera
                _camera = cameraProvider.BindToLifecycle((ILifecycleOwner)Context, cameraSelector, preview, imageAnalyzer);
                HandleTorch();
            }
            catch (Exception exc)
            {
                Log.Debug(nameof(CameraCallback), "Use case binding failed", exc);
            }
        }

        private void HandleTorch()
        {
            if (_camera == null || Element == null || !_camera.CameraInfo.HasFlashUnit) return;
            if (Element.TorchOn && IsTorchOn() || !Element.TorchOn && !IsTorchOn())
                return;
            _camera.CameraControl.EnableTorch(Element.TorchOn);
        }

        private bool IsTorchOn()
        {
            if (_camera == null || !_camera.CameraInfo.HasFlashUnit)
                return false;
            return (int)_camera.CameraInfo.TorchState?.Value == TorchState.On;
        }

        private void DisableTorchIfNeeded()
        {
            if (_camera == null || !_camera.CameraInfo.HasFlashUnit || (int)_camera.CameraInfo.TorchState?.Value != TorchState.On)
                return;
            _camera.CameraControl.EnableTorch(false);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_isDisposed)
                return;

            if (disposing)
            {
                DisableTorchIfNeeded();

                _cameraExecutor?.Shutdown();
                _cameraExecutor?.Dispose();
                _cameraExecutor = null;

                _cameraFuture?.Cancel(true);
                _cameraFuture?.Dispose();
                _cameraFuture = null;
            }

            _isDisposed = true;
        }

        private class TorchStateObserver : Java.Lang.Object, IObserver
        {
            private readonly CameraViewRenderer _renderer;

            public TorchStateObserver(CameraViewRenderer renderer)
            {
                _renderer = renderer;
            }

            public void OnChanged(Java.Lang.Object state) =>
                _renderer.Element.TorchOn = (int)state == TorchState.On;

        }

        public class BarcodeAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
        {
            private readonly IBarcodeScanner _barcodeScanner;
            private readonly CameraViewRenderer _renderer;
            long lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
        

            public BarcodeAnalyzer(CameraViewRenderer renderer)
            {
                _renderer = renderer;
                if(_renderer.Element != null)
                {
                    if (_renderer.Element.ScanInterval < 100)
                        _renderer.Element.ScanInterval = 500;
                }
                
                _barcodeScanner = BarcodeScanning.GetClient(new BarcodeScannerOptions.Builder().SetBarcodeFormats(
                    Configuration.BarcodeFormats)
                .Build());

            }

            public async void Analyze(IImageProxy proxy)
            {

                var mediaImage = proxy.Image;
                if (mediaImage == null) return;
                lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                try
                {
                    Console.WriteLine($"IsScanning : {_renderer.Element.IsScanning}");
                    if (lastRunTime - lastAnalysisTime > _renderer.Element.ScanInterval && _renderer.Element.IsScanning)
                    {
                        lastAnalysisTime = lastRunTime;
                        var image = InputImage.FromMediaImage(mediaImage, proxy.ImageInfo.RotationDegrees);
                        // Pass image to the scanner and have it do its thing
                        var result = await _barcodeScanner.Process(image);
                        var final = Methods.Process(result);
                        if (final != null && _renderer?.Element != null)
                        {
                            _renderer.Element.IsScanning = false;
                            _renderer.Element.TriggerOnDetected(final);
                            if (_renderer.Element.VibrationOnDetected)
                                Xamarin.Essentials.Vibration.Vibrate(200);
                        }
                    }
                  
                }
                catch (Exception ex)
                {
                    //Log somewhere
                }
                finally
                {
                    proxy.Close();
                }
            }
        }
    }
}