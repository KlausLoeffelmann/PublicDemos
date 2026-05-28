using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using WarpToolkit.WinForms.Extensions.UI;
using WarpToolkit.WinForms.Symbols;
using WingetPackageEditor.Core.Services;
using WingetPackageEditor.Core.ViewModels;

namespace Winget_Package_Editor;

public partial class MainForm : Form, IServiceProvider
{
    private static readonly string SettingsKey_MainFormBounds
        = nameof(SettingsKey_MainFormBounds);

    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_userSettingsService is not null)
        {
            if (!_userSettingsService.TryApplyFormBounds(this, SettingsKey_MainFormBounds))
            {
                Bounds = this.CenterToScreen(
                    horizontalFillGrade: 70,
                    verticalFillGrade: 70);
            }
        }

        if (_viewModel is not null)
        {
            InitializeViewModel(_viewModel);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (_userSettingsService is null)
        {
            return;
        }

        _userSettingsService.SaveFormBounds(this, SettingsKey_MainFormBounds);
    }

    private void InitializeViewModel(MainViewModel viewModel)
    {
        DataContext = viewModel;
        SetupCommands(viewModel);
        SetupGrid(viewModel);
        SetupTree(viewModel);
        SetupConsole(viewModel);
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _statusLabel.Text = viewModel.StatusText;
    }

    private void SetupCommands(MainViewModel viewModel)
    {
        ConfigureCommand(_newMenuItem, viewModel.NewPackageCommand);
        ConfigureCommand(_openMenuItem, viewModel.OpenPackageCommand);
        ConfigureCommand(_saveMenuItem, viewModel.SavePackageCommand);
        ConfigureCommand(_saveAsMenuItem, viewModel.SavePackageAsCommand);
        ConfigureCommand(_exportMenuItem, viewModel.ExportCommand);
        ConfigureCommand(_quitMenuItem, viewModel.QuitCommand);
        ConfigureCommand(_addAppMenuItem, viewModel.AddAppCommand);
        ConfigureCommand(_removeAppMenuItem, viewModel.RemoveAppCommand);
        ConfigureCommand(_propertiesMenuItem, viewModel.PropertiesCommand);
        ConfigureCommand(_applyNowMenuItem, viewModel.ApplyNowCommand);
        ConfigureCommand(_generateBundleFolderMenuItem, viewModel.GenerateBundleFolderCommand);
        ConfigureCommand(_optionsMenuItem, viewModel.OptionsCommand);

        ConfigureCommand(_newToolStripButton, viewModel.NewPackageCommand);
        ConfigureCommand(_openToolStripButton, viewModel.OpenPackageCommand);
        ConfigureCommand(_saveToolStripButton, viewModel.SavePackageCommand);
        ConfigureCommand(_addAppToolStripButton, viewModel.AddAppCommand);
        ConfigureCommand(_removeAppToolStripButton, viewModel.RemoveAppCommand);
        ConfigureCommand(_exportToolStripButton, viewModel.ExportCommand);
        ConfigureCommand(_applyNowToolStripButton, viewModel.ApplyNowCommand);

        _addAppButton.Command = viewModel.AddAppCommand;
        _removeAppButton.Command = viewModel.RemoveAppCommand;
        _propertiesButton.Command = viewModel.PropertiesCommand;

        _newToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.New, tooltipText: "New package");
        _openToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.Open, tooltipText: "Open package");
        _saveToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.Save, tooltipText: "Save package");
        _addAppToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.AddBold, tooltipText: "Add app");
        _removeAppToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.Delete, tooltipText: "Remove app");
        _exportToolStripButton.ConfigureItem(FluentSymbols.AllSymbols.Export, tooltipText: "Export YAML+Script");
        _applyNowToolStripButton.ConfigureItem(FluentSymbols.AllSymbols.Play, tooltipText: "Apply now");
    }

    private static void ConfigureCommand(ToolStripItem item, ICommand command)
    {
        item.Command = command;
    }

    private void SetupGrid(MainViewModel viewModel)
    {
        _appsWarpDataGridView.AutoGenerateColumns = false;
        _appsWarpDataGridView.Columns.Clear();
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.DisplayName), "Display Name"));
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Id), "WinGet Id"));
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.EntryType), "Type", readOnly: true));
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Source), "Source"));
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Scope), "Scope"));
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Version), "Version"));
        _appsWarpDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(AppEntryViewModel.AllowPrerelease),
            HeaderText = "Prerelease",
            Name = "_allowPrereleaseColumn",
            Width = 90
        });
        _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.ExtensionsSummary), "Extensions", readOnly: true));

        _appsBindingList = new ObservableBindingList<AppEntryViewModel>(viewModel.CurrentApps);
        _appsWarpDataGridView.DataSource = _appsBindingList;
        _gridSelectionBinder = new GridSelectionBinder(_appsWarpDataGridView, viewModel);
    }

    private static DataGridViewTextBoxColumn CreateTextColumn(string propertyName, string headerText, bool readOnly = false)
    {
        return new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = headerText,
            Name = $"_{propertyName}Column",
            ReadOnly = readOnly
        };
    }

    private void SetupTree(MainViewModel viewModel)
    {
        _treeViewBinder = new TreeViewBinder(_packageTreeView, viewModel.NavigationRoots);
        _treeViewBinder.SelectedNodeChanged += (_, selectedNode) => viewModel.SelectedNavigationNode = selectedNode;
        _treeViewBinder.SelectNode(viewModel.SelectedNavigationNode);
    }

    private void SetupConsole(MainViewModel viewModel)
    {
        viewModel.ConsoleMessages.CollectionChanged += ConsoleMessages_CollectionChanged;

        foreach (ConsoleMessage message in viewModel.ConsoleMessages)
        {
            _ = AppendConsoleMessageAsync(message);
        }
    }

    private void ConsoleMessages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null)
        {
            return;
        }

        foreach (ConsoleMessage message in e.NewItems)
        {
            _ = AppendConsoleMessageAsync(message);
        }
    }

    private async Task AppendConsoleMessageAsync(ConsoleMessage message)
    {
        Color color = message.Kind switch
        {
            ConsoleMessageKind.Error => Color.IndianRed,
            ConsoleMessageKind.Warning => Color.Goldenrod,
            ConsoleMessageKind.Command => Color.LightSkyBlue,
            ConsoleMessageKind.Debug => Color.Gray,
            _ => Color.Empty
        };

        string line = $"[{message.Timestamp:HH:mm:ss}] [{message.Kind}] {message.Text}";
        if (IsHandleCreated && InvokeRequired)
        {
            BeginInvoke(new Action(() => _ = AppendConsoleMessageAsync(message)));
            return;
        }

        await _consoleControl.WriteLineAsync(line, color == Color.Empty ? null : color);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.StatusText) && sender is MainViewModel viewModel)
        {
            _statusLabel.Text = viewModel.StatusText;
        }
        else if (e.PropertyName == nameof(MainViewModel.SelectedNavigationNode) && sender is MainViewModel vm)
        {
            _treeViewBinder?.SelectNode(vm.SelectedNavigationNode);
        }
    }
}
