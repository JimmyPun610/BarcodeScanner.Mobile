using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace GoogleVisionBarCodeScanner.Droid
{
    [Activity(Label = "BarcodeScannerActivity", NoHistory = true)]
    public class BarCodeScanningActivity : Activity
    {
        SurfaceView cameraPreview;
        TextView txtResult;
        
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        Android.Widget.Button flashLightBtn;
        const int RequestCameraPermissionID = 1001;
        bool isTorch = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.BarCodeScanningView);

            cameraPreview = FindViewById<SurfaceView>(Resource.Id.cameraPreview);
            txtResult = FindViewById<TextView>(Resource.Id.txtResult);
            flashLightBtn = FindViewById<Android.Widget.Button>(Resource.Id.flashlight_button);

            txtResult.Text = Configuration.ScanningDescription;
            flashLightBtn.Text = Configuration.FlashlightMessage;
            this.Title = Configuration.Title;

            barcodeDetector = new BarcodeDetector.Builder(this)
                .SetBarcodeFormats(Configuration.BarcodeFormats)
                .Build();
            cameraSource = new CameraSource
                .Builder(this, barcodeDetector)
                .SetRequestedPreviewSize(640, 480)
                .SetAutoFocusEnabled(true)
                .Build();
            cameraPreview.Holder.AddCallback(new SurfaceHolderCallback(cameraSource, cameraPreview));
            barcodeDetector.SetProcessor(new DetectorProcessor(txtResult, this));
            flashLightBtn.Click += FlashLightBtn_Click;
        }
        public static Android.Hardware.Camera GetCamera(CameraSource cameraSource)
        {
            var javaHero = cameraSource.JavaCast<Java.Lang.Object>();
            var fields = javaHero.Class.GetDeclaredFields();
            foreach (var field in fields)
            {
                if (field.Type.CanonicalName.Equals("android.hardware.camera", StringComparison.OrdinalIgnoreCase))
                {
                    field.Accessible = true;
                    var camera = field.Get(javaHero);
                    var cCamera = (Android.Hardware.Camera)camera;
                    return cCamera;
                }
            }

            return null;
        }


        private void FlashLightBtn_Click(object sender, EventArgs e)
        {
            var _myCamera = GetCamera(cameraSource);
            var prams = _myCamera.GetParameters();
            //prams.focus.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
            if (!isTorch)
                prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
            else prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
            isTorch = !isTorch;
            _myCamera.SetParameters(prams);
        }

        class DetectorProcessor : Java.Lang.Object, Detector.IProcessor
        {
            bool isScanning = true;
            TextView _txtResult;
            BarCodeScanningActivity _activity;
            public DetectorProcessor(TextView txtResult, BarCodeScanningActivity activity)
            {
                _txtResult = txtResult;
                _activity = activity;
            }
            public void ReceiveDetections(Detector.Detections detections)
            {
                SparseArray qrcodes = detections.DetectedItems;
                if (qrcodes.Size() != 0)
                {
                    if (isScanning)
                    {
                        isScanning = false;
                        Vibrator vib = (Vibrator)_activity.GetSystemService(Context.VibratorService);
                        vib.Vibrate(200);
                        Configuration.ScannedQRCode?.Invoke(((Barcode)qrcodes.ValueAt(0)).RawValue);
                        _activity.Finish();
                    }
                    //_txtResult.Post(() => {

                    //});
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
