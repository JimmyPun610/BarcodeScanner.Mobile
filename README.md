# BarcodeScanner.XF
BarcodeScanner using GoogleVision API for Xamarin Form
Works on Android and iOS 10+

For Android, it use Xamarin.GooglePlayServices.Vision
For iOS, it use GoogleMobileVision under MLKit library

Please feel free to improve my source code.

# Pending to fix
1. CameraView is not shown in Android 10 device

# Update on 4.2.1
1. Added ask permission methods
2. Added IsTorchOn methods
3. Added DefaultTorchOn property
4. Move some code on iOS to UIThread

# Installation
1. Install Nuget package to Forms, Android and iOS project
```
Install-Package BarcodeScanner.XF
```

# Android setup
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
  Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
...
  global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
  LoadApplication(new App());
```
```
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```
# iOS Setup
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


# Usage

1. Set support barcode format (Default is all), call it before you start to init CameraView
```
GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);
```

2. It is all about the camera view, use it on the page.xaml. For now, it will spend your whole width of the screen and the height will be equal to width.

#### VirbationOnDetected: Indicate the device will vibrate or not when detected barcode, default is True
#### DefaultTorchOn: Indicate the torch will on or not when the view appear, default is False
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
	 <!--If true on DefaultTorchOn, Torch will be on when the UI loaded-->
             <gv:CameraView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand" OnDetected="CameraView_OnDetected" Grid.Row="1"
                            DefaultTorchOn="True" VirbationOnDetected="False"/>
     </ScrollView>
   </ContentPage.Content>
</ContentPage>
```
#### In the page.cs, below code are making sure that torch is off when leaving
```C#
  protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //Off the torch when exit page
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
        }
```

3. Once barcode detected, "OnDetected" event will be triggered, do the stuff you want with the barcode, it will contains type and display value
```
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
                await Navigation.PopModalAsync();
            });
            
        }
```

4. To use torch, please call 
```
   GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
```

5. To ask for permission
```
bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
```

6. To check the condition of torch
```
   GoogleVisionBarCodeScanner.Methods.IsTorchOn();
```
