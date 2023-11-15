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
using Android.Graphics;
using Xamarin.Google.MLKit.Vision.BarCode;
using Android.Gms.Extensions;
using static BarcodeScanner.Mobile.OCRResult;

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
            foreach(var block in textResult.TextBlocks)
            {
                
                foreach (var line  in block.Lines)
                {
                    ocrResult.Lines.Add(line.Text);
                    foreach (var element in  line.Elements)
                    {
                        var ocrElement = new OCRElement();
                        ocrElement.Text = element.Text;
                        ocrElement.Confidence = element.Confidence;
                        ocrResult.Elements.Add(ocrElement);
                    }  
                }
                
            }
            ocrResult.Success = true;
            return ocrResult;
        }

        public static async Task<OCRResult> ScanFromImage(byte[] imageArray)
        {
            using Bitmap bitmap = await BitmapFactory.DecodeByteArrayAsync(imageArray, 0, imageArray.Length);
            if (bitmap == null)
                return null;
            using var image = InputImage.FromBitmap(bitmap, 0);

            using (var textScanner = TextRecognition.GetClient(Xamarin.Google.MLKit.Vision.Text.Latin.TextRecognizerOptions.DefaultOptions))
            {
                return OCRMethods.ProcessOCRResult(await textScanner.Process(image).AddOnSuccessListener(new OnSuccessListener()).AddOnFailureListener(new OnFailureListener()));
            }

        }

    }

}
