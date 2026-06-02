---
name: warptoolkit-containers-layout
description: Guide for WarpToolkit.WinForms container and layout controls — FluentTabControl, AdornerPanel, AdornerTableLayoutPanel, and TransparentPanel — including adorned grid layouts, per-cell styles, and click-through transparency. Use this when building modern tabbed UI, decorating a TableLayoutPanel with borders/error signalling, or overlaying a transparent/pass-through panel.
---

# WarpToolkit.WinForms Containers & Layout

Container and layout controls from `WarpToolkit.WinForms`. These are
**Designer-droppable** controls; everything in `InitializeComponent` must follow
the strict Designer rules (see the `winforms-designer-code` skill): backing
fields, no helpers/lambdas/object-initializers/control-flow.

> **Source of truth:** signatures below were verified against the WARP repo
> source in `src/WarpToolkit.WinForms/Containers/`. Document only what exists.

## When to Use This Skill

- Adding a modern, dark-mode-aware **tab control** (`FluentTabControl`).
- Decorating a panel/grid with consistent **borders, padding and error
  signalling** (`AdornerPanel`, `AdornerTableLayoutPanel`, `AdornerCellStyle`).
- Overlaying a **transparent / click-through** region on top of other controls
  (`TransparentPanel`).

## FluentTabControl

`FluentTabControl : Panel` — namespace `WarpToolkit.WinForms.Containers`.
A modern tab control whose header strip is a `MenuStrip` rendered with rounded
corners (dark/light selected by `Application.IsDarkModeEnabled`).

| Member | Signature |
|--------|-----------|
| ctor | `FluentTabControl()` |
| `AddTab` | `void AddTab(string tabPageTitle, ContainerControl tabContent)` |
| `CurrentTabIndex` | `int { get; set; }` |
| `CurrentTab` | `Control? { get; }` |
| `Tabs` | `IEnumerable<Panel> { get; }` (the page hosts) |
| `TabChanged` | `event EventHandler?` |

Critical detail: `AddTab` takes a **`ContainerControl`** (intended: one
`UserControl` per tab). Internally it wraps the content in a `Panel` page,
`Dock = Fill`s it, and adds a header menu item. There is no `TabPage` type.

```csharp
// Regular code (NOT InitializeComponent) — runtime wiring after construction.
protected override void OnLoad(EventArgs e)
{
    base.OnLoad(e);
    _fluentTab.AddTab("Chat", _chatView);          // _chatView : UserControl
    _fluentTab.AddTab("Settings", _settingsView);  // _settingsView : UserControl
}
```

`OnFontChanged` bumps the header font by 1pt automatically. For the full
sizing/scrolling rules of tab content, see the dedicated
`warp-fluent-tab-control` skill.

**Designer:** `InitializeComponent` may instantiate the field, set `Dock`,
`Size`, `Anchor`, and add it to `Controls`. **Never call `AddTab` from
`InitializeComponent`** — it adds runtime state the Designer parser cannot
round-trip.

## AdornerPanel

`AdornerPanel : Panel, ILayoutDefinitionProvider` — namespace
`WarpToolkit.WinForms.Containers.Adorners`. A panel that paints a border/padding
"adornment" around its content and can flash an error signal. The constructor
calls `SetStyle(OptimizedDoubleBuffer | ResizeRedraw, true)`.

| Member | Signature / notes |
|--------|-------------------|
| `LayoutDefinition` | `string { get; set; }` (e.g. `"Auto, *, Auto"`) |
| `ControlPadding` | `int { get; set; }` (default 10) |
| `BorderStyle` | `new AdornerBorderStyle { get; set; }` (default `Modern`) |
| `BorderColor` | `Color { get; set; }` |
| `BorderThickness` | `int { get; set; }` (default 1) |
| `Orientation` | `Orientation { get; set; }` |
| `Padding` | `new Padding { get; set; }` |
| `ToolStripVisibilitySize` | `int { get; set; }` — `> 15` materializes the optional `ToolStrip` |
| `ToolStrip` | `ToolStrip? { get; }` |
| `ErrorSignalingDuration` | `int { get; set; }` |
| `SignalError()` / `SignalErrorAsync()` / `ResetSignal()` | flash/clear the error border |
| events | `LayoutDefinitionChanged`, `ControlPaddingChanged`, `BorderStyleChanged`, `BorderThicknessChanged`, `OrientationChanged`, `ErrorSignalingDurationChanged`, `ToolStripVisibilitySizeChanged` |

`AdornerBorderStyle` enum: `None = 0`, `Classic = 1`, `Modern = 2`.

```csharp
// Regular code, not InitializeComponent.
_adornerPanel.BorderStyle = AdornerBorderStyle.Modern;
_adornerPanel.BorderThickness = 1;
_adornerPanel.ControlPadding = 8;

// Flash the border when validation fails:
await _adornerPanel.SignalErrorAsync();
```

## AdornerTableLayoutPanel

`AdornerTableLayoutPanel : TableLayoutPanel, ISupportInitialize` — same
namespace. A `TableLayoutPanel` that paints a consistent **per-cell** adornment
and can auto-apply margin/anchoring/etc. to hosted controls based on a default
style chosen by the control type. The constructor calls
`SetStyle(OptimizedDoubleBuffer | AllPaintingInWmPaint | UserPaint | ResizeRedraw, true)`.

| Member | Signature |
|--------|-----------|
| `AutoApplyControlAdjustments` | `bool { get; set; }` |
| `DefaultStyle` | `AdornerCellStyle { get; set; }` |
| `DefaultTextBoxStyle` / `DefaultMultilineTextBoxStyle` | `AdornerCellStyle { get; set; }` |
| `DefaultLabelStyle` / `DefaultAutoSizeLabelStyle` | `AdornerCellStyle { get; set; }` |
| `DefaultComboBoxStyle` / `DefaultCheckBoxStyle` | `AdornerCellStyle { get; set; }` |
| `DefaultButtonStyle` / `DefaultPictureBoxStyle` | `AdornerCellStyle { get; set; }` |
| `BeginInit()` / `EndInit()` | `ISupportInitialize` |
| `CreateDefaultStyleInstance` | `event EventHandler<CreateDefaultStyleInstanceEventArgs>?` — override the chosen style per control |

`CreateDefaultStyleInstanceEventArgs` exposes `Control Control { get; }` and
`AdornerCellStyle? Style { get; set; }` (set it to override).

### AdornerCellStyle

`public record class AdornerCellStyle(AutoPropertyApplySettings PropertyApplySettings, AdornerBorderStyle CellBorderStyle, AnchorStyles CellContentAnchoring, Padding? AdornerCellPadding = default, Padding? AdornerCellMargin = default, Color CellBorderColor = default)`.
Public mutable properties: `AdornerCellMargin`, `AdornerCellPadding`,
`AdornerLineWidth` (default 2), `AutoApplyControlProperties`, `CellBorderStyle`,
`CellBorderColor`, `CellContentAnchoring`. The default border color is
DarkMode-aware (`Color.Blue` in dark, `Color.DarkBlue` in light).

`AutoPropertyApplySettings` is a `[Flags]` enum:
`None = 0, Margin = 1, Anchoring = 2, AutoSize = 4, Borderstyle = 8, RenderStyle = 16`.

```csharp
// Customize a single control's adornment style at add-time:
_adornerTable.CreateDefaultStyleInstance += OnCreateDefaultStyle;

private void OnCreateDefaultStyle(object? sender, CreateDefaultStyleInstanceEventArgs e)
{
    if (e.Control is TextBox)
    {
        e.Style = new AdornerCellStyle(
            AutoPropertyApplySettings.Margin | AutoPropertyApplySettings.Anchoring,
            AdornerBorderStyle.Modern,
            AnchorStyles.Left | AnchorStyles.Right);
    }
}
```

## TransparentPanel

`TransparentPanel : Control` — namespace `WinForms.PowerTools.Controls` (note the
distinct namespace). A panel that paints nothing over its parent and can be
configured for click-through, a modal "sneeze guard", or interactive overlay.

| Member | Signature |
|--------|-----------|
| ctor | `TransparentPanel()` — calls `SetStyle(ControlStyles.Opaque, true)` |
| `TransparencyStyle` | `TransparencyStyle { get; set; }` |

`TransparencyStyle` enum: `PassThrough = 0`, `SneezeGuard = 1`, `Interactive = 2`.

Behavior (from overridden `CreateParams`): removes `WS_CLIPCHILDREN/SIBLINGS`;
adds `WS_EX_TRANSPARENT` unless `Interactive`; adds `WS_DISABLED` for
`SneezeGuard`/`PassThrough`.

## DarkMode & High-DPI notes

- `FluentTabControl`'s header renderer and `AdornerCellStyle`'s default border
  color react to `Application.IsDarkModeEnabled` (.NET 9+) automatically — do
  not hard-code colors.
- These controls do not override `AutoScaleMode`. Host them inside a
  `UserControl`/`Form` with `AutoScaleMode = AutoScaleMode.Font` and let layout
  scale per the `winforms-high-dpi-fluent-layout` guidance.

## Common gotchas

- **`PassThrough` must stay the enum zero value** for `TransparencyStyle`:
  `CreateParams` can be queried before field initializers run, so the default
  (0) is read before any field is set. Do not reorder the enum.
- Changing `TransparentPanel.TransparencyStyle` **recreates the window handle**
  at runtime — avoid toggling it on a hot path.
- `AdornerPanel.BorderStyle` and `Padding` are declared `new` (they shadow
  `Panel` members with the adorner-aware versions). Reference them through the
  `AdornerPanel` type, not a `Panel` base reference.
- `AdornerTableLayoutPanel` paints with `UserPaint` — keep heavy work out of
  `OnCellPaint`/`OnPaintBackground`.
