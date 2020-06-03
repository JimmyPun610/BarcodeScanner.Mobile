

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GoogleVisionBarCodeScanner
{
    public class Methods 
    {
        public static void SetIsScanning(bool isScanning)
        {
            throw new NotImplementedException();
        }
        [Obsolete("Please use SetIsScanning(true) to replace this method.")]
        public static void Reset()
        {
            throw new NotImplementedException();
        }

        public static bool IsTorchOn()
        {
            throw new NotImplementedException();
        }
        public static void ToggleFlashlight()
        {
            throw new NotImplementedException();
        }

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            throw new NotImplementedException();
        }

        public static async Task<bool> AskForRequiredPermission()
        {
            throw new NotImplementedException();
        }

        public static async Task<List<BarcodeResult>> ScanFromImage(byte[] imageArray)
        {
            throw new NotImplementedException();
        }
    }
}
