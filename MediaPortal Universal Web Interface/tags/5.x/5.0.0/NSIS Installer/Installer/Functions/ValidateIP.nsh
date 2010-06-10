;
;  ValidateIP   http://nsis.sourceforge.net/Validate_IP_function
;
!include LogicLib.nsh
!include WordFunc.nsh
 
!insertmacro WordFind
!insertmacro StrFilter
 
Function ValidateIP
 
  Exch $0
  Push $1
  Push $2
 
  ${StrFilter} $0 1 "." "" $1
  ${If} $0 != $1
    # invalid charcaters used
    #   example: a127.0.0.1
    Goto error
  ${EndIf}
 
  ${WordFind} $0 . "#" $1
  ${If} $1 != 4
    # wrong number of numbers
    #   example: 127.0.0.
    Goto error
  ${EndIf}
 
  ${WordFind} $0 . "*" $1
  ${If} $1 != 3
    # wrong number of dots
    #   example: 127.0.0.1.
    Goto error
  ${EndIf}
 
  ${For} $2 1 4
    ${WordFind} $0 . +$2 $1
 
    ${If} $1 > 255
    ${OrIf} $1 < 0
      # invalid number
      #   example: 500.0.0.1
      Goto error
    ${EndIf}
  ${Next}
 
  Pop $2
  Pop $1
  Pop $0
 
  ClearErrors
 
  Return
 
  error:
 
    Pop $2
    Pop $1
    Pop $0
 
    SetErrors
 
FunctionEnd