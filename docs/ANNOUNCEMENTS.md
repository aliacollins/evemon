# EVEMon Takeover Announcements

## EVE Online Forums Post

**Title:** EVEMon - New Maintainer, .NET 8 Update Available

---

Hello capsuleers,

I'm Alia Collins (in-game), and I've taken over maintenance of EVEMon from Peter Han.

EVEMon is a classic EVE tool that many of us have relied on for years. Rather than let it fade away, I've updated it to modern .NET 8 and fixed several issues that had been accumulating.

### What's Changed

**Technical Updates:**
- Migrated from .NET Framework 4.6.1 to .NET 8
- Fixed socket exhaustion issue that was causing connection failures
- Implemented ESI best practices (proper rate limiting, caching, error handling)
- Fixed settings loading errors for long-time users
- Modern async/await patterns throughout

**UI Improvements:**
- New About window with dark/light theme toggle
- Fixed clone location display
- Better countdown timer showing next API update
- Booster simulation in attribute optimizer

**Bug Fixes:**
- Fixed infinite retry loop on API failures
- Fixed InvalidCastException on certain menu clicks
- Fixed jumpy countdown timer

### Download

Get the latest release from GitHub:
https://github.com/aliacollins/evemon/releases

**Requirements:** Windows 10/11 with .NET 8.0 Runtime

### Contributing

The project is open source. Bug reports, suggestions, and contributions are welcome:
https://github.com/aliacollins/evemon

ISK donations to "Alia Collins" are appreciated but not required.

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
- New themed About window (dark/light modes)
- Various bug fixes

**Download:** https://github.com/aliacollins/evemon/releases

Requires Windows 10/11 with .NET 8.0 Runtime.

Bug reports and suggestions welcome on GitHub. ISK donations to "Alia Collins" are appreciated.

o7
