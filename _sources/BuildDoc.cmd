PATH %windir%\Microsoft.NET\Framework\v3.5;%PATH%

MSBuild FireflyDoc.shfbproj /t:Rebuild /p:Configuration=Release

pause
