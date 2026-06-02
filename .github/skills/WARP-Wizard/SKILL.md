---
name: warptoolkit-wizard
description: Guide for WarpToolkit.WinForms WizardContainer / WizardPage multi-step UI, including page navigation, per-page entering/leaving/validating events, DPI-aware ShowWizardDialogAsync, and the wizard result/scaling enums. Use this when building a multi-page wizard dialog with validation and Back/Next navigation in WinForms.
---

# WarpToolkit.WinForms Wizard

Multi-step wizard UI from `WarpToolkit.WinForms.Containers.Wizard`. A
`WizardContainer` coordinates an ordered set of `WizardPage`s, each hosting a
single content `Control` (typically a `UserControl`).

> **Source of truth:** verified against
> `src/WarpToolkit.WinForms/Containers/Wizard/`. Real usage:
> `src/Chatty/Views/FirstStartWizard/FirstStartWizardHost.cs`.

## When to Use This Skill

- Building a **multi-step dialog** with Back/Next/Finish navigation.
- Needing **per-page validation** that can block forward navigation.
- Showing a wizard **modally and asynchronously** and awaiting its result.

## WizardContainer

`WizardContainer : ContainerControl`. The constructor builds title/content/button
panels via `InitializeComponent()` and subscribes to `Pages` collection changes.

| Member | Signature |
|--------|-----------|
| `AddWizardPage` | `WizardPage AddWizardPage(string title, Control content)` |
| `RemoveWizardPage` | `bool RemoveWizardPage(WizardPage page)` |
| `Pages` | `ObservableCollection<WizardPage> { get; }` |
| `CurrentPage` | `WizardPage? { get; }` |
| `CurrentPageIndex` | `int { get; private set; }` |
| `PageCount` | `int { get; }` |
| `ScalingMode` | `WizardContainerScalingMode { get; set; }` |
| `WizardPageTitleFont` | `Font { get; set; }` |
| `ShowWizardDialogAsync` | `Task<WizardDialogResult> ShowWizardDialogAsync(IWin32Window? owner = null)` |
| `ShowWizardDialogAsync` | `Task<WizardDialogResult> ShowWizardDialogAsync(IWin32Window? owner, Size? dialogSize, Size? minimumSize = null)` |
| `GoBackAsync` / `GoNextAsync` | `Task<bool>` |
| `GoToPageAsync` | `Task<bool> GoToPageAsync(int pageIndex)` |
| `CanGoBack()` / `CanGoNext()` | `bool` |
| events | `CurrentPageChanged`, `PageCountChanged`, `PageTitleChanged`, `PageTitleFontChanged`, `WizardFinished`, `WizardCancelled`, `WizardScalingModeChanged` |

### Enums

- `WizardContainerScalingMode`: `Automatic = 0`, `Normal = 1`, `Large = 2`.
- `WizardDialogResult`: `Finished = 0`, `Cancelled = 1`, `None = 2`.
- `WizardNavigationDirection`: `Forward = 0`, `Backward = 1`.

## WizardPage

`WizardPage` — created **only** through `WizardContainer.AddWizardPage(...)`
(its constructor is `internal`; do not `new` it). Add the page, then wire its
per-page events.

| Member | Signature |
|--------|-----------|
| `Title` | `string { get; set; }` |
| `Content` | `Control { get; init; }` |
| `NeedsSkipping` | `bool { get; set; }` |
| `ParentContainer` | `WizardContainer { get; }` |
| `PageIndex` | `int { get; }` |
| `IsFirstPage` / `IsLastPage` | `bool { get; }` |
| `PageEntering` | `event EventHandler<WizardPageEventArgs>?` |
| `PageEntered` | `event EventHandler?` |
| `PageLeaving` | `event EventHandler<WizardPageLeavingEventArgs>?` |
| `PageLeft` | `event EventHandler?` |
| `PageValidating` | `event EventHandler<WizardPageValidatingEventArgs>?` |

Event args:
- `WizardPageLeavingEventArgs : CancelEventArgs` — `WizardNavigationDirection Direction { get; }`. Set `e.Cancel = true` to block leaving.
- `WizardPageValidatingEventArgs : CancelEventArgs` — `string? ErrorMessage { get; set; }`. Set `e.Cancel = true` (and optionally `ErrorMessage`) to block forward navigation.
- `WizardPageEventArgs : EventArgs` — passed to `PageEntering`.

## Usage (regular code — not InitializeComponent)

```csharp
using WarpToolkit.WinForms.Containers.Wizard;

WizardContainer wizard = new();

WizardPage page1 = wizard.AddWizardPage("Welcome", _welcomePage);
WizardPage page2 = wizard.AddWizardPage("Select AI Provider", _selectProviderPage);
WizardPage page3 = wizard.AddWizardPage("Enter API Key", _enterKeyPage);

// Per-page validation blocks Next when the page is invalid.
page2.PageValidating += (sender, e) =>
{
    e.Cancel = !_selectProviderPage.ValidateSelection();
};

WizardDialogResult result = await wizard.ShowWizardDialogAsync(owner);
if (result == WizardDialogResult.Finished)
{
    Commit();
}
```

The content controls (`_welcomePage`, etc.) are `UserControl`s — keep their
own `InitializeComponent` Designer-compliant; the wizard hosts them docked.

## DarkMode & High-DPI notes

- `ShowWizardDialogAsync` is **DPI-aware**: it scales the dialog using
  `DeviceDpi / 96f` and computes an optimal size. `ScalingMode.Automatic` picks
  larger button sizes when `DeviceDpi > 120`.
- `WizardPageTitleFont` is derived as a bold variant of the control font when
  left at its default; only override it if you need a specific title font.

## Common gotchas

- **Do not `new WizardPage(...)`** — the constructor is internal. Always go
  through `AddWizardPage`. (Older docs that show `Pages.Add(new WizardPage {...})`
  are outdated.)
- `WizardDialogResult` is **`Finished` / `Cancelled` / `None`** — there is no
  `Completed` member.
- Block navigation with `e.Cancel = true` in `PageValidating` /
  `PageLeaving`; returning without setting `Cancel` allows the move.
- `ShowWizardDialogAsync` is `async` — `await` it; do not call `.Result`/`.Wait()`
  on the UI thread.
