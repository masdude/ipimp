Section un.ApacheModAspNet

  StrCmp $InstallModAspNet "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "un.ApacheModAspNet skipped"
  ${EndIf}
  Return

  DetailPrint "Un-registering Apache .NET module"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$DotNetDir\v2.0.50727\RegAsm.exe /verbose /nologo /unregister $\"$INSTDIR\Utilities\Apache\Apache.Web.dll$\""
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "RegAsm unregister failed - Reason: $0"

  DetailPrint "Un-installing Apache .NET module"
  Sleep 100
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$INSTDIR\Utilities\Apache\gacutil.exe /nologo /u Apache.Web"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "GACUtil uninstall failed - Reason: $0"

  DetailPrint "Removing Apache .NET module files"
  !insertmacro RemoveFilesAndSubDirs "$INSTDIR\Utilities\Apache"
  RMDir "$INSTDIR\Utilities\Apache"

SectionEnd

