# GoogleVisionBarCodeScanner
BarCodeScanner using GoogleVision API for Xamarin Form Android

# Android setup
1. Install Nuget package
```
Install-Package GoogleVisionBarCodeScanner.Droid -Version 1.0.0.4
```

2. Manifest.xml
```xml
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.CAMERA" />
```

3. Config scanning page and support format
```C#
GoogleVisionBarCodeScanner.Droid.Configuration.FlashlightMessage = "電筒";
GoogleVisionBarCodeScanner.Droid.Configuration.Title = "掃瞄QR";
GoogleVisionBarCodeScanner.Droid.Configuration.ScanningDescription = "掃瞄QRCode";
GoogleVisionBarCodeScanner.Droid.Configuration.BarcodeFormats = BarcodeFormat.QrCode;
```

4. Start scanning in Android Project, use DependencyServices in forms project to call it, pass to action as parameter
```C#
GoogleVisionBarCodeScanner.Droid.Operation.StartScanning(Action<string>);
```
