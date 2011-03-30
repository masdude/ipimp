@ECHO OFF
SET /P BUILDVER=Enter the build version (e.g. 5.1.0): 
ECHO Build version is %BUILDVER%

SET /P AGREE=Is this correct? 
IF /I %AGREE% NEQ Y GOTO :EOF

REM ====================
REM iPiMPConfigurePlugin
REM ====================
CALL :BUILD iPiMPConfigurePlugin vb

REM ====================
REM iPiMPTranscodeToMP4
REM ====================
CALL :BUILD iPiMPTranscodeToMP4 vb
REM Update the installer
COPY /Y "%CD%\NSIS Installer\Installer\InstallSections\TVServerPlugin_Release.nsh" "%CD%\NSIS Installer\Installer\InstallSections\TVServerPlugin.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM ====================
REM MPClientController
REM ====================
CALL :BUILD MPClientController vb
REM Update the installer
COPY /Y "%CD%\NSIS Installer\Installer\InstallSections\MPClientPlugin_Release.nsh" "%CD%\NSIS Installer\Installer\InstallSections\MPClientPlugin.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM ====================
REM MPWebServices
REM ====================
CALL :BUILD MPWebServices cs
@ECHO ON
REM ==================
REM Website
REM ==================
CALL :BUILD Website vb

REM Clear down the Include folder
PUSHD Website
RD /S /Q "NSIS Installer\Include\Aspx\"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Copy all reference DLLs
XCOPY bin\Release\*.dll "..\NSIS Installer\Include\Aspx\bin" /Y /I
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Publish the website project
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /target:_CopyWebApplication /property:OutDir=..\NSISIn~1\Include\Aspx\;WebProjectOutputDir=..\NSISIn~1\Include\Aspx\ /p:Configuration=Release
POPD
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the website file installer
%WINDIR%\SYSTEM32\CScript //NoLogo "%CD%\NSIS Installer\Include\ListFiles.vbs" "%CD%\NSIS Installer\Include\Aspx" >"%CD%\NSIS Installer\Installer\InstallSections\Webfiles.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM ==================
REM Installer
REM ==================

REM Update installer version
CSCRIPT //NOLogo "NSIS Installer\Include\UpdateBuildNumber.vbs" "%CD%\NSIS Installer\Installer\Definitions\ProductDetails_temp.nsh" %BUILDVER%
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update installer revision with website SVN revision
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\Website" "%CD%\NSIS Installer\Installer\Definitions\ProductDetails_temp2.nsh" "%CD%\NSIS Installer\Installer\Definitions\ProductDetails.nsh"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the installer
"C:\Program Files (x86)\NSIS\makensis.exe" /V4 /NOTIFYHWND 1640592 "%CD%\NSIS Installer\Installer\Main installer.nsi"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

PAUSE
GOTO :EOF

:BUILD
SET PROJECT=%1
SET LANGUAGE=%2
ECHO Compiling %PROJECT%

REM Clear down the bin folder
DEL /Q %PROJECT%\bin\Release\*.*
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update assembly version
IF /I %LANGUAGE% EQU VB CSCRIPT //NOLogo "NSIS Installer\Include\UpdateBuildNumber.vbs" "%CD%\%PROJECT%\My Project\AssemblyInfo_temp.vb" %BUILDVER%
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

IF /I %LANGUAGE% EQU CS CSCRIPT //NOLogo "NSIS Installer\Include\UpdateBuildNumber.vbs" "%CD%\%PROJECT%\Properties\AssemblyInfo_temp.cs" %BUILDVER%
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Update assembly revision with SVN revision
IF /I %LANGUAGE% EQU VB "C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\%PROJECT%" "%CD%\%PROJECT%\My Project\AssemblyInfo_temp2.vb" "%CD%\%PROJECT%\My Project\AssemblyInfo.vb"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

IF /I %LANGUAGE% EQU CS "C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" "%CD%\%PROJECT%" "%CD%\%PROJECT%\Properties\AssemblyInfo_temp2.cs" "%CD%\%PROJECT%\Properties\AssemblyInfo.cs"
IF %ERRORLEVEL% NEQ 0 GOTO ERROR

REM Build the project
PUSHD %PROJECT%
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:Configuration=Release
IF %ERRORLEVEL% NEQ 0 GOTO ERROR
POPD
GOTO :EOF


:ERROR
ECHO ===================================
ECHO ERROR: %ERRORLEVEL%
ECHO ===================================
PAUSE

:EOF
