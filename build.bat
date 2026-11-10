@echo off
setlocal
cd /d "%~dp0"

echo Building Universal Mic Mute...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true

if errorlevel 1 (
    echo Build failed.
    pause
    exit /b 1
)

set "OUT=bin\Release\net8.0-windows\win-x64\publish\UniversalMicMute.exe"
if not exist "%OUT%" (
    echo EXE not found at %OUT%
    pause
    exit /b 1
)

if not exist "Umm" mkdir Umm
copy /Y "%OUT%" "Umm\UniversalMicMute.exe" >nul

echo.
echo Done. EXE is in the Umm folder.
pause
