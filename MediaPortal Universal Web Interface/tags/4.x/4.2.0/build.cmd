REM ====================
REM iPiMPConfigurePlugin
REM ====================

REM Clear down the bin folder
DEL /Q iPiMPConfigurePlugin\bin\Release\*.*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update assembly version with SVN revision
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\iPiMPConfigurePlugin" "%CD%\iPiMPConfigurePlugin\My Project\AssemblyInfo_temp.vb" "%CD%\iPiMPConfigurePlugin\My Project\AssemblyInfo.vb"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the iPiMPConfigurePlugin project
PUSHD iPiMPConfigurePlugin
%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe /p:Configuration=Release
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
POPD

REM ====================
REM iPiMPTranscodeToMP4
REM ====================

REM Clear down the bin folder
DEL /Q iPiMPTranscodeToMP4\bin\Release\*.*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update assembly version with SVN revision
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\iPiMPTranscodeToMP4" "%CD%\iPiMPTranscodeToMP4\My Project\AssemblyInfo_temp.vb" "%CD%\iPiMPTranscodeToMP4\My Project\AssemblyInfo.vb"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update the installer
COPY /Y "%CD%\NSIS Installer\Installer\InstallSections\TVServerPlugin_Release.nsh" "%CD%\NSIS Installer\Installer\InstallSections\TVServerPlugin.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the iPiMPTranscodeToMP4 project
PUSHD iPiMPTranscodeToMP4
%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe /p:Configuration=Release
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
POPD

REM ====================
REM MPClientController
REM ====================

REM Clear down the bin folder
DEL /Q MPClientController\bin\Release\*.*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update assembly version with SVN revision
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\MPClientController" "%CD%\MPClientController\My Project\AssemblyInfo_temp.vb" "%CD%\MPClientController\My Project\AssemblyInfo.vb"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

COPY /Y "%CD%\NSIS Installer\Installer\InstallSections\MPClientPlugin_Release.nsh" "%CD%\NSIS Installer\Installer\InstallSections\MPClientPlugin.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the MPClientController project
PUSHD MPClientController
%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe /p:Configuration=Release
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
POPD

REM ==================
REM Website
REM ==================

REM Clear down the bin folder
DEL /Q Website\bin\*.*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Clear down the Include folder
RD /S /Q "NSIS Installer\Include\Aspx\"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update assembly version with SVN revision
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\Website" "%CD%\Website\My Project\AssemblyInfo_temp.vb" "%CD%\Website\My Project\AssemblyInfo.vb"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the website project
PUSHD Website
%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe /p:Configuration=Release
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copy all reference DLLs
XCOPY bin\*.dll "..\NSIS Installer\Include\Aspx\bin" /Y /I
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Publish the website project
%WINDIR%\Microsoft.NET\Framework\v3.5\MSBuild.exe /target:_CopyWebApplication /property:OutDir=..\NSISIn~1\Include\Aspx\;WebProjectOutputDir=..\NSISIn~1\Include\Aspx\ /p:Configuration=Release
POPD
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the website file installer
%WINDIR%\SYSTEM32\CScript //NoLogo "%CD%\NSIS Installer\Include\ListFiles.vbs" "%CD%\NSIS Installer\Include\Aspx" >"%CD%\NSIS Installer\Installer\InstallSections\Webfiles.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM ==================
REM Installer
REM ==================

REM Update installer version with website SVN revision
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\Website" "%CD%\NSIS Installer\Installer\Definitions\ProductDetails_temp.nsh" "%CD%\NSIS Installer\Installer\Definitions\ProductDetails.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the installer
"C:\Program Files (x86)\NSIS\makensis.exe" /V4 /NOTIFYHWND 1640592 "%CD%\NSIS Installer\Installer\Main installer.nsi"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

pause
GOTO :EOF

:ERROR
ECHO ===================================
ECHO ERROR: %ERRORLEVEL%
ECHO ===================================
