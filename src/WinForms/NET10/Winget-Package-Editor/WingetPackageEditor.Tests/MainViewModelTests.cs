using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;
using WingetPackageEditor.Core.ViewModels;

namespace WingetPackageEditor.Tests;

public sealed class MainViewModelTests
{
    [Fact]
    public void Constructor_LoadsDemoPackageAndWritesConsoleMessage()
    {
        ConsoleService console = new();
        MainViewModel viewModel = new(
            new HardcodedCatalogService(),
            console,
            new FakeVisualStudioDiscoveryService(),
            new FakePackageStore(),
            new FakeInstalledAppScanner(),
            new FakeDialogService());

        Assert.Single(viewModel.Packages);
        Assert.NotNull(viewModel.SelectedPackage);
        Assert.NotEmpty(viewModel.CurrentApps);
        Assert.NotEmpty(viewModel.NavigationRoots);
        Assert.Contains(console.Messages, message => message.Text.Contains("Loaded V0 demo package", StringComparison.Ordinal));
    }

    [Fact]
    public void NewPackageCommand_CreatesAndSelectsPackage()
    {
        MainViewModel viewModel = CreateViewModel();
        int originalCount = viewModel.Packages.Count;

        viewModel.NewPackageCommand.Execute(null);

        Assert.Equal(originalCount + 1, viewModel.Packages.Count);
        Assert.Same(viewModel.Packages.Last(), viewModel.SelectedPackage);
        Assert.Contains("Selected package", viewModel.StatusText, StringComparison.Ordinal);
    }

    [Fact]
    public void AddAndRemoveAppCommands_RoundtripSelectedPackage()
    {
        MainViewModel viewModel = CreateViewModel();
        int originalCount = viewModel.SelectedPackage!.Apps.Count;

        Assert.True(viewModel.AddAppCommand.CanExecute(null));
        viewModel.AddAppCommand.Execute(null);

        Assert.Equal(originalCount + 1, viewModel.SelectedPackage.Apps.Count);
        Assert.NotNull(viewModel.SelectedApp);
        Assert.True(viewModel.RemoveAppCommand.CanExecute(null));

        viewModel.RemoveAppCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.SelectedPackage.Apps.Count);
        Assert.Null(viewModel.SelectedApp);
    }

    [Fact]
    public void RemoveAppCommand_TracksSelectionCanExecute()
    {
        MainViewModel viewModel = CreateViewModel();
        viewModel.SelectedApp = null;

        Assert.False(viewModel.RemoveAppCommand.CanExecute(null));

        viewModel.SelectedApp = viewModel.CurrentApps[0];

        Assert.True(viewModel.RemoveAppCommand.CanExecute(null));
    }

    [Fact]
    public void SelectingNavigationNode_UpdatesSelectedPackageAndApp()
    {
        MainViewModel viewModel = CreateViewModel();
        NavigationNodeViewModel appNode = viewModel.NavigationRoots[0].Children[0];

        viewModel.SelectedNavigationNode = appNode;

        Assert.Same(appNode.Value, viewModel.SelectedApp);
        Assert.Same(viewModel.Packages[0], viewModel.SelectedPackage);
        Assert.Contains("Selected app", viewModel.StatusText, StringComparison.Ordinal);
    }

    [Fact]
    public void Commands_WriteConsoleMessages()
    {
        ConsoleService console = new();
        MainViewModel viewModel = new(
            new HardcodedCatalogService(),
            console,
            new FakeVisualStudioDiscoveryService(),
            new FakePackageStore(),
            new FakeInstalledAppScanner(),
            new FakeDialogService());
        int originalCount = console.Messages.Count;

        viewModel.OptionsCommand.Execute(null);

        Assert.Equal(originalCount + 1, console.Messages.Count);
        Assert.Equal(ConsoleMessageKind.Command, console.Messages.Last().Kind);
    }

    [Theory]
    [InlineData(nameof(MainViewModel.ExpandAllNodesCommand), ViewCommandKind.ExpandAllNodes)]
    [InlineData(nameof(MainViewModel.CollapseSelectedNodeCommand), ViewCommandKind.CollapseSelectedNode)]
    [InlineData(nameof(MainViewModel.ExpandSelectedNodeCommand), ViewCommandKind.ExpandSelectedNode)]
    [InlineData(nameof(MainViewModel.OptionsCommand), ViewCommandKind.ShowOptions)]
    public void ViewCommands_RaiseViewCommandRequests(string commandPropertyName, ViewCommandKind expectedKind)
    {
        MainViewModel viewModel = CreateViewModel();
        ViewCommandKind? requestedKind = null;
        viewModel.ViewCommandRequested += (_, kind) => requestedKind = kind;

        System.Windows.Input.ICommand command = (System.Windows.Input.ICommand)typeof(MainViewModel)
            .GetProperty(commandPropertyName)!
            .GetValue(viewModel)!;
        command.Execute(null);

        Assert.Equal(expectedKind, requestedKind);
    }

    [Fact]
    public void NavigationNodes_UseStableKeysForPersistence()
    {
        MainViewModel viewModel = CreateViewModel();

        NavigationNodeViewModel packageNode = viewModel.NavigationRoots[0];
        NavigationNodeViewModel appNode = packageNode.Children[0];

        Assert.StartsWith("package:", packageNode.Key, StringComparison.Ordinal);
        Assert.Contains("/app:", appNode.Key, StringComparison.Ordinal);
        Assert.EndsWith("/vs", packageNode.Children.Last().Key, StringComparison.Ordinal);
    }

    [Fact]
    public void VisualStudioBranch_IsNestedUnderEachPackageWithVersionSkuInstanceHierarchy()
    {
        MainViewModel viewModel = CreateViewModel();

        NavigationNodeViewModel packageNode = viewModel.NavigationRoots[0];
        NavigationNodeViewModel visualStudioRoot = packageNode.Children.Last();

        Assert.Equal(NavigationNodeKind.VisualStudioRoot, visualStudioRoot.Kind);
        Assert.Equal("Visual Studio", visualStudioRoot.Text);
        Assert.EndsWith("/vs", visualStudioRoot.Key, StringComparison.Ordinal);

        NavigationNodeViewModel versionNode = visualStudioRoot.Children[0];
        Assert.Equal(NavigationNodeKind.VisualStudioVersion, versionNode.Kind);
        Assert.Equal("2026", versionNode.Text);

        NavigationNodeViewModel comboNode = versionNode.Children[0];
        Assert.Equal(NavigationNodeKind.VisualStudioSkuCombo, comboNode.Kind);
        Assert.Equal("Preview-Enterprise", comboNode.Text);

        NavigationNodeViewModel instanceNode = comboNode.Children[0];
        Assert.Equal(NavigationNodeKind.VisualStudioInstance, instanceNode.Kind);
        Assert.IsType<VisualStudioInstanceViewModel>(instanceNode.Value);
    }

    [Fact]
    public void SelectingVisualStudioRoot_ClearsPackageSelectionAndReportsRowCount()
    {
        MainViewModel viewModel = CreateViewModel();
        NavigationNodeViewModel visualStudioRoot = viewModel.NavigationRoots[0].Children.Last();

        viewModel.SelectedNavigationNode = visualStudioRoot;

        Assert.Null(viewModel.SelectedPackage);
        Assert.Null(viewModel.SelectedApp);
        // One main installation row plus one experimental-hive row.
        Assert.Equal(2, viewModel.VisualStudioBranch.Rows.Count);
        Assert.Contains("Visual Studio", viewModel.StatusText, StringComparison.Ordinal);
    }

    [Fact]
    public void SelectingVisualStudioInstance_ProducesMainAndExperimentalRows()
    {
        MainViewModel viewModel = CreateViewModel();
        NavigationNodeViewModel instanceNode = viewModel.NavigationRoots[0].Children.Last()
            .Children[0].Children[0].Children[0];

        viewModel.SelectedNavigationNode = instanceNode;

        VisualStudioInstanceViewModel instance = Assert.IsType<VisualStudioInstanceViewModel>(instanceNode.Value);
        Assert.Equal(2, instance.Rows.Count);
        Assert.False(instance.Rows[0].IsExperimental);
        Assert.True(instance.Rows[1].IsExperimental);
        Assert.Contains("Visual Studio instance", viewModel.StatusText, StringComparison.Ordinal);
    }

    [Fact]
    public void PackageJsonSerializer_RoundtripsPolymorphicAppEntries()
    {
        WingetPackage package = new HardcodedCatalogService().CreateDemoPackage();

        string json = PackageJsonSerializer.Serialize(package);
        WingetPackage? roundtripped = PackageJsonSerializer.Deserialize(json);

        Assert.NotNull(roundtripped);
        Assert.Contains("\"$type\": \"generic\"", json, StringComparison.Ordinal);
        Assert.Contains("\"$type\": \"vscode\"", json, StringComparison.Ordinal);
        Assert.Contains("\"$type\": \"vs\"", json, StringComparison.Ordinal);
        Assert.IsType<GenericAppEntry>(roundtripped.Apps[0]);
        Assert.IsType<VSCodeEntry>(roundtripped.Apps[1]);
        Assert.IsType<VisualStudioEntry>(roundtripped.Apps[2]);
    }

    [Fact]
    public void AppEntryViewModel_SettersUpdateUnderlyingModel()
    {
        GenericAppEntry model = new()
        {
            Id = "Old.Id",
            DisplayName = "Old"
        };
        AppEntryViewModel viewModel = new(model);

        viewModel.Id = "New.Id";
        viewModel.DisplayName = "New";
        viewModel.AllowPrerelease = true;

        Assert.Equal("New.Id", model.Id);
        Assert.Equal("New", model.DisplayName);
        Assert.True(model.AllowPrerelease);
    }

    [Fact]
    public void Constructor_SeedsAndSavesDemo_WhenStoreIsEmpty()
    {
        FakePackageStore store = new();

        MainViewModel viewModel = CreateViewModel(store: store);

        Assert.Single(viewModel.Packages);
        Assert.Contains(store.Saved, package => package.Apps.Count > 0);
    }

    [Fact]
    public void Constructor_LoadsPackagesFromStore_WhenNotEmpty()
    {
        WingetPackage stored = new() { Name = "Persisted", Apps = [new GenericAppEntry { Id = "X", DisplayName = "X" }] };
        FakePackageStore store = new() { Initial = [stored] };

        MainViewModel viewModel = CreateViewModel(store: store);

        Assert.Single(viewModel.Packages);
        Assert.Equal("Persisted", viewModel.Packages[0].Name);
        Assert.Empty(store.Saved);
    }

    [Fact]
    public void NewFromExistingPackage_ClonesSourceWithNewNameAndId()
    {
        WingetPackage source = new() { Id = "source-id", Name = "Source", Apps = [new GenericAppEntry { Id = "A", DisplayName = "A" }] };
        FakePackageStore store = new() { Initial = [source] };
        FakeDialogService dialog = new()
        {
            NewFromExisting = packages => new NewFromExistingResult("Copy", packages[0])
        };

        MainViewModel viewModel = CreateViewModel(store: store, dialog: dialog);
        viewModel.NewFromExistingPackageCommand.Execute(null);

        PackageViewModel created = Assert.Single(viewModel.Packages, package => package.Name == "Copy");
        Assert.NotEqual("source-id", created.Model.Id);
        Assert.Single(created.Apps);
        Assert.Contains(store.Saved, package => package.Name == "Copy");
    }

    [Fact]
    public void RemovePackageCommand_BacksUpAndRemovesSelectedPackage()
    {
        MainViewModel viewModel = CreateViewModel(out FakePackageStore store, dialog: new FakeDialogService { ConfirmRemove = true });
        PackageViewModel target = viewModel.SelectedPackage!;

        viewModel.RemovePackageCommand.Execute(null);

        Assert.DoesNotContain(target, viewModel.Packages);
        Assert.Contains(store.Deleted, package => ReferenceEquals(package, target.Model));
    }

    [Fact]
    public void RemovePackageCommand_DoesNothing_WhenNotConfirmed()
    {
        MainViewModel viewModel = CreateViewModel(out FakePackageStore store, dialog: new FakeDialogService { ConfirmRemove = false });
        int originalCount = viewModel.Packages.Count;

        viewModel.RemovePackageCommand.Execute(null);

        Assert.Equal(originalCount, viewModel.Packages.Count);
        Assert.Empty(store.Deleted);
    }

    [Fact]
    public void UpdateCurrentPackage_AddsOnlyInstalledAndMissingApps()
    {
        FakeInstalledAppScanner scanner = new() { InstalledIds = ["Git.Git", "Microsoft.PowerShell"] };
        MainViewModel viewModel = CreateViewModel(scanner: scanner);
        PackageViewModel package = viewModel.SelectedPackage!;

        // The demo package already contains Microsoft.PowerShell, so it must not be added again.
        int beforeCount = package.Apps.Count;

        viewModel.UpdateCurrentPackageCommand.Execute(null);

        Assert.Equal(beforeCount + 1, package.Apps.Count);
        Assert.Contains(package.Model.Apps, app => app.Id == "Git.Git");
        Assert.Equal(1, package.Model.Apps.Count(app => app.Id == "Microsoft.PowerShell"));
    }

    [Fact]
    public void AddAppCommand_PersistsSelectedPackage()
    {
        MainViewModel viewModel = CreateViewModel(out FakePackageStore store);
        store.Saved.Clear();

        viewModel.AddAppCommand.Execute(null);

        Assert.NotEmpty(store.Saved);
    }

    private static MainViewModel CreateViewModel()
        => CreateViewModel(store: new FakePackageStore());

    private static MainViewModel CreateViewModel(out FakePackageStore store, FakeDialogService? dialog = null)
    {
        store = new FakePackageStore();
        return CreateViewModel(store: store, dialog: dialog);
    }

    private static MainViewModel CreateViewModel(
        FakePackageStore? store = null,
        FakeInstalledAppScanner? scanner = null,
        FakeDialogService? dialog = null)
        => new(
            new HardcodedCatalogService(),
            new ConsoleService(),
            new FakeVisualStudioDiscoveryService(),
            store ?? new FakePackageStore(),
            scanner ?? new FakeInstalledAppScanner(),
            dialog ?? new FakeDialogService());

    private sealed class FakePackageStore : IPackageStore
    {
        public List<WingetPackage> Saved { get; } = [];

        public List<WingetPackage> Deleted { get; } = [];

        public IReadOnlyList<WingetPackage> Initial { get; init; } = [];

        public IReadOnlyList<WingetPackage> LoadAll() => Initial;

        public void Save(WingetPackage package) => Saved.Add(package);

        public void Delete(WingetPackage package) => Deleted.Add(package);
    }

    private sealed class FakeInstalledAppScanner : IInstalledAppScanner
    {
        public IReadOnlyList<string> InstalledIds { get; init; } = [];

        public IReadOnlyList<string> GetInstalledWingetIds() => InstalledIds;
    }

    private sealed class FakeDialogService : IPackageEditorDialogService
    {
        public Func<IReadOnlyList<WingetPackage>, NewFromExistingResult?>? NewFromExisting { get; init; }

        public bool ConfirmRemove { get; init; } = true;

        public Func<IReadOnlyList<AppEntry>, AppEntry?>? PickApp { get; init; }

        public NewFromExistingResult? AskNewFromExisting(IReadOnlyList<WingetPackage> existingPackages)
            => NewFromExisting?.Invoke(existingPackages);

        public bool ConfirmRemovePackage(string packageName) => ConfirmRemove;

        public AppEntry? PickAndConfigureApp(IReadOnlyList<AppEntry> wellKnownApps)
            => PickApp is not null
                ? PickApp(wellKnownApps)
                : WingetPackage.Clone(new WingetPackage { Apps = [wellKnownApps[0]] }).Apps[0];
    }

    private sealed class FakeVisualStudioDiscoveryService : IVisualStudioDiscoveryService
    {
        public IReadOnlyList<VisualStudioInstanceInfo> DiscoverInstances() =>
        [
            new VisualStudioInstanceInfo(
                InstanceId: "480e759d",
                DisplayName: "Visual Studio Enterprise 2026",
                Year: "2026",
                Edition: "Enterprise",
                Channel: VisualStudioChannel.Preview,
                ChannelId: "VisualStudio.18.Preview",
                Version: "18.7.11822.327",
                ShortVersion: "18.0",
                InstallDate: new DateTimeOffset(2025, 12, 9, 22, 35, 13, TimeSpan.Zero),
                InstallationPath: @"C:\Program Files\Microsoft Visual Studio\18\Insiders",
                ProductId: "Microsoft.VisualStudio.Product.Enterprise",
                IsPrerelease: true,
                Hives:
                [
                    new VisualStudioHiveInfo(
                        "18.0_480e759d",
                        @"C:\Users\demo\AppData\Local\Microsoft\VisualStudio\18.0_480e759d",
                        @"C:\Users\demo\AppData\Local\Microsoft\VisualStudio\18.0_480e759d\Settings\CurrentSettings.vssettings",
                        IsExperimental: false),
                    new VisualStudioHiveInfo(
                        "18.0_480e759dExp",
                        @"C:\Users\demo\AppData\Local\Microsoft\VisualStudio\18.0_480e759dExp",
                        @"C:\Users\demo\AppData\Local\Microsoft\VisualStudio\18.0_480e759dExp\Settings\CurrentSettings.vssettings",
                        IsExperimental: true)
                ])
        ];
    }
}
