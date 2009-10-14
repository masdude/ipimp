Section UpdateConfig

  ${If} $InstalliPiMPWeb = "0"
    ${If} ${IPIMPDEBUG} == "1"
      MessageBox MB_OK|MB_ICONINFORMATION "UpdateConfig skipped"
    ${EndIf}
    Return
  ${EndIf}

  DetailPrint "Patching web.config"
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\aspx\web.config" "##MP4PATH##" "$MP4Path" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\aspx\web.config" "##TIMEOUT##" "$Timeout" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\aspx\web.config" "##VERSION##" "${PRODUCT_VERSION}" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\aspx\web.config" "##PAGESIZE##" "$Pagesize" "/S=1 /C=0 /AO=1" $0
  
  ${If} $InstallNoTVServer = "1"
    ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\aspx\web.config" "##NOTVSRV##" "true" "/S=1 /C=0 /AO=1" $0
  ${EndIf}
  
  ${If} $InstallNoTVServer = "0"
    ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\aspx\web.config" "##NOTVSRV##" "false" "/S=1 /C=0 /AO=1" $0
  ${EndIf}
  
  ${If} $InstallApache = "0"
    ${If} ${IPIMPDEBUG} == "1"
      MessageBox MB_OK|MB_ICONINFORMATION "UpdateConfig part 2 skipped"
    ${EndIf}
    Return
  ${EndIf}
  
  ${If} $InstalliPiMPTVplugin = "0"
    StrCpy $MP4Path "DummyDir"
  ${EndIf}
  
  DetailPrint "Deploying iPiMP httpd.conf"

  SetOutPath "$INSTDIR\apache\conf"
  File "D:\Dev\vs2005\iPiMPWeb\iPiMPInstaller\files\extras\apache\iPiMP.conf"

  StrCmp "$LogoPath" "" 0 +2
  StrCpy $LogoPath "/"

  StrCmp "$MP4Path" "" 0 +2
  StrCpy $MP4Path "/"

  ${WordReplace} "$INSTDIR\Apache" "\" "/" "+*" $ApachePath
  ${WordReplace} "$INSTDIR\Aspx" "\" "/" "+*" $AspxPath
  ${WordReplace} "$LogoPath" "\" "/" "+*" $LogoPath
  ${WordReplace} "$MP4Path" "\" "/" "+*" $MP4Path
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##SERVERROOT##" "$ApachePath" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##DOCUMENTROOT##" "$AspxPath" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##TVIMAGEROOT##" "$LogoPath" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##MP4ROOT##" "$MP4Path" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##PORT##" "$TCPPort" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##LISTEN##" "$Listen" "/S=1 /C=0 /AO=1" $0
  ${textreplace::Unload}

SectionEnd