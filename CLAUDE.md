# Claude Code Instructions for EVEMon

## Commit Guidelines

- **NEVER commit without explicit approval** - Even after edits, wait for user's "nod" before committing
- **Draft content stays in conversation** - Announcements, forum posts, release notes drafts should NOT be committed to the repo
- **Batch related changes** - When committing, group related changes into single meaningful commits instead of many small ones
- **No Claude attribution** - Do not add "Co-Authored-By: Claude" or similar signatures to commits

## Bug Fix Documentation

For every bug fix in beta releases, document in this file:
- **Root Cause** - What was actually causing the bug (not just symptoms)
- **Fix Applied** - What changes were made and why
- **Files Changed** - List of modified files

This ensures we don't repeat mistakes and have a clear history of what was fixed.

## Versioning Scheme

### Stable Releases
- Version format: `x.x.x.0` (e.g., `5.1.1.0`)
- Display name: `x.x.x` (e.g., `5.1.1`)
- AssemblyInformationalVersion: `5.1.1`

### Beta Releases
- Version format: `x.x.x.N` (e.g., `5.1.1.1` for beta.1)
- Display name: `x.x.x-beta.N` (e.g., `5.1.1-beta.1`)
- AssemblyInformationalVersion: `5.1.1-beta.1`

### Version Numbering
- Major.Minor.Patch.Revision
- Stable uses Revision=0, Beta uses Revision=1,2,3...
- Use AssemblyInformationalVersion for display name

### Issue #4 Root Cause & Fix (Settings Not Saving)

**Problem:** When the new versioning scheme was set up, `5.1.0.0` was used for stable. Legacy code checked the 4th digit (revision) to detect ancient pre-1.3.0 settings files:

```csharp
// OLD broken logic - treated revision=0 same as "no revision attribute"
if (revision == 0)
    // Treat as old incompatible settings, reset everything
```

The `GetRevisionNumber()` function returned 0 for BOTH:
- Files with `revision="0"` (our stable builds)
- Files with NO revision attribute (actual ancient pre-1.3.0 files)

Every time stable (`5.1.0.0`) started, it saw revision=0 and wiped settings. Plans exported with stable also couldn't be re-imported.

**Root fix applied:** Changed `GetRevisionNumber()` to return -1 when no revision attribute found. Updated all checks from `== 0` to `< 0`. Now revision=0 is valid.

**Files changed:**
- `Util.cs` - GetRevisionNumber returns -1 for missing attribute
- `Settings.cs` - Three checks updated to `< 0`
- `PlanIOHelper.cs` - Two checks updated to `< 0` / `>= 0`

### Issue #5 Root Cause & Fix (Certificates Not Accurate)

**Problem:** User reported certificates showing as incomplete even though character has them at level V in-game.

**Root Cause:** Certificates were removed from EVE Online entirely. CCP replaced them with the Ship Tree / Mastery system. The certificate data in EVEMon is outdated and no longer reflects anything in the game.

**Fix Applied:** Certificate Browser marked as deprecated, no longer shows to user. Masteries are already shown in the Ship Browser tab.

**Files changed:**
- `PlanWindow.cs` - Remove `tpCertificateBrowser` tab on load

**Note:** Full removal of certificate code (~60 files) can be done in a future release if desired.

### Files to Update for Releases

**For ANY release (stable or beta):**
- `SharedAssemblyInfo.cs` - Update AssemblyVersion, AssemblyFileVersion, AssemblyInformationalVersion

**For STABLE releases only (not beta):**
- `updates/patch.xml` - Update version, URL, autopatchurl, message
  - This controls auto-update notifications
  - Beta users should NOT be notified to "update" to stable (would be downgrade)
  - Only bump patch.xml when releasing a new stable version

### Release Process
1. Update SharedAssemblyInfo.cs with new version
2. Build and test
3. If stable: Update updates/patch.xml
4. Create GitHub release (use --prerelease flag for beta)

## Project Context

- Maintainer: Alia Collins (EVE Online character name)
- GitHub: https://github.com/aliacollins/evemon
- This is a .NET 8 Windows Forms application for EVE Online character monitoring
