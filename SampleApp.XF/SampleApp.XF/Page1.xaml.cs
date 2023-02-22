using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using BarcodeScanner.Mobile;


namespace SampleApp.XF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage, INotifyPropertyChanged
    {
        //readonly SKPaint paint = new SKPaint
        //{
        //    Style = SKPaintStyle.Stroke,
        //    Color = Color.BlueViolet.ToSKColor(),
        //    StrokeWidth = 4
        //};


        public Page1()
        {
            InitializeComponent();
            BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeFormats.Code39 | BarcodeFormats.QRCode | BarcodeFormats.Code128);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }


        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void FlashlightButton_Clicked(object sender, EventArgs e)
        {
            Camera.TorchOn = !Camera.TorchOn;
        }

        private void SwitchCameraButton_Clicked(object sender, EventArgs e)
        {
            Camera.CameraFacing = Camera.CameraFacing == CameraFacing.Back
                                      ? CameraFacing.Front
                                      : CameraFacing.Back;
        }

        private void CameraView_OnDetected(object sender, OnDetectedEventArg e)
        {
            List<BarcodeResult> obj = e.BarcodeResults;

            string result = string.Empty;
            for (int i = 0; i < obj.Count; i++)
            {
                result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Result", result, "OK");
                Camera.IsScanning = true;
            });

            //Canvas.InvalidateSurface();
            
        }

        //void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        //{
        //    SKImageInfo info = args.Info;
        //    SKSurface surface = args.Surface;
        //    SKCanvas canvas = surface.Canvas;

        //    canvas.Clear();

        //    if (Barcodes != null)
        //    {
        //        foreach (var b in Barcodes)
        //        {
        //            canvas.DrawRect(new SKRect(b.BoundingBox.Left, b.BoundingBox.Top, b.BoundingBox.Right, b.BoundingBox.Bottom), paint);
        //        }
        //    }
        //}
    }
}
