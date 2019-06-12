using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoogleVisionBarCodeScanner.Droid
{
    public static class Configuration
    {
        public static string Title = "掃瞄QRCode";
        public static string FlashlightMessage = "Flashlight";
        public static string ScanningDescription = "請掃瞄QRCode";
        public static string CancelText = "取消";
        public static Action<string> ScannedQRCode;
        public static Android.Gms.Vision.Barcodes.BarcodeFormat BarcodeFormats = Android.Gms.Vision.Barcodes.BarcodeFormat.QrCode;
        
    }
}