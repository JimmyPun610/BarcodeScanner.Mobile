using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Renderer
{
    public class CameraViewRenderer : ViewRenderer
    {
        UICameraPreview liveCameraStream;
        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var cameraView = ((CameraView)e.NewElement);
                    liveCameraStream = new UICameraPreview(cameraView.DefaultTorchOn, cameraView.VibrationOnDetected, cameraView.AutoStartScanning, cameraView.ScanInterval);
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
