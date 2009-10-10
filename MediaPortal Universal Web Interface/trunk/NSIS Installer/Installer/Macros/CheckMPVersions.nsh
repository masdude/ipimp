!macro CheckVersion COMPONENT

  ${If} "${COMPONENT}" == "TV"
    ${VersionCompare} "$TVServerVer" "${MIN_MPVERSION}" $1
  ${EndIf}

  ${If} "${COMPONENT}" == "MP"
    ${VersionCompare} "$MPClientVer" "${MIN_MPVERSION}" $1
  ${EndIf}

  ${If} "${COMPONENT}" == "TV"
  ${AndIf} $1 == 2
      MessageBox MB_ICONINFORMATION|MB_OK "MediaPortal TV Server ${MIN_MPVERSION} or greater must be installed $\r$\nto install this component."
      Abort
  ${EndIf}

  ${If} "${COMPONENT}" == "MP"
  ${AndIf} $1 == 2
      MessageBox MB_ICONINFORMATION|MB_OK "MediaPortal Client ${MIN_MPVERSION} or greater must be installed $\r$\nto install this component."
      Abort
  ${EndIf}

!macroend