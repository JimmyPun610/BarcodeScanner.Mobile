echo "##[warning][Post-clone Action] - Lets do some Post clone transformations..."
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove "$APPCENTER_SOURCE_DIRECTORY/SampleApp.XF/SampleApp.XF.Android/SampleApp.XF.Android.csproj" 
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove "$APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.Maui/BarcodeScanner.Mobile.Maui.csproj" 
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove "$APPCENTER_SOURCE_DIRECTORY/SampleApp.Maui/SampleApp.Maui.csproj" 
