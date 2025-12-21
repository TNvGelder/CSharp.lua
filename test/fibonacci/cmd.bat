set dir=../../CSharp.lua.Launcher/bin/Debug/net10.0/
dotnet "%dir%CSharp.lua.Launcher.dll" -s src -d out -namespace CSharpLua
"../__bin/lua5.1/lua" launcher.lua