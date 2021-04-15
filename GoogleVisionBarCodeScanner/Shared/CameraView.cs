using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GoogleVisionBarCodeScanner
{
    public class CameraView : View
    {
        public static BindableProperty VibrationOnDetectedProperty = BindableProperty.Create(nameof(VibrationOnDetected), typeof(bool), typeof(CameraView), true);
        public bool VibrationOnDetected
        {
            get
            {
                return (bool)GetValue(VibrationOnDetectedProperty);
            }
            set
            {
                SetValue(VibrationOnDetectedProperty, value);
            }
        }


        public static BindableProperty DefaultTorchOnProperty = BindableProperty.Create(nameof(DefaultTorchOn), typeof(bool), typeof(CameraView), false);
        public bool DefaultTorchOn
        {
            get
            {
                return (bool)GetValue(DefaultTorchOnProperty);
            }
            set
            {
                SetValue(DefaultTorchOnProperty, value);
            }
        }

        public static BindableProperty AutoStartScanningProperty = BindableProperty.Create(nameof(AutoStartScanning), typeof(bool), typeof(CameraView), true);
        public bool AutoStartScanning
        {
            get
            {
                return (bool)GetValue(AutoStartScanningProperty);
            }
            set
            {
                SetValue(AutoStartScanningProperty, value);
            }
        }

        public static BindableProperty RequestedFPSProperty = BindableProperty.Create(nameof(RequestedFPS), typeof(float?), typeof(CameraView), null);
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public float? RequestedFPS
        {
            get
            {
                return (float?)GetValue(RequestedFPSProperty);
            }
            set
            {
                SetValue(RequestedFPSProperty, value);
            }
        }


        public static BindableProperty ScanIntervalProperty = BindableProperty.Create(nameof(ScanInterval), typeof(int), typeof(CameraView), 500);
        /// <summary>
        /// Only iOS will be reflected this setting, Default is 500ms, minimum value is 100ms
        /// </summary>
        public int ScanInterval
        {
            get
            {
                return (int)GetValue(ScanIntervalProperty);
            }
            set
            {
                SetValue(ScanIntervalProperty, value);
            }
        }


        public event EventHandler<OnDetectedEventArg> OnDetected;
        public void TriggerOnDetected(List<BarcodeResult> barCodeResults)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnDetected?.Invoke(this, new OnDetectedEventArg { BarcodeResults = barCodeResults });
            });
        }
    }
    
    public class OnDetectedEventArg : EventArgs
    {
        public List<BarcodeResult> BarcodeResults { get; set; }
        public OnDetectedEventArg()
        {
            BarcodeResults = new List<BarcodeResult>();
        }
    }
}
