namespace TreeViewNodeFontScaling;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _mainLayout = new TableLayoutPanel();
        _descriptionLabel = new Label();
        _treeView = new TreeViewEx();
        _statusLabel = new Label();
        _mainLayout.SuspendLayout();
        SuspendLayout();
        // 
        // _mainLayout
        // 
        _mainLayout.ColumnCount = 1;
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _mainLayout.Controls.Add(_descriptionLabel, 0, 0);
        _mainLayout.Controls.Add(_treeView, 0, 1);
        _mainLayout.Controls.Add(_statusLabel, 0, 2);
        _mainLayout.Dock = DockStyle.Fill;
        _mainLayout.Location = new Point(0, 0);
        _mainLayout.Name = "_mainLayout";
        _mainLayout.Padding = new Padding(12);
        _mainLayout.RowCount = 3;
        _mainLayout.RowStyles.Add(new RowStyle());
        _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _mainLayout.RowStyles.Add(new RowStyle());
        _mainLayout.Size = new Size(984, 711);
        _mainLayout.TabIndex = 0;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _descriptionLabel.AutoSize = true;
        _descriptionLabel.Location = new Point(15, 12);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Padding = new Padding(0, 0, 0, 8);
        _descriptionLabel.Size = new Size(954, 23);
        _descriptionLabel.TabIndex = 0;
        _descriptionLabel.Text = "TreeViewEx computes one ItemHeight from the largest styled TreeNodeEx and owner-draws per-node spacing, colors, full-row highlighting, and connector lines.";
        // 
        // _treeView
        // 
        _treeView.AccessibleDescription = "Demonstrates extended tree node rendering options.";
        _treeView.AccessibleName = "TreeViewEx rendering demonstration";
        _treeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _treeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
        _treeView.HideSelection = false;
        _treeView.HotTracking = true;
        _treeView.Location = new Point(15, 38);
        _treeView.Name = "_treeView";
        _treeView.ShowLines = true;
        _treeView.ShowPlusMinus = true;
        _treeView.ShowRootLines = true;
        _treeView.Size = new Size(954, 636);
        _treeView.TabIndex = 1;
        _treeView.CalculatedItemHeightChanged += TreeView_CalculatedItemHeightChanged;
        // 
        // _statusLabel
        // 
        _statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _statusLabel.AutoSize = true;
        _statusLabel.Location = new Point(15, 677);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Padding = new Padding(0, 6, 0, 0);
        _statusLabel.Size = new Size(954, 21);
        _statusLabel.TabIndex = 2;
        _statusLabel.Text = "Calculated global ItemHeight:";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(984, 711);
        Controls.Add(_mainLayout);
        MinimumSize = new Size(700, 500);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TreeViewEx / TreeNodeEx Font Scaling";
        Disposed += MainForm_Disposed;
        _mainLayout.ResumeLayout(false);
        _mainLayout.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _mainLayout;
    private Label _descriptionLabel;
    private TreeViewEx _treeView;
    private Label _statusLabel;
}
