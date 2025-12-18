@echo off
setlocal enabledelayedexpansion

REM Build script that reads namespace from config.lua
REM and compiles GameSystem with the correct -namespace flag

set "LUA=..\__bin\Lua\lua.exe"

REM Extract systemNamespace from config.lua using Lua
for /f "usebackq tokens=*" %%a in (`"%LUA%" -e "local c = dofile('config.lua'); print(c.systemNamespace)"`) do set "NAMESPACE=%%a"

if "%NAMESPACE%"=="" (
    echo ERROR: Could not read systemNamespace from config.lua
    exit /b 1
)

echo Building GameSystem with namespace: %NAMESPACE%
echo.

REM Clean output
if exist GameSystemOutput rmdir /s /q GameSystemOutput

REM Build
dotnet run --project ..\..\CSharp.lua.Launcher -- -s GameSystem -d GameSystemOutput -roblox -namespace %NAMESPACE%

if errorlevel 1 (
    echo Build failed!
    exit /b 1
)

echo.
echo Build complete! Output in GameSystemOutput/
echo Generated code uses: local System = _G.%NAMESPACE%.System
