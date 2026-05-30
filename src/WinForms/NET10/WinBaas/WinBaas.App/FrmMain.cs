using Microsoft.Extensions.Logging;
using WarpToolkit.Desktop.AppServices;
using WinBaas.Controls;
using WinBaas.Models;
using WinBaas.Services;

namespace WinBaas;

/// <summary>
///  The main WinBaas window: MenuStrip + ToolStrip on top, SplitContainer
///  (TreeView | pluggable detail view + FluentTabControl) in the middle, and a
///  StatusStrip at the bottom.
/// </summary>
public sealed partial class FrmMain : Form
{
    private const string SettingsKeyBounds = "FrmMain.Bounds";
    private const string SettingsKeyWindowState = "FrmMain.WindowState";
    private const string SettingsKeySplitOuter = "FrmMain.SplitOuter";
    private const string SettingsKeySplitInner = "FrmMain.SplitInner";
    private const string SettingsKeyTreeExpandedCategories = "WinBaas.Tree.ExpandedEntries";
    private const string SettingsKeyTreeExpandedEntries = "WinBaas.Tree.ExpandedNodes";
    private const string SettingsKeyTreeSelectedEntry = "WinBaas.Tree.SelectedEntry";

    private readonly IServiceProvider _serviceProvider;
    private readonly ICatalogService _catalog;
    private readonly IDiscoveryService _discovery;
    private readonly IRegistryDiscovery _registryDiscovery;
    private readonly IVisualStudioDiscovery _visualStudioDiscovery;
    private readonly IBackupService _backup;
    private readonly ConsoleLoggerSink _consoleSink;
    private readonly ILogger<FrmMain> _logger;
    private readonly IWinFormsAppExceptionService _exceptionService;
    private readonly WarpToolkit.ComponentModel.IUserSettingsService _settings;
    private readonly FilesGridControl _filesGridControl;
    private readonly RegistryGridControl _registryGridControl;
    private readonly VsOverviewControl _vsOverviewControl;
    private readonly VsHivesControl _vsHivesControl;
    private readonly VsExtensionsControl _vsExtensionsControl;

    private readonly Dictionary<TreeNode, List<DiscoveredItem>> _nodeItems = new();
    private IReadOnlyList<RegistryDiscoveredItem> _registryItems = [];
    private IReadOnlyList<VsSku> _visualStudioSkus = [];
    private Control? _activeDetailControl;
    private TreeNode? _registryRootNode;
    private TreeNode? _visualStudioRootNode;
    private bool _syncing;

    /// <summary>
    ///  DI-aware constructor. <see cref="_serviceProvider"/> is assigned <em>before</em>
    ///  <see cref="InitializeComponent"/> per the WinForms Designer rules.
    /// </summary>
    public FrmMain(
        IServiceProvider serviceProvider,
        ICatalogService catalog,
        IDiscoveryService discovery,
        IRegistryDiscovery registryDiscovery,
        IVisualStudioDiscovery visualStudioDiscovery,
        IBackupService backup,
        ConsoleLoggerSink consoleSink,
        IWinFormsAppExceptionService exceptionService,
        WarpToolkit.ComponentModel.IUserSettingsService settings,
        ILogger<FrmMain> logger)
    {
        _serviceProvider = serviceProvider;
        _catalog = catalog;
        _discovery = discovery;
        _registryDiscovery = registryDiscovery;
        _visualStudioDiscovery = visualStudioDiscovery;
        _backup = backup;
        _consoleSink = consoleSink;
        _exceptionService = exceptionService;
        _settings = settings;
        _logger = logger;

        InitializeComponent();

        _filesGridControl = new FilesGridControl();
        _registryGridControl = new RegistryGridControl();
        _vsOverviewControl = new VsOverviewControl();
        _vsHivesControl = new VsHivesControl();
        _vsExtensionsControl = new VsExtensionsControl();

        InitializeDetailControls();

        Load += FrmMain_Load;
        FormClosing += FrmMain_FormClosing;
        _treeSources.AfterSelect += TreeSources_AfterSelect;
        _treeSources.AfterCheck += TreeSources_AfterCheck;
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
            new DesignTimeRegistryDiscovery(),
            new DesignTimeVisualStudioDiscovery(),
            new DesignTimeBackup(),
            new ConsoleLoggerSink(),
            new DesignTimeExceptionService(),
            new DesignTimeSettings(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<FrmMain>.Instance)
    {
    }

    private void InitializeDetailControls()
    {
        Control[] controls = [_filesGridControl, _registryGridControl, _vsOverviewControl, _vsHivesControl, _vsExtensionsControl];
        foreach (Control control in controls)
        {
            control.Dock = DockStyle.Fill;
            control.Visible = false;
            _detailHost.Controls.Add(control);
        }

        _filesGridControl.CheckedItemsChanged += FilesGridControl_CheckedItemsChanged;
        _filesGridControl.SelectionSizeChanged += (_, text) => _statusSize.Text = text;
        _registryGridControl.CheckedItemsChanged += RegistryGridControl_CheckedItemsChanged;
        _registryGridControl.StatusTextChanged += (_, text) =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _statusInfo.Text = text;
            }
        };
        _vsHivesControl.StatusTextChanged += (_, text) =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _statusInfo.Text = text;
            }
        };
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
        ShowDetail(_filesGridControl);
        _logger.LogInformation("WinBaas ready.");
    }

    /// <summary>
    ///  Re-colors the controls that do not auto-theme.
    /// </summary>
    private void ApplyColorMode()
    {
        bool dark = Application.IsDarkModeEnabled;
        _filesGridControl.ApplyColorMode(dark);
        _registryGridControl.ApplyColorMode(dark);
        _vsOverviewControl.ApplyColorMode(dark);
        _vsExtensionsControl.ApplyColorMode(dark);
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
            _registryRootNode = null;
            _visualStudioRootNode = null;

            CatalogEntry? registryEntry = null;
            CatalogEntry? visualStudioEntry = null;
            var standardEntries = new List<CatalogEntry>();

            foreach (CatalogEntry entry in _catalog.GetAll())
            {
                switch (entry.Kind)
                {
                    case CatalogEntryKind.Registry:
                        registryEntry = entry;
                        break;
                    case CatalogEntryKind.VisualStudio:
                        visualStudioEntry = entry;
                        break;
                    default:
                        standardEntries.Add(entry);
                        break;
                }
            }

            if (registryEntry is not null)
            {
                _registryRootNode = new TreeNode(registryEntry.Name)
                {
                    Tag = new RegistryGroupTag(registryEntry),
                    ToolTipText = registryEntry.Description,
                };
                _treeSources.Nodes.Add(_registryRootNode);
            }

            if (visualStudioEntry is not null)
            {
                _visualStudioRootNode = new TreeNode(visualStudioEntry.Name)
                {
                    Tag = new VsRootTag(visualStudioEntry),
                    ToolTipText = visualStudioEntry.Description,
                };
                _treeSources.Nodes.Add(_visualStudioRootNode);
            }

            var grouped = standardEntries.GroupBy(entry => entry.Category ?? string.Empty);
            foreach (var group in grouped.OrderBy(group => CategoryOrder(group.Key)))
            {
                var categoryNode = new TreeNode(group.Key)
                {
                    NodeFont = new Font(_treeSources.Font, FontStyle.Bold),
                    Tag = new CategoryTag(group.Key),
                };

                foreach (CatalogEntry entry in group.OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase))
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

    private void PopulateVisualStudioTree()
    {
        if (_visualStudioRootNode is null)
        {
            return;
        }

        _visualStudioRootNode.Nodes.Clear();
        foreach (VsSku sku in _visualStudioSkus)
        {
            var skuNode = new TreeNode(sku.NodeLabel)
            {
                Tag = sku,
                Checked = sku.IsChecked,
                ToolTipText = sku.SettingsPath,
            };

            skuNode.Nodes.Add(new TreeNode("Hives")
            {
                Tag = new VsHivesTag(sku),
                Checked = sku.IsChecked,
            });
            skuNode.Nodes.Add(new TreeNode("Extensions")
            {
                Tag = new VsExtensionsTag(sku),
                Checked = sku.IsChecked,
            });

            _visualStudioRootNode.Nodes.Add(skuNode);
        }

        _visualStudioRootNode.Text = _visualStudioSkus.Count == 0
            ? "Visual Studio"
            : $"Visual Studio ({_visualStudioSkus.Count})";
        _visualStudioRootNode.ForeColor = _visualStudioSkus.Count == 0
            ? SystemColors.GrayText
            : SystemColors.ControlText;
    }

    /// <summary>
    ///  Marker that a tree node represents a category root (not a leaf entry).
    /// </summary>
    private sealed record CategoryTag(string Name);

    /// <summary>A marker for the top-level Registry branch.</summary>
    private sealed record RegistryGroupTag(CatalogEntry Entry);

    /// <summary>A marker for the top-level Visual Studio branch.</summary>
    private sealed record VsRootTag(CatalogEntry Entry);

    /// <summary>A marker for the hives child node of a Visual Studio SKU.</summary>
    private sealed record VsHivesTag(VsSku Sku);

    /// <summary>A marker for the extensions child node of a Visual Studio SKU.</summary>
    private sealed record VsExtensionsTag(VsSku Sku);

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
        string expandedRoots = _settings.Get(SettingsKeyTreeExpandedCategories, string.Empty);
        var expandedRootSet = expandedRoots
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        string expandedNodes = _settings.Get(SettingsKeyTreeExpandedEntries, string.Empty);
        var expandedNodeSet = expandedNodes
            .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        string selectedKey = _settings.Get(SettingsKeyTreeSelectedEntry, string.Empty);

        foreach (TreeNode node in _treeSources.Nodes)
        {
            string? key = GetNodePersistenceKey(node);
            if (!string.IsNullOrEmpty(key) && expandedRootSet.Contains(key))
            {
                node.Expand();
            }

            foreach (TreeNode descendant in EnumerateNodes(node.Nodes))
            {
                string? descendantKey = GetNodePersistenceKey(descendant);
                if (!string.IsNullOrEmpty(descendantKey) && expandedNodeSet.Contains(descendantKey))
                {
                    descendant.Expand();
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(selectedKey))
        {
            TreeNode? match = EnumerateNodes(_treeSources.Nodes)
                .FirstOrDefault(node => string.Equals(GetNodePersistenceKey(node), selectedKey, StringComparison.OrdinalIgnoreCase));
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
            var expandedRoots = new List<string>();
            var expandedNodes = new List<string>();
            foreach (TreeNode node in _treeSources.Nodes)
            {
                string? key = GetNodePersistenceKey(node);
                if (!string.IsNullOrEmpty(key) && node.IsExpanded)
                {
                    expandedRoots.Add(key);
                }

                foreach (TreeNode descendant in EnumerateNodes(node.Nodes))
                {
                    string? descendantKey = GetNodePersistenceKey(descendant);
                    if (!string.IsNullOrEmpty(descendantKey) && descendant.IsExpanded)
                    {
                        expandedNodes.Add(descendantKey);
                    }
                }
            }

            _settings.Set(SettingsKeyTreeExpandedCategories, string.Join('|', expandedRoots));
            _settings.Set(SettingsKeyTreeExpandedEntries, string.Join('|', expandedNodes));
            _settings.Set(SettingsKeyTreeSelectedEntry, GetNodePersistenceKey(_treeSources.SelectedNode));
            _settings.Flush();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not persist tree state.");
        }
    }

    private void ShowDetail(Control control)
    {
        if (ReferenceEquals(_activeDetailControl, control))
        {
            return;
        }

        foreach (Control child in _detailHost.Controls)
        {
            child.Visible = ReferenceEquals(child, control);
        }

        _activeDetailControl = control;
    }

    private static IEnumerable<TreeNode> EnumerateNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            yield return node;
            foreach (TreeNode descendant in EnumerateNodes(node.Nodes))
            {
                yield return descendant;
            }
        }
    }

    private static string? GetNodePersistenceKey(TreeNode? node) => node?.Tag switch
    {
        CategoryTag tag => $"cat:{tag.Name}",
        CatalogEntry entry => $"entry:{entry.Id}",
        RegistryGroupTag _ => "root:registry",
        VsRootTag _ => "root:visualstudio",
        VsSku sku => $"vs:{sku.Key}",
        VsHivesTag hivesTag => $"vs:{hivesTag.Sku.Key}:hives",
        VsExtensionsTag extensionsTag => $"vs:{extensionsTag.Sku.Key}:extensions",
        _ => null,
    };

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

    private sealed class DesignTimeRegistryDiscovery : IRegistryDiscovery
    {
        public Task<IReadOnlyList<RegistryDiscoveredItem>> DiscoverAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<RegistryDiscoveredItem>>([]);
    }

    private sealed class DesignTimeVisualStudioDiscovery : IVisualStudioDiscovery
    {
        public Task<IReadOnlyList<VsSku>> DiscoverAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<VsSku>>([]);
    }

    private sealed class DesignTimeBackup : IBackupService
    {
        public Task<BackupResult> BackupAsync(BackupSelection selection, BackupOptions options, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
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
