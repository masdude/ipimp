Section ApacheModAspNet

  StrCmp $InstallModAspNet "1" +5
  ${If} ${IPIMPDEBUG} == "1"
    MessageBox MB_OK|MB_ICONINFORMATION "ApacheModAspNet skipped"
  ${EndIf}
  Return

  DetailPrint "Installing Apache ASP.NET module"

  SetOverwrite TRY

  SetOutPath "$INSTDIR\Utilities\Apache"

  File "..\Include\Utilities\mod_aspdotnet\Apache.Web.dll"
  File "..\Include\Utilities\mod_aspdotnet\Apache.Web.Helpers.netmodule"
  File "..\Include\Utilities\mod_aspdotnet\gacutil.exe"
  File "..\Include\Utilities\mod_aspdotnet\gacutil.exe.config"

  SetOutPath "$INSTDIR\Apache\conf"
  File "..\Include\Apache\conf\iPiMPinclude.conf"

  SetOutPath "$INSTDIR\Apache\modules"
  File "..\Include\Utilities\mod_aspdotnet\mod_aspdotnet.so"

  DetailPrint "Registering Apache ASP.NET module"
  Sleep 100
  ExecDos::exec /TIMEOUT=10000 /DETAILED "$DotNetDir\v2.0.50727\RegAsm.exe /verbose /nologo $\"$INSTDIR\Utilities\Apache\Apache.Web.dll$\""
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "RegAsm register failed - Reason: $0"

  DetailPrint "Installing Apache ASP.NET module in GAC"
  Sleep 100
  ExecDos::exec /TIMEOUT=2000 /DETAILED "$INSTDIR\Utilities\Apache\gacutil.exe /nologo /i $\"$INSTDIR\Utilities\Apache\Apache.Web.dll$\" /r FILEPATH $\"$INSTDIR\Utilities\Apache\Apache.Web.dll$\" $\"mod_aspdotnet Apache.Web.dll$\""
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "GACUtil install failed - Reason: $0"

  DetailPrint "Granting SYSTEM access to ASP.NET"
  Sleep 100
  ExecDos::exec /TIMEOUT=2000 /DETAILED "$DotNetDir\v2.0.50727\aspnet_regiis.exe -ga SYSTEM"
  Pop $0 # return value
  StrCmp $0 "0" +2
  MessageBox MB_OK|MB_ICONSTOP "Aspnet_Regiis grant access failed - Reason: $0"

SectionEnd
