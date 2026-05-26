using BranchComposer.App.Models;
using WarpToolkit.WinForms.Github.Git;

namespace BranchComposer.App;

public sealed partial class BranchSetEditorDialog : Form
{
    public BranchSetEditorDialog()
    {
        InitializeComponent();
    }

    public BranchSetEditorDialog(IEnumerable<GitBranchInfo> branches, string? defaultBaseBranch) : this()
    {
        ArgumentNullException.ThrowIfNull(branches);

        ConfigureBranchControls(branches.ToArray(), defaultBaseBranch);
    }

    public BranchSetDefinition? Definition { get; private set; }

    private void ConfigureBranchControls(IReadOnlyCollection<GitBranchInfo> branches, string? defaultBaseBranch)
    {
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

        _sourceBranchesListBox.Items.Clear();
        foreach (string branchName in branches
            .Select(branch => branch.Name)
            .Where(branch => !string.Equals(branch, defaultBaseBranch, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(branch => branch, StringComparer.OrdinalIgnoreCase))
        {
            _sourceBranchesListBox.Items.Add(branchName);
        }
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

        string[] sourceBranches = _sourceBranchesListBox.CheckedItems
            .Cast<string>()
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
}
