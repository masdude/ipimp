Page custom InstallLocation InstallLocationValidate

Var p6Dialog
Var p6ImageControl
Var p6Image
Var p6Headline

Var p6Label1
Var p6Directory
Var p6Button1

Function InstallLocation

	nsDialogs::Create 1044
	Pop $p6Dialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $p6ImageControl

	StrCpy $0 $PLUGINSDIR\ipimp.bmp
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p6Image

	SendMessage $p6ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p6Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_INSTALLLOCATION_LINE1)"
	Pop $p6Headline

	SendMessage $p6Headline ${WM_SETFONT} $Headline_font 0

        ${NSD_CreateLabel} 120u 40u 130u 18u 

	Pop $p6Label1
        
        ${NSD_CreateDirRequest} 120u 60u 200u 12u "$INSTDIR"
        Pop $p6Directory
        
        ${NSD_CreateBrowseButton} 255u 40u 40u 14u "$(STRING_TVPLUGININSTALL_LINE9)"
        Pop $p6Button1
        ${NSD_OnClick} $p6Button1 p6OnDirBrowseButton


	SetCtlColors $p6Dialog "" 0xffffff
	SetCtlColors $p6Headline "" 0xffffff

	SetCtlColors $p6Label1 "" 0xffffff
	SetCtlColors $p6Button1 "" 0xffffff

        GetDlgItem $0 $HWNDPARENT 1 ; Next button
        SendMessage $0 ${WM_SETTEXT} 0 "STR:Install"

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd


Function p6OnDirBrowseButton
        Pop $R0
        ${If} $R0 == $p6Button1
              ${NSD_GetText} $p6Directory $R0
              nsDialogs::SelectFolderDialog /NOUNLOAD "$(STRING_INSTALLLOCATION_LINE3)" $R0
              Pop $R0

              ${If} $R0 != error
                    ${NSD_SetText} $p6Directory "$R0"
              ${EndIf}
        ${EndIf}
FunctionEnd

Function InstallLocationValidate

       ${NSD_GetText} $p6Directory $INSTDIR
       IfFileExists "$INSTDIR\*.*" 0 Yes
       MessageBox MB_ICONEXCLAMATION|MB_YESNO "$(STRING_INSTALLLOCATION_LINE4)" IDYES Yes IDNO No
       No:
         Abort
       Yes:

FunctionEnd