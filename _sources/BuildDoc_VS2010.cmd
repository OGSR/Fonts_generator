PATH %windir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

MSBuild FireflyDoc_VS2010.shfbproj /t:Rebuild /p:Configuration=Release

pause
