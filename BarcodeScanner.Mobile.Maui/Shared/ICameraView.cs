using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BarcodeScanner.Mobile
{
    public interface ICameraView : IView
    {
        public static BindableProperty OnDetectedCommandProperty { get; set; }
        public ICommand OnDetectedCommand { get; set; }
        public static BindableProperty VibrationOnDetectedProperty { get; set; }
        public bool VibrationOnDetected { get; set; }

        public static BindableProperty PreviewHeightProperty { get; set; }
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public int? PreviewHeight { get; set; }

        public static BindableProperty PreviewWidthProperty { get; set; }
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public int? PreviewWidth { get; set; }

        public static BindableProperty RequestedFPSProperty { get; set; }
        /// <summary>
        /// Only Android will be reflected this setting
        /// </summary>
        public float? RequestedFPS { get; set; }

        public static BindableProperty ScanIntervalProperty { get; set; }
        /// <summary>
        /// Only iOS will be reflected this setting, Default is 500ms, minimum value is 100ms
        /// </summary>
        public int ScanInterval { get; set; }
        public static BindableProperty IsScanningProperty { get; set; }
        /// <summary>
        /// Disables or enables scanning
        /// </summary>
        public bool IsScanning { get; set; }
        public static BindableProperty ReturnBarcodeImageProperty { get; set; }
        /// <summary>
        /// Disables or enables returning the image which the barcode was read from.
        /// </summary>
        bool ReturnBarcodeImage { get; set; }
        /// <summary>
        /// When enabled we are performing text scanning (OCR) instead of barcode scanning.
        /// </summary>
        bool IsOCR { get; set; }
        public static BindableProperty TorchOnProperty { get; set; }
        /// <summary>
        /// Disables or enables torch
        /// </summary>
        public bool TorchOn { get; set; }

        public static BindableProperty CameraFacingProperty { get; set; }
        /// <summary>
        /// Select Back or Front camera.
        /// Default value is Back Camera
        /// </summary>
        public CameraFacing CameraFacing { get; set; }
        public static BindableProperty CaptureQualityProperty { get; set; }
        /// <summary>
        /// Set the capture quality for the image analysys.
        /// Reccomended and default value is Medium.
        /// Use highest values for more precision or lower for fast scanning.
        /// </summary>
        public CaptureQuality CaptureQuality { get; set; }

        public event EventHandler<OnDetectedEventArg> OnDetected;
        public void TriggerOnDetected(List<BarcodeResult> barCodeResults, byte[] imageData);

        public void TriggerOnDetected(OCRResult ocrResult, byte[] imageData);
        public void TriggerOnDetected(OCRResult ocrResult, List<BarcodeResult> barCodeResults, byte[] imageData);

    }
}
