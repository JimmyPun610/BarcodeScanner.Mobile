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

namespace GoogleVisionBarCodeScanner.iOS
{
    public class UICameraPreview : UIView
    {
        public event Action<List<BarcodeResult>> OnDetected;
        AVCaptureVideoPreviewLayer previewLayer;
        CaptureVideoDelegate captureVideoDelegate;
        //CameraOptions cameraOptions;
        public AVCaptureSession CaptureSession { get; private set; }
        AVCaptureVideoDataOutput VideoDataOutput { get; set; }

        //public UICameraPreview(CameraOptions options)
        //{
        //    cameraOptions = options;
        //    IsPreviewing = false;
        //    Initialize();
        //}

        public UICameraPreview()
        {
            //cameraOptions = options;
            Initialize();
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
            if(connection != null)
            {
                var curentDevice = UIDevice.CurrentDevice;
                var orientation = curentDevice.Orientation;
                var previewLayerConnection = connection;
                if (previewLayerConnection.SupportsVideoOrientation)
                {
                    switch (orientation)
                    {
                        case UIDeviceOrientation.Portrait:
                            updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                            break;
                        case UIDeviceOrientation.LandscapeRight:
                            updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeLeft);
                            break;
                        case UIDeviceOrientation.LandscapeLeft:
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
        void Initialize()
        {
            Configuration.IsScanning = true;
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
            CaptureSession.StartRunning();
   
            VideoDataOutput = new AVCaptureVideoDataOutput();
            VideoDataOutput.AlwaysDiscardsLateVideoFrames = true;
            VideoDataOutput.WeakVideoSettings = new CVPixelBufferAttributes { PixelFormatType = CVPixelFormatType.CV32BGRA }.Dictionary;


            captureVideoDelegate = new CaptureVideoDelegate();
            captureVideoDelegate.OnDetected += (list) =>
            {
                this.OnDetected?.Invoke(list);
                CaptureSession.StopRunning();
            };
            VideoDataOutput.SetSampleBufferDelegateQueue(captureVideoDelegate, CoreFoundation.DispatchQueue.MainQueue);

            CaptureSession.AddOutput(VideoDataOutput);

        }

        public class CaptureVideoDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
        {
            public event Action<List<BarcodeResult>> OnDetected;
            VisionBarcodeDetector barcodeDetector;
            VisionImageMetadata metadata;
            VisionApi vision;
            public CaptureVideoDelegate()
            {
                metadata = new VisionImageMetadata();
                vision = VisionApi.Create();
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
            private UIImage GetImageFromSampleBuffer(CMSampleBuffer sampleBuffer)
            {

                // Get a pixel buffer from the sample buffer
                using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
                {
                    // Lock the base address
                    pixelBuffer.Lock(CVOptionFlags.None);

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
                                pixelBuffer.Unlock(CVOptionFlags.None);
                                return UIImage.FromImage(cgImage);
                            }
                        }
                    }
                }
            }
            public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
            {
                // TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
                if (Configuration.IsScanning)
                {
                    try
                    {
                        UIImage image = GetImageFromSampleBuffer(sampleBuffer);
                        var visionImage = new VisionImage(image);
                        visionImage.Metadata = metadata;
                        DetectBarcodeActionAsync(visionImage);
                    }
                    catch { }
                    finally { sampleBuffer.Dispose(); }
                }
            }
            private async void DetectBarcodeActionAsync(VisionImage image)
            {
                if (Configuration.IsScanning)
                {
                    try
                    {
                        VisionBarcode[] barcodes = await barcodeDetector.DetectAsync(image);
                        if (barcodes == null || barcodes.Length == 0)
                        {
                            return;
                        }
                        Configuration.IsScanning = false;
                        if (Configuration.IsVibrate)
                            SystemSound.Vibrate.PlayAlertSound();
                        List<BarcodeResult> resultList = new List<BarcodeResult>();
                        foreach (var barcode in barcodes)
                        {
                            resultList.Add(new BarcodeResult
                            {
                                BarcodeType = Methods.ConvertBarcodeResultTypes(barcode.ValueType),
                                DisplayValue = barcode.DisplayValue
                            });
                        }
                        OnDetected?.Invoke(resultList);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
               
                
            }
        }

    }
}