---
name: warptoolkit-binding-autoview
description: Guide for WarpToolkit.WinForms AutoView data-binding scaffolding — AutoViewBase<TControl>, AutoViewControlMappingBase, TypeToControlFactoryBase / StringToTextBoxFactory, and BindableComboBox binding. Use this when generating an editor UI from a view-model's properties via type-to-control factories, or mapping property types to controls.
---

# WarpToolkit.WinForms Binding & AutoView

Scaffolding that builds an editor UI from a view-model by mapping each property
**type** to a control **factory**. Types span two namespaces:

| Type | Namespace |
|------|-----------|
| `AutoViewBase<TControl>` | `WarpToolkit.WinForms.Extensions.AutoView` |
| `AutoViewControlMappingBase`, `TypeToControlFactoryBase`, `StringToTextBoxFactory` | `WarpToolkit.WinForms.Containers` |
| `BindableComboBox`, `ValueNotFoundException` | `WarpToolkit.WinForms.Experimental.Binding` |

> **Source of truth:** verified against
> `src/WarpToolkit.WinForms/Containers/AutoView/` and `Experimental/Binding/`.

## When to Use This Skill

- **Generating an editor view** from a view-model's properties (one control per
  property) instead of hand-laying-out a form.
- Defining how a **property type maps to a control** via a factory.
- Binding a `ComboBox` **value** (not just display) to a view-model property.

## AutoViewBase&lt;TControl&gt;

`public class AutoViewBase<TControl> where TControl : ContainerControl, new()`.
Wraps a generated `TControl` host bound to an `INotifyPropertyChanged`
view-model. It is **not** itself a control — it owns one via `Control`.

| Member | Signature |
|--------|-----------|
| ctor | `AutoViewBase(INotifyPropertyChanged viewModel)` |
| `Control` | `TControl { get; private set; }` |
| `ViewModel` | `INotifyPropertyChanged { get; set; }` |

```csharp
using WarpToolkit.WinForms.Extensions.AutoView;

var view = new AutoViewBase<TableLayoutPanel>(myViewModel);
hostPanel.Controls.Add(view.Control);
```

## AutoViewControlMappingBase

`public class AutoViewControlMappingBase`. Holds the property-type → control-
factory mapping used to build the view.

| Member | Signature |
|--------|-----------|
| `AddMapping<TProperty, TControlFactory>()` | `void` where `TControlFactory : TypeToControlFactoryBase, new()` |
| `DefaultMapping` | `Dictionary<Type, Type> { get; }` |

```csharp
var mapping = new AutoViewControlMappingBase();
mapping.AddMapping<string, StringToTextBoxFactory>();
```

## TypeToControlFactoryBase

`public abstract class TypeToControlFactoryBase`. Produces the control(s)
(and optional caption/layout) for one property.

Constructors:
```csharp
protected TypeToControlFactoryBase(PropertyInfo propertyInfo);
protected TypeToControlFactoryBase(string propertyName, string? displayName = default, string? category = default, string? description = default);
```

| Member | Signature |
|--------|-----------|
| `Category` / `DefaultValue` / `Description` / `DisplayName` | `string? { get; }` |
| `HasCaption` / `HasLayout` / `RequestStackedLayout` | `bool { get; }` |
| `GetCaption()` | `virtual IEnumerable<Control>` |
| `GetControls()` | `virtual IEnumerable<Control>?` |
| `GetControlLayout()` | `virtual TableLayoutPanel?` |
| `GetCaptionAndControlLayout()` | `virtual TableLayoutPanel?` |

### StringToTextBoxFactory

`public class StringToTextBoxFactory : TypeToControlFactoryBase`. The built-in
factory that maps a `string` property to a `TextBox`.

```csharp
public StringToTextBoxFactory(string propertyName, string? displayName = null, string? category = null, string? description = null);
public override IEnumerable<Control> GetControls();   // yields the TextBox
```

Write your own factory by subclassing `TypeToControlFactoryBase` and overriding
`GetControls()` (and `GetCaption()`/layout members as needed).

## BindableComboBox (binding usage)

`public class BindableComboBox : ComboBox` (namespace
`WarpToolkit.WinForms.Experimental.Binding`). For value binding in an AutoView /
data-bound form:

| Member | Signature |
|--------|-----------|
| `SelectedBindingValue` | `object? { get; set; }` `[Bindable(true)]` |
| `BindingValueChanged` | `event EventHandler?` |

When `DataContext` is an `IList` (and not design mode) it is assigned to
`DataSource` automatically. Bind `SelectedBindingValue` to the property:

```csharp
_combo.DataBindings.Add(
    nameof(BindableComboBox.SelectedBindingValue),
    viewModel,
    nameof(viewModel.SelectedItem),
    formattingEnabled: true,
    DataSourceUpdateMode.OnPropertyChanged);
```

`ValueNotFoundException : Exception` is thrown by the binding machinery when a
bound value cannot be resolved against the list — catch it where you set values
programmatically.

## DarkMode & High-DPI notes

- The generated host (`AutoViewBase.Control`) is a `ContainerControl`; place it
  in a form/UserControl with `AutoScaleMode = AutoScaleMode.Font`. Factories that
  return `TableLayoutPanel` layouts scale per `winforms-high-dpi-fluent-layout`.
- Controls produced by factories inherit ambient theme colors; no special
  DarkMode handling is built into the factories.

## Common gotchas

- `TypeToControlFactoryBase` has **only protected constructors** — instantiate
  through a concrete subclass (e.g. `StringToTextBoxFactory`) and register it via
  `AddMapping<TProperty, TFactory>()`.
- `AutoViewBase<TControl>` is a **wrapper, not a control** — add
  `view.Control` to the form, not the `AutoViewBase` instance.
- Catch `ValueNotFoundException` when assigning `SelectedBindingValue` to a value
  that may not be present in the bound list.
- These AutoView/Binding types live under `Extensions.AutoView` /
  `Experimental.Binding` and the factory bases under `Containers` — mind the
  three different `using` namespaces.
