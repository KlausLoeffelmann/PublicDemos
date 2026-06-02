using WarpToolkit.WinForms.Containers;
using WarpToolkit.WinForms.Specialized;

namespace WinBaas;

partial class FrmMain
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
        _menuStrip = new MenuStrip();
        _menuFile = new ToolStripMenuItem();
        _menuFileDiscover = new ToolStripMenuItem();
        _menuFileBackup = new ToolStripMenuItem();
        _menuFileSeparator = new ToolStripSeparator();
        _menuFileExit = new ToolStripMenuItem();
        _menuCatalog = new ToolStripMenuItem();
        _menuCatalogAdd = new ToolStripMenuItem();
        _menuCatalogDelete = new ToolStripMenuItem();
        _menuCatalogRestore = new ToolStripMenuItem();
        _menuView = new ToolStripMenuItem();
        _menuViewExpandAll = new ToolStripMenuItem();
        _menuViewCollapseAll = new ToolStripMenuItem();
        _menuViewExpandSelected = new ToolStripMenuItem();
        _menuViewOpenAffected = new ToolStripMenuItem();
        _menuTools = new ToolStripMenuItem();
        _menuToolsOptions = new ToolStripMenuItem();
        _toolStrip = new ToolStrip();
        _tsbDiscover = new ToolStripButton();
        _tsbBackup = new ToolStripButton();
        _tsbAdd = new ToolStripButton();
        _tsbDelete = new ToolStripButton();
        _tsbOptions = new ToolStripButton();
        _statusStrip = new StatusStrip();
        _statusInfo = new ToolStripStatusLabel();
        _statusSize = new ToolStripStatusLabel();
        _statusProgress = new ToolStripProgressBar();
        _splitOuter = new SplitContainer();
        _treeSources = new TreeView();
        _splitInner = new SplitContainer();
        _detailHost = new Panel();
        _toolTabs = new FluentTabControl();
        _consolePane = new UserControl();
        _console = new ConsoleControl();

        _menuStrip.SuspendLayout();
        _toolStrip.SuspendLayout();
        _statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_splitOuter).BeginInit();
        _splitOuter.Panel1.SuspendLayout();
        _splitOuter.Panel2.SuspendLayout();
        _splitOuter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_splitInner).BeginInit();
        _splitInner.Panel1.SuspendLayout();
        _splitInner.Panel2.SuspendLayout();
        _splitInner.SuspendLayout();
        _consolePane.SuspendLayout();
        SuspendLayout();

        // MenuStrip
        _menuStrip.Dock = DockStyle.Top;
        _menuStrip.Font = new Font("Segoe UI", 11F);
        _menuStrip.Items.AddRange(new ToolStripItem[] { _menuFile, _menuCatalog, _menuView, _menuTools });
        _menuStrip.Name = "_menuStrip";

        _menuFile.Text = "&File";
        _menuFile.DropDownItems.AddRange(new ToolStripItem[]
        {
            _menuFileDiscover, _menuFileBackup, _menuFileSeparator, _menuFileExit
        });
        _menuFileDiscover.Name = "_menuFileDiscover";
        _menuFileDiscover.Text = "&Discover objects to backup\u2026";
        _menuFileBackup.Name = "_menuFileBackup";
        _menuFileBackup.Text = "&Backup selected objects\u2026";
        _menuFileExit.Name = "_menuFileExit";
        _menuFileExit.Text = "E&xit";

        _menuCatalog.Text = "&Catalog";
        _menuCatalog.DropDownItems.AddRange(new ToolStripItem[]
        {
            _menuCatalogAdd, _menuCatalogDelete, _menuCatalogRestore
        });
        _menuCatalogAdd.Name = "_menuCatalogAdd";
        _menuCatalogAdd.Text = "&Add object\u2026";
        _menuCatalogDelete.Name = "_menuCatalogDelete";
        _menuCatalogDelete.Text = "&Delete object\u2026";
        _menuCatalogRestore.Name = "_menuCatalogRestore";
        _menuCatalogRestore.Text = "&Restore definition\u2026";

        _menuView.Text = "&View";
        _menuView.DropDownItems.AddRange(new ToolStripItem[]
        {
            _menuViewExpandAll, _menuViewCollapseAll, _menuViewExpandSelected, _menuViewOpenAffected
        });
        _menuViewExpandAll.Name = "_menuViewExpandAll";
        _menuViewExpandAll.Text = "E&xpand all nodes";
        _menuViewCollapseAll.Name = "_menuViewCollapseAll";
        _menuViewCollapseAll.Text = "&Collapse all nodes";
        _menuViewExpandSelected.Name = "_menuViewExpandSelected";
        _menuViewExpandSelected.Text = "Expand &selected";
        _menuViewOpenAffected.Name = "_menuViewOpenAffected";
        _menuViewOpenAffected.Text = "Open &affected nodes";

        _menuTools.Text = "&Tools";
        _menuTools.DropDownItems.AddRange(new ToolStripItem[] { _menuToolsOptions });
        _menuToolsOptions.Name = "_menuToolsOptions";
        _menuToolsOptions.Text = "&Options\u2026";

        // ToolStrip
        _toolStrip.Dock = DockStyle.Top;
        _toolStrip.Font = new Font("Segoe UI", 11F);
        _toolStrip.ImageScalingSize = new Size(36, 36);
        _toolStrip.AutoSize = false;
        _toolStrip.Height = 56;
        _toolStrip.GripStyle = ToolStripGripStyle.Hidden;
        _toolStrip.Items.AddRange(new ToolStripItem[]
        {
            _tsbDiscover, _tsbBackup, new ToolStripSeparator(), _tsbAdd, _tsbDelete,
            new ToolStripSeparator(), _tsbOptions
        });
        _toolStrip.Name = "_toolStrip";

        _tsbDiscover.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _tsbDiscover.Name = "_tsbDiscover";
        _tsbDiscover.Size = new Size(48, 48);
        _tsbBackup.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _tsbBackup.Name = "_tsbBackup";
        _tsbBackup.Size = new Size(48, 48);
        _tsbAdd.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _tsbAdd.Name = "_tsbAdd";
        _tsbAdd.Size = new Size(48, 48);
        _tsbDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _tsbDelete.Name = "_tsbDelete";
        _tsbDelete.Size = new Size(48, 48);
        _tsbOptions.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _tsbOptions.Name = "_tsbOptions";
        _tsbOptions.Size = new Size(48, 48);

        // StatusStrip
        _statusStrip.Dock = DockStyle.Bottom;
        _statusStrip.Font = new Font("Segoe UI", 11F);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusInfo, _statusSize, _statusProgress });
        _statusStrip.Name = "_statusStrip";
        _statusInfo.Name = "_statusInfo";
        _statusInfo.Spring = true;
        _statusInfo.TextAlign = ContentAlignment.MiddleLeft;
        _statusInfo.Text = "Ready.";
        _statusSize.Name = "_statusSize";
        _statusProgress.Name = "_statusProgress";
        _statusProgress.Visible = false;

        // SplitContainer outer (vertical splitter: Left = TreeView, Right = inner)
        _splitOuter.Dock = DockStyle.Fill;
        _splitOuter.Name = "_splitOuter";
        _splitOuter.Orientation = Orientation.Vertical;
        _splitOuter.SplitterDistance = 280;
        _splitOuter.Panel1.Controls.Add(_treeSources);
        _splitOuter.Panel2.Controls.Add(_splitInner);

        // TreeView
        _treeSources.Dock = DockStyle.Fill;
        _treeSources.Name = "_treeSources";
        _treeSources.CheckBoxes = true;
        _treeSources.HideSelection = false;
        _treeSources.Font = new Font("Segoe UI", 11F);

        // SplitContainer inner (horizontal splitter: Top = DataGridView, Bottom = FluentTabControl)
        _splitInner.Dock = DockStyle.Fill;
        _splitInner.Name = "_splitInner";
        _splitInner.Orientation = Orientation.Horizontal;
        _splitInner.SplitterDistance = 380;
        _splitInner.Panel1.Controls.Add(_detailHost);
        _splitInner.Panel2.Controls.Add(_toolTabs);

        // Detail host
        _detailHost.Dock = DockStyle.Fill;
        _detailHost.Name = "_detailHost";

        // FluentTabControl (Console + future tool windows)
        _toolTabs.Dock = DockStyle.Fill;
        _toolTabs.Name = "_toolTabs";

        // Console pane
        _consolePane.Dock = DockStyle.Fill;
        _consolePane.Name = "_consolePane";
        _consolePane.Controls.Add(_console);

        _console.Dock = DockStyle.Fill;
        _console.Name = "_console";
        _console.BorderStyle = BorderStyle.None;
        _console.BackColor = Color.FromArgb(0x1E, 0x1E, 0x1E);
        _console.ForeColor = Color.Gainsboro;
        _console.Font = new Font("Cascadia Mono", 10F);
        _console.ReadOnly = true;
        _console.HideSelection = false;

        // FrmMain
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1280, 800);
        Controls.Add(_splitOuter);
        Controls.Add(_toolStrip);
        Controls.Add(_menuStrip);
        Controls.Add(_statusStrip);
        Font = new Font("Segoe UI", 11F);
        MainMenuStrip = _menuStrip;
        Name = "FrmMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WinBaas - WinForms Backup Assist";

        _menuStrip.ResumeLayout(performLayout: false);
        _menuStrip.PerformLayout();
        _toolStrip.ResumeLayout(performLayout: false);
        _toolStrip.PerformLayout();
        _statusStrip.ResumeLayout(performLayout: false);
        _statusStrip.PerformLayout();
        _splitOuter.Panel1.ResumeLayout(performLayout: false);
        _splitOuter.Panel2.ResumeLayout(performLayout: false);
        ((System.ComponentModel.ISupportInitialize)_splitOuter).EndInit();
        _splitOuter.ResumeLayout(performLayout: false);
        _splitInner.Panel1.ResumeLayout(performLayout: false);
        _splitInner.Panel2.ResumeLayout(performLayout: false);
        ((System.ComponentModel.ISupportInitialize)_splitInner).EndInit();
        _splitInner.ResumeLayout(performLayout: false);
        _consolePane.ResumeLayout(performLayout: false);
        ResumeLayout(performLayout: false);
        PerformLayout();
    }

    #endregion

    private MenuStrip _menuStrip;
    private ToolStripMenuItem _menuFile;
    private ToolStripMenuItem _menuFileDiscover;
    private ToolStripMenuItem _menuFileBackup;
    private ToolStripSeparator _menuFileSeparator;
    private ToolStripMenuItem _menuFileExit;
    private ToolStripMenuItem _menuCatalog;
    private ToolStripMenuItem _menuCatalogAdd;
    private ToolStripMenuItem _menuCatalogDelete;
    private ToolStripMenuItem _menuCatalogRestore;
    private ToolStripMenuItem _menuView;
    private ToolStripMenuItem _menuViewExpandAll;
    private ToolStripMenuItem _menuViewCollapseAll;
    private ToolStripMenuItem _menuViewExpandSelected;
    private ToolStripMenuItem _menuViewOpenAffected;
    private ToolStripMenuItem _menuTools;
    private ToolStripMenuItem _menuToolsOptions;
    private ToolStrip _toolStrip;
    private ToolStripButton _tsbDiscover;
    private ToolStripButton _tsbBackup;
    private ToolStripButton _tsbAdd;
    private ToolStripButton _tsbDelete;
    private ToolStripButton _tsbOptions;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _statusInfo;
    private ToolStripStatusLabel _statusSize;
    private ToolStripProgressBar _statusProgress;
    private SplitContainer _splitOuter;
    private TreeView _treeSources;
    private SplitContainer _splitInner;
    private Panel _detailHost;
    private FluentTabControl _toolTabs;
    private UserControl _consolePane;
    private ConsoleControl _console;
}
