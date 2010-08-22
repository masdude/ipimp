!define LANG "ENGLISH"

!insertmacro LANG_STRING STRING_YES "Yes"
!insertmacro LANG_STRING STRING_NO "No"

!insertmacro LANG_STRING STRING_BRANDING "iPiMP by Cheezey - Manage MediaPortal from your iPhone"
!insertmacro LANG_STRING STRING_NOWIKI "You haven't read the wiki, do you want to continue?"
!insertmacro LANG_STRING STRING_BROWSE "Browse..."

!insertmacro LANG_STRING STRING_WELCOME_TITLE "Welcome to iPiMP!"
!insertmacro LANG_STRING STRING_WELCOME_LINE1 "iPiMP provides web access to various MediaPortal components through a nice iPhone interface.$\r$\n$\r$\nWith iPiMP you can view your TV guides, schedule recordings, watch recordings, manage recordings and schedules and remotely control MediaPortal clients."
!insertmacro LANG_STRING STRING_WELCOME_LINE2 "Please read the wiki for full installation and usage instructions."
!insertmacro LANG_STRING STRING_WELCOME_LINE3 "Perform advanced install"

!insertmacro LANG_STRING STRING_LICENCE_LINE1 "Welcome to iPiMP!"
!insertmacro LANG_STRING STRING_LICENCE_AGREE "I Agree"

!insertmacro LANG_STRING STRING_CHECKPREVIOUS_LINE1 "Another version of ${PRODUCT_NAME} is already installed. Would you like to uninstall it?"
!insertmacro LANG_STRING STRING_CHECKPREVIOUS_LINE2 "Previous version uninstall failed - Reason: $2"
!insertmacro LANG_STRING STRING_CHECKPREVIOUS_LINE3 "It is recommended that you uninstall any previous versions before installing ${PRODUCT_NAME} ${PRODUCT_VERSION}, would you like to uninstall the previous version?"
!insertmacro LANG_STRING STRING_CHECKPREVIOUS_LINE4 "OK - you may experience problems with ${PRODUCT_NAME} ${PRODUCT_VERSION}, please do NOT report these."

!insertmacro LANG_STRING STRING_CHECKVERSION_LINE1 "MediaPortal TV Server ${MIN_MPVERSION} or greater must be installed $\r$\nto install this component."
!insertmacro LANG_STRING STRING_CHECKVERSION_LINE2 "MediaPortal Client ${MIN_MPVERSION} or greater must be installed $\r$\nto install this component."

!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE1 "Standard iPiMP installs"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE2 "Singleseat installation"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE3 "Dedicated TV server installation"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE4 "MediaPortal remote control plugin"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE5 "MediaPortal remote control web installation"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE6 "Select your option..."
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE7 "This is the correct setup if you have the MediaPortal client and TV server installed on the same PC.$\r$\n$\r$\nThis option installs everything!"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE8 "This is the correct setup if you have a standalone TV server.$\r$\n$\r$\nThis option installs everything except the client plugin."
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE9 "This is the right setup if you have the MediaPortal client only installed and want to remotely control it from your iPhone and have the iPiMP web server running on another PC.$\r$\n$\r$\nFor example in a multi seat MediaPortal setup."
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE10 "This is the right setup if you have the MediaPortal client only installed and want to remotely control it from your iPhone and want the iPiMP web server running on the same PC.$\r$\n$\r$\nFor example if you don't use the TV server."
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE11 "Cannot detect MediaPortal client or TV Server!"
!insertmacro LANG_STRING STRING_SIMPLEINSTALL_LINE12 "You have to select one installation type!"

!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE1 "Advanced iPiMP installs"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE2 "Apache version"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE3 "Apache mod_aspdotnet"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE4 "iPiMP web files"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE5 "iPiMP transcode TV server plugin"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE6 "iPiMP MediaPortal client remote control plugin"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE7 "Select your options..."
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE8 "Installs a default Apache HTTP server version 2.2.14 (released on 05/10/2009).$\r$\n$\r$\nAlso installs mod_aspdotnet, a loadable Apache 2 module for serving ASP.NET content using Microsoft's ASP.NET hosting and .NET runtime within the Apache HTTP server process."
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE9 "Installs mod_aspdotnet, a loadable Apache 2 module for serving ASP.NET content using Microsoft's ASP.NET hosting and .NET runtime within the Apache HTTP server process."
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE10 "Installs the iPiMP ASP.NET application files."
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE11 "Installs a TV Server plugin to transcode recordings into iPhone compatible MP4 files, an iPiMP transcode client is also provided for manual transcoding."
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE12 "Installs a MediaPortal client plugin to allow remote control from the iPiMP web application."
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE13 "Cannot detect MediaPortal client or TV Server!"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE14 "You're not that advanced then....$\r$\n$\r$\nYou have to select one installation option!"
!insertmacro LANG_STRING STRING_ADVANCEDINSTALL_LINE15 "You're installing the iPiMP web files but not the Apache web server, this assumes you have your own web server and can configure it to run iPiMP.$\r$\n$\r$\nIs this correct?"

!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE1 "Apache web server settings"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE2 "Enter the TCP port to listen on:$\r$\n(This will default to port 80 if left blank)"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE3 "Enter the IP address to listen on:$\r$\n(This will default to all IPs if left blank)"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE4 "Confirm the TV logos folder:$\r$\n(These will be used in the web interface)"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE5 "Select a login timeout:$\r$\n(After that period you'll have to re-login)"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE6 "30 mins"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE7 "1 hour"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE8 "8 hours"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE9 "1 day"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE10 "1 week"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE11 "1 month"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE12 "1 year"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE13 "Select a size for long lists:$\r$\n(To make long lists more manageable.)"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE14 "Locate your TV logo folder."
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE15 "$IPAddress is not a valid ip address."
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE16 "Cannot find $LogoPath"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE17 "No TV logo path provided."
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE18 "Locate your Radio logo folder."
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE16 "Cannot find $RadioLogoPath"
!insertmacro LANG_STRING STRING_APACHEINSTALL_LINE17 "No Radio logo path provided."

!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE1 "Transcode plugin settings"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE2 "Do you want to delete transcoded files when a recording is deleted in MP?"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE3 "Do you want to transcode immediately?$\r$\n(No will transcode on a schedule)"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE4 "What time do you want to transcode?$\r$\n(Select never to transcode on demand)"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE5 "never"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE6 "Select the required video bitrate:"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE7 "Select the required audio bitrate:"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE8 "Select the folder to store the MP4s:"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE9 "Browse..."
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE10 "Cannot find $MP4Path"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE11 "No MP4 save path provided."
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE12 "Select the transcoding utility"
!insertmacro LANG_STRING STRING_TVPLUGININSTALL_LINE13 "Your TV service will be restarted to enable the transcode plugin.$\nDo you want to continue?"

!insertmacro LANG_STRING STRING_INSTALLLOCATION_LINE1 "iPiMP installation folder"
!insertmacro LANG_STRING STRING_INSTALLLOCATION_LINE2 "Select the folder in which to install any iPiMP files, this includes the uninstaller."
!insertmacro LANG_STRING STRING_INSTALLLOCATION_LINE3 "Select the install folder."
!insertmacro LANG_STRING STRING_INSTALLLOCATION_LINE4 "$INSTDIR already exists, are you sure you want to install there?"

!insertmacro LANG_STRING STRING_WEBFILESINSTALL_LINE1 "Installing iPiMP web files"
!insertmacro LANG_STRING STRING_WEBFILESINSTALL_LINE2 "Backing up iPiMP databases"
!insertmacro LANG_STRING STRING_WEBFILESINSTALL_LINE3 "Removing iPiMP web files"

!insertmacro LANG_STRING STRING_APACHESERVICE_LINE1 "iPiMP Apache service installation failed - Reason: It is already installed!"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE2 "iPiMP Apache service installation failed - Reason: There is an error in httpd.conf!"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE3 "iPiMP Apache service installation failed - Reason: $0"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE4 "iPiMP Apache service start failed - Reason: There is an error in httpd.conf!"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE5 "iPiMP Apache service start failed - Reason: $0"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE6 "Installing Apache service"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE7 "Starting Apache service"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE8 "Stopping Apache service"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE9 "Removing Apache service"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE10 "iPiMP Apache service stop failed - Reason: There is an error in httpd.conf!"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE11 "iPiMP Apache service stop failed - Reason: $0"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE12 "iPiMP Apache service removal failed - Reason: It is already installed!"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE13 "iPiMP Apache service removal failed - Reason: There is an error in httpd.conf!"
!insertmacro LANG_STRING STRING_APACHESERVICE_LINE14 "iPiMP Apache service removal failed - Reason: $0"

!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE1 "Installing TV Server plugin"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE2 "Configuring TV Server plugin"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE3 "Plugin config utility failed - use SetupTV to configure."
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE4 "Removing TV Server plugin"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE5 "Un-configuring TV Server plugin"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE6 "Plugin config removal failed."
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE7 "Stopping TV service"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE8 "TV Service stop service failed - Reason: $0"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE9 "Starting TV service"
!insertmacro LANG_STRING STRING_TVSERVICEPLUGIN_LINE10 "TV Service start service failed - Reason: $0"

!insertmacro LANG_STRING STRING_MPCLIENTPLUGIN_LINE1 "Installing MediaPortal client plugin"
!insertmacro LANG_STRING STRING_MPCLIENTPLUGIN_LINE2 "Removing MediaPortal client plugin"

!insertmacro LANG_STRING STRING_INIT_LINE1 "Are you sure you want to completely remove $(^Name) and all of its components?"
!insertmacro LANG_STRING STRING_INIT_LINE2 "Selecting 'Yes' will stop your TV service briefly!!!"
!insertmacro LANG_STRING STRING_INIT_LINE3 "Please ensure MediaPortal is NOT running."
