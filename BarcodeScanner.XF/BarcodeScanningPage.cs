using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace BarcodeScanner.XF
{
    public class BarcodeScanningPage : ContentPage
    {
        public string PageTitle = "掃瞄QRCode";
        public string FlashlightMessage = "Flashlight";
        public string ScanningDescription = "請掃瞄QRCode";
        public string CancelText = "取消";
        public event Action<string> ScannedQRCode;
        public BarcodeFormats SupportBarcodeFormat = BarcodeFormats.QRCode;
        public BarcodeScanningPage()
        {
        }
        public void SetupBarcodeScanningPage(string title, string cancelText, string scanningDesc, string flashlightMsg)
        {
            PageTitle = title;
            CancelText = cancelText;
            FlashlightMessage = flashlightMsg;
            ScanningDescription = scanningDesc;
            this.Title = PageTitle;
        }
        
    }
}