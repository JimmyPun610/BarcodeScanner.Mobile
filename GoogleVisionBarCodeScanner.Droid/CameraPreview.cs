using System;
using System.Collections.Generic;
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
       
        public CameraPreview(Context context)
            : base(context)
        {
            windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            barcodeDetector = new BarcodeDetector.Builder(context)
               .SetBarcodeFormats(Configuration.BarcodeFormats)
               .Build();
            cameraSource = new CameraSource
                .Builder(context, barcodeDetector)
                .SetRequestedPreviewSize(640, 480)
                .SetAutoFocusEnabled(true)
                .Build();
            Configuration.CameraSource = cameraSource;
            surfaceView = new SurfaceView(context);

            surfaceView.Holder.AddCallback(new SurfaceHolderCallback(cameraSource, surfaceView));
            AddView(surfaceView);

            var detectProcessor = new DetectorProcessor(context);
            detectProcessor.OnDetected += DetectProcessor_OnDetected;
            barcodeDetector.SetProcessor(detectProcessor);
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
            surfaceView.Layout(0, 0, r - l, r - l);

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
            public DetectorProcessor(Context context)
            {
                _context = context;
            }
            public void ReceiveDetections(Detector.Detections detections)
            {
                SparseArray qrcodes = detections.DetectedItems;
                if (qrcodes.Size() != 0)
                {
                    if (isScanning)
                    {
                        isScanning = false;
                        Vibrator vib = (Vibrator)_context.GetSystemService(Context.VibratorService);
                        vib.Vibrate(200);
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