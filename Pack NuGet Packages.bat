if not exist NuGet mkdir NuGet

del /Q NuGet\*.*

.\src\.nuget\NuGet.exe pack .\src\Prism.NinjectExtensions.nuspec -OutputDirectory NuGet

pause
