using AndroidX.Camera.Core;
using AndroidX.Lifecycle;

namespace BarcodeScanner.Mobile
{
    internal class TorchStateObserver : Java.Lang.Object, IObserver
    {
        private readonly ICameraView _cameraView;

        public TorchStateObserver(ICameraView cameraView)
        {
            _cameraView = cameraView;
        }

        public void OnChanged(Java.Lang.Object state) =>
            _cameraView.TorchOn = (int)state == TorchState.On;

    }
}
