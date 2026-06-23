# WARP DirectX Wall Clock

A hardware-accelerated analog wall clock for WinForms (.NET 11) where **every part
of the clock — face, numerals, ticks, hands, arbour — is its own DirectComposition
visual**, drawn with [WARP-Toolkit](../../../../) Direct2D and composed by a small
engine. Clock looks and behaviors are supplied by **drop-in plug-in themes**, and the
whole thing doubles as a demo for the new .NET 11 `KioskModeManager` component.

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
| `WarpClock.Engine` | `WarpClockControl` (a `D2DPanel`): authoritative time, anchor resolution, hand-pointing solver + grace, transform composition, the high-precision frame loop. |
| `WarpClock.Themes.Builtin` | The three built-in radial themes: **Railway Classic**, **Modern Minimal**, **Antique Worn**. |
| `WarpClock.App` | The kiosk host form (real `System.Windows.Forms.KioskModeManager`), theme menu, plug-in loader/watcher. |
| `WarpClock.Themes.Nerd` | Sample plug-in: a single second hand that encodes hour & minute in binary; octal hour markers. |
| `WarpClock.Themes.Scatter` | Sample free-floating plug-in: hour numerals scattered (and drifting) across the canvas to demonstrate **Magnetic numerals** and the tri-state numeral visibility. |
| `WarpClock.Themes.SunFlower` | Sample plug-in: a sunflower dial whose numerals are bees that spin a full turn when a branch-hand sweeps over them. |

## Kiosk mode

The host drops the .NET 11 `KioskModeManager` on the form and wires it to the clock:
**F11** toggles fullscreen, the taskbar can be hidden, sleep/screensaver can be
suppressed, and the `Wakeup` event reports user activity in the status bar.

## Plug-in themes

Drop a theme assembly into the app's `plugins` folder and pick it from the **Theme**
menu (or **Plug-ins ▸ Reload**). Each plug-in is loaded into its own collectible
`AssemblyLoadContext` that shares the contract/runtime types with the host. To author
a new theme from a natural-language description, use the **`warpclock-theme-authoring`**
skill.

## Build & run

```pwsh
dotnet build src/WinForms/NET11/WarpClock/WarpClock.slnx
src/WinForms/NET11/WarpClock/WarpClock.App/bin/Debug/net11.0-windows10.0.22000.0/WarpClock.exe
```

Requires the .NET 11 SDK and the WARP-Toolkit checkout next to this repository
(referenced via project references, like the other WARP demos).
