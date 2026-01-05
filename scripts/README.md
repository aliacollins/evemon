# EVEMon Release Scripts

## Workflow

```
[Make changes] --> [Push beta] --> [Test] --> [Push stable when ready]
     |                  |              |               |
     v                  v              v               v
  git commit    release-beta.ps1   Testers try   release-stable.ps1
                                   beta release
```

## Commands

### Push to Beta (do this often)
```powershell
.\scripts\release-beta.ps1
```
- Builds and uploads to rolling "beta" release
- Testers always get latest from: https://github.com/aliacollins/evemon/releases/tag/beta
- Overwrites previous beta each time

### Create Stable Release (when ready)
```powershell
.\scripts\release-stable.ps1 5.0.3
```
- Creates versioned release (v5.0.3)
- Creates git tag
- Users download specific stable version

## Quick Flow Example

```powershell
# Made some fixes, push to beta for testing
git add .
git commit -m "Fix clone location display"
git push
.\scripts\release-beta.ps1

# After testing confirms it works, release stable
.\scripts\release-stable.ps1 5.0.3
```
