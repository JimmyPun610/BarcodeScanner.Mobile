namespace GoogleVisionBarCodeScanner.Droid
{
    public class RendererInitializer
    {
        public static void Init()
        {
            Renderer.CameraViewRenderer.Init();
            BarcodeScanning.Init();
        }
    }
}