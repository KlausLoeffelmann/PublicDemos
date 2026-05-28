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
        MainViewModel viewModel = new(new HardcodedCatalogService(), console);

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
        MainViewModel viewModel = new(new HardcodedCatalogService(), console);
        int originalCount = console.Messages.Count;

        viewModel.OptionsCommand.Execute(null);

        Assert.Equal(originalCount + 1, console.Messages.Count);
        Assert.Equal(ConsoleMessageKind.Command, console.Messages.Last().Kind);
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

    private static MainViewModel CreateViewModel()
        => new(new HardcodedCatalogService(), new ConsoleService());
}
