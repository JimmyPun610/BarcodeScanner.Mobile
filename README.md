# GoogleVisionBarCodeScanner
BarCodeScanner using GoogleVision API for Xamarin Form
Currently works on android only
# Android setup
1. Install Nuget package
```
Install-Package GoogleVisionBarCodeScanner.Droid -Version 1.0.0.5
```

2. Manifest.xml
```xml
<uses-permission android:name="android.permission.VIBRATE" />
<uses-permission android:name="android.permission.CAMERA" />
```

3. Config scanning page and support format
```C#
GoogleVisionBarCodeScanner.Droid.Configuration.FlashlightMessage = "Flashlight";
GoogleVisionBarCodeScanner.Droid.Configuration.Title = "Scan QR";
GoogleVisionBarCodeScanner.Droid.Configuration.ScanningDescription = "Please scan QRCode";
GoogleVisionBarCodeScanner.Droid.Configuration.BarcodeFormats = BarcodeFormat.QrCode;
```

4. Start scanning in Android Project, use DependencyServices in forms project to call it, pass to action as parameter
```C#
GoogleVisionBarCodeScanner.Droid.Operation.StartScanning(Action<string>);
```
