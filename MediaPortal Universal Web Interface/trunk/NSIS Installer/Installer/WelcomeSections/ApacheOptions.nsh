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

	StrCpy $0 $PLUGINSDIR\apache.bmp
	System::Call 'user32::LoadImage(i 0, t r0, i ${IMAGE_BITMAP}, i 0, i 0, i ${LR_LOADFROMFILE}) i.s'
	Pop $p4Image

	SendMessage $p4ImageControl ${STM_SETIMAGE} ${IMAGE_BITMAP} $p4Image

	nsDialogs::CreateControl STATIC ${WS_VISIBLE}|${WS_CHILD}|${WS_CLIPSIBLINGS} 0 120u 10u -130u 20u "Apache web server settings"
	Pop $p4Headline

	SendMessage $p4Headline ${WM_SETFONT} $Headline_font 0

        ${NSD_CreateLabel} 120u 40u 130u 18u "Enter the TCP port to listen on:$\r$\n(This will default to port 80 if left blank)"
	Pop $p4Label1
        
	${NSD_CreateText} 255u 40u 36u 12u ""
	Pop $p4Textbox1
        ${NSD_SetText} $p4Textbox1 $TCPPort

        ${NSD_CreateLabel} 120u 70u 130u 18u "Enter the IP address to listen on:$\r$\n(This will default to all IPs if left blank)"
	Pop $p4Label2

	${NSD_CreateText} 255u 70u 63u 12u ""
	Pop $p4Textbox2
        ${NSD_SetText} $p4Textbox1 $IPAddress

        ${NSD_CreateLabel} 120u 100u 130u 18u "Confirm the TV logos folder:$\r$\n(These will be used in the web interface)"
	Pop $p4Label3
	
        ${NSD_CreateDirRequest} 120u 120u 200u 12u "$LogoPath"
        Pop $p4Directory
        
        ${NSD_CreateBrowseButton} 255u 100u 40u 14u "Browse..."
        Pop $p4Button1
        ${NSD_OnClick} $p4Button1 p4OnDirBrowseButton

        ${NSD_CreateLabel} 120u 140u 130u 18u "Select a login timeout:$\r$\n(After that period you'll have to re-login)"
	Pop $p4Label4

        ${NSD_CreateDropList} 255u 142u 40u 80u ""
        Pop $p4DropList

        ${NSD_CB_AddString} $p4DropList "30 mins"
        ${NSD_CB_AddString} $p4DropList "1 hour"
        ${NSD_CB_AddString} $p4DropList "8 hours"
        ${NSD_CB_AddString} $p4DropList "1 day"
        ${NSD_CB_AddString} $p4DropList "1 week"
        ${NSD_CB_AddString} $p4DropList "1 month"
        ${NSD_CB_AddString} $p4DropList "1 year"

        ${Select} $Timeout
            ${Case} "30"
              ${NSD_CB_SelectString} $p4DropList "30 mins"
            ${Case} "60"
              ${NSD_CB_SelectString} $p4DropList "1 hour"
            ${Case} "480"
              ${NSD_CB_SelectString} $p4DropList "8 hours"
            ${Case} "1440"
              ${NSD_CB_SelectString} $p4DropList "1 day"
            ${Case} "10080"
              ${NSD_CB_SelectString} $p4DropList "1 week"
            ${Case} "44640"
              ${NSD_CB_SelectString} $p4DropList "1 month"
            ${Case} "525600"
              ${NSD_CB_SelectString} $p4DropList "1 year"
            ${CaseElse}
              ${NSD_CB_SelectString} $p4DropList "30 mins"
        ${EndSelect}

        ${If} $Timeout == ""
        ${Else}
              ${NSD_CB_SelectString} $p4DropList $Timeout
        ${EndIf}

        ${NSD_CreateLabel} 120u 160u 130u 18u "Select a size for long lists:$\r$\n(To make long lists more manageable.)"
	Pop $p4Label5

        ${NSD_CreateDropList} 255u 162u 40u 80u ""
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
              nsDialogs::SelectFolderDialog /NOUNLOAD "Locate your TV logo folder." $R0
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
              MessageBox MB_ICONEXCLAMATION|MB_OK "$IPAddress is not a valid ip address."
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
       MessageBox MB_ICONINFORMATION|MB_OK "Cannot find $LogoPath"
       Abort

       StrCmp "$LogoPath" "" 0 +3
       MessageBox MB_ICONINFORMATION|MB_OK "No TV logo path provided."
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