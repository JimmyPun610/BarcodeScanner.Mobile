using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleVisionBarCodeScanner.iOS
{
    public class Initializer
    {
        public static void Init()
        {
            Renderer.CameraViewRenderer.Init();
        }
    }
}
