using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;


namespace SampleApp.Maui.ImageCapture;

public partial class ImageCaptureDemo : ContentPage
{
	public ImageCaptureDemo()
	{
		InitializeComponent();
        On<iOS>().SetUseSafeArea(true);
    }
    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();

    }
}