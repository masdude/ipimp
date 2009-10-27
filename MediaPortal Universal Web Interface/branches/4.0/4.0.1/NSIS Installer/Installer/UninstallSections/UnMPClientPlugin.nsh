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

SectionEnd
