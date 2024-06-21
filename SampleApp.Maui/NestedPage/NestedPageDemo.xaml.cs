using BarcodeScanner.Mobile;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace SampleApp.Maui.NestedPage;

public partial class NestedPageDemo : ContentPage
{

    public NestedPageDemo()
    {
        InitializeComponent();
        BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeFormats.Code39 | BarcodeFormats.QRCode | BarcodeFormats.Code128);
        On<iOS>().SetUseSafeArea(true);
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void CameraView_OnDetected(object sender, OnDetectedEventArg e)
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
            Camera.IsScanning = true;
        });

    }

    private void NextPage_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ContentPage()
        {
            Content = new Label()
            {
                Text = "Dummy nested page",
                Margin = 20,
                HorizontalTextAlignment = TextAlignment.Center
            }
        });
    }

}