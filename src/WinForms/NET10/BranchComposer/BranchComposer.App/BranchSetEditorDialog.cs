using System.Diagnostics;
using System.Globalization;
using BranchComposer.App.Models;
using WarpToolkit.ComponentModel;
using WarpToolkit.WinForms.Extensions.UI;
using WarpToolkit.WinForms.Github.Git;

namespace BranchComposer.App;

public sealed partial class BranchSetEditorDialog : Form
{
    private const int CommitMessagePreviewLength = 40;
    private const string SourceBranchGridSettingsKey = "BranchSetEditorDialog.SourceBranchGrid";

    private readonly RepositoryEntry? _repository;
    private readonly ILocalGitRepositoryService? _repositoryService;
    private readonly IUserSettingsService? _userSettingsService;
    private string? _defaultBaseBranch;

    public BranchSetEditorDialog()
    {
        InitializeComponent();
    }

    public BranchSetEditorDialog(IEnumerable<GitBranchInfo> branches, string? defaultBaseBranch) : this()
    {
        ArgumentNullException.ThrowIfNull(branches);

        ConfigureRepositoryControls();
        ConfigureBranchControls(branches.ToArray(), defaultBaseBranch);
    }

    public BranchSetEditorDialog(
        RepositoryEntry repository,
        IEnumerable<GitBranchInfo> branches,
        string? defaultBaseBranch,
        ILocalGitRepositoryService repositoryService,
        IUserSettingsService? userSettingsService = null) : this()
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(branches);
        ArgumentNullException.ThrowIfNull(repositoryService);

        _repository = repository;
        _repositoryService = repositoryService;
        _userSettingsService = userSettingsService;
        _defaultBaseBranch = defaultBaseBranch;

        ConfigureRepositoryControls();
        ConfigureBranchControls(branches.ToArray(), defaultBaseBranch);
        ApplyPersistedUiSettings();
    }

    public BranchSetDefinition? Definition { get; private set; }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SaveUiSettings();
        base.OnFormClosing(e);
    }

    private void ConfigureBranchControls(IReadOnlyCollection<GitBranchInfo> branches, string? defaultBaseBranch)
    {
        _defaultBaseBranch = defaultBaseBranch;

        string[] branchNames = branches
            .Select(branch => branch.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(branch => branch, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _baseBranchComboBox.Items.Clear();
        _baseBranchComboBox.Items.AddRange(branchNames);

        if (defaultBaseBranch is not null && _baseBranchComboBox.Items.Contains(defaultBaseBranch))
        {
            _baseBranchComboBox.SelectedItem = defaultBaseBranch;
        }
        else if (_baseBranchComboBox.Items.Count > 0)
        {
            _baseBranchComboBox.SelectedIndex = 0;
        }

        _sourceBranchesDataGridView.Rows.Clear();
        foreach (GitBranchInfo branch in branches
            .Where(branch => !string.Equals(branch.Name, defaultBaseBranch, StringComparison.OrdinalIgnoreCase))
            .GroupBy(branch => branch.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(branch => branch.Name, StringComparer.OrdinalIgnoreCase))
        {
            int rowIndex = _sourceBranchesDataGridView.Rows.Add(
                false,
                branch.Name,
                FormatCommitDate(branch.LatestCommit),
                FormatCommitMessage(branch.LatestCommit));

            _sourceBranchesDataGridView.Rows[rowIndex].Tag = branch.Name;
        }
    }

    private void ConfigureRepositoryControls()
    {
        if (_repository is null)
        {
            _repositoryLinkLabel.Text = "Repository";
            _repositoryLinkLabel.Links.Clear();
            _repositoryLinkLabel.Enabled = false;
            _fetchButton.Enabled = false;
            return;
        }

        _repositoryLinkLabel.Text = _repository.DisplayName;
        _repositoryLinkLabel.Links.Clear();

        if (!string.IsNullOrWhiteSpace(_repository.RemoteUrl))
        {
            _repositoryLinkLabel.Links.Add(0, _repositoryLinkLabel.Text.Length, GetBrowserUrl(_repository));
            _repositoryLinkLabel.Enabled = true;
        }
        else
        {
            _repositoryLinkLabel.Enabled = false;
        }

        _fetchButton.Enabled = _repositoryService is not null;
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        string name = _nameTextBox.Text.Trim();
        string targetBranch = _targetBranchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(this, "Enter a Branch-Set name.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_baseBranchComboBox.SelectedItem is not string baseBranch)
        {
            MessageBox.Show(this, "Select a base branch.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string[] sourceBranches = _sourceBranchesDataGridView.Rows
            .Cast<DataGridViewRow>()
            .Where(row => row.Tag is string && row.Cells[0].Value is true)
            .Select(row => (string)row.Tag)
            .ToArray();

        if (sourceBranches.Length == 0)
        {
            MessageBox.Show(this, "Select at least one source branch.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(targetBranch))
        {
            MessageBox.Show(this, "Enter a target branch name.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Definition = new BranchSetDefinition
        {
            Name = name,
            BaseBranch = baseBranch,
            SourceBranches = [.. sourceBranches],
            TargetBranchName = targetBranch,
            NamingMode = _namingModeComboBox.SelectedIndex switch
            {
                1 => TargetBranchNamingMode.DateFolder,
                2 => TargetBranchNamingMode.NumberedSuffix,
                _ => TargetBranchNamingMode.Fixed
            },
            NumberWidth = (int)_numberWidthNumericUpDown.Value,
            OverwriteExisting = _overwriteCheckBox.Checked
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private async void FetchButton_Click(object? sender, EventArgs e)
        => await RunDialogActionAsync(async () =>
        {
            if (_repository is null || _repositoryService is null)
            {
                throw new InvalidOperationException("Repository information is not available for Git Fetch.");
            }

            string? selectedBaseBranch = _baseBranchComboBox.SelectedItem as string ?? _defaultBaseBranch;

            await RunGitFetchAsync(_repository.RootPath).ConfigureAwait(true);
            IReadOnlyList<GitBranchInfo> branches = await _repositoryService.GetBranchesAsync(_repository.RootPath).ConfigureAwait(true);
            ConfigureBranchControls(branches, selectedBaseBranch);
        }).ConfigureAwait(true);

    private void RepositoryLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (e.Link.LinkData is not string url)
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, FormatException(ex), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SourceBranchesDataGridView_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_sourceBranchesDataGridView.IsCurrentCellDirty)
        {
            _sourceBranchesDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void ApplyPersistedUiSettings()
    {
        _userSettingsService?.TryApplyDataGridViewColumnWidths(_sourceBranchesDataGridView, SourceBranchGridSettingsKey);
    }

    private void SaveUiSettings()
    {
        _userSettingsService?.SaveDataGridViewColumnWidths(_sourceBranchesDataGridView, SourceBranchGridSettingsKey);
    }

    private async Task RunDialogActionAsync(Func<Task> action)
    {
        try
        {
            UseWaitCursor = true;
            _fetchButton.Enabled = false;
            await action().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, FormatException(ex), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _fetchButton.Enabled = _repositoryService is not null;
            UseWaitCursor = false;
        }
    }

    private static string FormatCommitDate(GitCommitInfo? commit)
        => commit is null
            ? "n/a"
            : commit.CommitterDate.LocalDateTime.ToString("g", CultureInfo.CurrentCulture);

    private static string FormatCommitMessage(GitCommitInfo? commit)
    {
        string subject = commit?.Subject ?? string.Empty;
        return subject.Length > CommitMessagePreviewLength
            ? subject[..CommitMessagePreviewLength]
            : subject;
    }

    private static string GetBrowserUrl(RepositoryEntry repository)
    {
        if (Uri.TryCreate(repository.RemoteUrl, UriKind.Absolute, out Uri? uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return repository.RemoteUrl;
        }

        return $"https://{repository.RepositoryKey}";
    }

    private static async Task RunGitFetchAsync(string repositoryPath)
    {
        using Process process = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        process.StartInfo.ArgumentList.Add("-C");
        process.StartInfo.ArgumentList.Add(repositoryPath);
        process.StartInfo.ArgumentList.Add("fetch");
        process.StartInfo.ArgumentList.Add("--prune");

        if (!process.Start())
        {
            throw new InvalidOperationException("Unable to start git. Ensure Git is installed and available on PATH.");
        }

        Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync().ConfigureAwait(false);
        string standardOutput = await standardOutputTask.ConfigureAwait(false);
        string standardError = await standardErrorTask.ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Git Fetch failed with exit code {process.ExitCode}.\n\n{standardError}\n{standardOutput}".Trim());
        }
    }

    private static string FormatException(Exception exception)
        => exception switch
        {
            GitCommandException gitException
                => $"{gitException.Message}\n\n{gitException.StandardError}\n{gitException.StandardOutput}".Trim(),
            _ => exception.Message
        };
}
