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

namespace GoogleVisionBarCodeScanner
{
    public static class Configuration
    {
        public static Android.Gms.Vision.Barcodes.BarcodeFormat BarcodeFormats = Android.Gms.Vision.Barcodes.BarcodeFormat.Code128 | Android.Gms.Vision.Barcodes.BarcodeFormat.Codabar |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Code39 | Android.Gms.Vision.Barcodes.BarcodeFormat.Code93 | Android.Gms.Vision.Barcodes.BarcodeFormat.DataMatrix |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Ean13 | Android.Gms.Vision.Barcodes.BarcodeFormat.Ean8 | Android.Gms.Vision.Barcodes.BarcodeFormat.Itf |
             Android.Gms.Vision.Barcodes.BarcodeFormat.Pdf417 | Android.Gms.Vision.Barcodes.BarcodeFormat.QrCode | Android.Gms.Vision.Barcodes.BarcodeFormat.UpcA | Android.Gms.Vision.Barcodes.BarcodeFormat.UpcE;
        internal static bool IsScanning = true;
        internal static CameraSource CameraSource;
    }
}
