;
; iPiMP installer - built with NSIS 2.45
;
; version 4.0.0
;

;
; Custom definitions
;
!include "Definitions\Global.nsh"
!include "Definitions\ProductDetails.nsh"
!include "Definitions\MediaPortalSupportedVersions.nsh"

;
; Custom variables
;
!include "Variables\Global.nsh"

;
; Custom macros
;
!include "Macros\Language.nsh"
!include "Macros\CheckMPVersions.nsh"

;
; NSIS functions
;
!include "MUI.nsh"
;!include "TextReplace.nsh"
!include "WordFunc.nsh"
!include "Sections.nsh"
!include "LogicLib.nsh"
!include "FileFunc.nsh"
!include "WinMessages.nsh"
!include "nsDialogs.nsh"

;
; Other functions
;
!include "Functions\ValidateIP.nsh"
!include "Functions\Initialisation.nsh"

;
; MUI Settings
;
!define MUI_ABORTWARNING
!define MUI_ICON "..\icons\iPiMP.ico"
!define MUI_UNICON "..\icons\uniPiMP.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "Images\iPiMPinstall.bmp"

!insertmacro LANG_LOAD "English"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "${PRODUCT_NAME}_${PRODUCT_VERSION}_setup.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_NAME}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "$(STRING_BRANDING)"

;-------------
; Setup pages
;-------------
!include "WelcomeSections\Welcome.nsh"
!include "WelcomeSections\SimpleInstall.nsh"
!include "WelcomeSections\AdvancedInstall.nsh"
!include "WelcomeSections\ApacheOptions.nsh"

;!insertmacro MUI_PAGE_INSTFILES

Section -AdditionalIcons
  SetOutPath $INSTDIR

  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateDirectory "$SMPROGRAMS\iPiMP"
  CreateShortCut "$SMPROGRAMS\iPiMP\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\iPiMP\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd