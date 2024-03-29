Section UpdateConfig


  ${If} $InstalliPiMPWeb = "0"
    ${If} ${IPIMPDEBUG} == "1"
      MessageBox MB_OK|MB_ICONINFORMATION "UpdateConfig skipped"
    ${EndIf}
    Return
  ${EndIf}

  DetailPrint "Patching web.config"
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##MP4PATH##" "$MP4Path" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##TIMEOUT##" "$Timeout" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##VERSION##" "${PRODUCT_VERSION}" "/S=1 /C=0 /AO=1" $0
  ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##PAGESIZE##" "$Pagesize" "/S=1 /C=0 /AO=1" $0
  
  ${If} $InstallNoTVServer = "0"
    ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##TVSERVER##" "true" "/S=1 /C=0 /AO=1" $0
  ${EndIf}
  
  ${If} $InstallNoTVServer = "1"
    ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##TVSERVER##" "false" "/S=1 /C=0 /AO=1" $0
  ${EndIf}
  
  ${If} $InstallNoMPClient = "0"
    ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##MPCLIENT##" "true" "/S=1 /C=0 /AO=1" $0
  ${EndIf}

  ${If} $InstallNoMPClient = "1"
    ${textreplace::ReplaceInFile} "$INSTDIR\Aspx\web.config" "$INSTDIR\Aspx\web.config" "##MPCLIENT##" "false" "/S=1 /C=0 /AO=1" $0
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
  
  StrCmp "$LogoPath" "" 0 +2
  StrCpy $LogoPath "/"

  StrCmp "$MP4Path" "" 0 +2
  StrCpy $MP4Path "/"

  Push "$INSTDIR\Apache"
  Push "\"
  Call StrSlash
  Pop $R0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##SERVERROOT##" "$R0" "/S=1 /C=0 /AO=1" $0

  Push "$INSTDIR\Aspx"
  Push "\"
  Call StrSlash
  Pop $R0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##DOCUMENTROOT##" "$R0" "/S=1 /C=0 /AO=1" $0

  Push "$LogoPath"
  Push "\"
  Call StrSlash
  Pop $R0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##TVIMAGEROOT##" "$R0" "/S=1 /C=0 /AO=1" $0

  Push "$MP4Path"
  Push "\"
  Call StrSlash
  Pop $R0
  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##MP4ROOT##" "$R0" "/S=1 /C=0 /AO=1" $0

  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##PORT##" "$TCPPort" "/S=1 /C=0 /AO=1" $0

  ${textreplace::ReplaceInFile} "$INSTDIR\Apache\conf\iPiMP.conf" "$INSTDIR\Apache\conf\iPiMP.conf" "##LISTEN##" "$Listen" "/S=1 /C=0 /AO=1" $0

  ${textreplace::Unload}

SectionEnd