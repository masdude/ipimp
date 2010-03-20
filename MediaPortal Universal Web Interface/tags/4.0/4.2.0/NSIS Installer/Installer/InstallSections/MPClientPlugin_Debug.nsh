Section MPClientPlugin

  StrCmp $InstalliPiMPMPplugin "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "MPClientPlugin skipped"
  ${EndIf}
  Return

  DetailPrint "$(STRING_MPCLIENTPLUGIN_LINE1)"

  SetOverwrite TRY

  SetOutPath "$ClientPath\plugins\process"
  File "..\..\MPClientController\bin\Debug\MPClientController.dll"
  File "..\..\MPClientController\bin\Debug\Jayrock.Json.dll"

  SetOutPath "$ClientPath\InputDeviceMappings\defaults"
  File "..\Include\Remote\iPiMP.xml"

SectionEnd


