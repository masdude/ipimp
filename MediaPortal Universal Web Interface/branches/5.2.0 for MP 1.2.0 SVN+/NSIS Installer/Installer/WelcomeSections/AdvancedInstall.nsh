Page custom AdvancedInstall AdvancedInstallValidate

Var p3Dialog
Var p3ImageControl
Var p3Image
Var p3Headline

Var p3Checkbox1
Var p3Checkbox2
Var p3Checkbox3
Var p3Checkbox4
Var p3Checkbox5

Var p3Text1

Var p3Checkbox1_State
Var p3Checkbox2_State
Var p3Checkbox3_State
Var p3Checkbox4_State
Var p3Checkbox5_State

Function AdvancedInstall

        StrCmp $AdvancedInstall "0" +1 +2
        Abort

	nsDialogs::Create 1044
	Pop $p3Dialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $p3ImageControl

	StrCpy $0 $PLUGINSDIR\ipimpgeek.bmp
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p3Image

	SendMessage $p3ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p3Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_ADVANCEDINSTALL_LINE1)"
	Pop $p3Headline

	SendMessage $p3Headline ${WM_SETFONT} $Headline_font 0

	${NSD_CreateCheckbox} 120u 40u 160u 8u "$(STRING_ADVANCEDINSTALL_LINE2) ${APACHE_VERSION}"
	Pop $p3Checkbox1
        ${If} $InstallApache == "1"
              ${NSD_Check} $p3Checkbox1
        ${EndIf}

	GetFunctionAddress $0 Onp3Checkbox1
	nsDialogs::OnClick $p3Checkbox1 $0

	${NSD_CreateCheckbox} 120u 50u 200u 8u "$(STRING_ADVANCEDINSTALL_LINE3)"
	Pop $p3Checkbox2
        ${If} $InstallModAspNet == "1"
              ${NSD_Check} $p3Checkbox2
        ${EndIf}
	GetFunctionAddress $0 Onp3Checkbox2
	nsDialogs::OnClick $p3Checkbox2 $0

	${NSD_CreateCheckbox} 120u 60u 200u 8u "$(STRING_ADVANCEDINSTALL_LINE4)"
	Pop $p3Checkbox3
        ${If} $InstalliPiMPWeb == "1"
              ${NSD_Check} $p3Checkbox3
        ${EndIf}
	GetFunctionAddress $0 Onp3Checkbox3
	nsDialogs::OnClick $p3Checkbox3 $0

	${NSD_CreateCheckbox} 120u 70u 200u 8u "$(STRING_ADVANCEDINSTALL_LINE5)"
	Pop $p3Checkbox4
        ${If} $InstalliPiMPTVplugin == "1"
              ${NSD_Check} $p3Checkbox4
        ${EndIf}
	GetFunctionAddress $0 Onp3Checkbox4
	nsDialogs::OnClick $p3Checkbox4 $0

	${NSD_CreateCheckbox} 120u 80u 200u 8u "$(STRING_ADVANCEDINSTALL_LINE6)"
	Pop $p3Checkbox5
        ${If} $InstalliPiMPMPplugin == "1"
              ${NSD_Check} $p3Checkbox5
        ${EndIf}
	GetFunctionAddress $0 Onp3Checkbox5
	nsDialogs::OnClick $p3Checkbox5 $0

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 120u 190u 70u "$(STRING_ADVANCEDINSTALL_LINE7)"
	Pop $p3Text1

	SetCtlColors $p3Dialog "" 0xffffff
	SetCtlColors $p3Headline "" 0xffffff

        SetCtlColors $p3Checkbox1 "" 0xffffff
        SetCtlColors $p3Checkbox2 "" 0xffffff
        SetCtlColors $p3Checkbox3 "" 0xffffff
        SetCtlColors $p3Checkbox4 "" 0xffffff
        SetCtlColors $p3Checkbox5 "" 0xffffff
        SetCtlColors $p3Text1 "" 0xffffff

        ${If} $TVServerMaj <> "1"
              EnableWindow $p3Checkbox1 0
              EnableWindow $p3Checkbox2 0
              EnableWindow $p3Checkbox3 0
              EnableWindow $p3Checkbox4 0
              ${NSD_Check} $p3Checkbox5
        ${EndIf}

        ${If} $MPClientMaj <> "1"
              EnableWindow $p3Checkbox5 0
              ${NSD_UnCheck} $p3Checkbox5
        ${EndIf}

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd


Function Onp3Checkbox1
	Pop $0 # HWND
        ${NSD_GetState} $p3Checkbox1 $p3Checkbox1_State
        ${If} $p3Checkbox1_State == ${BST_CHECKED}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE8)"
              ${NSD_Check} $p3Checkbox2
              ${NSD_Check} $p3Checkbox3
        ${Else}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE7)"
        ${EndIf}
FunctionEnd

Function Onp3Checkbox2
	Pop $0 # HWND
        ${NSD_GetState} $p3Checkbox2 $p3Checkbox2_State
        ${If} $p3Checkbox2_State == ${BST_CHECKED}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE9)"
        ${Else}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE7)"
	      ${NSD_Uncheck} $p3Checkbox1
        ${EndIf}
FunctionEnd

Function Onp3Checkbox3
	Pop $0 # HWND

        ${NSD_GetState} $p3Checkbox3 $p3Checkbox3_State
        ${If} $p3Checkbox3_State == ${BST_CHECKED}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE10)"
        ${Else}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE7)"
	      ${NSD_Uncheck} $p3Checkbox1
        ${EndIf}
FunctionEnd

Function Onp3Checkbox4
	Pop $0 # HWND
        ${NSD_GetState} $p3Checkbox4 $p3Checkbox4_State
        ${If} $p3Checkbox4_State == ${BST_CHECKED}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE11)"
        ${Else}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE7)"
        ${EndIf}
FunctionEnd

Function Onp3Checkbox5
	Pop $0 # HWND
        ${NSD_GetState} $p3Checkbox5 $p3Checkbox5_State
        ${If} $p3Checkbox5_State == ${BST_CHECKED}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE12)"
        ${Else}
              ${NSD_SetText} $p3Text1 "$(STRING_ADVANCEDINSTALL_LINE7)"
        ${EndIf}
FunctionEnd

Function AdvancedInstallValidate

        ${If} $TVServerMaj <> "1"
        ${AndIf} $MPClientMaj <> "1"
                 MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_ADVANCEDINSTALL_LINE13)"
                 Abort
        ${EndIf}

        ${NSD_GetState} $p3Checkbox1 $p3Checkbox1_State
        ${NSD_GetState} $p3Checkbox2 $p3Checkbox2_State
        ${NSD_GetState} $p3Checkbox3 $p3Checkbox3_State
        ${NSD_GetState} $p3Checkbox4 $p3Checkbox4_State
        ${NSD_GetState} $p3Checkbox5 $p3Checkbox5_State

        ${If} $p3Checkbox1_State == ${BST_UNCHECKED}
        ${AndIf} $p3Checkbox2_State == ${BST_UNCHECKED}
        ${AndIf} $p3Checkbox3_State == ${BST_UNCHECKED}
        ${AndIf} $p3Checkbox4_State == ${BST_UNCHECKED}
        ${AndIf} $p3Checkbox5_State == ${BST_UNCHECKED}
                 MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_ADVANCEDINSTALL_LINE14)"
                 Abort
        ${EndIf}

        ${If} $p3Checkbox3_State == ${BST_CHECKED}
        ${AndIf} $p3Checkbox1_State == ${BST_UNCHECKED}
                 MessageBox MB_ICONINFORMATION|MB_YESNO "$(STRING_ADVANCEDINSTALL_LINE15)" IDYES Yes IDNO No
                 No:
                   Abort
                 Yes:
        ${EndIf}

       	StrCpy $InstallApache "0"
       	StrCpy $InstallModAspNet "0"
       	StrCpy $InstalliPiMPWeb "0"
       	StrCpy $InstalliPiMPTVplugin "0"
       	StrCpy $InstalliPiMPMPplugin "0"
        StrCpy $UpdateWebConfig "0"
        StrCpy $UpdateApacheConfig "0"

        ${If} $p3Checkbox1_State == ${BST_CHECKED}
              	StrCpy $InstallApache "1"
                StrCpy $UpdateApacheConfig "1"
        ${EndIf}

        ${If} $p3Checkbox2_State == ${BST_CHECKED}
                StrCpy $InstallModAspNet "1"
                StrCpy $UpdateApacheConfig "1"
        ${EndIf}

        ${If} $p3Checkbox3_State == ${BST_CHECKED}
                StrCpy $InstalliPiMPWeb "1"
                StrCpy $UpdateWebConfig "1"
        ${EndIf}

        ${If} $p3Checkbox4_State == ${BST_CHECKED}
                StrCpy $InstalliPiMPTVplugin "1"
        ${EndIf}

        ${If} $p3Checkbox5_State == ${BST_CHECKED}
                StrCpy $InstalliPiMPMPplugin "1"
        ${EndIf}

        ${If} $p3Checkbox4_State == ${BST_CHECKED}
         !insertmacro CheckVersion "TV"
        ${EndIf}

        ${If} $p3Checkbox5_State == ${BST_CHECKED}
         !insertmacro CheckVersion "MP"
        ${EndIf}

FunctionEnd