# Alpha Branch Warning

```
    ___   __    ____  _   _    ___
   / _ \ / /   / __ \| | | |  / _ \
  / /_\ \/ /   / /_/ /| |_| | / /_\ \
 /  __  / /___/ ____/ |  _  |/  __  \
/_/  |_\____/_/      |_| |_|/_/  |_\

USE AT YOUR OWN RISK
```

## What is this branch?

The `alpha` branch contains **experimental features** that are actively being developed. This code is:

- **Unstable** - May crash, freeze, or behave unexpectedly
- **Incomplete** - Features may be partially implemented
- **Breaking** - Could corrupt your settings or data
- **Changing** - Code changes frequently without notice

## Who should use this?

- Developers contributing to EVEMon
- Testers who want to help find bugs
- Advanced users who understand the risks

## Who should NOT use this?

- Anyone who needs EVEMon to work reliably
- Users who don't want to risk their settings
- Anyone not prepared to report bugs

## Recommended Branches

| Branch | Stability | Use Case |
|--------|-----------|----------|
| `main` | Stable | Daily use, production |
| `beta` | Testing | Help test before release |
| `alpha` | Experimental | Development only |

## Before using alpha

1. **Backup your settings**: `%APPDATA%\EVEMon\`
2. **Expect things to break**
3. **Report issues**: [GitHub Issues](https://github.com/aliacollins/evemon/issues)

## Current experimental features

- Splash screen with loading progress
- Tiered timer system (SecondTick, FiveSecondTick, ThirtySecondTick)
- Event batching/coalescing for performance
- API request queue with rate limiting awareness
- JSON settings (alongside XML)
- Per-character settings files
- Reduced startup stagger (75ms vs 200ms)

## Getting stable releases

Download stable releases from: [GitHub Releases](https://github.com/aliacollins/evemon/releases)

---

**You have been warned!**
