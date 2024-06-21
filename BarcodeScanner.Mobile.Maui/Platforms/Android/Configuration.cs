using Xamarin.Google.MLKit.Vision.Barcode.Common;

namespace BarcodeScanner.Mobile
{
    public static class Configuration
    {
        public static int BarcodeFormats = Barcode.FormatAllFormats;
        /// <summary>
        /// In milliseconds, Default is 3000ms
        /// </summary>
        public static int AutofocusInterval = 3000;
    }
}
