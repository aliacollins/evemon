# EVEMon ALPHA

[![GPL licensed](https://img.shields.io/badge/license-GPL%20v2-blue.svg)]()
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)]()
[![ALPHA](https://img.shields.io/badge/branch-ALPHA-red.svg)]()

> **WARNING:** This is an **ALPHA** build. Expect bugs, crashes, and breaking changes.
>
> **Backup your settings before using:** `%APPDATA%\EVEMon\`

## Current Version: 5.1.2-alpha.10

---

## Installation

**Recommended:** Download the installer which automatically installs .NET 8 if needed:
- [EVEMon Installer (Alpha)](https://github.com/aliacollins/evemon/releases/tag/alpha)

**Manual:** Download the portable ZIP and ensure you have:
- Windows 10/11
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Alpha Changelog (Cumulative)

### alpha.10 - Version Correction
- Fixed version numbering from 5.2.0 to 5.1.2
- Cumulative changelog in README and release notes

### alpha.9 - Auto-Launch After Update
- EVEMon now auto-launches after silent update completes
- Installer force-closes running EVEMon before updating
- Fixed git scripts for branch/tag ambiguity

### alpha.8 - Auto-Update Fix
- Fixed auto-updater to use branch-specific URLs
- Alpha/Beta builds now check correct update channels
- Rolling release tags (single `alpha` tag instead of versioned tags)

### alpha.7 - Loading Indicators
- Toast notification on first API connection success/failure
- Shows character count or warning for connection issues

### alpha.6 - API Loading UX
- Loading screen shows "Fetching API data..." instead of "Loading..."
- Improved user feedback during startup

### alpha.5 - ESI Key Warnings
- Warning indicators for ESI keys with errors
- Visual feedback for authentication issues
- Installer creation and alpha/beta update channels

### alpha.4 - Installer & Updates
- Inno Setup installer with .NET 8 runtime auto-download
- Fork notice page during installation
- Settings backup before upgrade
- Separate update channels (alpha/beta/stable)

### alpha.3 - Performance & UI
- Splash screen with loading progress
- Tiered timers (1s, 5s, 30s) to reduce CPU usage
- Event batching to reduce UI thrashing
- Virtual ListView for 5000+ assets
- Window title shows ALPHA designation

### alpha.2 - JSON Settings
- Automatic XML to JSON settings conversion
- Per-character files (`characters/{id}.json`)
- Atomic writes to prevent corruption
- Settings backup and migration from older forks

### alpha.1 - .NET 8 Foundation
- Migrated from .NET Framework 4.6.1 to .NET 8
- SDK-style project format
- Booster injection simulation in skill plans
- Fork migration detection (peterhaneve, mpogenner)

---

## Features Being Tested

### Core Improvements
- **JSON Settings**: Modern settings format with atomic writes
- **Performance**: Reduced CPU usage, faster UI updates
- **Auto-Update**: Seamless updates with auto-restart

### User Experience
- **Splash Screen**: Loading progress visibility
- **Toast Notifications**: API connection feedback
- **Loading Indicators**: Clear status during startup

### Installation
- **One-Click Installer**: Handles .NET 8 runtime
- **Update Channels**: Alpha/Beta/Stable separation
- **Settings Migration**: Preserves data from older versions

---

## Want Stable Instead?

| Branch | Use Case |
|--------|----------|
| **main** | Stable releases - recommended for daily use |
| beta | Pre-release testing |
| alpha | Experimental (you are here) |

**Download stable:** [GitHub Releases](https://github.com/aliacollins/evemon/releases)

---

## Report Issues

Found a bug? **Please report it!** That's why alpha exists.

- [GitHub Issues](https://github.com/aliacollins/evemon/issues)

---

## Maintainer

**Alia Collins** (EVE Online) | [CapsuleerKit](https://www.capsuleerkit.com/)

---

## License

GPL v2 - See [LICENSE](src/EVEMon.Common/Resources/License/gpl.txt) for details.

---

## Credits

### Previous Maintainer
**Peter Han** (EVE Online)
- [GitHub (upstream fork)](https://github.com/peterhaneve/evemon)

### Original Creator
**EVEMonDevTeam**
- [GitHub](https://github.com/evemondevteam/)
- [Bitbucket](https://bitbucket.org/EVEMonDevTeam)
- [Website](https://evemondevteam.github.io/evemon/)
- [Documentation](https://evemon.readthedocs.org/)
