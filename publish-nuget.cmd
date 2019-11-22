@echo off

del /f ".\EagleServer\bin\Debug\*.nupkg"

SET /p apiKey=Input the Nuget API Key:  

dotnet pack 

dotnet nuget push .\EagleServer\bin\Debug\EagleServer.*.nupkg -s nuget.org -k %apiKey%

