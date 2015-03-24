To help with troubleshooting you need to recreate the issue with debug logging enabled...
  1. Stop MediaPortal
  1. Set your logging level to Debug
  1. Start MediaPortal
  1. Retry what you were attempting in MediaPortal
  1. Then run the following commands...

> %comspec% /c ipconfig /all > c:\ipconfig.txt<br>
<blockquote>%comspec% /c netstat -abnop TCP > c:\netstat.txt<br>
%comspec% /c dir "%programfiles%\ipimp" /o:n /s > c:\ipimpfiles.txt<br></blockquote>

Once complete collect the following files, then zip & <a href='http://forum.team-mediaportal.com/ipimp-518/'>post</a> them:<br>
<br>
<blockquote>%appdata%\Team MediaPortal\MediaPortal\log\error.log<br>
%appdata%\Team MediaPortal\MediaPortal\log\MediaPortal.log<br>
%appdata%\Team MediaPortal\MediaPortal TV Server\log\error.log<br>
%appdata%\Team MediaPortal\MediaPortal TV Server\log\tv.log<br>
%appdata%\Team MediaPortal\MediaPortal TV Server\Gentle.Config<br>
C:\Program Files\iPiMP\apache\conf\iPiMP.conf<br>
C:\Program Files\iPiMP\apache\logs\error.log<br>
C:\Program Files\iPiMP\apache\logs\access.log<br>
C:\Program Files\iPiMP\aspx\web.config<br>
C:\Program Files\iPiMP\aspx\App_Data\uWiMP.db<br>
C:\ipconfig.txt<br>
C:\netstat.txt<br>
C:\ipimpfiles.txt<br>