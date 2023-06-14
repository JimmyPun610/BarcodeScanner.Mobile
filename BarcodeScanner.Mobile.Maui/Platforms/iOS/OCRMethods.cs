using System;
using Foundation;
using UIKit;
using Vision;


namespace BarcodeScanner.Mobile
{
	public class OCRMethods
	{

        static OCRResult ProcessResult(VNRequest result)
        {
            var ocrResult = new OCRResult();

            if (result.GetType() != typeof(VNRecognizeTextRequest))
                return ocrResult; //only interested in text

            if (result.GetResults<VNRecognizedTextObservation>().Count() < 1)
                return ocrResult;

            foreach(var o in result.GetResults<VNRecognizedTextObservation>())
            {
                var topCandidate = o.TopCandidates(1).First();
                ocrResult.AllText += " " + topCandidate.String;
                ocrResult.Lines.Add(topCandidate.String);

                topCandidate.String.Split(" ").ToList().ForEach(e =>
                {
                    ocrResult.Elements.Add(new OCRResult.OCRElement { Text = e, Confidence = topCandidate.Confidence });
                });

            }

            ocrResult.Success = true;
            return ocrResult;
        }

        public static OCRResult ScanFromImage(UIImage image)
        {
            var ocrResult = new OCRResult();
            var options = new NSDictionary();
            var ocrHandler = new VNImageRequestHandler(image.CGImage, options);
            var ocrRequest = new VNRecognizeTextRequest((request, error) =>
            {
                ocrResult = ProcessResult(request);
            });
            ocrRequest.RecognitionLevel = VNRequestTextRecognitionLevel.Accurate;
            var success = ocrHandler.Perform(new VNRequest[] { ocrRequest }, out NSError error);

            if (!success)
                ocrResult.Success = false;

            return ocrResult;
        }

        public static async Task<OCRResult> ScanFromImage(byte[] imageArray)
        {
            var tcs = new TaskCompletionSource<OCRResult>();
            var ocrResult = new OCRResult();
            UIImage image = new UIImage(NSData.FromArray(imageArray));
            var options = new NSDictionary();
            var ocrHandler = new VNImageRequestHandler(image.CGImage, options);
            var ocrRequest = new VNRecognizeTextRequest((request, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"{error.ToString()}");
                    ocrResult.Success = false;
                    tcs.TrySetResult(ocrResult);
                }

                ocrResult = ProcessResult(request);
                ocrResult.Success = true;
                tcs.TrySetResult(ocrResult);
            });
            ocrRequest.RecognitionLevel = VNRequestTextRecognitionLevel.Accurate;
            ocrHandler.Perform(new VNRequest[] { ocrRequest }, out NSError error);

            return await tcs.Task;
        }

    }
}

