using BarcodeScanner.Mobile;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;


namespace SampleApp.Maui;

public partial class Page4 : ContentPage
{
    public Page4()
    {
        InitializeComponent();
        On<iOS>().SetUseSafeArea(true);
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void FlashlightButton_Clicked(object sender, EventArgs e)
    {
        Camera.TorchOn = !Camera.TorchOn;
    }

    private void StartScanningButton_Clicked(object sender, EventArgs e)
    {
        Camera.IsScanning = true;
    }

    private void SwitchCameraButton_Clicked(object sender, EventArgs e)
    {
        Camera.CameraFacing = Camera.CameraFacing == CameraFacing.Back
                                  ? CameraFacing.Front
                                  : CameraFacing.Back;
    }

    private async void CameraView_OnDetected(object sender, OnDetectedEventArg e)
    {
        List<BarcodeResult> obj = e.BarcodeResults;

        string result = string.Empty;
        for (int i = 0; i < obj.Count; i++)
        {
            result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
        }
        this.Dispatcher.Dispatch(async () =>
        {
            await DisplayAlert("Result", result, "OK");
            //GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            await Navigation.PopModalAsync();
        });

    }

    private async void ButtonScan_Clicked(object sender, EventArgs e)
    {
        var storageStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        while (storageStatus != PermissionStatus.Granted)
        {
            storageStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
        }
        if (storageStatus == PermissionStatus.Granted)
        {
            var file = await FilePicker.PickAsync();
            if (file == null)
                return;
            using (Stream stream = await file.OpenReadAsync())
            {
                byte[] bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                List<BarcodeResult> obj = await BarcodeScanner.Mobile.Methods.ScanFromImage(bytes);
                if (obj == null)
                {
                    this.Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Bad item", null, "OK");
                    });
                    return;
                }
                if (obj.Count > 0)
                {
                    string result = string.Empty;
                    foreach (var barcode in obj)
                    {
                        result += $"Type : {barcode.BarcodeType}, Value : {barcode.DisplayValue}{Environment.NewLine}";
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
        }
        else
        {
            this.Dispatcher.Dispatch(async () =>
            {
                await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
            });
        }

    }
}