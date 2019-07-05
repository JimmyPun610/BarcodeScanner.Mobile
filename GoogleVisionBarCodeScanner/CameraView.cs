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
        public event EventHandler ViewSizeChanged;
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            ViewSizeChanged?.Invoke(this, new EventArgs());
        }
    }
    
    public class OnDetectedEventArg : EventArgs
    {
        public List<BarcodeResult> BarcodeResults = new List<BarcodeResult>();
    }
}
