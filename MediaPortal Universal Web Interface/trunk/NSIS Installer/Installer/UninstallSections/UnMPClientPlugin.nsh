Section un.MPClientPlugin

  StrCmp $InstalliPiMPMPplugin "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "un.MPClientPlugin skipped"
  ${EndIf}
  Return

  DetailPrint "$(STRING_MPCLIENTPLUGIN_LINE2)"

  Delete "$ClientPath\plugins\process\MPClientController.dll"
  Delete "$ClientPath\plugins\process\Jayrock.Json.dll"
  Delete "$ClientPath\InputDeviceMappings\defaults\iPiMP.xml"

  DetailPrint "Deleting URL reservation entry for http listener"
  Sleep 100
  ExecDos::exec /TIMEOUT=2000 /DETAILED "netsh http delete urlacl url=http://*:55668/mpcc"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "URL reservation removal failed"

SectionEnd
