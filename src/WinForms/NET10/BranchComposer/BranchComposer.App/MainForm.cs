using System.Diagnostics.CodeAnalysis;
using BranchComposer.App.Models;
using BranchComposer.App.Services;
using WarpToolkit.WinForms.Github.Git;

namespace BranchComposer.App;

public partial class MainForm : Form, IServiceProvider
{
    [AllowNull]
    private readonly IServiceProvider _serviceProvider;

    private readonly AppStateStore? _stateStore;
    private readonly ILocalGitRepositoryService? _repositoryService;
    private readonly IGitBranchCompositionService? _branchCompositionService;
    private AppState _state = new();

    public MainForm()
    {
        InitializeComponent();
        WireEvents();
        UpdateCommandState();
    }

    public MainForm(
        IServiceProvider serviceProvider,
        AppStateStore stateStore,
        ILocalGitRepositoryService repositoryService,
        IGitBranchCompositionService branchCompositionService)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(stateStore);
        ArgumentNullException.ThrowIfNull(repositoryService);
        ArgumentNullException.ThrowIfNull(branchCompositionService);

        _serviceProvider = serviceProvider;
        _stateStore = stateStore;
        _repositoryService = repositoryService;
        _branchCompositionService = branchCompositionService;

        InitializeComponent();
        WireEvents();
        UpdateCommandState();
    }

    object? IServiceProvider.GetService(Type serviceType)
        => _serviceProvider?.GetService(serviceType);

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (_stateStore is null)
        {
            return;
        }

        await RunUiActionAsync(async () =>
        {
            _state = await _stateStore.LoadAsync().ConfigureAwait(true);
            RenderRepositories();
            RestoreSelection();
            UpdateCommandState();
        }).ConfigureAwait(true);
    }

    private void WireEvents()
    {
        addGithubRepoToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(AddRepositoryAsync).ConfigureAwait(true);
        removeGithubRepoToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(RemoveRepositoryAsync).ConfigureAwait(true);
        quitToolStripMenuItem.Click += (_, _) => Close();
        createBranchSetToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(CreateBranchSetAsync).ConfigureAwait(true);
        deleteBranchSetToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(DeleteBranchSetAsync).ConfigureAwait(true);
        composeBranchSetToolStripMenuItem.Click += async (_, _) => await RunUiActionAsync(ComposeBranchSetAsync).ConfigureAwait(true);
        repositoryListView.SelectedIndexChanged += (_, _) =>
        {
            RenderBranchSets();
            UpdateCommandState();
        };
        branchSetListView.SelectedIndexChanged += async (_, _) =>
        {
            UpdateCommandState();
            await RunUiActionAsync(UpdateSelectedBranchStatusAsync).ConfigureAwait(true);
        };
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
        selectedBranchStatusLabel.Text = $"Added {entry.DisplayName}.";
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
        selectedBranchStatusLabel.Text = $"Removed {repository.DisplayName}.";
    }

    private async Task CreateBranchSetAsync()
    {
        EnsureServices();

        RepositoryEntry repository = SelectedRepository
            ?? throw new InvalidOperationException("Select a Github repo before creating a Branch-Set.");

        IReadOnlyList<GitBranchInfo> branches = await _repositoryService!.GetBranchesAsync(repository.RootPath).ConfigureAwait(true);

        using BranchSetEditorDialog dialog = new(branches, repository.DefaultBranch);
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
        selectedBranchStatusLabel.Text = $"Deleted Branch-Set '{branchSet.Name}'.";
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

        selectedBranchStatusLabel.Text = $"Composed {compositionResult.TargetBranch} @ {compositionResult.NewSha[..Math.Min(12, compositionResult.NewSha.Length)]}.";

        MessageBox.Show(
            this,
            $"Composed and pushed '{compositionResult.TargetBranch}'.\n\nReplayed commits: {compositionResult.ReplayedCommits.Count}",
            "Compose Branch-Set",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private async Task UpdateSelectedBranchStatusAsync()
    {
        if (_repositoryService is null || SelectedRepository is not { } repository || SelectedBranchSet is not { } branchSet)
        {
            selectedBranchStatusLabel.Text = "No branch selected.";
            return;
        }

        string? branchName = branchSet.SourceBranches.FirstOrDefault();
        if (branchName is null)
        {
            selectedBranchStatusLabel.Text = "No source branch selected.";
            return;
        }

        GitCommitInfo commit = await _repositoryService.GetBranchTipAsync(repository.RootPath, branchName).ConfigureAwait(true);
        selectedBranchStatusLabel.Text = $"{branchName}: {commit.AuthorDate.LocalDateTime:g} | {commit.AbbreviatedSha} | {commit.Subject} | {commit.Sha}";
    }

    private void RenderRepositories()
    {
        repositoryListView.BeginUpdate();
        repositoryListView.Items.Clear();

        foreach (RepositoryEntry repository in _state.Repositories)
        {
            ListViewItem item = new(repository.DisplayName)
            {
                Tag = repository
            };
            item.SubItems.Add(repository.RootPath);
            item.SubItems.Add(repository.DefaultBranch ?? string.Empty);
            repositoryListView.Items.Add(item);
        }

        repositoryListView.EndUpdate();
    }

    private void RenderBranchSets()
    {
        branchSetListView.BeginUpdate();
        branchSetListView.Items.Clear();

        if (SelectedRepository is { } repository)
        {
            foreach (BranchSetDefinition branchSet in GetBranchSets(repository.Key))
            {
                ListViewItem item = new(branchSet.Name)
                {
                    Tag = branchSet
                };
                item.SubItems.Add(branchSet.BaseBranch);
                item.SubItems.Add(string.Join(", ", branchSet.SourceBranches));
                item.SubItems.Add($"{branchSet.Name}/{branchSet.TargetBranchName}");
                branchSetListView.Items.Add(item);
            }
        }

        branchSetListView.EndUpdate();
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
        foreach (ListViewItem item in repositoryListView.Items)
        {
            if (item.Tag is RepositoryEntry repository && string.Equals(repository.Key, repositoryKey, StringComparison.OrdinalIgnoreCase))
            {
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
                break;
            }
        }
    }

    private void SelectBranchSet(string branchSetName)
    {
        foreach (ListViewItem item in branchSetListView.Items)
        {
            if (item.Tag is BranchSetDefinition branchSet && string.Equals(branchSet.Name, branchSetName, StringComparison.OrdinalIgnoreCase))
            {
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
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
            selectedBranchStatusLabel.Text = ex.Message;
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

    [MemberNotNull(nameof(_stateStore), nameof(_repositoryService), nameof(_branchCompositionService))]
    private void EnsureServices()
    {
        if (_stateStore is null || _repositoryService is null || _branchCompositionService is null)
        {
            throw new InvalidOperationException("Application services are not available.");
        }
    }

    private RepositoryEntry? SelectedRepository
        => repositoryListView.SelectedItems.Count == 0 ? null : repositoryListView.SelectedItems[0].Tag as RepositoryEntry;

    private BranchSetDefinition? SelectedBranchSet
        => branchSetListView.SelectedItems.Count == 0 ? null : branchSetListView.SelectedItems[0].Tag as BranchSetDefinition;
}

