@echo off
pushd "%~dp0"

pushd RobloxCompat
call test
if not %errorlevel%==0 goto :Fail
popd

pushd TestCases
call test
if not %errorlevel%==0 goto :Fail
popd

pushd self-compiling
call test
if not %errorlevel%==0 goto :Fail
popd

pushd BridgeNetTests\Tests
call test
if not %errorlevel%==0 goto :Fail
popd

popd
exit /b 0

:Fail
popd
popd
pause
exit /b 1
