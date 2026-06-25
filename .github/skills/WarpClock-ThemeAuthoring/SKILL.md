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
  `CustomElement(n)`, etc.
- **`IClockLayout.TryGetAnchor(id, surface, out anchor)`** — return `true` with a
  pixel anchor to relocate an element; return `false` to use the engine's default
  radial placement. Anchors you relocate are what hands aim at.
- **`IClockElementRenderer.DrawElement(ID2DGraphics g, IClockRenderContext ctx)`** —
  draw the element in its local pixel space (origin top-left of `ctx.ContentSize`).
  The surface is pre-cleared and `BeginDraw` already issued; do **not** call
  BeginDraw/EndDraw. Scale design→pixels with `ctx.Scale`.
- **`IThemeAnimator.OnTick(IClockTickContext ctx)`** — called ~10×/second. Read
  `ctx.Time` (authoritative, read-only). Mutate `ctx.GetParameters(id)` and
  `ctx.FaceRotationDegrees`. Never try to set hand angles.
- **`ClockElementParameters`** — the only runtime levers: `Visible`,
  `AnchorOffset` (design units), `Scale`, `SkewDegrees`, `ExtraRotationDegrees`
  (clamped to ±5° for hands), `Opacity`, `Text`, `Progress`, `RedrawRequested`.

## Radial vs free-floating

- **Radial** (default): anchors sit on the dial circle; hands use the authoritative
  angle. `ClockHandMotion.Crawling`/`Sweep`/`Tick` apply. Best for classic looks.
- **Free-floating** (`Capabilities.FreeFloating = true`): you place anchors anywhere
  and hands *aim at them*, so a hand's tip follows a relocated visual. The engine
  **disables Crawling** here and uses **grace catch-up** (1–30s) so hands ease toward
  a moved target. Set `HandsFollowFaceRotation` to choose whether the hands rotate
  with a spinning face.

## Scaffold a new theme

1. Create a project `src/WinForms/NET11/WarpClock/WarpClock.Themes.<Name>` modeled on
   `WarpClock.Themes.Nerd` (TFM `net11.0-windows10.0.22000.0`; reference
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
and pick the theme from the **Theme** menu (or use **Plug-ins ▸ Reload** after
dropping a freshly built dll into the `plugins` folder).

## Anti-patterns

- **Do not** compute or set a hand angle. Move anchors instead.
- **Do not** call `BeginDraw`/`EndDraw` in a renderer — the engine does that.
- **Do not** block in `OnTick` — it runs on the UI thread at ~10 Hz.
- **Do not** copy `WarpClock.Abstractions`/`WarpToolkit.WinForms.DirectX` next to the
  plug-in (`Private="false"`); they must bind to the host's copies.
- **Do not** enable `Crawling` for a free-floating theme — the engine overrides it.
