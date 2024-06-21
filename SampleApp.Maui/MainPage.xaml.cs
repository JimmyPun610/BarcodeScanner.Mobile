using BarcodeScanner.Mobile;

namespace SampleApp.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void Button1_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page1()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");

        }

        private async void Button2_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page2()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button3_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page3()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button4_Clicked(object sender, EventArgs e)
        {
            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (storageStatus != PermissionStatus.Granted)
                storageStatus = await Permissions.RequestAsync<Permissions.StorageRead>();

            if (storageStatus == PermissionStatus.Granted)
            {
                var file = await MediaPicker.PickPhotoAsync();
                if (file == null)
                    return;
                using Stream sourceStream = await file.OpenReadAsync();
                byte[] bytes = new byte[sourceStream.Length];
                sourceStream.Read(bytes, 0, bytes.Length);
                sourceStream.Seek(0, SeekOrigin.Begin);
                List<BarcodeScanner.Mobile.BarcodeResult> obj = await BarcodeScanner.Mobile.Methods.ScanFromImage(bytes);
                if (obj.Count > 0)
                {
                    string result = string.Empty;
                    for (int i = 0; i < obj.Count; i++)
                    {
                        result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
                    }
                    this.Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Result", result, "OK");
                    });
                }
                else
                {
                    this.Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Result", "No barcode found!", "OK");
                    });
                }

            }
            else
            {
                this.Dispatcher.Dispatch(async () =>
                {
                    await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                });
            }

        }

        private async void Button5_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Mvvm.MvvmDemo()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }


        private async void Button6_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new Page4()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button7_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new ImageCapture.ImageCaptureDemo()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button8_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new OCRImageCapture.OCRImageCaptureDemo()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }

        private async void Button9_Clicked(object sender, EventArgs e)
        {
            var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            while (storageStatus != PermissionStatus.Granted)
            {
                storageStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
            }
            if (storageStatus == PermissionStatus.Granted)
            {
                var file = await MediaPicker.PickPhotoAsync();
                if (file == null)
                    return;
                using Stream sourceStream = await file.OpenReadAsync();
                byte[] bytes = new byte[sourceStream.Length];
                sourceStream.Read(bytes, 0, bytes.Length);
                sourceStream.Seek(0, SeekOrigin.Begin);
                OCRResult obj = await BarcodeScanner.Mobile.OCRMethods.ScanFromImage(bytes);
                if (obj.Success == true && obj.Elements.Count > 0)
                {
                    string result = string.Empty;
                    foreach (var element in obj.Elements)
                    {
                        result += $"Confidence: {element.Confidence}, value: {element.Text}{Environment.NewLine}";
                    }

                    this.Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Result", result, "OK");
                    });
                }
                else
                {
                    this.Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Result", "No text found!", "OK");
                    });
                }

            }
            else
            {
                this.Dispatcher.Dispatch(async () =>
                {
                    await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
                });
            }

        }

        private async void Button10_Clicked(object sender, EventArgs e)
        {
            //Ask for permission first
            bool allowed = false;
            allowed = await Methods.AskForRequiredPermission();
            if (allowed)
                Navigation.PushModalAsync(new NavigationPage(new NestedPage.NestedPageDemo()));
            else DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }
    }
}