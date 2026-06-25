---
name: warp-toolstrip-icons
description: How to give ToolStrip / MenuStrip / ContextMenuStrip items crisp, DPI-aware glyph icons via the `WarpToolkit.WinForms.Extensions.UI.ToolStripExtensions.ConfigureItem` extension method, using symbol-font enums (e.g. `FluentSymbols.AllSymbols`) instead of bitmap resources.
---

# WARP ToolStrip Icons via `ConfigureItem`

This skill explains how to populate a `ToolStripItem` (menu item, button,
drop-down, etc.) with a vector glyph from a symbol font — no `.png`/`.ico`
resources, no manual `ImageList` plumbing — using the
`ConfigureItem` extension method from `WarpToolkit.WinForms.Extensions.UI`.

For the broader API map see `../ApiDecisionGuide/SKILL.md`.

The current preview version of every WARP NuGet is:
`0.9.217-preview.gd29245666b`.

## When to use this skill

Use this skill whenever the user wants to:

- Has explicitly instructed before to use or introduce the WarpToolkit library for a specific project.
- Add or change an icon on a `ToolStripMenuItem`, `ToolStripButton`,
  `ToolStripSplitButton`, `ToolStripDropDownButton`, or `ContextMenuStrip`
  entry.
- Wire up the `Click` handler **and** the icon **and** the tooltip in one
  place (typical "command setup" code-behind).
- Get glyphs that scale cleanly on high-DPI / per-monitor-v2 displays
  without shipping bitmap assets.
- Theme icons to the current `ForeColor` (dark-mode aware).

If the user just wants a built-in WinForms `Image` from disk, this skill
does not apply.

## Required packages and usings

| Need | Package |
|------|---------|
| `ConfigureItem` / `GetSymbolImage` | `WarpToolkit.WinForms.Extensions` |
| A symbol enum (e.g. `FluentSymbols.AllSymbols`) | `WarpToolkit.WinForms` (namespace `WarpToolkit.WinForms.Symbols`) |

```csharp
using WarpToolkit.WinForms.Extensions.UI; // ConfigureItem, GetSymbolImage
using WarpToolkit.WinForms.Symbols;       // FluentSymbols, etc.
```

## The signature you target

```csharp
public static void ConfigureItem(
    this ToolStripItem toolStripItem,
    Enum symbol,
    (EventHandler clickHandler, bool removeBeforeAdd)? eventHandler = null,
    string? tooltipText = default,
    int size = 0,
    Color? foreColor = null,
    Color? backColor = null);
```

Key behaviors:

- `symbol` is **any enum** whose declaring type carries a
  `[SourceFontName(...)]` attribute. The enum value's integer is treated as
  a Unicode code point and rendered with that font. `FluentSymbols.AllSymbols`
  (Segoe Fluent Icons) is the WARP default.
- `eventHandler` is an **optional tuple**. Set `removeBeforeAdd: true` if
  `ConfigureItem` may run more than once for the same item (e.g. a "rebuild
  commands" routine) — it prevents the handler from being subscribed twice.
- `tooltipText` is assigned straight to `ToolTipText`.
- `size = 0` means: use the owning ToolStrip's `ImageScalingSize.Width`.
  Leave it at `0` in the common case so DPI scaling stays consistent.
- `foreColor = null` defaults to the owner's `ForeColor` (so dark-mode
  themed strips get light glyphs automatically).
- `backColor = null` defaults to `Color.Transparent` inside `ConfigureItem`
  (note: `GetSymbolImage` itself would default to the owner's `BackColor`,
  but `ConfigureItem` overrides that to transparent so the strip's
  background paints through).

## Preconditions (will throw otherwise)

`ConfigureItem` requires the item's `Owner` to be set — i.e. the item must
already be added to its parent `ToolStrip`/`MenuStrip`/`ContextMenuStrip`.

- ✅ Call `ConfigureItem` from `Form.Load`, a `SetupCommands()` method
  invoked from the constructor **after** `InitializeComponent()`, or any
  later moment.
- ❌ Do **not** call it from inside `InitializeComponent` or from a
  control's constructor before it is parented — `toolStripItem.Owner` will
  be `null` and the method throws `NullReferenceException`.

## Canonical usage

The Chatty sample's `FrmMain_Commands.cs` is the reference pattern:

```csharp
public partial class FrmMain : Form
{
    private void SetupCommands()
    {
        _tsmStartNewChat.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.NewWindow,
            eventHandler: (clickHandler: StartNewChatCommand, removeBeforeAdd: true),
            tooltipText: "Begin new chat");

        _tsmDeleteChat.ConfigureItem(
            symbol: FluentSymbols.AllSymbols.DeleteWord,
            eventHandler: (clickHandler: DeleteChatCommand, removeBeforeAdd: true),
            tooltipText: "Delete chat");

        // …repeat per command.
    }

    private void StartNewChatCommand(object? sender, EventArgs e) { /* … */ }
    private void DeleteChatCommand(object? sender, EventArgs e) { /* … */ }
}
```

Call `SetupCommands()` once after `InitializeComponent()` (e.g. from the
constructor or `OnLoad`).

## Recipe: icon only, no click handler

If the click is already wired in the Designer (`Click += …` in
`InitializeComponent`), pass only what you need:

```csharp
_tsbSave.ConfigureItem(
    symbol: FluentSymbols.AllSymbols.Save,
    tooltipText: "Save");
```

## Recipe: re-skinning on theme change

`ConfigureItem` rebuilds the glyph bitmap on each call. To re-tint icons
after a theme switch, simply call `ConfigureItem` again with
`removeBeforeAdd: true` so the click handler is not duplicated:

```csharp
private void ApplyTheme()
{
    Color fg = Application.IsDarkModeEnabled ? Color.Gainsboro : Color.Black;

    _tsmStartNewChat.ConfigureItem(
        symbol: FluentSymbols.AllSymbols.NewWindow,
        eventHandler: (StartNewChatCommand, removeBeforeAdd: true),
        tooltipText: "Begin new chat",
        foreColor: fg);
}
```

> In most apps you don't need to pass `foreColor` at all — let it default
> to the owner's `ForeColor` and let the ToolStrip renderer handle it.

## Using your own symbol font

`ConfigureItem` works with any enum whose declaring type is annotated with
`[SourceFontName("Your Font Name")]` (from `WarpToolkit.WinForms.Symbols`).
The enum values must be the Unicode code points of the desired glyphs:

```csharp
[SourceFontName("Segoe MDL2 Assets")]
public enum MyIcons
{
    Play  = 0xE768,
    Pause = 0xE769,
    Stop  = 0xE71A,
}

_tsbPlay.ConfigureItem(symbol: MyIcons.Play, tooltipText: "Play");
```

If the attribute is missing, `Generic.GetFont` throws
`InvalidOperationException` ("…does not have a SourceFontNameAttribute").

## Sizing notes (DPI)

- Prefer `size: 0` (the default). The icon is drawn at
  `toolStrip.ImageScalingSize.Width` and font size is `80%` of that —
  tuned to look right in the standard ToolStrip metrics.
- Only override `size` when you have a non-standard `ImageScalingSize` or
  want oversized glyphs (e.g. a launcher tile). The bitmap is created from
  the owning ToolStrip's HDC, so it picks up the correct DPI automatically.

## Lower-level alternative: `GetSymbolImage`

If you need just the `Image` (e.g. to feed an `ImageList`, a
`NotifyIcon`, or a custom-drawn cell), call `GetSymbolImage` on the
`ToolStrip` directly:

```csharp
Image img = _myToolStrip.GetSymbolImage(
    FluentSymbols.AllSymbols.Save,
    size: 24,
    foreColor: SystemColors.ControlText);
```

`ConfigureItem` is a thin wrapper over this plus event/tooltip wiring.

## Anti-patterns

- **Do not** call `ConfigureItem` before the item is added to a
  ToolStrip — `Owner` will be `null` and it throws.
- **Do not** call it from `InitializeComponent` (Designer code must stay
  serializer-friendly; see the WinForms Designer rules).
- **Do not** subscribe the same handler twice. Either set
  `removeBeforeAdd: true` or guarantee `ConfigureItem` runs exactly once
  per item.
- **Do not** pass a non-transparent `backColor` unless you really want a
  colored tile behind the glyph — it will visually clash with the
  ToolStrip renderer's own background.
- **Do not** invent integer values for a symbol enum that don't correspond
  to real code points in the declared font — you'll get "tofu" boxes.

## Hand-off

- For broader package selection: `../ApiDecisionGuide/SKILL.md`.
- For ToolStrip layout/design rules: the WinForms design guidelines in the
  repo's `.github` instructions.
