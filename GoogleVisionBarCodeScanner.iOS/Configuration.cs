using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.MLKit.Vision;
using Foundation;
using UIKit;

namespace GoogleVisionBarCodeScanner.iOS
{
    internal class Configuration
    {
        internal static bool IsScanning = true;
        public static VisionBarcodeDetectorOptions BarcodeDetectorSupportFormat = new VisionBarcodeDetectorOptions(VisionBarcodeFormat.All);
        
    }
}