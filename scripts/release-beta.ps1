# release-beta.ps1 - Build and push to rolling beta release
# Usage: .\scripts\release-beta.ps1

$ErrorActionPreference = "Stop"

Write-Host "Building EVEMon Beta Release..." -ForegroundColor Cyan

# Read version from SharedAssemblyInfo.cs
$SharedAssemblyInfo = Get-Content "SharedAssemblyInfo.cs" -Raw
if ($SharedAssemblyInfo -match 'AssemblyInformationalVersion\("([^"]+)"\)') {
    $Version = $Matches[1]
    # Extract base version for installer (e.g., "5.2.0-alpha.1" -> "5.2.0")
    $InstallerVersion = $Version -replace '-.*$', ''
    Write-Host "Version: $Version (Installer: $InstallerVersion)" -ForegroundColor Gray
} else {
    Write-Host "Could not read version from SharedAssemblyInfo.cs" -ForegroundColor Red
    exit 1
}

# Build
dotnet publish "src\EVEMon\EVEMon.csproj" -c Release -r win-x64 --self-contained false -o "publish\win-x64"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Create zip
$zipPath = "publish\EVEMon-$Version-win-x64.zip"
if (Test-Path $zipPath) { Remove-Item $zipPath }
Compress-Archive -Path "publish\win-x64\*" -DestinationPath $zipPath

# Build installer
Write-Host "Building installer..." -ForegroundColor Cyan
& "$PSScriptRoot\build-installer.ps1" -Version $InstallerVersion -SkipBuild

$installerPath = "publish\EVEMon-install-$InstallerVersion.exe"
$hasInstaller = Test-Path $installerPath

if (-not $hasInstaller) {
    Write-Host "Warning: Installer build failed or Inno Setup not installed. Continuing with ZIP only." -ForegroundColor Yellow
}

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
## EVEMon Beta Build - $Version

**This is a pre-release build for testing.**

Built: $buildDate
Commit: $commitHash

### Installation Options

**Option 1: Installer (Recommended)**
- Download ``EVEMon-install-$InstallerVersion.exe``
- The installer will download .NET 8 Desktop Runtime if needed

**Option 2: Portable ZIP**
- Download ``EVEMon-$Version-win-x64.zip``
- Requires [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Changes since last stable
$changes

---
Report issues at: https://github.com/aliacollins/evemon/issues
"@

# Upload files based on what's available
if ($hasInstaller) {
    gh release create beta $zipPath $installerPath --prerelease --title "EVEMon Beta ($Version)" --notes $releaseNotes
} else {
    gh release create beta $zipPath --prerelease --title "EVEMon Beta ($Version)" --notes $releaseNotes
}

Write-Host "Beta release updated!" -ForegroundColor Green
Write-Host "URL: https://github.com/aliacollins/evemon/releases/tag/beta" -ForegroundColor Yellow
