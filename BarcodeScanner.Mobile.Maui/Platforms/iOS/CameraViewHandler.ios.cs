using AVFoundation;
using BarcodeScanner.Mobile.Platforms.iOS;
using CoreVideo;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

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

                if(VideoDataOutput != null)
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
                CaptureQuality.Medium => AVCaptureSession.Preset1280x720,
                CaptureQuality.High => AVCaptureSession.Preset1920x1080,
                CaptureQuality.Highest => AVCaptureSession.Preset3840x2160,
                _ => throw new ArgumentOutOfRangeException(nameof(VirtualView.CaptureQuality))
            };
        }
        public void SetFocusMode(AVCaptureFocusMode focusMode = AVCaptureFocusMode.ContinuousAutoFocus)
        {
            Microsoft.Maui.Controls.Application.Current.Dispatcher.Dispatch(() => {
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

                var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaTypes.Video.GetConstant());
                foreach (var device in devices)
                {
                    if (VirtualView.CameraFacing == CameraFacing.Front &&
                        device.Position == AVCaptureDevicePosition.Front)
                    {
                        CaptureDevice = device;
                        break;
                    }
                    else if (VirtualView.CameraFacing == CameraFacing.Back && device.Position == AVCaptureDevicePosition.Back)
                    {
                        CaptureDevice = device;
                        break;
                    }
                }

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
       
    }
}
