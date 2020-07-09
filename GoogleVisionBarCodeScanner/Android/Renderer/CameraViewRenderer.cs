using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Hardware;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Renderer
{
    internal class CameraViewRenderer : ViewRenderer<CameraView, CameraPreview>
    {
        CameraPreview cameraPreview;
        CameraView cameraView;
        public CameraViewRenderer(Context context) : base(context)
        {

        }

        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    cameraView = ((CameraView)e.NewElement);
                    cameraPreview = new CameraPreview(Context, cameraView.DefaultTorchOn, cameraView.VibrationOnDetected, cameraView.AutoStartScanning, cameraView.RequestedFPS);
                    cameraPreview.OnDetected += (list) =>
                    {
                        cameraView?.TriggerOnDetected(list);
                    };
                    SetNativeControl(cameraPreview);

                }
            }
        }


    }
}