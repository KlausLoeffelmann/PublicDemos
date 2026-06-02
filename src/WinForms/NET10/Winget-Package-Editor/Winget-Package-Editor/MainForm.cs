using Microsoft.Extensions.DependencyInjection;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using WarpToolkit.ComponentModel;
using WarpToolkit.WinForms.Extensions.UI;
using WarpToolkit.WinForms.Specialized;
using WarpToolkit.WinForms.Symbols;
using WingetPackageEditor.Core.Services;
using WingetPackageEditor.Core.ViewModels;
using CoreConsoleMessageKind = WingetPackageEditor.Core.Services.ConsoleMessageKind;

namespace Winget_Package_Editor;

public partial class MainForm : Form, IServiceProvider
{
    private static readonly string SettingsKey_MainFormBounds
        = nameof(SettingsKey_MainFormBounds);

    private const string SettingsKey_MainFormWindowState = "MainForm.WindowState";
    private const string SettingsKey_MainSplitter = "MainForm.MainSplitter";
    private const string SettingsKey_RightSplitter = "MainForm.RightSplitter";
    private const string SettingsKey_AppGridColumns = "MainForm.AppGrid.Columns";
    private const string SettingsKey_VisualStudioInstanceGridColumns = "MainForm.VisualStudioInstanceGrid.Columns";
    private const string VisualStudioDataPathColumnName = "_visualStudioDataPathColumn";
    private const string SettingsKey_TreeExpansion = "MainForm.TreeExpansion";
    private const string SettingsKey_FontFamily = "MainForm.FontFamily";
    private const string SettingsKey_MenuStripFontSize = "MainForm.MenuStripFontSize";
    private const string SettingsKey_StandardFontSize = "MainForm.StandardFontSize";
    private const string SettingsKey_TreeMainNodeDelta = "MainForm.TreeMainNodeDelta";
    private const string SettingsKey_TreeMainNodeBold = "MainForm.TreeMainNodeBold";
    private const string SettingsKey_StatusStripFontSize = "MainForm.StatusStripFontSize";

    private UiFontSettings _fontSettings = new();
    private string? _currentGridColumnSettingsKey;
    private bool _layoutStateRestored;

    private readonly MainViewModel? _viewModel;
    private ObservableBindingList<AppEntryViewModel>? _appsBindingList;
    private TreeViewBinder? _treeViewBinder;
    private GridSelectionBinder? _gridSelectionBinder;

    private readonly IUserSettingsService? _userSettingsService;
    private readonly IServiceProvider? _serviceProvider;

    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="MainForm"/> class with dependency injection support.
    /// </summary>
    /// <param name="serviceProvider">
    ///  The service provider that contains all registered services for dependency injection.
    ///  This parameter is used to resolve dependencies and configure the form with the required services.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  Thrown when <paramref name="serviceProvider"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    ///  Thrown when the required <see cref="IUserSettingsService"/> is not registered in the service provider.
    /// </exception>
    /// <remarks>
    ///  This constructor overload is specifically designed to be used when the Form is instantiated 
    ///  through Dependency Injection (DI) using the <c>WinFormsApplication</c> class and the 
    ///  <c>WinFormsApplicationBuilder</c>. This approach provides the same infrastructure pattern 
    ///  as ASP.NET Core applications, enabling familiar service registration, configuration, 
    ///  and dependency injection patterns in WinForms applications.
    ///  <para>
    ///   When using this constructor, the Form acts as a ServiceProvider-aware component, 
    ///   allowing it to resolve and utilize services that have been registered in the 
    ///   application's service container. This enables loose coupling, testability, 
    ///   and modern application architecture patterns in WinForms development.
    ///  </para>
    ///  <para>
    ///   The constructor automatically assigns the service provider to the form using the 
    ///   <c>AssignServiceProvider</c> extension method and resolves the required 
    ///   <see cref="IUserSettingsService"/> from the container.
    ///  </para>
    /// </remarks>
    public MainForm(IServiceProvider serviceProvider) : this()
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        _serviceProvider = new DeferredServiceProvider(serviceProvider);

        _userSettingsService = serviceProvider.GetRequiredService<IUserSettingsService>();
        _viewModel = serviceProvider.GetRequiredService<MainViewModel>();

        if (_userSettingsService is null)
        {
            throw new NullReferenceException($"The service '{nameof(IUserSettingsService)}' is not registered.");
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_viewModel is not null)
        {
            InitializeViewModel(_viewModel);
        }

        // Bounds must be restored AFTER fonts have been applied (in InitializeViewModel).
        // With AutoScaleMode.Font, applying a font rescales the form and would otherwise
        // clobber any previously restored size/position.
        RestoreWindowBounds();
    }

    object IServiceProvider.GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType, nameof(serviceType));

        if (_serviceProvider is null)
        {
            throw new InvalidOperationException("Service provider is not initialized.");
        }

        return _serviceProvider.GetService(serviceType)
            ?? throw new InvalidOperationException($"Service of type '{serviceType.Name}' is not registered.");
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // Splitter distances depend on the final laid-out size of the (possibly nested)
        // SplitContainers, which is only settled once the form has been shown. Restoring
        // earlier lets the clamp logic silently reject otherwise-valid saved distances.
        RestoreLayoutState();
    }

    private void RestoreWindowBounds()
    {
        if (_userSettingsService is null)
        {
            return;
        }

        if (!_userSettingsService.TryApplyFormBounds(this, SettingsKey_MainFormBounds))
        {
            Bounds = this.CenterToScreen(
                horizontalFillGrade: 70,
                verticalFillGrade: 70);
        }

        if (_userSettingsService.TryGet(SettingsKey_MainFormWindowState, out FormWindowState windowState)
            && windowState == FormWindowState.Maximized)
        {
            WindowState = FormWindowState.Maximized;
        }
    }

    private void RestoreLayoutState()
    {
        if (_layoutStateRestored || _userSettingsService is null)
        {
            return;
        }

        _layoutStateRestored = true;
        _userSettingsService.TryApplySplitterDistance(_mainSplitContainer, SettingsKey_MainSplitter);
        _userSettingsService.TryApplySplitterDistance(_rightSplitContainer, SettingsKey_RightSplitter);
        _treeViewBinder?.RestoreExpandedNodeKeys(_userSettingsService.Get<string[]>(SettingsKey_TreeExpansion, []));
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        if (_userSettingsService is null)
        {
            return;
        }

        _userSettingsService.SaveFormBounds(this, SettingsKey_MainFormBounds);
        _userSettingsService.Set(SettingsKey_MainFormWindowState, WindowState);
        _userSettingsService.SaveSplitterDistance(_mainSplitContainer, SettingsKey_MainSplitter);
        _userSettingsService.SaveSplitterDistance(_rightSplitContainer, SettingsKey_RightSplitter);
        SaveCurrentGridColumnWidths();
        _userSettingsService.Set(SettingsKey_TreeExpansion, _treeViewBinder?.GetExpandedNodeKeys() ?? []);
        _userSettingsService.Flush();
    }

    private void InitializeViewModel(MainViewModel viewModel)
    {
        DataContext = viewModel;
        SetupCommands(viewModel);
        SetupGrid(viewModel);
        SetupTree(viewModel);
        SetupConsole(viewModel);
        viewModel.ViewCommandRequested += ViewModel_ViewCommandRequested;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _fontSettings = LoadFontSettings();
        ApplyFontSettings(_fontSettings);
        _statusLabel.Text = viewModel.StatusText;
    }

    private void SetupCommands(MainViewModel viewModel)
    {
        ConfigureCommand(_newMenuItem, viewModel.NewPackageCommand);
        ConfigureCommand(_newFromExistingMenuItem, viewModel.NewFromExistingPackageCommand);
        ConfigureCommand(_removePackageMenuItem, viewModel.RemovePackageCommand);
        ConfigureCommand(_openMenuItem, viewModel.OpenPackageCommand);
        ConfigureCommand(_exportMenuItem, viewModel.ExportCommand);
        ConfigureCommand(_quitMenuItem, viewModel.QuitCommand);
        ConfigureCommand(_addAppMenuItem, viewModel.AddAppCommand);
        ConfigureCommand(_removeAppMenuItem, viewModel.RemoveAppCommand);
        ConfigureCommand(_propertiesMenuItem, viewModel.PropertiesCommand);
        ConfigureCommand(_expandNodesMenuItem, viewModel.ExpandAllNodesCommand);
        ConfigureCommand(_collapseNodeMenuItem, viewModel.CollapseSelectedNodeCommand);
        ConfigureCommand(_expandSelectedMenuItem, viewModel.ExpandSelectedNodeCommand);
        ConfigureCommand(_updatePackageMenuItem, viewModel.UpdateCurrentPackageCommand);
        ConfigureCommand(_applyNowMenuItem, viewModel.ApplyNowCommand);
        ConfigureCommand(_generateBundleFolderMenuItem, viewModel.GenerateBundleFolderCommand);
        ConfigureCommand(_optionsMenuItem, viewModel.OptionsCommand);

        ConfigureCommand(_newToolStripButton, viewModel.NewPackageCommand);
        ConfigureCommand(_addAppToolStripButton, viewModel.AddAppCommand);
        ConfigureCommand(_removeAppToolStripButton, viewModel.RemoveAppCommand);
        ConfigureCommand(_exportToolStripButton, viewModel.ExportCommand);
        ConfigureCommand(_applyNowToolStripButton, viewModel.ApplyNowCommand);

        _newToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.New, tooltipText: "New package", size: 36);
        _addAppToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.AddBold, tooltipText: "Add app", size: 36);
        _removeAppToolStripButton.ConfigureItem(FluentSymbols.CommonToolStripSymbols.Delete, tooltipText: "Remove app", size: 36);
        _exportToolStripButton.ConfigureItem(FluentSymbols.AllSymbols.Export, tooltipText: "Export YAML+Script", size: 36);
        _applyNowToolStripButton.ConfigureItem(FluentSymbols.AllSymbols.Play, tooltipText: "Apply now", size: 36);
    }

    private static void ConfigureCommand(ToolStripItem item, ICommand command)
    {
        item.Command = command;
    }

    private void SetupGrid(MainViewModel viewModel)
    {
        _appsBindingList = new ObservableBindingList<AppEntryViewModel>(viewModel.CurrentApps);
        _gridSelectionBinder = new GridSelectionBinder(_appsWarpDataGridView, viewModel);
        _appsWarpDataGridView.CellFormatting += AppsGrid_CellFormatting;
        _appsWarpDataGridView.CellContentClick += AppsGrid_CellContentClick;
        _appsWarpDataGridView.CellMouseDown += AppsGrid_CellMouseDown;
        ConfigureGridForSelectedNode(viewModel);
    }

    private static DataGridViewTextBoxColumn CreateTextColumn(string propertyName, string headerText, int width, bool readOnly = false)
    {
        return new DataGridViewTextBoxColumn
        {
            DataPropertyName = propertyName,
            HeaderText = headerText,
            Name = $"_{propertyName}Column",
            ReadOnly = readOnly,
            Width = width
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
            CoreConsoleMessageKind.Error => Color.IndianRed,
            CoreConsoleMessageKind.Warning => Color.Goldenrod,
            CoreConsoleMessageKind.Command => Color.LightSkyBlue,
            CoreConsoleMessageKind.Debug => Color.Gray,
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

    private void ViewModel_ViewCommandRequested(object? sender, ViewCommandKind e)
    {
        switch (e)
        {
            case ViewCommandKind.ExpandAllNodes:
                _treeViewBinder?.ExpandAll();
                break;
            case ViewCommandKind.CollapseSelectedNode:
                _treeViewBinder?.CollapseSelected();
                break;
            case ViewCommandKind.ExpandSelectedNode:
                _treeViewBinder?.ExpandSelected();
                break;
            case ViewCommandKind.ShowOptions:
                ShowOptionsDialog();
                break;
        }
    }

    private void ShowOptionsDialog()
    {
        using OptionsDialog dialog = new(_fontSettings);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _fontSettings = dialog.Settings;
        SaveFontSettings(_fontSettings);
        ApplyFontSettings(_fontSettings);
    }

    private UiFontSettings LoadFontSettings()
    {
        if (_userSettingsService is null)
        {
            return new UiFontSettings();
        }

        return new UiFontSettings
        {
            FontFamily = _userSettingsService.Get(SettingsKey_FontFamily, "Segoe UI"),
            MenuStripSize = _userSettingsService.Get(SettingsKey_MenuStripFontSize, 11F),
            StandardSize = _userSettingsService.Get(SettingsKey_StandardFontSize, 10F),
            TreeMainNodeDelta = _userSettingsService.Get(SettingsKey_TreeMainNodeDelta, 1F),
            TreeMainNodeBold = _userSettingsService.Get(SettingsKey_TreeMainNodeBold, true),
            StatusStripSize = _userSettingsService.Get(SettingsKey_StatusStripFontSize, 10F)
        };
    }

    private void SaveFontSettings(UiFontSettings settings)
    {
        if (_userSettingsService is null)
        {
            return;
        }

        _userSettingsService.Set(SettingsKey_FontFamily, settings.FontFamily);
        _userSettingsService.Set(SettingsKey_MenuStripFontSize, settings.MenuStripSize);
        _userSettingsService.Set(SettingsKey_StandardFontSize, settings.StandardSize);
        _userSettingsService.Set(SettingsKey_TreeMainNodeDelta, settings.TreeMainNodeDelta);
        _userSettingsService.Set(SettingsKey_TreeMainNodeBold, settings.TreeMainNodeBold);
        _userSettingsService.Set(SettingsKey_StatusStripFontSize, settings.StatusStripSize);
        _userSettingsService.Flush();
    }

    private void ApplyFontSettings(UiFontSettings settings)
    {
        Font standardFont = new(settings.FontFamily, settings.StandardSize, FontStyle.Regular, GraphicsUnit.Point);
        Font menuFont = new(settings.FontFamily, settings.MenuStripSize, FontStyle.Regular, GraphicsUnit.Point);
        Font statusFont = new(settings.FontFamily, settings.StatusStripSize, FontStyle.Regular, GraphicsUnit.Point);
        Font treeRootFont = new(
            settings.FontFamily,
            settings.StandardSize + settings.TreeMainNodeDelta,
            settings.TreeMainNodeBold ? FontStyle.Bold : FontStyle.Regular,
            GraphicsUnit.Point);

        Font = standardFont;
        _mainMenuStrip.Font = menuFont;
        _mainToolStrip.Font = standardFont;
        _packageTreeView.Font = standardFont;
        _appsWarpDataGridView.Font = standardFont;
        _consoleControl.Font = standardFont;
        _statusStrip.Font = statusFont;
        _treeViewBinder?.SetRootNodeFont(treeRootFont);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.StatusText) && sender is MainViewModel viewModel)
        {
            _statusLabel.Text = viewModel.StatusText;
        }
        else if (e.PropertyName == nameof(MainViewModel.SelectedNavigationNode) && sender is MainViewModel vm)
        {
            ConfigureGridForSelectedNode(vm);
            _treeViewBinder?.SelectNode(vm.SelectedNavigationNode);
        }
    }

    private void ConfigureGridForSelectedNode(MainViewModel viewModel)
    {
        IReadOnlyList<VisualStudioInstallationRowViewModel>? rows = viewModel.SelectedNavigationNode?.Value switch
        {
            VisualStudioBranchViewModel branch => branch.Rows,
            VisualStudioVersionViewModel version => version.Rows,
            VisualStudioSkuComboViewModel combo => combo.Rows,
            VisualStudioInstanceViewModel instance => instance.Rows,
            _ => null
        };

        if (rows is not null)
        {
            ConfigureVisualStudioInstanceGrid(rows);
            return;
        }

        ConfigureAppGrid();
    }

    private void ConfigureAppGrid()
    {
        ConfigureGrid(
            SettingsKey_AppGridColumns,
            _appsBindingList,
            () =>
            {
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.DisplayName), "Display Name", width: 220));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Id), "WinGet Id", width: 260));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.EntryType), "Type", width: 120, readOnly: true));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Source), "Source", width: 100));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Scope), "Scope", width: 100));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.Version), "Version", width: 120));
                _appsWarpDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    DataPropertyName = nameof(AppEntryViewModel.AllowPrerelease),
                    HeaderText = "Prerelease",
                    Name = "_allowPrereleaseColumn",
                    Width = 90
                });
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(AppEntryViewModel.ExtensionsSummary), "Extensions", width: 170, readOnly: true));
            });
    }

    private void ConfigureVisualStudioInstanceGrid(IReadOnlyList<VisualStudioInstallationRowViewModel> rows)
    {
        ConfigureGrid(
            SettingsKey_VisualStudioInstanceGridColumns,
            rows,
            () =>
            {
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(VisualStudioInstallationRowViewModel.SkuName), "SKU Name", width: 240, readOnly: true));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(VisualStudioInstallationRowViewModel.Version), "Version", width: 130, readOnly: true));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(VisualStudioInstallationRowViewModel.InstallDateDisplay), "Install Date", width: 150, readOnly: true));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(VisualStudioInstallationRowViewModel.InstanceId), "Instance ID", width: 110, readOnly: true));
                _appsWarpDataGridView.Columns.Add(CreateTextColumn(nameof(VisualStudioInstallationRowViewModel.InstallationPathDisplay), "Install Path", width: 200, readOnly: true));
                _appsWarpDataGridView.Columns.Add(new DataGridViewButtonColumn
                {
                    DataPropertyName = nameof(VisualStudioInstallationRowViewModel.DataPathDisplay),
                    HeaderText = "Path to Data",
                    Name = VisualStudioDataPathColumnName,
                    Width = 200,
                    UseColumnTextForButtonValue = false
                });
            });
    }

    private VisualStudioInstallationRowViewModel? GetVisualStudioRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _appsWarpDataGridView.Rows.Count)
        {
            return null;
        }

        return _appsWarpDataGridView.Rows[rowIndex].DataBoundItem as VisualStudioInstallationRowViewModel;
    }

    private void AppsGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (GetVisualStudioRow(e.RowIndex) is not { IsExperimental: true })
        {
            return;
        }

        // Experimental-hive rows are rendered in a muted gray: light-light-gray over the
        // dark theme background, dark-dark-gray over the classic (light) background.
        e.CellStyle.ForeColor = Application.IsDarkModeEnabled
            ? Color.FromArgb(190, 190, 190)
            : Color.FromArgb(90, 90, 90);
    }

    private void AppsGrid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        if (!string.Equals(_appsWarpDataGridView.Columns[e.ColumnIndex].Name, VisualStudioDataPathColumnName, StringComparison.Ordinal))
        {
            return;
        }

        if (GetVisualStudioRow(e.RowIndex) is { } row)
        {
            OpenInExplorer(row.DataPath);
        }
    }

    private void AppsGrid_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right || e.RowIndex < 0)
        {
            return;
        }

        if (GetVisualStudioRow(e.RowIndex) is not { } row)
        {
            return;
        }

        _appsWarpDataGridView.ClearSelection();
        _appsWarpDataGridView.Rows[e.RowIndex].Selected = true;
        _appsWarpDataGridView.CurrentCell = _appsWarpDataGridView.Rows[e.RowIndex].Cells[Math.Max(0, e.ColumnIndex)];
        ShowVisualStudioContextMenu(row);
    }

    private void ShowVisualStudioContextMenu(VisualStudioInstallationRowViewModel row)
    {
        ContextMenuStrip menu = new();
        menu.Closed += (_, _) => menu.Dispose();

        bool hasDataPath = !string.IsNullOrEmpty(row.DataPath);

        ToolStripMenuItem openInstall = (ToolStripMenuItem)menu.Items.Add("Open Explorer Install Path");
        openInstall.ConfigureItem(FluentSymbols.AllSymbols.FolderOpen,
            (clickHandler: (_, _) => OpenInExplorer(row.InstallationPath), removeBeforeAdd: false));

        ToolStripMenuItem openData = (ToolStripMenuItem)menu.Items.Add("Open Explorer Data Path");
        openData.Enabled = hasDataPath;
        openData.ConfigureItem(FluentSymbols.AllSymbols.Folder,
            (clickHandler: (_, _) => OpenInExplorer(row.DataPath), removeBeforeAdd: false));

        menu.Items.Add(new ToolStripSeparator());

        ToolStripMenuItem copyVersion = (ToolStripMenuItem)menu.Items.Add("Copy Version Info to Clipboard");
        copyVersion.ConfigureItem(FluentSymbols.AllSymbols.ClipboardList,
            (clickHandler: (_, _) => SetClipboardText($"{row.SkuName} {row.Version} ({row.InstanceId})"), removeBeforeAdd: false));

        ToolStripMenuItem copyInstall = (ToolStripMenuItem)menu.Items.Add("Copy Install Path to Clipboard");
        copyInstall.ConfigureItem(FluentSymbols.AllSymbols.Copy,
            (clickHandler: (_, _) => SetClipboardText(row.InstallationPath), removeBeforeAdd: false));

        ToolStripMenuItem copyData = (ToolStripMenuItem)menu.Items.Add("Copy Data Path to Clipboard");
        copyData.Enabled = hasDataPath;
        copyData.ConfigureItem(FluentSymbols.AllSymbols.Copy,
            (clickHandler: (_, _) => SetClipboardText(row.DataPath), removeBeforeAdd: false));

        menu.Items.Add(new ToolStripSeparator());

        ToolStripMenuItem enableUnsigned = (ToolStripMenuItem)menu.Items.Add("Enable running unsigned .NET Runtimes");
        enableUnsigned.Click += (_, _) => EnableUnsignedDotnetRuntimes(row);

        menu.Show(Cursor.Position);
    }

    private void OpenInExplorer(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            _viewModel?.WriteConsole(CoreConsoleMessageKind.Error, $"Path not found: {path}");
            return;
        }

        try
        {
            using System.Diagnostics.Process? _ = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _viewModel?.WriteConsole(CoreConsoleMessageKind.Error, $"Could not open '{path}': {ex.Message}");
        }
    }

    private void SetClipboardText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        try
        {
            Clipboard.SetText(text);
        }
        catch (Exception ex)
        {
            _viewModel?.WriteConsole(CoreConsoleMessageKind.Error, $"Clipboard error: {ex.Message}");
        }
    }

    private void EnableUnsignedDotnetRuntimes(VisualStudioInstallationRowViewModel row)
    {
        string vsRegEdit = Path.Combine(row.InstallationPath, "Common7", "IDE", "VsRegEdit.exe");
        if (!File.Exists(vsRegEdit))
        {
            _viewModel?.WriteConsole(CoreConsoleMessageKind.Error, $"VsRegEdit.exe not found at: {vsRegEdit}");
            return;
        }

        System.Diagnostics.ProcessStartInfo startInfo = new()
        {
            FileName = vsRegEdit,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        foreach (string argument in new[]
                 {
                     "set", "local", row.HiveName, "HKCU",
                     @"Debugger\EngineSwitches", "ValidateDotnetDebugLibSignatures", "dword", "0"
                 })
        {
            startInfo.ArgumentList.Add(argument);
        }

        _viewModel?.WriteConsole(CoreConsoleMessageKind.Info,
            $"Enabling unsigned .NET runtimes for hive {row.HiveName}...");

        try
        {
            using System.Diagnostics.Process process = new() { StartInfo = startInfo };
            process.OutputDataReceived += (_, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    _viewModel?.WriteConsole(CoreConsoleMessageKind.Info, args.Data);
                }
            };
            process.ErrorDataReceived += (_, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    _viewModel?.WriteConsole(CoreConsoleMessageKind.Error, args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            _viewModel?.WriteConsole(
                process.ExitCode == 0 ? CoreConsoleMessageKind.Info : CoreConsoleMessageKind.Error,
                $"VsRegEdit.exe exited with code {process.ExitCode}.");
        }
        catch (Exception ex)
        {
            _viewModel?.WriteConsole(CoreConsoleMessageKind.Error, $"VsRegEdit.exe failed: {ex.Message}");
        }
    }

    private void ConfigureGrid(string settingsKey, object? dataSource, Action configureColumns)
    {
        if (string.Equals(_currentGridColumnSettingsKey, settingsKey, StringComparison.Ordinal)
            && ReferenceEquals(_appsWarpDataGridView.DataSource, dataSource))
        {
            return;
        }

        SaveCurrentGridColumnWidths();
        _appsWarpDataGridView.DataSource = null;
        _appsWarpDataGridView.AutoGenerateColumns = false;
        _appsWarpDataGridView.Columns.Clear();
        configureColumns();
        _appsWarpDataGridView.DataSource = dataSource;
        _currentGridColumnSettingsKey = settingsKey;
        _userSettingsService?.TryApplyDataGridViewColumnWidths(_appsWarpDataGridView, settingsKey);
    }

    private void SaveCurrentGridColumnWidths()
    {
        if (_userSettingsService is null || string.IsNullOrWhiteSpace(_currentGridColumnSettingsKey))
        {
            return;
        }

        _userSettingsService.SaveDataGridViewColumnWidths(_appsWarpDataGridView, _currentGridColumnSettingsKey);
    }
}
