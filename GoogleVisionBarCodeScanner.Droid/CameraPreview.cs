using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace GoogleVisionBarCodeScanner.Droid
{
    public sealed class CameraPreview : ViewGroup
    {
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        SurfaceView surfaceView;
        IWindowManager windowManager;
        public event Action<List<BarcodeResult>> OnDetected;
        public CameraPreview(Context context, bool defaultTorchOn, bool virbationOnDetected)
            : base(context)
        {
            windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            barcodeDetector = new BarcodeDetector.Builder(context)
               .SetBarcodeFormats(Configuration.BarcodeFormats)
               .Build();
            cameraSource = new CameraSource
                .Builder(context, barcodeDetector)
                .SetRequestedPreviewSize(1280, 720)
                .SetAutoFocusEnabled(true)
                .Build();
            Configuration.CameraSource = cameraSource;
            surfaceView = new SurfaceView(context);            
            surfaceView.Holder.AddCallback(new SurfaceHolderCallback(cameraSource, surfaceView));
            AddView(surfaceView);

            var detectProcessor = new DetectorProcessor(context, virbationOnDetected);
            detectProcessor.OnDetected += DetectProcessor_OnDetected;
            barcodeDetector.SetProcessor(detectProcessor);
            if (defaultTorchOn)
                AutoSwitchOnTorch();
        }

        private void AutoSwitchOnTorch()
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

            surfaceView.Measure(msw, msh);
            surfaceView.Layout(0, 0, r - l, b - t);

            SetOrientation();
        }


        public void SetOrientation()
        {

            Android.Hardware.Camera camera = Methods.GetCamera(cameraSource);
            switch (windowManager.DefaultDisplay.Rotation)
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
        }


     


        class DetectorProcessor : Java.Lang.Object, Detector.IProcessor
        {
            bool isScanning = true;
            public event Action<List<BarcodeResult>> OnDetected;
            Context _context;
            bool _vibrationOnDetected = true;
            public DetectorProcessor(Context context, bool vibrationOnDetected)
            {
                _context = context;
                _vibrationOnDetected = vibrationOnDetected;
            }
            public void ReceiveDetections(Detector.Detections detections)
            {
                SparseArray qrcodes = detections.DetectedItems;
                if (qrcodes.Size() != 0)
                {
                    if (isScanning)
                    {
                        isScanning = false;
                        if (_vibrationOnDetected)
                        {
                            Vibrator vib = (Vibrator)_context.GetSystemService(Context.VibratorService);
                            vib.Vibrate(200);
                        }
                        List<BarcodeResult> barcodeResults = new List<BarcodeResult>();
                        for(int i = 0; i < qrcodes.Size(); i++)
                        {
                            Barcode barcode = qrcodes.ValueAt(i) as Barcode;
                            var type = Methods.ConvertBarcodeResultTypes(barcode.ValueFormat);
                            var value = barcode.DisplayValue;
                            barcodeResults.Add(new BarcodeResult
                            {
                                BarcodeType = type,
                                DisplayValue = value
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

        class SurfaceHolderCallback : Java.Lang.Object, ISurfaceHolderCallback
        {
            SurfaceView _cameraPreview;
            CameraSource _cameraSource;
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
                catch (InvalidOperationException)
                {

                }
            }

            public void SurfaceDestroyed(ISurfaceHolder holder)
            {
                _cameraSource.Stop();
            }
        }
    }
}