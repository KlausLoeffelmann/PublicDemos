using System.Diagnostics.CodeAnalysis;
using LayoutTests.App.Carrier;
using LayoutTests.App.Designer;
using LayoutTests.App.Models;
using LayoutTests.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WarpToolkit.ComponentModel;

namespace LayoutTests.App;

public partial class MainForm : Form, IServiceProvider
{
    private const string MainSplitterSettingsKey = "MainForm.MainSplitter";
    private const string WindowSizeSettingsKey = "MainForm.WindowSize";
    private const string WindowStateSettingsKey = "MainForm.WindowState";

    private IServiceProvider _serviceProvider = null!;
    private ProbeSetStore? _store;
    private IUserSettingsService? _settings;
    private UselessFacts? _facts;
    private ILogger<MainForm>? _logger;

    private ProbeSet _probeSet = new();
    private string? _currentPath;
    private bool _isDirty;
    private bool _suppressEditorEvents;

    public MainForm()
    {
        InitializeComponent();
        WireEvents();
        UpdateCommandState();
    }

    public MainForm(IServiceProvider serviceProvider) : this()
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = new DeferredServiceProvider(serviceProvider);
        EnsureServices();
    }

    object IServiceProvider.GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (_serviceProvider is null)
        {
            throw new InvalidOperationException(
                "MainForm was constructed without a DI service provider. Resolve it from WinFormsApplication instead of calling new MainForm().");
        }

        return _serviceProvider.GetService(serviceType)
            ?? throw new InvalidOperationException(
                $"Service of type '{serviceType.Name}' is not registered.");
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EnsureServices();
        ApplyPersistedUiSettings();

        containerPropertyPanel.AttachServices(_facts!);

        RebindTree();
        ShowFormDefinitionInEditor();
        UpdateCommandState();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!ConfirmDiscardChanges())
        {
            e.Cancel = true;
            return;
        }

        SaveUiSettings();
        base.OnFormClosing(e);
    }

    [MemberNotNull(nameof(_store), nameof(_settings), nameof(_facts), nameof(_logger))]
    private void EnsureServices()
    {
        _store ??= ((IServiceProvider)this).GetService(typeof(ProbeSetStore)) as ProbeSetStore
            ?? throw new InvalidOperationException("ProbeSetStore is not registered.");
        _settings ??= ((IServiceProvider)this).GetService(typeof(IUserSettingsService)) as IUserSettingsService
            ?? throw new InvalidOperationException("IUserSettingsService is not registered.");
        _facts ??= ((IServiceProvider)this).GetService(typeof(UselessFacts)) as UselessFacts
            ?? throw new InvalidOperationException("UselessFacts is not registered.");
        _logger ??= ((IServiceProvider)this).GetService(typeof(ILogger<MainForm>)) as ILogger<MainForm>
            ?? throw new InvalidOperationException("ILogger<MainForm> is not registered.");
    }

    private void WireEvents()
    {
        newProbeSetToolStripMenuItem.Click += (_, _) => NewProbeSet();
        loadProbeSetToolStripMenuItem.Click += async (_, _) => await LoadProbeSetAsync().ConfigureAwait(true);
        saveProbeSetToolStripMenuItem.Click += async (_, _) => await SaveProbeSetAsync(saveAs: false).ConfigureAwait(true);
        saveProbeSetAsToolStripMenuItem.Click += async (_, _) => await SaveProbeSetAsync(saveAs: true).ConfigureAwait(true);
        quitToolStripMenuItem.Click += (_, _) => Close();

        addContainerToolStripMenuItem.Click += (_, _) => AddContainerInteractive();
        removeContainerToolStripMenuItem.Click += (_, _) => RemoveSelectedContainer();

        newProbeSetButton.Click += (_, _) => NewProbeSet();
        loadProbeSetButton.Click += async (_, _) => await LoadProbeSetAsync().ConfigureAwait(true);
        saveProbeSetButton.Click += async (_, _) => await SaveProbeSetAsync(saveAs: false).ConfigureAwait(true);

        addCtorContainerButton.Click += (_, _) => AddContainer(ContainerKind.CTor);
        addLazyContainerButton.Click += (_, _) => AddContainer(ContainerKind.Lazy);
        removeContainerButton.Click += (_, _) => RemoveSelectedContainer();
        actionButton.Click += (_, _) => RunAction();

        probeTreeView.AfterSelect += (_, _) => ShowSelectionInEditor();

        containerPropertyPanel.ContainerParametersChanged += OnEditorContainerChanged;
        containerPropertyPanel.FormDefinitionChanged += OnEditorFormChanged;
    }

    private void ApplyPersistedUiSettings()
    {
        int splitter = _settings!.Get(MainSplitterSettingsKey, 320);
        if (splitter > 0 && splitter < mainSplitContainer.Width - 50)
        {
            mainSplitContainer.SplitterDistance = splitter;
        }

        Size storedSize = _settings.Get(WindowSizeSettingsKey, Size.Empty);
        if (storedSize.Width >= MinimumSize.Width && storedSize.Height >= MinimumSize.Height)
        {
            Size = storedSize;
        }

        FormWindowState storedState = _settings.Get(WindowStateSettingsKey, FormWindowState.Normal);
        if (storedState != FormWindowState.Minimized)
        {
            WindowState = storedState;
        }
    }

    private void SaveUiSettings()
    {
        if (_settings is null)
        {
            return;
        }

        _settings.Set(MainSplitterSettingsKey, mainSplitContainer.SplitterDistance);
        if (WindowState == FormWindowState.Normal)
        {
            _settings.Set(WindowSizeSettingsKey, Size);
        }

        _settings.Set(WindowStateSettingsKey, WindowState);
        _settings.Flush();
    }

    private void NewProbeSet()
    {
        if (!ConfirmDiscardChanges())
        {
            return;
        }

        _probeSet = new ProbeSet { Name = "Untitled" };
        _currentPath = null;
        MarkClean();
        RebindTree();
        ShowFormDefinitionInEditor();
        UpdateCommandState();
        SetStatus("New probe set created.");
    }

    private async Task LoadProbeSetAsync()
    {
        if (!ConfirmDiscardChanges())
        {
            return;
        }

        using var dialog = new OpenFileDialog
        {
            Filter = "Probe Set (*.probeset.json)|*.probeset.json|JSON (*.json)|*.json|All files (*.*)|*.*",
            Title = "Load Probe Set",
        };

        if (!string.IsNullOrEmpty(_store?.LastOpenedPath) && File.Exists(_store.LastOpenedPath))
        {
            dialog.InitialDirectory = Path.GetDirectoryName(_store.LastOpenedPath);
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _probeSet = await _store!.LoadAsync(dialog.FileName).ConfigureAwait(true);
            _currentPath = dialog.FileName;
            MarkClean();
            RebindTree();
            ShowFormDefinitionInEditor();
            UpdateCommandState();
            SetStatus($"Loaded '{Path.GetFileName(dialog.FileName)}'.");
        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, "Failed to load probe set from {Path}", dialog.FileName);
            MessageBox.Show(this, ex.Message, "Load Probe Set", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SaveProbeSetAsync(bool saveAs)
    {
        string? path = _currentPath;
        if (saveAs || string.IsNullOrEmpty(path))
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Probe Set (*.probeset.json)|*.probeset.json|JSON (*.json)|*.json",
                Title = "Save Probe Set",
                FileName = (_probeSet.Name ?? "Untitled") + ".probeset.json",
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            path = dialog.FileName;
        }

        try
        {
            await _store!.SaveAsync(_probeSet, path!).ConfigureAwait(true);
            _currentPath = path;
            MarkClean();
            UpdateCommandState();
            SetStatus($"Saved '{Path.GetFileName(path!)}'.");
        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, "Failed to save probe set to {Path}", path);
            MessageBox.Show(this, ex.Message, "Save Probe Set", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AddContainerInteractive()
    {
        using var dialog = new AddContainerDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        AddContainer(dialog.Kind, dialog.ContainerName);
    }

    private void AddContainer(ContainerKind kind, string? name = null)
    {
        var def = new ContainerDefinition
        {
            Name = name ?? $"{kind}Container",
            Kind = kind,
        };

        var parent = probeTreeView.SelectedContainer;
        if (parent is null)
        {
            _probeSet.Roots.Add(def);
        }
        else
        {
            parent.Children.Add(def);
        }

        MarkDirty();
        RebindTree();
        probeTreeView.SelectContainer(def);
        UpdateCommandState();
        SetStatus($"Added {kind} container '{def.Name}'.");
    }

    private void RemoveSelectedContainer()
    {
        var selected = probeTreeView.SelectedContainer;
        if (selected is null)
        {
            return;
        }

        if (MessageBox.Show(
                this,
                $"Remove container '{selected.Name}' and all its children?",
                "Remove Container",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning) != DialogResult.OK)
        {
            return;
        }

        if (!TryRemove(_probeSet.Roots, selected))
        {
            return;
        }

        MarkDirty();
        RebindTree();
        ShowFormDefinitionInEditor();
        UpdateCommandState();
        SetStatus($"Removed container '{selected.Name}'.");
    }

    private static bool TryRemove(List<ContainerDefinition> list, ContainerDefinition target)
    {
        if (list.Remove(target))
        {
            return true;
        }

        foreach (var item in list)
        {
            if (TryRemove(item.Children, target))
            {
                return true;
            }
        }

        return false;
    }

    private void RunAction()
    {
        if (!HasAnyContainers(_probeSet.Roots))
        {
            MessageBox.Show(
                this,
                "Add at least one container before running the action.",
                "No containers",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            var carrier = new CarrierForm(_probeSet);
            carrier.Show(this);
            SetStatus("Carrier form launched.");
        }
        catch (Exception ex)
        {
            _logger!.LogError(ex, "Failed to launch carrier form.");
            MessageBox.Show(this, ex.Message, "Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static bool HasAnyContainers(List<ContainerDefinition> list)
    {
        if (list.Count > 0)
        {
            return true;
        }

        foreach (var item in list)
        {
            if (HasAnyContainers(item.Children))
            {
                return true;
            }
        }

        return false;
    }

    private void RebindTree() => probeTreeView.Bind(_probeSet);

    private void ShowSelectionInEditor()
    {
        var selected = probeTreeView.SelectedContainer;

        _suppressEditorEvents = true;
        try
        {
            if (selected is null)
            {
                containerPropertyPanel.ShowFormDefinition(_probeSet.Form);
            }
            else
            {
                containerPropertyPanel.ShowContainer(selected);
            }
        }
        finally
        {
            _suppressEditorEvents = false;
        }

        UpdateCommandState();
    }

    private void ShowFormDefinitionInEditor()
    {
        _suppressEditorEvents = true;
        try
        {
            containerPropertyPanel.ShowFormDefinition(_probeSet.Form);
        }
        finally
        {
            _suppressEditorEvents = false;
        }
    }

    private void OnEditorContainerChanged(object? sender, EventArgs e)
    {
        if (_suppressEditorEvents)
        {
            return;
        }

        probeTreeView.RefreshSelectedNodeText();
        MarkDirty();
    }

    private void OnEditorFormChanged(object? sender, EventArgs e)
    {
        if (_suppressEditorEvents)
        {
            return;
        }

        probeTreeView.RefreshRootNodeText();
        MarkDirty();
    }

    private void UpdateCommandState()
    {
        bool hasSelection = probeTreeView.SelectedContainer is not null;
        removeContainerToolStripMenuItem.Enabled = hasSelection;
        removeContainerButton.Enabled = hasSelection;
        saveProbeSetToolStripMenuItem.Enabled = _isDirty || string.IsNullOrEmpty(_currentPath);
        saveProbeSetButton.Enabled = saveProbeSetToolStripMenuItem.Enabled;
        actionButton.Enabled = HasAnyContainers(_probeSet.Roots);

        string title = "Layout Tests — Probe Set Designer";
        string name = string.IsNullOrEmpty(_currentPath) ? "Untitled" : Path.GetFileName(_currentPath);
        Text = _isDirty ? $"{title} — {name} *" : $"{title} — {name}";
    }

    private void MarkDirty()
    {
        _isDirty = true;
        UpdateCommandState();
    }

    private void MarkClean()
    {
        _isDirty = false;
        UpdateCommandState();
    }

    private bool ConfirmDiscardChanges()
    {
        if (!_isDirty)
        {
            return true;
        }

        var result = MessageBox.Show(
            this,
            "The current probe set has unsaved changes. Discard them?",
            "Unsaved changes",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Warning);

        return result == DialogResult.OK;
    }

    private void SetStatus(string message) => statusLabel.Text = message;

    private sealed class DeferredServiceProvider : IServiceProvider
    {
        private readonly Func<IServiceProvider> _serviceProviderFactory;

        public DeferredServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProviderFactory = () => serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            return _serviceProviderFactory().GetService(serviceType)
                ?? throw new InvalidOperationException(
                    $"Service of type '{serviceType.Name}' is not registered.");
        }
    }
}
