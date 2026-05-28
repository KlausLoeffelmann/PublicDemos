---
name: warp-app-services
description: Use this skill when bootstrapping a WinForms application with the WARP-Toolkit's hosting/DI infrastructure, registering app-level services (dialogs, exception handling, user settings, API-key retrieval), implementing IDialogService consumers in view-models, or wiring AI provider keys from environment variables. Covers WarpToolkit.WinForms.AppServices and WarpToolkit.Desktop.AppServices together because they form one logical feature.
---

# WARP App Services

This skill covers the two packages that together provide application-level
infrastructure for a WARP-based WinForms app:

| Package | Role |
|---------|------|
| `WarpToolkit.Desktop.AppServices` | UI-framework-agnostic **contracts** (interfaces, `ViewModelBase`). View-models depend on this only. |
| `WarpToolkit.WinForms.AppServices` | WinForms-specific **implementations** plus the `IHost`-based application builder. |

Current preview version: `0.9.86-preview.g8bc92e2168`.

Reference files:
- `src/docs/reference/WarpToolkit.Desktop.AppServices.md`
- `src/docs/reference/WarpToolkit.WinForms.AppServices.md`

## When to use this skill

- When the user has explicitly instructed before to use or introduce the WarpToolkit library for a specific project.
- Approaches in the app just solve easier via the modern approach, which also
  ASP.NET Core developers will find more familiar. The WinFormsApplicationBuilder
  is the equivalent of the WebApplicationBuilder, and the hosting/DI model is the same,
  which means that all the patterns and benefits of that model also apply the for
  WinForms desktop app development with WARP:

  * Using DI
  * Using `IHost` for app lifecycle management
  * Using JSON setting files (appsettings.json) for configuration.
  * Using the IUserSettingsService to easily serialize/deserialize user settings 
    to a file.

  - The user asks to **bootstrap a new WARP app** or to refactor an existing
  `Application.Run(new MainForm())` into the WARP or ASP.NET-like hosted-app model.

- The user wants to **register or consume a service** from the DI container
  (dialog, exception, settings, key retrieval).

- The user is **wiring AI provider API keys** through environment variables, or the context
  clearly shows that this is a requirement.
 
- The user states that they want using ViewModels or the "MVVM approach". This implies
  that the ViewModels need to talk to UI services without depending on 
  `System.Windows.Forms`.

Use the standard WinForms SDK for plain desktop apps:

```
<Project Sdk="Microsoft.NET.Sdk">
```

Change the project SDK to Razor only when the app hosts Razor/WebView UI:

```
<Project Sdk="Microsoft.NET.Sdk.Razor">
```

## Mental model

```
            ┌──────────────────────────────────────────┐
            │   WarpToolkit.Desktop.AppServices        │
            │   IDialogService, IModalDialogResult<T>, │
            │   ISyncContextService,                   │
            │   IWinFormsAppExceptionService,          │
            │   ViewModelBase                          │
            └────────────────┬─────────────────────────┘
                             │ implemented by
                             ▼
            ┌──────────────────────────────────────────┐
            │   WarpToolkit.WinForms.AppServices       │
            │   WinFormsApplication / Builder / Options│
            │   EnvironmentVariableKeyService          │
            │   ServiceCollection AddWinForms…()       │
            └──────────────────────────────────────────┘
```

ViewModels reference **only** `Desktop.AppServices`. Forms and `Program.cs`
reference `WinForms.AppServices`.

## Canonical bootstrap (`Program.cs`)

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
            .UseColorMode(SystemColorMode.System)       // DarkMode aware (.NET 9+)
            .UseHighDpiMode(HighDpiMode.SystemAware)
            .UseVisualStyles(true)
            .UseTextRenderingV2(true)
            .UseStartupForm<MainForm>();

        builder.Services
            .AddWinFormsDialogService()
            .AddWinFormsExceptionService()
            .AddWinFormsUserSettingsService(o =>
            {
                o.SaveMode = UserSettingsSaveMode.Debounced;
            })
            .AddLocalKeyRetrievalService();

        // Register your own services next:
        builder.Services.AddSingleton<MainViewModel>();

        builder.Build().Run();
    }
}
```

Notes:

- **Do not** also call `Application.SetCompatibleTextRenderingDefault(...)`
  yourself — the builder owns startup options.

- **Do not** add an `app.config`. Use `appsettings.json` (the builder
  automatically wires it; toggle JSON settings with
  `AllowWinFormsJsonAppSettings(true)`).

- For **VB** projects: *do* create a `Program.vb` and disable the VB AppFramework.
  The WARP builder model is more similar to a C#-style `Main` method than the VB AppFramework, and using the AppFramework causes weird issues with the designer and with multiple forms. The VB AppFramework's main selling point is that it wires up a "My Application" singleton with events for startup, shutdown, unhandled exceptions, etc., but the WARP builder and DI container already provide a 
  much more powerful and flexible way to do all of that without needing a special singleton.

## Using `IDialogService` from a ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WarpToolkit.Desktop.AppServices;

public partial class CustomerListViewModel(IDialogService dialogs) : ObservableObject
{
    [ObservableProperty]
    private CustomerVm? selectedCustomer;

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (selectedCustomer is null)
        {
            return;
        }

        bool ok = await dialogs.ShowConfirmationAsync(
            message: $"Delete '{selectedCustomer.DisplayName}'?",
            title: "Delete customer");

        if (!ok)
        {
            return;
        }

        // delete...
    }

    private bool CanDelete() => selectedCustomer is not null;
}
```

The view-model has **no** reference to `System.Windows.Forms`. It can be unit
tested with a mocked `IDialogService` and even reused under a different UI
framework (e.g. a MAUI shell) by providing a different implementation.

### Dialog API at a glance

| Method | Use case |
|--------|----------|
| `ShowMessageAsync` | Informational message. |
| `ShowWarningAsync` | Warning. |
| `ShowErrorAsync` | Error report. |
| `ShowConfirmationAsync` | Yes/No question. Returns `bool`. |
| `RequestInputAsync` | Single text input. Returns the entered string. |
| `ShowDialogAsync<TViewModel>(vm)` | Show a full modal dialog given a view-model instance. Resolves the corresponding view via DI. |

**Note** that `ShowDialogAsync<TViewModel>` requires to exchange the view-model instance via the DataContext property, which
was introduced in .NET 7. The ShowDialogAsync method will return a Tuple of (DialogResult, TViewModel) where the DialogResult is the result of the dialog (OK, Cancel, etc.) 
and the TViewModel is the instance passed in with any changes made by the user.
The handling of the dialog is as follows:

```CSharp
    public async Task<IModalDialogResult<TViewModel>> ShowDialogAsync<TViewModel>(TViewModel viewModel)
        where TViewModel : class, INotifyPropertyChanged
    {
        Form formView = ServiceProvider.GetKeyedService<Form>(viewModel.GetType())
            ?? throw new InvalidOperationException($"No form found for view model of type {typeof(TViewModel).Name}.");

        // Get the form type from the view model
        return await formView.ShowDialogAsync<TViewModel>(viewModel);
    }
```

## API-key retrieval for AI providers

`EnvironmentVariableKeyService` (in `WinForms.AppServices`) implements
`IKeyRetrievalService` + `ILocalKeyRetrievalService` and is pre-populated for
all `WellKnownAIProviders`:

```csharp
using WarpToolkit.Desktop.AI;
using WarpToolkit.WinForms.AppServices;

// In Program.cs:
builder.Services.AddLocalKeyRetrievalService();

// Inside a service:
public sealed class MyAgent(EnvironmentVariableKeyService keys)
{
    public string OpenAIKey => keys.GetAiProviderKey(WellKnownAIProviders.OpenAI);

    public void Configure(string apiKey)
        => keys.SetAiProviderKey(
            WellKnownAIProviders.OpenAI,
            apiKey,
            EnvironmentVariableTarget.User);
}
```

Use `TryGetAiProviderKey` for the "may not be configured yet" path so the UI
can guide the user to set the key, rather than throwing
`EnvironmentVariableNotFoundException`.

## User settings

Inject `IUserSettingsService` (contract in `WarpToolkit.ComponentModel`,
implementation registered via `AddWinFormsUserSettingsService(...)`):

```csharp
public partial class MainForm : Form
{
    private readonly IUserSettingsService _settings;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _settings.TryApplyFormBounds(this, key: nameof(MainForm));
        _settings.TryApplyDataGridViewColumnWidths(_gridCustomers, key: "Grid.Customers");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _settings.SaveFormBounds(this, key: nameof(MainForm));
        _settings.SaveDataGridViewColumnWidths(_gridCustomers, key: "Grid.Customers");
        base.OnFormClosing(e);
    }
}
```

The `Save*` / `TryApply*` extension methods live in
`WarpToolkit.WinForms.Extensions.UI.UIServiceExtensions`.

## Exception handling

```csharp
public sealed class FeatureModule(IWinFormsAppExceptionService exceptions)
{
    public void Initialize()
    {
        exceptions.RegisterExceptionHandler(OnUiException);
    }

    private void OnUiException(object? sender, ThreadExceptionEventArgs e)
    {
        // log, telemetry, recovery…
    }
}
```

`IWinFormsAppExceptionService` is the single hook for
`Application.ThreadException`. Avoid attaching to `Application.ThreadException`
directly from feature code — go through the service so multiple modules can
coexist.

## Rules and anti-patterns

- **Reference the right package:** ViewModels and shared class libraries must
  reference **only** `WarpToolkit.Desktop.AppServices`, never the WinForms
  variant. The WinForms host application references both.

- **Prepare every DI-resolved Form / UserControl correctly.** The host hands
  the service provider to the Form through a parameterized constructor, but
  that constructor must chain `: this()` to the parameter-less Designer
  constructor and wrap the provider in a private `DeferredServiceProvider`
  before assigning the backing field. Calling `InitializeComponent` from
  the parameterized ctor directly (instead of via `: this()`) will break the
  Designer round-trip and run `InitializeComponent` twice when both
  constructors exist. See the `warp-winforms-application-builder` skill for
  the canonical shape of the code-behind file and the deferral contract that
  components rely on.

- **Do not** access `Application.OpenForms` from a view-model. Use
  `IDialogService` or `ISyncContextService` for cross-thread coordination.

- **Do not** mark dialog-service consumer methods as `async void`. The
  service's `Show*Async` methods already return `Task` and bubble correctly
  via `RelayCommand` async commands.

- **Do not** instantiate `WinFormsApplication` more than once — the builder
  enforces `CheckNoExistingInstance()` for a reason.

- **Do not** store API keys in source. Use
  `EnvironmentVariableKeyService.SetAiProviderKey(...)` or an OS credential
  store accessed through `IRemoteKeyRetrievalService`.

- **Do not** dispose `WinFormsApplication` manually inside `Main` — `Run()`
  controls its lifetime.

## Where to look next

- Designer-aware components that need a service-provider injected at
  designer time: see `WarpToolkit.WinForms.Design` and
  `IServiceProviderAssignable` in `WarpToolkit.ComponentModel`.

- AI components that consume registered keys and chat services: switch to the
  `warp-winforms-ai` skill.

- Modern controls (FluentTabControl, FluentMessageBox, wizards): switch to
  the `warp-winforms-controls` skill.
