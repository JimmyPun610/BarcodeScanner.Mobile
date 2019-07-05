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

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Droid.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Droid.Renderer
{
    public class CameraViewRenderer : ViewRenderer<CameraView, GoogleVisionBarCodeScanner.Droid.CameraPreview>
    {
        CameraPreview cameraPreview;


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
                    if (Control == null)
                    {
                        cameraPreview = new CameraPreview(Context);
                        SetNativeControl(cameraPreview);
                    }
                    //liveCameraStream = new UICameraPreview();
                    //SetNativeControl(liveCameraStream);
                    //var cameraView = ((CameraView)e.NewElement);
                    //liveCameraStream.OnDetected += (list) =>
                    //{
                    //    cameraView?.TriggerOnDetected(list);
                    //};
                    //cameraView.ViewSizeChanged += (obj, arg) =>
                    //{
                    //    liveCameraStream.SizeChange();
                    //};
                }
            }
        }

    }
}