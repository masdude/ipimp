Section Uninstall

  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"

  Delete "$SMPROGRAMS\iPiMP\Uninstall.lnk"
  Delete "$SMPROGRAMS\iPiMP\Website.lnk"

  DetailPrint "Removing other iPiMP files"
  !insertmacro RemoveFilesAndSubDirs "$INSTDIR"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  
SectionEnd