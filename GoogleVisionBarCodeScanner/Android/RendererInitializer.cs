using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleVisionBarCodeScanner.Droid
{
    public class RendererInitializer
    {
        public static void Init()
        {
            Renderer.CameraViewRenderer.Init();
        }
    }
}
