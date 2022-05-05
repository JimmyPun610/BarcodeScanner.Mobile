﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Tasks;
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
            _cameraFuture   = ProcessCameraProvider.GetInstance(context);
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
            {
                HandleTorch();
            }
            else if (e.PropertyName == CameraView.CameraFacingProperty.PropertyName)
            {
                CameraCallback();
            }
            else if (e.PropertyName == CameraView.CaptureQualityProperty.PropertyName)
            {
                CameraCallback();
            }
        }

        protected override PreviewView CreateNativeControl() => new PreviewView(Context);

        private void CameraCallback()
        {
            if (_isDisposed)
                return;

            // Used to bind the lifecycle of cameras to the lifecycle owner
            if (!(_cameraFuture.Get() is ProcessCameraProvider cameraProvider))
                return;

            // Preview
            var previewBuilder = new Preview.Builder();
            var preview = previewBuilder.Build();
            preview.SetSurfaceProvider(Control.SurfaceProvider);

            // Frame by frame analyze
            var imageAnalyzerBuilder = new ImageAnalysis.Builder();
            if (Element.RequestedFPS.HasValue)
            {
                Camera2Interop.Extender ext = new Camera2Interop.Extender(imageAnalyzerBuilder);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeMode, 0);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeTargetFpsRange, new Android.Util.Range((int)Element.RequestedFPS.Value, (int)Element.RequestedFPS.Value));
            }

            //https://developers.google.com/ml-kit/vision/barcode-scanning/android#input-image-guidelines
            var imageAnalyzer = imageAnalyzerBuilder
                                .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest) //<!-- only one image will be delivered for analysis at a time
                                .SetTargetResolution(TargetResolution())
                                .Build();

            imageAnalyzer.SetAnalyzer(_cameraExecutor, new BarcodeAnalyzer(this));

            var cameraSelector = SelectCamera(cameraProvider);

            try
            {
                // Unbind use cases before rebinding
                cameraProvider.UnbindAll();

                // Searching for lifecycle owner
                // There can be context wrapper instead of context it self, so we have to check it.
                var lifecycleOwner = Context as ILifecycleOwner ?? (Context as ContextWrapper)?.BaseContext as ILifecycleOwner;

                if (lifecycleOwner == null)
                    throw new Exception("Unable to find lifecycle owner");
                
                // Bind use cases to camera
                _camera = cameraProvider.BindToLifecycle(lifecycleOwner, cameraSelector, preview, imageAnalyzer);

                HandleCustomPreviewSize(preview);
                HandleTorch();
            }
            catch (Exception exc)
            {
                Log.Debug(nameof(CameraCallback), "Use case binding failed", exc);
            }
        }

        private CameraSelector SelectCamera(ProcessCameraProvider cameraProvider)
        {
	        if (Element.CameraFacing == CameraFacing.Front)
	        {
		        if (cameraProvider.HasCamera(CameraSelector.DefaultFrontCamera))
			        return CameraSelector.DefaultFrontCamera;

                throw new NotSupportedException("Front camera is not supported in this device");
            }

	        if (cameraProvider.HasCamera(CameraSelector.DefaultBackCamera))
		        return CameraSelector.DefaultBackCamera;

            throw new NotSupportedException("Back camera is not supported in this device");
        }

        private Android.Util.Size TargetResolution()
        {
            return Element.CaptureQuality switch
            {
                CaptureQuality.Lowest => new Android.Util.Size(352, 288),
                CaptureQuality.Low => new Android.Util.Size(640, 480),
                CaptureQuality.Medium => new Android.Util.Size(1280, 720),
                CaptureQuality.High => new Android.Util.Size(1920, 1080),
                CaptureQuality.Highest => new Android.Util.Size(3840, 2160),
                _ => throw new ArgumentOutOfRangeException(nameof(CaptureQuality))
            };
        }

        private void HandleCustomPreviewSize(Preview preview)
        {
            if (Element.PreviewWidth.HasValue && Element.PreviewHeight.HasValue)
            {
                var width = Element.PreviewWidth.Value;
                var height = Element.PreviewHeight.Value;
                preview.UpdateSuggestedResolution(new Android.Util.Size(width, height));
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

                ClearCameraProvider();

                _cameraFuture?.Cancel(true);
                _cameraFuture?.Dispose();
                _cameraFuture = null;
            }

            _isDisposed = true;
        }


        private void ClearCameraProvider()
        {
            try
            {
                // Used to bind the lifecycle of cameras to the lifecycle owner
                if (!(_cameraFuture.Get() is ProcessCameraProvider cameraProvider))
                    return;

                cameraProvider.UnbindAll();
                cameraProvider.Dispose();
            }
            catch (Exception ex)
            {
                Log.Debug($"{nameof(CameraViewRenderer)}-{nameof(ClearCameraProvider)}", ex.ToString());
            }
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
            private long _lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            private long _lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();


            public BarcodeAnalyzer(CameraViewRenderer renderer)
            {
                _renderer = renderer;
                if (_renderer.Element != null && _renderer.Element.ScanInterval < 100)
                    _renderer.Element.ScanInterval = 500;

                _barcodeScanner = BarcodeScanning.GetClient(new BarcodeScannerOptions.Builder().SetBarcodeFormats(
                    Configuration.BarcodeFormats)
                .Build());

            }

            public async void Analyze(IImageProxy proxy)
            {
                try
                {
                    var mediaImage = proxy.Image;
                    if (mediaImage == null) return;
                    _lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    
                    if (_lastRunTime - _lastAnalysisTime > _renderer.Element.ScanInterval && _renderer.Element.IsScanning)
                    {
                        _lastAnalysisTime = _lastRunTime;
                        var image = InputImage.FromMediaImage(mediaImage, proxy.ImageInfo.RotationDegrees);
                        // Pass image to the scanner and have it do its thing
                        var result = await ToAwaitableTask(_barcodeScanner.Process(image));
                        var final = Methods.Process(result);
                        if (final == null || _renderer?.Element == null) return;
                        if (!_renderer.Element.IsScanning)
                            return;

                        _renderer.Element.IsScanning = false;
                        _renderer.Element.TriggerOnDetected(final);
                        if (_renderer.Element.VibrationOnDetected)
                            Xamarin.Essentials.Vibration.Vibrate(200);
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug(nameof(CameraViewRenderer), ex.ToString());
                }
                catch (System.Exception ex)
                {
                    Log.Debug(nameof(CameraViewRenderer), ex.ToString());
                }
                finally
                {
                    SafeCloseImageProxy(proxy);
                }
            }

            private void SafeCloseImageProxy(IImageProxy proxy)
            {
                try
                {
                    proxy?.Close();
                }
                catch (ObjectDisposedException) { }
                catch (ArgumentException)
                {
                    //Ignore argument exception, it will be thrown if BarcodeAnalyzer get disposed during processing
                }
            }
        }

        private static Task<Java.Lang.Object> ToAwaitableTask(Android.Gms.Tasks.Task task)
        {
            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
            var taskCompleteListener = new TaskCompleteListener(taskCompletionSource);
            task.AddOnCompleteListener(taskCompleteListener);

            return taskCompletionSource.Task;
        }
    }

    class TaskCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        private readonly TaskCompletionSource<Java.Lang.Object> _taskCompletionSource;

        public TaskCompleteListener(TaskCompletionSource<Java.Lang.Object> tcs)
        {
            _taskCompletionSource = tcs;
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            if (task.IsCanceled)
            {
                _taskCompletionSource.SetCanceled();
            }
            else if (task.IsSuccessful)
            {
                _taskCompletionSource.SetResult(task.Result);
            }
            else
            {
                _taskCompletionSource.SetException(task.Exception);
            }
        }
    }
}
