# EVEMon BETA

[![GPL licensed](https://img.shields.io/badge/license-GPL%20v2-blue.svg)]()
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)]()
[![BETA](https://img.shields.io/badge/branch-BETA-yellow.svg)]()

> **BETA:** This is a pre-release build for community testing before stable release.
>
> Please report any issues you find!

## Current Version: 5.1.2-beta.1

---

## Installation

**Recommended:** Download the installer which automatically installs .NET 8 if needed:
- [EVEMon Installer (Beta)](https://github.com/aliacollins/evemon/releases/tag/beta)

**Manual:** Download the portable ZIP and ensure you have:
- Windows 10/11
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## What's New in 5.1.2

### Major Features

**Modern Framework**
- Migrated from .NET Framework 4.8 to .NET 8
- Improved performance and security
- SDK-style project format

**New Installer**
- One-click installer with automatic .NET 8 runtime download
- Settings backup before upgrade
- Fork notice for users migrating from older versions

**Auto-Update System**
- Seamless background updates
- Automatic app restart after update
- Separate alpha/beta/stable update channels

**Performance Improvements**
- Splash screen with loading progress
- Tiered update timers (1s/5s/30s) reduce CPU usage
- Event batching reduces UI thrashing
- Virtual ListView handles 5000+ assets smoothly

**User Experience**
- Loading indicators during API fetch
- Toast notifications for connection status
- ESI key warning indicators

**Skill Planning**
- Booster injection simulation (cerebral accelerators)

### Bug Fixes

- Settings not saving correctly (revision number detection)
- Fork migration from peterhaneve and mpogenner versions
- Certificate Browser tab hidden (CCP removed certificates)
- 30+ character crash (Hammertime API removal)
- Structure lookups with proper deduplication

### Technical Changes

- JSON settings format with atomic writes (auto-migrates from XML)
- Per-character settings files
- Window title shows version channel

---

## Want Stable Instead?

| Branch | Use Case |
|--------|----------|
| **main** | Stable releases - recommended for daily use |
| beta | Pre-release testing (you are here) |
| alpha | Experimental |

**Download stable:** [GitHub Releases](https://github.com/aliacollins/evemon/releases)

---

## Report Issues

Found a bug? **Please report it!** That's why beta exists.

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

### Support the Project
I don't accept donations. If you want to support EVEMon, please donate to Peter Han or the original EVEMonDevTeam who built this tool over many years.
