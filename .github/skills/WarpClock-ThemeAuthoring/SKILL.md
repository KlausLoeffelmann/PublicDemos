---
name: warpclock-theme-authoring
description: Use this skill to turn a natural-language "mad clock design" description into a compiling WarpClock plug-in theme (an IClockTheme drop-in assembly) for the WARP DirectX visual wall clock in src/WinForms/NET11/WarpClock. It explains the plug-in contract, the absolute time-correctness invariant (a theme can NEVER set a hand angle), the limited influence surface (anchors, per-element parameters, face rotation), the radial vs free-floating + grace-catch-up rules, and gives a copy-paste scaffold plus build/drop-in steps. Invoke it whenever the user asks to "create a clock theme", "make a WarpClock plug-in", or describes a custom analog-clock look/behavior to generate.
---

# WarpClock — Theme Authoring

Generate a **drop-in plug-in theme** for the WARP DirectX visual wall clock
(`src/WinForms/NET11/WarpClock`). A theme describes the clock's parts as visuals,
draws them, and (optionally) animates a limited set of parameters. The **engine**
owns time and hand pointing — your theme can never show the wrong time.

## The one rule you must never break

**A theme cannot set a hand's angle.** Hand rotation is *derived by the engine*
from the position of the hand's target anchor. You may:

- move where the hour/minute/second anchors are (layout),
- change how parts look (renderer),
- tweak per-element parameters and a global face rotation each tick (animator).

If your idea seems to require "rotate the second hand to X°", express it instead
as "put the second-hand target anchor at position P" — the engine will aim the
hand there. This is what guarantees the time is always right.

## The contract (`WarpClock.Abstractions`)

Implement `IClockTheme`:

```csharp
public interface IClockTheme
{
    string Name { get; }
    string Description { get; }
    string Author { get; }
    ThemeCapabilities Capabilities { get; }
    IReadOnlyList<ClockElementDescriptor> CreateElements();
    IReadOnlyList<ClockThemeVariantKind> SupportedVariants { get; } // default: Day only
    IClockTheme ResolveVariant(ClockThemeVariantKind variant);      // default supports Day
    IClockLayout CreateLayout();
    IClockElementRenderer CreateRenderer();
    IThemeAnimator? CreateAnimator();   // null for a static theme
}
```

Key types:

- **`ClockElementDescriptor`** — one visual. `Id` (`ClockElementId`), `ContentSize`
  (design units; dial radius is 500), `Pivot` (rotation center within the content),
  `Hand` (`ClockHandKind.None`/`Hour`/`Minute`/`Second`/`SubSecond`), `ZOrder`,
  `RedrawPerFrame` (set true if the content depends on the current time).
  **Hands must be authored pointing straight up (toward 12) from the pivot.**
- **`ClockElementId`** — `Face`, `HourMarker(0..11)` (0 = the 12 position),
  `MinuteTick(0..59)`, `HourHand`, `MinuteHand`, `SecondHand`, `Arbour`,
  plus optional `TimeZone`, `Day`, `Weekday`, `OverlayMessage`,
  `IndexedImage(n)`, `FractionSecondDial`, and `SubSecondHand` visuals.
- **`IClockLayout.TryGetAnchor(id, surface, out anchor)`** — return `true` with a
  pixel anchor to relocate an element; return `false` to use the engine's default
  radial placement. Anchors you relocate are what hands aim at.
- **`IClockElementRenderer.DrawElement(ID2DGraphics g, IClockRenderContext ctx)`** —
  draw the element in its local pixel space (origin top-left of `ctx.ContentSize`).
  The surface is pre-cleared and `BeginDraw` already issued; do **not** call
  BeginDraw/EndDraw. Scale design→pixels with `ctx.Scale`.
- **`IThemeAnimator.OnTick(IClockTickContext ctx)`** — called once per rendered frame
  on the dedicated render thread. Integrate with `ctx.FrameDelta`; read `ctx.Time`,
  `ctx.TimeZone`, `ctx.Ambient`, and `ctx.SurfaceSize`. Mutate
  `ctx.GetParameters(id)` and `ctx.FaceRotationDegrees`. Never set hand angles.
- **`IThemeAnimator.OnTimeZoneChanged(...)`** — optional default interface callback
  for animating a host-selected timezone or DST-offset transition.
- **`IClockRenderContext`** also exposes the authoritative `TimeZone` and immutable
  host `Ambient` snapshot. Ambient data can contain timezone alias/designation,
  default/alternate presentation state, ticker text, overlay text, and ordered image
  paths. Treat it as read-only and do not perform file I/O in the renderer.
- **`ClockElementParameters`** — the only runtime levers: `Visible`,
  `AnchorOffset` (design units), `Scale`, `SkewDegrees`, `ExtraRotationDegrees`
  (clamped to ±5° for hands), `Opacity`, `Text`, `Progress`, `RedrawRequested`,
  `HandTargetMode`, and an optional theme-local `HandMotion`. A hand may request
  `Radial`, `FreeFloating`, or
  `MagneticNumerals`; the engine still computes the authoritative angle and safely
  rejects unsupported requests. `FreeFloating` aims the hour hand at the `HourMarker`
  anchors but the minute/second hands at the 60 `MinuteTick` anchors — a theme that
  does not materialize minute ticks gets the engine's default radial ring for those
  hands. `MagneticNumerals` uses only the current live hour-numeral anchor, then adds
  the authoritative clockwise progress through that numeral's 30-degree interval.
  Hour hands use one numeral per hour; minute and second hands use one numeral per
  five units. Exact boundaries point at the numeral center, and the next numeral is
  never used as an interpolation target. A theme whose design depends on this aiming
  must request `MagneticNumerals` explicitly: that mode is honored even when the
  host's global magnetic switch is off, whereas `ThemeCapabilities.MagneticByDefault`
  is only a hint the host may ignore.

## Variants and OLED

Existing plug-ins remain compatible: default interface members make them Day-only.
New families can expose `ClockThemeVariants.DayNight` or
`ClockThemeVariants.DayNightOled` and return a concrete instance from
`ResolveVariant`. Day is the compatibility default. Night palettes must avoid bright
faces and harsh yellow/red accents. OLED variants should use a pitch-black background
and restrained contrast. If no dedicated OLED variant exists, the host applies
engine-owned pixel drift and slow scale movement.

## Optional visuals

Declare only the visuals the theme actually renders. The host can independently gate
timezone, date, weekday, fraction-second, overlay/ticker, and indexed-image elements.
Use `RedrawPerFrame = true` when text depends on current time or ambient content.
Timezone labels should prefer `ctx.Ambient.TimeZoneAlias`, then designation, then the
engine snapshot. Do not convert UTC or cache offsets in a theme; the engine supplies
DST-correct displayed time and calls `OnTimeZoneChanged`.

The app-owned bottom ticker is separate from a theme-owned `OverlayMessage` visual:
the app ticker consumes layout space below the clock, while a theme visual remains
inside the DirectComposition scene.

## Custom theme properties

Theme-specific values can appear in the safe Properties proxy and persist across
variants. Every exposed property must be public, convertible through a
`TypeConverter`, have a public getter and setter, and include all three attributes:

```csharp
[Browsable(true)]
[Description("Controls the accent used for numerals and hands.")]
[Category("Custom Properties")]
public Color AccentColor { get; set; }
```

Use the same public property on every concrete variant. The host stores values by
logical family/property name and reapplies them after variant resolution.

## Radial vs free-floating

- **Radial** (default): anchors sit on the dial circle; hands use the authoritative
  angle. `Crawling` eases to each step and pauses, `Sweep` glides continuously,
  `FastTick` advances in quarter steps, and `Tick` jumps once per step.
- **Free-floating** (`Capabilities.FreeFloating = true`): you place anchors anywhere
  and hands *aim at them*, so a hand's tip follows a relocated visual. The engine
  applies the configured Crawl/Sweep/Tick quantization to the engine-owned target and
  uses the global **grace catch-up** (1–30s) where smoothing is needed. Set
  `HandsFollowFaceRotation` to choose whether hands rotate with a spinning face.

## Scaffold a new theme

1. Create a project `src/WinForms/NET11/WarpClock/WarpClock.Themes.<Name>` modeled on
   `WarpClock.Themes.SunFlower` (TFM `net11.0-windows10.0.22000.0`; reference
   `WarpClock.Abstractions` and `WarpToolkit.WinForms.DirectX` with `Private="false"`).
2. Add it to `WarpClock.App.csproj` as a `ReferenceOutputAssembly=false` ProjectReference
   and to the `CopyClockPlugins` target so its dll is copied to `bin/.../plugins`.
3. Implement `IClockTheme` plus its layout/renderer/(animator).

Minimal static example:

```csharp
public sealed class MyTheme : IClockTheme
{
    public string Name => "My Theme";
    public string Description => "…";
    public string Author => "…";
    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

    public IReadOnlyList<ClockElementDescriptor> CreateElements() =>
    [
        new() { Id = ClockElementId.Face, ContentSize = new(1000,1000), Pivot = new(500,500), ZOrder = 0 },
        new() { Id = ClockElementId.HourHand,   ContentSize = new(60,360), Pivot = new(30,290), Hand = ClockHandKind.Hour,   ZOrder = 30 },
        new() { Id = ClockElementId.MinuteHand, ContentSize = new(50,470), Pivot = new(25,400), Hand = ClockHandKind.Minute, ZOrder = 31 },
        new() { Id = ClockElementId.SecondHand, ContentSize = new(30,520), Pivot = new(15,440), Hand = ClockHandKind.Second, ZOrder = 32 },
        new() { Id = ClockElementId.Arbour, ContentSize = new(60,60), Pivot = new(30,30), ZOrder = 40 },
    ];

    public IClockLayout CreateLayout() => new RadialLayout();        // returns false from TryGetAnchor
    public IClockElementRenderer CreateRenderer() => new MyRenderer();
    public IThemeAnimator? CreateAnimator() => null;
}
```

## Recipes mapped to the contract

- *"Show only N hours / lose hours"* → create only those `HourMarker` descriptors,
  or toggle `Parameters.Visible` from the animator; relocate anchors via a custom
  layout (free-floating) so the hour hand follows them. See
  `WarpClock.Themes.LoseHour`.
- *"A hand that displays data (binary, text, …)"* → set `RedrawPerFrame = true` and
  read `ctx.Time` in the renderer to draw the read-out onto the hand. See
  `WarpClock.Themes.Nerd`.
- *"A permanently rotating face with a still hand"* → drive `ctx.FaceRotationDegrees`
  in the animator and set `Capabilities.HandsFollowFaceRotation = false`.
- *"A numeral that falls / blends"* → animate `Parameters.AnchorOffset`,
  `Parameters.Opacity`, `Parameters.Text`, and `Parameters.Progress`, with
  `RedrawRequested = true`.

## Build & drop in

```pwsh
dotnet build src/WinForms/NET11/WarpClock/WarpClock.slnx
```

The plug-in dll is copied to
`WarpClock.App/bin/Debug/net11.0-windows10.0.22000.0/plugins`. Run `WarpClock.exe`
and pick the theme from the **Theme** menu (or use **File ▸ Reload Plug-Ins** after
dropping a freshly built DLL into the theme folder configured under
**Tools ▸ Options ▸ Folders**).

The app schedules theme families with JSON **Themesets** (`*.themeset.json`). A
Themeset defines Day/Night thresholds, rotation timing, theme membership, and
day-only/night-only eligibility. This scheduling is host behavior; plug-ins do not
read or modify Themeset files.

## Anti-patterns

- **Do not** compute or set a hand angle. Move anchors instead.
- **Do not** call `BeginDraw`/`EndDraw` in a renderer — the engine does that.
- **Do not** block, access controls, or perform file/network I/O in `OnTick` or
  `DrawElement` — both run on the dedicated render thread.
- **Do not** copy `WarpClock.Abstractions`/`WarpToolkit.WinForms.DirectX` next to the
  plug-in (`Private="false"`); they must bind to the host's copies.
- **Do not** implement timezone conversion in a theme — consume the host snapshot.
