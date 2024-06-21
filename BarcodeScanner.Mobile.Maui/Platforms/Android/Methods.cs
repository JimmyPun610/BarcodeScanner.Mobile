using Android.Gms.Extensions;
using Android.Graphics;
using Android.Runtime;
using Java.Util;
using Xamarin.Google.MLKit.Vision.Barcode.Common;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Common;

namespace BarcodeScanner.Mobile
{
    // All the code in this file is only included on Android.
    public class Methods
    {
        internal static BarcodeTypes ConvertBarcodeResultTypes(int barcodeValueType)
        {
            switch (barcodeValueType)
            {
                case Barcode.TypeCalendarEvent:
                    return BarcodeTypes.CalendarEvent;
                case Barcode.TypeContactInfo:
                    return BarcodeTypes.ContactInfo;
                case Barcode.TypeDriverLicense:
                    return BarcodeTypes.DriversLicense;
                case Barcode.TypeEmail:
                    return BarcodeTypes.Email;
                case Barcode.TypeGeo:
                    return BarcodeTypes.GeographicCoordinates;
                case Barcode.TypeIsbn:
                    return BarcodeTypes.Isbn;
                case Barcode.TypePhone:
                    return BarcodeTypes.Phone;
                case Barcode.TypeProduct:
                    return BarcodeTypes.Product;
                case Barcode.TypeSms:
                    return BarcodeTypes.Sms;
                case Barcode.TypeText:
                    return BarcodeTypes.Text;
                case Barcode.TypeUrl:
                    return BarcodeTypes.Url;
                case Barcode.TypeWifi:
                    return BarcodeTypes.WiFi;
                default: return BarcodeTypes.Unknown;
            }
        }

        internal static int ConvertBarcodeFormats(BarcodeFormats barcodeFormats)
        {
            var formats = Barcode.FormatAllFormats;

            if (barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                formats |= Barcode.FormatCodabar;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code128))
                formats |= Barcode.FormatCode128;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code93))
                formats |= Barcode.FormatCode93;
            if (barcodeFormats.HasFlag(BarcodeFormats.Code39))
                formats |= Barcode.FormatCode39;
            if (barcodeFormats.HasFlag(BarcodeFormats.CodaBar))
                formats |= Barcode.FormatCodabar;
            if (barcodeFormats.HasFlag(BarcodeFormats.DataMatrix))
                formats |= Barcode.FormatDataMatrix;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean13))
                formats |= Barcode.FormatEan13;
            if (barcodeFormats.HasFlag(BarcodeFormats.Ean8))
                formats |= Barcode.FormatEan8;
            if (barcodeFormats.HasFlag(BarcodeFormats.Itf))
                formats |= Barcode.FormatItf;
            if (barcodeFormats.HasFlag(BarcodeFormats.Pdf417))
                formats |= Barcode.FormatPdf417;
            if (barcodeFormats.HasFlag(BarcodeFormats.QRCode))
                formats |= Barcode.FormatQrCode;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upca))
                formats |= Barcode.FormatUpcA;
            if (barcodeFormats.HasFlag(BarcodeFormats.Upce))
                formats |= Barcode.FormatUpcE;
            if (barcodeFormats.HasFlag(BarcodeFormats.Aztec))
                formats |= Barcode.FormatAztec;
            if (barcodeFormats.HasFlag(BarcodeFormats.All))
                formats |= Barcode.FormatAllFormats;
            return formats;
        }
        #region Public Methods

        public static void SetAutofocusInterval(int interval) => Configuration.AutofocusInterval = interval;

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            int supportFormats = Methods.ConvertBarcodeFormats(barcodeFormats);
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
            using Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageArray, 0, imageArray.Length);
            if (bitmap == null)
                return null;
            using var image = InputImage.FromBitmap(bitmap, 0);
            var scanner = BarcodeScanning.GetClient(new BarcodeScannerOptions.Builder().SetBarcodeFormats(Configuration.BarcodeFormats)
                .Build());
            return ProcessBarcodeResult(await scanner.Process(image));
        }

        public static List<BarcodeResult> ProcessBarcodeResult(Java.Lang.Object result)
        {
            if (result == null)
                return null;
            var javaList = result.JavaCast<ArrayList>();
            if (javaList.IsEmpty)
                return null;
            List<BarcodeResult> resultList = new List<BarcodeResult>();
            foreach (var barcode in javaList.ToArray())
            {
                var mapped = barcode.JavaCast<Barcode>();

                List<Microsoft.Maui.Graphics.Point> cornerPoints = new List<Microsoft.Maui.Graphics.Point>();

                foreach (var cornerPoint in mapped.GetCornerPoints())
                    cornerPoints.Add(new Microsoft.Maui.Graphics.Point(cornerPoint.X, cornerPoint.Y));

                resultList.Add(new BarcodeResult()
                {
                    BarcodeType = ConvertBarcodeResultTypes(mapped.ValueType),
                    BarcodeFormat = (BarcodeFormats)mapped.Format,
                    DisplayValue = mapped.DisplayValue,
                    RawValue = mapped.RawValue,
                    CornerPoints = cornerPoints.ToArray(),
                    RawData = mapped.GetRawBytes()
                });
            }

            return resultList;
        }
        #endregion
    }
}