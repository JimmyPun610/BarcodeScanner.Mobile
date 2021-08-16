using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreImage;
using CoreMedia;
using CoreVideo;
using Firebase.MLKit.Vision;
using Foundation;
using AudioToolbox;
using UIKit;
using System.Drawing;
using System.Threading;


namespace GoogleVisionBarCodeScanner
{
    internal sealed class UICameraPreview : UIView
    {
        public event EventHandler<List<BarcodeResult>> OnDetected;
        public event EventHandler IsScanningChanged;
        AVCaptureVideoPreviewLayer previewLayer;
        CaptureVideoDelegate captureVideoDelegate;
        //CameraOptions cameraOptions;
        public AVCaptureSession CaptureSession { get; private set; }

        public bool IsScanning { get; private set; } = true;

        public void SetIsScanning(bool isScanning)
        {
            bool shouldChange = isScanning != IsScanning;
            IsScanning = isScanning;
            if (shouldChange)
                IsScanningChanged?.Invoke(this, EventArgs.Empty);
        }

        AVCaptureVideoDataOutput VideoDataOutput { get; set; }
        //public UICameraPreview(CameraOptions options)
        //{
        //    cameraOptions = options;
        //    IsPreviewing = false;
        //    Initialize();
        //}

        public UICameraPreview(bool defaultTorchOn, bool vibrationOnDetected, bool startScanningOnCreate, int scanInterval)
        {
            //cameraOptions = options;
            Initialize(defaultTorchOn, vibrationOnDetected, startScanningOnCreate, scanInterval);
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
        void Initialize(bool defaultTorchOn, bool vibrationOnDetected, bool startScanningOnCreate, int scanInterval)
        {
            IsScanning = startScanningOnCreate;
            CaptureSession = new AVCaptureSession();
            CaptureSession.BeginConfiguration();
            this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
            {
                Frame = this.Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill
            };
            var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var cameraPosition = AVCaptureDevicePosition.Back;
            //var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
            var device = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);


            if (device == null)
                return;

            NSError error;
            var input = new AVCaptureDeviceInput(device, out error);

            CaptureSession.AddInput(input);
            CaptureSession.SessionPreset = AVFoundation.AVCaptureSession.Preset1280x720;
            Layer.AddSublayer(previewLayer);

            CaptureSession.CommitConfiguration();



            VideoDataOutput = new AVCaptureVideoDataOutput
            {
                AlwaysDiscardsLateVideoFrames = true,
                WeakVideoSettings = new CVPixelBufferAttributes { PixelFormatType = CVPixelFormatType.CV32BGRA }
                    .Dictionary
            };


            captureVideoDelegate = new CaptureVideoDelegate(vibrationOnDetected, scanInterval, this);
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
                //Torch on by default
                if (defaultTorchOn && !IsTorchOn())
                    ToggleFlashlight();
            });


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
            VisionBarcodeDetector barcodeDetector;
            VisionImageMetadata metadata;
            VisionApi vision;
            bool _vibrationOnDetected = true;
            int scanIntervalInMs = 500;
            long lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
            long lastRunTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
            private UICameraPreview _cameraPreview;

            public CaptureVideoDelegate(bool vibrationOnDetected, int scanInterval, UICameraPreview cameraPreview)
            {
                _vibrationOnDetected = vibrationOnDetected;
                _cameraPreview = cameraPreview;
                metadata = new VisionImageMetadata();
                vision = VisionApi.Create();
                if (scanInterval < 100)
                    scanIntervalInMs = 500;
                else scanIntervalInMs = scanInterval;
                barcodeDetector = vision.GetBarcodeDetector(Configuration.BarcodeDetectorSupportFormat);
                // Using back-facing camera
                var devicePosition = AVCaptureDevicePosition.Back;
                var deviceOrientation = UIDevice.CurrentDevice.Orientation;
                switch (deviceOrientation)
                {
                    case UIDeviceOrientation.Portrait:
                        metadata.Orientation = devicePosition == AVCaptureDevicePosition.Front ? VisionDetectorImageOrientation.LeftTop : VisionDetectorImageOrientation.RightTop;
                        break;
                    case UIDeviceOrientation.LandscapeLeft:
                        metadata.Orientation = devicePosition == AVCaptureDevicePosition.Front ? VisionDetectorImageOrientation.BottomLeft : VisionDetectorImageOrientation.TopLeft;
                        break;
                    case UIDeviceOrientation.PortraitUpsideDown:
                        metadata.Orientation = devicePosition == AVCaptureDevicePosition.Front ? VisionDetectorImageOrientation.RightBottom : VisionDetectorImageOrientation.LeftBottom;
                        break;
                    case UIDeviceOrientation.LandscapeRight:
                        metadata.Orientation = devicePosition == AVCaptureDevicePosition.Front ? VisionDetectorImageOrientation.TopRight : VisionDetectorImageOrientation.BottomRight;
                        break;
                    case UIDeviceOrientation.FaceUp:
                    case UIDeviceOrientation.FaceDown:
                    case UIDeviceOrientation.Unknown:
                        metadata.Orientation = VisionDetectorImageOrientation.LeftTop;
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
                if (lastRunTime - lastAnalysisTime > scanIntervalInMs && _cameraPreview.IsScanning)
                {
                    lastAnalysisTime = lastRunTime;
                    try
                    {
                        var image = GetImageFromSampleBuffer(sampleBuffer);
                        if (image == null) return;
                        var visionImage = new VisionImage(image) { Metadata = metadata };
                        releaseSampleBuffer(sampleBuffer);
                        DetectBarcodeActionAsync(visionImage);
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(exception.Message);
                    }
                }
                releaseSampleBuffer(sampleBuffer);
            }
            private async void DetectBarcodeActionAsync(VisionImage image)
            {
                if (!_cameraPreview.IsScanning) return;

                try
                {
                    VisionBarcode[] barcodes = await barcodeDetector.DetectAsync(image);
                    if (barcodes == null || barcodes.Length == 0)
                    {
                        return;
                    }
                    Console.WriteLine($"Successfully read barcode");
                    _cameraPreview.SetIsScanning(false);
                    if (_vibrationOnDetected)
                        SystemSound.Vibrate.PlayAlertSound();
                    List<BarcodeResult> resultList = new List<BarcodeResult>();
                    foreach (var barcode in barcodes)
                    {
                        resultList.Add(new BarcodeResult
                        {
                            BarcodeType = Methods.ConvertBarcodeResultTypes(barcode.ValueType),
                            DisplayValue = barcode.DisplayValue,
                            RawValue = barcode.RawValue
                        });
                    }
                    OnDetected?.Invoke(resultList);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }


            }
        }

    }
}

