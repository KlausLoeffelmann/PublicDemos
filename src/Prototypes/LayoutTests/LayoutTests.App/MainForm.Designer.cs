namespace LayoutTests.App;

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
        newProbeSetToolStripMenuItem = new ToolStripMenuItem();
        loadProbeSetToolStripMenuItem = new ToolStripMenuItem();
        saveProbeSetToolStripMenuItem = new ToolStripMenuItem();
        saveProbeSetAsToolStripMenuItem = new ToolStripMenuItem();
        fileToolStripSeparator = new ToolStripSeparator();
        quitToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem = new ToolStripMenuItem();
        addContainerToolStripMenuItem = new ToolStripMenuItem();
        removeContainerToolStripMenuItem = new ToolStripMenuItem();
        primaryToolStrip = new ToolStrip();
        newProbeSetButton = new ToolStripButton();
        loadProbeSetButton = new ToolStripButton();
        saveProbeSetButton = new ToolStripButton();
        toolStripSeparator1 = new ToolStripSeparator();
        addCtorContainerButton = new ToolStripButton();
        addLazyContainerButton = new ToolStripButton();
        removeContainerButton = new ToolStripButton();
        toolStripSeparator2 = new ToolStripSeparator();
        actionButton = new ToolStripButton();
        mainSplitContainer = new SplitContainer();
        probeTreeView = new Designer.ProbeTreeView();
        containerPropertyPanel = new Designer.ContainerPropertyPanel();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        menuStrip.SuspendLayout();
        primaryToolStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
        mainSplitContainer.Panel1.SuspendLayout();
        mainSplitContainer.Panel2.SuspendLayout();
        mainSplitContainer.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip
        //
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1100, 24);
        menuStrip.TabIndex = 0;
        //
        // fileToolStripMenuItem
        //
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            newProbeSetToolStripMenuItem,
            loadProbeSetToolStripMenuItem,
            saveProbeSetToolStripMenuItem,
            saveProbeSetAsToolStripMenuItem,
            fileToolStripSeparator,
            quitToolStripMenuItem,
        });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(37, 20);
        fileToolStripMenuItem.Text = "&File";
        //
        // newProbeSetToolStripMenuItem
        //
        newProbeSetToolStripMenuItem.Name = "newProbeSetToolStripMenuItem";
        newProbeSetToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        newProbeSetToolStripMenuItem.Size = new Size(200, 22);
        newProbeSetToolStripMenuItem.Text = "&New Probe Set...";
        //
        // loadProbeSetToolStripMenuItem
        //
        loadProbeSetToolStripMenuItem.Name = "loadProbeSetToolStripMenuItem";
        loadProbeSetToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        loadProbeSetToolStripMenuItem.Size = new Size(200, 22);
        loadProbeSetToolStripMenuItem.Text = "&Load Probe Set...";
        //
        // saveProbeSetToolStripMenuItem
        //
        saveProbeSetToolStripMenuItem.Name = "saveProbeSetToolStripMenuItem";
        saveProbeSetToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        saveProbeSetToolStripMenuItem.Size = new Size(200, 22);
        saveProbeSetToolStripMenuItem.Text = "&Save Probe Set";
        //
        // saveProbeSetAsToolStripMenuItem
        //
        saveProbeSetAsToolStripMenuItem.Name = "saveProbeSetAsToolStripMenuItem";
        saveProbeSetAsToolStripMenuItem.Size = new Size(200, 22);
        saveProbeSetAsToolStripMenuItem.Text = "Save Probe Set &As...";
        //
        // fileToolStripSeparator
        //
        fileToolStripSeparator.Name = "fileToolStripSeparator";
        fileToolStripSeparator.Size = new Size(197, 6);
        //
        // quitToolStripMenuItem
        //
        quitToolStripMenuItem.Name = "quitToolStripMenuItem";
        quitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        quitToolStripMenuItem.Size = new Size(200, 22);
        quitToolStripMenuItem.Text = "&Quit";
        //
        // editToolStripMenuItem
        //
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
        {
            addContainerToolStripMenuItem,
            removeContainerToolStripMenuItem,
        });
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.Size = new Size(39, 20);
        editToolStripMenuItem.Text = "&Edit";
        //
        // addContainerToolStripMenuItem
        //
        addContainerToolStripMenuItem.Name = "addContainerToolStripMenuItem";
        addContainerToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.A;
        addContainerToolStripMenuItem.Size = new Size(195, 22);
        addContainerToolStripMenuItem.Text = "&Add Container...";
        //
        // removeContainerToolStripMenuItem
        //
        removeContainerToolStripMenuItem.Name = "removeContainerToolStripMenuItem";
        removeContainerToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Delete;
        removeContainerToolStripMenuItem.Size = new Size(195, 22);
        removeContainerToolStripMenuItem.Text = "&Remove Container...";
        //
        // primaryToolStrip
        //
        primaryToolStrip.ImageScalingSize = new Size(20, 20);
        primaryToolStrip.Items.AddRange(new ToolStripItem[]
        {
            newProbeSetButton,
            loadProbeSetButton,
            saveProbeSetButton,
            toolStripSeparator1,
            addCtorContainerButton,
            addLazyContainerButton,
            removeContainerButton,
            toolStripSeparator2,
            actionButton,
        });
        primaryToolStrip.Location = new Point(0, 24);
        primaryToolStrip.Name = "primaryToolStrip";
        primaryToolStrip.Size = new Size(1100, 27);
        primaryToolStrip.TabIndex = 1;
        //
        // newProbeSetButton
        //
        newProbeSetButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        newProbeSetButton.Name = "newProbeSetButton";
        newProbeSetButton.Size = new Size(35, 24);
        newProbeSetButton.Text = "New";
        newProbeSetButton.ToolTipText = "New Probe Set (Ctrl+N)";
        //
        // loadProbeSetButton
        //
        loadProbeSetButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        loadProbeSetButton.Name = "loadProbeSetButton";
        loadProbeSetButton.Size = new Size(40, 24);
        loadProbeSetButton.Text = "Load";
        loadProbeSetButton.ToolTipText = "Load Probe Set (Ctrl+O)";
        //
        // saveProbeSetButton
        //
        saveProbeSetButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        saveProbeSetButton.Name = "saveProbeSetButton";
        saveProbeSetButton.Size = new Size(40, 24);
        saveProbeSetButton.Text = "Save";
        saveProbeSetButton.ToolTipText = "Save Probe Set (Ctrl+S)";
        //
        // toolStripSeparator1
        //
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(6, 27);
        //
        // addCtorContainerButton
        //
        addCtorContainerButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        addCtorContainerButton.Name = "addCtorContainerButton";
        addCtorContainerButton.Size = new Size(115, 24);
        addCtorContainerButton.Text = "+ CTor Container";
        addCtorContainerButton.ToolTipText = "Add a CTor-time container under the selected node.";
        //
        // addLazyContainerButton
        //
        addLazyContainerButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        addLazyContainerButton.Name = "addLazyContainerButton";
        addLazyContainerButton.Size = new Size(115, 24);
        addLazyContainerButton.Text = "+ Lazy Container";
        addLazyContainerButton.ToolTipText = "Add a Lazy (BeginInvoke after Load) container under the selected node.";
        //
        // removeContainerButton
        //
        removeContainerButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        removeContainerButton.Name = "removeContainerButton";
        removeContainerButton.Size = new Size(70, 24);
        removeContainerButton.Text = "− Remove";
        removeContainerButton.ToolTipText = "Remove the selected container.";
        //
        // toolStripSeparator2
        //
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(6, 27);
        //
        // actionButton
        //
        actionButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        actionButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        actionButton.Name = "actionButton";
        actionButton.Size = new Size(85, 24);
        actionButton.Text = "▶ Action";
        actionButton.ToolTipText = "Instantiate the Carrier Form using the current probe set.";
        //
        // mainSplitContainer
        //
        mainSplitContainer.Dock = DockStyle.Fill;
        mainSplitContainer.Location = new Point(0, 51);
        mainSplitContainer.Name = "mainSplitContainer";
        mainSplitContainer.Panel1.Controls.Add(probeTreeView);
        mainSplitContainer.Panel2.Controls.Add(containerPropertyPanel);
        mainSplitContainer.Size = new Size(1100, 627);
        mainSplitContainer.SplitterDistance = 320;
        mainSplitContainer.TabIndex = 2;
        //
        // probeTreeView
        //
        probeTreeView.Dock = DockStyle.Fill;
        probeTreeView.HideSelection = false;
        probeTreeView.Location = new Point(0, 0);
        probeTreeView.Name = "probeTreeView";
        probeTreeView.Size = new Size(320, 627);
        probeTreeView.TabIndex = 0;
        //
        // containerPropertyPanel
        //
        containerPropertyPanel.Dock = DockStyle.Fill;
        containerPropertyPanel.Location = new Point(0, 0);
        containerPropertyPanel.Name = "containerPropertyPanel";
        containerPropertyPanel.Size = new Size(776, 627);
        containerPropertyPanel.TabIndex = 0;
        //
        // statusStrip
        //
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 678);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1100, 22);
        statusStrip.TabIndex = 3;
        //
        // statusLabel
        //
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(1085, 17);
        statusLabel.Spring = true;
        statusLabel.Text = "Ready.";
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 700);
        Controls.Add(mainSplitContainer);
        Controls.Add(primaryToolStrip);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        Text = "Layout Tests — Probe Set Designer";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        primaryToolStrip.ResumeLayout(false);
        primaryToolStrip.PerformLayout();
        mainSplitContainer.Panel1.ResumeLayout(false);
        mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
        mainSplitContainer.ResumeLayout(false);
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private System.ComponentModel.IContainer components = null!;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem newProbeSetToolStripMenuItem;
    private ToolStripMenuItem loadProbeSetToolStripMenuItem;
    private ToolStripMenuItem saveProbeSetToolStripMenuItem;
    private ToolStripMenuItem saveProbeSetAsToolStripMenuItem;
    private ToolStripSeparator fileToolStripSeparator;
    private ToolStripMenuItem quitToolStripMenuItem;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem addContainerToolStripMenuItem;
    private ToolStripMenuItem removeContainerToolStripMenuItem;
    private ToolStrip primaryToolStrip;
    private ToolStripButton newProbeSetButton;
    private ToolStripButton loadProbeSetButton;
    private ToolStripButton saveProbeSetButton;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton addCtorContainerButton;
    private ToolStripButton addLazyContainerButton;
    private ToolStripButton removeContainerButton;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripButton actionButton;
    private SplitContainer mainSplitContainer;
    private Designer.ProbeTreeView probeTreeView;
    private Designer.ContainerPropertyPanel containerPropertyPanel;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;
}
