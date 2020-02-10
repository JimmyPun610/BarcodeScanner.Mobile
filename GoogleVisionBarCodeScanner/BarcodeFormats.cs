using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleVisionBarCodeScanner
{
    public enum BarcodeFormats
    {
        //UnKnown = 0,
        Code128 = 1,
        Code39 = 2,
        Code93 = 4,
        CodaBar = 8,
        DataMatrix = 16,
        Ean13 = 32,
        Ean8 = 64,
        Itf = 128,
        QRCode = 256,
        Upca = 512,
        Upce = 1024,
        Pdf417 = 2048,
        Aztec = 4096,
        All = 65535,
    }
}
