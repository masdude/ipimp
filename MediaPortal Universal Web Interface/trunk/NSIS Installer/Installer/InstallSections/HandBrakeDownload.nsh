Section Handbrake

  StrCmp $InstallHandbrake "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "Handbrake skipped"
  ${EndIf}
  Return

  SetOverwrite TRY

  CreateDirectory "$INSTDIR\Utilities\Handbrake"

  DetailPrint "Downloading Handbrake"

    NSISdl::download http://ipimp.googlecode.com/files/Handbrake.zip $INSTDIR\Utilities\Handbrake\Handbrake.zip
    
  DetailPrint "Un-zipping Handbrake"

  ZipDLL::extractall $INSTDIR\Utilities\Handbrake\Handbrake.zip $INSTDIR\Utilities

  DetailPrint "Deleting Handbrake zip"

  Delete $INSTDIR\Utilities\Handbrake\Handbrake.zip
  
  CreateShortCut "$SMPROGRAMS\iPiMP\Handbrake.lnk" "$INSTDIR\Utilities\Handbrake\Handbrake.exe"

SectionEnd

