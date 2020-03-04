
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
            DependencyService.Get<Interface.IBarcodeScanning>().SetIsScanning(isScanning);
        }
        [Obsolete("Please use SetIsScanning(true) to replace this method.")]
        public static void Reset()
        {
            DependencyService.Get<Interface.IBarcodeScanning>().Reset();
        }

        public static bool IsTorchOn()
        {
            return DependencyService.Get<Interface.IBarcodeScanning>().IsTorchOn();
        }
        public static void ToggleFlashlight()
        {
            DependencyService.Get<Interface.IBarcodeScanning>().ToggleFlashlight();
        }

        public static void SetSupportBarcodeFormat(BarcodeFormats barcodeFormats)
        {
            DependencyService.Get<Interface.IBarcodeScanning>().SetSupportFormat(barcodeFormats);
        }

        public static async Task<bool> AskForRequiredPermission()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.Camera>();
                }
                status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status == PermissionStatus.Granted)
                    return true;
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
            return false;
        }

        public static async Task<List<BarcodeResult>> ScanFromImage(byte[] imageArray)
        {
            return await DependencyService.Get<Interface.IBarcodeScanning>().ScanFromImage(imageArray);
        }
    }
}
