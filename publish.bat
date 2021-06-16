dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
"%ProgramFiles%\WinRAR\WinRAR.exe" -cp winrar_profile
git add .
git commit -m "Auto Commit"
git push