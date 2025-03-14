using AVFoundation;
using BarcodeScanner.Mobile.Platforms.iOS;
using CoreVideo;
using Foundation;

namespace BarcodeScanner.Mobile
{
    public partial class CameraViewHandler
    {
        public event EventHandler IsScanningChanged;

        AVCaptureVideoPreviewLayer VideoPreviewLayer;
        AVCaptureDevice CaptureDevice;
        AVCaptureInput CaptureInput = null;
        CaptureVideoDelegate CaptureVideoDelegate;
        public AVCaptureSession CaptureSession { get; private set; }
        AVCaptureVideoDataOutput VideoDataOutput { get; set; }

        bool isUpdatingTorch { get; set; }

        UICameraPreview _uiCameraPerview;
        protected override UICameraPreview CreatePlatformView()
        {
            CaptureSession = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.Preset640x480
            };

            VideoPreviewLayer = new AVCaptureVideoPreviewLayer(CaptureSession);
            VideoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;

            _uiCameraPerview = new UICameraPreview(VideoPreviewLayer);
            return _uiCameraPerview;
        }

        public void Connect()
        {
            if (DeviceInfo.Current.DeviceType == DeviceType.Virtual)
                return;

            ChangeCameraFacing();
            ChangeCameraQuality();

            if (VideoDataOutput == null)
            {
                VideoDataOutput = new AVCaptureVideoDataOutput
                {
                    AlwaysDiscardsLateVideoFrames = true,
                    WeakVideoSettings = new CVPixelBufferAttributes { PixelFormatType = CVPixelFormatType.CV32BGRA }
                 .Dictionary
                };

                if (CaptureVideoDelegate == null)
                {
                    CaptureVideoDelegate = new CaptureVideoDelegate(VirtualView);
                    CaptureVideoDelegate.OnDetected += (eventArg) =>
                    {
                        PlatformView.InvokeOnMainThread(() =>
                        {
                            //CaptureSession.StopRunning();
                            this.VirtualView?.TriggerOnDetected(eventArg.OCRResult, eventArg.BarcodeResults, eventArg.ImageData);
                        });
                    };
                }
                VideoDataOutput.AlwaysDiscardsLateVideoFrames = true;
                VideoDataOutput.SetSampleBufferDelegate(CaptureVideoDelegate, CoreFoundation.DispatchQueue.MainQueue);
            }

            CaptureSession.AddOutput(VideoDataOutput);

            CaptureSession.StartRunning();
            HandleTorch();
            SetFocusMode();
            HandleZoom();
        }

        public void Dispose()
        {
            //Stop the capture session if not null
            try
            {
                if (CaptureDevice != null)
                {
                    CaptureDevice.Dispose();
                    CaptureDevice = null;
                }

                if (CaptureInput != null)
                {
                    CaptureInput.Dispose();
                    CaptureInput = null;
                }

                if (CaptureSession != null)
                {
                    CaptureSession.RemoveOutput(VideoDataOutput);
                    CaptureSession.StopRunning();
                }

                if (VideoDataOutput != null)
                {
                    VideoDataOutput.Dispose();
                    VideoDataOutput = null;
                }
            }
            catch
            {

            }
        }

        NSString GetCaptureSessionResolution(CaptureQuality captureQuality)
        {
            return captureQuality switch
            {
                CaptureQuality.Lowest => AVCaptureSession.Preset352x288,
                CaptureQuality.Low => AVCaptureSession.Preset640x480,
                CaptureQuality.Default => AVCaptureSession.Preset1280x720,
                CaptureQuality.Medium => AVCaptureSession.Preset1280x720,
                CaptureQuality.High => AVCaptureSession.Preset1920x1080,
                CaptureQuality.Highest => AVCaptureSession.Preset3840x2160,
                _ => throw new ArgumentOutOfRangeException(nameof(VirtualView.CaptureQuality))
            };
        }
        public void SetFocusMode(AVCaptureFocusMode focusMode = AVCaptureFocusMode.ContinuousAutoFocus)
        {
            Microsoft.Maui.Controls.Application.Current.Dispatcher.Dispatch(() =>
            {
                var videoDevice = AVCaptureDevice.GetDefaultDevice(AVFoundation.AVMediaTypes.Video);
                if (videoDevice == null) return;

                NSError error;
                videoDevice.LockForConfiguration(out error);
                if (error == null)
                {
                    videoDevice.FocusMode = focusMode;
                }
                videoDevice.UnlockForConfiguration();
            });
        }

        public bool IsTorchOn()
        {
            try
            {
                if (CaptureDevice == null || !CaptureDevice.HasTorch || !CaptureDevice.TorchAvailable)
                    return false;
                else return CaptureDevice.TorchMode == AVCaptureTorchMode.On;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"iOS IsTorchOn error : {ex.Message}, StackTrace : {ex.StackTrace}");
            }
            return false;
        }
        public void HandleTorch()
        {
            Microsoft.Maui.Controls.Application.Current.Dispatcher.Dispatch(() =>
            {
                if (isUpdatingTorch)
                    return;

                if (CaptureDevice == null || !CaptureDevice.HasTorch || !CaptureDevice.TorchAvailable)
                    return;

                isUpdatingTorch = true;

                NSError error;
                CaptureDevice.LockForConfiguration(out error);
                if (error == null)
                {
                    if (VirtualView.TorchOn == true)
                    {
                        CaptureDevice.SetTorchModeLevel(1.0f, out error);
                    }
                    else
                    {
                        CaptureDevice.TorchMode = AVCaptureTorchMode.Off;
                    }
                }
                CaptureDevice.UnlockForConfiguration();

                isUpdatingTorch = false;
            });
        }

        private void HandleZoom()
        {
            Application.Current.Dispatcher.Dispatch(() =>
            {
                if (CaptureDevice == null)
                    return;

                if (CaptureDevice.LockForConfiguration(out NSError error))
                {
                    // Set the desired zoom factor
                    // We need to translate the zoomRequest because the Android implementation uses the SetLinearZoom which accepts values in range of 0.0 - 1.0.
                    // We should consider to change or implement an alternative zoom option for consistent zoom value setting in both Android and iOS platforms
                    // in the form of scaling by using in the method SetZoomRatio() in Android which accepts the same values as VideoZoomFactor here
                    CaptureDevice.VideoZoomFactor = TranslateZoom(VirtualView.Zoom);

                    // Apply the configuration
                    CaptureDevice.UnlockForConfiguration();
                }
            });
        }

        /// <summary>
        /// Translates a zoom request of the range 0.0 - 1.0 to iOS VideoZoomFactor acceptable value of scale x1 - x4.
        /// </summary>
        /// <param name="zoomRequest"></param>
        /// <param name="maxZoomFactor"></param>
        /// <returns></returns>
        private float TranslateZoom(float zoomRequest)
        {
            float zoom = 1F;

            // For values 0.00 - 0.49 apply zoom of x1 - x2 for better experience
            if (zoomRequest < 0.5F)
                zoom = (zoomRequest + 0.5F) * 2F;

            // For values of 0.50 - 1.00 apply zoom of x2 - x4
            if (zoomRequest >= 0.5F)
                zoom = zoomRequest * 4;

            return zoom;
        }

        public void ChangeCameraFacing()
        {

            if (CaptureSession != null)
            {
                CaptureSession.BeginConfiguration();

                // Clean old input
                if (CaptureInput != null && CaptureSession.Inputs.Contains(CaptureInput))
                {
                    CaptureSession.RemoveInput(CaptureInput);
                    CaptureInput.Dispose();
                    CaptureInput = null;
                }

                // Clean old device
                if (CaptureDevice != null)
                {
                    CaptureDevice.Dispose();
                    CaptureDevice = null;
                }

                var position = VirtualView.CameraFacing == CameraFacing.Front ?
                    AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
                CaptureDevice = GetCamera(position);


                if (CaptureDevice == null)
                {
                    throw new NotSupportedException("The selected camera is not supported on this device");
                }

                CaptureInput = new AVCaptureDeviceInput(CaptureDevice, out var err);
                CaptureSession.AddInput(CaptureInput);

                CaptureSession.CommitConfiguration();
            }
        }

        public void ChangeCameraQuality()
        {

            var input = CaptureSession.Inputs.FirstOrDefault();
            if (input != null && CaptureSession != null)
            {
                CaptureSession.BeginConfiguration();

                CaptureSession.SessionPreset = GetCaptureSessionResolution(VirtualView.CaptureQuality);

                CaptureSession.CommitConfiguration();
            }

        }

        private AVCaptureDevice GetCamera(AVCaptureDevicePosition position)
        {
            if (AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInTripleCamera, AVMediaTypes.Video, position) != null)
            {
                return AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInTripleCamera, AVMediaTypes.Video, position);
            }

            if (AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualCamera, AVMediaTypes.Video, position) != null)
            {
                return AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInDualCamera, AVMediaTypes.Video, position);
            }

            using var session = AVCaptureDeviceDiscoverySession.Create(
                                    new[] {
                                        AVCaptureDeviceType.BuiltInDualCamera,
                                        AVCaptureDeviceType.BuiltInTripleCamera,
                                        AVCaptureDeviceType.BuiltInTrueDepthCamera,
                                        AVCaptureDeviceType.BuiltInDualWideCamera,
                                        AVCaptureDeviceType.BuiltInWideAngleCamera,
                                        AVCaptureDeviceType.BuiltInUltraWideCamera,
                                        AVCaptureDeviceType.BuiltInTelephotoCamera
                                    },
                                    AVMediaTypes.Video,
                                    position);

            return session.Devices.FirstOrDefault();
        }
    }
}