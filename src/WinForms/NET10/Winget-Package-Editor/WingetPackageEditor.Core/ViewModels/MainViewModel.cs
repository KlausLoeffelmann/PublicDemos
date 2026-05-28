using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;

namespace WingetPackageEditor.Core.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly ICatalogService _catalogService;
    private readonly IConsoleService _consoleService;

    [ObservableProperty]
    private PackageViewModel? _selectedPackage;

    [ObservableProperty]
    private AppEntryViewModel? _selectedApp;

    [ObservableProperty]
    private NavigationNodeViewModel? _selectedNavigationNode;

    [ObservableProperty]
    private string _statusText = "Ready";

    public MainViewModel(ICatalogService catalogService, IConsoleService consoleService)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));

        ConsoleMessages = _consoleService.Messages;
        LoadDemoPackage();
    }

    public ObservableCollection<PackageViewModel> Packages { get; } = [];

    public ObservableCollection<AppEntryViewModel> CurrentApps { get; } = [];

    public ObservableCollection<NavigationNodeViewModel> NavigationRoots { get; } = [];

    public ObservableCollection<ConsoleMessage> ConsoleMessages { get; }

    partial void OnSelectedPackageChanged(PackageViewModel? value)
    {
        RefreshCurrentApps();
        AddAppCommand.NotifyCanExecuteChanged();
        SavePackageCommand.NotifyCanExecuteChanged();
        SavePackageAsCommand.NotifyCanExecuteChanged();
        ExportCommand.NotifyCanExecuteChanged();
        ApplyNowCommand.NotifyCanExecuteChanged();
        GenerateBundleFolderCommand.NotifyCanExecuteChanged();
        UpdateStatus();
    }

    partial void OnSelectedAppChanged(AppEntryViewModel? value)
    {
        RemoveAppCommand.NotifyCanExecuteChanged();
        PropertiesCommand.NotifyCanExecuteChanged();
        UpdateStatus();
    }

    partial void OnSelectedNavigationNodeChanged(NavigationNodeViewModel? value)
    {
        switch (value?.Value)
        {
            case PackageViewModel package:
                SelectedPackage = package;
                SelectedApp = null;
                break;
            case AppEntryViewModel app:
                SelectedPackage = Packages.FirstOrDefault(package => package.Apps.Contains(app));
                SelectedApp = app;
                break;
            default:
                SelectedApp = null;
                break;
        }
    }

    [RelayCommand]
    private void NewPackage()
    {
        PackageViewModel package = new(new WingetPackage
        {
            Name = $"New Package {Packages.Count + 1}",
            Author = Environment.UserName
        });

        Packages.Add(package);
        SelectedPackage = package;
        SelectedApp = null;
        RebuildNavigation();
        _consoleService.Write(ConsoleMessageKind.Command, $"Created package '{package.Name}'.");
    }

    [RelayCommand]
    private void OpenPackage()
    {
        _consoleService.Write(ConsoleMessageKind.Command, "Open package command executed (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void SavePackage()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Save package command executed for '{SelectedPackage!.Name}' (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void SavePackageAs()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Save package as command executed for '{SelectedPackage!.Name}' (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void Export()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Export YAML+Script command executed for '{SelectedPackage!.Name}' (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void AddApp()
    {
        AppEntryViewModel app = SelectedPackage!.AddApp(_catalogService.CreateDefaultApp());
        SelectedApp = app;
        RebuildNavigation();
        _consoleService.Write(ConsoleMessageKind.Command, $"Added app '{app.DisplayName}' to '{SelectedPackage.Name}'.");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedApp))]
    private void RemoveApp()
    {
        AppEntryViewModel app = SelectedApp!;
        PackageViewModel? package = SelectedPackage;
        if (package is null)
        {
            return;
        }

        package.RemoveApp(app);
        SelectedApp = null;
        RebuildNavigation();
        _consoleService.Write(ConsoleMessageKind.Command, $"Removed app '{app.DisplayName}' from '{package.Name}'.");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedApp))]
    private void Properties()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Properties command executed for '{SelectedApp!.DisplayName}' (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void ApplyNow()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Apply Now command executed for '{SelectedPackage!.Name}' (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void GenerateBundleFolder()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Generate Bundle Folder command executed for '{SelectedPackage!.Name}' (V0 placeholder).");
    }

    [RelayCommand]
    private void Options()
    {
        _consoleService.Write(ConsoleMessageKind.Command, "Options command executed (V0 placeholder).");
    }

    [RelayCommand]
    private void Quit()
    {
        _consoleService.Write(ConsoleMessageKind.Command, "Quit command executed.");
    }

    private bool HasSelectedPackage() => SelectedPackage is not null;

    private bool HasSelectedApp() => SelectedApp is not null;

    private void LoadDemoPackage()
    {
        PackageViewModel package = new(_catalogService.CreateDemoPackage());
        Packages.Add(package);
        SelectedPackage = package;
        RebuildNavigation();
        _consoleService.Write(ConsoleMessageKind.Info, "Loaded V0 demo package.");
    }

    private void RefreshCurrentApps()
    {
        CurrentApps.Clear();
        if (SelectedPackage is null)
        {
            return;
        }

        foreach (AppEntryViewModel app in SelectedPackage.Apps)
        {
            CurrentApps.Add(app);
        }
    }

    private void RebuildNavigation()
    {
        NavigationRoots.Clear();

        foreach (PackageViewModel package in Packages)
        {
            NavigationNodeViewModel packageNode = new(package.TreeText, NavigationNodeKind.Package, package);
            foreach (AppEntryViewModel app in package.Apps)
            {
                NavigationNodeViewModel appNode = new(app.TreeText, NavigationNodeKind.App, app);
                AddExtensionNodes(appNode, app.Model);
                packageNode.Children.Add(appNode);
            }

            NavigationRoots.Add(packageNode);
        }

        SelectedNavigationNode = FindSelectedNavigationNode();
    }

    private static void AddExtensionNodes(NavigationNodeViewModel appNode, AppEntry app)
    {
        switch (app)
        {
            case VSCodeEntry code:
                foreach (string extension in code.Extensions)
                {
                    appNode.Children.Add(new NavigationNodeViewModel(extension, NavigationNodeKind.Extension, extension));
                }
                break;
            case VisualStudioEntry visualStudio:
                foreach (VsixReference extension in visualStudio.Extensions)
                {
                    appNode.Children.Add(new NavigationNodeViewModel(extension.Identifier, NavigationNodeKind.Extension, extension));
                }
                break;
        }
    }

    private void UpdateStatus()
    {
        StatusText = (SelectedPackage, SelectedApp) switch
        {
            (_, { } app) => $"Selected app: {app.DisplayName} ({app.Id})",
            ({ } package, _) => $"Selected package: {package.Name} ({package.Apps.Count} app(s))",
            _ => "Ready"
        };
    }

    private NavigationNodeViewModel? FindSelectedNavigationNode()
    {
        if (SelectedApp is not null)
        {
            return NavigationRoots
                .SelectMany(packageNode => packageNode.Children)
                .FirstOrDefault(appNode => ReferenceEquals(appNode.Value, SelectedApp));
        }

        if (SelectedPackage is not null)
        {
            return NavigationRoots.FirstOrDefault(packageNode => ReferenceEquals(packageNode.Value, SelectedPackage));
        }

        return null;
    }
}
