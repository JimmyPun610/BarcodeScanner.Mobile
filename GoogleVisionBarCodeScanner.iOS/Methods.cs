using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.MLKit.Vision;
using Foundation;
using UIKit;

namespace GoogleVisionBarCodeScanner.iOS
{
    internal class Methods
    {
        internal static BarcodeTypes ConvertBarcodeResultTypes(VisionBarcodeValueType visionBarcodeValueType)
        {
            switch (visionBarcodeValueType)
            {
                case VisionBarcodeValueType.CalendarEvent:
                    return BarcodeTypes.CalendarEvent;
                case VisionBarcodeValueType.ContactInfo:
                    return BarcodeTypes.ContactInfo;
                case VisionBarcodeValueType.DriversLicense:
                    return BarcodeTypes.DriversLicense;
                case VisionBarcodeValueType.Email:
                    return BarcodeTypes.Email;
                case VisionBarcodeValueType.GeographicCoordinates:
                    return BarcodeTypes.GeographicCoordinates;
                case VisionBarcodeValueType.Isbn:
                    return BarcodeTypes.Isbn;
                case VisionBarcodeValueType.Phone:
                    return BarcodeTypes.Phone;
                case VisionBarcodeValueType.Product:
                    return BarcodeTypes.Product;
                case VisionBarcodeValueType.Sms:
                    return BarcodeTypes.Sms;
                case VisionBarcodeValueType.Text:
                    return BarcodeTypes.Text;
                case VisionBarcodeValueType.Unknown:
                    return BarcodeTypes.Unknown;
                case VisionBarcodeValueType.Url:
                    return BarcodeTypes.Url;
                case VisionBarcodeValueType.WiFi:
                    return BarcodeTypes.WiFi;
                default: return BarcodeTypes.Unknown;
            }
        }
        internal static VisionBarcodeFormat ConvertBarcodeFormats(BarcodeFormats barcodeFormats)
        {
            VisionBarcodeFormat visionBarcodeFormat = VisionBarcodeFormat.All;

            if (barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                visionBarcodeFormat |= VisionBarcodeFormat.CodaBar;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code128))
                visionBarcodeFormat |= VisionBarcodeFormat.Code128;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code39))
                visionBarcodeFormat |= VisionBarcodeFormat.Code39;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code93))
                visionBarcodeFormat |= VisionBarcodeFormat.Code93;
            if (barcodeFormats.HasFlag(BarcodeFormats.DataMatrix))
                visionBarcodeFormat |= VisionBarcodeFormat.DataMatrix;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean13))
                visionBarcodeFormat |= VisionBarcodeFormat.Ean13;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean8))
                visionBarcodeFormat |= VisionBarcodeFormat.Ean8;
            if (barcodeFormats.HasFlag(BarcodeFormats.Itf))
                visionBarcodeFormat |= VisionBarcodeFormat.Itf;
            if (barcodeFormats.HasFlag(BarcodeFormats.Pdf417))
                visionBarcodeFormat |= VisionBarcodeFormat.Pdf417;
            if (barcodeFormats.HasFlag(BarcodeFormats.QRCode))
                visionBarcodeFormat |= VisionBarcodeFormat.QRCode;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upca))
                visionBarcodeFormat |= VisionBarcodeFormat.Upca;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upce))
                visionBarcodeFormat |= VisionBarcodeFormat.Upce;
            if (barcodeFormats.HasFlag(BarcodeFormats.Aztec))
                visionBarcodeFormat |= VisionBarcodeFormat.Aztec;
            if (barcodeFormats.HasFlag(BarcodeFormats.All))
                visionBarcodeFormat |= VisionBarcodeFormat.All;
            return visionBarcodeFormat;
        }

      
    }
}