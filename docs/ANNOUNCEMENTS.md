# EVEMon Takeover Announcements

## EVE Online Forums Post

**Title:** EVEMon - New Maintainer, .NET 8 Update Available

---

Hello capsuleers,

I'm Alia Collins (in-game), and I've taken over maintenance of EVEMon from Peter Han.

EVEMon has been a staple tool for EVE players for years. Rather than let it fade away, I've updated it to modern .NET 8 and fixed several issues that had accumulated.

### What's Changed

**Technical Updates:**
- Migrated from .NET Framework 4.6.1 to .NET 8
- Fixed socket exhaustion issue that was causing connection failures
- Implemented ESI best practices (proper rate limiting, caching, error handling)
- Fixed settings loading errors for long-time users
- Rebuilt SDE tools and regenerated all game data from CCP's Static Data Export

**Bug Fixes:**
- Fixed infinite retry loop on API failures
- Fixed InvalidCastException on certain menu clicks
- Fixed clone location showing blank in implant sets
- Fixed jumpy countdown timer

**UI Improvements:**
- Countdown timer now shows which API endpoint is next
- Booster simulation in attribute optimizer
- Solar system names display correctly

### Download

**Stable:** https://github.com/aliacollins/evemon/releases/tag/v5.0.3
**Beta:** https://github.com/aliacollins/evemon/releases/tag/beta

This is a portable application - extract and run, no installer needed.

### Requirements

Windows 10/11 with both runtimes installed:
1. [.NET Desktop Runtime 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [ASP.NET Core Runtime 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

Note: Both runtimes are required. If EVEMon crashes on startup, make sure you have ASP.NET Core Runtime installed (it's separate from .NET Desktop Runtime).

### Feedback & Issues

Bug reports, feature requests, and suggestions:
https://github.com/aliacollins/evemon/issues

The project remains open source. Contributions welcome.

Fly safe o7

---

## Reddit Post (r/Eve)

**Title:** EVEMon Updated - New Maintainer, .NET 8 Migration

---

I've taken over maintenance of EVEMon from Peter Han.

For those unfamiliar, EVEMon is a character monitoring and skill planning tool that's been around since the early days of EVE. Rather than let it die, I've updated it to work with modern systems.

**What's new:**
- Migrated to .NET 8 (was on .NET Framework 4.6.1)
- Fixed connection issues caused by socket exhaustion
- Proper ESI rate limiting and caching
- Fixed settings errors that affected long-time users
- Rebuilt SDE tools, regenerated all game data
- Various bug fixes

**Download:** https://github.com/aliacollins/evemon/releases

**Requirements:** Windows 10/11 with .NET Desktop Runtime 8.0 AND ASP.NET Core Runtime 8.0 (both required - if it crashes on startup, install ASP.NET Core separately)

Bug reports and feature requests: https://github.com/aliacollins/evemon/issues

o7
