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
    private const string SettingsKeyTreeExpandedEntries = "WinBaas.Tree.ExpandedEntries";
    private const string SettingsKeyTreeExpandedCategories = "WinBaas.Tree.ExpandedCategories";
    private const string SettingsKeyTreeSelectedEntry = "WinBaas.Tree.SelectedEntry";

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
        ApplyColorMode();
        RestoreWindowState();
        PopulateSourceTree();
        RestoreTreeState();
        _logger.LogInformation("WinBaas ready.");
    }

    /// <summary>
    ///  Re-colors the controls that don't auto-theme under WARP's
    ///  <see cref="SystemColorMode"/>. Currently: the DataGridView column
    ///  header band, which keeps a bright fallback style otherwise.
    /// </summary>
    private void ApplyColorMode()
    {
        bool dark = Application.IsDarkModeEnabled;

        _grid.EnableHeadersVisualStyles = false;
        _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = dark
            ? Color.FromArgb(0x2D, 0x2D, 0x30)
            : SystemColors.Control;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = dark
            ? Color.Gainsboro
            : SystemColors.ControlText;
        _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor =
            _grid.ColumnHeadersDefaultCellStyle.BackColor;
        _grid.ColumnHeadersDefaultCellStyle.SelectionForeColor =
            _grid.ColumnHeadersDefaultCellStyle.ForeColor;
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font(_grid.Font, FontStyle.Regular);

        _grid.BackgroundColor = dark ? Color.FromArgb(0x1E, 0x1E, 0x1E) : SystemColors.Window;
        _grid.GridColor = dark ? Color.FromArgb(0x3F, 0x3F, 0x46) : SystemColors.ControlDark;
        _grid.DefaultCellStyle.BackColor = dark ? Color.FromArgb(0x25, 0x25, 0x26) : SystemColors.Window;
        _grid.DefaultCellStyle.ForeColor = dark ? Color.Gainsboro : SystemColors.WindowText;
        _grid.DefaultCellStyle.SelectionBackColor = dark
            ? Color.FromArgb(0x37, 0x37, 0x3D)
            : SystemColors.Highlight;
        _grid.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
    }

    private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
    {
        PersistTreeState();
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
            _nodeItems.Clear();

            var grouped = _catalog.GetAll().GroupBy(e => e.Category ?? string.Empty);
            foreach (var group in grouped.OrderBy(g => CategoryOrder(g.Key)))
            {
                var categoryNode = new TreeNode(group.Key)
                {
                    NodeFont = new Font(_treeSources.Font, FontStyle.Bold),
                    Tag = new CategoryTag(group.Key),
                };

                foreach (CatalogEntry entry in group.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var leaf = new TreeNode(entry.Name)
                    {
                        Tag = entry,
                        ToolTipText = entry.Description,
                    };
                    categoryNode.Nodes.Add(leaf);
                }

                _treeSources.Nodes.Add(categoryNode);
            }
        }
        finally
        {
            _treeSources.EndUpdate();
        }
    }

    /// <summary>
    ///  Marker that a tree node represents a category root (not a leaf entry).
    /// </summary>
    private sealed record CategoryTag(string Name);

    private static int CategoryOrder(string category) => category switch
    {
        "AI Tools" => 0,
        "Developer Tools" => 1,
        "Creator / Design / Photo" => 2,
        "Musician / Audio" => 3,
        "System" => 4,
        "User" => 5,
        _ => 100,
    };

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

    private void RestoreTreeState()
    {
        string expandedCats = _settings.Get(SettingsKeyTreeExpandedCategories, string.Empty);
        var expandedCatSet = expandedCats
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        string expandedEntries = _settings.Get(SettingsKeyTreeExpandedEntries, string.Empty);
        var expandedEntrySet = expandedEntries
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToHashSet();

        Guid selectedEntry = Guid.TryParse(_settings.Get(SettingsKeyTreeSelectedEntry, string.Empty), out var sel)
            ? sel
            : Guid.Empty;

        foreach (TreeNode node in _treeSources.Nodes)
        {
            if (node.Tag is CategoryTag cat && expandedCatSet.Contains(cat.Name))
            {
                node.Expand();
            }

            foreach (TreeNode child in node.Nodes)
            {
                if (child.Tag is CatalogEntry e && expandedEntrySet.Contains(e.Id))
                {
                    child.Expand();
                }
            }
        }

        if (selectedEntry != Guid.Empty)
        {
            TreeNode? match = EnumerateLeafNodes(_treeSources.Nodes)
                .FirstOrDefault(n => n.Tag is CatalogEntry e && e.Id == selectedEntry);
            if (match is not null)
            {
                _treeSources.SelectedNode = match;
                match.EnsureVisible();
            }
        }
    }

    private void PersistTreeState()
    {
        try
        {
            var expandedCategories = new List<string>();
            var expandedEntries = new List<string>();
            foreach (TreeNode node in _treeSources.Nodes)
            {
                if (node.Tag is CategoryTag cat && node.IsExpanded)
                {
                    expandedCategories.Add(cat.Name);
                }

                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Tag is CatalogEntry e && child.IsExpanded)
                    {
                        expandedEntries.Add(e.Id.ToString());
                    }
                }
            }

            _settings.Set(SettingsKeyTreeExpandedCategories, string.Join('|', expandedCategories));
            _settings.Set(SettingsKeyTreeExpandedEntries, string.Join(',', expandedEntries));
            _settings.Set(
                SettingsKeyTreeSelectedEntry,
                _treeSources.SelectedNode?.Tag is CatalogEntry sel ? sel.Id.ToString() : string.Empty);
            _settings.Flush();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not persist tree state.");
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
        public Task<BackupResult> BackupAsync(IReadOnlyList<DiscoveredItem> items, BackupOptions options, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
            => Task.FromResult(new BackupResult(options.Destination, options.Destination, Guid.Empty, 0));
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
