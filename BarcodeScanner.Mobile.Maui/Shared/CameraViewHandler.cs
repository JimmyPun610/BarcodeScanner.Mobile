using Microsoft.Maui.Handlers;

#if IOS
using NativeCameraView = BarcodeScanner.Mobile.Platforms.iOS.UICameraPreview;
#elif ANDROID
using NativeCameraView = AndroidX.Camera.View.PreviewView;
using AndroidX.Camera.View;
#elif WINDOWS
using NativeCameraView = Microsoft.UI.Xaml.Controls.GridView;
#endif

namespace BarcodeScanner.Mobile
{
#if ANDROID || IOS || WINDOWS
    public partial class CameraViewHandler : ViewHandler<ICameraView, NativeCameraView>
    {
        public static PropertyMapper<ICameraView, CameraViewHandler> CameraViewMapper = new()
        {
            [nameof(ICameraView.TorchOn)] = (handler, virtualView) => handler.HandleTorch(),
            [nameof(ICameraView.Zoom)] = (handler, virtualView) => handler.HandleZoom(),
#if ANDROID
            [nameof(ICameraView.CameraFacing)] = (handler, virtualView) => handler.CameraCallback(),
            [nameof(ICameraView.CaptureQuality)] = (handler, virtualView) => handler.CameraCallback()
#elif IOS
            [nameof(ICameraView.CameraFacing)] = (handler, virtualView) => handler.ChangeCameraFacing(),
            [nameof(ICameraView.CaptureQuality)] = (handler, virtualView) => handler.ChangeCameraQuality()
#endif
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
            Connect();
        }

        protected override void DisconnectHandler(NativeCameraView platformView)
        {
            Dispose();
            platformView.Dispose();
            base.DisconnectHandler(platformView);
        }
    }
#else
    public partial class CameraViewHandler { }
#endif
}
