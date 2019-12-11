using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using CoreGraphics;
using Firebase.MLKit.Vision;
using Foundation;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.iOS.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.iOS.Renderer
{
    public class CameraViewRenderer : ViewRenderer
    {
        UICameraPreview liveCameraStream;
        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if(e.OldElement != null || Element == null)
                return;
            if(e.NewElement != null)
            {
                if(Control == null)
                {
                    var cameraView = ((CameraView)e.NewElement);
                    liveCameraStream = new UICameraPreview(cameraView.DefaultTorchOn, cameraView.VirbationOnDetected);
                    SetNativeControl(liveCameraStream);
                    liveCameraStream.OnDetected += (list) =>
                    {
                        cameraView?.TriggerOnDetected(list);
                    };
         

                }
            }
        }

    }
}