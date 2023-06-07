using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;


namespace SampleApp.Maui.OCRImageCapture;

public partial class OCRImageCaptureDemo : ContentPage
{
	public OCRImageCaptureDemo()
	{
		InitializeComponent();
        On<iOS>().SetUseSafeArea(true);
    }
    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();

    }
}