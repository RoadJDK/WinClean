dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
"%ProgramFiles%\WinRAR\WinRAR.exe" a -ep -sfx -s1 WinClean.exe .\bin\Debug\netcoreapp3.1\win-x64\publish\WinClean.exe .\bin\Debug\netcoreapp3.1\win-x64\publish\WinClean.pdb
git add .
git commit -m "Auto Commit"
git pull
git push