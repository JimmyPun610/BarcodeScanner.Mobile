using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BarcodeScanner.Mobile
{
    public class CameraView : View
    {
        public static BindableProperty OnDetectedCommandProperty = BindableProperty.Create(nameof(OnDetectedCommand)
            , typeof(ICommand
            ), typeof(CameraView)
            , null
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).OnDetectedCommand = (ICommand)newValue);
        public ICommand OnDetectedCommand
        {
            get => (ICommand)GetValue(OnDetectedCommandProperty);
            set => SetValue(OnDetectedCommandProperty, value);
        }


        public static BindableProperty VibrationOnDetectedProperty = BindableProperty.Create(nameof(VibrationOnDetected)
            , typeof(bool)
            , typeof(CameraView)
            , true
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).VibrationOnDetected= (bool)newValue);
        public bool VibrationOnDetected
        {
            get => (bool)GetValue(VibrationOnDetectedProperty);
            set => SetValue(VibrationOnDetectedProperty, value);
        }

        public static BindableProperty PreviewHeightProperty = BindableProperty.Create(nameof(PreviewHeight), typeof(int?), typeof(CameraView), null);
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public int? PreviewHeight
        {
            get => (int?)GetValue(PreviewHeightProperty);
            set => SetValue(PreviewHeightProperty, value);
        }

        public static BindableProperty PreviewWidthProperty = BindableProperty.Create(nameof(PreviewWidth), typeof(int?), typeof(CameraView), null);
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public int? PreviewWidth
        {
            get => (int?)GetValue(PreviewWidthProperty);
            set => SetValue(PreviewWidthProperty, value);
        }

        public static BindableProperty RequestedFPSProperty = BindableProperty.Create(nameof(RequestedFPS)
            , typeof(float?)
            , typeof(CameraView)
            , null
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).RequestedFPS = (float?)newValue);
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public float? RequestedFPS
        {
            get => (float?)GetValue(RequestedFPSProperty);
            set => SetValue(RequestedFPSProperty, value);
        }


        public static BindableProperty ScanIntervalProperty = BindableProperty.Create(nameof(ScanInterval)
            , typeof(int)
            , typeof(CameraView)
            , 500
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).ScanInterval = (int)newValue);
        /// <summary>
        /// Only iOS will be reflected this setting, Default is 500ms, minimum value is 100ms
        /// </summary>
        public int ScanInterval
        {
            get => (int)GetValue(ScanIntervalProperty);
            set => SetValue(ScanIntervalProperty, value);
        }

        public static BindableProperty IsScanningProperty = BindableProperty.Create(nameof(IsScanning)
            , typeof(bool)
            , typeof(CameraView)
            , true
            , defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        /// Disables or enables scanning
        /// </summary>
        public bool IsScanning
        {
            get => (bool)GetValue(IsScanningProperty);
            set => SetValue(IsScanningProperty, value);
        }

        public static BindableProperty ReturnBarcodeImageProperty = BindableProperty.Create(nameof(ReturnBarcodeImage)
            , typeof(bool)
            , typeof(CameraView)
            , false
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).ReturnBarcodeImage = (bool)newValue);
        /// <summary>
        /// Disables or enables returning the image which the barcode was read from.
        /// </summary>
        public bool ReturnBarcodeImage
        {
            get => (bool)GetValue(ReturnBarcodeImageProperty);
            set => SetValue(ReturnBarcodeImageProperty, value);
        }

        public static BindableProperty TorchOnProperty = BindableProperty.Create(nameof(TorchOn)
            , typeof(bool)
            , typeof(CameraView)
            , false
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).TorchOn = (bool)newValue);
        /// <summary>
        /// Disables or enables torch
        /// </summary>
        public bool TorchOn
        {
            get => (bool)GetValue(TorchOnProperty);
            set => SetValue(TorchOnProperty, value);
        }

        public static BindableProperty CameraFacingProperty = BindableProperty.Create(nameof(CameraFacing)
            , typeof(CameraFacing)
            , typeof(CameraView)
            , CameraFacing.Back
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).CameraFacing = (CameraFacing)newValue);
        /// <summary>
        /// Select Back or Front camera.
        /// Default value is Back Camera
        /// </summary>
        public CameraFacing CameraFacing
        {
            get => (CameraFacing)GetValue(CameraFacingProperty);
            set => SetValue(CameraFacingProperty, value);
        }

        public static BindableProperty CaptureQualityProperty = BindableProperty.Create(nameof(CaptureQuality)
            , typeof(CaptureQuality)
            , typeof(CameraView)
            , CaptureQuality.Medium
            , defaultBindingMode: BindingMode.TwoWay
            , propertyChanged: (bindable, value, newValue) => ((CameraView)bindable).CaptureQuality = (CaptureQuality)newValue);

        /// <summary>
        /// Set the capture quality for the image analysys.
        /// Reccomended and default value is Medium.
        /// Use highest values for more precision or lower for fast scanning.
        /// </summary>
        public CaptureQuality CaptureQuality
        {
            get => (CaptureQuality)GetValue(CaptureQualityProperty);
            set => SetValue(CaptureQualityProperty, value);
        }

        public event EventHandler<OnDetectedEventArg> OnDetected;
        public void TriggerOnDetected(List<BarcodeResult> barCodeResults, byte[] imageData)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnDetected?.Invoke(this, new OnDetectedEventArg { BarcodeResults = barCodeResults, ImageData = imageData });
                OnDetectedCommand?.Execute( new OnDetectedEventArg { BarcodeResults = barCodeResults, ImageData = imageData });
            });
        }
    }
}
