using Android.Runtime;
using Android.Util;
using AndroidX.Camera.Core;
using AndroidX.Camera.Core.Internal.Utils;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using static BarcodeScanner.Mobile.OCRMethods;
using Size = Android.Util.Size;

namespace BarcodeScanner.Mobile
{
    public class BarcodeAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        private readonly IBarcodeScanner _barcodeScanner;
        private readonly ICameraView _cameraView;
        private long _lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long _lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();
        private Action _barcodeDisposed;


        public BarcodeAnalyzer(ICameraView cameraView, Action barcodeDisposed)
        {
            _barcodeDisposed = barcodeDisposed;
            _cameraView = cameraView;
            if (_cameraView != null && _cameraView.ScanInterval < 100)
                _cameraView.ScanInterval = 500;

            _barcodeScanner = BarcodeScanning.GetClient(new BarcodeScannerOptions.Builder().SetBarcodeFormats(
                Configuration.BarcodeFormats)
            .Build());

        }

        public async void Analyze(IImageProxy proxy)
        {
            try
            {
                var mediaImage = proxy.Image;
                if (mediaImage == null) return;

                _lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                if (_lastRunTime - _lastAnalysisTime > _cameraView.ScanInterval && _cameraView.IsScanning)
                {
                    _lastAnalysisTime = _lastRunTime;
                    var image = InputImage.FromMediaImage(mediaImage, proxy.ImageInfo.RotationDegrees);

                    List<BarcodeResult> barcodeFinalResult = null;
                    OCRResult ocrFinalResult = null;

                    // Pass image to the scanner and have it do its thing
                    if (_cameraView.IsOCR)
                    {
                        using (var textScanner = TextRecognition.GetClient(Xamarin.Google.MLKit.Vision.Text.Latin.TextRecognizerOptions.DefaultOptions))
                        {
                            var result = await ToAwaitableTask(textScanner.Process(image).AddOnSuccessListener(new OnSuccessListener()).AddOnFailureListener(new OnFailureListener()));
                            ocrFinalResult = OCRMethods.ProcessOCRResult(result);
                        }
                    }
                    else
                    {
                        var result = await ToAwaitableTask(_barcodeScanner.Process(image));
                        barcodeFinalResult = Methods.ProcessBarcodeResult(result);
                        if (barcodeFinalResult == null || _cameraView == null) return;
                    }

                    if (!_cameraView.IsScanning)
                        return;

                    var imageData = Array.Empty<byte>();
                    if (_cameraView.ReturnBarcodeImage)
                        imageData = ImageUtil.YuvImageToJpegByteArray(proxy, new Android.Graphics.Rect(0, 0, mediaImage.Width, mediaImage.Height), 100, GetImageRotationCorrectionDegrees());

                    _cameraView.IsScanning = false;
                    _cameraView.TriggerOnDetected(ocrFinalResult, barcodeFinalResult, imageData);
                    if (_cameraView.VibrationOnDetected)
                        Vibration.Vibrate(200);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Debug(nameof(BarcodeAnalyzer), ex.ToString());
            }
            catch (Exception ex)
            {
                Log.Debug(nameof(BarcodeAnalyzer), ex.ToString());
            }
            finally
            {
                SafeCloseImageProxy(proxy);
            }
        }

        private static int GetImageRotationCorrectionDegrees()
        {
            bool isAutoRotateEnabled = global::Android.Provider.Settings.System.GetInt(global::Android.App.Application.Context.ContentResolver,
                global::Android.Provider.Settings.System.AccelerometerRotation, 0) == 1;

            if (!isAutoRotateEnabled)
                return 90;

            global::Android.Views.IWindowManager windowManager = global::Android.App.Application.Context.GetSystemService(global::Android.Content.Context.WindowService).JavaCast<global::Android.Views.IWindowManager>();

            switch (windowManager.DefaultDisplay.Rotation)
            {
                case global::Android.Views.SurfaceOrientation.Rotation0:
                    return 90;
                case global::Android.Views.SurfaceOrientation.Rotation90:
                    return 0;
                case global::Android.Views.SurfaceOrientation.Rotation180:
                    return -90;
                case global::Android.Views.SurfaceOrientation.Rotation270:
                    return 180;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Fix for https://github.com/xamarin/AndroidX/issues/767
        /// </summary>
        public Size DefaultTargetResolution => _cameraView.CaptureQuality.GetTargetResolution();

        private void SafeCloseImageProxy(IImageProxy proxy)
        {
            try
            {
                proxy?.Close();
            }
            catch (ObjectDisposedException) { }
            catch (ArgumentException)
            {
                //Ignore argument exception, it will be thrown if BarcodeAnalyzer get disposed during processing
                _barcodeDisposed?.Invoke();
            }
        }

        private Task<Java.Lang.Object> ToAwaitableTask(global::Android.Gms.Tasks.Task task)
        {
            var taskCompletionSource = new TaskCompletionSource<Java.Lang.Object>();
            var taskCompleteListener = new TaskCompleteListener(taskCompletionSource);
            task.AddOnCompleteListener(taskCompleteListener);

            return taskCompletionSource.Task;
        }
    }
}
