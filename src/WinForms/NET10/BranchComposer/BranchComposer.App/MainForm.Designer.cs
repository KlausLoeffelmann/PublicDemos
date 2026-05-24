namespace BranchComposer.App;

partial class MainForm
{
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
    private SplitContainer splitContainer;
    private ListView repositoryListView;
    private ColumnHeader repositoryNameColumnHeader;
    private ColumnHeader repositoryPathColumnHeader;
    private ColumnHeader repositoryDefaultBranchColumnHeader;
    private ListView branchSetListView;
    private ColumnHeader branchSetNameColumnHeader;
    private ColumnHeader branchSetBaseColumnHeader;
    private ColumnHeader branchSetSourcesColumnHeader;
    private ColumnHeader branchSetTargetColumnHeader;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel selectedBranchStatusLabel;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
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
        splitContainer = new SplitContainer();
        repositoryListView = new ListView();
        repositoryNameColumnHeader = new ColumnHeader();
        repositoryPathColumnHeader = new ColumnHeader();
        repositoryDefaultBranchColumnHeader = new ColumnHeader();
        branchSetListView = new ListView();
        branchSetNameColumnHeader = new ColumnHeader();
        branchSetBaseColumnHeader = new ColumnHeader();
        branchSetSourcesColumnHeader = new ColumnHeader();
        branchSetTargetColumnHeader = new ColumnHeader();
        statusStrip = new StatusStrip();
        selectedBranchStatusLabel = new ToolStripStatusLabel();
        menuStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
        splitContainer.Panel1.SuspendLayout();
        splitContainer.Panel2.SuspendLayout();
        splitContainer.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange([fileToolStripMenuItem, branchSetToolStripMenuItem]);
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1184, 24);
        menuStrip.TabIndex = 0;
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange([addGithubRepoToolStripMenuItem, removeGithubRepoToolStripMenuItem, fileToolStripSeparator, quitToolStripMenuItem]);
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(37, 20);
        fileToolStripMenuItem.Text = "&File";
        // 
        // addGithubRepoToolStripMenuItem
        // 
        addGithubRepoToolStripMenuItem.Name = "addGithubRepoToolStripMenuItem";
        addGithubRepoToolStripMenuItem.Size = new Size(206, 22);
        addGithubRepoToolStripMenuItem.Text = "Add Github Repo...";
        // 
        // removeGithubRepoToolStripMenuItem
        // 
        removeGithubRepoToolStripMenuItem.Name = "removeGithubRepoToolStripMenuItem";
        removeGithubRepoToolStripMenuItem.Size = new Size(206, 22);
        removeGithubRepoToolStripMenuItem.Text = "Remove Github Repo ...";
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
        branchSetToolStripMenuItem.DropDownItems.AddRange([createBranchSetToolStripMenuItem, deleteBranchSetToolStripMenuItem, branchSetToolStripSeparator, composeBranchSetToolStripMenuItem]);
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
        // splitContainer
        // 
        splitContainer.Dock = DockStyle.Fill;
        splitContainer.Location = new Point(0, 24);
        splitContainer.Name = "splitContainer";
        splitContainer.Orientation = Orientation.Horizontal;
        // 
        // splitContainer.Panel1
        // 
        splitContainer.Panel1.Controls.Add(repositoryListView);
        // 
        // splitContainer.Panel2
        // 
        splitContainer.Panel2.Controls.Add(branchSetListView);
        splitContainer.Size = new Size(1184, 715);
        splitContainer.SplitterDistance = 320;
        splitContainer.TabIndex = 1;
        // 
        // repositoryListView
        // 
        repositoryListView.Columns.AddRange([repositoryNameColumnHeader, repositoryPathColumnHeader, repositoryDefaultBranchColumnHeader]);
        repositoryListView.Dock = DockStyle.Fill;
        repositoryListView.FullRowSelect = true;
        repositoryListView.GridLines = true;
        repositoryListView.MultiSelect = false;
        repositoryListView.Name = "repositoryListView";
        repositoryListView.Size = new Size(1184, 320);
        repositoryListView.TabIndex = 0;
        repositoryListView.UseCompatibleStateImageBehavior = false;
        repositoryListView.View = View.Details;
        // 
        // repositoryNameColumnHeader
        // 
        repositoryNameColumnHeader.Text = "Github Repo";
        repositoryNameColumnHeader.Width = 260;
        // 
        // repositoryPathColumnHeader
        // 
        repositoryPathColumnHeader.Text = "Local Path";
        repositoryPathColumnHeader.Width = 700;
        // 
        // repositoryDefaultBranchColumnHeader
        // 
        repositoryDefaultBranchColumnHeader.Text = "Default Branch";
        repositoryDefaultBranchColumnHeader.Width = 160;
        // 
        // branchSetListView
        // 
        branchSetListView.Columns.AddRange([branchSetNameColumnHeader, branchSetBaseColumnHeader, branchSetSourcesColumnHeader, branchSetTargetColumnHeader]);
        branchSetListView.Dock = DockStyle.Fill;
        branchSetListView.FullRowSelect = true;
        branchSetListView.GridLines = true;
        branchSetListView.MultiSelect = false;
        branchSetListView.Name = "branchSetListView";
        branchSetListView.Size = new Size(1184, 391);
        branchSetListView.TabIndex = 0;
        branchSetListView.UseCompatibleStateImageBehavior = false;
        branchSetListView.View = View.Details;
        // 
        // branchSetNameColumnHeader
        // 
        branchSetNameColumnHeader.Text = "Branch-Set";
        branchSetNameColumnHeader.Width = 220;
        // 
        // branchSetBaseColumnHeader
        // 
        branchSetBaseColumnHeader.Text = "Base Branch";
        branchSetBaseColumnHeader.Width = 220;
        // 
        // branchSetSourcesColumnHeader
        // 
        branchSetSourcesColumnHeader.Text = "Source Branches";
        branchSetSourcesColumnHeader.Width = 420;
        // 
        // branchSetTargetColumnHeader
        // 
        branchSetTargetColumnHeader.Text = "Target";
        branchSetTargetColumnHeader.Width = 260;
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange([selectedBranchStatusLabel]);
        statusStrip.Location = new Point(0, 739);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1184, 22);
        statusStrip.TabIndex = 2;
        // 
        // selectedBranchStatusLabel
        // 
        selectedBranchStatusLabel.Name = "selectedBranchStatusLabel";
        selectedBranchStatusLabel.Size = new Size(119, 17);
        selectedBranchStatusLabel.Text = "No branch selected.";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 761);
        Controls.Add(splitContainer);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        Text = "BranchComposer";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        splitContainer.Panel1.ResumeLayout(false);
        splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
        splitContainer.ResumeLayout(false);
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}

