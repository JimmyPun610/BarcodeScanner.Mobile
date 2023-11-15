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

        public static void AddBarcodeScannerHandler(this IMauiHandlersCollection handlers)
        {
            handlers.AddHandler(typeof(ICameraView), typeof(CameraViewHandler));
        }

#if ANDROID
        public static Android.Util.Size GetTargetResolution(this CaptureQuality quality) => quality switch
        {
            CaptureQuality.Lowest => new Android.Util.Size(352, 288),
            CaptureQuality.Low => new Android.Util.Size(640, 480),
            CaptureQuality.Medium => new Android.Util.Size(1280, 720),
            CaptureQuality.High => new Android.Util.Size(1920, 1080),
            CaptureQuality.Highest => new Android.Util.Size(3840, 2160),
            _ => throw new ArgumentOutOfRangeException(nameof(CaptureQuality))
        };
#endif
    }
}
