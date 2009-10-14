Section un.TVServerPlugin
  
  StrCmp $InstalliPiMPTVplugin "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "un.TVServerPlugin skipped"
  ${EndIf}
  Return

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE4)"

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE5)"
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPtest=iPiMPtest"
  Pop $0 # return value
  StrCmp $0 "0" +3 0
  MessageBox MB_OK|MB_ICONSTOP "$(STRING_TVSERVICEPLUGIN_LINE6)"
  Return
  
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPtest=iPiMPtest"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_SavePath=$MP4Path"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_FFmpegPath=$INSTDIR\Utilities\ffmpeg.exe"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_Delete=true"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=pluginiPiMPTranscodeToMP4=false"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_TranscodeNow=$TranscodeNow"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_TranscodeTime=$TranscodeTime"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_VideoBitrate=$VideoBitrate"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPTranscodeToMP4_AudioBitrate=$AudioBitrate"

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE7)"
  ExecDos::exec /TIMEOUT=10000 /DETAILED "NET STOP TVSERVICE"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "$(STRING_TVSERVICEPLUGIN_LINE8)"

  Delete "$INSTDIR\Utilities\ffmpeg.exe"
  Delete "$ServerPath\iPiMPConfigurePlugin.exe"
  Delete "$ServerPath\iPiMPTranscodeClient.exe"
  Delete "$ServerPath\plugins\iPiMPTranscodeToMP4.dll"
  Delete "$SMPROGRAMS\iPiMP\iPiMPTranscodeClient.lnk"

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE9)"
  ExecDos::exec /TIMEOUT=20000 /DETAILED "NET START TVSERVICE"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "$(STRING_TVSERVICEPLUGIN_LINE10)"

SectionEnd