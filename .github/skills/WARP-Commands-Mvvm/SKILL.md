---
name: warptoolkit-commands-mvvm
description: Guide for WarpToolkit.WinForms ICommand-style command types — UiCommandBase, FormCommand, MessageBoxCommand, the IUiCommand interface, the CommandTypeProvider extender, and the design-time type converters. Use this when wiring ICommand-style commands to WinForms Buttons/ToolStrip items or exposing commands at design time.
---

# WarpToolkit.WinForms Commands (MVVM)

`ICommand`-style command components for WinForms, from namespace
`WarpToolkit.WinForms.Experimental.CommandTypes`. They let you bind reusable
command objects to buttons / tool-strip items and expose them at design time via
an extender provider.

> **Source of truth:** verified against
> `src/WarpToolkit.WinForms/Experimental/CommandTypes/`. These types live under
> `Experimental` — treat the API as evolving.

## When to Use This Skill

- Wiring an **`ICommand`-style command** to a WinForms `Button`/`ToolStripItem`.
- Providing a built-in command (open a **form**, show a **message box**) without
  writing a handler.
- Exposing commands to the **WinForms Designer** through an extender provider.

## IUiCommand

`public interface IUiCommand : ICommand, INotifyPropertyChanged, IDisposable`.

| Member | Signature |
|--------|-----------|
| `CommandName` | `string { get; set; }` |
| `RaiseCanExecuteChanged()` | `void` |

(Inherits `Execute`, `CanExecute`, `CanExecuteChanged` from `ICommand`.)

## UiCommandBase

`public abstract class UiCommandBase : Component, IUiCommand`. The base for custom
commands; being a `Component` makes it Designer-droppable.

| Member | Signature |
|--------|-----------|
| `CommandName` | `string { get; set; }` |
| `CanExecute` | `virtual bool CanExecute(object? parameter)` |
| `Execute` | `abstract void Execute(object? parameter)` |
| `RaiseCanExecuteChanged()` | `void` |
| `RequestCanExecute` | `event EventHandler<RequestCanExecuteEventArgs>?` |
| `CanExecuteChanged` | `event EventHandler?` |
| `PropertyChanged` | `event PropertyChangedEventHandler?` |

`RequestCanExecuteEventArgs(object? parameter) : EventArgs` exposes
`object? CommandParameter { get; }` and `bool CanExecute { get; set; }` — handle
`RequestCanExecute` and set `e.CanExecute` to drive enablement without
subclassing.

```csharp
public sealed class SaveCommand : UiCommandBase
{
    public override bool CanExecute(object? parameter) => _document.IsDirty;
    public override void Execute(object? parameter) => _document.Save();
}
```

## FormCommand

`public class FormCommand : UiCommandBase`. Opens a form when executed.

| Member | Signature |
|--------|-----------|
| `FormType` | `Type? { get; set; }` |
| `CanExecute` | `override bool` |
| `Execute` | `override void` |

```csharp
FormCommand openOptions = new() { CommandName = "OpenOptions", FormType = typeof(FrmOptions) };
_optionsButton.Click += (s, e) => openOptions.Execute(null);
```

## MessageBoxCommand

`public class MessageBoxCommand : UiCommandBase`. Shows a message box when
executed.

| Member | Signature |
|--------|-----------|
| `MessageText` | `string? { get; set; }` |
| `Caption` | `string? { get; set; }` |
| `Buttons` | `MessageBoxButtons { get; set; }` |
| `Icon` | `MessageBoxIcon { get; set; }` |
| `Execute` | `override void` |

## CommandTypeProvider

`public class CommandTypeProvider : Component, IExtenderProvider`. A Designer
extender that attaches a command (or command *type*) to other components and
wires them up.

| Member | Signature |
|--------|-----------|
| `CanExtend(object extendee)` | `bool` |
| `GetUICommandType` / `SetUICommandType` | `Type? (Component)` / `void (Component, Type?)` |
| `GetUICommand` / `SetUICommand` | `UiCommandBase? (Component)` / `void (Component, UiCommandBase?)` |
| `ShouldSerialize*` / `Reset*` | serialization helpers for the two extended properties |
| `WireUpCommands()` | `void` — connect the assigned commands to their components |

```csharp
// Regular code (e.g. OnLoad), after InitializeComponent:
_commandProvider.SetUICommand(_saveButton, new SaveCommand { CommandName = "Save" });
_commandProvider.WireUpCommands();
```

## Design-time type converters

These `TypeConverter`s power the Designer drop-downs and serialization; you
normally don't call them directly:

- `UiCommandInstanceConverter` — standard-values list of command *instances*.
- `UiCommandTypeConverter` — standard-values list of command *types*.
- `FormsTypeConverter` — standard-values list of `Form` types (used by
  `FormCommand.FormType`).

## DarkMode & High-DPI notes

These are non-visual `Component`s — no DarkMode/DPI concerns of their own. The
controls they target (buttons, tool-strip items) follow the host theme.

## Common gotchas

- `UiCommandBase.Execute` is **abstract** — a custom command must override it;
  `CanExecute` is virtual (defaults to handling `RequestCanExecute`).
- Call `RaiseCanExecuteChanged()` when your command's enablement changes so bound
  controls refresh.
- After assigning commands through `CommandTypeProvider`, call `WireUpCommands()`
  once (in regular code, not `InitializeComponent`).
- These types are under `Experimental` — pin the package version and re-verify
  signatures before relying on them long-term.
