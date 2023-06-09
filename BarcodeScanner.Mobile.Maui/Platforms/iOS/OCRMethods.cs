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
                ocrResult.AllText += o.TopCandidates(1).First();
            }

            ocrResult.Success = true;
            return ocrResult;
        }

        public static OCRResult ScanFromImage(UIImage image)
        {
            var ocrResult = new OCRResult();
            var options = new NSDictionary();
            var ocrHandler = new VNImageRequestHandler(image.CIImage, options);
            var ocrRequest = new VNRecognizeTextRequest((request, error) =>
            {
                ocrResult = ProcessResult(request);
            });
            return ocrResult;
        }

        public static async Task<OCRResult> ScanFromImage(byte[] imageArray)
        {
            var tcs = new TaskCompletionSource<OCRResult>();
            UIImage image = new UIImage(NSData.FromArray(imageArray));
            var options = new NSDictionary();
            var ocrHandler = new VNImageRequestHandler(image.CIImage, options);
            var ocrRequest = new VNRecognizeTextRequest((request, error) =>
            {
                var ocrResult = ProcessResult(request);

                tcs.TrySetResult(ocrResult);
            });
            return await tcs.Task;

            //VisionImageMetadata metadata = new VisionImageMetadata();
            //VisionApi vision = VisionApi.Create();
            //VisionBarcodeDetector barcodeDetector = vision.GetBarcodeDetector(Configuration.BarcodeDetectorSupportFormat);
            //VisionBarcode[] barcodes = await barcodeDetector.DetectAsync(visionImage);
            /*var options = new BarcodeScannerOptions(Configuration.BarcodeDetectorSupportFormat);
            var barcodeScanner = MLKit.BarcodeScanning.BarcodeScanner.BarcodeScannerWithOptions(options);

            var tcs = new TaskCompletionSource<List<BarcodeResult>>();

            barcodeScanner.ProcessImage(visionImage, (barcodes, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Error occurred : {error}");
                    tcs.TrySetResult(null);
                    return;
                }
                if (barcodes == null || barcodes.Length == 0)
                {
                    tcs.TrySetResult(new List<BarcodeResult>());
                    return;
                }

                var s = image.Size;
                List<BarcodeResult> resultList = new List<BarcodeResult>();
                foreach (var barcode in barcodes)
                    resultList.Add(ProcessBarcodeResult(barcode));

                tcs.TrySetResult(resultList);
                return;
            });
            return await tcs.Task;*/
        }

    }
}

