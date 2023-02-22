using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

#if IOS
using NativeCameraView = BarcodeScanner.Mobile.Maui.Platforms.iOS.UICameraPreview;
#elif ANDROID
using NativeCameraView = AndroidX.Camera.View.PreviewView;
using AndroidX.Camera.View;
#endif

namespace BarcodeScanner.Mobile.Maui
{
    public partial class CameraViewHandler : ViewHandler<ICameraView, NativeCameraView>
    {
        public static PropertyMapper<ICameraView, CameraViewHandler> CameraViewMapper = new()
        {
            [nameof(ICameraView.TorchOn)] = (handler, virtualView) => handler.HandleTorch(),
#if ANDROID
            [nameof(ICameraView.CameraFacing)] = (handler, virtualView) => handler.CameraCallback(),
            [nameof(ICameraView.CaptureQuality)] = (handler, virtualView) => handler.CameraCallback()
#elif IOS
            [nameof(ICameraView.CameraFacing)] = (handler, virtualView) => handler.ChangeCameraFacing(),
            [nameof(ICameraView.CaptureQuality)] = (handler, virtualView) => handler.ChangeCameraQuality()
#endif
        };

        public static CommandMapper<ICameraView, CameraViewHandler> CameraCommandMapper = new()
        {
        };

        public CameraViewHandler() : base(CameraViewMapper)
        {
        }

   
        public CameraViewHandler(PropertyMapper mapper = null) : base(mapper ?? CameraViewMapper)
        {
        }

        protected override void ConnectHandler(NativeCameraView nativeView)
        {
            base.ConnectHandler(nativeView);
            this.Connect();
        }

        protected override void DisconnectHandler(NativeCameraView platformView)
        {
            this.Dispose();
            base.DisconnectHandler(platformView);
        }
    }
}
