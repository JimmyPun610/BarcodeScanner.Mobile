using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SampleApp.XF.ImageCapture
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageCaptureDemo : ContentPage
    {
        public ImageCaptureDemo()
        {
            InitializeComponent();
        }

        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();

        }
    }
}