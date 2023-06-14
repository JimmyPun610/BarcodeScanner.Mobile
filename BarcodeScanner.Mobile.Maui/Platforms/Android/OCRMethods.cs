using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Google.MLKit.Vision.Text;
using Android.Gms.Tasks;
using Android.Util;
using BarcodeScanner.Mobile.Platforms.Android;
using Xamarin.Google.MLKit.Vision.Common;

namespace BarcodeScanner.Mobile
{
    public class OCRMethods
    {

        public class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            public void OnSuccess(Java.Lang.Object result)
            {

            }
        }

        public class OnFailureListener : Java.Lang.Object, IOnFailureListener
        {
            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Debug(nameof(BarcodeAnalyzer), e.ToString());
            }
        }

        public static OCRResult ProcessOCRResult(Java.Lang.Object result)
        {
            var ocrResult = new OCRResult();
            var textResult = (Xamarin.Google.MLKit.Vision.Text.Text)result;

            ocrResult.AllText = textResult.GetText();
            /*foreach(var block in textResult.TextBlocks)
            {
                block.Text
            }*/
            return ocrResult;
        }

    }

}
