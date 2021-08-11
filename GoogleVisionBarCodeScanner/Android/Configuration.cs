using AndroidX.Camera.Core;
using Xamarin.Google.MLKit.Vision.BarCode;

namespace GoogleVisionBarCodeScanner
{
    public static class Configuration
    {
        
        public static int BarcodeFormats = Barcode.FormatAllFormats;

        internal static bool IsScanning = true;

        internal static ICamera Camera { get; set; }
    }
}
