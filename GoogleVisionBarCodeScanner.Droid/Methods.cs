using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoogleVisionBarCodeScanner.Droid
{
    internal class Methods
    {
        public static Android.Hardware.Camera GetCamera(CameraSource cameraSource)
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

        public static Android.Gms.Vision.Barcodes.BarcodeFormat ConvertBarcodeFormats(BarcodeFormats barcodeFormats)
        {
            switch (barcodeFormats)
            {
                case BarcodeFormats.CodaBar:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Codabar;
                case BarcodeFormats.Code128:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Code128;
                case BarcodeFormats.Code39:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Code93;
                case BarcodeFormats.DataMatrix:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.DataMatrix;
                case BarcodeFormats.Ean13:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Ean13;
                case BarcodeFormats.Ean8:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Ean8;
                case BarcodeFormats.Itf:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Itf;
                case BarcodeFormats.Pdf417:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Pdf417;
                case BarcodeFormats.QRCode:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.QrCode;
                case BarcodeFormats.Upca:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.UpcA;
                case BarcodeFormats.Upce:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.UpcE;
                case BarcodeFormats.All:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Code128 | Android.Gms.Vision.Barcodes.BarcodeFormat.Codabar |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Code39 | Android.Gms.Vision.Barcodes.BarcodeFormat.Code93 | Android.Gms.Vision.Barcodes.BarcodeFormat.DataMatrix |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Ean13 | Android.Gms.Vision.Barcodes.BarcodeFormat.Ean8 | Android.Gms.Vision.Barcodes.BarcodeFormat.Itf |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Pdf417 | Android.Gms.Vision.Barcodes.BarcodeFormat.QrCode | Android.Gms.Vision.Barcodes.BarcodeFormat.UpcA | Android.Gms.Vision.Barcodes.BarcodeFormat.UpcE;
                default:
                    return Android.Gms.Vision.Barcodes.BarcodeFormat.Code128 | Android.Gms.Vision.Barcodes.BarcodeFormat.Codabar |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Code39 | Android.Gms.Vision.Barcodes.BarcodeFormat.Code93 | Android.Gms.Vision.Barcodes.BarcodeFormat.DataMatrix |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Ean13 | Android.Gms.Vision.Barcodes.BarcodeFormat.Ean8 | Android.Gms.Vision.Barcodes.BarcodeFormat.Itf |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Pdf417 | Android.Gms.Vision.Barcodes.BarcodeFormat.QrCode | Android.Gms.Vision.Barcodes.BarcodeFormat.UpcA | Android.Gms.Vision.Barcodes.BarcodeFormat.UpcE;
            }
        }
    }
}