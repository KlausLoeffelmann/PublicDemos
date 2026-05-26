using Microsoft.Extensions.Logging;
using WarpToolkit.Desktop.AppServices;
using WinBaas.Models;
using WinBaas.Services;

namespace WinBaas;

/// <summary>
///  The main WinBaas window: MenuStrip + ToolStrip on top, SplitContainer
///  (TreeView | DataGridView+FluentTabControl) in the middle, StatusStrip at
///  the bottom.
/// </summary>
public sealed partial class FrmMain : Form
{
    private const string SettingsKeyBounds = "FrmMain.Bounds";
    private const string SettingsKeyWindowState = "FrmMain.WindowState";
    private const string SettingsKeySplitOuter = "FrmMain.SplitOuter";
    private const string SettingsKeySplitInner = "FrmMain.SplitInner";

    private readonly IServiceProvider _serviceProvider;
    private readonly ICatalogService _catalog;
    private readonly IDiscoveryService _discovery;
    private readonly IBackupService _backup;
    private readonly ConsoleLoggerSink _consoleSink;
    private readonly ILogger<FrmMain> _logger;
    private readonly IWinFormsAppExceptionService _exceptionService;
    private readonly WarpToolkit.ComponentModel.IUserSettingsService _settings;

    private readonly Dictionary<TreeNode, List<DiscoveredItem>> _nodeItems = new();
    private bool _syncing;

    /// <summary>
    ///  DI-aware constructor. <see cref="_serviceProvider"/> is assigned <em>before</em>
    ///  <see cref="InitializeComponent"/> per the WinForms Designer rules.
    /// </summary>
    public FrmMain(
        IServiceProvider serviceProvider,
        ICatalogService catalog,
        IDiscoveryService discovery,
        IBackupService backup,
        ConsoleLoggerSink consoleSink,
        IWinFormsAppExceptionService exceptionService,
        WarpToolkit.ComponentModel.IUserSettingsService settings,
        ILogger<FrmMain> logger)
    {
        _serviceProvider = serviceProvider;
        _catalog = catalog;
        _discovery = discovery;
        _backup = backup;
        _consoleSink = consoleSink;
        _exceptionService = exceptionService;
        _settings = settings;
        _logger = logger;

        InitializeComponent();

        Load += FrmMain_Load;
        FormClosing += FrmMain_FormClosing;
        _treeSources.AfterSelect += TreeSources_AfterSelect;
        _treeSources.AfterCheck += TreeSources_AfterCheck;
        _grid.CellValueChanged += Grid_CellValueChanged;
        _grid.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
        _grid.SelectionChanged += Grid_SelectionChanged;
        _menuFileExit.Click += (_, _) => Close();
    }

    /// <summary>
    ///  Parameterless constructor for the WinForms Designer. Not used at runtime.
    /// </summary>
    public FrmMain()
        : this(
            new ServiceContainer(),
            new DesignTimeCatalog(),
            new DesignTimeDiscovery(),
            new DesignTimeBackup(),
            new ConsoleLoggerSink(),
            new DesignTimeExceptionService(),
            new DesignTimeSettings(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<FrmMain>.Instance)
    {
    }

    private void FrmMain_Load(object? sender, EventArgs e)
    {
        _exceptionService.RegisterExceptionHandler(OnUnhandledThreadException);
        _consoleSink.Attach(_console);
        ConfigureCommands();
        RestoreWindowState();
        PopulateSourceTree();
        _logger.LogInformation("WinBaas ready.");
    }

    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        PersistWindowState();
        _exceptionService.UnregisterExceptionHandler(OnUnhandledThreadException);
    }

    private void OnUnhandledThreadException(object? sender, System.Threading.ThreadExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Unhandled UI exception.");
        MessageBox.Show(
            this,
            e.Exception.Message,
            "WinBaas - Unexpected error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private void PopulateSourceTree()
    {
        _treeSources.BeginUpdate();
        try
        {
            _treeSources.Nodes.Clear();
            foreach (CatalogEntry entry in _catalog.GetAll())
            {
                var node = new TreeNode(entry.Name)
                {
                    Tag = entry,
                    ToolTipText = entry.Description,
                };
                _treeSources.Nodes.Add(node);
            }
        }
        finally
        {
            _treeSources.EndUpdate();
        }
    }

    private void RestoreWindowState()
    {
        if (_settings.TryGet<Rectangle>(SettingsKeyBounds, out var bounds) && bounds.Width > 200 && bounds.Height > 200)
        {
            StartPosition = FormStartPosition.Manual;
            Bounds = bounds;
        }

        if (_settings.TryGet<FormWindowState>(SettingsKeyWindowState, out var state) && state != FormWindowState.Minimized)
        {
            WindowState = state;
        }

        if (_settings.TryGet<int>(SettingsKeySplitOuter, out int outerDistance) && outerDistance > 100)
        {
            _splitOuter.SplitterDistance = outerDistance;
        }

        if (_settings.TryGet<int>(SettingsKeySplitInner, out int innerDistance) && innerDistance > 100)
        {
            _splitInner.SplitterDistance = innerDistance;
        }
    }

    private void PersistWindowState()
    {
        try
        {
            _settings.Set(SettingsKeyWindowState, WindowState);
            _settings.Set(SettingsKeyBounds, WindowState == FormWindowState.Normal ? Bounds : RestoreBounds);
            _settings.Set(SettingsKeySplitOuter, _splitOuter.SplitterDistance);
            _settings.Set(SettingsKeySplitInner, _splitInner.SplitterDistance);
            _settings.Flush();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not persist window state.");
        }
    }

    // --- Designer-only stand-ins --------------------------------------------------

    private sealed class ServiceContainer : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private sealed class DesignTimeCatalog : ICatalogService
    {
        public IReadOnlyList<CatalogEntry> GetAll() => [];
        public void Add(CatalogEntry entry) { }
        public bool Remove(Guid id) => false;
        public void RestoreDefaults() { }
        public void Save() { }
    }

    private sealed class DesignTimeDiscovery : IDiscoveryService
    {
        public Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(CatalogEntry entry, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DiscoveredItem>>([]);
    }

    private sealed class DesignTimeBackup : IBackupService
    {
        public Task BackupAsync(IReadOnlyList<DiscoveredItem> items, BackupOptions options, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class DesignTimeExceptionService : IWinFormsAppExceptionService
    {
        public void RegisterExceptionHandler(ThreadExceptionEventHandler threadExceptionEventHandler) { }
        public void UnregisterExceptionHandler(ThreadExceptionEventHandler threadExceptionEventHandler) { }
    }

    private sealed class DesignTimeSettings : WarpToolkit.ComponentModel.IUserSettingsService
    {
        public T Get<T>(string key, T defaultValue) => defaultValue;
        public bool TryGet<T>(string key, out T value) { value = default!; return false; }
        public void Set<T>(string key, T value) { }
        public bool Remove(string key) => false;
        public bool Contains(string key) => false;
        public void Clear() { }
        public void Flush() { }
    }
}
