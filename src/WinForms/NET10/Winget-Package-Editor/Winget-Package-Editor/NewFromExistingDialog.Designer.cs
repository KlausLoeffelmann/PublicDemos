namespace Winget_Package_Editor;

partial class NewFromExistingDialog
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        _layoutPanel = new TableLayoutPanel();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _sourceLabel = new Label();
        _sourceComboBox = new ComboBox();
        _buttonPanel = new FlowLayoutPanel();
        _okButton = new Button();
        _cancelButton = new Button();
        _layoutPanel.SuspendLayout();
        _buttonPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 2;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layoutPanel.Controls.Add(_nameLabel, 0, 0);
        _layoutPanel.Controls.Add(_nameTextBox, 1, 0);
        _layoutPanel.Controls.Add(_sourceLabel, 0, 1);
        _layoutPanel.Controls.Add(_sourceComboBox, 1, 1);
        _layoutPanel.Controls.Add(_buttonPanel, 1, 2);
        _layoutPanel.Dock = DockStyle.Fill;
        _layoutPanel.Location = new Point(0, 0);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.Padding = new Padding(12);
        _layoutPanel.RowCount = 3;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _layoutPanel.Size = new Size(458, 160);
        _layoutPanel.TabIndex = 0;
        // 
        // _nameLabel
        // 
        _nameLabel.Anchor = AnchorStyles.Left;
        _nameLabel.AutoSize = true;
        _nameLabel.Name = "_nameLabel";
        _nameLabel.Text = "New package name:";
        // 
        // _nameTextBox
        // 
        _nameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _nameTextBox.Name = "_nameTextBox";
        _nameTextBox.TabIndex = 0;
        // 
        // _sourceLabel
        // 
        _sourceLabel.Anchor = AnchorStyles.Left;
        _sourceLabel.AutoSize = true;
        _sourceLabel.Name = "_sourceLabel";
        _sourceLabel.Text = "Copy definition from:";
        // 
        // _sourceComboBox
        // 
        _sourceComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _sourceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _sourceComboBox.Name = "_sourceComboBox";
        _sourceComboBox.TabIndex = 1;
        // 
        // _buttonPanel
        // 
        _buttonPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _buttonPanel.AutoSize = true;
        _buttonPanel.Controls.Add(_okButton);
        _buttonPanel.Controls.Add(_cancelButton);
        _buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        _buttonPanel.Name = "_buttonPanel";
        _buttonPanel.TabIndex = 2;
        // 
        // _okButton
        // 
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Name = "_okButton";
        _okButton.Size = new Size(90, 27);
        _okButton.TabIndex = 0;
        _okButton.Text = "Create";
        _okButton.UseVisualStyleBackColor = true;
        _okButton.Click += OkButton_Click;
        // 
        // _cancelButton
        // 
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Name = "_cancelButton";
        _cancelButton.Size = new Size(90, 27);
        _cancelButton.TabIndex = 1;
        _cancelButton.Text = "Cancel";
        _cancelButton.UseVisualStyleBackColor = true;
        // 
        // NewFromExistingDialog
        // 
        AcceptButton = _okButton;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        ClientSize = new Size(458, 160);
        Controls.Add(_layoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "NewFromExistingDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "New from existing package";
        _layoutPanel.ResumeLayout(false);
        _layoutPanel.PerformLayout();
        _buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _layoutPanel = null!;
    private Label _nameLabel = null!;
    private TextBox _nameTextBox = null!;
    private Label _sourceLabel = null!;
    private ComboBox _sourceComboBox = null!;
    private FlowLayoutPanel _buttonPanel = null!;
    private Button _okButton = null!;
    private Button _cancelButton = null!;
}
