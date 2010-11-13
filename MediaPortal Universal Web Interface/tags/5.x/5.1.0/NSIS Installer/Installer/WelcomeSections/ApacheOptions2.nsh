Page custom ApacheOptions2 ApacheOptions2Validate

Var p4_2Dialog
Var p4_2ImageControl
Var p4_2Image
Var p4_2Headline

Var p4_2Label1
Var p4_2Directory1
Var p4_2Button1

Var p4_2Label2
Var p4_2Directory2
Var p4_2Button2

Function ApacheOptions2

        StrCmp $UpdateApacheConfig "1" +2
        Abort

	nsDialogs::Create 1044
	Pop $p4_2Dialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $p4_2ImageControl

	StrCpy $0 $PLUGINSDIR\ipimpapache.bmp
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p4_2Image

	SendMessage $p4_2ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p4_2Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 30u "$(STRING_APACHEINSTALL_LINE1)"
	Pop $p4_2Headline

	SendMessage $p4_2Headline ${WM_SETFONT} $Headline_font 0

        ${NSD_CreateLabel} 120u 40u 160u 20u "$(STRING_APACHEINSTALL_LINE14)"
	Pop $p4_2Label1
        
        ${NSD_CreateDirRequest} 120u 60u 200u 12u "$LogoPath"
        Pop $p4_2Directory1
        
        ${NSD_CreateBrowseButton} 280u 40u 40u 14u "$(STRING_BROWSE)"
        Pop $p4_2Button1
        ${NSD_OnClick} $p4_2Button1 p4_2OnDirBrowseButton1
        
        ${NSD_CreateLabel} 120u 80u 160u 20u "$(STRING_APACHEINSTALL_LINE18)"
	Pop $p4_2Label2

        ${NSD_CreateDirRequest} 120u 100u 200u 12u "$RadioLogoPath"
        Pop $p4_2Directory2

        ${NSD_CreateBrowseButton} 280u 80u 40u 14u "$(STRING_BROWSE)"
        Pop $p4_2Button2
        ${NSD_OnClick} $p4_2Button2 p4_2OnDirBrowseButton2


	SetCtlColors $p4_2Dialog "" 0xffffff
	SetCtlColors $p4_2Headline "" 0xffffff

	SetCtlColors $p4_2Label1 "" 0xffffff
	SetCtlColors $p4_2Button1 "" 0xffffff

	SetCtlColors $p4_2Label2 "" 0xffffff
	SetCtlColors $p4_2Button2 "" 0xffffff

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd


Function p4_2OnDirBrowseButton1
        Pop $R0
        ${If} $R0 == $p4_2Button1
              ${NSD_GetText} $p4_2Directory1 $R0
              nsDialogs::SelectFolderDialog /NOUNLOAD "$(STRING_APACHEINSTALL_LINE14)" $R0
              Pop $R0

              ${If} $R0 != error
                    ${NSD_SetText} $p4_2Directory1 "$R0"
              ${EndIf}
        ${EndIf}
FunctionEnd

Function p4_2OnDirBrowseButton2
        Pop $R0
        ${If} $R0 == $p4_2Button2
              ${NSD_GetText} $p4_2Directory2 $R0
              nsDialogs::SelectFolderDialog /NOUNLOAD "$(STRING_APACHEINSTALL_LINE18)" $R0
              Pop $R0

              ${If} $R0 != error
                    ${NSD_SetText} $p4_2Directory2 "$R0"
              ${EndIf}
        ${EndIf}
FunctionEnd

Function ApacheOptions2Validate

       ${NSD_GetText} $p4_2Directory1 $LogoPath
       IfFileExists "$LogoPath\*.*" +3 0
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_APACHEINSTALL_LINE16)"
       Abort

       StrCmp "$LogoPath" "" 0 +3
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_APACHEINSTALL_LINE17)"
       Abort

       ${NSD_GetText} $p4_2Directory2 $RadioLogoPath
       IfFileExists "$RadioLogoPath\*.*" +3 0
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_APACHEINSTALL_LINE19)"
       Abort

       StrCmp "$RadioLogoPath" "" 0 +3
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_APACHEINSTALL_LINE20)"
       Abort

  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "LogoPath=$LogoPath"
  ${EndIf}
  
FunctionEnd