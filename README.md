# BarcodeScanner.XF
Powerful barcode scanning library for Xamarin Forms using GoogleVision API. 

Works on iOS 11+ and Android (MonoAndroid10.0 and MonoAndroid11.0)

For Android, it use Xamarin.GooglePlayServices.Vision
For iOS, it use GoogleMobileVision under MLKit library

<b>For user who are using Visual Studio for Windows, please make sure Hot-Restart is disabled to run the debug mode.
https://docs.microsoft.com/en-us/xamarin/xamarin-forms/deploy-test/hot-restart</b>

Please feel free to improve my source code.

## Current Version
5.0.0.9 for Xamarin Forms 5.0.0.2012, fixed Android setOrientation crash the app

5.0.0.6 for Xamarin Forms 5.0.0.1905

4.8.2 for Xamarin Forms 4.8.0.1687+

4.8.1.1	for Xamarin Forms 4.8.0.1541+, Removed MonoAndroid 9 support, please make sure the target framework is Android 10 or 11

4.7.0.2   for Xamarin Form 4.7.0.968+

4.6.1   for Xamarin Form 4.6.0.772+, Please notice the assembly name is changed. (See sample xaml)

## Release notes
https://github.com/JimmyPun610/BarcodeScanner.XF/tree/master/ReleaseNotes

## App Center Build
##### Android [![Build status](https://build.appcenter.ms/v0.1/apps/2db667d5-b04d-46ef-99c4-3d2e91369e9b/branches/master/badge)](https://appcenter.ms)
##### iOS [![Build status](https://build.appcenter.ms/v0.1/apps/8469c6ae-f101-4f93-89ab-4504d195885d/branches/master/badge)](https://appcenter.ms)


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
            // Temporary work around for bug on Firebase Library
	    // https://github.com/xamarin/GoogleApisForiOSComponents/issues/368
	    Firebase.Core.App.Configure();
            // This line is not needed in version 5.0.5
	    // _ = Firebase.RemoteConfig.RemoteConfig.SharedInstance;
            ....
            return base.FinishedLaunching(app, options);
```

5. Set Linker Behavior to Link SDK assemblies only

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
             xmlns:gv="clr-namespace:GoogleVisionBarCodeScanner;assembly=BarcodeScanner.XF"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d"
             x:Class="SampleApp.Page1">
   <ContentPage.Content>
     <ScrollView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
	 <!--VirbationOnDetected: Indicate the device will vibrate or not when detected barcode, default is True
		 DefaultTorchOn: Indicate the torch will on or not when the view appear, default is False
		 AutoStartScanning : Indicate whether the device will start scanning after it is opened, default is True
		 RequestedFPS: Affect Android only, leave it if you want a default value (https://developers.google.com/android/reference/com/google/android/gms/vision/CameraSource.Builder.html#public-camerasource.builder-setrequestedfps-float-fps)
		 ScanInterval: Scan interval for iOS, default is 500ms and the minimum is 100ms, please be reminded that double scanning may be occurred if it is too small
		 -->
             <gv:CameraView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" OnDetected="CameraView_OnDetected" Grid.Row="1"
                            DefaultTorchOn="True" VibrationOnDetected="False" AutoStartScanning="True" RequestedFPS="30" ScanInterval="500"/>
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

#### 2. Analyze the image and wait for result
``` C#
List<GoogleVisionBarCodeScanner.BarcodeResult> obj = await GoogleVisionBarCodeScanner.Methods.ScanFromImage(bytes);
```
