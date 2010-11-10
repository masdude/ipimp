Section InstallWebFiles

  ${If} $InstalliPiMPWeb = "0"
    ${If} ${IPIMPDEBUG} == "1"
      MessageBox MB_OK|MB_ICONINFORMATION "InstallWebFiles skipped"
    ${EndIf}
    Return
  ${EndIf}

  DetailPrint "$(STRING_WEBFILESINSTALL_LINE1)"

  SetOverwrite TRY

  !include "InstallSections\InstallFFMpeg.nsh"

  !include "InstallSections\InstallHandbrake.nsh"

  !include "InstallSections\Webfiles.nsh"
  
  SetOutPath "$INSTDIR\Aspx"
  File "..\Include\Website\web.config"

  SetOutPath "$INSTDIR\Aspx\App_Data"
  File "..\Include\Database\uWiMP.db"
    
  IfFileExists "$TEMP\uWiMP.db" 0 +5
    ;Delete "$INSTDIR\Aspx\App_Data\uWiMP.db"
    ;!insertmacro RestoreFile "$INSTDIR\Aspx\App_Data" "uWiMP.db"
    Delete "$TEMP\uWiMP.db"
    
    
SectionEnd

