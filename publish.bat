dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
git add .
git commit -m "Auto Commit"
git pull
git push