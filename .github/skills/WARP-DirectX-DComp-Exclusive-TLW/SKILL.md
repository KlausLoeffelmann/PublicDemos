---
name: warp-directx-dcomp-exclusive-tlw
description: Use this skill when building a WinForms app whose top-level window is rendered purely by WARP-DirectX (a D2DForm) using DirectComposition-exclusive rendering — fullscreen, kiosk, or game-style windows that have NO GDI parent redirection surface. It explains the prerequisites, the benefits, the UseDirectCompositionExclusiveRendering switch (WS_EX_NOREDIRECTIONBITMAP), the critical "every child must also be Direct2D/DComp content" constraint, and shows a minimal working DComp-exclusive form.
---

# WARP DirectX — DirectComposition-Exclusive Top-Level Window

> Read **`warp-directx-getting-started`** first for the base types, the render
> callback, and `RenderMode`. This skill is the deep dive on top-level windows
> that are owned **exclusively** by DirectComposition.

A `D2DForm` renders its client area through a flip-model DXGI swap chain hosted
by DirectComposition. By going one step further and making the form's HWND
**DComp-exclusive**, Windows never allocates a GDI **redirection bitmap** for
that window — the window is pure composited content.

Current preview version: `0.9.217-preview.gd29245666b`.

## What "exclusive" means

`UseDirectCompositionExclusiveRendering` (on `D2DControl` / `D2DForm`) controls
one thing: whether the HWND requests the `WS_EX_NOREDIRECTIONBITMAP` extended
style.

- **Enabled** → no GDI redirection bitmap is allocated; the OS treats the HWND
  as DirectComposition-only content.
- The default differs by host:
  - `D2DPanel` / `D2DControl`: default **`true`** (their HWND is meant to be a
    DComp surface).
  - `D2DForm`: default **`false`**, because a top-level redirection bitmap is the
    fallback surface that **classic child HWNDs** paint onto.

So a `D2DForm` only becomes DComp-*exclusive* when you opt in.

## The one hard prerequisite: no classic child HWNDs

Classic WinForms child controls (`Button`, `TextBox`, `Panel`, a stock
`DataGridView`, …) render onto the **parent** window's redirection bitmap.

A DComp-exclusive top-level window **has no parent redirection surface** — there
is nothing for classic child content to render onto. Therefore:

> **Every child of a DComp-exclusive `D2DForm` must itself be Direct2D /
> DirectComposition content** (e.g. `D2DPanel`, `D2DLabel`, other `D2DControl`s,
> or visuals you draw yourself). Do not place classic GDI child controls on it.

If you need ordinary WinForms controls (menus, toolbars, dialogs), keep the form
**non-exclusive** (the default `D2DForm`) — you still get an accelerated client
area, just with the redirection bitmap available for classic children.

## Prerequisites checklist

- `net10.0-windows10.0.22000.0`+ (DirectComposition is a Windows-only target).
- A form deriving from `D2DForm`.
- Children, if any, are Direct2D/DComp content — **not** classic GDI controls.
- For animated content, pick a render mode and/or drive frames (see
  *Using-Visuals*).

## Benefits

- **No redirection bitmap** → lower memory and no GDI copy-back step; the
  window is composited directly by the DWM.
- **Tear-free, flip-model presentation** for fullscreen / kiosk / game UIs.
- **Clean compositor ownership** — ideal when the whole window is your scene and
  there are no stock controls to host.

## Minimal working DComp-exclusive form

```csharp
using WarpToolkit.WinForms.DirectX.Controls;
using WarpToolkit.WinForms.DirectX.D2D;

internal sealed partial class KioskForm : D2DForm
{
    public KioskForm()
    {
        InitializeComponent();

        // Opt the top-level window into DirectComposition-exclusive rendering.
        UseDirectCompositionExclusiveRendering = true;

        FormBorderStyle = FormBorderStyle.None;
        WindowState     = FormWindowState.Maximized; // fullscreen kiosk

        Render += OnFrameRender;
    }

    private void OnFrameRender(object? sender, D2DRenderEventArgs e)
    {
        ID2DGraphics g = e.Graphics;
        g.Clear(Color.Black);

        using Font font = new("Segoe UI", 48f, FontStyle.Bold);
        g.DrawString("KIOSK READY", font, Color.White, 80f, 80f);
    }
}
```

Notes:
- Set `UseDirectCompositionExclusiveRendering` early (constructor / before the
  handle is created). Use `ResetUseDirectCompositionExclusiveRendering()` to
  return to the host default.
- `DefaultUseDirectCompositionExclusiveRendering` tells you the host's default
  (`false` for forms, `true` for panels/controls).
- The OS still draws the **title bar / borders** if you keep them; only the
  client area is Direct2D. For a true fullscreen kiosk, drop the chrome as above.

## Render modes & frame loop

All `RenderMode` options and the `PreserveLastFrame` / `TargetFrameRate` /
`VSyncEnabled` knobs from *getting-started* apply unchanged. For a continuously
animating kiosk/game, select a background render mode and/or drive frames from a
timer and use retained visuals — see **`warp-directx-using-visuals`**.

## Anti-patterns

- **Do not** put classic GDI child controls on a DComp-exclusive `D2DForm` —
  they have no redirection surface to paint onto and will not render correctly.
  Either make the children Direct2D content, or keep the form non-exclusive.
- **Do not** toggle `UseDirectCompositionExclusiveRendering` after the window is
  shown and expecting child-hosting behavior to change retroactively — decide at
  construction time.
- **Do not** assume exclusivity is required for acceleration — a plain
  (non-exclusive) `D2DForm` already renders its client area with Direct2D.
