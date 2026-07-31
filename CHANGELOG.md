# Changelog

## 1.0.2

- Refactored config into a dedicated `ConfigManager` with debounced hot-reload
- Use SparrohUILib `HudAnchors` / `EnableReposition` instead of a local HudRepositionClient
- Removed thin `BossTimer` pass-through layer
- Require SparrohUILib 1.2.0+

## 1.0.1

- Reload the timer HUD after returning to the menu so it recreates cleanly on the next run
- Improved HUD lifecycle handling when the reticle UI is destroyed
- Renamed config option `Enable Amalgam Timer` to `Enable Amalgamation Timer` (existing configs need re-enabling if
  disabled)

## 1.0.0

- Initial release
- Tracks Amalgamation boss fight time from spawn to brain kill
- On-screen HUD with waiting, live, and final time display (`MM:SS.mmm`)
- Resets the timer when a new mission starts
- Configurable HUD toggle, anchor position, and timer color with hot reload
- Built on SparrohUILib, with optional HudRepositionAPI support for in-game repositioning
