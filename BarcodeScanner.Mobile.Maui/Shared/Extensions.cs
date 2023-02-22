using Microsoft.Maui.Controls.Compatibility.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScanner.Mobile
{
    public static class Extensions
    {
//        public static MauiAppBuilder ConfigureBarcodeScanner(this MauiAppBuilder builder)
//        {
//            return builder
//                .UseMauiCompatibility()
//                .ConfigureMauiHandlers(handlers =>
//                {
//#if ANDROID
//                handlers.AddCompatibilityRenderer(typeof(CameraView), typeof(Platforms.Android.Renderer.CameraViewRenderer));
//#elif IOS
//                    handlers.AddCompatibilityRenderer(typeof(CameraView), typeof(Platforms.iOS.Renderer.CameraViewRenderer));
//#endif
//                });
//        }

        public static void AddBarcodeScannerHandler(this IMauiHandlersCollection handlers)
        {
            handlers.AddHandler(typeof(ICameraView), typeof(CameraViewHandler));
        }
    }
}
