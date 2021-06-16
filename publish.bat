dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
"%ProgramFiles%\WinRAR\WinRAR.exe" a WinClean.rar .\bin\Debug\netcoreapp3.1\win-x64\publish\WinClean.exe -AP / .\bin\Debug\netcoreapp3.1\win-x64\publish\WinClean.pdb -AP /
git add .
git commit -m "Auto Commit"
git pull
git push