using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GoogleVisionBarCodeScanner.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(BarcodeScanning))]
namespace GoogleVisionBarCodeScanner.Droid
{
    public class BarcodeScanning : Interface.IBarcodeScanning
    {
        public void SetSupportFormat(BarcodeFormats barcodeFormats)
        {
            Android.Gms.Vision.Barcodes.BarcodeFormat supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
            Configuration.BarcodeFormats = supportFormats;
        }

        public void ToggleFlashlight()
        {
            var _myCamera = Methods.GetCamera(Configuration.CameraSource);
            var prams = _myCamera.GetParameters();
            //prams.focus.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
            if (!Configuration.isTorch)
                prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
            else prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
            Configuration.isTorch = !Configuration.isTorch;
            _myCamera.SetParameters(prams);
        }
    }
}