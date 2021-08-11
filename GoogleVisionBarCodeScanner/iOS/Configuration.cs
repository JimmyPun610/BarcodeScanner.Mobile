using Firebase.MLKit.Vision;

namespace GoogleVisionBarCodeScanner
{
    internal class Configuration
    {
        internal static bool IsScanning = true;
        public static VisionBarcodeDetectorOptions BarcodeDetectorSupportFormat = new VisionBarcodeDetectorOptions(VisionBarcodeFormat.All);
    }
}
