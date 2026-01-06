# Bug Fix: QueryOnStartup Not Working (Assets Not Fetched on Restart)

## Issue Summary

**Symptom:** Character data like Assets, Market Orders, Contracts, etc. would not be fetched on application restart for pre-existing characters. Newly added characters worked fine.

**Severity:** Medium - Data was stale until cache expired (up to 2 hours for assets)

**Status:** Fixed in v5.1.2

---

## Root Cause Analysis

### The Intended Design

The `QueryMonitor` class has a `QueryOnStartup` property meant to force API queries when the application starts, regardless of cached expiration times:

```csharp
// CharacterDataQuerying.cs - Assets configured to query on startup
m_characterQueryMonitors.Add(new PagedQueryMonitor<EsiAPIAssetList,
    EsiAssetListItem>(new CharacterQueryMonitor<EsiAPIAssetList>(ccpCharacter,
    ESIAPICharacterMethods.AssetList, OnAssetsUpdated,
    notifiers.NotifyCharacterAssetsError) { QueryOnStartup = true }));  // <-- Set to true
```

This property was set to `true` for 15+ query types including:
- AssetList
- MarketOrders
- Contracts
- IndustryJobs
- MailMessages
- And more...

### The Bug

The `QueryOnStartup` property was **never actually checked** in the query decision logic. It was a dead property.

When a character loads from settings, the saved API update times are restored via `ResetLastAPIUpdates()`:

```csharp
// CCPCharacter.cs:764
ResetLastAPIUpdates(m_lastAPIUpdates.Where(lastUpdate => Enum.IsDefined(
    typeof(ESIAPICharacterMethods), lastUpdate.Method)));
```

This calls `QueryMonitor.Reset()` for each API method:

```csharp
// QueryMonitor.cs (BEFORE FIX)
private void Reset(DateTime lastUpdate)
{
    Cancel();              // <-- BUG: This sets m_forceUpdate = false!
    LastUpdate = lastUpdate;
    LastResult = null;
}

private void Cancel()
{
    m_isCanceled = true;
    m_forceUpdate = false; // <-- This disables the force-query-on-startup behavior
}
```

The query decision logic at line 242:
```csharp
else if (EsiErrors.IsErrorCountExceeded || (!m_forceUpdate && NextUpdate > DateTime.UtcNow))
{
    Status = QueryStatus.Pending;  // Query is SKIPPED
}
```

### The Result

1. Constructor sets `m_forceUpdate = true` (correct)
2. Character loads from settings
3. `Reset()` is called with saved update time
4. `Cancel()` sets `m_forceUpdate = false` (BUG!)
5. Query logic sees `m_forceUpdate = false` and `NextUpdate > Now`
6. Query is skipped because cache appears valid
7. **But the actual data (assets, etc.) was never persisted - only the cache time was!**

---

## The Fix

Modified `Reset()` to preserve `m_forceUpdate` when `QueryOnStartup` is true:

```csharp
// QueryMonitor.cs (AFTER FIX)
private void Reset(DateTime lastUpdate)
{
    // Cancel any running request, but preserve m_forceUpdate for startup queries
    m_isCanceled = true;
    LastUpdate = lastUpdate;
    LastResult = null;
    // If QueryOnStartup is true, ensure first query runs regardless of cached time
    // This fixes the bug where assets weren't fetched on restart because
    // the cached time was restored but the actual data was not persisted
    if (QueryOnStartup)
        m_forceUpdate = true;
}
```

---

## Impact

### Before Fix
- Pre-existing characters: Assets/Orders/etc. empty until cache expires (up to 2 hours)
- Newly added characters: Worked correctly (no saved cache time to restore)

### After Fix
- All characters with `QueryOnStartup = true` queries fetch data on every app start
- Respects ESI rate limits (still uses ETag/304 Not Modified when data unchanged)
- No unnecessary API calls - just ensures the first request happens

---

## Files Changed

| File | Change |
|------|--------|
| `src/EVEMon.Common/QueryMonitor/QueryMonitor.cs` | Fixed `Reset()` method to preserve `m_forceUpdate` when `QueryOnStartup = true` |

---

## Testing

1. Add a character, view assets - works
2. Close EVEMon
3. Reopen EVEMon
4. Check trace log for `OnCharacterAssetsUpdated` for ALL characters
5. Verify assets display immediately for all characters

**Trace log evidence (fixed):**
```
0d 0h 00m 07s > QueryMonitor.Starting - AssetList
0d 0h 00m 07s > QueryMonitor.OnQueried - AssetList completed
0d 0h 00m 07s > EveMonClient.OnCharacterAssetsUpdated - Max Collins
0d 0h 00m 08s > QueryMonitor.Starting - AssetList
0d 0h 00m 08s > QueryMonitor.OnQueried - AssetList completed
0d 0h 00m 08s > EveMonClient.OnCharacterAssetsUpdated - Alia Collins  <-- Previously missing!
```

---

## Notes

This was a **pre-existing bug** in the original EVEMon codebase, not introduced by the .NET 8 migration. The `QueryOnStartup` property existed but was never properly implemented - it was set but never checked in the query decision logic.

The bug likely went unnoticed because:
- Users often keep EVEMon running for extended periods
- Manual refresh would work around the issue
- The data would eventually load after cache expiration
