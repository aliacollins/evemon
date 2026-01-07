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

### Fork Migration Detection (v5.1.2)

**Purpose:** Detect users migrating from peterhaneve's EVEMon fork and handle their settings appropriately.

**Problem:** ESI refresh tokens are tied to the specific EVEMon application's SSO credentials. Users migrating from peterhaneve's fork have tokens that won't work with our SSO credentials, causing authentication failures.

**Solution:** Detect migration at startup, clear invalid ESI keys, preserve other settings (plans, UI preferences, etc.), and show a friendly message explaining the situation.

**Detection Logic:**
1. Check for `forkId` attribute in settings.xml root element
2. If `forkId="aliacollins"` → Our user, no migration needed
3. If `forkId` present but different → Migration from that fork
4. If `forkId` missing, use revision number to distinguish:
   - `revision > 1000` → peterhaneve user (they use auto-incrementing build numbers like 4986)
   - `revision <= 1000` → Our existing user (we use 0 for stable, 1-N for betas)

**Key Constants:**
```csharp
private const string OurForkId = "aliacollins";
private const int PeterhaneveRevisionThreshold = 1000;
```

**Migration Behavior:**
- Shows welcome message explaining the situation
- Clears ESI keys (from file AND memory)
- Preserves: skill plans, characters (without auth), UI settings, all other preferences
- Adds `forkId` and `forkVersion` attributes to settings.xml
- Sets `MigrationFromOtherForkDetected = true` for other code to check

**Silent forkId Addition:**
For our existing users who don't have `forkId` yet (pre-5.1.2), we silently add it without any migration message or disruption.

**Files Changed:**
- `Settings.cs` - Main migration detection and handling logic:
  - `MigrationDetectionResult` class
  - `DetectForkMigration()` - Detection logic
  - `UpdateSettingsFileForMigration()` - Clear ESI keys, add forkId/forkVersion
  - `AddForkIdToSettingsFile()` - Silent update for existing users
  - `ShowMigrationMessage()` - User-friendly migration message
  - `Export()` - Sets forkId and forkVersion on every save
- `SerializableSettings.cs` - Added ForkId and ForkVersion properties

**Known Limitations:**
- Cloud storage loading doesn't have migration detection (edge case)
- Backup file restore doesn't have migration detection (user-initiated)
- Both are acceptable since ESI keys wouldn't work anyway

**Testing Scenarios:**
1. peterhaneve 4.0.2 (revision=4986) → Our fork: Shows migration popup, clears ESI keys
2. Our 5.1.1 (revision=0, no forkId) → Our 5.1.2: Silent forkId addition, no popup
3. Our 5.1.2 (forkId=aliacollins) → Our 5.1.2: No changes needed

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
