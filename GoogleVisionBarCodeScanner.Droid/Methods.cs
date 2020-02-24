using System;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Runtime;

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
            Android.Gms.Vision.Barcodes.BarcodeFormat formats = BarcodeFormat.AllFormats;

            if(barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                formats |= BarcodeFormat.Codabar;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code128))
                formats |= BarcodeFormat.Code128;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code93))
                formats |= BarcodeFormat.Code93;
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
    }
}