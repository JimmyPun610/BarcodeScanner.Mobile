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
            switch (barcodeFormats)
            {
                case BarcodeFormats.CodaBar:
                    return VisionBarcodeFormat.CodaBar;
                case BarcodeFormats.Code128:
                    return VisionBarcodeFormat.Code128;
                case BarcodeFormats.Code39:
                    return VisionBarcodeFormat.Code39;
                case BarcodeFormats.Code93:
                    return VisionBarcodeFormat.Code93;
                case BarcodeFormats.DataMatrix:
                    return VisionBarcodeFormat.DataMatrix;
                case BarcodeFormats.Ean13:
                    return VisionBarcodeFormat.Ean8;
                case BarcodeFormats.Itf:
                    return VisionBarcodeFormat.Itf;
                case BarcodeFormats.Pdf417:
                    return VisionBarcodeFormat.Pdf417;
                case BarcodeFormats.QRCode:
                    return VisionBarcodeFormat.QRCode;
                case BarcodeFormats.Upca:
                    return VisionBarcodeFormat.Upca;
                case BarcodeFormats.Upce:
                    return VisionBarcodeFormat.Upce;
                case BarcodeFormats.All:
                    return VisionBarcodeFormat.All;
                default:
                    return VisionBarcodeFormat.All;
            }
        }
    }
}