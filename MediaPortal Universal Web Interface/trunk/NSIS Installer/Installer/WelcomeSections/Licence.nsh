Page custom Licence

Var LicenceDialog
Var LicenceImageControl
Var LicenceImage
Var LicenceHeadline

Var LicenceLabel1
Var LicenceDirectory
Var LicenceButton1
Var RICHEDIT
Var LicFile

Function Licence

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

        nsDialogs::CreateControl /NOUNLOAD RichEdit20A   ${WS_VISIBLE}|${WS_CHILD}|${WS_TABSTOP}|${WS_VSCROLL}|${ES_MULTILINE}|${ES_WANTRETURN}|${ES_READONLY} ${WS_EX_STATICEDGE} 120u 40u 200u 150u ''
        Pop $RICHEDIT

        System::Call 'kernel32::CreateFile(t "GPL3.txt", i 0x80000000, i 1, i 0, i 3, i 0, i 0) i .r0'
        System::Call 'kernel32::GetFileSize(i r0, i 0) i .r1'
        System::Alloc $1
        Pop $2
        System::Call 'kernel32::ReadFile(i r0, i r2, i r1, *i .r3, i 0)'
        System::Call 'kernel32::CloseHandle(i r0)'
        Push $2
        Pop $LicFile

        SendMessage $RICHEDIT ${WM_SETTEXT} 0 $LicFile

	SetCtlColors $RICHEDIT "" 0xffffff
	SetCtlColors $LicenceHeadline "" 0xffffff

	SetCtlColors $LicenceDialog "" 0xffffff

        GetDlgItem $0 $HWNDPARENT 1 ; Next button
        SendMessage $0 ${WM_SETTEXT} 0 "STR:I Agree"

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd

