# Build script that reads namespace from config.lua
# and compiles GameSystem with the correct -namespace flag

$ErrorActionPreference = "Stop"

$lua = "..\__bin\Lua\lua.exe"

# Extract systemNamespace from config.lua using Lua
$namespace = & $lua -e "local c = dofile('config.lua'); print(c.systemNamespace)"

if ([string]::IsNullOrEmpty($namespace)) {
    Write-Error "ERROR: Could not read systemNamespace from config.lua"
    exit 1
}

Write-Host "Building GameSystem with namespace: $namespace" -ForegroundColor Cyan
Write-Host ""

# Clean output (preserve structure, remove only .lua files)
if (Test-Path "GameSystemOutput") {
    Get-ChildItem "GameSystemOutput" -Filter "*.lua" | Remove-Item -Force
}
else {
    New-Item -ItemType Directory -Path "GameSystemOutput" | Out-Null
}

# Ensure Rojo meta file exists
$metaContent = @'
{
  "className": "Folder"
}
'@
Set-Content -Path "GameSystemOutput\default.meta.json" -Value $metaContent

# Build
& dotnet run --project "..\..\CSharp.lua.Launcher" -- -s GameSystem -d GameSystemOutput -roblox -namespace $namespace

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

Write-Host ""
Write-Host "Build complete! Output in GameSystemOutput/" -ForegroundColor Green
Write-Host "Generated code uses: local System = _G.$namespace.System" -ForegroundColor Green
