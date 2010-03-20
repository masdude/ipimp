Section TVServerPlugin

  StrCmp $InstalliPiMPTVplugin "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "TVServerPlugin skipped"
  ${EndIf}
  Return

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE1)"

  SetOverwrite TRY

  SetOutPath "$ServerPath"
  File "..\..\iPiMPConfigurePlugin\bin\Debug\iPiMPConfigurePlugin.exe"

  SetOutPath "$ServerPath\Plugins"
  File "..\..\iPiMPTranscodeToMP4\bin\Debug\iPiMPTranscodeToMP4.dll"

  !include "InstallSections\InstallFFMpeg.nsh"
  
  !include "InstallSections\InstallHandbrake.nsh"

  !include "InstallSections\InstallMTN.nsh"
  
  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE2)"
  
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPtest=iPiMPtest"
  Pop $0 # return value
  StrCmp $0 "0" +3 0
  MessageBox MB_OK|MB_ICONSTOP "$(STRING_TVSERVICEPLUGIN_LINE3)"
  Return
  
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe del=iPiMPtest=iPiMPtest"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPTranscodeToMP4_SavePath=$MP4Path"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPTranscodeToMP4_Delete=$Delete"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=pluginiPiMPTranscodeToMP4=true"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPTranscodeToMP4_TranscodeNow=$TranscodeNow"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPTranscodeToMP4_TranscodeTime=$TranscodeTime"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPTranscodeToMP4_Transcoder=$Transcoder"

  ExecDos::exec /TIMEOUT=10000 /DETAILED "$ServerPath\iPiMPConfigurePlugin.exe add=iPiMPTranscodeToMP4_Preset=$Preset"

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE7)"
  ExecDos::exec /TIMEOUT=10000 /DETAILED "NET STOP TVSERVICE"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "$(STRING_TVSERVICEPLUGIN_LINE8)"

  DetailPrint "$(STRING_TVSERVICEPLUGIN_LINE9)"
  ExecDos::exec /TIMEOUT=20000 /DETAILED "NET START TVSERVICE"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "$(STRING_TVSERVICEPLUGIN_LINE10)"

SectionEnd

