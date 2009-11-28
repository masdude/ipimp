Page custom ApacheOptions ApacheOptionsValidate

Var p4Dialog
Var p4ImageControl
Var p4Image
Var p4Headline

Var p4Label1
Var p4Textbox1

Var p4Label2
Var p4Textbox2

Var p4Label3
Var p4Directory
Var p4Button1

Var p4Label4
Var p4DropList

Var p4Label5
Var p4DropList2

Function ApacheOptions

        StrCmp $InstallApache "1" +2
        Abort

	nsDialogs::Create 1044
	Pop $p4Dialog

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS}|${SS_BITMAP} 0 0 0 109u 193u ""
	Pop $p4ImageControl

	StrCpy $0 $PLUGINSDIR\ipimpapache.bmp
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p4Image

	SendMessage $p4ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p4Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "$(STRING_APACHEINSTALL_LINE1)"
	Pop $p4Headline

	SendMessage $p4Headline ${WM_SETFONT} $Headline_font 0

        ${NSD_CreateLabel} 120u 40u 130u 18u "$(STRING_APACHEINSTALL_LINE2)"
	Pop $p4Label1
        
	${NSD_CreateText} 280u 40u 40u 12u ""
	Pop $p4Textbox1
        ${NSD_SetText} $p4Textbox1 $TCPPort

        ${NSD_CreateLabel} 120u 70u 135u 28u "$(STRING_APACHEINSTALL_LINE3)"
	Pop $p4Label2

	${NSD_CreateText} 255u 70u 65u 12u ""
	Pop $p4Textbox2
        ${NSD_SetText} $p4Textbox1 $IPAddress

        ${NSD_CreateLabel} 120u 100u 135u 18u "$(STRING_APACHEINSTALL_LINE4)"
	Pop $p4Label3
	
        ${NSD_CreateDirRequest} 120u 120u 200u 12u "$LogoPath"
        Pop $p4Directory
        
        ${NSD_CreateBrowseButton} 280u 100u 40u 14u "Browse..."
        Pop $p4Button1
        ${NSD_OnClick} $p4Button1 p4OnDirBrowseButton

        ${NSD_CreateLabel} 120u 140u 160u 18u "$(STRING_APACHEINSTALL_LINE5)"
	Pop $p4Label4

        ${NSD_CreateDropList} 280u 142u 40u 80u ""
        Pop $p4DropList

        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE6)"
        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE7)"
        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE8)"
        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE9)"
        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE10)"
        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE11)"
        ${NSD_CB_AddString} $p4DropList "$(STRING_APACHEINSTALL_LINE12)"

        ${Select} $Timeout
            ${Case} "30"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE6)"
            ${Case} "60"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE7)"
            ${Case} "480"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE8)"
            ${Case} "1440"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE9)"
            ${Case} "10080"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE10)"
            ${Case} "44640"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE11)"
            ${Case} "525600"
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE12)"
            ${CaseElse}
              ${NSD_CB_SelectString} $p4DropList "$(STRING_APACHEINSTALL_LINE6)"
        ${EndSelect}

        ${If} $Timeout == ""
        ${Else}
              ${NSD_CB_SelectString} $p4DropList $Timeout
        ${EndIf}

        ${NSD_CreateLabel} 120u 160u 160u 18u "$(STRING_APACHEINSTALL_LINE13)"
	Pop $p4Label5

        ${NSD_CreateDropList} 280u 162u 40u 80u ""
        Pop $p4DropList2

        ${NSD_CB_AddString} $p4DropList2 "5"
        ${NSD_CB_AddString} $p4DropList2 "10"
        ${NSD_CB_AddString} $p4DropList2 "15"
        ${NSD_CB_AddString} $p4DropList2 "20"
        ${NSD_CB_AddString} $p4DropList2 "50"
        ${NSD_CB_AddString} $p4DropList2 "100"

        ${If} $Pagesize == ""
              ${NSD_CB_SelectString} $p4DropList2 "10"
        ${Else}
              ${NSD_CB_SelectString} $p4DropList2 $Pagesize
        ${EndIf}

	SetCtlColors $p4Dialog "" 0xffffff
	SetCtlColors $p4Headline "" 0xffffff

	SetCtlColors $p4Label1 "" 0xffffff
	SetCtlColors $p4Textbox1 "" 0xffffff

	SetCtlColors $p4Label2 "" 0xffffff
	SetCtlColors $p4Textbox2 "" 0xffffff

	SetCtlColors $p4Label3 "" 0xffffff
	SetCtlColors $p4Button1 "" 0xffffff

	SetCtlColors $p4Label4 "" 0xffffff

	SetCtlColors $p4Label5 "" 0xffffff

	Call HideControls

	nsDialogs::Show

	Call ShowControls

	System::Call gdi32::DeleteObject(i$IMAGE)

FunctionEnd


Function p4OnDirBrowseButton
        Pop $R0
        ${If} $R0 == $p4Button1
              ${NSD_GetText} $p4Directory $R0
              nsDialogs::SelectFolderDialog /NOUNLOAD "$(STRING_APACHEINSTALL_LINE14)" $R0
              Pop $R0

              ${If} $R0 != error
                    ${NSD_SetText} $p4Directory "$R0"
              ${EndIf}
        ${EndIf}
FunctionEnd

Function ApacheOptionsValidate

        ${NSD_GetText} $p4Textbox2 $IPAddress
        StrCmp $IPAddress "" p4conf
        Push $IPAddress
        Call ValidateIP
        ${If} ${Errors}
              ClearErrors
              MessageBox MB_ICONEXCLAMATION|MB_OK "$(STRING_APACHEINSTALL_LINE15)"
              Abort
        ${EndIf}

        p4conf:
        ;Build the IP/Port string for httpd.conf
        ${NSD_GetText} $p4Textbox1 $TCPPort
        StrCmp $TCPPort "" +1 +2
        StrCpy $TCPPort "80"
        StrCmp $IPAddress "" +1 +3
        StrCpy $Listen "$TCPPort"
        Goto +2
        StrCpy $Listen "$IPAddress:$TCPPort"

       ${NSD_GetText} $p4Directory $LogoPath
       IfFileExists "$LogoPath\*.*" +3 0
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_APACHEINSTALL_LINE16)"
       Abort

       StrCmp "$LogoPath" "" 0 +3
       MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_APACHEINSTALL_LINE17)"
       Abort

       ${NSD_GetText} $p4DropList $Timeout
       ${Select} $Timeout
            ${Case} "30 mins"
                    StrCpy $Timeout "30"
            ${Case} "1 hour"
                    StrCpy $Timeout "60"
            ${Case} "8 hours"
                    StrCpy $Timeout "480"
            ${Case} "1 day"
                    StrCpy $Timeout "1440"
            ${Case} "1 week"
                    StrCpy $Timeout "10080"
            ${Case} "1 month"
                    StrCpy $Timeout "44640"
            ${Case} "1 year"
                    StrCpy $Timeout "525600"
            ${CaseElse}
                    StrCpy $Timeout "30"
       ${EndSelect}

       ${NSD_GetText} $p4DropList2 $Pagesize
       
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "Listen=$Listen"
    MessageBox MB_OK|MB_ICONINFORMATION "LogoPath=$LogoPath"
    MessageBox MB_OK|MB_ICONINFORMATION "Timeout=$Timeout"
    MessageBox MB_OK|MB_ICONINFORMATION "Pagesize=$Pagesize"
  ${EndIf}
  
FunctionEnd