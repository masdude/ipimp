Section MPClientPlugin

  StrCmp $InstalliPiMPMPplugin "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "MPClientPlugin skipped"
  ${EndIf}
  Return

  DetailPrint "$(STRING_MPCLIENTPLUGIN_LINE1)"

  SetOverwrite TRY

  SetOutPath "$ClientPath\plugins\process"
  File "..\..\MPClientController\bin\Release\MPClientController.dll"
  File "..\..\MPClientController\bin\Release\Jayrock.Json.dll"

  SetOutPath "$ClientPath\defaults\InputDeviceMappings"
  File "..\Include\Remote\iPiMP.xml"

  DetailPrint "Adding URL reservation for http listener"
  Sleep 100
  ExecDos::exec /TIMEOUT=2000 /DETAILED "netsh http add urlacl url=http://*:55668/mpcc sddl=D:(A;;GX;;;WD)"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "URL reservation failed"
  
SectionEnd


