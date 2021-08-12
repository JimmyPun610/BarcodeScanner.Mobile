using Firebase.MLKit.Vision;

namespace GoogleVisionBarCodeScanner
{
    internal class Configuration
    {
        public static VisionBarcodeDetectorOptions BarcodeDetectorSupportFormat = new VisionBarcodeDetectorOptions(VisionBarcodeFormat.All);
    }
}
