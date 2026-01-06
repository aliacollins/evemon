# EveIDToStation Refactoring Documentation

## Branch: `feature/refactor-eveid-to-station`

## Problem Statement

EVEMon crashed with 30+ characters due to:
1. **Dead Hammertime API** - Third-party fallback (stop.hammerti.me.uk) returns HTTP 500
2. **Async anti-pattern** - `ContinueWith` fire-and-forget swallowed exceptions
3. **No cross-character deduplication** - 30 chars = 30 duplicate API requests for same citadel
4. **Legacy code** - `CitadelStationProvider` and `IDToObjectProvider<T,X>` patterns

---

## Solution Overview

Replaced the old `CitadelStationProvider` with a modern `StructureLookupService` that:
- **Deduplicates requests** - Multiple characters requesting same citadel share one ESI call
- **Rotates characters** - If one character gets 403, tries another
- **Rate limits** - SemaphoreSlim(3) + EsiErrors check
- **Proper async/await** - No more fire-and-forget ContinueWith

---

## Files Changed

### Modified Files

| File | Changes |
|------|---------|
| `src/EVEMon.Common/Service/EveIDToStation.cs` | Major refactor - replaced CitadelStationProvider with StructureLookupService |
| `src/EVEMon.Common/Models/Asset.cs` | Fixed Location property to retry lookup when empty, added trace logging |
| `src/EVEMon.Common/QueryMonitor/QueryMonitor.cs` | Fixed QueryOnStartup bug - assets now fetch on restart |
| `src/EVEMon.Common/Service/EveIDToName.cs` | Added trace logging, handle 404 gracefully (deleted chars/corps) |
| `src/EVEMon.Common/Constants/NetworkConstants.resx` | Removed HammertimeCitadel URL |
| `src/EVEMon/DetailsWindow/ContractDetailsWindow.cs` | Updated comment (removed Hammertime reference) |
| `src/EVEMon.Common/Resources/eve-geography-en-US.xml.gzip` | Regenerated with station names |
| `src/EVEMon.Common/Resources/MD5Sums.txt` | Updated checksums |
| `tools/YamlToSqlite/Program.cs` | Added ESI station name fetching during SDE generation |

### New Files

| File | Purpose |
|------|---------|
| `src/EVEMon.Common/Service/StructureLookupService.cs` | Core lookup service with deduplication |
| `src/EVEMon.Common/Service/PendingStructureRequest.cs` | Per-request state tracking with TaskCompletionSource |
| `src/EVEMon.Common/Service/StructureRequestState.cs` | Enum: Pending, InProgress, Completed, Inaccessible, Destroyed |

### Deleted Files

| File | Reason |
|------|--------|
| `src/EVEMon.Common/Serialization/Hammertime/HammertimeStructure.cs` | Dead API removed |

---

## Architecture

### Old Architecture (Removed)
```
EveIDToStation
    └── CitadelStationProvider (IDToObjectProvider<T,X>)
            └── ContinueWith fire-and-forget pattern
            └── HammertimeAPI fallback (dead)
            └── No deduplication
```

### New Architecture
```
EveIDToStation (static facade)
    └── StructureLookupService
            ├── ConcurrentDictionary<long, SerializableOutpost> cache
            ├── ConcurrentDictionary<long, PendingStructureRequest> pending
            ├── ConcurrentQueue<long> request queue
            └── SemaphoreSlim(3) rate limiter
                    │
                    └── PendingStructureRequest
                            ├── TaskCompletionSource (multi-waiter)
                            ├── HashSet<long> tried characters
                            └── StructureRequestState
```

---

## Key Code Changes

### 1. Request Deduplication
```csharp
// Multiple callers for same structure share one request
var request = _pendingRequests.GetOrAdd(structureId, id =>
{
    isNew = true;
    return new PendingStructureRequest(id);
});
```

### 2. Character Rotation
```csharp
foreach (var character in request.GetUntriedCharacters(candidates))
{
    var result = await QueryEsiAsync(...);

    if (!result.HasError)
    {
        request.Complete(result);
        return;
    }

    if (result.ResponseCode == 403)
        continue; // Try next character
}
request.SetInaccessible();
```

### 3. Proper Async/Await
```csharp
// OLD (bad)
EveMonClient.APIProviders.CurrentProvider.QueryEsi<EsiAPIStructure>(
    method, callback, params, state);

// NEW (good)
var result = await EveMonClient.APIProviders.CurrentProvider
    .QueryEsiAsync<EsiAPIStructure>(method, params)
    .ConfigureAwait(false);
```

### 4. QueryOnStartup Bug Fix
```csharp
// QueryMonitor.Reset() now preserves m_forceUpdate when QueryOnStartup=true
private void Reset(DateTime lastUpdate)
{
    m_isCanceled = true;
    LastUpdate = lastUpdate;
    LastResult = null;
    if (QueryOnStartup)
        m_forceUpdate = true;  // FIX: Was being cleared by Cancel()
}
```

---

## Additional Fixes (Beyond Original Plan)

### 1. QueryOnStartup Bug (Pre-existing)
- **Symptom**: Assets/Market Orders not fetched on restart for pre-existing characters
- **Root Cause**: `QueryOnStartup` property was set but never used in query logic
- **Fix**: Reset() now preserves `m_forceUpdate` when `QueryOnStartup=true`
- **Documentation**: `docs/bugfixes/QueryOnStartup-fix.md`

### 2. NPC Station Names Empty
- **Symptom**: Asset locations showing blank for NPC stations
- **Root Cause**: YAML SDE doesn't include station names
- **Fix**: YamlToSqlite now fetches names from ESI during data generation
- **Result**: Regenerated `eve-geography-en-US.xml.gzip` with all station names

### 3. EveIDToName 404 Handling
- **Symptom**: Errors when looking up deleted characters/corps
- **Fix**: 404 responses now handled gracefully (shown as "Unknown")

---

## Plan Completion Status

### Phase 1: Remove Hammertime ✅
- [x] Remove `#define HAMMERTIME`
- [x] Remove `LoadCitadelInformationFromHammertimeAPI` method
- [x] Remove Hammertime fallback
- [x] Delete HammertimeStructure.cs
- [x] Remove HammertimeCitadel from NetworkConstants

### Phase 2: Create New Infrastructure ✅
- [x] StructureRequestState.cs - Enum
- [x] PendingStructureRequest.cs - Request tracking
- [x] StructureLookupService.cs - Core lookup service

### Phase 3: Refactor EveIDToStation ✅
- [x] Replace CitadelStationProvider with StructureLookupService
- [x] Remove IDToObjectProvider<T,X> inheritance
- [x] Keep existing public API
- [x] Add GetIDToStationAsync() for callers that can await

### Phase 4: Cleanup ✅
- [x] Remove unused CitadelInfo nested class
- [x] Remove unused using statements
- [x] Verify no other Hammertime references

---

## Test Code Status

**NO TEST/SIMULATION CODE PRESENT**

The branch contains no:
- Fake character simulation
- 30-account test code
- Mock ESI responses
- Debug-only code paths

All trace logging (`EveMonClient.Trace()`) is for production debugging and follows existing patterns.

---

## Verification Checklist

- [x] Build succeeds with no errors
- [x] User confirmed: 60 characters loads without crashes
- [x] Request deduplication working (trace logs show "DEDUPLICATED")
- [x] Inaccessible citadels show placeholder
- [x] Cache persists between sessions
- [x] Assets fetch on restart (QueryOnStartup fix)
- [x] Station names display correctly (NPC and citadels)

---

## Known Limitations (Out of Scope)

1. **UI Performance with 60+ chars** - Not addressed by this refactoring
   - 4m40s startup time is due to Settings.xml parsing and WinForms single-threaded UI
   - Would require separate performance optimization work

2. **Structure cache not session-persistent** - Inaccessible/Destroyed states
   - Cache file only stores successfully resolved structures
   - Inaccessible structures will be re-queried on restart
