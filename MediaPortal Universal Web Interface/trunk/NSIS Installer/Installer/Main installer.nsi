;
; iPiMP installer - built with NSIS 2.45
;

;
; If this fails to compile and provides no warnings then it's likely there is a missing variable declaration
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
!include "Macros\RemoveFilesAndFolders.nsh"
!include "Macros\FileManagement.nsh"

;
; NSIS functions
;
!include "MUI.nsh"
!include "TextReplace.nsh"
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
!include "Functions\StrSlash.nsh"
!include "Functions\IsPortOpen.nsh"

;
; MUI Settings
;
!define MUI_ABORTWARNING
!define MUI_ICON "..\icons\iPiMP.ico"
!define MUI_UNICON "..\icons\uniPiMP.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "Images\iPiMPinstall.bmp"
!insertmacro LANG_LOAD "English"
!insertmacro LANG_LOAD "German"
!insertmacro LANG_LOAD "Danish"
!insertmacro LANG_LOAD "Dutch"
!insertmacro LANG_LOAD "Swedish"
!insertmacro LANG_LOAD "French"

;
; Setup pages
;
!include "WelcomeSections\Welcome.nsh"
!include "WelcomeSections\Licence.nsh"
!include "WelcomeSections\SimpleInstall.nsh"
!include "WelcomeSections\AdvancedInstall.nsh"
!include "WelcomeSections\ApacheOptions.nsh"
!include "WelcomeSections\ApacheOptions2.nsh"
!include "WelcomeSections\TranscodeOptions.nsh"
!include "WelcomeSections\InstallLocation.nsh"

;
; Installation
;
!insertmacro MUI_PAGE_INSTFILES
!include "InstallSections\CheckPrevious.nsh"
!include "InstallSections\ApacheInstall.nsh"
!include "InstallSections\ApacheModAspNet.nsh"
!include "InstallSections\WebfilesInstall.nsh"
!include "InstallSections\UpdateConfig.nsh"
!include "InstallSections\ApacheService.nsh"
!include "InstallSections\TVServerPlugin.nsh"
!include "InstallSections\MPClientPlugin.nsh"
!include "InstallSections\AdditionalIcons.nsh"
!include "InstallSections\PostInstall.nsh"
;
;
; UnInstallation
;
!include "UnInstallSections\UnMPClientPlugin.nsh"
!include "UnInstallSections\UnTVServerPlugin.nsh"
!include "UnInstallSections\UnApacheService.nsh"
!include "UnInstallSections\UnWebfilesInstall.nsh"
!include "UnInstallSections\UnApacheModAspNet.nsh"
!include "UninstallSections\UnApacheInstall.nsh"
!include "UninstallSections\Uninstall.nsh"

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "..\${PRODUCT_NAME} Setup ${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_NAME}"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "$(STRING_BRANDING)"