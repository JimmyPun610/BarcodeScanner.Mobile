# Define the files to manipulate
INFO_PLIST_FILE=${APPCENTER_SOURCE_DIRECTORY}/SampleApp.XF/SampleApp.XF.iOS/Info.plist 
  
######################## Changes on iOS
if [ -e "$INFO_PLIST_FILE" ] 
then 
    echo "##[command][Pre-Build Action] - Changing the App ID on iOS to: $APP_ID "  
    plutil -replace CFBundleIdentifier -string "$APP_ID" $INFO_PLIST_FILE
 
    echo "##[section][Pre-Build Action] - Info.plist File content:" 
    cat $INFO_PLIST_FILE 
    echo "##[section][Pre-Build Action] - Info.plist EOF" 
fi
