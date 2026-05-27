using System.Diagnostics.CodeAnalysis;
using BranchComposer.App.Models;
using BranchComposer.App.Services;
using Microsoft.Extensions.DependencyInjection;
using WarpToolkit.ComponentModel;
using WarpToolkit.WinForms.Extensions.UI;
using WarpToolkit.WinForms.Github.Git;

namespace BranchComposer.App;

public partial class MainForm : Form, IServiceProvider
{
    private const string MainSplitterSettingsKey = "MainForm.MainSplitter";
    private const string BranchSetSplitterSettingsKey = "MainForm.BranchSetSplitter";
    private const string BranchSetGridSettingsKey = "MainForm.BranchSetGrid";
    private const string GitConsoleVisibleSettingsKey = "MainForm.GitConsoleVisible";

    private IServiceProvider _serviceProvider = null!;
    private AppStateStore? _stateStore;
    private ILocalGitRepositoryService? _repositoryService;
    private IGitBranchCompositionService? _branchCompositionService;
    private GitConsoleService? _gitConsoleService;
    private IUserSettingsService? _userSettingsService;
    private AppState _state = new();
    private TreeNode? _githubReposRootNode;
    private GitConsoleView? _gitConsoleView;
    private bool _gitConsoleTabInitialized;

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

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        await RunUiActionAsync(async () =>
        {
            EnsureServices();
            InitializeGitConsoleTab();
            ApplyPersistedUiSettings();
            if (_gitConsoleView is not null)
            {
                await _gitConsoleService.AttachAsync(_gitConsoleView.Console).ConfigureAwait(true);
            }

            _state = await _stateStore.LoadAsync().ConfigureAwait(true);
            RenderRepositories();
            RestoreSelection();
            UpdateCommandState();
        }).ConfigureAwait(true);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SaveUiSettings();
        base.OnFormClosing(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (_gitConsoleView is not null && _gitConsoleService is not null)
        {
            _gitConsoleService.Detach(_gitConsoleView.Console);
        }

        base.OnFormClosed(e);
    }

    private void WireEvents()
    {
        addGithubRepoToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(AddRepositoryAsync).ConfigureAwait(true);
        removeGithubRepoToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(RemoveRepositoryAsync).ConfigureAwait(true);
        quitToolStripMenuItem.Click += (_, _) => Close();
        createBranchSetToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(CreateBranchSetAsync).ConfigureAwait(true);
        deleteBranchSetToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(DeleteBranchSetAsync).ConfigureAwait(true);
        composeBranchSetToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(ComposeBranchSetAsync).ConfigureAwait(true);
        gitConsoleToolStripMenuItem.Click += (_, _) => SetGitConsoleVisible(gitConsoleToolStripMenuItem.Checked, persist: true);
        repositoryTreeView.AfterSelect += (_, _) =>
        {
            RenderBranchSets();
            UpdateSelectedRepositoryStatus();
            UpdateCommandState();
        };
        branchSetDataGridView.SelectionChanged += async (_, _) =>
        {
            UpdateCommandState();
            await RunUiActionAsync(UpdateSelectedBranchStatusAsync).ConfigureAwait(true);
        };
        branchSetDataGridView.ColumnHeaderMouseClick += (_, e) =>
        {
            SetStatus($"Column '{branchSetDataGridView.Columns[e.ColumnIndex].HeaderText}' selected. Sorting is not implemented yet.");
        };
    }

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

    private async Task AddRepositoryAsync()
    {
        EnsureServices();

        using FolderBrowserDialog dialog = new()
        {
            Description = "Choose a directory inside a local GitHub repository.",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        LocalGitRepositoryInfo repositoryInfo = await _repositoryService!.GetRepositoryInfoAsync(dialog.SelectedPath).ConfigureAwait(true);

        if (repositoryInfo.Origin is null)
        {
            throw new InvalidOperationException("The selected repository does not have a parseable GitHub origin remote.");
        }

        RepositoryEntry entry = new()
        {
            RootPath = repositoryInfo.RootPath,
            RepositoryKey = repositoryInfo.Origin.RepositoryKey,
            DisplayName = $"{repositoryInfo.Origin.Owner}/{repositoryInfo.Origin.RepositoryName}",
            RemoteUrl = repositoryInfo.Origin.Url,
            DefaultBranch = repositoryInfo.DefaultBranch
        };

        _state.Repositories.RemoveAll(repo => string.Equals(repo.Key, entry.Key, StringComparison.OrdinalIgnoreCase));
        _state.Repositories.Add(entry);
        _state.Repositories = _state.Repositories.OrderBy(repo => repo.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();
        _state.LastSelectedRepositoryKey = entry.Key;

        await SaveStateAsync().ConfigureAwait(true);
        RenderRepositories();
        SelectRepository(entry.Key);
        SetStatus($"Added {entry.DisplayName}.");
    }

    private async Task RemoveRepositoryAsync()
    {
        RepositoryEntry? repository = SelectedRepository;
        if (repository is null)
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            this,
            $"Remove '{repository.DisplayName}' and its saved Branch-Sets from BranchComposer?",
            "Remove Github Repo",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (result != DialogResult.Yes)
        {
            return;
        }

        _state.Repositories.RemoveAll(repo => string.Equals(repo.Key, repository.Key, StringComparison.OrdinalIgnoreCase));
        _state.BranchSetsByRepository.Remove(repository.Key);
        _state.LastSelectedRepositoryKey = null;
        _state.LastSelectedBranchSetName = null;

        await SaveStateAsync().ConfigureAwait(true);
        RenderRepositories();
        RenderBranchSets();
        SetStatus($"Removed {repository.DisplayName}.");
    }

    private async Task CreateBranchSetAsync()
    {
        EnsureServices();

        RepositoryEntry repository = SelectedRepository
            ?? throw new InvalidOperationException("Select a Github repo before creating a Branch-Set.");

        IReadOnlyList<GitBranchInfo> branches = await _repositoryService!.GetBranchesAsync(repository.RootPath).ConfigureAwait(true);

        using BranchSetEditorDialog dialog = new(repository, branches, repository.DefaultBranch, _repositoryService, _userSettingsService);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Definition is null)
        {
            return;
        }

        BranchSetDefinition definition = dialog.Definition;
        definition.RepositoryKey = repository.Key;

        List<BranchSetDefinition> branchSets = GetBranchSets(repository.Key);
        branchSets.RemoveAll(branchSet => string.Equals(branchSet.Name, definition.Name, StringComparison.OrdinalIgnoreCase));
        branchSets.Add(definition);
        branchSets.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase));

        _state.LastSelectedBranchSetName = definition.Name;
        await SaveStateAsync().ConfigureAwait(true);
        RenderBranchSets();
        SelectBranchSet(definition.Name);
    }

    private async Task DeleteBranchSetAsync()
    {
        RepositoryEntry? repository = SelectedRepository;
        BranchSetDefinition? branchSet = SelectedBranchSet;

        if (repository is null || branchSet is null)
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            this,
            $"Delete the saved Branch-Set definition '{branchSet.Name}'? This will not delete any real git branches.",
            "Delete Branch-Set",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (result != DialogResult.Yes)
        {
            return;
        }

        GetBranchSets(repository.Key).Remove(branchSet);
        _state.LastSelectedBranchSetName = null;

        await SaveStateAsync().ConfigureAwait(true);
        RenderBranchSets();
        SetStatus($"Deleted Branch-Set '{branchSet.Name}'.");
    }

    private async Task ComposeBranchSetAsync()
    {
        EnsureServices();

        RepositoryEntry repository = SelectedRepository
            ?? throw new InvalidOperationException("Select a Github repo before composing.");

        BranchSetDefinition branchSet = SelectedBranchSet
            ?? throw new InvalidOperationException("Select a Branch-Set before composing.");

        DialogResult result = MessageBox.Show(
            this,
            $"Compose Branch-Set '{branchSet.Name}' and push the target branch?\n\nThis may update a remote branch when overwrite is enabled.",
            "Compose Branch-Set",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (result != DialogResult.Yes)
        {
            return;
        }

        BranchCompositionResult compositionResult = await _branchCompositionService!.ComposeAsync(new BranchCompositionRequest
        {
            RepositoryPath = repository.RootPath,
            BaseBranch = branchSet.BaseBranch,
            SourceBranches = branchSet.SourceBranches,
            TargetOptions = new BranchTargetOptions
            {
                BranchSetName = branchSet.Name,
                TargetBranchName = branchSet.TargetBranchName,
                NamingMode = branchSet.NamingMode,
                NumberWidth = branchSet.NumberWidth,
                OverwriteExisting = branchSet.OverwriteExisting
            }
        }).ConfigureAwait(true);

        SetStatus($"Composed {compositionResult.TargetBranch} @ {compositionResult.NewSha[..Math.Min(12, compositionResult.NewSha.Length)]}.");

        MessageBox.Show(
            this,
            $"Composed and pushed '{compositionResult.TargetBranch}'.\n\nReplayed commits: {compositionResult.ReplayedCommits.Count}",
            "Compose Branch-Set",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private async Task UpdateSelectedBranchStatusAsync()
    {
        if (_repositoryService is null || SelectedRepository is not { } repository)
        {
            SetStatus("No repository selected.");
            return;
        }

        if (SelectedBranchSet is not { } branchSet)
        {
            UpdateSelectedRepositoryStatus();
            return;
        }

        string? branchName = branchSet.SourceBranches.FirstOrDefault();
        if (branchName is null)
        {
            SetStatus("No source branch selected.");
            return;
        }

        GitCommitInfo commit = await _repositoryService.GetBranchTipAsync(repository.RootPath, branchName).ConfigureAwait(true);
        SetStatus($"{branchName}: {commit.AuthorDate.LocalDateTime:g} | {commit.AbbreviatedSha} | {commit.Subject} | {commit.Sha}");
    }

    private void RenderRepositories()
    {
        repositoryTreeView.BeginUpdate();
        repositoryTreeView.Nodes.Clear();
        _githubReposRootNode = new TreeNode("Github Repos")
        {
            Name = "githubReposRootNode"
        };

        foreach (RepositoryEntry repository in _state.Repositories)
        {
            TreeNode node = new(repository.DisplayName)
            {
                Tag = repository
            };
            node.ToolTipText = repository.RootPath;
            _githubReposRootNode.Nodes.Add(node);
        }

        repositoryTreeView.Nodes.Add(_githubReposRootNode);
        _githubReposRootNode.Expand();
        repositoryTreeView.EndUpdate();
    }

    private void RenderBranchSets()
    {
        branchSetDataGridView.SuspendLayout();
        branchSetDataGridView.Rows.Clear();

        if (SelectedRepository is { } repository)
        {
            foreach (BranchSetDefinition branchSet in GetBranchSets(repository.Key))
            {
                int rowIndex = branchSetDataGridView.Rows.Add(
                    branchSet.Name,
                    branchSet.BaseBranch,
                    string.Join(", ", branchSet.SourceBranches),
                    $"{branchSet.Name}/{branchSet.TargetBranchName}");

                branchSetDataGridView.Rows[rowIndex].Tag = branchSet;
            }
        }

        branchSetDataGridView.ClearSelection();
        branchSetDataGridView.CurrentCell = null;
        branchSetDataGridView.ResumeLayout();
        UpdateCommandState();
    }

    private void RestoreSelection()
    {
        if (_state.LastSelectedRepositoryKey is not null)
        {
            SelectRepository(_state.LastSelectedRepositoryKey);
        }

        if (_state.LastSelectedBranchSetName is not null)
        {
            SelectBranchSet(_state.LastSelectedBranchSetName);
        }
    }

    private void SelectRepository(string repositoryKey)
    {
        if (_githubReposRootNode is null)
        {
            return;
        }

        foreach (TreeNode node in _githubReposRootNode.Nodes)
        {
            if (node.Tag is RepositoryEntry repository && string.Equals(repository.Key, repositoryKey, StringComparison.OrdinalIgnoreCase))
            {
                repositoryTreeView.SelectedNode = node;
                node.EnsureVisible();
                break;
            }
        }
    }

    private void SelectBranchSet(string branchSetName)
    {
        foreach (DataGridViewRow row in branchSetDataGridView.Rows)
        {
            if (row.Tag is BranchSetDefinition branchSet && string.Equals(branchSet.Name, branchSetName, StringComparison.OrdinalIgnoreCase))
            {
                row.Selected = true;
                branchSetDataGridView.CurrentCell = row.Cells[0];
                branchSetDataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                break;
            }
        }
    }

    private List<BranchSetDefinition> GetBranchSets(string repositoryKey)
    {
        if (!_state.BranchSetsByRepository.TryGetValue(repositoryKey, out List<BranchSetDefinition>? branchSets))
        {
            branchSets = [];
            _state.BranchSetsByRepository[repositoryKey] = branchSets;
        }

        return branchSets;
    }

    private void InitializeGitConsoleTab()
    {
        if (_gitConsoleTabInitialized)
        {
            return;
        }

        _gitConsoleView = new GitConsoleView();
        gitConsoleTabControl.AddTab("Git Console", _gitConsoleView);
        _gitConsoleTabInitialized = true;
    }

    private void ApplyPersistedUiSettings()
    {
        if (_userSettingsService is null)
        {
            return;
        }

        _userSettingsService.TryApplySplitterDistance(mainSplitContainer, MainSplitterSettingsKey);
        _userSettingsService.TryApplySplitterDistance(branchSetSplitContainer, BranchSetSplitterSettingsKey);
        _userSettingsService.TryApplyDataGridViewColumnWidths(branchSetDataGridView, BranchSetGridSettingsKey);
        bool gitConsoleVisible = _userSettingsService.Get(GitConsoleVisibleSettingsKey, true);
        SetGitConsoleVisible(gitConsoleVisible, persist: false);
    }

    private void SaveUiSettings()
    {
        if (_userSettingsService is null)
        {
            return;
        }

        _userSettingsService.SaveSplitterDistance(mainSplitContainer, MainSplitterSettingsKey);
        _userSettingsService.SaveSplitterDistance(branchSetSplitContainer, BranchSetSplitterSettingsKey);
        _userSettingsService.SaveDataGridViewColumnWidths(branchSetDataGridView, BranchSetGridSettingsKey);
        _userSettingsService.Set(GitConsoleVisibleSettingsKey, !branchSetSplitContainer.Panel2Collapsed);
    }

    private void SetGitConsoleVisible(bool visible, bool persist)
    {
        gitConsoleToolStripMenuItem.Checked = visible;
        branchSetSplitContainer.Panel2Collapsed = !visible;

        if (persist && _userSettingsService is not null)
        {
            _userSettingsService.Set(GitConsoleVisibleSettingsKey, visible);
        }
    }

    private void UpdateSelectedRepositoryStatus()
    {
        if (SelectedRepository is not { } repository)
        {
            SetStatus("No repository selected.");
            return;
        }

        string defaultBranch = string.IsNullOrWhiteSpace(repository.DefaultBranch)
            ? "unknown default branch"
            : repository.DefaultBranch;

        SetStatus($"{repository.DisplayName} | {defaultBranch} | {repository.RootPath} | {repository.RemoteUrl}");
    }

    private void SetStatus(string text)
    {
        selectedBranchStatusLabel.Text = text;
        selectedBranchStatusLabel.ToolTipText = text;
    }

    private void UpdateCommandState()
    {
        bool hasRepository = SelectedRepository is not null;
        bool hasBranchSet = SelectedBranchSet is not null;

        removeGithubRepoToolStripMenuItem.Enabled = hasRepository;
        createBranchSetToolStripMenuItem.Enabled = hasRepository;
        deleteBranchSetToolStripMenuItem.Enabled = hasBranchSet;
        composeBranchSetToolStripMenuItem.Enabled = hasBranchSet;
    }

    private async Task SaveStateAsync()
    {
        if (_stateStore is null)
        {
            return;
        }

        _state.LastSelectedRepositoryKey = SelectedRepository?.Key ?? _state.LastSelectedRepositoryKey;
        _state.LastSelectedBranchSetName = SelectedBranchSet?.Name ?? _state.LastSelectedBranchSetName;

        await _stateStore.SaveAsync(_state).ConfigureAwait(true);
    }

    private async Task RunUiActionAsync(Func<Task> action)
    {
        try
        {
            UseWaitCursor = true;
            await action().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, FormatException(ex), "BranchComposer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus(ex.Message);
        }
        finally
        {
            UseWaitCursor = false;
            UpdateCommandState();
        }
    }

    private static string FormatException(Exception exception)
        => exception switch
        {
            BranchCompositionException branchException when branchException.ConflictedFiles.Count > 0
                => $"{branchException.Message}\n\nConflicted files:\n{string.Join(Environment.NewLine, branchException.ConflictedFiles)}",
            GitCommandException gitException
                => $"{gitException.Message}\n\n{gitException.StandardError}\n{gitException.StandardOutput}".Trim(),
            _ => exception.Message
        };

    [MemberNotNull(nameof(_stateStore), nameof(_repositoryService), nameof(_branchCompositionService), nameof(_gitConsoleService), nameof(_userSettingsService))]
    private void EnsureServices()
    {
        if (_stateStore is not null
            && _repositoryService is not null
            && _branchCompositionService is not null
            && _gitConsoleService is not null
            && _userSettingsService is not null)
        {
            return;
        }

        IServiceProvider serviceProvider = _serviceProvider
            ?? throw new InvalidOperationException("MainForm must be resolved from WinFormsApplication so DI can provide the service provider.");

        _stateStore = serviceProvider.GetRequiredService<AppStateStore>();
        _repositoryService = serviceProvider.GetRequiredService<ILocalGitRepositoryService>();
        _branchCompositionService = serviceProvider.GetRequiredService<IGitBranchCompositionService>();
        _gitConsoleService = serviceProvider.GetRequiredService<GitConsoleService>();
        _userSettingsService = serviceProvider.GetRequiredService<IUserSettingsService>();
    }

    private RepositoryEntry? SelectedRepository
        => repositoryTreeView.SelectedNode?.Tag as RepositoryEntry;

    private BranchSetDefinition? SelectedBranchSet
        => branchSetDataGridView.SelectedRows.Count == 0 ? null : branchSetDataGridView.SelectedRows[0].Tag as BranchSetDefinition;
}
