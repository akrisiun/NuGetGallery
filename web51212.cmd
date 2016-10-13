@ECHO http://localhost:51212
set dir=%~dp0src\NuGetGallery\

"%ProgramFiles%\IIS Express\IISExpress.exe" /port:51212 /path:%dir% /clr:4.0 /systray:true /ntlm
@PAUSE 
