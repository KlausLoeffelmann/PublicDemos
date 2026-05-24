using BranchComposer.App.Models;
using WarpToolkit.WinForms.Github.Git;

namespace BranchComposer.App;

public sealed class BranchSetEditorDialog : Form
{
    private readonly TextBox _nameTextBox = new();
    private readonly ComboBox _baseBranchComboBox = new();
    private readonly CheckedListBox _sourceBranchesListBox = new();
    private readonly TextBox _targetBranchTextBox = new();
    private readonly ComboBox _namingModeComboBox = new();
    private readonly NumericUpDown _numberWidthNumericUpDown = new();
    private readonly CheckBox _overwriteCheckBox = new();

    public BranchSetEditorDialog(IEnumerable<GitBranchInfo> branches, string? defaultBaseBranch)
    {
        Text = "Create Branch-Set";
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        ClientSize = new Size(720, 650);

        Button okButton = new()
        {
            Text = "OK",
            DialogResult = DialogResult.None,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Location = new Point(526, 604),
            Size = new Size(85, 30)
        };
        okButton.Click += OkButton_Click;

        Button cancelButton = new()
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Location = new Point(620, 604),
            Size = new Size(85, 30)
        };

        Controls.Add(CreateLabeledControl("Branch-Set name:", _nameTextBox, 15, 15));
        Controls.Add(CreateLabeledControl("Base branch:", _baseBranchComboBox, 15, 75));
        Controls.Add(CreateSourceBranchesGroup(branches, defaultBaseBranch));
        Controls.Add(CreateLabeledControl("Target branch name:", _targetBranchTextBox, 15, 400));
        Controls.Add(CreateLabeledControl("Target naming:", _namingModeComboBox, 15, 460));
        Controls.Add(CreateNumberWidthPanel());
        Controls.Add(_overwriteCheckBox);
        Controls.Add(okButton);
        Controls.Add(cancelButton);

        AcceptButton = okButton;
        CancelButton = cancelButton;

        ConfigureBranchControls(branches, defaultBaseBranch);
    }

    public BranchSetDefinition? Definition { get; private set; }

    private void ConfigureBranchControls(IEnumerable<GitBranchInfo> branches, string? defaultBaseBranch)
    {
        string[] branchNames = branches
            .Select(branch => branch.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(branch => branch, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _baseBranchComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _baseBranchComboBox.Items.AddRange(branchNames);

        if (defaultBaseBranch is not null && _baseBranchComboBox.Items.Contains(defaultBaseBranch))
        {
            _baseBranchComboBox.SelectedItem = defaultBaseBranch;
        }
        else if (_baseBranchComboBox.Items.Count > 0)
        {
            _baseBranchComboBox.SelectedIndex = 0;
        }

        _namingModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _namingModeComboBox.Items.AddRange(["Fixed / overwrite target", "Date folder", "Increasing numeric suffix"]);
        _namingModeComboBox.SelectedIndex = 0;

        _numberWidthNumericUpDown.Minimum = 2;
        _numberWidthNumericUpDown.Maximum = 3;
        _numberWidthNumericUpDown.Value = 2;

        _overwriteCheckBox.Text = "Overwrite existing target branch when naming resolves to an existing branch";
        _overwriteCheckBox.Location = new Point(180, 555);
        _overwriteCheckBox.Size = new Size(500, 24);
    }

    private GroupBox CreateSourceBranchesGroup(IEnumerable<GitBranchInfo> branches, string? defaultBaseBranch)
    {
        GroupBox groupBox = new()
        {
            Text = "Source branches to replay",
            Location = new Point(15, 135),
            Size = new Size(690, 250)
        };

        _sourceBranchesListBox.CheckOnClick = true;
        _sourceBranchesListBox.Location = new Point(15, 25);
        _sourceBranchesListBox.Size = new Size(660, 205);
        _sourceBranchesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        foreach (string branchName in branches
            .Select(branch => branch.Name)
            .Where(branch => !string.Equals(branch, defaultBaseBranch, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(branch => branch, StringComparer.OrdinalIgnoreCase))
        {
            _sourceBranchesListBox.Items.Add(branchName);
        }

        groupBox.Controls.Add(_sourceBranchesListBox);
        return groupBox;
    }

    private Panel CreateNumberWidthPanel()
    {
        Panel panel = new()
        {
            Location = new Point(15, 515),
            Size = new Size(690, 30)
        };

        Label label = new()
        {
            Text = "Numeric suffix width:",
            Location = new Point(0, 4),
            AutoSize = true
        };

        _numberWidthNumericUpDown.Location = new Point(165, 1);
        _numberWidthNumericUpDown.Size = new Size(70, 23);

        panel.Controls.Add(label);
        panel.Controls.Add(_numberWidthNumericUpDown);
        return panel;
    }

    private static Panel CreateLabeledControl(string labelText, Control control, int x, int y)
    {
        Panel panel = new()
        {
            Location = new Point(x, y),
            Size = new Size(690, 50)
        };

        Label label = new()
        {
            Text = labelText,
            Location = new Point(0, 4),
            AutoSize = true
        };

        control.Location = new Point(165, 0);
        control.Size = new Size(500, 23);
        control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        panel.Controls.Add(label);
        panel.Controls.Add(control);
        return panel;
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

