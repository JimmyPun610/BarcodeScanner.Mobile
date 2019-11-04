using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}