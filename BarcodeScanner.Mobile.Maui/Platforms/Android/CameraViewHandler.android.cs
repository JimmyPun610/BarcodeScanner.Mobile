using Android.Content;
using Android.Hardware.Camera2;
using Android.Util;
using AndroidX.Camera.Camera2.InterOp;
using AndroidX.Camera.Core;
using AndroidX.Camera.Core.ResolutionSelector;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using AndroidX.Lifecycle;
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

            var selector = new ResolutionSelector.Builder()
                .SetResolutionStrategy(VirtualView.CaptureQuality.GetTargetResolutionStrategy())
                .Build();
            // Preview
            var previewBuilder = new Preview.Builder()
                .SetResolutionSelector(selector);
            var preview = previewBuilder.Build();
            preview.SetSurfaceProvider(_previewView.SurfaceProvider);

            var imageAnalyzerBuilder = new ImageAnalysis.Builder();
            // Frame by frame analyze
            if (VirtualView.RequestedFPS.HasValue)
            {
                Camera2Interop.Extender ext = new Camera2Interop.Extender(imageAnalyzerBuilder);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeMode, 0);
                ext.SetCaptureRequestOption(CaptureRequest.ControlAeTargetFpsRange, new Android.Util.Range((int)VirtualView.RequestedFPS.Value, (int)VirtualView.RequestedFPS.Value));
            }

            //https://developers.google.com/ml-kit/vision/barcode-scanning/android#input-image-guidelines
            var imageAnalyzer = imageAnalyzerBuilder
                                .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest) //<!-- only one image will be delivered for analysis at a time
                                .SetResolutionSelector(selector)
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

                HandleTorch();
                HandleAutoFocus();
                HandleZoom();
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

        /// <summary>
        /// Logic from https://stackoverflow.com/a/66659592/9032777
        /// Focus every 3s
        /// </summary>
        private async void HandleAutoFocus()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(3000);

                    if (_camera == null || _previewView == null)
                    {
                        continue;
                    }

                    float x = _previewView.GetX() + _previewView.Width / 2f;
                    float y = _previewView.GetY() + _previewView.Height / 2f;

                    MeteringPointFactory pointFactory = _previewView.MeteringPointFactory;
                    float afPointWidth = 1.0f / 6.0f;  // 1/6 total area
                    float aePointWidth = afPointWidth * 1.5f;
                    MeteringPoint afPoint = pointFactory.CreatePoint(x, y, afPointWidth);
                    MeteringPoint aePoint = pointFactory.CreatePoint(x, y, aePointWidth);

                    _camera.CameraControl.StartFocusAndMetering(
                new FocusMeteringAction.Builder(afPoint,
                        FocusMeteringAction.FlagAf).AddPoint(aePoint,
                        FocusMeteringAction.FlagAe).Build());
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void HandleTorch()
        {
            if (_camera == null || !_camera.CameraInfo.HasFlashUnit) return;

            _camera.CameraControl.EnableTorch(VirtualView.TorchOn);
        }

        private void HandleZoom()
        {
            if (_camera == null)
                return;

            _camera.CameraControl.SetLinearZoom(VirtualView.Zoom);
        }

        private void DisableTorchIfNeeded()
        {
            if (_camera == null || !_camera.CameraInfo.HasFlashUnit || (int)_camera.CameraInfo.TorchState?.Value != TorchState.On)
                return;
            _camera.CameraControl.EnableTorch(false);
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
