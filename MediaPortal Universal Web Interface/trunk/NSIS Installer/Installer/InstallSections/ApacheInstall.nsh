Section ApacheInstallFiles

  StrCmp $InstallApache "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "ApacheInstallFiles skipped"
  ${EndIf}
  Return

  SetOverwrite TRY

  CreateDirectory "$INSTDIR\Apache"

  DetailPrint "Installing Apache files"
  !include "InstallSections\Apache.2.2.14.files.nsh"

SectionEnd

