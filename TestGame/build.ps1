# TestGame build script
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$luaExe = "$repoRoot\test\__bin\Lua\lua.exe"
$launcherProject = "$repoRoot\CSharp.lua.Launcher"
$robloxTypesLib = "$repoRoot\RobloxTypes\bin\Debug\net10.0\RobloxTypes.dll"
$metadataFiles = "$repoRoot\RobloxMetadata\Roblox.xml;$repoRoot\RobloxMetadata\Roblox.Generated.xml"

# Get namespace from config.lua to stay in sync
$namespace = & $luaExe -e "local c = dofile('$scriptDir/config.lua'); print(c.systemNamespace)"

Write-Host "Building TestGame with namespace: $namespace" -ForegroundColor Cyan

# Build RobloxTypes first (needed for transpilation)
Write-Host "Building RobloxTypes..." -ForegroundColor Yellow
dotnet build "$repoRoot\RobloxTypes\RobloxTypes.csproj" -c Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: RobloxTypes build failed" -ForegroundColor Red
    exit 1
}

# Ensure output directory exists
if (-not (Test-Path "$scriptDir\Output")) {
    New-Item -ItemType Directory -Path "$scriptDir\Output" | Out-Null
}

# Run the transpiler
Write-Host "Running transpiler..." -ForegroundColor Yellow
dotnet run --project $launcherProject -- `
    -s "$scriptDir" `
    -d "$scriptDir\Output" `
    -l "$robloxTypesLib" `
    -m "$metadataFiles" `
    -roblox `
    -namespace $namespace

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Transpiler failed" -ForegroundColor Red
    exit 1
}

Write-Host "Build complete! Output in Output/" -ForegroundColor Green
Write-Host ""
Write-Host "To run in Roblox Studio:" -ForegroundColor Cyan
Write-Host "  1. Start Rojo: rojo serve default.project.json" -ForegroundColor White
Write-Host "  2. Connect Rojo plugin in Studio" -ForegroundColor White
Write-Host "  3. Run the game (F5)" -ForegroundColor White
