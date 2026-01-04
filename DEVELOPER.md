# Contributing to EVEMon

Thank you for your interest in contributing to EVEMon!

## Development Setup

### Prerequisites
- Windows 10/11
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 or VS Code with C# extension

### Building from Source

```bash
# Clone the repository
git clone https://github.com/Arpanjha/evemon.git
cd evemon

# Build the solution
dotnet build EVEMon.sln -c Debug

# Run
dotnet run --project src/EVEMon/EVEMon.csproj
```

### ESI Credentials for Development

EVEMon requires ESI API credentials. For development:

1. Go to [EVE Developers](https://developers.eveonline.com/) and log in
2. Create a new application:
   - **Name:** EVEMon-Dev (or any name)
   - **Callback URL:** `http://localhost:4916/callback/`
   - **Scopes:** Select all scopes needed for testing
3. Create `esi-credentials.json` in the repo root (it's gitignored):
```json
{
  "ClientID": "your-client-id",
  "ClientSecret": "your-client-secret"
}
```

Alternatively, enter credentials in EVEMon via **Tools → Options → Network → ESI Settings**.

---

## Project Structure

```
evemon/
├── src/
│   ├── EVEMon/                    # Main WinForms application
│   ├── EVEMon.Common/             # Core library (models, services, API)
│   ├── EVEMon.LogitechG15/        # Logitech keyboard support
│   ├── EVEMon.PieChart/           # Chart controls
│   ├── EVEMon.Sales/              # Market/sales functionality
│   ├── EVEMon.Watchdog/           # Process watchdog
│   └── EVEMon.WindowsApi/         # Windows API interop
├── tools/
│   ├── XmlGenerator/              # Generates EVEMon data files from SDE
│   └── YamlToSqlite/              # Converts EVE SDE YAML to SQLite
└── SharedAssemblyInfo.cs          # Version info for all projects
```

---

## SDE Data Tools

EVEMon uses static data from CCP's SDE (Static Data Export). To update:

### 1. Download SDE
The SDE is published by CCP. [Fuzzwork](https://www.fuzzwork.co.uk/dump/) provides a convenient SQLite conversion:
```bash
curl -O https://www.fuzzwork.co.uk/dump/sqlite-latest.sqlite.bz2
bunzip2 sqlite-latest.sqlite.bz2
```
Note: The data comes from CCP's official SDE - Fuzzwork just converts it to SQLite format for easier use.

### 2. Run XmlGenerator
```bash
dotnet run --project tools/XmlGenerator/XmlGenerator.csproj
```

This generates the XML/gzip files in `src/EVEMon.Common/Resources/`.

---

## Creating a Release

### 1. Update Version
Edit `SharedAssemblyInfo.cs`:
```csharp
[assembly: AssemblyVersion("5.0.x.0")]
[assembly: AssemblyFileVersion("5.0.x.0")]
[assembly: AssemblyInformationalVersion("5.0.x")]
```

### 2. Build Release
```bash
dotnet publish src/EVEMon/EVEMon.csproj -c Release -r win-x64 --self-contained false -o publish
```

### 3. Create Zip
```powershell
Compress-Archive -Path publish/* -DestinationPath EVEMon-5.0.x-win-x64.zip
```

### 4. Create GitHub Release
```bash
gh release create v5.0.x EVEMon-5.0.x-win-x64.zip --title "EVEMon v5.0.x" --notes "Release notes here"
```

---

## Debug vs Release Builds

- **Debug builds** show the Test menu (for exception testing, notifications, etc.)
- **Release builds** hide the Test menu automatically via `[Conditional("DEBUG")]`

---

## Code Style

- Use C# 12 features where appropriate
- Follow existing code patterns
- Add XML documentation for public APIs
- Use async/await for all I/O operations

---

## Deprecated Features

These features were removed due to API changes:
- **OneDrive** - Requires Microsoft Graph API rewrite
- **Dropbox** - Dropbox API v7 breaking changes
- **IGB Service** - EVE removed In-Game Browser

---

## Questions?

Open an issue or check the [wiki](https://github.com/Arpanjha/evemon/wiki).
