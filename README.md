[![GPL licensed](https://img.shields.io/badge/license-GPL%20v2-blue.svg)]()
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)]()

# **EVEMon**

A lightweight, easy-to-use standalone Windows application designed to assist you in keeping track of your EVE Online character progression.

## Fork Maintainer

**Alia Collins** (EVE Online)

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

### UI Improvements
- Countdown timer now shows which API endpoint is next (e.g., "Skills: 00:02:45")
- Regenerated SDE data files with correct solar system names

### Email Notifications
- Migrated from deprecated `System.Net.Mail.SmtpClient` to MailKit
- Proper async email sending with thread-safe UI callbacks

---

## Requirements

- Windows 10/11
- .NET 8.0 Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))

---

## Building from Source

```bash
# Clone the repository
git clone https://github.com/Arpanjha/evemon.git

# Build the solution
dotnet build EVEMon.sln -c Release

# Run
dotnet run --project src/EVEMon/EVEMon.csproj
```

---

## ESI Application Setup

To use your own ESI credentials:

1. Register an application at [EVE Developers](https://developers.eveonline.com/)
2. Create `esi-credentials.json` in `%APPDATA%\EVEMon\`:
```json
{
  "ClientId": "your-client-id",
  "ClientSecret": "your-client-secret"
}
```

---

## Deprecated Features

The following features have been excluded from this fork:
- **OneDrive cloud storage** - Requires Microsoft Graph API rewrite
- **Dropbox cloud storage** - Dropbox API v7 breaking changes
- **IGB Service** - EVE Online removed the In-Game Browser

---

## Original Project Credits

This is a fork of the EVEMon project. Original credits:

- **ESI Fork:** [Peter Han](https://github.com/peterhaneve/evemon)
- **Original Team:** [EVEMonDevTeam](https://github.com/evemondevteam/)
- **Documentation:** [EVEMon Docs](https://evemon.readthedocs.org/)

---

## License

GPL v2 - See [LICENSE](src/EVEMon.Common/Resources/License/gpl.txt) for details.