using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GoogleVisionBarCodeScanner;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace SampleApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage, INotifyPropertyChanged
    {
        readonly SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.BlueViolet.ToSKColor(),
            StrokeWidth = 4
        };

        List<BarcodeResult> Barcodes { get; set; }

        public Page1()
        {
            InitializeComponent();
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.Code39 | BarcodeFormats.QRCode);
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
            Barcodes = e.BarcodeResults;
            Canvas.InvalidateSurface();
            Camera.IsScanning = true;
        }

        void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (Barcodes != null)
            {
                foreach (var b in Barcodes)
                {
                    if (b.CornerPoints?.Length > 1)
                    {
                        var points = b.CornerPoints.Select(p => new SKPoint((float)p.X, (float)p.Y)).ToList();
                        points.Add(points[0]);
                        canvas.DrawPoints(SKPointMode.Polygon, points.ToArray(), paint);
                    }
                }
            }
        }
    }
}
