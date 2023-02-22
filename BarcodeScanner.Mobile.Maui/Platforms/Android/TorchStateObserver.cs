using AndroidX.Camera.Core;
using AndroidX.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeScanner.Mobile.Platforms.Android
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
