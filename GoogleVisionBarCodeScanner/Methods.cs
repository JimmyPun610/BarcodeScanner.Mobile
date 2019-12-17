using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                }
                status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status == PermissionStatus.Granted)
                    return true;
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
            return false;
        }
    }
}
