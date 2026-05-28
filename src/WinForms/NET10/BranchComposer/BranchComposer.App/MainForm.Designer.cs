namespace BranchComposer.App;

partial class MainForm
{
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        menuStrip = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        addGithubRepoToolStripMenuItem = new ToolStripMenuItem();
        removeGithubRepoToolStripMenuItem = new ToolStripMenuItem();
        fileToolStripSeparator = new ToolStripSeparator();
        quitToolStripMenuItem = new ToolStripMenuItem();
        branchSetToolStripMenuItem = new ToolStripMenuItem();
        createBranchSetToolStripMenuItem = new ToolStripMenuItem();
        deleteBranchSetToolStripMenuItem = new ToolStripMenuItem();
        branchSetToolStripSeparator = new ToolStripSeparator();
        composeBranchSetToolStripMenuItem = new ToolStripMenuItem();
        viewToolStripMenuItem = new ToolStripMenuItem();
        gitConsoleToolStripMenuItem = new ToolStripMenuItem();
        mainSplitContainer = new SplitContainer();
        repositoryTreeView = new TreeView();
        branchSetSplitContainer = new SplitContainer();
        branchSetDataGridView = new BranchSetDataGridView();
        branchSetNameColumn = new DataGridViewTextBoxColumn();
        branchSetBaseColumn = new DataGridViewTextBoxColumn();
        branchSetSourcesColumn = new DataGridViewTextBoxColumn();
        branchSetTargetColumn = new DataGridViewTextBoxColumn();
        gitConsoleTabControl = new WarpToolkit.WinForms.Containers.FluentTabControl();
        statusStrip = new StatusStrip();
        selectedBranchStatusLabel = new ToolStripStatusLabel();
        menuStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)branchSetSplitContainer).BeginInit();
        branchSetSplitContainer.Panel1.SuspendLayout();
        branchSetSplitContainer.Panel2.SuspendLayout();
        branchSetSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)branchSetDataGridView).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip
        //
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, branchSetToolStripMenuItem, viewToolStripMenuItem });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1184, 24);
        menuStrip.TabIndex = 0;
        //
        // fileToolStripMenuItem
        //
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addGithubRepoToolStripMenuItem, removeGithubRepoToolStripMenuItem, fileToolStripSeparator, quitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(37, 20);
        fileToolStripMenuItem.Text = "&File";
        //
        // addGithubRepoToolStripMenuItem
        //
        addGithubRepoToolStripMenuItem.Name = "addGithubRepoToolStripMenuItem";
        addGithubRepoToolStripMenuItem.Size = new Size(206, 22);
        addGithubRepoToolStripMenuItem.Text = "Add GitHub repository...";
        //
        // removeGithubRepoToolStripMenuItem
        //
        removeGithubRepoToolStripMenuItem.Name = "removeGithubRepoToolStripMenuItem";
        removeGithubRepoToolStripMenuItem.Size = new Size(206, 22);
        removeGithubRepoToolStripMenuItem.Text = "Remove GitHub repository...";
        //
        // fileToolStripSeparator
        //
        fileToolStripSeparator.Name = "fileToolStripSeparator";
        fileToolStripSeparator.Size = new Size(203, 6);
        //
        // quitToolStripMenuItem
        //
        quitToolStripMenuItem.Name = "quitToolStripMenuItem";
        quitToolStripMenuItem.Size = new Size(206, 22);
        quitToolStripMenuItem.Text = "Quit";
        //
        // branchSetToolStripMenuItem
        //
        branchSetToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { createBranchSetToolStripMenuItem, deleteBranchSetToolStripMenuItem, branchSetToolStripSeparator, composeBranchSetToolStripMenuItem });
        branchSetToolStripMenuItem.Name = "branchSetToolStripMenuItem";
        branchSetToolStripMenuItem.Size = new Size(76, 20);
        branchSetToolStripMenuItem.Text = "Branch-Set";
        //
        // createBranchSetToolStripMenuItem
        //
        createBranchSetToolStripMenuItem.Name = "createBranchSetToolStripMenuItem";
        createBranchSetToolStripMenuItem.Size = new Size(152, 22);
        createBranchSetToolStripMenuItem.Text = "Create new...";
        //
        // deleteBranchSetToolStripMenuItem
        //
        deleteBranchSetToolStripMenuItem.Name = "deleteBranchSetToolStripMenuItem";
        deleteBranchSetToolStripMenuItem.Size = new Size(152, 22);
        deleteBranchSetToolStripMenuItem.Text = "Delete...";
        //
        // branchSetToolStripSeparator
        //
        branchSetToolStripSeparator.Name = "branchSetToolStripSeparator";
        branchSetToolStripSeparator.Size = new Size(149, 6);
        //
        // composeBranchSetToolStripMenuItem
        //
        composeBranchSetToolStripMenuItem.Name = "composeBranchSetToolStripMenuItem";
        composeBranchSetToolStripMenuItem.Size = new Size(152, 22);
        composeBranchSetToolStripMenuItem.Text = "Compose ...";
        //
        // viewToolStripMenuItem
        //
        viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { gitConsoleToolStripMenuItem });
        viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        viewToolStripMenuItem.Size = new Size(44, 20);
        viewToolStripMenuItem.Text = "&View";
        //
        // gitConsoleToolStripMenuItem
        //
        gitConsoleToolStripMenuItem.Checked = true;
        gitConsoleToolStripMenuItem.CheckOnClick = true;
        gitConsoleToolStripMenuItem.CheckState = CheckState.Checked;
        gitConsoleToolStripMenuItem.Name = "gitConsoleToolStripMenuItem";
        gitConsoleToolStripMenuItem.Size = new Size(134, 22);
        gitConsoleToolStripMenuItem.Text = "Git Console";
        //
        // mainSplitContainer
        //
        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Location = new Point(0, 24);
        mainSplitContainer.Name = "mainSplitContainer";
        mainSplitContainer.Panel1.Controls.Add(repositoryTreeView);
        mainSplitContainer.Panel1MinSize = 220;
        mainSplitContainer.Panel2.Controls.Add(branchSetSplitContainer);
        mainSplitContainer.Panel2MinSize = 500;
        mainSplitContainer.Size = new Size(1184, 715);
        mainSplitContainer.SplitterDistance = 280;
        mainSplitContainer.TabIndex = 1;
        //
        // repositoryTreeView
        //
        repositoryTreeView.Dock = DockStyle.Fill;
        repositoryTreeView.HideSelection = false;
        repositoryTreeView.Location = new Point(0, 0);
        repositoryTreeView.Name = "repositoryTreeView";
        repositoryTreeView.Size = new Size(280, 715);
        repositoryTreeView.TabIndex = 0;
        //
        // branchSetSplitContainer
        //
        branchSetSplitContainer.Dock = DockStyle.Fill;
        branchSetSplitContainer.Location = new Point(0, 0);
        branchSetSplitContainer.Name = "branchSetSplitContainer";
        branchSetSplitContainer.Orientation = Orientation.Horizontal;
        branchSetSplitContainer.Panel1.Controls.Add(branchSetDataGridView);
        branchSetSplitContainer.Panel1MinSize = 220;
        branchSetSplitContainer.Panel2.Controls.Add(gitConsoleTabControl);
        branchSetSplitContainer.Panel2MinSize = 120;
        branchSetSplitContainer.Size = new Size(900, 715);
        branchSetSplitContainer.SplitterDistance = 470;
        branchSetSplitContainer.TabIndex = 0;
        //
        // branchSetDataGridView
        //
        branchSetDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        branchSetDataGridView.Columns.AddRange(new DataGridViewColumn[] { branchSetNameColumn, branchSetBaseColumn, branchSetSourcesColumn, branchSetTargetColumn });
        branchSetDataGridView.Dock = DockStyle.Fill;
        branchSetDataGridView.Location = new Point(0, 0);
        branchSetDataGridView.Name = "branchSetDataGridView";
        branchSetDataGridView.Size = new Size(900, 470);
        branchSetDataGridView.TabIndex = 0;
        //
        // branchSetNameColumn
        //
        branchSetNameColumn.HeaderText = "Branch-Set name";
        branchSetNameColumn.Name = "branchSetNameColumn";
        branchSetNameColumn.ReadOnly = true;
        branchSetNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        branchSetNameColumn.Width = 220;
        //
        // branchSetBaseColumn
        //
        branchSetBaseColumn.HeaderText = "Base branch";
        branchSetBaseColumn.Name = "branchSetBaseColumn";
        branchSetBaseColumn.ReadOnly = true;
        branchSetBaseColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        branchSetBaseColumn.Width = 220;
        //
        // branchSetSourcesColumn
        //
        branchSetSourcesColumn.HeaderText = "Branches to replay";
        branchSetSourcesColumn.Name = "branchSetSourcesColumn";
        branchSetSourcesColumn.ReadOnly = true;
        branchSetSourcesColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        branchSetSourcesColumn.Width = 360;
        //
        // branchSetTargetColumn
        //
        branchSetTargetColumn.HeaderText = "Target branch";
        branchSetTargetColumn.Name = "branchSetTargetColumn";
        branchSetTargetColumn.ReadOnly = true;
        branchSetTargetColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        branchSetTargetColumn.Width = 260;
        //
        // gitConsoleTabControl
        //
        gitConsoleTabControl.Dock = DockStyle.Fill;
        gitConsoleTabControl.Location = new Point(0, 0);
        gitConsoleTabControl.Name = "gitConsoleTabControl";
        gitConsoleTabControl.Size = new Size(900, 241);
        gitConsoleTabControl.TabIndex = 0;
        //
        // statusStrip
        //
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { selectedBranchStatusLabel });
        statusStrip.Location = new Point(0, 739);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1184, 22);
        statusStrip.TabIndex = 2;
        //
        // selectedBranchStatusLabel
        //
        selectedBranchStatusLabel.Name = "selectedBranchStatusLabel";
        selectedBranchStatusLabel.Size = new Size(1169, 17);
        selectedBranchStatusLabel.Spring = true;
        selectedBranchStatusLabel.Text = "No repository selected.";
        selectedBranchStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 761);
        Controls.Add(mainSplitContainer);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        Text = "BranchComposer";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        branchSetSplitContainer.Panel1.ResumeLayout(false);
        branchSetSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)branchSetSplitContainer).EndInit();
        branchSetSplitContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)branchSetDataGridView).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.ComponentModel.IContainer components = null!;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem addGithubRepoToolStripMenuItem;
    private ToolStripMenuItem removeGithubRepoToolStripMenuItem;
    private ToolStripSeparator fileToolStripSeparator;
    private ToolStripMenuItem quitToolStripMenuItem;
    private ToolStripMenuItem branchSetToolStripMenuItem;
    private ToolStripMenuItem createBranchSetToolStripMenuItem;
    private ToolStripMenuItem deleteBranchSetToolStripMenuItem;
    private ToolStripSeparator branchSetToolStripSeparator;
    private ToolStripMenuItem composeBranchSetToolStripMenuItem;
    private ToolStripMenuItem viewToolStripMenuItem;
    private ToolStripMenuItem gitConsoleToolStripMenuItem;
    private SplitContainer mainSplitContainer;
    private TreeView repositoryTreeView;
    private SplitContainer branchSetSplitContainer;
    private BranchSetDataGridView branchSetDataGridView;
    private DataGridViewTextBoxColumn branchSetNameColumn;
    private DataGridViewTextBoxColumn branchSetBaseColumn;
    private DataGridViewTextBoxColumn branchSetSourcesColumn;
    private DataGridViewTextBoxColumn branchSetTargetColumn;
    private WarpToolkit.WinForms.Containers.FluentTabControl gitConsoleTabControl;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel selectedBranchStatusLabel;
}
