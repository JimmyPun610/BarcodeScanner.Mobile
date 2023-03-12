using Android.Content;
using Android.Hardware.Camera2;
using Android.Util;
using AndroidX.Camera.Camera2.InterOp;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
using BarcodeScanner.Mobile.Platforms.Android;
using Google.Common.Util.Concurrent;
using Java.Lang;
using Java.Util.Concurrent;
using Exception = System.Exception;

namespace BarcodeScanner.Mobile
{
    public partial class CameraViewHandler
    {
        private bool _isDisposed;

        private IListenableFuture _cameraFuture;
        private IExecutorService _cameraExecutor;

        private ICamera _camera;

        PreviewView _previewView;

        protected override PreviewView CreatePlatformView()
        {
            _previewView = new PreviewView(Context);
            return _previewView;
        }


        private void Connect()
        {
            _cameraExecutor = Executors.NewSingleThreadExecutor();
            _cameraFuture = ProcessCameraProvider.GetInstance(Context);
            _cameraFuture.AddListener(new Runnable(CameraCallback), ContextCompat.GetMainExecutor(Context));
        }

        private void CameraCallback()
        {
            if (_isDisposed)
                return;

            // Used to bind the lifecycle of cameras to the lifecycle owner
            if (_cameraFuture?.Get() is not ProcessCameraProvider cameraProvider)
                return;

            // Preview
            var previewBuilder = new Preview.Builder();
            var preview = previewBuilder.Build();
            preview.SetSurfaceProvider(_previewView.SurfaceProvider);

            var imageAnalyzerBuilder = new ImageAnalysis.Builder();
            // Frame by frame analyze
            if (this.VirtualView.RequestedFPS.HasValue)
            {
                Camera2Interop.Extender ext = new Camera2Interop.Extender(imageAnalyzerBuilder);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeMode, 0);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeTargetFpsRange, new Android.Util.Range((int)VirtualView.RequestedFPS.Value, (int)VirtualView.RequestedFPS.Value));
            }

            //https://developers.google.com/ml-kit/vision/barcode-scanning/android#input-image-guidelines
            var imageAnalyzer = imageAnalyzerBuilder
                                .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest) //<!-- only one image will be delivered for analysis at a time
                                .SetTargetResolution(TargetResolution())
                                .Build();

            imageAnalyzer.SetAnalyzer(_cameraExecutor, new BarcodeAnalyzer(VirtualView));

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
            if (VirtualView.CameraFacing == CameraFacing.Front)
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
            return VirtualView.CaptureQuality switch
            {
                CaptureQuality.Lowest => new Android.Util.Size(352, 288),
                CaptureQuality.Low => new Android.Util.Size(640, 480),
                CaptureQuality.Medium => new Android.Util.Size(1280, 720),
                CaptureQuality.High => new Android.Util.Size(1920, 1080),
                CaptureQuality.Highest => new Android.Util.Size(3840, 2160),
                _ => throw new ArgumentOutOfRangeException(nameof(CaptureQuality))
            };
        }

        public void HandleTorch()
        {
            if (_camera == null || VirtualView == null || !_camera.CameraInfo.HasFlashUnit) return;
           
            _camera.CameraControl.EnableTorch(VirtualView.TorchOn);
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

        private void HandleCustomPreviewSize(Preview preview)
        {
            if (VirtualView.PreviewWidth.HasValue && VirtualView.PreviewHeight.HasValue)
            {
                var width = VirtualView.PreviewWidth.Value;
                var height = VirtualView.PreviewHeight.Value;
                preview.UpdateSuggestedResolution(new Android.Util.Size(width, height));
            }
        }

        private void Dispose()
        {
            if (_isDisposed)
                return;

            DisableTorchIfNeeded();

            _cameraExecutor?.Shutdown();
            _cameraExecutor?.Dispose();
            _cameraExecutor = null;

            ClearCameraProvider();

            _cameraFuture?.Cancel(true);
            _cameraFuture?.Dispose();
            _cameraFuture = null;

            _isDisposed = true;
        }
     
        private void ClearCameraProvider()
        {
            try
            {
                // Used to bind the lifecycle of cameras to the lifecycle owner
                if (_cameraFuture?.Get() is not ProcessCameraProvider cameraProvider)
                    return;

                cameraProvider.UnbindAll();
                cameraProvider.Dispose();
            }
            catch (Exception ex)
            {
                Log.Debug($"{nameof(CameraViewHandler)}-{nameof(ClearCameraProvider)}", ex.ToString());
            }
        }


    }
}
