Page custom SimpleInstall SimpleInstallValidate

Var p2Dialog
Var p2ImageControl
Var p2Image
Var p2Headline
Var p2Selection

Var p2Checkbox1
Var p2Checkbox2
Var p2Checkbox3
Var p2Checkbox4

Var p2Text1

Var p2Checkbox1_State
Var p2Checkbox2_State
Var p2Checkbox3_State
Var p2Checkbox4_State

Function SimpleInstall

        StrCmp $AdvancedInstall "1" +1 +2
        Abort

	nsDialogs::Create 1044
	Pop $p2Dialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $p2ImageControl

	StrCpy $0 $PLUGINSDIR\ipimp.bmp
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p2Image

	SendMessage $p2ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p2Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_SIMPLEINSTALL_LINE1)"
	Pop $p2Headline

	SendMessage $p2Headline ${WM_SETFONT} $Headline_font 0

	${NSD_CreateCheckbox} 120u 40u 160u 8u "$(STRING_SIMPLEINSTALL_LINE2)"
	Pop $p2Checkbox1
	GetFunctionAddress $0 Onp2Checkbox1
	nsDialogs::OnClick $p2Checkbox1 $0

	${NSD_CreateCheckbox} 120u 50u 160u 8u "$(STRING_SIMPLEINSTALL_LINE3)"
	Pop $p2Checkbox2
	GetFunctionAddress $0 Onp2Checkbox2
	nsDialogs::OnClick $p2Checkbox2 $0

	${NSD_CreateCheckbox} 120u 70u 160u 8u "$(STRING_SIMPLEINSTALL_LINE4)"
	Pop $p2Checkbox3
	GetFunctionAddress $0 Onp2Checkbox3
	nsDialogs::OnClick $p2Checkbox3 $0

	${NSD_CreateCheckbox} 120u 60u 160u 8u "$(STRING_SIMPLEINSTALL_LINE5)"
	Pop $p2Checkbox4
	GetFunctionAddress $0 Onp2Checkbox4
	nsDialogs::OnClick $p2Checkbox4 $0

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 120u 180u 60u "$(STRING_SIMPLEINSTALL_LINE6)"
	Pop $p2Text1

	SetCtlColors $p2Dialog "" 0xffffff
	SetCtlColors $p2Headline "" 0xffffff

        SetCtlColors $p2Checkbox1 "" 0xffffff
        SetCtlColors $p2Checkbox2 "" 0xffffff
        SetCtlColors $p2Checkbox3 "" 0xffffff
        SetCtlColors $p2Checkbox4 "" 0xffffff
        SetCtlColors $p2Text1 "" 0xffffff

        ${If} $p2Checkbox1_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE7)"
              ${NSD_Check} $p2Checkbox1
        ${ElseIf} $p2Checkbox2_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE8)"
              ${NSD_Check} $p2Checkbox2
        ${ElseIf} $p2Checkbox3_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE9)"
              ${NSD_Check} $p2Checkbox3
        ${ElseIf} $p2Checkbox4_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE10)"
              ${NSD_Check} $p2Checkbox4
        ${Else}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE6)"
        ${EndIf}


        ${If} $TVServerMaj <> "1"
              EnableWindow $p2Checkbox1 0
              EnableWindow $p2Checkbox2 0
        ${EndIf}

        ${If} $MPClientMaj <> "1"
              EnableWindow $p2Checkbox3 0
              EnableWindow $p2Checkbox4 0
        ${EndIf}

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd


Function Onp2Checkbox1
	Pop $0 # HWND
        ${NSD_GetState} $p2Checkbox1 $p2Checkbox1_State
        ${If} $p2Checkbox1_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE7)"
        ${Else}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE6)"
        ${EndIf}
        ${NSD_Uncheck} $p2Checkbox2
        ${NSD_Uncheck} $p2Checkbox3
        ${NSD_Uncheck} $p2Checkbox4
FunctionEnd

Function Onp2Checkbox2
	Pop $0 # HWND
        ${NSD_GetState} $p2Checkbox2 $p2Checkbox2_State
        ${If} $p2Checkbox2_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE8)"
        ${Else}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE6)"
        ${EndIf}
        ${NSD_Uncheck} $p2Checkbox1
        ${NSD_Uncheck} $p2Checkbox3
        ${NSD_Uncheck} $p2Checkbox4
FunctionEnd

Function Onp2Checkbox3
	Pop $0 # HWND

        ${NSD_GetState} $p2Checkbox3 $p2Checkbox3_State
        ${If} $p2Checkbox3_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE9)"
        ${Else}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE6)"
        ${EndIf}
        ${NSD_Uncheck} $p2Checkbox1
        ${NSD_Uncheck} $p2Checkbox2
        ${NSD_Uncheck} $p2Checkbox4
FunctionEnd

Function Onp2Checkbox4
	Pop $0 # HWND
        ${NSD_GetState} $p2Checkbox4 $p2Checkbox4_State
        ${If} $p2Checkbox4_State == ${BST_CHECKED}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE10)"
        ${Else}
              ${NSD_SetText} $p2Text1 "$(STRING_SIMPLEINSTALL_LINE6)"
        ${EndIf}
        ${NSD_Uncheck} $p2Checkbox1
        ${NSD_Uncheck} $p2Checkbox2
        ${NSD_Uncheck} $p2Checkbox3
FunctionEnd

Function SimpleInstallValidate

        ${If} $TVServerMaj <> "1"
        ${AndIf} $MPClientMaj <> "1"
                 MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_SIMPLEINSTALL_LINE11)"
                 Abort
        ${EndIf}

        ${NSD_GetState} $p2Checkbox1 $p2Checkbox1_State
        ${NSD_GetState} $p2Checkbox2 $p2Checkbox2_State
        ${NSD_GetState} $p2Checkbox3 $p2Checkbox3_State
        ${NSD_GetState} $p2Checkbox4 $p2Checkbox4_State

        ${If} $p2Checkbox1_State == ${BST_UNCHECKED}
        ${AndIf} $p2Checkbox2_State == ${BST_UNCHECKED}
        ${AndIf} $p2Checkbox3_State == ${BST_UNCHECKED}
        ${AndIf} $p2Checkbox4_State == ${BST_UNCHECKED}
                 MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_SIMPLEINSTALL_LINE12)"
                 Abort
        ${EndIf}

        ${If} $p2Checkbox1_State == ${BST_CHECKED}
              	StrCpy $InstallApache "1"
                StrCpy $InstallModAspNet "1"
                StrCpy $InstalliPiMPWeb "1"
                StrCpy $InstalliPiMPTVplugin "1"
                StrCpy $InstallHandbrake "1"
                StrCpy $InstallFFMpeg "1"
                StrCpy $InstalliPiMPMPplugin "1"
                StrCpy $InstallNoTVServer "0"
                StrCpy $InstallNoMPClient "0"
                StrCpy $p2Selection "1"
        ${EndIf}

        ${If} $p2Checkbox2_State == ${BST_CHECKED}
              	StrCpy $InstallApache "1"
                StrCpy $InstallModAspNet "1"
                StrCpy $InstalliPiMPWeb "1"
                StrCpy $InstalliPiMPTVplugin "1"
                StrCpy $InstallHandbrake "1"
                StrCpy $InstallFFMpeg "1"
                StrCpy $InstalliPiMPMPplugin "0"
                StrCpy $InstallNoTVServer "0"
                StrCpy $InstallNoMPClient "1"
                StrCpy $p2Selection "2"
        ${EndIf}

        ${If} $p2Checkbox3_State == ${BST_CHECKED}
              	StrCpy $InstallApache "0"
                StrCpy $InstallModAspNet "0"
                StrCpy $InstalliPiMPWeb "0"
                StrCpy $InstalliPiMPTVplugin "0"
                StrCpy $InstallHandbrake "0"
                StrCpy $InstallFFMpeg "0"
                StrCpy $InstalliPiMPMPplugin "1"
                StrCpy $InstallNoTVServer "1"
                StrCpy $InstallNoMPClient "0"
                StrCpy $p2Selection "3"
        ${EndIf}

        ${If} $p2Checkbox4_State == ${BST_CHECKED}
              	StrCpy $InstallApache "1"
                StrCpy $InstallModAspNet "1"
                StrCpy $InstalliPiMPWeb "1"
                StrCpy $InstalliPiMPTVplugin "0"
                StrCpy $InstallHandbrake "0"
                StrCpy $InstallFFMpeg "0"
                StrCpy $InstalliPiMPMPplugin "1"
                StrCpy $InstallNoTVServer "1"
                StrCpy $InstallNoMPClient "0"
                StrCpy $p2Selection "4"
        ${EndIf}

        ${If} $p2Checkbox1_State == ${BST_CHECKED}
          !insertmacro CheckVersion "TV"
        ${EndIf}

        ${If} $p2Checkbox1_State == ${BST_CHECKED}
          !insertmacro CheckVersion "MP"
        ${EndIf}
        
        ${If} $p2Checkbox2_State == ${BST_CHECKED}
          !insertmacro CheckVersion "TV"
        ${EndIf}

        ${If} $p2Checkbox3_State == ${BST_CHECKED}
          !insertmacro CheckVersion "MP"
        ${EndIf}

        ${If} $p2Checkbox4_State == ${BST_CHECKED}
          !insertmacro CheckVersion "MP"
        ${EndIf}

FunctionEnd