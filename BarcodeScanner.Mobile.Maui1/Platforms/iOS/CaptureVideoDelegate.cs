using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using MLKit.BarcodeScanning;
using Foundation;
using AudioToolbox;
using UIKit;
using MLKit.Core;
using BarcodeScanner.Mobile.Core;

namespace BarcodeScanner.Mobile.Maui.Platforms.iOS
{
    public class CaptureVideoDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public event Action<OnDetectedEventArg> OnDetected;
        MLKit.BarcodeScanning.BarcodeScanner barcodeDetector;
        UIImageOrientation orientation = UIImageOrientation.Up;
        long lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
        long lastRunTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
        ICameraView _cameraView;
        public CaptureVideoDelegate(ICameraView cameraView)
        {
            _cameraView = cameraView;

            if (_cameraView != null)
            {
                if (_cameraView.ScanInterval < 100)
                    _cameraView.ScanInterval = 500;
            }
            var options = new BarcodeScannerOptions(BarcodeScanner.Mobile.Core.Configuration.BarcodeDetectorSupportFormat);
            barcodeDetector = MLKit.BarcodeScanning.BarcodeScanner.BarcodeScannerWithOptions(options);
            orientation = GetUIImageOrientation();
        }

        private UIImageOrientation GetUIImageOrientation()
        {
            var orientation = UIImageOrientation.Up;
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
                    orientation = UIImageOrientation.Right;
                    break;
            }

            return orientation;
        }


        private static UIImage GetImageFromSampleBuffer(CMSampleBuffer sampleBuffer, UIImageOrientation? orientation)
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

                                if (orientation == null)
                                {
                                    return UIImage.FromImage(cgImage);
                                }

                                return UIImage.FromImage(cgImage, 1, orientation.Value);
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
            if (lastRunTime - lastAnalysisTime > _cameraView.ScanInterval && _cameraView.IsScanning)
            {
                lastAnalysisTime = lastRunTime;
                try
                {
                    var shouldReturnBarcodeImage = _cameraView.ReturnBarcodeImage;
                    var image = GetImageFromSampleBuffer(sampleBuffer, shouldReturnBarcodeImage ? GetUIImageOrientation() : null);
                    if (image == null) return;

                    var visionImage = new MLImage(image) { Orientation = orientation };
                    releaseSampleBuffer(sampleBuffer);
                    barcodeDetector.ProcessImage(visionImage, (barcodes, error) =>
                    {
                        if (_cameraView == null) return;
                        if (!_cameraView.IsScanning) return;

                        if (error != null)
                        {
                            System.Diagnostics.Debug.WriteLine(error);
                            return;
                        }

                        if (barcodes == null || barcodes.Length == 0)
                        {
                            return;
                        }

                        _cameraView.IsScanning = false;

                        if (_cameraView.VibrationOnDetected)
                            SystemSound.Vibrate.PlayAlertSound();

                        List<BarcodeResult> resultList = new List<BarcodeResult>();
                        foreach (var barcode in barcodes)
                            resultList.Add(Core.Methods.ProcessBarcodeResult(barcode));

                        var imageDataByteArray = new byte[0];
                        if (shouldReturnBarcodeImage)
                        {
                            using (NSData imageData = image.AsJPEG())
                            {
                                imageDataByteArray = new byte[imageData.Length];
                                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, imageDataByteArray, 0, Convert.ToInt32(imageData.Length));
                            }
                        }

                        OnDetected?.Invoke(new OnDetectedEventArg { BarcodeResults = resultList, ImageData = imageDataByteArray });
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
