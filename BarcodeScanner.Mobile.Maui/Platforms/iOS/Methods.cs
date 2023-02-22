using CoreGraphics;
using Foundation;
using MLKit.BarcodeScanning;
using MLKit.Core;
using UIKit;

namespace BarcodeScanner.Mobile
{
    // All the code in this file is only included on iOS.
    public class Methods
    {
        internal static BarcodeTypes ConvertBarcodeResultTypes(BarcodeValueType visionBarcodeValueType)
        {
            switch (visionBarcodeValueType)
            {
                case BarcodeValueType.CalendarEvent:
                    return BarcodeTypes.CalendarEvent;
                case BarcodeValueType.ContactInfo:
                    return BarcodeTypes.ContactInfo;
                case BarcodeValueType.DriversLicense:
                    return BarcodeTypes.DriversLicense;
                case BarcodeValueType.Email:
                    return BarcodeTypes.Email;
                case BarcodeValueType.GeographicCoordinates:
                    return BarcodeTypes.GeographicCoordinates;
                case BarcodeValueType.Isbn:
                    return BarcodeTypes.Isbn;
                case BarcodeValueType.Phone:
                    return BarcodeTypes.Phone;
                case BarcodeValueType.Product:
                    return BarcodeTypes.Product;
                case BarcodeValueType.Sms:
                    return BarcodeTypes.Sms;
                case BarcodeValueType.Text:
                    return BarcodeTypes.Text;
                case BarcodeValueType.Unknown:
                    return BarcodeTypes.Unknown;
                case BarcodeValueType.Url:
                    return BarcodeTypes.Url;
                case BarcodeValueType.WiFi:
                    return BarcodeTypes.WiFi;
                default: return BarcodeTypes.Unknown;
            }
        }
        internal static BarcodeFormat ConvertBarcodeFormats(BarcodeFormats barcodeFormats)
        {
            BarcodeFormat visionBarcodeFormat = BarcodeFormat.Unknown;

            if (barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                visionBarcodeFormat |= BarcodeFormat.CodaBar;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code128))
                visionBarcodeFormat |= BarcodeFormat.Code128;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code39))
                visionBarcodeFormat |= BarcodeFormat.Code39;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code93))
                visionBarcodeFormat |= BarcodeFormat.Code93;
            if (barcodeFormats.HasFlag(BarcodeFormats.DataMatrix))
                visionBarcodeFormat |= BarcodeFormat.DataMatrix;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean13))
                visionBarcodeFormat |= BarcodeFormat.Ean13;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean8))
                visionBarcodeFormat |= BarcodeFormat.Ean8;
            if (barcodeFormats.HasFlag(BarcodeFormats.Itf))
                visionBarcodeFormat |= BarcodeFormat.Itf;
            if (barcodeFormats.HasFlag(BarcodeFormats.Pdf417))
                visionBarcodeFormat |= BarcodeFormat.Pdf417;
            if (barcodeFormats.HasFlag(BarcodeFormats.QRCode))
                visionBarcodeFormat |= BarcodeFormat.QrCode;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upca))
                visionBarcodeFormat |= BarcodeFormat.Upca;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upce))
                visionBarcodeFormat |= BarcodeFormat.Upce;
            if (barcodeFormats.HasFlag(BarcodeFormats.Aztec))
                visionBarcodeFormat |= BarcodeFormat.Aztec;
            if (barcodeFormats.HasFlag(BarcodeFormats.All))
                visionBarcodeFormat |= BarcodeFormat.All;

            if (visionBarcodeFormat == BarcodeFormat.Unknown)
                visionBarcodeFormat = BarcodeFormat.All;

            return visionBarcodeFormat;
        }

        #region Public Methods

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            BarcodeFormat supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
            Configuration.BarcodeDetectorSupportFormat = supportFormats;
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
            var visionImage = new MLImage(image);
            //VisionImageMetadata metadata = new VisionImageMetadata();
            //VisionApi vision = VisionApi.Create();
            //VisionBarcodeDetector barcodeDetector = vision.GetBarcodeDetector(Configuration.BarcodeDetectorSupportFormat);
            //VisionBarcode[] barcodes = await barcodeDetector.DetectAsync(visionImage);
            var options = new BarcodeScannerOptions(Configuration.BarcodeDetectorSupportFormat);
            var barcodeScanner = MLKit.BarcodeScanning.BarcodeScanner.BarcodeScannerWithOptions(options);

            var tcs = new TaskCompletionSource<List<BarcodeResult>>();

            barcodeScanner.ProcessImage(visionImage, (barcodes, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Error occurred : {error}");
                    tcs.TrySetResult(null);
                    return;
                }
                if (barcodes == null || barcodes.Length == 0)
                {
                    tcs.TrySetResult(new List<BarcodeResult>());
                    return;
                }

                var s = image.Size;
                List<BarcodeResult> resultList = new List<BarcodeResult>();
                foreach (var barcode in barcodes)
                    resultList.Add(ProcessBarcodeResult(barcode));

                tcs.TrySetResult(resultList);
                return;
            });
            return await tcs.Task;
        }

        public static BarcodeResult ProcessBarcodeResult(Barcode barcode)
        {
            List<Microsoft.Maui.Graphics.Point> cornerPoints = new List<Microsoft.Maui.Graphics.Point>();

            foreach (var cornerPoint in barcode.CornerPoints)
                cornerPoints.Add(new Microsoft.Maui.Graphics.Point(cornerPoint.CGPointValue.X, cornerPoint.CGPointValue.Y));
            
            return new BarcodeResult
            {
                BarcodeType = ConvertBarcodeResultTypes(barcode.ValueType),
                BarcodeFormat = (BarcodeFormats)barcode.Format,
                DisplayValue = barcode.DisplayValue,
                RawValue = barcode.RawValue,
                CornerPoints = cornerPoints.ToArray(),
                RawData = barcode.RawData.ToArray()
            };
        }
        #endregion
    }
}