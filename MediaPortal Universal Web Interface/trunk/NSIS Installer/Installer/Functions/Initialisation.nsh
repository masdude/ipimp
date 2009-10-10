Function .onInit

  InitPluginsDir
  CreateFont $Headline_font "Tahoma" "14" "700"

  ReadRegStr $ServerPath HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "InstallPath"
  ReadRegStr $TVServerMaj HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "VersionMajor"
  ReadRegStr $TVServerMin HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "VersionMinor"
  ReadRegStr $TVServerRev HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal TV Server" "VersionRevision"
  ReadRegStr $MPClientMaj HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "VersionMajor"
  ReadRegStr $MPClientMin HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "VersionMinor"
  ReadRegStr $MPClientRev HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "VersionRevision"
  ReadRegStr $ClientPath HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MediaPortal" "InstallPath"
  ReadRegStr $CommonApp HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\explorer\Shell Folders" "Common AppData"
  ReadRegStr $DotNetDir HKLM "SOFTWARE\Microsoft\.NETFramework" "InstallRoot"

  StrCpy $LogoPath "$CommonApp\Team MediaPortal\MediaPortal\thumbs\tv\logos"
  StrCpy $TVPath "$CommonApp\Team MediaPortal\MediaPortal TV Server"
  StrCpy $TVServerVer "$TVServerMaj.$TVServerMin.$TVServerRev"
  StrCpy $MPClientVer "$MPClientMaj.$MPClientMin.$MPClientRev"

FunctionEnd