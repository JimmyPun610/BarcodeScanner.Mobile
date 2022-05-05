using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Java.Util;
using Xamarin.Essentials;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Common;

namespace GoogleVisionBarCodeScanner
{
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

        public static async Task<List<BarcodeResult>> ScanFromImage(byte[] imageArray, float formsWitdh, float formsHeight)
        {
            using Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageArray, 0, imageArray.Length);
            if (bitmap == null)
                return null;
            using var image = InputImage.FromBitmap(bitmap, 0);
            var scanner = BarcodeScanning.GetClient(new BarcodeScannerOptions.Builder().SetBarcodeFormats(Configuration.BarcodeFormats)
                .Build());
            return Process(await scanner.Process(image), image, formsWitdh, formsHeight);
        }

        internal static List<BarcodeResult> Process(Java.Lang.Object result, InputImage image, float formsWitdh, float formsHeight)
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
                var corners = mapped.GetCornerPoints().Select(p => MapPoint(p, image, formsWitdh, formsHeight)).ToArray();
                resultList.Add(new BarcodeResult()
                {
                    BarcodeType = ConvertBarcodeResultTypes(mapped.ValueType),
                    BarcodeFormat = (BarcodeFormats)mapped.Format,
                    DisplayValue = mapped.DisplayValue,
                    RawValue = mapped.RawValue,
                    CornerPoints = corners,
                });
            }

            return resultList;
        }

        private static BarcodePoint MapPoint(Point originalPoint, InputImage image, float formsWitdh, float formsHeight)
        {
            var width = (double)image.Width;
            var height = (double)image.Height;

            // Calculate points with the origin in the center of the image
            // range: [-1 ... 1]
            var x = ((originalPoint.X / width) - 0.5) * 2;
            var y = ((originalPoint.Y / height) - 0.5) * 2;

            var fcx = formsWitdh * 0.5;
            var fcy = formsHeight * 0.5;

            var orientation = Platform.CurrentActivity.WindowManager.DefaultDisplay.Rotation;
            switch (orientation)
            {
                case SurfaceOrientation.Rotation180:
                    return new BarcodePoint(fcx + (fcx * y), fcy - (fcy * x));

                case SurfaceOrientation.Rotation90: // button on the right
                case SurfaceOrientation.Rotation270: // button on the left
                    return new BarcodePoint(fcx + (fcx * x), fcy + (fcy * y));

                default: //SurfaceOrientation.Rotation0
                    return new BarcodePoint(fcx + (fcx * x), fcy + (fcy * y));
            }
        }
        #endregion
    }
}
