#!/usr/bin/env bash
echo "##[warning][Post-clone Action] - Lets do some Post clone transformations..."
# Calling main script in solution root
# This post clone script is to fix issue on XCode 15
# https://github.com/microsoft/appcenter/issues/2610#issuecomment-1910734646
ROOT_FOLDER=$APPCENTER_SOURCE_DIRECTORY/

dotnet sln $ROOT_FOLDER/BarcodeScanner.Mobile.sln remove $ROOT_FOLDER/BarcodeScanner.Mobile.Maui/BarcodeScanner.Mobile.Maui.csproj
dotnet sln $ROOT_FOLDER/BarcodeScanner.Mobile.sln remove $ROOT_FOLDER/SampleApp.Maui/SampleApp.Maui.csproj
dotnet sln $ROOT_FOLDER/BarcodeScanner.Mobile.sln remove $ROOT_FOLDER/SampleApp.XF/SampleApp.XF.Android/SampleApp.XF.Android.csproj
