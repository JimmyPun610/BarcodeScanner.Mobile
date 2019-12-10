using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using GoogleVisionBarCodeScanner.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(BarcodeScanning))]
namespace GoogleVisionBarCodeScanner.Droid
{
    public class BarcodeScanning : Interface.IBarcodeScanning
    {
      

        public bool IsTorchOn()
        {
            var _myCamera = Methods.GetCamera(Configuration.CameraSource);
            bool torchOn = false;
            if(_myCamera != null)
            {
                string flashModeStatus = _myCamera.GetParameters().FlashMode;
                torchOn = flashModeStatus == Android.Hardware.Camera.Parameters.FlashModeTorch;
            }
            return torchOn;
        }

        public void SetSupportFormat(BarcodeFormats barcodeFormats)
        {
            Android.Gms.Vision.Barcodes.BarcodeFormat supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
            Configuration.BarcodeFormats = supportFormats;
        }

        public void ToggleFlashlight()
        {
            var _myCamera = Methods.GetCamera(Configuration.CameraSource);
            if (_myCamera != null)
            {
                var prams = _myCamera.GetParameters();
                //prams.focus.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
                if (!IsTorchOn())
                    prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
                else prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
                _myCamera.SetParameters(prams);
            }
            else Console.WriteLine($"Do not find camera");
           
        }
    }
}