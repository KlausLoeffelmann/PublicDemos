# WARP DirectX Wall Clock

A hardware-accelerated analog wall clock for WinForms (.NET 11) where **every part
of the clock — face, numerals, ticks, hands, arbour — is its own DirectComposition
visual**, drawn with [WARP-Toolkit](../../../../) Direct2D and composed by a small
engine. Clock looks and behaviors are supplied by **in-process stock themes** plus
optional **drop-in plug-in themes**, and the whole thing doubles as a demo for the
new .NET 11 `KioskModeManager` component.

> Successor in spirit to the GDI+ `WinFormsClock`: the analog clock is reimagined on
> Direct2D Visuals. The digital clock is intentionally dropped.

## Why visuals?

Each element is a retained `D2DVisual` that is drawn once (full alpha) and then
**transformed, rotated and skewed by the compositor** — so a hand is just a visual
the engine rotates. To make that possible this demo added a per-visual transform API
to WARP's `D2DVisual` (`Transform`, `SetRotation`, `SetTransform`), applied through
DirectComposition.

## The time can never be wrong

The hard invariant: **only the engine computes time and hand pointing.** A hand's
rotation is *derived* so its tip always points at the engine-owned target anchor. A
theme may relocate anchors, restyle visuals, and tweak a small set of parameters — it
can never set a hand angle. Move the hour anchors into a column on the left and the
hour hand's tip follows them; you still cannot make it lie about the time.

- **Radial layout** (built-in themes): classic dial; hands sweep/tick/crawl.
- **Free-floating layout** (e.g. *Lose-Hour*): anchors go anywhere and hands *aim* at
  them. Continuous crawling is disabled here; a **grace catch-up** (1–30 s, adjustable)
  eases a hand toward a relocated target.
- A global **face rotation** can spin the dial with or without the hands.

## Projects

| Project | Role |
|---------|------|
| `WarpClock.Abstractions` | The plug-in contract (`IClockTheme`, descriptors, layout, renderer, animator, parameters). |
| `WarpClock.Engine` | `WarpClockControl` (a `D2DPanel`): authoritative time, anchor resolution, hand-pointing solver + grace, OLED scene motion, and a dedicated DirectX render-thread loop. |
| `WarpClock.Themes.Builtin` | Six stock families — **Railway Classic**, **Modern Minimal**, **Antique Worn**, **NERD**, **Scatter (Magnetic)**, and OLED-oriented **Logical** — with explicit Day/Night palettes and OLED variants where specialized rendering is available. |
| `WarpClock.Tests` / `WarpClock.Engine.Tests` / `WarpClock.App.Tests` | Focused xUnit coverage for theme contracts and catalogs, OLED transforms, scheduling, persistence, plug-in loading, CLI parsing, and diagnostics. |
| `WarpClock.App` | Hosted kiosk application with centralized logging/exception routing, Themesets, Tools/Options, timezone rotation, a global ticker, complete UI persistence, diagnostics, and a reloadable plug-in catalog. |
| `WarpClock.Themes.SunFlower` | Sample plug-in: a sunflower dial whose numerals are bees that spin a full turn when a branch-hand sweeps over them. |

## Kiosk mode

The host drops the .NET 11 `KioskModeManager` on the form and wires it to the clock:
**F11** toggles fullscreen, the taskbar can be hidden, sleep/screensaver can be
suppressed, and the `Wakeup` event reports user activity in the status bar.

## Plug-in themes

Drop a theme assembly into the app's `plugins` folder and pick it from the **Theme**
menu (or use **File ▸ Reload Plug-Ins**). Each plug-in is loaded into its own collectible
`AssemblyLoadContext` that shares the contract/runtime types with the host. To author
a new theme from a natural-language description, use the **`warpclock-theme-authoring`**
skill. The stock catalog now ships in-process from `WarpClock.Themes.Builtin`; the
remaining sample drop-in assembly in this solution is `WarpClock.Themes.SunFlower`.

`IClockTheme` remains compatible with existing plug-ins through default interface
members. A logical theme can additionally advertise Day, Night, OLED-Day, and
OLED-Night variants. Themes without the new members continue to behave as Day-only
themes. OLED View uses a dedicated OLED variant when available and otherwise applies
engine-owned pixel drift and slow scaling to the active Day/Night variant.

Themes may optionally declare timezone, weekday, day-of-month, ticker/overlay,
indexed-image, and fraction-second visuals. Render and tick contexts expose immutable
timezone and host ambient snapshots, and `IThemeAnimator.OnTimeZoneChanged` lets a
theme animate a timezone transition. The host still owns the displayed
`TimeZoneInfo`, DST conversion, auxiliary visibility, and all hand targets.

Theme-specific settings can be exposed in the Properties panel. A supported property
must be public and convertible, have a getter/setter, and carry
`[Browsable(true)]`, `[Description]`, and `[Category("Custom Properties")]`. Values
are persisted by logical theme family and reapplied across Day/Night/OLED variants.

## Options, timezones, and ticker

**Tools ▸ Options** uses one Designer-backed page per FluentTabControl tab:

- **Hands** configures Hour, Minute, and Second motion plus the global grace period.
- **Timezones** configures up to six zones (including the default), aliases,
  default/alternate rotation timing, clock-face visibility, and the fallback headline.
- **Display** configures the app-owned bottom ticker and optional in-theme ticker and
  fraction-second visuals.
- **Folders** selects theme, calendar, short-message, and picture locations. Picture
  paths are supplied to themes as ordered ambient image entries.

Timezone rotation alternates default → alternate → default → next alternate. Conversion
is DST-aware and supports fractional-hour zones. If a theme has no timezone visual,
the engine can render a fading upper-left fallback headline. The global ticker consumes
its own band below the clock; it does not overlap or shrink theme-owned coordinates.

## Automation, logs, and persistence

- UI state persists to `%AppData%\WarpClock\settings.json`.
- The default day/night rotation set persists to
  `%AppData%\WarpClock\default.themeset.json`; legacy `themelist.json` data migrates
  automatically. Themesets can also be created, loaded, and saved at arbitrary paths.
- Rolling application logs live under `%AppData%\WarpClock\Logs` with 14-day retention.
- **File** provides New/Edit/Load/Save Themeset, plug-in reload, and Exit commands.
  **Theme** selects the active family/variant, **View** controls presentation, and
  **Tools** opens Options.
- Supported CLI options: `--StartTheme`, `--StartKioskMode`, `--AlwaysOn`,
  `--RecordFramerate`, `--DebugRun 1-15`, and `--DontPersist`.
- `--DebugRun` captures the window and clock every second under
  `%AppData%\WarpClock\Diagnostics\DebugRun-*`.
- Frame-rate recording writes a two-second average for two minutes after each theme
  change to `%AppData%\WarpClock\Diagnostics\FrameRate-*\framerate.csv`.

## Build & run

```pwsh
dotnet build src/WinForms/NET11/WarpClock/WarpClock.slnx
dotnet test src/WinForms/NET11/WarpClock/WarpClock.App.Tests/WarpClock.App.Tests.csproj
src/WinForms/NET11/WarpClock/WarpClock.App/bin/Debug/net11.0-windows10.0.22000.0/WarpClock.exe
```

Requires the .NET 11 SDK and package restore access to the WARP-Toolkit feeds / NuGet
packages used by `WarpClock.Engine` and `WarpClock.App`.
