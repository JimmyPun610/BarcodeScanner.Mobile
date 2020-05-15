using Xamarin.Forms;

namespace GoogleVisionBarCodeScanner.iOS
{
    public class Initializer
    {
        public static void Init()
        {
            Renderer.CameraViewRenderer.Init();
            BarcodeScanning.Init();
        }
    }
}