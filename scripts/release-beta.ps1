# release-beta.ps1 - Build and push to rolling beta release
# Usage: .\scripts\release-beta.ps1

$ErrorActionPreference = "Stop"

Write-Host "Building EVEMon Release..." -ForegroundColor Cyan

# Build
dotnet publish "src\EVEMon\EVEMon.csproj" -c Release -r win-x64 --self-contained false -o "publish\beta"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create zip
$zipPath = "publish\EVEMon-beta-win-x64.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath }
Compress-Archive -Path "publish\beta\*" -DestinationPath $zipPath

Write-Host "Uploading to beta release..." -ForegroundColor Cyan

# Delete existing beta release and recreate (gh doesn't have a good "update" option)
gh release delete beta --yes 2>$null

$commitHash = git rev-parse --short HEAD
$buildDate = Get-Date -Format "yyyy-MM-dd HH:mm UTC"

# Get changes since last tag
$lastTag = git describe --tags --abbrev=0 2>$null
if ($lastTag) {
    $changes = git log --oneline "$lastTag..HEAD" 2>$null
} else {
    $changes = "First beta"
}

$releaseNotes = @"
## EVEMon Beta Build

**This is a pre-release build for testing.**

Built: $buildDate
Commit: $commitHash

### Changes since last stable
$changes

---
Download, extract, and run EVEMon.exe
Requires [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
"@

gh release create beta $zipPath --prerelease --title "EVEMon Beta (Latest)" --notes $releaseNotes

Write-Host "Beta release updated!" -ForegroundColor Green
Write-Host "URL: https://github.com/aliacollins/evemon/releases/tag/beta" -ForegroundColor Yellow
