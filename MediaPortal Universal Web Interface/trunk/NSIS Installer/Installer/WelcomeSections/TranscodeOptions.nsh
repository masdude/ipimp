Page custom TranscodeOptions TranscodeOptionsValidate

Var p5Dialog
Var p5ImageControl
Var p5Image
Var p5Headline

Var p5Label1
Var p5DropList1

Var p5Label2
Var p5DropList2

Var p5Label3
Var p5DropList3

Var p5Label4
Var p5DropList4

Var p5Label5
Var p5Directory
Var p5Button1

Var p5Label6
Var p5DropList6

Function TranscodeOptions

        StrCmp $InstalliPiMPTVplugin "1" +2
        Abort

	nsDialogs::Create 1044
	Pop $p5Dialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $p5ImageControl

	StrCpy $0 "Images\iPiMPinstall.bmp"
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p5Image

	SendMessage $p5ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p5Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_TVPLUGININSTALL_LINE1)"
	Pop $p5Headline

	SendMessage $p5Headline ${WM_SETFONT} $Headline_font 0

        ${NSD_CreateLabel} 120u 40u 130u 18u "$(STRING_TVPLUGININSTALL_LINE2)"
	Pop $p5Label6

        ${NSD_CreateDropList} 255u 40u 40u 80u ""
        Pop $p5DropList6

        ${NSD_CB_AddString} $p5DropList6 "$(STRING_YES)"
        ${NSD_CB_AddString} $p5DropList6 "$(STRING_NO)"

        ${If} $Delete == "true"
              ${NSD_CB_SelectString} $p5DropList6 "$(STRING_YES)"
        ${ElseIf} $Delete == "false"
              ${NSD_CB_SelectString} $p5DropList6 "$(STRING_NO)"
        ${Else}
              ${NSD_CB_SelectString} $p5DropList6 "$(STRING_YES)"
        ${EndIf}

        ${NSD_CreateLabel} 120u 60u 130u 18u "$(STRING_TVPLUGININSTALL_LINE3)"
	Pop $p5Label1

        ${NSD_CreateDropList} 255u 60u 40u 80u ""
        Pop $p5DropList1

        ${NSD_CB_AddString} $p5DropList1 "$(STRING_YES)"
        ${NSD_CB_AddString} $p5DropList1 "$(STRING_NO)"

        ${If} $TranscodeNow == "true"
              ${NSD_CB_SelectString} $p5DropList1 "$(STRING_YES)"
        ${ElseIf} $TranscodeNow == "false"
              ${NSD_CB_SelectString} $p5DropList1 "$(STRING_NO)"
        ${Else}
              ${NSD_CB_SelectString} $p5DropList1 "$(STRING_NO)"
        ${EndIf}

        ${NSD_CreateLabel} 120u 80u 130u 18u "$(STRING_TVPLUGININSTALL_LINE4)"
	Pop $p5Label2

        ${NSD_CreateDropList} 255u 80u 40u 80u ""
        Pop $p5DropList2

        ${NSD_CB_AddString} $p5DropList2 "$(STRING_TVPLUGININSTALL_LINE5)"
        ${NSD_CB_AddString} $p5DropList2 "00:00"
        ${NSD_CB_AddString} $p5DropList2 "01:00"
        ${NSD_CB_AddString} $p5DropList2 "02:00"
        ${NSD_CB_AddString} $p5DropList2 "03:00"
        ${NSD_CB_AddString} $p5DropList2 "04:00"
        ${NSD_CB_AddString} $p5DropList2 "05:00"
        ${NSD_CB_AddString} $p5DropList2 "06:00"
        ${NSD_CB_AddString} $p5DropList2 "07:00"
        ${NSD_CB_AddString} $p5DropList2 "08:00"
        ${NSD_CB_AddString} $p5DropList2 "09:00"
        ${NSD_CB_AddString} $p5DropList2 "10:00"
        ${NSD_CB_AddString} $p5DropList2 "11:00"
        ${NSD_CB_AddString} $p5DropList2 "12:00"
        ${NSD_CB_AddString} $p5DropList2 "13:00"
        ${NSD_CB_AddString} $p5DropList2 "14:00"
        ${NSD_CB_AddString} $p5DropList2 "15:00"
        ${NSD_CB_AddString} $p5DropList2 "16:00"
        ${NSD_CB_AddString} $p5DropList2 "17:00"
        ${NSD_CB_AddString} $p5DropList2 "18:00"
        ${NSD_CB_AddString} $p5DropList2 "19:00"
        ${NSD_CB_AddString} $p5DropList2 "20:00"
        ${NSD_CB_AddString} $p5DropList2 "21:00"
        ${NSD_CB_AddString} $p5DropList2 "22:00"
        ${NSD_CB_AddString} $p5DropList2 "23:00"

        ${If} $TranscodeTime == ""
              ${NSD_CB_SelectString} $p5DropList2 "01:00"
        ${Else}
              ${NSD_CB_SelectString} $p5DropList2 $TranscodeTime
        ${EndIf}

        ${NSD_CreateLabel} 120u 100u 130u 18u "$(STRING_TVPLUGININSTALL_LINE6)"
	Pop $p5Label3

        ${NSD_CreateDropList} 255u 100u 40u 80u ""
        Pop $p5DropList3

        ${NSD_CB_AddString} $p5DropList3 "64"
        ${NSD_CB_AddString} $p5DropList3 "96"
        ${NSD_CB_AddString} $p5DropList3 "128"
        ${NSD_CB_AddString} $p5DropList3 "160"
        ${NSD_CB_AddString} $p5DropList3 "192"
        ${NSD_CB_AddString} $p5DropList3 "224"
        ${NSD_CB_AddString} $p5DropList3 "256"
        ${NSD_CB_AddString} $p5DropList3 "288"
        ${NSD_CB_AddString} $p5DropList3 "320"
        ${NSD_CB_AddString} $p5DropList3 "352"
        ${NSD_CB_AddString} $p5DropList3 "384"
        ${NSD_CB_AddString} $p5DropList3 "416"
        ${NSD_CB_AddString} $p5DropList3 "448"
        ${NSD_CB_AddString} $p5DropList3 "480"
        ${NSD_CB_AddString} $p5DropList3 "512"
        ${NSD_CB_AddString} $p5DropList3 "544"
        ${NSD_CB_AddString} $p5DropList3 "576"
        ${NSD_CB_AddString} $p5DropList3 "608"
        ${NSD_CB_AddString} $p5DropList3 "640"
        ${NSD_CB_AddString} $p5DropList3 "672"
        ${NSD_CB_AddString} $p5DropList3 "704"
        ${NSD_CB_AddString} $p5DropList3 "736"
        ${NSD_CB_AddString} $p5DropList3 "768"
        ${NSD_CB_AddString} $p5DropList3 "800"
        ${NSD_CB_AddString} $p5DropList3 "832"
        ${NSD_CB_AddString} $p5DropList3 "896"
        ${NSD_CB_AddString} $p5DropList3 "928"
        ${NSD_CB_AddString} $p5DropList3 "960"
        ${NSD_CB_AddString} $p5DropList3 "992"
        ${NSD_CB_AddString} $p5DropList3 "1024"

        ${If} $VideoBitrate == ""
              ${NSD_CB_SelectString} $p5DropList3 "256"
        ${Else}
              ${NSD_CB_SelectString} $p5DropList3 $VideoBitrate
        ${EndIf}
        
        ${NSD_CreateLabel} 120u 120u 130u 18u "$(STRING_TVPLUGININSTALL_LINE7)"
	Pop $p5Label4

        ${NSD_CreateDropList} 255u 120u 40u 80u ""
        Pop $p5DropList4

        ${NSD_CB_AddString} $p5DropList4 "32"
        ${NSD_CB_AddString} $p5DropList4 "64"
        ${NSD_CB_AddString} $p5DropList4 "96"
        ${NSD_CB_AddString} $p5DropList4 "128"
        ${NSD_CB_AddString} $p5DropList4 "160"
        ${NSD_CB_AddString} $p5DropList4 "192"
        ${NSD_CB_AddString} $p5DropList4 "224"
        ${NSD_CB_AddString} $p5DropList4 "256"

        ${If} $AudioBitrate == ""
              ${NSD_CB_SelectString} $p5DropList4 "128"
        ${Else}
              ${NSD_CB_SelectString} $p5DropList4 $AudioBitrate
        ${EndIf}

        ${NSD_CreateLabel} 120u 140u 130u 18u "$(STRING_TVPLUGININSTALL_LINE8)"
	Pop $p5Label5
	
	StrCmp $MP4Path "" 0 +2
	StrCpy $MP4Path "C:\"
        ${NSD_CreateDirRequest} 120u 160u 200u 12u "$MP4Path"
        Pop $p5Directory
        
        ${NSD_CreateBrowseButton} 255u 140u 40u 14u "$(STRING_TVPLUGININSTALL_LINE9)"
        Pop $p5Button1
        ${NSD_OnClick} $p5Button1 p5OnDirBrowseButton

	SetCtlColors $p5Dialog "" 0xffffff
	SetCtlColors $p5Headline "" 0xffffff

	SetCtlColors $p5Label1 "" 0xffffff
	SetCtlColors $p5DropList1 "" 0xffffff

	SetCtlColors $p5Label2 "" 0xffffff
	SetCtlColors $p5DropList2 "" 0xffffff

	SetCtlColors $p5Label3 "" 0xffffff
	SetCtlColors $p5DropList3 "" 0xffffff

	SetCtlColors $p5Label4 "" 0xffffff
	SetCtlColors $p5DropList4 "" 0xffffff

	SetCtlColors $p5Label5 "" 0xffffff
	SetCtlColors $p5Button1 "" 0xffffff

	SetCtlColors $p5Label6 "" 0xffffff
	SetCtlColors $p5DropList6 "" 0xffffff

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd


Function p5OnDirBrowseButton
        Pop $R0
        ${If} $R0 == $p5Button1
              ${NSD_GetText} $p5Directory $R0
              nsDialogs::SelectFolderDialog /NOUNLOAD "$(STRING_TVPLUGININSTALL_LINE8)" $R0
              Pop $R0

              ${If} $R0 != error
                    ${NSD_SetText} $p5Directory "$R0"
              ${EndIf}
        ${EndIf}
FunctionEnd

Function TranscodeOptionsValidate

       ${NSD_GetText} $p5DropList6 $Delete
       ${NSD_GetText} $p5DropList1 $TranscodeNow
       ${NSD_GetText} $p5DropList2 $TranscodeTime
       ${NSD_GetText} $p5DropList3 $VideoBitrate
       ${NSD_GetText} $p5DropList4 $AudioBitrate
       
       ${If} $Delete == "$(STRING_YES)"
         StrCpy $Delete "true"
       ${ElseIf} $Delete == "$(STRING_NO)"
         StrCpy $Delete "false"
       ${EndIf}
       
       ${If} $TranscodeNow == "$(STRING_YES)"
         StrCpy $TranscodeNow "true"
       ${ElseIf} $TranscodeNow == "$(STRING_NO)"
         StrCpy $TranscodeNow "false"
       ${EndIf}

       ${NSD_GetText} $p5Directory $MP4Path
       IfFileExists "$MP4Path\*.*" +3 0
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_TVPLUGININSTALL_LINE10)"
       Abort

       StrCmp "$MP4Path" "" 0 +3
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_TVPLUGININSTALL_LINE11)"
       Abort

  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "Delete=$Delete"
    MessageBox MB_OK|MB_ICONINFORMATION "TranscodeNow=$TranscodeNow"
    MessageBox MB_OK|MB_ICONINFORMATION "TranscodeTime=$TranscodeTime"
    MessageBox MB_OK|MB_ICONINFORMATION "VideoBitrate=$VideoBitrate"
    MessageBox MB_OK|MB_ICONINFORMATION "AudioBitrate=$AudioBitrate"
    MessageBox MB_OK|MB_ICONINFORMATION "MP4Path=$MP4Path"
  ${EndIf}

FunctionEnd