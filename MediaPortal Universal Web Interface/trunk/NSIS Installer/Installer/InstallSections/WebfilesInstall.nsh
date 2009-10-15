Section InstallWebFiles

  ${If} $InstalliPiMPWeb = "0"
    ${If} ${IPIMPDEBUG} == "1"
      MessageBox MB_OK|MB_ICONINFORMATION "InstallWebFiles skipped"
    ${EndIf}
    Return
  ${EndIf}

  DetailPrint "$(STRING_WEBFILESINSTALL_LINE1)"

  SetOverwrite TRY

  IfFileExists "$TEMP\uWiMP.db" 0 +3
    Delete "$INSTDIR\Aspx\App_Data\uWiMP.db"
    !insertmacro RestoreFile "$INSTDIR\Aspx\App_Data" "uWiMP.db"

  !include "InstallSections\Webfiles.nsh"
  
  SetOutPath "$INSTDIR\Aspx"
  File "..\Include\Website\web.config"

  SetOutPath "$INSTDIR\Aspx\App_Data"
  File "..\Include\Database\uWiMP.db"
    
SectionEnd

