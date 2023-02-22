using AVFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace BarcodeScanner.Mobile.Platforms.iOS
{
    public class UICameraPreview : UIView
    {
        CameraViewHandler _cameraViewHandler;
        public UICameraPreview(AVCaptureVideoPreviewLayer layer) : base()
        {
            PreviewLayer = layer;
            PreviewLayer.Frame = Layer.Bounds;
            Layer.AddSublayer(PreviewLayer);
        }

        public readonly AVCaptureVideoPreviewLayer PreviewLayer;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            setPreviewOrientation();
            PreviewLayer.Frame = Layer.Bounds;
        }
        private void setPreviewOrientation()
        {
            var connection = PreviewLayer.Connection;
            if (connection != null)
            {
                var currentDevice = UIDevice.CurrentDevice;
                if (currentDevice.CheckSystemVersion(15, 0))
                {
                    try
                    {
                        UIInterfaceOrientation orientation = ((UIWindowScene)UIApplication.SharedApplication.ConnectedScenes.AnyObject)?.InterfaceOrientation ?? UIInterfaceOrientation.Portrait;

                        var previewLayerConnection = connection;
                        if (previewLayerConnection.SupportsVideoOrientation)
                        {
                            switch (orientation)
                            {
                                case UIInterfaceOrientation.Portrait:
                                    updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                    break;
                                case UIInterfaceOrientation.LandscapeLeft:
                                    updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeLeft);
                                    break;
                                case UIInterfaceOrientation.LandscapeRight:
                                    updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeRight);
                                    break;
                                case UIInterfaceOrientation.PortraitUpsideDown:
                                    updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.PortraitUpsideDown);
                                    break;
                                default:
                                    updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{nameof(setPreviewOrientation)}: {ex.Message}, {ex.StackTrace}");
                    }

                }
                else if (currentDevice.CheckSystemVersion(13, 0))
                {
                    UIInterfaceOrientation orientation = UIApplication.SharedApplication.Windows.FirstOrDefault()?.WindowScene?.InterfaceOrientation ?? UIInterfaceOrientation.Portrait;

                    var previewLayerConnection = connection;
                    if (previewLayerConnection.SupportsVideoOrientation)
                    {
                        switch (orientation)
                        {
                            case UIInterfaceOrientation.Portrait:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                            case UIInterfaceOrientation.LandscapeLeft:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeLeft);
                                break;
                            case UIInterfaceOrientation.LandscapeRight:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeRight);
                                break;
                            case UIInterfaceOrientation.PortraitUpsideDown:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.PortraitUpsideDown);
                                break;
                            default:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                        }
                    }
                }
                else
                {
                    // Apporach on iOS 12 or below, but this will have wrong value, please read issue #55 for more information
                    // https://github.com/JimmyPun610/BarcodeScanner.XF/issues/55
                    var orientation = currentDevice.Orientation;
                    var previewLayerConnection = connection;
                    if (previewLayerConnection.SupportsVideoOrientation)
                    {
                        switch (orientation)
                        {
                            case UIDeviceOrientation.Portrait:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                            case UIDeviceOrientation.LandscapeLeft:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeLeft);
                                break;
                            case UIDeviceOrientation.LandscapeRight:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.LandscapeRight);
                                break;
                            case UIDeviceOrientation.PortraitUpsideDown:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.PortraitUpsideDown);
                                break;
                            default:
                                updatePreviewLayer(previewLayerConnection, AVCaptureVideoOrientation.Portrait);
                                break;
                        }
                    }
                }

            }
        }
        private void updatePreviewLayer(AVCaptureConnection layer, AVCaptureVideoOrientation orientation)
        {
            layer.VideoOrientation = orientation;
        }


    }

}

