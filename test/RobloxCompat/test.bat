@echo off
set lua="../__bin/Lua/lua.exe"

echo Running Roblox compatibility tests...
%lua% test.lua
if errorlevel 1 goto :error

echo.
echo Running Roblox transform validation tests...
%lua% test_roblox_transforms.lua
if errorlevel 1 goto :error

echo.
echo All test suites passed!
goto :end

:error
echo.
echo Tests failed!
exit /b 1

:end
