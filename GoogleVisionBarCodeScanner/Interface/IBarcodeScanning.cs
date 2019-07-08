using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleVisionBarCodeScanner.Interface
{
    public interface IBarcodeScanning
    {
        void SetSupportFormat(BarcodeFormats barcodeFormats);
        void ToggleFlashlight();
    }
}
