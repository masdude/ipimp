Section FFMpeg

  StrCmp $InstallFFMpeg "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "FFMpeg skipped"
  ${EndIf}
  Return

  SetOverwrite TRY

  CreateDirectory "$INSTDIR\Utilities\FFMpeg"

  DetailPrint "Downloading FFMpeg"

  NSISdl::download http://ipimp.googlecode.com/files/ffmpeg.zip $INSTDIR\Utilities\FFMpeg\ffmpeg.zip
  
  DetailPrint "Un-zipping FFMpeg"

  ZipDLL::extractall $INSTDIR\Utilities\FFMpeg\ffmpeg.zip $INSTDIR\Utilities\FFMpeg

  DetailPrint "Deleting FFMpeg zip"

  Delete $INSTDIR\Utilities\FFMpeg\ffmpeg.zip
  
SectionEnd

