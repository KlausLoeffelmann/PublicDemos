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
    private readonly IVisualStudioDiscoveryService _visualStudioDiscoveryService;
    private readonly IPackageStore _packageStore;
    private readonly IInstalledAppScanner _installedAppScanner;
    private readonly IPackageEditorDialogService _dialogService;
    private bool _suppressAutoSave;

    [ObservableProperty]
    private PackageViewModel? _selectedPackage;

    [ObservableProperty]
    private AppEntryViewModel? _selectedApp;

    [ObservableProperty]
    private NavigationNodeViewModel? _selectedNavigationNode;

    [ObservableProperty]
    private string _statusText = "Ready";

    public MainViewModel(
        ICatalogService catalogService,
        IConsoleService consoleService,
        IVisualStudioDiscoveryService visualStudioDiscoveryService,
        IPackageStore packageStore,
        IInstalledAppScanner installedAppScanner,
        IPackageEditorDialogService dialogService)
    {
        _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        _consoleService = consoleService ?? throw new ArgumentNullException(nameof(consoleService));
        _visualStudioDiscoveryService = visualStudioDiscoveryService ?? throw new ArgumentNullException(nameof(visualStudioDiscoveryService));
        _packageStore = packageStore ?? throw new ArgumentNullException(nameof(packageStore));
        _installedAppScanner = installedAppScanner ?? throw new ArgumentNullException(nameof(installedAppScanner));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

        ConsoleMessages = _consoleService.Messages;
        VisualStudioBranch = new VisualStudioBranchViewModel(_visualStudioDiscoveryService.DiscoverInstances());
        LoadPackages();
    }

    public ObservableCollection<PackageViewModel> Packages { get; } = [];

    public ObservableCollection<AppEntryViewModel> CurrentApps { get; } = [];

    public ObservableCollection<NavigationNodeViewModel> NavigationRoots { get; } = [];

    public ObservableCollection<ConsoleMessage> ConsoleMessages { get; }

    public VisualStudioBranchViewModel VisualStudioBranch { get; }

    public void WriteConsole(ConsoleMessageKind kind, string text) => _consoleService.Write(kind, text);

    public event EventHandler<ViewCommandKind>? ViewCommandRequested;

    partial void OnSelectedPackageChanged(PackageViewModel? value)
    {
        RefreshCurrentApps();
        AddAppCommand.NotifyCanExecuteChanged();
        ExportCommand.NotifyCanExecuteChanged();
        RemovePackageCommand.NotifyCanExecuteChanged();
        UpdateCurrentPackageCommand.NotifyCanExecuteChanged();
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
            case VisualStudioBranchViewModel branch:
                SelectedPackage = null;
                SelectedApp = null;
                StatusText = $"Visual Studio: {branch.Rows.Count} installation/hive item(s)";
                break;
            case VisualStudioVersionViewModel version:
                SelectedPackage = null;
                SelectedApp = null;
                StatusText = $"Visual Studio {version.Year}: {version.Rows.Count} installation/hive item(s)";
                break;
            case VisualStudioSkuComboViewModel combo:
                SelectedPackage = null;
                SelectedApp = null;
                StatusText = $"Visual Studio {combo.ComboLabel}: {combo.Rows.Count} installation/hive item(s)";
                break;
            case VisualStudioInstanceViewModel instance:
                SelectedPackage = null;
                SelectedApp = null;
                StatusText = $"Visual Studio instance: {instance.Model.DisplayName} ({instance.Model.Version})";
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

        AddPackage(package);
        SelectedPackage = package;
        SelectedApp = null;
        RebuildNavigation();
        SavePackage(package);
        _consoleService.Write(ConsoleMessageKind.Command, $"Created package '{package.Name}'.");
    }

    [RelayCommand]
    private void NewFromExistingPackage()
    {
        if (Packages.Count == 0)
        {
            _consoleService.Write(ConsoleMessageKind.Warning, "No existing packages to copy from.");
            return;
        }

        NewFromExistingResult? result = _dialogService.AskNewFromExisting(
            Packages.Select(package => package.Model).ToList());

        if (result is null)
        {
            return;
        }

        WingetPackage clone = WingetPackage.Clone(result.SourcePackage);
        clone.Id = Guid.NewGuid().ToString("N");
        clone.Name = result.NewName;

        PackageViewModel package = new(clone);
        AddPackage(package);
        SelectedPackage = package;
        SelectedApp = null;
        RebuildNavigation();
        SavePackage(package);
        _consoleService.Write(ConsoleMessageKind.Command, $"Created package '{package.Name}' from '{result.SourcePackage.Name}'.");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void RemovePackage()
    {
        PackageViewModel package = SelectedPackage!;
        if (!_dialogService.ConfirmRemovePackage(package.Name))
        {
            return;
        }

        _packageStore.Delete(package.Model);
        Packages.Remove(package);
        SelectedApp = null;
        SelectedPackage = Packages.FirstOrDefault();
        RebuildNavigation();
        _consoleService.Write(ConsoleMessageKind.Command, $"Removed package '{package.Name}' (backup written).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void UpdateCurrentPackage()
    {
        PackageViewModel package = SelectedPackage!;
        IReadOnlyList<string> installedIds = _installedAppScanner.GetInstalledWingetIds();
        HashSet<string> installed = new(installedIds, StringComparer.OrdinalIgnoreCase);
        HashSet<string> existing = new(
            package.Model.Apps.Select(app => app.Id),
            StringComparer.OrdinalIgnoreCase);

        int added = 0;
        _suppressAutoSave = true;
        try
        {
            foreach (AppEntry template in _catalogService.GetWellKnownApps())
            {
                if (!installed.Contains(template.Id) || existing.Contains(template.Id))
                {
                    continue;
                }

                package.AddApp(WingetPackage.Clone(new WingetPackage { Apps = [template] }).Apps[0]);
                existing.Add(template.Id);
                added++;
            }
        }
        finally
        {
            _suppressAutoSave = false;
        }

        if (added > 0)
        {
            RefreshCurrentApps();
            RebuildNavigation();
            SavePackage(package);
        }

        _consoleService.Write(ConsoleMessageKind.Command,
            $"Update current package: added {added} installed app(s) to '{package.Name}'.");
    }

    [RelayCommand]
    private void OpenPackage()
    {
        _consoleService.Write(ConsoleMessageKind.Command, "Import package command executed (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void Export()
    {
        _consoleService.Write(ConsoleMessageKind.Command, $"Export YAML+Script command executed for '{SelectedPackage!.Name}' (V0 placeholder).");
    }

    [RelayCommand(CanExecute = nameof(HasSelectedPackage))]
    private void AddApp()
    {
        AppEntry? configured = _dialogService.PickAndConfigureApp(_catalogService.GetWellKnownApps());
        if (configured is null)
        {
            return;
        }

        AppEntryViewModel app = SelectedPackage!.AddApp(configured);
        SelectedApp = app;
        RefreshCurrentApps();
        RebuildNavigation();
        SavePackage(SelectedPackage);
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
        RefreshCurrentApps();
        RebuildNavigation();
        SavePackage(package);
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
        RequestViewCommand(ViewCommandKind.ShowOptions, "Options dialog requested.");
    }

    [RelayCommand]
    private void ExpandAllNodes()
    {
        RequestViewCommand(ViewCommandKind.ExpandAllNodes, "Expand all tree nodes requested.");
    }

    [RelayCommand]
    private void CollapseSelectedNode()
    {
        RequestViewCommand(ViewCommandKind.CollapseSelectedNode, "Collapse selected tree node requested.");
    }

    [RelayCommand]
    private void ExpandSelectedNode()
    {
        RequestViewCommand(ViewCommandKind.ExpandSelectedNode, "Expand selected tree node requested.");
    }

    [RelayCommand]
    private void Quit()
    {
        _consoleService.Write(ConsoleMessageKind.Command, "Quit command executed.");
    }

    private bool HasSelectedPackage() => SelectedPackage is not null;

    private bool HasSelectedApp() => SelectedApp is not null;

    private void LoadPackages()
    {
        IReadOnlyList<WingetPackage> stored = _packageStore.LoadAll();

        if (stored.Count == 0)
        {
            PackageViewModel demo = new(_catalogService.CreateDemoPackage());
            AddPackage(demo);
            SavePackage(demo);
            SelectedPackage = demo;
            RebuildNavigation();
            _consoleService.Write(ConsoleMessageKind.Info, "Loaded V0 demo package.");
            return;
        }

        foreach (WingetPackage model in stored.OrderBy(package => package.Name, StringComparer.OrdinalIgnoreCase))
        {
            AddPackage(new PackageViewModel(model));
        }

        SelectedPackage = Packages.FirstOrDefault();
        RebuildNavigation();
        _consoleService.Write(ConsoleMessageKind.Info, $"Loaded {Packages.Count} package(s) from disk.");
    }

    private void AddPackage(PackageViewModel package)
    {
        Packages.Add(package);
        AttachAutoSave(package);
    }

    private void AttachAutoSave(PackageViewModel package)
    {
        package.PropertyChanged += (_, _) => SavePackage(package);
        package.Apps.CollectionChanged += (_, args) =>
        {
            if (args.NewItems is not null)
            {
                foreach (AppEntryViewModel app in args.NewItems.OfType<AppEntryViewModel>())
                {
                    app.PropertyChanged += (_, _) => SavePackage(package);
                }
            }
        };

        foreach (AppEntryViewModel app in package.Apps)
        {
            app.PropertyChanged += (_, _) => SavePackage(package);
        }
    }

    private void SavePackage(PackageViewModel package)
    {
        if (_suppressAutoSave)
        {
            return;
        }

        _packageStore.Save(package.Model);
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
            string packageKey = $"package:{package.Name}";
            NavigationNodeViewModel packageNode = new(
                package.TreeText,
                NavigationNodeKind.Package,
                package,
                packageKey);

            foreach (AppEntryViewModel app in package.Apps)
            {
                NavigationNodeViewModel appNode = new(
                    app.TreeText,
                    NavigationNodeKind.App,
                    app,
                    $"{packageKey}/app:{app.Id}");
                AddExtensionNodes(appNode, app.Model);
                packageNode.Children.Add(appNode);
            }

            packageNode.Children.Add(CreateVisualStudioNode(packageKey));
            NavigationRoots.Add(packageNode);
        }

        SelectedNavigationNode = FindSelectedNavigationNode();
    }

    private NavigationNodeViewModel CreateVisualStudioNode(string packageKey)
    {
        string visualStudioKey = $"{packageKey}/vs";
        NavigationNodeViewModel root = new(
            VisualStudioBranch.TreeText,
            NavigationNodeKind.VisualStudioRoot,
            VisualStudioBranch,
            visualStudioKey);

        foreach (VisualStudioVersionViewModel version in VisualStudioBranch.Versions)
        {
            string versionKey = $"{visualStudioKey}/year:{version.Year}";
            NavigationNodeViewModel versionNode = new(
                version.TreeText,
                NavigationNodeKind.VisualStudioVersion,
                version,
                versionKey);

            foreach (VisualStudioSkuComboViewModel combo in version.SkuCombos)
            {
                string comboKey = $"{versionKey}/sku:{combo.ComboLabel}";
                NavigationNodeViewModel comboNode = new(
                    combo.TreeText,
                    NavigationNodeKind.VisualStudioSkuCombo,
                    combo,
                    comboKey);

                foreach (VisualStudioInstanceViewModel instance in combo.Instances)
                {
                    comboNode.Children.Add(new NavigationNodeViewModel(
                        instance.TreeText,
                        NavigationNodeKind.VisualStudioInstance,
                        instance,
                        $"{comboKey}/instance:{instance.Id}"));
                }

                versionNode.Children.Add(comboNode);
            }

            root.Children.Add(versionNode);
        }

        return root;
    }

    private static void AddExtensionNodes(NavigationNodeViewModel appNode, AppEntry app)
    {
        switch (app)
        {
            case VSCodeEntry code:
                foreach (string extension in code.Extensions)
                {
                    appNode.Children.Add(new NavigationNodeViewModel(
                        extension,
                        NavigationNodeKind.Extension,
                        extension,
                        $"{appNode.Key}/extension:{extension}"));
                }
                break;
            case VisualStudioEntry visualStudio:
                foreach (VsixReference extension in visualStudio.Extensions)
                {
                    appNode.Children.Add(new NavigationNodeViewModel(
                        extension.Identifier,
                        NavigationNodeKind.Extension,
                        extension,
                        $"{appNode.Key}/extension:{extension.Identifier}"));
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

        if (SelectedNavigationNode is not null)
        {
            return FindNodeByKey(NavigationRoots, SelectedNavigationNode.Key);
        }

        return null;
    }

    private static NavigationNodeViewModel? FindNodeByKey(IEnumerable<NavigationNodeViewModel> nodes, string key)
    {
        foreach (NavigationNodeViewModel node in nodes)
        {
            if (StringComparer.Ordinal.Equals(node.Key, key))
            {
                return node;
            }

            NavigationNodeViewModel? child = FindNodeByKey(node.Children, key);
            if (child is not null)
            {
                return child;
            }
        }

        return null;
    }

    private void RequestViewCommand(ViewCommandKind kind, string message)
    {
        _consoleService.Write(ConsoleMessageKind.Command, message);
        ViewCommandRequested?.Invoke(this, kind);
    }
}
