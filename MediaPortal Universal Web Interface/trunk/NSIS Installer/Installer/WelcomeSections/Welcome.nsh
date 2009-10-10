Page custom Welcome WelcomeValidate

Var DIALOG
Var HEADLINE
Var TEXT
Var LINK
Var IMAGECTL
Var IMAGE
Var CHECKBOX
Var CHECKBOX_State

Function Welcome

        nsDialogs::Create 1044
	Pop $DIALOG

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $IMAGECTL

	StrCpy $0 "Images\iPiMPinstall.bmp"
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $IMAGE
	SendMessage $IMAGECTL ${STM_SETIMAGE} ${IMAGE_BITMAP} $IMAGE

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_WELCOME_TITLE)"
	Pop $HEADLINE
	SendMessage $HEADLINE ${WM_SETFONT} $Headline_font 0

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 32u 200u 82u "$(STRING_WELCOME_LINE1)"
	Pop $TEXT
	
	${NSD_CreateLink} 120u 120u 200u 8u "$(STRING_WELCOME_LINE2)"
        Pop $LINK
        ${NSD_OnClick} $LINK onClickMyLink
	
	${NSD_CreateCheckbox} 225u -32 90u 8u "$(STRING_WELCOME_LINE3)"
	Pop $CHECKBOX

        ${If} $AdvancedInstall == "1"
              ${NSD_Check} $CHECKBOX
        ${EndIf}
        
	SetCtlColors $DIALOG "" 0xffffff
	SetCtlColors $HEADLINE "" 0xffffff
	SetCtlColors $TEXT "" 0xffffff
	SetCtlColors $LINK 0x0000ff 0xffffff
	SetCtlColors $CHECKBOX "" 0xffffff

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd

Function HideControls

    LockWindow on
    GetDlgItem $0 $HWNDPARENT 1028
    ShowWindow $0 ${SW_HIDE}

    GetDlgItem $0 $HWNDPARENT 1256
    ShowWindow $0 ${SW_HIDE}

    GetDlgItem $0 $HWNDPARENT 1035
    ShowWindow $0 ${SW_HIDE}

    GetDlgItem $0 $HWNDPARENT 1037
    ShowWindow $0 ${SW_HIDE}

    GetDlgItem $0 $HWNDPARENT 1038
    ShowWindow $0 ${SW_HIDE}

    GetDlgItem $0 $HWNDPARENT 1039
    ShowWindow $0 ${SW_HIDE}

    GetDlgItem $0 $HWNDPARENT 1045
    ShowWindow $0 ${SW_NORMAL}
    LockWindow off

FunctionEnd

Function ShowControls

    LockWindow on
    GetDlgItem $0 $HWNDPARENT 1028
    ShowWindow $0 ${SW_NORMAL}

    GetDlgItem $0 $HWNDPARENT 1256
    ShowWindow $0 ${SW_NORMAL}

    GetDlgItem $0 $HWNDPARENT 1035
    ShowWindow $0 ${SW_NORMAL}

    GetDlgItem $0 $HWNDPARENT 1037
    ShowWindow $0 ${SW_NORMAL}

    GetDlgItem $0 $HWNDPARENT 1038
    ShowWindow $0 ${SW_NORMAL}

    GetDlgItem $0 $HWNDPARENT 1039
    ShowWindow $0 ${SW_NORMAL}

    GetDlgItem $0 $HWNDPARENT 1045
    ShowWindow $0 ${SW_HIDE}
    LockWindow off

FunctionEnd

Function onClickMyLink
  StrCpy $Wiki "1"
  Pop $0
  ExecShell "open" "http://www.team-mediaportal.com/manual/Extensions-TV-Server-Plugins/iPiMP"
FunctionEnd

Function WelcomeValidate

  ${If} $Wiki != "1"
    MessageBox MB_ICONINFORMATION|MB_YESNO "$(STRING_NOWIKI)" IDYES YES IDNO NO
    NO:
      Abort
    YES:
      StrCpy $WIKI "1"
  ${EndIf}
  
        ${NSD_GetState} $CHECKBOX $CHECKBOX_State

        ${If} $CHECKBOX_State == ${BST_CHECKED}
              	StrCpy $AdvancedInstall "1"
        ${Else}
              	StrCpy $AdvancedInstall "0"
        ${EndIf}

FunctionEnd