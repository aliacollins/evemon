[![GPL licensed](https://img.shields.io/badge/license-GPL%20v2-blue.svg)]()
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)]()

# **EVEMon**

A lightweight, easy-to-use standalone Windows application designed to assist you in keeping track of your EVE Online character progression.

For complete info on How To Contribute please visit the [wiki section](https://github.com/Arpanjha/evemon/wiki).

---

## Download

**[Download EVEMon v5.0.1](https://github.com/Arpanjha/evemon/releases/latest)**

### Requirements
- Windows 10/11
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation
1. Download `EVEMon-5.0.1-win-x64.zip` from the releases page
2. Extract to a folder
3. Run `EVEMon.exe`

---

## Maintainer

**Alia Collins** (EVE Online)

Also maintainer of [CapsuleerKit](https://www.capsuleerkit.com/)

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

## Deprecated Features

The following features have been excluded from this fork:
- **OneDrive cloud storage** - Requires Microsoft Graph API rewrite
- **Dropbox cloud storage** - Dropbox API v7 breaking changes
- **IGB Service** - EVE Online removed the In-Game Browser

---

## License

GPL v2 - See [LICENSE](src/EVEMon.Common/Resources/License/gpl.txt) for details.

---

## Credits

### Previous Maintainer
**Peter Han** (EVE Online)
- [GitHub](https://github.com/peterhaneve/evemon)

### Original Creator
**EVEMonDevTeam**
- [GitHub](https://github.com/evemondevteam/)
- [Bitbucket](https://bitbucket.org/EVEMonDevTeam)
- [Website](https://evemondevteam.github.io/evemon/)
- [Documentation](https://evemon.readthedocs.org/)
