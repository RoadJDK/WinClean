@ECHO OFF
dotnet publish -r win-x64 -c Release --nologo -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true