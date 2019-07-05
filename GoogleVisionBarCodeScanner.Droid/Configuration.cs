using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoogleVisionBarCodeScanner.Droid
{
    public static class Configuration
    {
        public static Android.Gms.Vision.Barcodes.BarcodeFormat BarcodeFormats = Android.Gms.Vision.Barcodes.BarcodeFormat.QrCode;

        internal static bool isTorch = false;
        internal static CameraSource CameraSource;
    }
}