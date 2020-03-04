using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
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
            try
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
            catch(Exception ex)
            {
                Console.WriteLine($"Error on switch on/off flashlight");
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
            BarcodeDetector detector = new BarcodeDetector.Builder(Android.App.Application.Context)
                                        .SetBarcodeFormats(Configuration.BarcodeFormats)
                                        .Build();
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            Android.Gms.Vision.Frame frame = new Android.Gms.Vision.Frame.Builder().SetBitmap(bitmap).Build();
            SparseArray qrcodes = detector.Detect(frame);
            List<BarcodeResult> barcodeResults = new List<BarcodeResult>();
            for (int i = 0; i < qrcodes.Size(); i++)
            {
                Barcode barcode = qrcodes.ValueAt(i) as Barcode;
                var type = Methods.ConvertBarcodeResultTypes(barcode.ValueFormat);
                var value = barcode.DisplayValue;
                barcodeResults.Add(new BarcodeResult
                {
                    BarcodeType = type,
                    DisplayValue = value
                });
            }
            return barcodeResults;
        }
    }
}