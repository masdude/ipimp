Section un.ApacheInstallFiles

  StrCmp $InstallApache "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "un.ApacheInstallFiles aborted"
  ${EndIf}
  Return

  DetailPrint "Removing Apache files"
  !insertmacro RemoveFilesAndSubDirs "$INSTDIR\Apache"
  RMDir "$INSTDIR\Apache"

SectionEnd