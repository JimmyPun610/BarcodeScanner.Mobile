echo "##[warning][Post-clone Action] - Lets do some Post clone transformations..."
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove "../../SampleApp.XF/SampleApp.XF.Android/SampleApp.XF.Android.csproj"
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove "../../BarcodeScanner.Mobile.Maui/BarcodeScanner.Mobile.Maui.csproj"
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove "../../SampleApp.Maui/SampleApp.Maui.csproj"
