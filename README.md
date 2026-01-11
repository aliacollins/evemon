# EVEMon ALPHA

[![GPL licensed](https://img.shields.io/badge/license-GPL%20v2-blue.svg)]()
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)]()
[![ALPHA](https://img.shields.io/badge/branch-ALPHA-red.svg)]()

> **WARNING:** This is an **ALPHA** build. Expect bugs, crashes, and breaking changes.
>
> **Backup your settings before using:** `%APPDATA%\EVEMon\`

## Current Version: 5.2.0-alpha.2

### What's Being Tested

**JSON Settings Migration**
- Automatic XML to JSON settings conversion
- Per-character files (`characters/{id}.json`)
- Atomic writes to prevent corruption

**Performance Improvements**
- Splash screen with loading progress
- Tiered timers (1s, 5s, 30s) to reduce CPU usage
- Event batching to reduce UI thrashing
- Virtual ListView for 5000+ assets

**Installer & Updates**
- Inno Setup installer with .NET 8 runtime auto-download
- Separate update channels (alpha/beta/stable)
- Window title shows ALPHA designation

**Booster Injection**
- Simulate cerebral accelerators in skill plans

---

## Want Stable Instead?

| Branch | Use Case |
|--------|----------|
| **main** | Stable releases - recommended for daily use |
| beta | Pre-release testing |
| alpha | Experimental (you are here) |

**Download stable:** [GitHub Releases](https://github.com/aliacollins/evemon/releases)

---

## Installation

**Recommended:** Download the installer which automatically installs .NET 8 if needed:
- [EVEMon Installer (Alpha)](https://github.com/aliacollins/evemon/releases/tag/alpha)

**Manual:** Download the portable ZIP and ensure you have:
- Windows 10/11
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Report Issues

Found a bug? **Please report it!** That's why alpha exists.

- [GitHub Issues](https://github.com/aliacollins/evemon/issues)

---

## Maintainer

**Alia Collins** (EVE Online) | [CapsuleerKit](https://www.capsuleerkit.com/)

For full project history and changelog, see `main` branch or [CHANGELOG.md](CHANGELOG.md).

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
