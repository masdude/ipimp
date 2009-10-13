!macro CheckVersion COMPONENT

  ${If} "${COMPONENT}" == "TV"
    ${VersionCompare} "$TVServerVer" "${MIN_MPVERSION}" $1
  ${EndIf}

  ${If} "${COMPONENT}" == "MP"
    ${VersionCompare} "$MPClientVer" "${MIN_MPVERSION}" $1
  ${EndIf}

  ${If} "${COMPONENT}" == "TV"
  ${AndIf} $1 == 2
      MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_CHECKVERSION_LINE1)"
      Abort
  ${EndIf}

  ${If} "${COMPONENT}" == "MP"
  ${AndIf} $1 == 2
      MessageBox MB_ICONINFORMATION|MB_OK "$(STRING_CHECKVERSION_LINE2)"
      Abort
  ${EndIf}

!macroend