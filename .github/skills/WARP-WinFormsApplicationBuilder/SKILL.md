---
name: warp-winforms-application-builder
description: >-
  Use this skill when you need to bootstrap or retrofit a WinForms app with
  WARP's IHost-based WinFormsApplication / WinFormsApplicationBuilder. It
  covers preparing Forms and UserControls to act as IServiceProvider façades
  for designer-droppable components, the canonical two-constructor pattern,
  and why service resolution must be deferred until after InitializeComponent.
---

# WARP WinFormsApplicationBuilder

`WinFormsApplication` + `WinFormsApplicationBuilder` (namespace
`Microsoft.Extensions.WinForms`, ship in `WarpToolkit.WinForms.AppServices`)
turn a classic WinForms program into a hosted application that uses the
.NET Generic Host's DI, configuration, logging and lifetime machinery — the
same pattern ASP.NET Core uses.

## When to use this skill

Basic tenet: Use this skill, when the user has explicitly instructed before to use or introduce the WarpToolkit library for a specific project.

This skill exists because:

1. **Retrofitting an existing app** to use this builder is the common case.
   Most WARP customers start with a working `Application.Run(new Form1())`
   program and need to move incrementally without breaking their forms.
2. **DI is not intrinsically compatible with the WinForms Designer.** WARP
   solves the impedance mismatch with a Designer-SDK serializer
   (`ServiceProviderAssignableComponentCodeDomSerializer`) plus a base class
   (`BindableServiceProviderComponent`) and an interface
   (`IServiceProviderAssignable`). Forms have to be **prepared** for this
   pattern, otherwise components like `AIServicesComponent`, `ChatView`,
   or any custom `BindableServiceProviderComponent`-derived component will
   not get a service provider.
3. **WebView2-based features require DI.** The chat surface
   (`WarpToolkit.WinForms.Chat.ChatView`) is built on WebView2 and is
   acquired from the service provider — using it without the
   hosted-app model is not supported.

Use this skill whenever the user wants to:

- Bootstrap a new WARP app.
- Retrofit `Application.Run(new MainForm())` to `WinFormsApplication`.
- Prepare a Form to host components that need an `IServiceProvider`.
- Diagnose "service provider is null" / `NullServiceProvider` errors.

## The 30-second mental model

```
Program.cs
  WinFormsApplication.CreateBuilder(args)
    .UseStartupForm<MainForm>()             ← TryAddScoped<MainForm>()
    .Services.AddWinFormsXxx() etc.
  .Build()
  .Run();

  ▼  app start

WinFormsApplication.Run()
  startupForm = _host.Services.GetRequiredService<MainForm>();
  Application.Run(startupForm);

  ▼  DI calls the *parameterized ctor*

MainForm(IServiceProvider serviceProvider) : this()
{
    // 1. `: this()` runs FIRST — the parameterless ctor executes
    //    InitializeComponent(). The Designer-emitted
    //    `comp.SetServiceProvider(this)` calls store a reference to
    //    `this` (the Form). They MUST NOT resolve services synchronously
    //    at that point — _serviceProvider is still null. Deferral is the
    //    component's responsibility (ISupportInitialize / OnLoad).
    //
    // 2. Now the parameterized ctor body runs:
    ArgumentNullException.ThrowIfNull(serviceProvider);
    _serviceProvider = new DeferredServiceProvider(serviceProvider);
    //    DeferredServiceProvider holds a `Func<IServiceProvider>` closure
    //    over the real provider. Components that captured `this` during
    //    InitializeComponent can now safely call GetService — the Form's
    //    explicit IServiceProvider.GetService forwards into the deferred
    //    wrapper, which resolves against the real provider.
}
```

There is no separate "service provider factory" object — **the Form
itself** is the factory façade. The Form implements `IServiceProvider`
explicitly and forwards every `GetService(Type)` call to the
`DeferredServiceProvider` stored in a field.

Why not assign before `InitializeComponent`? Because the Designer
requires a parameter-less constructor, and the Designer's serializer
emits `InitializeComponent` as the canonical initialization entry
point. By making the parameterized ctor chain `: this()`, both code
paths run the *same* `InitializeComponent` and the Form ends up in a
consistent state regardless of how it was constructed. The
`DeferredServiceProvider` is what makes this safe: by the time any
component actually *calls* `GetService`, the wrapper is in place.

## Bootstrap pattern (canonical Program.cs)

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WinForms;
using WarpToolkit.WinForms.AppServices.ServiceExtensions;

namespace ContosoLob;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = WinFormsApplication.CreateBuilder(args);

        builder
            .UseColorMode(SystemColorMode.System)
            .UseHighDpiMode(HighDpiMode.SystemAware)
            .UseVisualStyles(true)
            .UseTextRenderingV2(true)
            .UseStartupForm<MainForm>();          // registers MainForm scoped

        builder.Services
            .AddWinFormsDialogService()
            .AddWinFormsExceptionService()
            .AddWinFormsUserSettingsService()
            .AddLocalKeyRetrievalService();

        // Domain services:
        builder.Services.AddSingleton<ICustomerRepository, SqlCustomerRepository>();
        builder.Services.AddScoped<MainViewModel>();

        builder.Build().Run();
    }
}
```

Calling `UseStartupForm<TForm>()` does `Services.TryAddScoped<TForm>();`.
Any other Form / UserControl you want DI-resolved must be registered the
same way:

```csharp
builder.Services.TryAddScoped<EditCustomerDialog>();
builder.Services.TryAddScoped<ChatView>();          // brings in WebView2
builder.Services.TryAddScoped<SettingsView>();      // tab UserControl
```

## Form preparation — the part you must not skip

Every Form (and every UserControl that hosts
`BindableServiceProviderComponent`-derived components) must be turned
into an `IServiceProvider` façade.

### Required shape

The canonical pattern lives in the **code-behind / Designer file** of
the Form or UserControl, because the explicit `IServiceProvider`
implementation and the nested `DeferredServiceProvider` need to live
right next to the constructors and the Designer-owned `components`
container. The Designer-generated parameter-less constructor stays in
the regular `*.cs` file (or wherever it currently is) and is left
untouched.

```csharp
// MainForm.Designer.cs   (Designer-owned code-behind)
using WarpToolkit.ComponentModel;

namespace ContosoLob;

partial class MainForm
{
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null;

    private IServiceProvider? _serviceProvider;

    /// <summary>
    ///  DI constructor. Called by <see cref="WinFormsApplication"/>.
    /// </summary>
    /// <remarks>
    ///  Chains <c>: this()</c> to the parameter-less constructor so the
    ///  Designer can still instantiate the Form without a service provider.
    ///  The supplied <paramref name="serviceProvider"/> is wrapped in a
    ///  <c>DeferredServiceProvider</c> and assigned to the backing field —
    ///  this lets the Form act as a valid IServiceProvider façade even
    ///  though the field is only populated AFTER <c>InitializeComponent</c>
    ///  has finished running.
    /// </remarks>
    public MainForm(IServiceProvider serviceProvider) : this()
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = new DeferredServiceProvider(serviceProvider);
    }

    // Explicit IServiceProvider implementation forwards every lookup.
    object IServiceProvider.GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (_serviceProvider is null)
        {
            throw new InvalidOperationException(
                "MainForm was constructed without a DI service provider. Resolve it from WinFormsApplication instead of calling new MainForm().");
        }

        return _serviceProvider.GetService(serviceType)
            ?? throw new InvalidOperationException(
                $"Service of type '{serviceType.Name}' is not registered.");
    }

#pragma warning disable WFOWARP9901 // Code-behind files should only contain InitializeComponent, Dispose, and constructors
    private class DeferredServiceProvider : IServiceProvider
    {
        private readonly Func<IServiceProvider> _serviceProviderFactory;

        public DeferredServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProviderFactory = () => serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return _serviceProviderFactory().GetService(serviceType)
                ?? throw new InvalidOperationException(
                    $"Service of type '{serviceType.Name}' is not registered.");
        }
    }
#pragma warning restore WFOWARP9901

    protected override void Dispose(bool disposing) { /* … */ }

    #region Windows Form Designer generated code
    private void InitializeComponent() { /* Designer-owned */ }
    #endregion
}
```

```csharp
// MainForm.cs   (regular code-behind — Designer never touches this)
namespace ContosoLob;

public partial class MainForm : Form, IServiceProvider
{
    /// <summary>
    ///  Designer-only constructor. The WinForms Designer instantiates the
    ///  Form via this parameter-less ctor and never calls into services.
    ///  It is also the constructor the DI ctor chains into via `: this()`.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // First safe point for service-touching work — see deferral section.
    }
}
```

Key rules baked into this shape:

- **`_serviceProvider` is nullable and only assigned in the DI
  constructor.** The parameter-less Designer path intentionally leaves it
  `null`, and the Form should not try to use it before the DI ctor has
  run. The runtime path always goes through the parameterized ctor.
- **The parameterized ctor chains `: this()`** so `InitializeComponent`
  runs exactly once, in the same way the Designer would have run it.
- **The real provider is wrapped in `DeferredServiceProvider`.** The
  wrapper holds a closure over the provider; nothing about the
  wrapper is unique per call.
- **The explicit `IServiceProvider.GetService` guards the field.** If a
  component calls `GetService` during `InitializeComponent` (i.e. before
  the parameterized ctor body has assigned the field), that is a bug in
  the component — it failed to defer. The explicit implementation should
  throw a clear `InvalidOperationException` instead of letting the form
  act like a partially initialized façade.
- **The `#pragma warning disable WFOWARP9901`** silences the
  analyzer that normally restricts the code-behind file to
  `InitializeComponent`, `Dispose`, and constructors. The nested
  `DeferredServiceProvider` is an intentional exception.

### What the Designer emits

For every component on the form whose type derives from
`BindableServiceProviderComponent` (or otherwise carries
`[DesignerSerializer("WarpToolkit.WinForms.Design.ServiceProviderAssignableComponentCodeDomSerializer", …)]`),
the Designer generates a call like this **inside** `InitializeComponent`:

```csharp
// Designer-generated:
_openAIMetaDataService.SetServiceProvider(this);
_openAIUserDialogService.SetServiceProvider(this);
```

This works because:

- `this` is the Form, which implements `IServiceProvider`.
- The component's `SetServiceProvider` implementation only **stores**
  the reference to `this`. It must not synchronously call back into
  `GetService` — at that moment the Form's `_serviceProvider` field
  has not been assigned yet (the parameterless ctor that
  `InitializeComponent` runs under has no chance to set it). See the
  "too-early trap" section below for the deferral pattern.
- Once the parameterized ctor body finishes, the Form's field points
  at a `DeferredServiceProvider` wrapping the real provider, so the
  reference the component captured during `InitializeComponent`
  becomes fully usable from `EndInit` / `OnLoad` onward.

At **design time**, `BindableServiceProviderComponent.EnsurePlausibleServiceProvider`
detects design-mode and substitutes a `NullServiceProvider` so the designer
surface never blows up.

### What you do **not** need to do

- **No** "set provider after construction" plumbing — the Designer wires
  it up for you.
- **No** call to `services.AddSingleton(_serviceProvider)` from inside
  the Form — the host's `ServiceProvider` is already passed in.
- **No** custom factory class — the Form itself is the factory.

## Retrofitting an existing app

To migrate an existing `Application.Run(new Form1())` codebase:

1. **Add the NuGets** (`0.9.86-preview.g8bc92e2168`):
   `WarpToolkit.WinForms.AppServices` (transitively pulls
   `Desktop.AppServices`, `ComponentModel`, `Microsoft.Extensions.*`).
2. **Replace `Main`** with the `WinFormsApplication.CreateBuilder(...)`
   pattern shown above. Keep the old `Application.SetCompatibleText…`
   calls only if the builder doesn't already configure that for you (see
   `WinFormsApplicationOptions` and the `Use*` fluent methods).
3. **For each Form / UserControl** the host should be able to resolve:
   - Keep the existing parameter-less constructor (Designer needs it,
     and the DI constructor will chain into it via `: this()`).
   - Add a new `Form(IServiceProvider sp) : this()` constructor
     following the shape above. Wrap the supplied provider in a
     `DeferredServiceProvider` before assigning the field.
   - Make the Form / UserControl implement `IServiceProvider`
     explicitly and add the nested `DeferredServiceProvider` class.
   - Register the Form: `builder.Services.TryAddScoped<MyForm>();` (or
     pass it to `UseStartupForm<TForm>()` for the start-up Form).
4. **Move static singletons** (logging, settings, dialog helpers) into
   DI: register implementations, inject via constructor.
5. **Search for `MessageBox.Show` in view-models** and replace with
   `IDialogService` calls.
6. **Run the app.** The hardest-to-spot failure mode is a component
   that synchronously calls `GetService` from inside its own
   `SetServiceProvider` override — at that point `_serviceProvider`
   is still `null` because the parameterized ctor body has not run
   yet. You will notice it as an `InvalidOperationException` from the
   form's `IServiceProvider.GetService`, or as components silently
   falling back to `NullServiceProvider` (no exceptions, but no
   services either). Defer the actual service lookups to
   `ISupportInitialize.EndInit` or `OnLoad`.

> **Tip:** VB projects do not get a `Program.vb`. Use the VB Application
> Framework (`ApplicationEvents.vb`) for `ApplyApplicationDefaults` and
> the WARP-provided startup integration for DI registration. See the
> `warp-app-services` skill for the VB recipe.

## The "too early" trap

Even with the Form correctly prepared, code inside a component must not
assume the service provider is *fully usable* at the moment
`SetServiceProvider(provider)` runs. There are three subtleties:

1. **Order of component initialization.** Inside `InitializeComponent`,
   components are added one by one. When component *A* has its
   `SetServiceProvider` called, component *B* later in the file does not
   yet exist in the Form's `components` container.
2. **The Form's Handle is not yet created.** Anything that depends on
   `Form.Handle`, `Control.InvokeAsync`, or a captured
   `WindowsFormsSynchronizationContext` may not work yet during
   `InitializeComponent`.
3. **`Form.Load` has not yet fired.** Many WARP services and view-models
   key off `OnLoad` to start work.

If a component immediately tries to "say hello" to other services from
inside `SetServiceProvider`, you may hit `NullReferenceException`,
`InvalidOperationException`, or silent no-ops. **Defer.**

### The deferral pattern: `ISupportInitialize` + `OnLoad`

`AIServicesComponent` is the reference implementation. It:

- Implements `ISupportInitialize`.
- Sets `_isInitializing = true` in `BeginInit` so any property setter
  that *would* trigger a service lookup just records the value.
- Inside `EndInit` (called by the Designer **after** all
  `SetServiceProvider` calls for the form's components have completed),
  it calls `TrySetServiceProviderFromEitherSource()` once and is now
  safe to use.

When you write a new component that derives from
`BindableServiceProviderComponent`, follow the same shape:

```csharp
[DesignerSerializer(
    "WarpToolkit.WinForms.Design.ServiceProviderAssignableComponentCodeDomSerializer",
    "Microsoft.DotNet.DesignTools.Serialization.CodeDomSerializer")]
[ToolboxItem(true)]
public partial class MyServiceComponent
    : BindableServiceProviderComponent, ISupportInitialize
{
    private bool _isInitializing;

    void ISupportInitialize.BeginInit() => _isInitializing = true;

    void ISupportInitialize.EndInit()
    {
        _isInitializing = false;
        TryWireUpDependencies();
    }

    public override IServiceProvider SetServiceProvider(object serviceProvider)
    {
        var provider = base.SetServiceProvider(serviceProvider);

        // DO NOT resolve services here directly while _isInitializing is true —
        // the surrounding Form may still be inside InitializeComponent.
        if (!_isInitializing)
        {
            TryWireUpDependencies();
        }

        return provider;
    }

    private void TryWireUpDependencies()
    {
        try
        {
            // Now safe(r): the Form's constructor body has presumably run,
            // EndInit has been called, and the rest of the components on the
            // form have been registered.
            var sp = ((IServiceProvider)this);
            _logger = sp.GetService<ILogger<MyServiceComponent>>() ?? NullLogger<MyServiceComponent>.Instance;
            _logger.LogDebug("MyServiceComponent wired up.");
        }
        catch (Exception ex)
        {
            // Swallow with diagnostics — the surrounding Form is allowed to
            // be in an indeterminate state. Real work happens later, when
            // Form.Load fires.
            Debug.WriteLine($"[MyServiceComponent] Deferred wire-up failed: {ex}");
        }
    }
}
```

### Form-side deferral

Things inside the **Form** that need real services should also be moved
out of the constructor body:

| Where to put what | Why |
|-------------------|-----|
| `InitializeComponent()` | Designer-only. No service access. |
| Ctor body, **after** `InitializeComponent` | Only operations that touch *the form's own state* (event handler wiring, command setup) — these can lightly touch `_serviceProvider` for sync-only services (e.g. fetching a singleton ViewModel). |
| `OnLoad` | Anything that needs HWNDs, async, the message pump, other forms, or services that might themselves require the Form to be fully constructed (e.g. ChatView's WebView2). |
| `OnShown` | First-time-visible work (focus, "what's new" banners). |

```csharp
protected override async void OnLoad(EventArgs e)
{
    base.OnLoad(e);

    try
    {
        // Now is the right time to talk to WebView2-backed services etc.
        var services = _serviceProvider ?? throw new InvalidOperationException(
            "MainForm must be resolved from WinFormsApplication so DI can provide the service provider.");

        _chatView = services.GetRequiredService<ChatView>();
        _chatView.AIUserChatService = _openAIChatService;
        _mainTabControl.AddTab("Chat", _chatView);
    }
    catch (Exception ex)
    {
        await _dialogs.ShowErrorAsync(
            $"Failed to initialize chat: {ex.Message}", "Startup");
    }
}
```

> **`async void` is OK for OnLoad / event handlers**, but **must** be
> wrapped in `try`/`catch`. An unhandled exception from `async void` will
> tear down the process even if you registered an
> `IWinFormsAppExceptionService`.

## Anti-patterns

- **Do not** access `_serviceProvider` from a field initializer. Field
  initializers run *before* the constructor body and *before* DI has
  passed the provider in.
- **Do not** call `InitializeComponent()` directly from the
  parameterized DI constructor. Chain `: this()` instead so the
  parameter-less constructor (the one the Designer also uses) owns
  `InitializeComponent`. Calling `InitializeComponent` from both
  constructors will run it twice and double-register every component.
- **Do not** assign the real provider directly to `_serviceProvider` —
  wrap it in `DeferredServiceProvider`. The wrapper is what lets the
  Form act as a valid IServiceProvider façade for component
  references captured during `InitializeComponent`, even though the
  field itself is only populated after `InitializeComponent` has
  returned.
- **Do not** synchronously call `GetService` from a component's
  `SetServiceProvider` override. The Form's field is still `null` at
  that point. Defer to `ISupportInitialize.EndInit` or `OnLoad`.
- **Do not** add a second IServiceProvider implementation that races
  with the explicit one — implement `IServiceProvider` once, via
  explicit interface implementation.
- **Do not** use `Application.Run` directly while you also use
  `WinFormsApplication.Run` — the latter owns the message loop. Only one
  of them runs in a given process.
- **Do not** instantiate components in the Designer with a custom
  parameterized constructor — Designer requires a parameterless ctor.
  Use a property setter or `ISupportInitialize.EndInit` to apply
  parameters at runtime.
- **Do not** consume services inside `SetServiceProvider` while
  `_isInitializing` is true. Defer to `EndInit` or `OnLoad`.
- **Do not** throw out of `SetServiceProvider` for non-fatal reasons.
  At design-time the provider is a `NullServiceProvider` by construction;
  tolerate it. At runtime, a missing service should still let the form
  open with a degraded feature, not crash the Designer.
- **Do not** pass `this` as the service provider to a child UserControl
  via a custom property if the UserControl itself is a Designer-aware
  `BindableServiceProviderComponent`. The Designer will already emit
  `SetServiceProvider(this)` on it.
- **Do not** store the `IServiceProvider` in a `static` field. Each
  `WinFormsApplication` instance has its own root scope; static caches
  leak across tests and across logical app restarts.

## Debugging checklist when DI fails

| Symptom | Likely cause |
|---------|--------------|
| `NullReferenceException` in component code accessing services | A component synchronously called `GetService` from `SetServiceProvider` — the Form's `_serviceProvider` field is only assigned *after* the parameter-less ctor (and thus `InitializeComponent`) has finished. Defer the lookup to `EndInit` / `OnLoad`. |
| `InvalidOperationException` from the Form's own `IServiceProvider.GetService` | The Form was constructed via the parameter-less ctor at runtime (e.g. `new MainForm()`), or the parameterized ctor failed to chain `: this()` and assign the `DeferredServiceProvider`. Always resolve Forms through DI. |
| Component reports it has a `NullServiceProvider` at runtime | The Form is not implementing `IServiceProvider`, or the Form was constructed via `new MainForm()` instead of being resolved from DI. |
| Designer crash when opening the form | `SetServiceProvider` threw because `EnsurePlausibleServiceProvider` was bypassed by an override. Always call `base.SetServiceProvider(...)`. |
| `InvalidOperationException: A startup form must be specified` | Missing `UseStartupForm<T>()` in the builder pipeline. |
| Multiple instances complain about each other | A previous run leaked `WinFormsApplication.Current`. Make sure tests dispose the app before creating a new one. |

## Where to look next

- Dialog / settings / exception / key services that live in the same
  bootstrap — `warp-app-services`.
- AI components (`AIServicesComponent`, provider-specific chat services,
  `ChatView`) that *require* this DI bootstrap — `warp-winforms-ai`.
- Multi-tab UIs whose tab pages are themselves DI-resolved UserControls —
  `warp-fluent-tab-control`.
