Section CheckPrevious

  Start:
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "UninstallString"
  ${WordFind} $0 "\" "-2{*" $1
  ${WordReplace} $1 '"' "" "+" $1

  ${If} $0 == ""
    Goto End
  ${Else}
    MessageBox MB_YESNO|MB_ICONQUESTION "$(STRING_CHECKPREVIOUS_LINE1)" IDYES Uninstall IDNO Warning
  ${EndIf}

  Uninstall:
    GetDlgItem $4 $HWNDPARENT 1
    ShowWindow $4 ${SW_HIDE}
    ExecWait '"$0" _?=$1' $2
    ShowWindow $4 ${SW_SHOW}
    ${If} $2 == "0"
      Goto Start
    ${OrIf} $2 == "1223"
      Goto Start
    ${Else}
      MessageBox MB_OK|MB_ICONSTOP "$(STRING_CHECKPREVIOUS_LINE2)"
      Goto Start
    ${EndIf}

  Warning:
    MessageBox MB_YESNO|MB_ICONQUESTION "$(STRING_CHECKPREVIOUS_LINE3)" IDYES Uninstall IDNO Warning2

  Warning2:
    MessageBox MB_OK|MB_ICONEXCLAMATION "$(STRING_CHECKPREVIOUS_LINE4)"

  End:

SectionEnd