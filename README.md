# BarcodeScanner.XF
BarcodeScanner using GoogleVision API for Xamarin Forms
Works on Android and iOS 10+

For Android, it use Xamarin.GooglePlayServices.Vision
For iOS, it use GoogleMobileVision under MLKit library

Please feel free to improve my source code.

## Current Version
4.2.2.3

## Release notes
https://github.com/JimmyPun610/BarcodeScanner.XF/tree/master/ReleaseNotes

## Installation
1. Install Nuget package to Forms, Android and iOS project
```
Install-Package BarcodeScanner.XF
```

## Android setup
1. Manifest.xml
```xml
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.FLASHLIGHT" />
<uses-permission android:name="android.permission.CAMERA" />
```

2. Init the library in MainActivity.cs
```    
  base.OnCreate(savedInstanceState);
...
  GoogleVisionBarCodeScanner.Droid.RendererInitializer.Init();
...
  global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
  LoadApplication(new App());
```
```
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```
## iOS Setup
1. Edit Info.plist, add camera rights
```
	<key>NSCameraUsageDescription</key>
	<string>Require to use camera</string>
```

2. Create an project in Google Firebase Console, download GoogleService-Info.plist
https://console.firebase.google.com/

3. Put GoogleService-Info.plist into Resources folder of iOS project,  set Build Action as BundleResource

4. Init project and firebase on AppDelegate.cs
```
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            ....
            GoogleVisionBarCodeScanner.iOS.Initializer.Init();
            Firebase.Core.App.Configure();
            ....
            return base.FinishedLaunching(app, options);
```

## Scan from Camera

#### 1. Set support barcode format (Default is all), call it before you start to init CameraView
```C#
GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);
```

#### 2. It is all about the camera view, use it on the page.xaml. For now, it will spend your whole width of the screen and the height will be equal to width.


```XAML
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:d="http://xamarin.com/schemas/2014/forms/design"
             xmlns:gv="clr-namespace:GoogleVisionBarCodeScanner;assembly=GoogleVisionBarCodeScanner"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d"
             x:Class="SampleApp.Page1">
   <ContentPage.Content>
     <ScrollView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
	 <!--VirbationOnDetected: Indicate the device will vibrate or not when detected barcode, default is True
		 DefaultTorchOn: Indicate the torch will on or not when the view appear, default is False
		 AutoStartScanning : Indicate whether the device will start scanning after it is opened, default is True-->
             <gv:CameraView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" OnDetected="CameraView_OnDetected" Grid.Row="1"
                            DefaultTorchOn="True" VibrationOnDetected="False" AutoStartScanning="True"/>
     </ScrollView>
   </ContentPage.Content>
</ContentPage>
```
#### 3. Once barcode detected, "OnDetected" event will be triggered, do the stuff you want with the barcode, it will contains type and display value
```C#
   private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> obj = e.BarcodeResults;

            string result = string.Empty;
            for(int i = 0; i < obj.Count; i++)
            {
                result += $"{i + 1}. Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
            }
            Device.BeginInvokeOnMainThread(async() =>
            {
                await DisplayAlert("Result", result, "OK");
		//If you want to stop scanning, you can close the scanning page
                await Navigation.PopModalAsync();
		//if you want to keep scanning the next barcode, do not close the scanning page and call below function
		//GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            });
            
        }
```

#### 4. To use torch, please call 
```C#
   GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
```

#### 5. To ask for permission
```C#
bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
```

#### 6. To check the condition of torch
```C#
   GoogleVisionBarCodeScanner.Methods.IsTorchOn();
```
#### 7. Restart scanning
```C#
//Old method, you can use the new one.
//GoogleVisionBarCodeScanner.Methods.Reset();
GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
```


## Scan from Image
#### 1. Get the image byte array by your method.
``` C#
//Used MediaPlugin in sample for example
 var file = await CrossMedia.Current.PickPhotoAsync();
Stream stream = file.GetStream();
byte[] bytes = new byte[stream.Length];
stream.Read(bytes, 0, bytes.Length);
stream.Seek(0, SeekOrigin.Begin);
```

#### 2. Analysis the image and wait for result
``` C#
List<GoogleVisionBarCodeScanner.BarcodeResult> obj = await GoogleVisionBarCodeScanner.Methods.ScanFromImage(bytes);
```
