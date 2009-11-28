Section un.ApacheServiceInstall

  StrCmp $InstallApache "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "un.ApacheServiceInstall skipped"
  ${EndIf}
  Return

  DetailPrint "$(STRING_APACHESERVICE_LINE8)"

  Sleep 100
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$INSTDIR\apache\bin\httpd.exe -k stop -n iPiMPweb"
  Pop $0 # return value

  ${If} $0 == "1"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE10)"
  ${ElseIf} $0 != "0"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE11)"
  ${EndIf}

  DetailPrint "$(STRING_APACHESERVICE_LINE9)"

  Sleep 100
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$INSTDIR\apache\bin\httpd.exe -k uninstall -n iPiMPweb"
  Pop $0 # return value

  ${If} $0 == "2"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE12)"
  ${ElseIf} $0 == "1"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE13)"
  ${ElseIf} $0 != "0"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE14)"
  ${EndIf}

SectionEnd

