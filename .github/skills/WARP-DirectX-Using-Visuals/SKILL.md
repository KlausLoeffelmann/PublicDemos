---
name: warp-directx-using-visuals
description: Use this skill for WARP-DirectX apps with a render/animation loop — games, info tables, dashboards, and fullscreen kiosk scenes — that need flicker-free animation and complex scene composition via retained DirectComposition Visuals (sprites). It explains the Visuals collection (Visuals.AddNew, D2DVisual.Bounds/Visible), per-visual drawing with D2DGraphics.FromVisual, committing changes with CommitVisualsAsync, the PreserveLastFrame/TargetFrameRate/VSyncEnabled loop knobs, and how to drive frames from a high-precision timer.
---

# WARP DirectX — Using Visuals (render loop, animation, scene composition)

> Read **`warp-directx-getting-started`** first for base types, the render
> callback and `RenderMode`. This skill covers **retained Visuals** — the path
> for animated, composited scenes that stay flicker-free.

## Immediate-mode vs retained Visuals

- **Immediate mode** (the *getting-started* `RenderBackground` / `Render`
  handler) redraws the whole surface each frame. Great for simple scenes.
- **Retained Visuals** give each scene element its own DirectComposition visual
  with cached content. You move/show/hide visuals and the **compositor** redraws
  only what changed — ideal for many moving sprites, dashboards, or layered
  scenes where you don't want to re-issue every draw call every frame.

Hosts (`D2DPanel`, `D2DForm`, `D2DControl`) expose a retained `Visuals`
collection (`D2DVisualCollection`).

Current preview version: `0.9.217-preview.gd29245666b`.
                        
## The Visual API

```csharp
using WarpToolkit.WinForms.DirectX.Controls; // D2DVisual, RenderMode
using WarpToolkit.WinForms.DirectX.D2D;      // ID2DGraphics, D2DGraphics
```

- `host.Visuals.AddNew(Rectangle bounds)` → `D2DVisual` (also `AddNew(Point, Size)`).
- `D2DVisual.Bounds` — position/size in host-client coordinates (set to move/resize).
- `D2DVisual.Visible` — show/hide without redrawing content.
- `host.Visuals.Remove(visual)` — remove a visual.
- `D2DGraphics.FromVisual(visual)` → an `ID2DGraphics` to (re)draw **that
  visual's** cached content. Wrap drawing in `BeginDraw()` / `EndDraw()`.
- `await host.CommitVisualsAsync(waitForVSync, cancellationToken)` — commit all
  pending visual changes (adds, bounds, visibility) on the render thread.

## Loop knobs for flicker-free animation

Set these on the host (and on any visual-hosting child surface):

```csharp
RenderMode        = RenderMode.D2DWinFormsClassic; // or a background-thread mode
PreserveLastFrame = true;   // keep last frame when a tick issues no draws
VSyncEnabled      = false;  // pair with TargetFrameRate for a fixed cadence
TargetFrameRate   = 60d;    // Hz, used when VSyncEnabled is false
```

Flip-model presentation + DirectComposition means there is no GDI "erase then
paint" step, so retained visuals are inherently flicker-free.

## Setting up a visual collection

```csharp
private readonly List<D2DVisual> _visuals = new();

private void BuildScene(Rectangle sceneBounds)
{
    for (int i = 0; i < 200; i++)
    {
        Rectangle bounds = CreateRandomBounds(sceneBounds);
        D2DVisual visual = Visuals.AddNew(bounds);
        DrawVisual(visual, i);
        _visuals.Add(visual);
    }

    CommitVisuals();
}

private void DrawVisual(D2DVisual visual, int index)
{
    // Draw in the visual's own coordinate space (0,0 = visual top-left).
    using ID2DGraphics g = D2DGraphics.FromVisual(visual);
    g.BeginDraw();
    g.Clear(Color.Transparent);

    Rectangle local = new(0, 0, visual.Bounds.Width, visual.Bounds.Height);
    g.FillRectangle(Color.FromArgb(200, 60, 160, 220), local);
    using Font font = new("Segoe UI", 10f, FontStyle.Bold);
    g.DrawString(index.ToString(), font, Color.White, 6f, 5f);

    g.EndDraw();
}

private void CommitVisuals()
    => CommitVisualsAsync(VSyncEnabled).GetAwaiter().GetResult();
```

Redraw a visual's content (`FromVisual` + `BeginDraw`/`EndDraw`) only when its
**appearance** changes. To merely move it, set `visual.Bounds` and commit — no
redraw needed.

## Driving the animation loop

Use a high-precision timer (`WarpToolkit.Windows.Interop.PrecisionTimer`) to tick
the simulation, then advance bounds and commit. Compute motion from elapsed time
so speed is frame-rate independent.

```csharp
using System.Diagnostics;
using WarpToolkit.Windows.Interop.PrecisionTimer;

private readonly HighPrecisionTimer _timer = new();
private object? _registration;
private long _lastTimestamp;

private void StartLoop()
{
    _lastTimestamp = Stopwatch.GetTimestamp();
    _registration = _timer.Register(
        this,
        static form => form.Tick(),
        new PeriodicTimerOptions
        {
            Interval       = TimeSpan.FromMilliseconds(1000d / 60d),
            MaxConcurrency = 1,
            OverloadPolicy = OverloadPolicy.SkipAndLog,
        });
}

private void Tick()
{
    float seconds = (float)Stopwatch.GetElapsedTime(_lastTimestamp).TotalSeconds;
    _lastTimestamp = Stopwatch.GetTimestamp();

    Rectangle scene = ClientRectangle;
    foreach (D2DVisual v in _visuals)
    {
        // advance position using your own velocity state…
        v.Bounds = StepBounds(v.Bounds, scene, seconds);
    }

    CommitVisuals();   // one commit per frame
    Invalidate();      // request presentation
}

private void StopLoop()
{
    if (_registration is not null)
    {
        _timer.Unregister(_registration);
        _registration = null;
    }
}
```

For a continuously running scene you can also choose a background render mode
(`D2DSharedRenderThread` / `D2DDedicatedRenderThread`); remember those run off
the UI thread, so snapshot any WinForms state on the UI thread first.

## Recommended frame shape

1. **Tick** — update simulation state from elapsed time.
2. **Apply** — set `visual.Bounds` / `visual.Visible`; redraw content only if
   appearance changed (`D2DGraphics.FromVisual` + `BeginDraw`/`EndDraw`).
3. **Commit** — exactly **one** `CommitVisualsAsync` per frame.
4. **Present** — `Invalidate()`.

## Anti-patterns

- **Do not** call `CommitVisualsAsync` per-visual — batch all changes and commit
  once per frame.
- **Do not** recreate visuals every frame — create once, then mutate
  `Bounds`/`Visible`. Recreate only when the scene's element set changes.
- **Do not** redraw a visual's cached content every frame just to move it — set
  `Bounds` instead; redraw only on appearance changes.
- **Do not** use wall-clock deltas from a low-resolution timer for motion — use a
  high-precision timer / `Stopwatch.GetElapsedTime` so animation is smooth and
  frame-rate independent.
- **Do not** touch UI-thread WinForms state from a background render mode without
  snapshotting it on the UI thread first.
