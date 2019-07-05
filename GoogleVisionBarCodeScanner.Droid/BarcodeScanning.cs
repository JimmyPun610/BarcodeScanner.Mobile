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

namespace GoogleVisionBarCodeScanner.Droid
{
    public class BarcodeScanning : Interface.IBarcodeScanning
    {
        public void SetSupportFormat(BarcodeFormats barcodeFormats)
        {
            throw new NotImplementedException();
        }

        public void ToggleFlashlight()
        {
            var _myCamera = GetCamera(Configuration.CameraSource);
            var prams = _myCamera.GetParameters();
            //prams.focus.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
            if (!Configuration.isTorch)
                prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
            else prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
            Configuration.isTorch = !Configuration.isTorch;
            _myCamera.SetParameters(prams);
        }

        private  Android.Hardware.Camera GetCamera(CameraSource cameraSource)
        {
            var javaHero = cameraSource.JavaCast<Java.Lang.Object>();
            var fields = javaHero.Class.GetDeclaredFields();
            foreach (var field in fields)
            {
                if (field.Type.CanonicalName.Equals("android.hardware.camera", StringComparison.OrdinalIgnoreCase))
                {
                    field.Accessible = true;
                    var camera = field.Get(javaHero);
                    var cCamera = (Android.Hardware.Camera)camera;
                    return cCamera;
                }
            }

            return null;
        }


    }
}