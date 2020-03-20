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

 
  global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
  ...
  FaceDetection.XF.Droid.Initializer.Init();

  ...
  LoadApplication(new App());
```

## iOS Setup
1. Create an project in Google Firebase Console, download GoogleService-Info.plist
https://console.firebase.google.com/

2. Put GoogleService-Info.plist into Resources folder of iOS project,  set Build Action as BundleResource

3. Init project and firebase on AppDelegate.cs
```
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            ....
            GoogleVisionBarCodeScanner.iOS.Initializer.Init();
            Firebase.Core.App.Configure();
            ....
            return base.FinishedLaunching(app, options);
```

## Scan from Image
#### 1. Get the image byte array by your method. (You should grant permission accordingly)
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
	//Set the options first then call the methods
     FaceDetection.XF.Shared.Classes.XFVisionFaceDetectorOptions option = new 	FaceDetection.XF.Shared.Classes.XFVisionFaceDetectorOptions();
                option.PerformanceMode = FaceDetection.XF.Shared.PerformanceMode.Accurate;
                option.LandmarkMode = FaceDetection.XF.Shared.LandmarkMode.All;
                option.ClassificationMode = FaceDetection.XF.Shared.ClassificationMode.All;
                option.ContourMode = FaceDetection.XF.Shared.ContourMode.None;
                option.IsTrackingEnabled = false;
                option.MinFaceSize = 0.1;
                List<FaceDetection.XF.Shared.Classes.XFVisionFace> results = await FaceDetection.XF.Shared.Methods.DetectFaceFromImageAsync(option, bytes);
```
