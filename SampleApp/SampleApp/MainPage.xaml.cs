using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        private void Button_Clicked(object sender, EventArgs e)
        {
            //var p = new GoogleVisionBarCodeScanner.BarcodeScanningPage();
            //p.SetupBarcodeScanningPage("AD", "Cancel", "ScanningDesc", "Flashlight");
            //Navigation.PushModalAsync(new NavigationPage(p));
            Navigation.PushModalAsync(new NavigationPage(new Page1()));

        }
    }
}
