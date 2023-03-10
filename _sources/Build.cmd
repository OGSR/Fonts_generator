PATH %windir%\Microsoft.NET\Framework\v3.5;%PATH%

MSBuild Firefly.sln /t:Rebuild /p:Configuration=Release

copy Doc\Readme.*.txt ..\Bin\
copy Doc\UpdateLog.*.txt ..\Bin\
copy Doc\License.*.txt ..\Bin\

pause
