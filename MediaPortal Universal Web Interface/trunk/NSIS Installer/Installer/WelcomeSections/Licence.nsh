Page custom Licence

Var LicenceDialog
Var LicenceImageControl
Var LicenceImage
Var LicenceHeadline

Var LicenceLabel1
Var LicenceDirectory
Var LicenceButton1

Function InstallLocation

	nsDialogs::Create 1044
	Pop $LicenceDialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $LicenceImageControl

	StrCpy $0 "Images\iPiMPinstall.bmp"
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $LicenceImage

	SendMessage $LicenceImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $LicenceImage

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_LICENCE_LINE1)"
	Pop $LicenceHeadline

	SendMessage $LicenceHeadline ${WM_SETFONT} $Headline_font 0

        ;${NSD_CreateText} 120u 40u 130u 18u "blah blah"
	;Pop $LicenceText1

	;SetCtlColors $LicenceText1 "" 0xffffff
	;SetCtlColors $LicenceHeadline "" 0xffffff

	;SetCtlColors $LicenceLabel1 "" 0xffffff
	;SetCtlColors $LicenceButton1 "" 0xffffff

        GetDlgItem $0 $HWNDPARENT 1 ; Next button
        SendMessage $0 ${WM_SETTEXT} 0 "STR:Install"

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd

