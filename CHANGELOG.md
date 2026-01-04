# Changelog

All notable changes to EVEMon will be documented in this file.

## [Unreleased] - .NET 8 Modernization

### Maintainer
- Project fork now maintained by **Alia Collins**
- Original credits preserved for EVEMon Development Team (2006-2021)

### Changed
- **Framework**: Migrated from .NET Framework 4.6.1 to .NET 8
- **Project Format**: Converted all 6 projects to SDK-style .csproj format
- **Solution**: Created new modern solution file with proper project references

### Added
- `SplitContainerMinFixed` control for consistent UI behavior
- `CredentialsLevel` enum for API credential management
- Resources facade class for unified resource access
- `Clone()` methods to column settings classes for proper `ICloneable` support

### Fixed
- System.Windows.Threading (Dispatcher) references by enabling WPF support
- Missing `using` statements for extension methods
- Accessibility modifiers for class inheritance compatibility
- Resource namespace resolution for image assets

### Deprecated (Excluded from compilation)
- **OneDrive Cloud Storage**: Microsoft OneDrive SDK is deprecated; needs rewrite using Microsoft Graph API
- **Dropbox Cloud Storage**: Dropbox API v7 has breaking changes requiring API update
- **IGB Service**: EVE Online removed the In-Game Browser feature
- **SuggestionWindow**: References non-existent `PlanSuggestions` class

### Technical Notes

#### Build Requirements
- .NET 8 SDK
- Windows (WinForms/WPF dependency)

#### Known Warnings
- NuGet package version mismatches (minor, auto-resolved)
- NetOfficeFw packages use .NET Framework compatibility shim
- BinaryFormatter deprecation warnings in .resx files (non-blocking)

#### Project Structure
```
EVEMon.sln
├── src/EVEMon.Common/        - Core library
├── src/EVEMon/               - Main WinForms application
├── src/EVEMon.PieChart/      - Pie chart controls
├── src/EVEMon.Watchdog/      - Application watchdog
├── src/EVEMon.LogitechG15/   - Logitech G15 keyboard support
└── src/EVEMon.WindowsApi/    - Windows API interop
```

---

## Previous Releases

For historical changelog entries, see the original EVEMon project documentation.
