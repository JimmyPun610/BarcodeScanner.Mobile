using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using MLKit.BarcodeScanning;
using Foundation;
using AudioToolbox;
using UIKit;
using System.Drawing;
using System.Threading;
using GoogleVisionBarCodeScanner.Renderer;
using MLKit.Core;

namespace GoogleVisionBarCodeScanner
{
    internal sealed class UICameraPreview : UIView
    {
        public event EventHandler<List<BarcodeResult>> OnDetected;
        public event EventHandler IsScanningChanged;
        AVCaptureVideoPreviewLayer previewLayer;
        CaptureVideoDelegate captureVideoDelegate;
        public AVCaptureSession  CaptureSession  { get; private set; }
        AVCaptureVideoDataOutput VideoDataOutput { get; set; }
        readonly CameraFacing _cameraFacing;
        readonly CaptureQuality _captureQuality;

        public UICameraPreview(CameraViewRenderer renderer, CameraFacing cameraFacing, CaptureQuality captureQuality)
        {
            _cameraFacing   = cameraFacing;
            _captureQuality = captureQuality;
            Initialize(renderer);
        }
        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            //Off the torch when exit page
            if (IsTorchOn())
                ToggleFlashlight();
            //Stop the capture session if not null
            try
            {
                if (CaptureSession != null)
                    CaptureSession.StopRunning();
            }
            catch
            {

            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            setPreviewOrientation();
        }

        internal void ChangeCamera(CameraFacing facing)
        {
            var input = CaptureSession.Inputs.FirstOrDefault();
            if (input != null)
            {
                CaptureSession.BeginConfiguration();
                CaptureSession.RemoveInput(input);
                AddInputToCameraSession(facing);
                CaptureSession.CommitConfiguration();
            }
        }

        internal void ChangeSessionPreset(CaptureQuality quality)
        {
            var input = CaptureSession.Inputs.FirstOrDefault();
            if (input != null)
            {
                CaptureSession.BeginConfiguration();
                CaptureSession.RemoveInput(input);
                AddSessionPreset(quality);
                CaptureSession.CommitConfiguration();
            }
        }

        private void updatePreviewLayer(AVCaptureConnection layer, AVCaptureVideoOrientation orientation)
        {
            layer.VideoOrientation = orientation;
            previewLayer.Frame = Bounds;
        }
        private void setPreviewOrientation()
        {
            var connection = previewLayer.Connection;
            if (connection != null)
            {
                var currentDevice = UIDevice.CurrentDevice;
                if (currentDevice.CheckSystemVersion(13, 0))
                {
                    UIInterfaceOrientation orientation = UIApplication.SharedApplication.Windows.FirstOrDefault()?.WindowScene?.InterfaceOrientation ?? UIInterfaceOrientation.Portrait;

                    var previewLayerConnection = connection;
                    if (previewLayerConnection.SupportsVideoOrientation)
                    {
                        switch (orientation)
                        {
                            case UIInterfaceOrientation.Portrait:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                            case UIInterfaceOrientation.LandscapeLeft:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeLeft);
                                break;
                            case UIInterfaceOrientation.LandscapeRight:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeRight);
                                break;
                            case UIInterfaceOrientation.PortraitUpsideDown:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.PortraitUpsideDown);
                                break;
                            default:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                        }
                    }
                }
                else
                {
                    // Apporach on iOS 12 or below, but this will have wrong value, please read issue #55 for more information
                    // https://github.com/JimmyPun610/BarcodeScanner.XF/issues/55
                    var orientation = currentDevice.Orientation;
                    var previewLayerConnection = connection;
                    if (previewLayerConnection.SupportsVideoOrientation)
                    {
                        switch (orientation)
                        {
                            case UIDeviceOrientation.Portrait:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                            case UIDeviceOrientation.LandscapeLeft:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeLeft);
                                break;
                            case UIDeviceOrientation.LandscapeRight:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeRight);
                                break;
                            case UIDeviceOrientation.PortraitUpsideDown:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.PortraitUpsideDown);
                                break;
                            default:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                        }
                    }
                }

            }
        }

        void Initialize(CameraViewRenderer renderer)
        {
            CaptureSession = new AVCaptureSession();
            CaptureSession.BeginConfiguration();
            this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
            {
                Frame = this.Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill
            };

            Layer.AddSublayer(previewLayer);

            AddSessionPreset(_captureQuality);
            AddInputToCameraSession(_cameraFacing);

            CaptureSession.CommitConfiguration();


            VideoDataOutput = new AVCaptureVideoDataOutput
            {
                AlwaysDiscardsLateVideoFrames = true,
                WeakVideoSettings = new CVPixelBufferAttributes { PixelFormatType = CVPixelFormatType.CV32BGRA }
                    .Dictionary
            };


            captureVideoDelegate = new CaptureVideoDelegate(renderer);
            captureVideoDelegate.OnDetected += (list) =>
            {
                InvokeOnMainThread(() =>
                {
                    //CaptureSession.StopRunning();
                    this.OnDetected?.Invoke(this, list);
                });

            };
            VideoDataOutput.SetSampleBufferDelegateQueue(captureVideoDelegate, CoreFoundation.DispatchQueue.MainQueue);

            CaptureSession.AddOutput(VideoDataOutput);
            InvokeOnMainThread(() =>
            {
                CaptureSession.StartRunning();
                if(renderer.Element != null)
                {
                    if (renderer.Element.TorchOn && !IsTorchOn())
                        ToggleFlashlight();
                }
            });
        }

        void AddInputToCameraSession(CameraFacing facing)
        {
            var videoDevices   = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var cameraPosition = (facing == CameraFacing.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
            var device         = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);

            if (device == null)
                throw new NotSupportedException("The selected camera is not supported on this device");

            NSError error;
            var     input = new AVCaptureDeviceInput(device, out error);

            CaptureSession.AddInput(input);
        }

        void AddSessionPreset(CaptureQuality captureQuality)
        {
            CaptureSession.SessionPreset = GetSessionPreset(captureQuality);
        }

        NSString GetSessionPreset(CaptureQuality captureQuality)
        {
            return captureQuality switch
            {
                CaptureQuality.Lowest => AVCaptureSession.Preset352x288,
                CaptureQuality.Low => AVCaptureSession.Preset640x480,
                CaptureQuality.Medium => AVCaptureSession.Preset1280x720,
                CaptureQuality.High => AVCaptureSession.Preset1920x1080,
                CaptureQuality.Highest => AVCaptureSession.Preset3840x2160,
                _ => throw new ArgumentOutOfRangeException(nameof(_captureQuality))
            };
        }


        public bool IsTorchOn()
        {
            try
            {
                var videoDevices = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
                if (videoDevices != null && videoDevices.HasTorch)
                    return videoDevices.TorchMode == AVCaptureTorchMode.On;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"iOS IsTorchOn error : {ex.Message}, StackTrace : {ex.StackTrace}");
            }
            

            return false;
        }
        public void ToggleFlashlight()
        {
            var videoDevices = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (videoDevices == null || !videoDevices.HasTorch) return;

            NSError error;
            videoDevices.LockForConfiguration(out error);
            if (error == null)
            {
                if (videoDevices.TorchMode == AVCaptureTorchMode.On)
                    videoDevices.TorchMode = AVCaptureTorchMode.Off;
                else
                {
                    videoDevices.SetTorchModeLevel(1.0f, out error);
                }
            }
            videoDevices.UnlockForConfiguration();

        }

        public class CaptureVideoDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
        {
            public event Action<List<BarcodeResult>> OnDetected;
            BarcodeScanner barcodeDetector;
            UIImageOrientation orientation = UIImageOrientation.Up;
            long lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
            long lastRunTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
            CameraViewRenderer _renderer;
            public CaptureVideoDelegate(CameraViewRenderer renderer)
            {
                _renderer = renderer;
           
                if (_renderer.Element != null)
                {
                    if (_renderer.Element.ScanInterval < 100)
                        _renderer.Element.ScanInterval = 500;
                }
                var options = new BarcodeScannerOptions(Configuration.BarcodeDetectorSupportFormat);
                barcodeDetector = BarcodeScanner.BarcodeScannerWithOptions(options);
                // Using back-facing camera
                var devicePosition = AVCaptureDevicePosition.Back;
                var deviceOrientation = UIDevice.CurrentDevice.Orientation;
                switch (deviceOrientation)
                {
                    case UIDeviceOrientation.Portrait:
                        
                        orientation = devicePosition == AVCaptureDevicePosition.Front ? UIImageOrientation.LeftMirrored : UIImageOrientation.Right;
                        break;
                    case UIDeviceOrientation.LandscapeLeft:
                        orientation = devicePosition == AVCaptureDevicePosition.Front ? UIImageOrientation.DownMirrored : UIImageOrientation.Up;
                        break;
                    case UIDeviceOrientation.PortraitUpsideDown:
                        orientation = devicePosition == AVCaptureDevicePosition.Front ? UIImageOrientation.RightMirrored : UIImageOrientation.Left;
                        break;
                    case UIDeviceOrientation.LandscapeRight:
                        orientation = devicePosition == AVCaptureDevicePosition.Front ? UIImageOrientation.UpMirrored : UIImageOrientation.Down;
                        break;
                    case UIDeviceOrientation.FaceUp:
                    case UIDeviceOrientation.FaceDown:
                    case UIDeviceOrientation.Unknown:
                        orientation = UIImageOrientation.Up;
                        break;
                }
            }


            private static UIImage GetImageFromSampleBuffer(CMSampleBuffer sampleBuffer)
            {
                // Get a pixel buffer from the sample buffer
                using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
                {
                    // Lock the base address
                    if (pixelBuffer != null)
                    {
                        pixelBuffer.Lock(CVPixelBufferLock.None);

                        // Prepare to decode buffer
                        var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

                        // Decode buffer - Create a new colorspace
                        using (var cs = CGColorSpace.CreateDeviceRGB())
                        {
                            // Create new context from buffer
                            using (var context = new CGBitmapContext(pixelBuffer.BaseAddress,
                                pixelBuffer.Width,
                                pixelBuffer.Height,
                                8,
                                pixelBuffer.BytesPerRow,
                                cs,
                                (CGImageAlphaInfo)flags))
                            {
                                // Get the image from the context
                                using (var cgImage = context.ToImage())
                                {
                                    // Unlock and return image
                                    pixelBuffer.Unlock(CVPixelBufferLock.None);
                                    return UIImage.FromImage(cgImage);
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            private void releaseSampleBuffer(CMSampleBuffer sampleBuffer)
            {
                if (sampleBuffer != null)
                {
                    sampleBuffer.Dispose();
                    sampleBuffer = null;
                }
            }
            public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
            {
                lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (lastRunTime - lastAnalysisTime > _renderer.Element.ScanInterval && _renderer.Element.IsScanning)
                {
                    lastAnalysisTime = lastRunTime;
                    try
                    {
                        var image = GetImageFromSampleBuffer(sampleBuffer);
                        if (image == null) return;
                        var visionImage = new MLImage(image) { Orientation = orientation };
                        releaseSampleBuffer(sampleBuffer);
                        barcodeDetector.ProcessImage(visionImage, (barcodes, error) =>
                        {
                            if (_renderer.Element == null) return;
                            if (!_renderer.Element.IsScanning) return;

                            if(error != null)
                            {
                                System.Diagnostics.Debug.WriteLine(error);
                                return;
                            }

                            if (barcodes == null || barcodes.Length == 0)
                            {
                                return;
                            }

                            _renderer.Element.IsScanning = false;

                            if (_renderer.Element.VibrationOnDetected)
                                SystemSound.Vibrate.PlayAlertSound();

                            List<BarcodeResult> resultList = new List<BarcodeResult>();
                            foreach (var barcode in barcodes)
                                resultList.Add(Methods.MapBarcodeResult(barcode));

                            OnDetected?.Invoke(resultList);
                        });
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(exception.Message);
                    }
                }
                releaseSampleBuffer(sampleBuffer);
            }
        }

    }
}

