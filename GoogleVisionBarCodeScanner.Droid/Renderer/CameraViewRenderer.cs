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
using GoogleVisionBarCodeScanner.Droid.Renderer;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Droid.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Droid.Renderer
{
    public class CameraViewRenderer : ViewRenderer<CameraView, GoogleVisionBarCodeScanner.Droid.CameraPreview>
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
                    cameraPreview = new CameraPreview(Context, cameraView.DefaultTorchOn, cameraView.VirbationOnDetected);
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