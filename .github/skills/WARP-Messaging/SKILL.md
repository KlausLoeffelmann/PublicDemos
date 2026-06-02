---
name: warptoolkit-messaging
description: Guide for WarpToolkit.WinForms FluentMessageBox (a DarkMode- and app-font-aware MessageBox replacement) and the underlying FluentMessageBoxForm. Use this when showing a themed message/confirmation dialog in a WARP WinForms app instead of System.Windows.Forms.MessageBox.
---

# WarpToolkit.WinForms Messaging

`FluentMessageBox` is a drop-in replacement for `System.Windows.Forms.MessageBox`
that respects DarkMode and the application font. Both types live in namespace
`WarpToolkit.WinForms`.

> **Source of truth:** verified against
> `src/WarpToolkit.WinForms/FluentMessageBox/`.

## When to Use This Skill

- Showing a **themed** information / confirmation / error dialog in a WARP app.
- Replacing a `MessageBox.Show(...)` call so it matches Dark/Light mode and the
  app font.

## FluentMessageBox (static)

`public static class FluentMessageBox`. Overloads (note: **no `owner`
parameter** — unlike `MessageBox`):

```csharp
DialogResult Show(string text, string caption);
DialogResult Show(string text, string caption, MessageBoxButtons buttons);
DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
```

```csharp
using WarpToolkit.WinForms;

DialogResult dr = FluentMessageBox.Show(
    "Discard your unsaved edits?",
    "Unsaved changes",
    MessageBoxButtons.YesNoCancel,
    MessageBoxIcon.Question);

if (dr == DialogResult.Yes)
{
    DiscardEdits();
}
```

Prefer `FluentMessageBox` over `MessageBox` in WARP UIs. When the call originates
in a ViewModel, route it through `IDialogService` (see the `warp-app-services` /
`winforms-mvvm` skills) instead of calling the static directly.

## FluentMessageBoxForm

`public class FluentMessageBoxForm : Form` — the dialog window behind
`FluentMessageBox`. You rarely need it directly; construct one only if you must
host or customize the message dialog yourself.

```csharp
FluentMessageBoxForm(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon);
FluentMessageBoxForm(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton);
```

```csharp
using FluentMessageBoxForm dialog = new(
    "File saved.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
dialog.ShowDialog(this);
```

It is a normal `Form`, so it works with WARP's async dialog helpers
(`ShowDialogAsync`) and standard `ShowDialog(owner)`.

## DarkMode & High-DPI notes

- The dialog automatically honors `Application.IsDarkModeEnabled` and the
  application default font — do not theme it manually.
- Because it is built as a regular `Form`, it scales under Per-Monitor-V2 DPI
  like any WARP form; no special handling needed.

## Common gotchas

- The **argument order is `(text, caption, …)`** and there is **no owner
  overload**. Don't copy a `MessageBox.Show(owner, text, …)` call shape — drop
  the owner and keep text first.
- For ViewModel code, don't call the static `FluentMessageBox` directly; depend
  on a dialog service so the logic stays testable.
