# AmalgamationTracker

A BepInEx client mod for Mycopunk that tracks Amalgamation boss fight time with an on-screen HUD timer.

## Features

- **Automatic timing**: Starts when the Amalgamation spawns and stops when the brain is killed
- **On-screen HUD**: Shows waiting, live elapsed, and final time near the reticle (`MM:SS.mmm`)
- **Mission reset**: Timer resets when a new mission starts
- **Configurable**: Toggle the HUD, set position anchors, and choose the timer color
- **Hot reload**: Config changes apply while the game is running
- **Optional repositioning**: Drag the timer in-game via ModSettingsMenu (F9) through SparrohUILib

## Dependencies

- Mycopunk
- [BepInExPack_Mycopunk](https://thunderstore.io/c/mycopunk/p/BepInEx/BepInExPack_Mycopunk/) (BepInEx 5.4.2403 or
  compatible)
- [SparrohUILib](https://thunderstore.io/c/mycopunk/p/Sparroh/SparrohUILib/) (1.2.0 or compatible)
- [ModSettingsMenu](https://thunderstore.io/c/mycopunk/p/Sparroh/ModSettingsMenu/) (optional, for F9 drag-reposition)

## Installing

**Via Thunderstore (recommended)**

1. Install with a Thunderstore mod manager (for example r2modman or the Thunderstore App).
2. Ensure **SparrohUILib** is installed as a dependency.

**Manual installation**

1. Install BepInEx and SparrohUILib for Mycopunk.
2. Place `AmalgamationTracker.dll` in `<Mycopunk Directory>/BepInEx/plugins/`.

The mod loads automatically with BepInEx. Check the BepInEx log for:

`AmalgamationTracker loaded successfully.`

## Building

1. Clone this repository.
2. Restore and build in Release mode:

```bash
dotnet build --configuration Release
```

3. Output DLL:

`bin/Release/netstandard2.1/AmalgamationTracker.dll`

Game and SparrohUILib assembly paths in the project file may need to match your local install.

## Configuration

Config file:

`<Mycopunk Directory>/BepInEx/config/sparroh.amalgamationtracker.cfg`

| Section           | Option                      | Default     | Description                                   |
|-------------------|-----------------------------|-------------|-----------------------------------------------|
| `General`         | `Enable Amalgamation Timer` | `true`      | Enables the Amalgamation timer HUD            |
| `HUD Positioning` | `Timer X`                   | `0.8229749` | X anchor position (0–1)                       |
| `HUD Positioning` | `Timer Y`                   | `0.9050629` | Y anchor position (0–1)                       |
| `Colors`          | `Timer Color`               | Amber       | Rich-text value color (`RRGGBB` or `#RRGGBB`) |

Config changes are hot-reloaded while the game is running. With ModSettingsMenu installed, press **F9** to drag the
timer; anchor values are written back to this config.

## Usage

1. Start an Amalgamation mission.
2. The HUD shows `Amalgamation: Waiting...` until the boss spawns.
3. Timing begins on spawn and updates live.
4. Timing stops when the brain is killed and the final time remains on screen.
5. Starting a new mission resets the timer.

## Help

- **Mod not loading?** Confirm BepInEx and SparrohUILib are installed, then check the BepInEx log for errors.
- **Timer not appearing?** Set `Enable Amalgamation Timer` to `true` and make sure you are in-game with a reticle HUD.
- **Timer not starting?** Confirm you are on an Amalgamation mission and the boss has spawned.
- **Wrong position or color?** Adjust `Timer X`, `Timer Y`, and `Timer Color` in the config file, or drag with F9 when
  ModSettingsMenu is installed.

## Authors

- Sparroh

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
