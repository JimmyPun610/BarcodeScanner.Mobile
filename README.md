# BarcodeScanner.XF
BarcodeScanner using GoogleVision API for Xamarin Form
Works on Android and iOS 10+

For Android, it use Xamarin.GooglePlayServices.Vision
For iOS, it use GoogleMobileVision under MLKit library

Please feel free to improve my source code.

# Installation
1. Install Nuget package to Forms, Android and iOS project
```
Install-Package BarcodeScanner.XF -Version 2.0.1
```
# Android setup
1. Make sure Xamarin.GooglePlayServices.Vision installed
```
Install-Package Xamarin.GooglePlayServices.Vision -Version 60.1142.1
```
2. Manifest.xml
```xml
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.FLASHLIGHT" />
<uses-permission android:name="android.permission.CAMERA" />
```

3. Init the library in MainActivity.cs
```    
  base.OnCreate(savedInstanceState);
...
  GoogleVisionBarCodeScanner.Droid.RendererInitializer.Init();
...
  global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
  LoadApplication(new App());
<<<<<<< HEAD
```

# iOS Setup
1. Make sure MLKit is installed
```
Install-Package Xamarin.Firebase.iOS.MLKit -Version 0.13.0.1
Install-Package Xamarin.Firebase.iOS.MLKit.Common -Version 0.13.0
Install-Package Xamarin.Firebase.iOS.Core -Version 5.1.8
```

2. Edit Info.plist, add camera rights
```
	<key>NSCameraUsageDescription</key>
	<string>Require to use camera</string>
```

3. Create an project in Google Firebase Console, download GoogleService-Info.plist
https://console.firebase.google.com/

4. Put GoogleService-Info.plist into root folder of iOS project,  set Build Action as BundleResource

5. Init project and firebase on AppDelegate.cs
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
```
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:d="http://xamarin.com/schemas/2014/forms/design"
             xmlns:gv="clr-namespace:GoogleVisionBarCodeScanner;assembly=GoogleVisionBarCodeScanner"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d"
             x:Class="SampleApp.Page1">
   <ContentPage.Content>
     <ScrollView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
       <gv:CameraView HorizontalOptions="FillAndExpand" Margin="0,30,0,0" OnDetected="CameraView_OnDetected"/>
     </ScrollView>
   </ContentPage.Content>
</ContentPage>
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
=======
```

# iOS Setup
1. Make sure MLKit is installed
```
Install-Package Xamarin.Firebase.iOS.MLKit -Version 0.13.0.1
```

2. Edit Info.plist, add camera rights
```
	<key>NSCameraUsageDescription</key>
	<string>Require to use camera</string>
```

3. Create an project in Google Firebase Console, download GoogleService-Info.plist
https://console.firebase.google.com/

4. Put GoogleService-Info.plist into root folder of iOS project,  set Build Action as BundleResource

5. Init project and firebase on AppDelegate.cs
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
```
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:d="http://xamarin.com/schemas/2014/forms/design"
             xmlns:gv="clr-namespace:GoogleVisionBarCodeScanner;assembly=GoogleVisionBarCodeScanner"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d"
             x:Class="SampleApp.Page1">
   <ContentPage.Content>
     <ScrollView HorizontalOptions="FillAndExpand" VerticalOptions="FillAndExpand">
       <gv:CameraView HorizontalOptions="FillAndExpand" Margin="0,30,0,0" OnDetected="CameraView_OnDetected"/>
     </ScrollView>
   </ContentPage.Content>
</ContentPage>
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