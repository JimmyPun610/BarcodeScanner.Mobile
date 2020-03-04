using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SampleApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        
        private async void Button1_Clicked(object sender, EventArgs e)
        {
            //var p = new GoogleVisionBarCodeScanner.BarcodeScanningPage();
            //p.SetupBarcodeScanningPage("AD", "Cancel", "ScanningDesc", "Flashlight");
            //Navigation.PushModalAsync(new NavigationPage(p));

            //Ask for permission first
            bool allowed = false;
            allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page1()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");

        }

        private async void Button2_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page2()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button3_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page3()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button4_Clicked(object sender, EventArgs e)
        {
            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            while(storageStatus != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            if (storageStatus == PermissionStatus.Granted)
            {
                var file = await CrossMedia.Current.PickPhotoAsync();
                Stream stream = file.GetStream();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                List<GoogleVisionBarCodeScanner.BarcodeResult> obj = await GoogleVisionBarCodeScanner.Methods.ScanFromImage(bytes);
                if(obj.Count > 0)
                {
                    string result = string.Empty;
                    for (int i = 0; i < obj.Count; i++)
                    {
                        result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
                    }
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Result", result, "OK");
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Result", "No barcode found!", "OK");
                    });
                }
                
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                });
            }
            
        }
    }
}
