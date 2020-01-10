using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using Firebase.MLKit.Vision;
using Foundation;
using GoogleVisionBarCodeScanner.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(BarcodeScanning))]
namespace GoogleVisionBarCodeScanner.iOS
{
    public class BarcodeScanning : Interface.IBarcodeScanning
    {


        public bool IsTorchOn()
        {
            var videoDevices = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (videoDevices.HasTorch)
            {
                return videoDevices.TorchMode == AVCaptureTorchMode.On;
            }
            return false;
        }

        public void SetSupportFormat(BarcodeFormats barcodeFormats)
        {
            VisionBarcodeFormat supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
            Configuration.BarcodeDetectorSupportFormat = new Firebase.MLKit.Vision.VisionBarcodeDetectorOptions(supportFormats);
        }

        public void ToggleFlashlight()
        {
            var videoDevices = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (videoDevices.HasTorch)
            {
                NSError error;
                videoDevices.LockForConfiguration(out error);
                if(error == null)
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
        }

        public void Reset()
        {
            Configuration.IsScanning = true;
        }

        public void SetIsScanning(bool isScanning)
        {
            Configuration.IsScanning = isScanning;
        }

        public async Task<List<BarcodeResult>> ScanFromImage(byte[] imageArray)
        {
            UIImage image = new UIImage(NSData.FromArray(imageArray));
            var visionImage = new VisionImage(image);
            VisionImageMetadata metadata = new VisionImageMetadata();
            VisionApi vision = VisionApi.Create();
            VisionBarcodeDetector barcodeDetector = vision.GetBarcodeDetector(Configuration.BarcodeDetectorSupportFormat);
            VisionBarcode[] barcodes = await barcodeDetector.DetectAsync(visionImage);
            if (barcodes == null || barcodes.Length == 0)
                return new List<BarcodeResult>();
 
            List<BarcodeResult> resultList = new List<BarcodeResult>();
            foreach (var barcode in barcodes)
            {
                resultList.Add(new BarcodeResult
                {
                    BarcodeType = Methods.ConvertBarcodeResultTypes(barcode.ValueType),
                    DisplayValue = barcode.DisplayValue
                });
            }
            return resultList;
        }
    }
}