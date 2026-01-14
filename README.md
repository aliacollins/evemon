[![GPL licensed](https://img.shields.io/badge/license-GPL%20v2-blue.svg)]()
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)]()
[![SDE](https://img.shields.io/badge/SDE-Catalyst%20Expansion-green.svg)]()

## Project Status / Fork Note

This repository is **Alia Collins' independent fork** of EVEMon. My goal is to build features that matter and ship them fast - including **DarkMon** (dark mode for EVEMon).

There is also an **established community-maintained fork** here:
https://github.com/mgoeppner/evemon

- If you want the long-running community fork: **use mgoeppner/evemon**
- If you want my fork (building what matters, shipping fast): **use this repo**

**Lineage / credit:** EVEMon is originally by the EVEMonDevTeam and Peter Han, and many community contributors. Full history and attribution are preserved in this repository.

---

# **EVEMon**

A lightweight, easy-to-use standalone Windows application designed to assist you in keeping track of your EVE Online character progression.

For developers: See [DEVELOPER.md](DEVELOPER.md) for build instructions and development setup.

---

## Download

**[Download EVEMon v5.1.1](https://github.com/aliacollins/evemon/releases/tag/v5.1.1)** (Stable)

**[Download Beta](https://github.com/aliacollins/evemon/releases/tag/beta)** (Rolling pre-release for testing)

This is a **portable application** - no installer required, just extract and run.

### Requirements
- Windows 10/11
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- [ASP.NET Core 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (required separately)

> **Note:** If EVEMon crashes on startup, make sure you have both runtimes installed. ASP.NET Core Runtime is separate from .NET Desktop Runtime.

### Installation & Setup

1. Download `EVEMon-5.1.1-win-x64.zip` from the releases page
2. Extract to a folder (e.g., `C:\EVEMon`)
3. Run `EVEMon.exe`
4. Add your character via **File → Add Character**
5. Log in with your EVE account when prompted

---

## Stability Note

This is a major rebuild from .NET Framework 4.6.1 to .NET 8. Everything worked fine in my testing, but please expect some stability issues as we shake things out. If you encounter any problems, please report them.

---

## Maintainer

**Alia Collins** (EVE Online)

Also maintainer of [CapsuleerKit](https://www.capsuleerkit.com/)

---

## Feedback & Issues

What features do you want to see? Found a bug? Let me know:

- **GitHub Issues:** https://github.com/aliacollins/evemon/issues

---

## Beta: v5.1.2-beta.1

### What's New

**Fork Migration Support** — Coming from peterhaneve's EVEMon fork? This version detects that automatically. Your skill plans and settings are preserved; you just need to re-authenticate your characters.

### Bug Fixes

- **30+ Characters Crash** — Fixed crashes when loading many characters. Removed dead Hammertime API; replaced with ESI-native structure lookups.
- **Assets Not Refreshing** — Assets, market orders, and contracts now refresh immediately on startup when "Query on Startup" is enabled.
- **Missing Station Names** — NPC station names now display correctly.
- **Deleted Character Errors** — Looking up deleted characters/corps no longer causes errors.

### Deprecated

- **Hammertime API** — Removed dead third-party citadel lookup. Structure lookups now use ESI directly with your character tokens.

### Technical Details

<details>
<summary>Click to expand for developers</summary>

**30+ Characters Crash** — Root cause: Dead Hammertime API (`stop.hammerti.me.uk`) returning HTTP 500, async fire-and-forget pattern swallowing exceptions, no cross-character request deduplication. Fix: Replaced `CitadelStationProvider` with new `StructureLookupService` featuring request deduplication via `ConcurrentDictionary` + `TaskCompletionSource`, character rotation for 403 errors, and rate limiting with `SemaphoreSlim(3)`.

**Assets Not Refreshing** — Root cause: `QueryOnStartup` property set but never checked; `Reset()` called `Cancel()` which cleared `m_forceUpdate`. Fix: Modified `QueryMonitor.Reset()` to preserve `m_forceUpdate` when `QueryOnStartup = true`.

**Missing Station Names** — Root cause: YAML SDE doesn't include station names. Fix: `YamlToSqlite` now fetches station names from ESI during SDE generation.

**Deleted Character Errors** — Root cause: ESI returns 404 for deleted entities, not handled. Fix: Added 404 handling in `EveIDToName.cs`.

</details>

---

⚠️ **Beta release for testing. Please report issues.**

---

## What's New (Since Taking Over)

Since taking over maintenance of this fork, the following improvements have been made:

### .NET 8 Migration
- Migrated from .NET Framework 4.6.1 to .NET 8
- Converted all project files to SDK-style format
- Updated all NuGet dependencies to modern versions

### ESI Best Practices
- Proper User-Agent header with maintainer contact
- X-ESI-Error-Limit-Reset header tracking for rate limiting
- ETag and caching implementation following ESI guidelines
- Proper error count reset after timeout periods

### Async Modernization
- Modernized API calling patterns from callback-based to async/await
- Removed legacy .NET Framework networking code (ServicePointManager, GlobalProxySelection)
- Implemented proper HttpClient with SocketsHttpHandler for connection pooling

### Bug Fixes
- Fixed socket exhaustion issue that prevented ESI API calls
- Fixed InvalidCastException when clicking menu separators
- Fixed infinite retry loop when API queries fail
- Fixed jumpy countdown timer caused by intermittent HasAccess checks
- Added null safety for API key lookups
- Fixed settings "pre-1.3.0" error that appeared on every launch
- Fixed clone location showing blank in implant set names

#### Issue #4: Settings Not Saving Between Restarts
**Root Cause:** The versioning scheme used `5.1.0.0` for stable releases. Legacy code checked if revision=0 to detect ancient pre-1.3.0 settings files and would reset them. But revision=0 also matched our stable builds, causing settings to reset on every restart.

**Fix Applied:** Changed `GetRevisionNumber()` to return -1 when no revision attribute found, updated all checks from `== 0` to `< 0`. Now revision=0 is valid for modern builds.

#### Issue #5: Certificates Not Accurate
**Root Cause:** Certificates were removed from EVE Online entirely. CCP replaced them with the Ship Tree / Mastery system. The certificate data in EVEMon was outdated and no longer reflected anything in the game.

**Fix Applied:** Certificate Browser marked as deprecated, no longer shows to user. Masteries are already available in Ship Browser tab.

### UI Improvements
- Countdown timer now shows which API endpoint is next (e.g., "Skills: 00:02:45")
- Regenerated SDE data files with correct solar system names
- Added booster simulation to attribute optimizer (check "Simulating Booster" to include active booster effects)
- Status bar shows current booster simulation state

### Email Notifications
- Migrated from deprecated `System.Net.Mail.SmtpClient` to MailKit
- Proper async email sending with thread-safe UI callbacks

### SDE Tools Rebuilt
- Rebuilt `YamlToSqlite` tool to convert EVE SDE YAML files to SQLite database
- Rebuilt `XmlGenerator` tool to generate EVEMon data files from SDE
- All game data regenerated from CCP's Static Data Export (SDE)
- **Current SDE: Catalyst Expansion (December 2025)**

### New UI Direction
- Started experimenting with modern UI styling
- Check out **Help → About** for a preview of where the UI is heading
- Dark/Light theme toggle with smooth transitions

---

## Deprecated Features

The following features have been excluded from this fork:
- **OneDrive cloud storage** - Requires Microsoft Graph API rewrite
- **Dropbox cloud storage** - Dropbox API v7 breaking changes
- **IGB Service** - EVE Online removed the In-Game Browser
- **Certificate Browser** - CCP removed certificates from EVE; replaced by Ship Mastery system

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
