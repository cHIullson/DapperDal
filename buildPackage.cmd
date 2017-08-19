@call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat"
MSBuild DapperDal.csproj /t:Clean;Rebuild /p:Configuration=Release
..\.nuget\nuget.exe pack DapperDal.nuspec /o ..\releases