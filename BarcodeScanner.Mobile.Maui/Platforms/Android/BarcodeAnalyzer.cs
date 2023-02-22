using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using AndroidX.Camera.Core;
using Java.Nio;
using Xamarin.Google.MLKit.Vision.BarCode;
using Xamarin.Google.MLKit.Vision.Common;

namespace BarcodeScanner.Mobile.Platforms.Android
{
    public class BarcodeAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        private readonly IBarcodeScanner _barcodeScanner;
        private readonly ICameraView _cameraView;
        private long _lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long _lastAnalysisTime = DateTimeOffset.MinValue.ToUnixTimeMilliseconds();


        public BarcodeAnalyzer(ICameraView cameraView)
        {
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
                    // Pass image to the scanner and have it do its thing
                    var result = await ToAwaitableTask(_barcodeScanner.Process(image));


                    var final = Methods.ProcessBarcodeResult(result);

                    if (final == null || _cameraView == null) return;
                    if (!_cameraView.IsScanning)
                        return;

                    var imageData = new byte[0];
                    if (_cameraView.ReturnBarcodeImage)
                    {
                        imageData = NV21toJPEG(YUV_420_888toNV21(mediaImage), mediaImage.Width, mediaImage.Height);
                        imageData = RotateJpeg(imageData, GetImageRotationCorrectionDegrees());
                    }

                    _cameraView.IsScanning = false;
                    _cameraView.TriggerOnDetected(final, imageData);
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

        /// <summary>
        /// https://stackoverflow.com/a/45926852
        /// </summary>
        private static byte[] YUV_420_888toNV21(global::Android.Media.Image image)
        {
            byte[] nv21;
            ByteBuffer yBuffer = image.GetPlanes()[0].Buffer;
            ByteBuffer uBuffer = image.GetPlanes()[1].Buffer;
            ByteBuffer vBuffer = image.GetPlanes()[2].Buffer;

            int ySize = yBuffer.Remaining();
            int uSize = uBuffer.Remaining();
            int vSize = vBuffer.Remaining();

            nv21 = new byte[ySize + uSize + vSize];

            //U and V are swapped
            yBuffer.Get(nv21, 0, ySize);
            vBuffer.Get(nv21, ySize, vSize);
            uBuffer.Get(nv21, ySize + vSize, uSize);

            return nv21;
        }

        /// <summary>
        /// https://stackoverflow.com/a/45926852
        /// </summary>
        private static byte[] NV21toJPEG(byte[] nv21, int width, int height)
        {
            MemoryStream outstran = new MemoryStream();
            YuvImage yuv = new YuvImage(nv21, ImageFormatType.Nv21, width, height, null);
            yuv.CompressToJpeg(new global::Android.Graphics.Rect(0, 0, width, height), 100, outstran);
            return outstran.ToArray();
        }

        /// <summary>
        /// https://stackoverflow.com/a/44323834
        /// </summary>
        private static byte[] RotateJpeg(byte[] jpegData, int rotationDegrees)
        {
            var bmp = BitmapFactory.DecodeByteArray(jpegData, 0, jpegData.Length);
            var matrix = new Matrix();
            matrix.PostRotate(rotationDegrees);
            bmp = Bitmap.CreateBitmap(bmp, 0, 0, bmp.Width, bmp.Height, matrix, true);

            var ms = new MemoryStream();
            bmp.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
            return ms.ToArray();
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
