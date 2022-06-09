using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;


namespace SampleApp.Maui.Mvvm;

public partial class MvvmDemo : ContentPage
{
	public MvvmDemo()
	{
		InitializeComponent();
        On<iOS>().SetUseSafeArea(true);
    }
    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();

    }
}