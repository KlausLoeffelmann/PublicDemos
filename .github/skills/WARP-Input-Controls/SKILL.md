---
name: warptoolkit-input-controls
description: Guide for WarpToolkit.WinForms input controls — FilePathPicker (file/folder/save picker), BindableComboBox, and the floating-point sliders FloatingPointSlider / FloatingPointTrackBar / EditFloatingPointSlider. Use this when adding a path picker, a binding-aware combo box, or a fractional/decimal slider to a WinForms form.
---

# WarpToolkit.WinForms Input Controls

Binding-friendly input controls from `WarpToolkit.WinForms`. All are
Designer-droppable; configure them in `InitializeComponent` per the
`winforms-designer-code` rules (backing fields, no object initializers/lambdas).

> **Source of truth:** verified against `src/WarpToolkit.WinForms/Controls/` and
> `Experimental/Binding/`.

## When to Use This Skill

- A **file / folder / save-as picker** field with a browse button
  (`FilePathPicker`).
- A `ComboBox` whose selected **value** binds to a view-model property
  (`BindableComboBox`).
- A slider/track bar over a **floating-point** range, optionally with an inline
  edit box (`FloatingPointSlider`, `FloatingPointTrackBar`,
  `EditFloatingPointSlider`).

## FilePathPicker

`FilePathPicker : Control` — namespace `WarpToolkit.WinForms.Controls`. A text
field plus glyph buttons (pick + optional reveal) wired to the appropriate
common dialog. Default `Size` is `300 x 24`.

Key properties (all Designer-serialized with `[DefaultValue]`):

| Property | Type | Default |
|----------|------|---------|
| `FileOrFolderPath` | `string` | `""` |
| `PickerMode` | `FilePathPickerMode` | `OpenFile` |
| `InitialDirectory` | `string` | `""` |
| `Filter` | `string` | `""` |
| `FilterIndex` | `int` | `1` |
| `DialogTitle` | `string` | `""` |
| `DefaultExt` | `string` | `""` |
| `ShowReadOnly` | `bool` | `false` |
| `MultiSelect` | `bool` | `false` |
| `CheckFileExists` | `bool` | `true` |
| `CheckPathExists` | `bool` | `true` |
| `AddExtension` | `bool` | `true` |
| `ButtonWidth` | `int` | `24` |
| `ButtonText` | `string` | `"…"` |
| `PickGlyph` | `string` | `"…"` |
| `RevealGlyph` | `string` | `"↗"` |
| `ShowPickButton` | `bool` | `true` |
| `ShowRevealButton` | `bool` | `false` |
| `AutoFitGlyphs` | `bool` | `true` |
| `ReadOnly` | `bool` | `true` |

Events: a `*Changed` event for nearly every property (`FileOrFolderPathChanged`,
`PickerModeChanged`, `FilterChanged`, `DialogTitleChanged`, `DefaultExtChanged`,
`FilterIndexChanged`, `ShowReadOnlyChanged`, `MultiSelectChanged`,
`CheckFileExistsChanged`, `CheckPathExistsChanged`, `AddExtensionChanged`,
`ButtonTextChanged`, `ReadOnlyChanged`, `PathChanged`) plus
`PickButtonClicked` and `RevealButtonClicked`.

`FilePathPickerMode` enum: `OpenFile`, `SaveFile`, `FolderBrowser`.

```csharp
// MyForm.Designer.cs — inside InitializeComponent (flat, Designer-compliant):
_pathPicker = new FilePathPicker();
_pathPicker.Dock = DockStyle.Top;
_pathPicker.PickerMode = FilePathPickerMode.FolderBrowser;
_pathPicker.DialogTitle = "Select chat folder";
_pathPicker.ShowRevealButton = true;
Controls.Add(_pathPicker);

// MyForm.cs — react to changes in regular code:
_pathPicker.FileOrFolderPathChanged += OnChatFolderChanged;
```

The selected path is read from `FileOrFolderPath` (there is **no** `Path`
property).

## BindableComboBox

`BindableComboBox : ComboBox` — namespace
`WarpToolkit.WinForms.Experimental.Binding`. Adds a bindable *value* on top of
`ComboBox`.

| Member | Signature |
|--------|-----------|
| `SelectedBindingValue` | `object? { get; set; }` `[Bindable(true)]` |
| `BindingValueChanged` | `event EventHandler?` |

When `DataContext` is an `IList` (and not in design mode), the control assigns it
to `DataSource` automatically. Bind `SelectedBindingValue` to a view-model
property for two-way selection:

```csharp
_combo.DataBindings.Add(
    nameof(BindableComboBox.SelectedBindingValue),
    _viewModel,
    nameof(_viewModel.SelectedProvider),
    formattingEnabled: true,
    DataSourceUpdateMode.OnPropertyChanged);
```

(See `warptoolkit-binding-autoview` for the AutoView/factory integration.)

## Floating-point sliders

Three controls in `WarpToolkit.WinForms.Controls` expose **`float`** ranges. They
re-declare `Minimum`/`Maximum`/`Value`/etc. with `new` to shadow the integer
`TrackBar` members.

### FloatingPointSlider : TrackBar
`new float` members: `Minimum` (0), `Maximum` (1), `Value` (0),
`TickFrequency` (0.1), `LargeChange` (0.1), `SmallChange` (0.01).

### FloatingPointTrackBar : TrackBar
`new float` members: `Minimum` (0), `Maximum` (1), `Value` (0),
`SmallChange` (0.1), `LargeChange` (0.25), `TickFrequency` (0.1). The constructor
enables `OptimizedDoubleBuffer | AllPaintingInWmPaint`.

### EditFloatingPointSlider : ContainerControl
Composes a `TextBox` + a `FloatingPointTrackBar` so the user can type or drag.

| Member | Type / default |
|--------|----------------|
| `Value` | `float` (0) |
| `Minimum` | `float` (0) |
| `Maximum` | `float` (1) |
| `SmallChange` | `float` (0.1) |
| `LargeChange` | `float` (0.25) |
| `TickFrequency` | `float` (0.1) |
| `ValueChanged` | `event EventHandler?` |

```csharp
// Designer file — flat configuration:
_opacitySlider = new EditFloatingPointSlider();
_opacitySlider.Minimum = 0f;
_opacitySlider.Maximum = 1f;
_opacitySlider.Value = 0.75f;
_opacitySlider.SmallChange = 0.05f;
Controls.Add(_opacitySlider);

// Regular code:
_opacitySlider.ValueChanged += (s, e) => _layer.Opacity = _opacitySlider.Value;
```

## DarkMode & High-DPI notes

- None of these controls override `AutoScaleMode`; rely on the host's
  `AutoScaleMode = AutoScaleMode.Font`. `FilePathPicker`'s default `300 x 24`
  size scales with the container.
- `FilePathPicker` does not apply DarkMode colors itself; it inherits ambient
  `BackColor`/`ForeColor`. Set those from the theme if needed.

## Common gotchas

- The floating-point controls' `Minimum`/`Maximum`/`Value` are **`new float`**
  shadows. If you reference the control through a `TrackBar` base variable you
  get the **integer** members instead — always use the concrete type.
- `FilePathPicker.ReadOnly` defaults to **`true`** (the text box is read-only;
  users pick via the button). Set it `false` to allow manual typing.
- `BindableComboBox` only auto-assigns `DataSource` from `DataContext` when that
  context is an `IList` and the control is not in design mode.
