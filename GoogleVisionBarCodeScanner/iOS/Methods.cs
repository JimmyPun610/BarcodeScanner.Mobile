using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using Firebase.MLKit.Vision;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace GoogleVisionBarCodeScanner
{
    public class Methods
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
            VisionBarcodeFormat visionBarcodeFormat = VisionBarcodeFormat.UnKnown;
            
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

            if (visionBarcodeFormat == VisionBarcodeFormat.UnKnown)
                visionBarcodeFormat = VisionBarcodeFormat.All;

            return visionBarcodeFormat;
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
            var videoDevices = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (videoDevices.HasTorch)
            {
                return videoDevices.TorchMode == AVCaptureTorchMode.On;
            }
            return false;
        }
        public static void ToggleFlashlight()
        {
            var videoDevices = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (videoDevices.HasTorch)
            {
                NSError error;
                videoDevices.LockForConfiguration(out error);
                if (error == null)
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

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            VisionBarcodeFormat supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
            Configuration.BarcodeDetectorSupportFormat = new Firebase.MLKit.Vision.VisionBarcodeDetectorOptions(supportFormats);
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
            UIImage image = new UIImage(NSData.FromArray(imageArray));
            var visionImage = new VisionImage(image);
            VisionImageMetadata metadata = new VisionImageMetadata();
            VisionApi vision = VisionApi.Create();
            VisionBarcodeDetector barcodeDetector = vision.GetBarcodeDetector(Configuration.BarcodeDetectorSupportFormat);
            VisionBarcode[] barcodes = await barcodeDetector.DetectAsync(visionImage);
            if (barcodes == null || barcodes.Length == 0)
                return new List<BarcodeResult>();

            List<BarcodeResult> resultList = new List<BarcodeResult>();
            foreach (var barcode in barcodes)
            {
                resultList.Add(new BarcodeResult
                {
                    BarcodeType = Methods.ConvertBarcodeResultTypes(barcode.ValueType),
                    DisplayValue = barcode.DisplayValue,
                    RawValue = barcode.RawValue
                });
            }
            return resultList;
        }

        #endregion
    }
}
