using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GoogleVisionBarCodeScanner
{
    public class Methods
    {
        public static void ToggleFlashlight()
        {
            DependencyService.Get<Interface.IBarcodeScanning>().ToggleFlashlight();
        }

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            DependencyService.Get<Interface.IBarcodeScanning>().SetSupportFormat(barcodeFormats);
        }
    }
}
