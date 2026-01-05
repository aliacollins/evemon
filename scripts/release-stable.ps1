# release-stable.ps1 - Promote current build to stable release
# Usage: .\scripts\release-stable.ps1 5.0.3

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "Invalid version format. Use: X.Y.Z (e.g., 5.0.3)" -ForegroundColor Red
    exit 1
}

Write-Host "Creating stable release v$Version..." -ForegroundColor Cyan

# Build fresh
Write-Host "Building EVEMon Release..." -ForegroundColor Cyan
dotnet publish "src\EVEMon\EVEMon.csproj" -c Release -r win-x64 --self-contained false -o "publish\stable"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create zip with version in name
$zipPath = "publish\EVEMon-$Version-win-x64.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath }
Compress-Archive -Path "publish\stable\*" -DestinationPath $zipPath

# Create git tag
git tag -a "v$Version" -m "Release v$Version"
git push origin "v$Version"

Write-Host "Creating GitHub release..." -ForegroundColor Cyan

gh release create "v$Version" $zipPath --title "EVEMon v$Version" --notes @"
## EVEMon v$Version

### Installation
1. Download ``EVEMon-$Version-win-x64.zip``
2. Extract to a folder
3. Run ``EVEMon.exe``
4. Add your character via **File -> Add Character**

### Requirements
- Windows 10/11
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Changelog
$(git log --oneline $(git describe --tags --abbrev=0 HEAD^ 2>$null)..HEAD 2>$null || echo "Initial release")

---
See [README](https://github.com/aliacollins/evemon#readme) for full documentation.
"@

Write-Host ""
Write-Host "Stable release v$Version created!" -ForegroundColor Green
Write-Host "URL: https://github.com/aliacollins/evemon/releases/tag/v$Version" -ForegroundColor Yellow
