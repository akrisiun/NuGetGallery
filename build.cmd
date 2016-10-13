
nuget restore NuGetGallery.sln
call msbuild14 NuGetGallery.sln
@REM PowerShell.exe -NoProfile -ExecutionPolicy ByPass ".\build.ps1 -Configuration 'Release' -Verbose"

@PAUSE