using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Xamarin.Essentials;

namespace GoogleVisionBarCodeScanner
{
    public class Methods
    {
        internal static Android.Hardware.Camera GetCamera(CameraSource cameraSource)
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

        internal static BarcodeTypes ConvertBarcodeResultTypes(BarcodeValueFormat barcodeValueType)
        {
            switch (barcodeValueType)
            {
                case BarcodeValueFormat.CalendarEvent:
                    return BarcodeTypes.CalendarEvent;
                case BarcodeValueFormat.ContactInfo:
                    return BarcodeTypes.ContactInfo;
                case BarcodeValueFormat.DriverLicense:
                    return BarcodeTypes.DriversLicense;
                case BarcodeValueFormat.Email:
                    return BarcodeTypes.Email;
                case BarcodeValueFormat.Geo:
                    return BarcodeTypes.GeographicCoordinates;
                case BarcodeValueFormat.Isbn:
                    return BarcodeTypes.Isbn;
                case BarcodeValueFormat.Phone:
                    return BarcodeTypes.Phone;
                case BarcodeValueFormat.Product:
                    return BarcodeTypes.Product;
                case BarcodeValueFormat.Sms:
                    return BarcodeTypes.Sms;
                case BarcodeValueFormat.Text:
                    return BarcodeTypes.Text;
                case BarcodeValueFormat.Url:
                    return BarcodeTypes.Url;
                case BarcodeValueFormat.Wifi:
                    return BarcodeTypes.WiFi;
                default: return BarcodeTypes.Unknown;
            }
        }

        internal static Android.Gms.Vision.Barcodes.BarcodeFormat ConvertBarcodeFormats(BarcodeFormats barcodeFormats)
        {
            Android.Gms.Vision.Barcodes.BarcodeFormat formats = BarcodeFormat.AllFormats;

            if (barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                formats |= BarcodeFormat.Codabar;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code128))
                formats |= BarcodeFormat.Code128;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code93))
                formats |= BarcodeFormat.Code93;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code39))
                formats |= BarcodeFormat.Code39;
            if (barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                formats |= BarcodeFormat.Codabar;
            if (barcodeFormats.HasFlag(BarcodeFormats.DataMatrix))
                formats |= BarcodeFormat.DataMatrix;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean13))
                formats |= BarcodeFormat.Ean13;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean8))
                formats |= BarcodeFormat.Ean8;
            if (barcodeFormats.HasFlag(BarcodeFormats.Itf))
                formats |= BarcodeFormat.Itf;
            if (barcodeFormats.HasFlag(BarcodeFormats.Pdf417))
                formats |= BarcodeFormat.Pdf417;
            if (barcodeFormats.HasFlag(BarcodeFormats.QRCode))
                formats |= BarcodeFormat.QrCode;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upca))
                formats |= BarcodeFormat.UpcA;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upce))
                formats |= BarcodeFormat.UpcE;
            if (barcodeFormats.HasFlag(BarcodeFormats.Aztec))
                formats |= BarcodeFormat.Aztec;
            if (barcodeFormats.HasFlag(BarcodeFormats.All))
                formats |= BarcodeFormat.AllFormats;
            return formats;
        }
        #region Public Methods
        public static void SetIsScanning(bool isScanning)
        {
            Configuration.IsScanning = isScanning;
        }

        public static void Reset()
        {
            Configuration.IsScanning = true;
        }


        public static bool IsTorchOn()
        {
            var _myCamera = Methods.GetCamera(Configuration.CameraSource);
            bool torchOn = false;
            if (_myCamera != null)
            {
                string flashModeStatus = _myCamera.GetParameters().FlashMode;
                torchOn = flashModeStatus == Android.Hardware.Camera.Parameters.FlashModeTorch;
            }
            return torchOn;
        }
        public static void ToggleFlashlight()
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error on switch on/off flashlight");
            }

        }

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            Android.Gms.Vision.Barcodes.BarcodeFormat supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
            Configuration.BarcodeFormats = supportFormats;
        }

        public static async Task<bool> AskForRequiredPermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.Camera>();
                }
                status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status == PermissionStatus.Granted)
                    return true;
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
            return false;
        }

        public static async Task<List<BarcodeResult>> ScanFromImage(byte[] imageArray)
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

        #endregion
    }
}
