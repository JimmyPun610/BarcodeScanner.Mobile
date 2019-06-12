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
using Xamarin.Forms;

namespace GoogleVisionBarCodeScanner.Droid
{
    public static class Operation
    {
        public static void StartScanning(Action<string> callback)
        {
            Configuration.ScannedQRCode = callback;
            (Forms.Context).StartActivity(typeof(BarCodeScanningActivity));
        }
    }
}