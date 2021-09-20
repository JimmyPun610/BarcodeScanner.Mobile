using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace GoogleVisionBarCodeScanner
{
    internal sealed class CameraPreview : ViewGroup
    {
        private readonly BarcodeDetector _barcodeDetector;
        private readonly CameraSource _cameraSource;
        private readonly SurfaceView _surfaceView;
        private readonly IWindowManager _windowManager;
        public event Action<List<BarcodeResult>> OnDetected;

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            //Off the torch when exit page
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
        }


        public CameraPreview(Context context, bool defaultTorchOn, bool virbationOnDetected, bool startScanningOnCreate, float? requestedFPS)
            : base(context)
        {
            Configuration.IsScanning = startScanningOnCreate;
            _windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            _barcodeDetector = new BarcodeDetector.Builder(context)
               .SetBarcodeFormats(Configuration.BarcodeFormats)
               .Build();
            if(requestedFPS == null)
            {
                _cameraSource = new CameraSource
                .Builder(context, _barcodeDetector)
                .SetRequestedPreviewSize(1280, 720)
                .SetAutoFocusEnabled(true)
                .Build();
            }
            else
            {
                _cameraSource = new CameraSource
                .Builder(context, _barcodeDetector)
                .SetRequestedPreviewSize(1280, 720)
                .SetAutoFocusEnabled(true)
                .SetRequestedFps(requestedFPS.Value)
                .Build();
            }
            
            Configuration.CameraSource = _cameraSource;
            _surfaceView = new SurfaceView(context);
            _surfaceView.Holder.AddCallback(new SurfaceHolderCallback(_cameraSource, _surfaceView));
            AddView(_surfaceView);

            var detectProcessor = new DetectorProcessor(context, virbationOnDetected);
            detectProcessor.OnDetected += DetectProcessor_OnDetected;
            _barcodeDetector.SetProcessor(detectProcessor);
            if (defaultTorchOn)
                AutoSwitchOnTorch();
        }

        private static void AutoSwitchOnTorch()
        {
            var ts = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken ct = ts.Token;
            Task.Factory.StartNew(async () =>
            {
                bool isTorchOn = false;
                do
                {
                    try
                    {
                        isTorchOn = GoogleVisionBarCodeScanner.Methods.IsTorchOn();
                        if (!isTorchOn)
                        {
                            //Try to switch on the torch
                            GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
                            //break the loop if the torch on succesfully
                            isTorchOn = GoogleVisionBarCodeScanner.Methods.IsTorchOn();
                            if (isTorchOn)
                                break;
                            else
                            {
                                //Wait 500ms to run the loop again
                                await Task.Delay(500);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //May be the view is not loaded
                        //Wait 500ms to run the loop again
                        await Task.Delay(500);
                    }
                } while (!isTorchOn);
                //Stop the task
                ts.Cancel();


            }, ct);
        }

        private void DetectProcessor_OnDetected(List<BarcodeResult> obj)
        {
            OnDetected?.Invoke(obj);

        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            _surfaceView.Measure(msw, msh);
            _surfaceView.Layout(0, 0, r - l, b - t);

            SetOrientation();
        }


        public void SetOrientation()
        {

            Android.Hardware.Camera camera = Methods.GetCamera(_cameraSource);
            camera?.StopPreview();
            switch (_windowManager.DefaultDisplay.Rotation)
            {
                case SurfaceOrientation.Rotation0:
                    camera?.SetDisplayOrientation(90);
                    break;
                case SurfaceOrientation.Rotation90:
                    camera?.SetDisplayOrientation(0);
                    break;
                case SurfaceOrientation.Rotation180:
                    camera?.SetDisplayOrientation(270);
                    break;
                case SurfaceOrientation.Rotation270:
                    camera?.SetDisplayOrientation(180);
                    break;
            }
            camera?.StartPreview();
        }


        private class DetectorProcessor : Java.Lang.Object, Detector.IProcessor
        {
            public event Action<List<BarcodeResult>> OnDetected;
            private readonly Context _context;
            private readonly bool _vibrationOnDetected;

            public DetectorProcessor(Context context, bool vibrationOnDetected)
            {
                _context = context;
                _vibrationOnDetected = vibrationOnDetected;
            }

            public void ReceiveDetections(Detector.Detections detections)
            {
                var qrcodes = detections.DetectedItems;
                if (qrcodes.Size() != 0)
                {
                    if (Configuration.IsScanning)
                    {
                        Configuration.IsScanning = false;
                        if (_vibrationOnDetected)
                        {
                            Vibrator vib = (Vibrator)_context.GetSystemService(Context.VibratorService);
                            vib.Vibrate(200);
                        }
                        List<BarcodeResult> barcodeResults = new List<BarcodeResult>();
                        for (int i = 0; i < qrcodes.Size(); i++)
                        {
                            Barcode barcode = qrcodes.ValueAt(i) as Barcode;
                            if (barcode == null) continue;
                            var type = Methods.ConvertBarcodeResultTypes(barcode.ValueFormat);
                            var value = barcode.DisplayValue;
                            var rawValue = barcode.RawValue;
                            barcodeResults.Add(new BarcodeResult
                            {
                                BarcodeType = type,
                                DisplayValue = value,
                                RawValue = rawValue
                            });
                        }
                        OnDetected?.Invoke(barcodeResults);
                    }
                }
            }

            public void Release()
            {
            }
        }

        private class SurfaceHolderCallback : Java.Lang.Object, ISurfaceHolderCallback
        {
            private readonly SurfaceView _cameraPreview;
            private readonly CameraSource _cameraSource;

            public SurfaceHolderCallback(CameraSource cameraSource, SurfaceView cameraPreview)
            {
                _cameraSource = cameraSource;
                _cameraPreview = cameraPreview;
            }


            public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
            {

            }

            public void SurfaceCreated(ISurfaceHolder holder)
            {
                try
                {

                    _cameraSource.Start(_cameraPreview.Holder);
                }
                catch (Exception e)
                {
                    Log.Error("BarcodeScanner.Droid", e.Message);
                }
            }

            public void SurfaceDestroyed(ISurfaceHolder holder)
            {
                _cameraSource.Stop();
            }
        }
    }
}
