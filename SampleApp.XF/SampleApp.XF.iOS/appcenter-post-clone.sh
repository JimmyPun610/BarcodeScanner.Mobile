#!/usr/bin/env bash
echo "##[warning][Post-clone Action] - Lets do some Post clone transformations..."
# Calling main script in solution root
# This post clone script is to fix issue on XCode 15
# https://github.com/microsoft/appcenter/issues/2610#issuecomment-1910734646
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln list
 
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove $APPCENTER_SOURCE_DIRECTORY/SampleApp.XF/SampleApp.XF.Android/SampleApp.XF.Android.csproj
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.Maui/BarcodeScanner.Mobile.Maui.csproj
dotnet sln $APPCENTER_SOURCE_DIRECTORY/BarcodeScanner.Mobile.sln remove $APPCENTER_SOURCE_DIRECTORY/SampleApp.Maui/SampleApp.Maui.csproj
