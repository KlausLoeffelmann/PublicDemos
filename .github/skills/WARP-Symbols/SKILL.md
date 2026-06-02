---
name: warptoolkit-symbols
description: Guide for WarpToolkit.WinForms FluentSymbols — the Segoe Fluent Icons glyph enums (AllSymbols, CommonToolStripSymbols, DevelopmentSymbols, TreeViewSymbols) and how they render onto ToolStrip items via the Extensions ConfigureItem helper. Use this when adding crisp, DarkMode-aware icon glyphs to ToolStrip/menu/button UI instead of bitmap icons.
---

# WarpToolkit.WinForms Symbols

`FluentSymbols` exposes named glyphs from the **"Segoe Fluent Icons"** font as
strongly-typed enums, so you can render crisp, scalable, theme-aware icons
instead of shipping bitmap icon libraries.

`public static partial class FluentSymbols` — namespace
`WarpToolkit.WinForms.Symbols`. The glyph enums are **nested** in it and decorated
with `[SourceFontName("Segoe Fluent Icons")]`.

> **Source of truth:** verified against
> `src/WarpToolkit.WinForms/SymbolFactory/FluentSymbols.*.cs`. Real usage:
> `samples/Self Managed/cs/WarpToolkit.GitScanner/UI/Console/ConsoleTab.cs`. For
> a usage-focused companion, see the `WARP-ToolStripIcons` skill.

## When to Use This Skill

- Adding **icon glyphs** to ToolStrip buttons, menu items, or buttons.
- Preferring **vector font icons** over bitmaps for crisp High-DPI scaling and
  automatic Light/Dark parity.

## The glyph enums

All are nested under `FluentSymbols` (reference them as `FluentSymbols.<Enum>`):

| Enum | Purpose | Sample members (value = Unicode code point) |
|------|---------|---------------------------------------------|
| `FluentSymbols.AllSymbols` | The full Segoe Fluent Icons set | `GlobalNavButton = 0xE700`, `Wifi = 0xE701`, `Copy`, `SaveAs`, `Stopwatch` |
| `FluentSymbols.CommonToolStripSymbols` | Curated command-bar icons | `Send = 0xE724`, `Accept = 0xE8FB`, `Cancel = 0xE711`, `New = AddBold`, `Open = OpenFile`, `Edit = 0xE70F` |
| `FluentSymbols.DevelopmentSymbols` | Dev-tool icons | `Code = 0xE943`, `Component = 0xE950`, `DeveloperTools = 0xEC7A`, `CommandPrompt = 0xE756` |
| `FluentSymbols.TreeViewSymbols` | Tree/expander/folder icons | `ChevronDown = 0xE70D`, `ChevronRight = 0xE76C`, `Folder = 0xE8B7`, `FolderOpen = 0xE838` |

Each enum value is the glyph's Unicode code point in the source font. Some
members alias others (e.g. `New = AddBold`, `Open = OpenFile`).

## Rendering glyphs onto ToolStrip items

The render helper is the **`ConfigureItem` extension** in
`WarpToolkit.WinForms.Extensions` (namespace
`WarpToolkit.WinForms.Extensions.UI`) — not in this package. Signature:

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

It accepts **any** of the glyph enums (parameter type is `Enum`). The item's
`Owner` must be set first (it throws otherwise), so call it **after** the item is
added to a `ToolStrip`/menu.

```csharp
using WarpToolkit.WinForms.Extensions.UI;
using WarpToolkit.WinForms.Symbols;

// 1) Add the item to its owner first.
_strip.Items.Add(_copyButton);

// 2) Then configure the glyph (Owner is now set).
_copyButton.ConfigureItem(
    symbol: FluentSymbols.AllSymbols.Copy,
    tooltipText: "Copy selection");

_saveButton.ConfigureItem(
    symbol: FluentSymbols.CommonToolStripSymbols.Save,
    eventHandler: (OnSaveClick, removeBeforeAdd: true),
    tooltipText: "Save (Ctrl+S)",
    size: 20);
```

To show both glyph and label, set the item's `DisplayStyle` to
`ImageAndText` and `TextImageRelation` (e.g. `ImageAboveText`) after configuring,
as the GitScanner console tab does.

## DarkMode & High-DPI notes

- Font glyphs render at any size without bitmap blur — ideal for Per-Monitor-V2
  High-DPI. Use `ConfigureItem(size: …)` or the `ToolStrip.ImageScalingSize` to
  control glyph size.
- Glyph color follows the ToolStrip's `ForeColor` (or the `foreColor` argument),
  so icons track Light/Dark theme automatically. Don't bake a fixed color unless
  you need a specific accent.

## Common gotchas

- **The enums live here, the renderer lives in
  `WarpToolkit.WinForms.Extensions`.** Reference both packages: `FluentSymbols.*`
  from `WarpToolkit.WinForms.Symbols`, `ConfigureItem` from
  `WarpToolkit.WinForms.Extensions.UI`.
- `ConfigureItem` **throws if `toolStripItem.Owner` is null** — add the item to
  its strip/menu before configuring it.
- These are vector glyphs from "Segoe Fluent Icons"; on systems lacking that
  font the glyph may fall back. It ships with Windows 11 and is the standard WARP
  icon source.
- `CommonToolStripSymbols` contains aliases (`New`, `Open`, …) that point at
  other members — that's intentional, not a duplicate.
