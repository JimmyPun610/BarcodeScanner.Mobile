using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVisionBarCodeScanner.Interface
{
    public interface IBarcodeScanning
    {
        void SetSupportFormat(BarcodeFormats barcodeFormats);
        void ToggleFlashlight();

        bool IsTorchOn();

        void Reset();
        void SetIsScanning(bool isScanning);
    }
}
