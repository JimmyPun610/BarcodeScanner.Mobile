using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GoogleVisionBarCodeScanner.CameraView), typeof(GoogleVisionBarCodeScanner.Renderer.CameraViewRenderer))]
namespace GoogleVisionBarCodeScanner.Renderer
{
    internal class CameraViewRenderer : ViewRenderer<CameraView, UICameraPreview>
    {
        UICameraPreview liveCameraStream;
        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<CameraView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
            {
                if (liveCameraStream != null)
                {
                    liveCameraStream.OnDetected -= OnDetected;
                    liveCameraStream.IsScanningChanged -= InternalScanningChanged;
                }

                return;
            }

            if (e.NewElement != null && Control == null)
            {
                var cameraView = e.NewElement;
                liveCameraStream = new UICameraPreview(cameraView.TorchOn, cameraView.VibrationOnDetected, cameraView.IsScanning, cameraView.ScanInterval);
                SetNativeControl(liveCameraStream);
                liveCameraStream.OnDetected += OnDetected;
                liveCameraStream.IsScanningChanged += InternalScanningChanged;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == CameraView.DefaultTorchOnProperty.PropertyName
            || e.PropertyName == CameraView.TorchOnProperty.PropertyName)
                HandleTorch();
            if (e.PropertyName == CameraView.IsScanningProperty.PropertyName && Element != null)
                liveCameraStream?.SetIsScanning(Element.IsScanning);
        }

        private void HandleTorch()
        {
            if (Element == null || liveCameraStream == null) return;
            if (Element.TorchOn && liveCameraStream.IsTorchOn() || !Element.TorchOn && !liveCameraStream.IsTorchOn())
                return;
            liveCameraStream.ToggleFlashlight();
        }


        private void OnDetected(object sender, List<BarcodeResult> arg) =>
            Element?.TriggerOnDetected(arg);


        private void InternalScanningChanged(object sender, EventArgs arg)
        {
            if (Element == null || liveCameraStream == null || Element.IsScanning == liveCameraStream.IsScanning)
                return;
            Element.IsScanning = liveCameraStream.IsScanning;
        }
    }
}
