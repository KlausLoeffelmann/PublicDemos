---
name: warp-directx-getting-started
description: Use this skill get first-steps for how to use Direct2D and DirectWrite via the WARP-Toolkit Direct2D/DirectWrite libraries, when tasks contexts require to improve render speed (drastically!), need to visualize complex scenarios, use Visuals (sprites) for animation, or need to modernize WinForms custom controls for speedy, animated content rendering. These are fundamentals — package, namespaces, the D2DControl/D2DPanel/D2DForm base types, the ID2DGraphics drawing surface, and the RenderMode backends — which shows the minimal code to reliably render shapes and text, and provides a decision table that routes deeper questions to the DComp-exclusive top-level-window skill and the Visuals skill.
---

# WARP DirectX — Decision & Getting Started

`WarpToolkit.WinForms.DirectX` gives WinForms a **hardware-accelerated**
(Direct2D / DirectWrite) drawing surface with a GDI+-style API. You draw with
familiar verbs (`FillRectangle`, `DrawLine`, `DrawString`) but the surface is a
flip-model DXGI swap chain composited through DirectComposition.

Current preview version: `0.9.217-preview.gd29245666b`.
Targets: `net10.0-windows10.0.22000.0` / `net11.0-windows10.0.22000.0`.

Reference: `src/docs/reference/WarpToolkit.WinForms.DirectX.md`.

## Namespaces

```csharp
using WarpToolkit.WinForms.DirectX.Controls; // D2DControl, D2DPanel, D2DForm, RenderMode
using WarpToolkit.WinForms.DirectX.D2D;      // ID2DGraphics, D2DGraphics, D2DRenderEventArgs
```

## The base types

| Type | Use it for |
|------|------------|
| `D2DPanel` | A Direct2D drawing **panel** that can also host child WinForms controls. The everyday choice for "GDI+ but accelerated" regions. |
| `D2DForm` | A **top-level form** whose entire client area is a Direct2D surface. Use for fullscreen / kiosk / game-loop apps. See the *DComp-Exclusive-TLW* skill. |
| `D2DControl` | The shared base both derive from. Subclass directly only when you need a custom accelerated control. |

Ready-made controls (`D2DLabel`, `D2DPictureBox`, document elements) also exist;
start with `D2DPanel` / `D2DForm` and reach for those later.

> Each host owns its own device-manager bundle. **Do not** share an
> `IDWriteFactory` / device manager across hosts or threads.

## How you draw: the render callback

A host raises a render event each frame. The event args
(`D2DRenderEventArgs`) carry everything you need:

- `e.Graphics` — an `ID2DGraphics` (GDI+-style surface: `FillRectangle`,
  `DrawRectangle`, `DrawLine`, `DrawString`, `Clear`, `Transform`, `ClipBounds`,
  `AntialiasMode`, …).
- `e.FrameDelta` — `TimeSpan` since the previous frame (drive animation with this).
- `e.FrameIndex` — monotonically increasing frame counter.

`D2DPanel` exposes a **`RenderBackground`** event (or override
`OnRenderBackground`); `D2DForm` exposes a **`Render`** event.

## Quick start A — accelerated panel (most common)

```csharp
using WarpToolkit.WinForms.DirectX.Controls;
using WarpToolkit.WinForms.DirectX.D2D;

internal sealed class MyCanvas : D2DPanel
{
    public MyCanvas()
    {
        BackColor = Color.Black;
        RenderBackground += OnRenderBackground;
    }

    private void OnRenderBackground(object? sender, D2DRenderEventArgs e)
    {
        ID2DGraphics g = e.Graphics;
        g.AntialiasMode = AntialiasMode.AntiAlias;

        g.FillRectangle(Color.FromArgb(220, 40, 120, 210), new RectangleF(20, 20, 240, 120));
        g.DrawRectangle(Color.White, 3f, new RectangleF(20, 20, 240, 120));

        using Font font = new("Segoe UI", 14f, FontStyle.Bold);
        g.DrawString("Hello Direct2D", font, Color.White, 32f, 40f);
    }
}
```

Drop `MyCanvas` on a form like any panel. With the default render mode it paints
on demand; call `Invalidate()` to request a repaint.

## Quick start B — full-window form

```csharp
internal sealed partial class GameForm : D2DForm
{
    public GameForm()
    {
        InitializeComponent();
        Render += OnFrameRender;
    }

    private void OnFrameRender(object? sender, D2DRenderEventArgs e)
    {
        e.Graphics.Clear(Color.Black);
        // draw your scene using e.FrameDelta for time-based motion
    }
}
```

For animated content drive frames with a background render mode (below) and/or a
timer — see the *Using-Visuals* skill.

## Render modes — `RenderMode`

`RenderMode` selects the backend and, for the Direct2D modes, the threading /
frame-driver model. Set it via the `RenderMode` property (default
`D2DWinFormsClassic`).

| Mode | What it does | Choose when |
|------|--------------|-------------|
| `GDIPlus` | **Not** Direct2D — normal WinForms/GDI+ painting, no `ID2DGraphics`. | You want a GDI+ fallback / A-B comparison. |
| `D2DWinFormsClassic` *(default)* | D2D driven on demand by `WM_PAINT` on the **UI thread**. The render handler may read WinForms control state directly. | Most UI; static or invalidate-driven content. |
| `D2DSharedRenderThread` | One process-wide background thread clocked by a high-precision timer; render runs **off** the UI thread. | Many simple controls that should stay visually in sync. |
| `D2DDedicatedRenderThread` | One background render thread **per host**. | A heavy, independently-animated surface. |

Background-thread modes do **not** run on the UI thread: snapshot any WinForms
state (sizes, view-model values) on the UI thread before the render handler
consumes it.

Frame-loop knobs (on every host):

- `PreserveLastFrame` (default `true`) — GDI-like "what's on screen stays"; a
  render handler that issues no draw calls keeps the last frame. Set `false`
  for game-loop auto-clear to `BackColor` every frame.
- `TargetFrameRate` — used by timer-driven modes when `VSyncEnabled` is `false`
  (default 50 Hz).
- `VSyncEnabled` — whether `Present` waits for vblank; defaults derive from the
  render mode.

## Decision table — where to go next

| If you want to… | Go to |
|-----------------|-------|
| Draw shapes/text in a panel, pick a render mode, basic invalidate-driven UI | **this skill** |
| Build a **fullscreen / kiosk / game-loop** app whose whole window is Direct2D, with no GDI redirection surface | **`warp-directx-dcomp-exclusive-tlw`** |
| Run a **render/animation loop** with retained **Visuals** for flicker-free, composited scenes | **`warp-directx-using-visuals`** |
| Add normal modern WinForms controls around the surface | `warp-winforms-controls` |
| Decide which WARP package a non-rendering task needs | `warp-api-decision-guide` |

## Anti-patterns

- **Do not** share an `IDWriteFactory` / device manager across hosts or threads
  — each host owns its own bundle by design.
- **Do not** read WinForms control state from a background-thread render handler
  (`D2DSharedRenderThread` / `D2DDedicatedRenderThread`) — snapshot it on the UI
  thread first.
- **Do not** expect `e.Graphics` in `GDIPlus` mode — that mode has no Direct2D
  surface.
