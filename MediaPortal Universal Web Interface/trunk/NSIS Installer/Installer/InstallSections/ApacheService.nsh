Section ApacheServiceInstall

  StrCmp $InstallApache "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "ApacheServiceInstall skipped"
  ${EndIf}
  Return

  ${If} ${TCPPortOpen} $TCPPort
    MessageBox MB_OK "TCP port $TCPPort is currently in use, the Apache service install may throw errors.  If so you'll need to select a different port update $INSTDIR\Apache\conf/iPiMP.conf manually."
  ${EndIf}

  DetailPrint "$(STRING_APACHESERVICE_LINE6)"
  Sleep 100
  ExecDos::exec /TIMEOUT=10000 /DETAILED '"$INSTDIR\Apache\bin\httpd.exe" -k install -n iPiMPweb -f conf/iPiMP.conf'
  Pop $0 # return value

  ${If} $0 == "2"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE1)"
  ${ElseIf} $0 == "1"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE2)"
  ${ElseIf} $0 != "0"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE3)"
  ${EndIf}

  DetailPrint "$(STRING_APACHESERVICE_LINE7)"
  Sleep 100
  ExecDos::exec /TIMEOUT=10000 /DETAILED '"$INSTDIR\Apache\bin\httpd.exe" -k start -n iPiMPweb'
  Pop $0 # return value

  ${If} $0 == "1"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE4)"
  ${ElseIf} $0 != "0"
    MessageBox MB_OK|MB_ICONSTOP "$(STRING_APACHESERVICE_LINE5)"
  ${EndIf}

SectionEnd