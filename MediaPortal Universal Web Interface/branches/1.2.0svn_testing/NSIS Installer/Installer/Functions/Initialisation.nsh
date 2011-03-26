Function .onInit

  InitPluginsDir
  CreateFont $Headline_font "Tahoma" "14" "700"
  File /oname=$PLUGINSDIR\ipimp.bmp "Images\iPiMPinstall.bmp"
  File /oname=$PLUGINSDIR\ipimpapache.bmp "Images\iPiMPapache.bmp"
  File /oname=$PLUGINSDIR\ipimpgeek.bmp "Images\iPiMPgeek.bmp"
  File /oname=$PLUGINSDIR\GPL3.txt "GPL3.txt"

  UAC_Elevate:
    !insertmacro UAC_RunElevated
    StrCmp 1223 $0 UAC_ElevationAborted ; UAC dialog aborted by user?
    StrCmp 0 $0 0 UAC_Err ; Error?
    StrCmp 1 $1 0 UAC_Success ;Are we the real deal or just the wrapper?
    Quit

  UAC_Err:
    MessageBox mb_iconstop "Unable to elevate, error $0"
    Abort

  UAC_ElevationAborted:
    # elevation was aborted, run as normal?
    MessageBox mb_iconstop "This installer requires admin access, aborting!"
    Abort

  UAC_Success:
    StrCmp 1 $3 +4 ;Admin?
    StrCmp 3 $1 0 UAC_ElevationAborted ;Try again?
    MessageBox mb_iconstop "This installer requires admin access, try again"
    goto UAC_Elevate
    
  ReadRegStr $ServerPath HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "InstallPath"
  ReadRegStr $TVServerMaj HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "VersionMajor"
  ReadRegStr $TVServerMin HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "VersionMinor"
  ReadRegStr $TVServerRev HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "VersionRevision"
  ReadRegStr $MPClientMaj HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "VersionMajor"
  ReadRegStr $MPClientMin HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "VersionMinor"
  ReadRegStr $MPClientRev HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "VersionRevision"
  ReadRegStr $ClientPath HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "InstallPath"
  ReadRegStr $CommonApp HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\explorer\Shell Folders" "Common AppData"
  ReadRegStr $DotNetDir HKLM "SOFTWARE\Microsoft\.NETFramework" "InstallRoot"

  StrCpy $LogoPath "$CommonApp\Team MediaPortal\MediaPortal\thumbs\tv\logos"
  StrCpy $RadioLogoPath "$CommonApp\Team MediaPortal\MediaPortal\thumbs\radio"
  StrCpy $TVPath "$CommonApp\Team MediaPortal\MediaPortal TV Server"
  StrCpy $TVServerVer "$TVServerMaj.$TVServerMin.$TVServerRev"
  StrCpy $MPClientVer "$MPClientMaj.$MPClientMin.$MPClientRev"

FunctionEnd

Function un.onInit

  UAC_Elevate:
    !insertmacro UAC_RunElevated
    StrCmp 1223 $0 UAC_ElevationAborted ; UAC dialog aborted by user?
    StrCmp 0 $0 0 UAC_Err ; Error?
    StrCmp 1 $1 0 UAC_Success ;Are we the real deal or just the wrapper?
    Quit

  UAC_Err:
    MessageBox mb_iconstop "Unable to elevate, error $0"
    Abort

  UAC_ElevationAborted:
    # elevation was aborted, run as normal?
    MessageBox mb_iconstop "This installer requires admin access, aborting!"
    Abort

  UAC_Success:
    StrCmp 1 $3 +4 ;Admin?
    StrCmp 3 $1 0 UAC_ElevationAborted ;Try again?
    MessageBox mb_iconstop "This installer requires admin access, try again"
    goto UAC_Elevate
    
  ReadRegStr $InstallApache ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "InstallApache"
  ReadRegStr $InstallModAspNet ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "InstallModAspNet"
  ReadRegStr $InstalliPiMPWeb ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "InstalliPiMPWeb"
  ReadRegStr $InstalliPiMPTVplugin ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "InstalliPiMPTVplugin"
  ReadRegStr $InstalliPiMPMPplugin ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "InstalliPiMPMPplugin"
  ReadRegStr $ServerPath HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "InstallPath"
  ReadRegStr $ClientPath HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "InstallPath"
  ReadRegStr $CommonApp HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\explorer\Shell Folders" "Common AppData"
  ReadRegStr $DotNetDir HKLM "SOFTWARE\Microsoft\.NETFramework" "InstallRoot"

  StrCpy $unmsg "$(STRING_INIT_LINE1)"

  ${If} $InstalliPiMPTVplugin = "1"
        StrCpy $unmsg "$unmsg $\n$(STRING_INIT_LINE2)"
  ${EndIf}

  ${If} $InstalliPiMPMPplugin = "1"
        StrCpy $unmsg "$unmsg $\n$(STRING_INIT_LINE3)"
  ${EndIf}

  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 $unmsg IDYES +2
  Abort
FunctionEnd