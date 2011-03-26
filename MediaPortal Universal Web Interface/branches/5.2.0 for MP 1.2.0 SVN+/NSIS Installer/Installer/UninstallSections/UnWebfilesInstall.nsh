Section un.InstallWebFiles
  
  ${If} $InstalliPiMPWeb = "0"
    ${If} ${IPIMPDEBUG} == "1"
      MessageBox MB_OK|MB_ICONINFORMATION "un.InstallWebFiles skipped"
    ${EndIf}
    Return
  ${EndIf}

  DetailPrint "$(STRING_WEBFILESINSTALL_LINE2)"
  !insertmacro BackupFile "$INSTDIR\Aspx\App_Data" "uWiMP.db"

  DetailPrint "$(STRING_WEBFILESINSTALL_LINE3)"
  !insertmacro RemoveFilesAndSubDirs "$INSTDIR\Aspx"
  RMDir "$INSTDIR\Aspx"

SectionEnd
