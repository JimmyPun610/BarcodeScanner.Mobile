using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GoogleVisionBarCodeScanner
{
    public class CameraView : View
    {

        public event EventHandler<OnDetectedEventArg> OnDetected;
        public void TriggerOnDetected(List<BarcodeResult> barCodeResults)
        {
            OnDetected?.Invoke(this, new OnDetectedEventArg { BarcodeResults = barCodeResults });
        }
     
    }
    
    public class OnDetectedEventArg : EventArgs
    {
        public List<BarcodeResult> BarcodeResults = new List<BarcodeResult>();
    }
}
