Section -AdditionalIcons
  SetOutPath $INSTDIR

  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateDirectory "$SMPROGRAMS\iPiMP"
  CreateShortCut "$SMPROGRAMS\iPiMP\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\iPiMP\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd