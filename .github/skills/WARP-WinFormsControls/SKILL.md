---
name: warp-winforms-controls
description: Use this skill when adding modern WinForms UI with the WARP-Toolkit — fluent controls (FluentTabControl, FluentMessageBox, WizardContainer, FilePathPicker, BindableComboBox), adorned/grid layouts, MVVM-style commands for WinForms, control-tree helpers, dark-mode for DataGridView, persisting form/grid state, async ShowDialog, and developer tools such as ConsoleControl and FilenameDisambiguator. Bundles WarpToolkit.WinForms, WarpToolkit.WinForms.Extensions, and WarpToolkit.WinForms.Tooling because they are designed to be used together.
---

# WARP WinForms Controls, Components and Helpers

This skill bundles the three packages that, together, modernize the
day-to-day WinForms control surface:

| Package | Role |
|---------|------|
| `WarpToolkit.WinForms` | Flagship controls, adorners, wizards, commands, symbol fonts. |
| `WarpToolkit.WinForms.Extensions` | Static helper methods (control-tree traversal, binding converters, dark-mode helpers, persistence). **Often references types from `WarpToolkit.WinForms`** — keep them paired. |
| `WarpToolkit.WinForms.Tooling` | Developer-flavored helpers: interactive `ConsoleControl`, `FilenameDisambiguator`. |

Current preview version: `0.9.7-preview.g32895b766b`.

Reference files:
- `src/docs/reference/WarpToolkit.WinForms.md`
- `src/docs/reference/WarpToolkit.WinForms.Extensions.md`
- `src/docs/reference/WarpToolkit.WinForms.Tooling.md`

## When to use this skill

- Adding a **modern tab control**, **message box**, **wizard**,
  **file/folder picker**, or **bindable combo** to a form.
- Building a **TableLayoutPanel-style layout with adornments** (borders,
  per-cell padding, error signalling) using `AdornerPanel` /
  `AdornerTableLayoutPanel` and the shared grid-layout primitives.
- Wiring **`ICommand`-style commands** to buttons or tool-strip items via
  `IUiCommand`/`WinFormsCommand`/`FormCommand`.
- Walking the **control tree** to find children, ascendants or roots in a
  strongly-typed way.
- Plugging an **`IValueConverter`** into an existing `Binding`.
- Applying **dark-mode** to a `DataGridView` (system colors alone do not
  cover all grid surfaces).
- **Persisting** form bounds, splitter distances, column widths via
  `IUserSettingsService` extensions.
- Showing a **modal dialog asynchronously** with a typed data context.
- Adding an **interactive in-process console pane** or
  **collision-free filename generation** for save/export features.

## Designer rules apply

This skill is about producing **WinForms Designer-compatible** code.
Everything in `InitializeComponent` must follow the strict Designer rules:

- Backing fields are class-level. No local variables for controls.
- No `if`, `for`, lambdas, collection expressions, ternary, null-coalescing,
  `nameof()`, or custom helper methods inside `InitializeComponent`.
- Repetitive instantiate→configure→add-to-parent code is **correct** for the
  Designer parser. Do not refactor it into helpers.
- Bind event handlers to methods (not lambdas) inside `InitializeComponent`.

Detailed Designer rules and modern-C# rules (which apply only in **regular**
`.cs` files) are in the WinForms Designer skill (`winforms-designer-code`).
This skill assumes those are already followed.

## Flagship controls — `WarpToolkit.WinForms`

### FluentTabControl

A dark-mode-aware tab control with modern visuals. Drop it on a form like a
`TabControl`; bind `SelectedTab` to a view-model property; or use it
directly in code.

### FluentMessageBox

Replacement for `MessageBox` that respects DarkMode and the application font:

```csharp
DialogResult dr = FluentMessageBox.Show(
    owner: this,
    text: "Discard your unsaved edits?",
    caption: "Unsaved changes",
    buttons: MessageBoxButtons.YesNoCancel,
    icon: MessageBoxIcon.Question);
```

Prefer `FluentMessageBox` in WARP apps; reach for `IDialogService` from the
`warp-app-services` skill when the call is in a ViewModel.

### WizardContainer + WizardPage

Multi-step UI with `WizardPage` hosting controls and `WizardContainer`
coordinating navigation. Validate via `WizardPageValidatingEventArgs`, exit
with `WizardDialogResult.Completed` / `Cancelled`. Use
`WizardContainerScalingMode` for HiDPI parity across pages.

### BindableComboBox, FilePathPicker, EditFloatingPointSlider, TransparentPanel

Drag-and-drop binding-friendly building blocks; use them like their stock
counterparts but with strongly-typed data binding and (where applicable)
async data sources.

### Adorners (`WarpToolkit.WinForms.Containers.Adorners`)

Use `AdornerPanel` / `AdornerTableLayoutPanel` together with
`AdornerCellStyle` and the layout primitives from
`WarpToolkit.ComponentModel.GridLayouting` to render a consistent
border / padding / error-signal style across all cells:

```csharp
// In a regular .cs file (NOT InitializeComponent)
_adornerPanel.BorderStyle = AdornerBorderStyle.Modern;
_adornerPanel.BorderThickness = 1;
_adornerPanel.Orientation = LayoutOrientation.Grid;
_adornerPanel.LayoutDefinition = "Auto, *, Auto"; // shared with the DirectX panel
```

`LayoutSizeType.Star` / `Absolute` and `LayoutAlignment.Start/Center/End/Fill`
are the same primitives used elsewhere in WARP, so layout definitions
transfer between GDI+ and Direct2D rendering paths.

### Commands

`IUiCommand` / `UiCommandBase` / `FormCommand` / `MessageBoxCommand` /
`WinFormsCommand` model `ICommand`-style commands for WinForms — bind them
to Buttons or ToolStrip items. `CommandTypeProvider` and
`UiCommandTypeConverter` make commands discoverable at design time.

## Helper extensions — `WarpToolkit.WinForms.Extensions`

### Control-tree traversal

Strongly-typed traversal is the main draw:

```csharp
using WarpToolkit.WinForms.Extensions.UI;

foreach (Button enabled in this.DescendantControls<Button>(b => b.Enabled))
{
    enabled.Visible = false;
}

Form? owner = this.FirstAscendantOrDefault<Form>(f => f.MdiParent is not null);
Control root = this.Root();

this.TraverseAction(c => c.UseWaitCursor = true);
```

Methods come in four flavors — `Ascendant` / `Child` / `Descendant` / `Root`
— each with `First*`, `First*OrDefault`, and `*<T>` typed overloads.
`EnsureNotNull<T>` is the "fail fast at startup" guard for designer-touched
fields that NRT cannot prove non-null.

### Binding converters

Use `WarpToolkit.ComponentModel.IValueConverter` for two-way conversion
without subclassing `Binding`:

```csharp
textBox.AddBindingConverter(nameof(TextBox.Text), new DecimalToCurrencyConverter());
```

For one-off conversions, attach to `Binding.Format` / `Binding.Parse`
manually — the converter pattern wraps that idiom.

### DataGridView dark-mode

```csharp
if (Application.IsDarkModeEnabled)
{
    _grid.ApplyDarkMode();
}
```

`Application.IsDarkModeEnabled` is .NET 9+. Without `ApplyDarkMode`, system
colors will leave the column header and selection backgrounds at default
light colors.

### Form helpers

```csharp
form.CenterToScreen(Screen.PrimaryScreen, horizontalFillGrade: 60, verticalFillGrade: 70);
Rectangle restorable = form.GetRestorableBounds();
DialogResult dr = await form.ShowDialogAsync(viewModel);
```

`ShowDialogAsync<T>` sets the form's `DataContext` to the passed view-model
and awaits closing — the recommended way to wait for modal dialogs from
async code.

### Persistence (`UIServiceExtensions`)

```csharp
settings.SaveFormBounds(form, key: nameof(MainForm));
settings.TryApplyFormBounds(form, key: nameof(MainForm));
settings.SaveDataGridViewColumnWidths(_grid, key: "Grid.Customers");
settings.TryApplyDataGridViewColumnWidths(_grid, key: "Grid.Customers");
settings.SaveSplitterDistance(_split, key: "Split.Main");
settings.TryApplySplitterDistance(_split, key: "Split.Main");
```

`settings` is an `IUserSettingsService` registered by
`AddWinFormsUserSettingsService()` from the AppServices skill.

### Font extensions

`font.GetAscent()`, `GetDescent()`, `GetLeading()`, `GetBaseLine()`,
`GetLineHeight()` — the typography numbers you actually need for custom
painting and aligning controls with text baselines.

### Symbol fonts

`AllSymbols`, `CommonToolStripSymbols`, `DevelopmentSymbols`,
`TreeViewSymbols`, plus `Generic.GetFont(symbol, size)` and the
`ToolStripExtensions.ConfigureItem(...)` overload, render icons from
"Segoe UI Symbol" / "Segoe Fluent Icons". Prefer these over bitmap icon
libraries for crisp scaling and dark-mode parity.

```csharp
toolStripButton.ConfigureItem(
    symbol: CommonToolStripSymbols.Save,
    eventHandler: (OnSaveClick, addHandler: true),
    tooltipText: "Save (Ctrl+S)",
    size: 20);
```

## Developer tools — `WarpToolkit.WinForms.Tooling`

### ConsoleControl

An interactive in-process terminal control. Use it to embed a "Copilot CLI"
or PowerShell pane inside a form:

```csharp
await _console.SetStyleAsync(
    textColor: Color.LightSkyBlue,
    style: CustomFontStyle.Bold,
    size: FontSize.Normal,
    keepSetting: true);

await _console.WriteLineAsync("Running build…");
await _console.RunCommandAsync("dotnet", "build /clp:Summary");
```

Hook `ConsoleOutAsync` / output callbacks to stream output into a
view-model or chat history.

### FilenameDisambiguator

Generates collision-free filenames using a `GenerationStrategy`
(counter, timestamp, etc.) and an optional base path. Use it everywhere you
save files the user might re-export (chat sessions, code-block extractions,
exports):

```csharp
var dis = new FilenameDisambiguator(
    title: "ChatExport",
    basePath: chatBaseFolder,
    extension: ".md",
    generationStrategy: GenerationStrategy.TimestampSuffix,
    requestPathForFile: false);

string fullPath = dis.FullFilename;
```

## Common recipes

### A modal dialog with a typed result

```csharp
// View-model
public sealed class EditCustomerVm
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

// Form code-behind:
public partial class EditCustomerDialog : Form
{
    public EditCustomerDialog() => InitializeComponent();

    private void OnLoad(object? sender, EventArgs e)
    {
        if (DataContext is EditCustomerVm vm)
        {
            _txtFirstName.Text = vm.FirstName;
            _txtLastName.Text  = vm.LastName;
        }
    }
}

// Caller (any other form/view-model):
var vm = new EditCustomerVm { FirstName = "Ada", LastName = "Lovelace" };

using EditCustomerDialog dlg = new();
DialogResult dr = await dlg.ShowDialogAsync(vm);

if (dr == DialogResult.OK)
{
    customer.FirstName = vm.FirstName;
    customer.LastName  = vm.LastName;
}
```

### A "first child of type T matching predicate" lookup

```csharp
TextBox? primary = this.FirstDescendantOrDefault<TextBox>(t => t.Tag is "Primary");
```

### A small wizard

```csharp
private void InitializeWizard()
{
    _wizard.Pages.Add(new WizardPage { Title = "Welcome" });
    _wizard.Pages.Add(new WizardPage { Title = "Account" });
    _wizard.Pages.Add(new WizardPage { Title = "Finish" });

    _wizard.PageValidating += (s, e) => { /* validate, set e.Cancel if not OK */ };
    _wizard.Completed      += (s, e) => SaveAsync().FireAndForget();
}
```

(`FireAndForget` is illustrative — for async event handlers in WARP, see the
async patterns skill / docs.)

## Rules and anti-patterns

- **Never** put helpers, lambdas, control flow, or local variables into
  `InitializeComponent`. That is a serialization format, not arbitrary C#.
- **Never** instantiate adorners or custom controls in `InitializeComponent`
  with complex constructor logic — give them a parameterless constructor and
  configure them in `OnLoad` / via Designer-set properties only.
- **Do not** apply `ApplyDarkMode` to a `DataGridView` unconditionally —
  gate on `Application.IsDarkModeEnabled` so the same code works on
  light-mode systems and < .NET 9.
- **Do not** mix `Margin` and `Padding` in TLP cells. `Margin` on the
  child has the effect you want; `Padding` of the parent cell does not.
- **Do not** call `WinFormsCommand`'s `Execute` from inside a `Click`
  handler — bind it once and let the command machinery dispatch.
- **Do not** use the raw `Application.OpenForms[name]` pattern. Look forms
  up through DI or via `ControlExtensions.FirstAscendant<Form>(...)`.
- **Do not** read forms or controls from the wrong thread. Use the
  `ISyncContextService` or `Control.InvokeAsync` (.NET 9+) for cross-thread
  UI updates.

## Where to look next

- **AI features in the same form** (chat, services, model picker): use the
  `warp-winforms-ai` skill.
- **Hosting / DI / dialog service** consumed by ViewModels: use the
  `warp-app-services` skill.
- **Custom drawing** beyond GDI+: see `WarpToolkit.WinForms.DirectX`
  reference (D2DPanel, D2DGraphics).
- **Markdown / rich-text rendering**: see `WarpToolkit.WinForms.Typography`
  reference.
