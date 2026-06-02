using Microsoft.Extensions.DependencyInjection;
using WarpToolkit.ComponentModel;
using WarpToolkit.WinForms.Specialized;
using WingetPackageEditor.Core.ViewModels;

namespace Winget_Package_Editor;

public partial class MainForm : Form, IServiceProvider
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null!;

#pragma warning disable WFOWARP9901
    private sealed class DeferredServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DeferredServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
#pragma warning restore WFOWARP9901

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            if (_viewModel is not null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                _viewModel.ConsoleMessages.CollectionChanged -= ConsoleMessages_CollectionChanged;
                _viewModel.ViewCommandRequested -= ViewModel_ViewCommandRequested;
            }

            _appsBindingList?.Dispose();
            _treeViewBinder?.Dispose();
            _gridSelectionBinder?.Dispose();
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
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
        _mainMenuStrip = new MenuStrip();
        _fileMenuItem = new ToolStripMenuItem();
        _newMenuItem = new ToolStripMenuItem();
        _newFromExistingMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        _removePackageMenuItem = new ToolStripMenuItem();
        toolStripSeparator4 = new ToolStripSeparator();
        _openMenuItem = new ToolStripMenuItem();
        toolStripSeparator2 = new ToolStripSeparator();
        _exportMenuItem = new ToolStripMenuItem();
        _quitMenuItem = new ToolStripMenuItem();
        _editMenuItem = new ToolStripMenuItem();
        _addAppMenuItem = new ToolStripMenuItem();
        _removeAppMenuItem = new ToolStripMenuItem();
        toolStripSeparator3 = new ToolStripSeparator();
        _propertiesMenuItem = new ToolStripMenuItem();
        _viewMenuItem = new ToolStripMenuItem();
        _expandNodesMenuItem = new ToolStripMenuItem();
        _collapseNodeMenuItem = new ToolStripMenuItem();
        _expandSelectedMenuItem = new ToolStripMenuItem();
        _actionMenuItem = new ToolStripMenuItem();
        _updatePackageMenuItem = new ToolStripMenuItem();
        _applyNowMenuItem = new ToolStripMenuItem();
        _generateBundleFolderMenuItem = new ToolStripMenuItem();
        _toolsMenuItem = new ToolStripMenuItem();
        _optionsMenuItem = new ToolStripMenuItem();
        _helpMenuItem = new ToolStripMenuItem();
        _mainToolStrip = new ToolStrip();
        _newToolStripButton = new ToolStripButton();
        _addAppToolStripButton = new ToolStripButton();
        _removeAppToolStripButton = new ToolStripButton();
        _exportToolStripButton = new ToolStripButton();
        _applyNowToolStripButton = new ToolStripButton();
        _mainSplitContainer = new SplitContainer();
        _packageTreeView = new TreeView();
        _rightSplitContainer = new SplitContainer();
        _gridHostPanel = new Panel();
        _appsWarpDataGridView = new WarpDataGridView();
        _consoleControl = new ConsoleControl();
        _statusStrip = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();
        _mainMenuStrip.SuspendLayout();
        _mainToolStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).BeginInit();
        _mainSplitContainer.Panel1.SuspendLayout();
        _mainSplitContainer.Panel2.SuspendLayout();
        _mainSplitContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_rightSplitContainer).BeginInit();
        _rightSplitContainer.Panel1.SuspendLayout();
        _rightSplitContainer.Panel2.SuspendLayout();
        _rightSplitContainer.SuspendLayout();
        _gridHostPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_appsWarpDataGridView).BeginInit();
        _statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // _mainMenuStrip
        // 
        _mainMenuStrip.ImageScalingSize = new Size(24, 24);
        _mainMenuStrip.Items.AddRange(new ToolStripItem[] { _fileMenuItem, _editMenuItem, _viewMenuItem, _actionMenuItem, _toolsMenuItem, _helpMenuItem });
        _mainMenuStrip.Location = new Point(0, 0);
        _mainMenuStrip.Margin = new Padding(0, 2, 0, 0);
        _mainMenuStrip.Name = "_mainMenuStrip";
        _mainMenuStrip.Padding = new Padding(8, 2, 0, 2);
        _mainMenuStrip.Size = new Size(1348, 33);
        _mainMenuStrip.TabIndex = 0;
        // 
        // _fileMenuItem
        // 
        _fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _newMenuItem, _newFromExistingMenuItem, toolStripSeparator1, _removePackageMenuItem, toolStripSeparator4, _openMenuItem, toolStripSeparator2, _exportMenuItem, _quitMenuItem });
        _fileMenuItem.Name = "_fileMenuItem";
        _fileMenuItem.Size = new Size(54, 29);
        _fileMenuItem.Text = "&File";
        // 
        // _newMenuItem
        // 
        _newMenuItem.Name = "_newMenuItem";
        _newMenuItem.Size = new Size(341, 34);
        _newMenuItem.Text = "&New empty package...";
        // 
        // _newFromExistingMenuItem
        // 
        _newFromExistingMenuItem.Name = "_newFromExistingMenuItem";
        _newFromExistingMenuItem.Size = new Size(341, 34);
        _newFromExistingMenuItem.Text = "New from existing package...";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(338, 6);
        // 
        // _removePackageMenuItem
        // 
        _removePackageMenuItem.Name = "_removePackageMenuItem";
        _removePackageMenuItem.Size = new Size(341, 34);
        _removePackageMenuItem.Text = "Remove package";
        // 
        // toolStripSeparator4
        // 
        toolStripSeparator4.Name = "toolStripSeparator4";
        toolStripSeparator4.Size = new Size(338, 6);
        // 
        // _openMenuItem
        // 
        _openMenuItem.Name = "_openMenuItem";
        _openMenuItem.Size = new Size(341, 34);
        _openMenuItem.Text = "&Import package from file...";
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(338, 6);
        // 
        // _exportMenuItem
        // 
        _exportMenuItem.Name = "_exportMenuItem";
        _exportMenuItem.Size = new Size(341, 34);
        _exportMenuItem.Text = "&Export YAML+Script...";
        // 
        // _quitMenuItem
        // 
        _quitMenuItem.Name = "_quitMenuItem";
        _quitMenuItem.Size = new Size(341, 34);
        _quitMenuItem.Text = "&Quit";
        // 
        // _editMenuItem
        // 
        _editMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _addAppMenuItem, _removeAppMenuItem, toolStripSeparator3, _propertiesMenuItem });
        _editMenuItem.Name = "_editMenuItem";
        _editMenuItem.Size = new Size(58, 29);
        _editMenuItem.Text = "&Edit";
        // 
        // _addAppMenuItem
        // 
        _addAppMenuItem.Name = "_addAppMenuItem";
        _addAppMenuItem.Size = new Size(217, 34);
        _addAppMenuItem.Text = "&Add App...";
        // 
        // _removeAppMenuItem
        // 
        _removeAppMenuItem.Name = "_removeAppMenuItem";
        _removeAppMenuItem.Size = new Size(217, 34);
        _removeAppMenuItem.Text = "&Remove App";
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        toolStripSeparator3.Size = new Size(214, 6);
        // 
        // _propertiesMenuItem
        // 
        _propertiesMenuItem.Name = "_propertiesMenuItem";
        _propertiesMenuItem.Size = new Size(217, 34);
        _propertiesMenuItem.Text = "&Properties";
        // 
        // _viewMenuItem
        // 
        _viewMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _expandNodesMenuItem, _collapseNodeMenuItem, _expandSelectedMenuItem });
        _viewMenuItem.Name = "_viewMenuItem";
        _viewMenuItem.Size = new Size(65, 29);
        _viewMenuItem.Text = "&View";
        // 
        // _expandNodesMenuItem
        // 
        _expandNodesMenuItem.Name = "_expandNodesMenuItem";
        _expandNodesMenuItem.Size = new Size(241, 34);
        _expandNodesMenuItem.Text = "Expand &nodes";
        // 
        // _collapseNodeMenuItem
        // 
        _collapseNodeMenuItem.Name = "_collapseNodeMenuItem";
        _collapseNodeMenuItem.Size = new Size(241, 34);
        _collapseNodeMenuItem.Text = "&Collapse node";
        // 
        // _expandSelectedMenuItem
        // 
        _expandSelectedMenuItem.Name = "_expandSelectedMenuItem";
        _expandSelectedMenuItem.Size = new Size(241, 34);
        _expandSelectedMenuItem.Text = "Expand &selected";
        // 
        // _actionMenuItem
        // 
        _actionMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _updatePackageMenuItem, _applyNowMenuItem, _generateBundleFolderMenuItem });
        _actionMenuItem.Name = "_actionMenuItem";
        _actionMenuItem.Size = new Size(79, 29);
        _actionMenuItem.Text = "&Action";
        // 
        // _updatePackageMenuItem
        // 
        _updatePackageMenuItem.Name = "_updatePackageMenuItem";
        _updatePackageMenuItem.Size = new Size(315, 34);
        _updatePackageMenuItem.Text = "Update current package...";
        // 
        // _applyNowMenuItem
        // 
        _applyNowMenuItem.Name = "_applyNowMenuItem";
        _applyNowMenuItem.Size = new Size(315, 34);
        _applyNowMenuItem.Text = "&Apply package now...";
        // 
        // _generateBundleFolderMenuItem
        // 
        _generateBundleFolderMenuItem.Name = "_generateBundleFolderMenuItem";
        _generateBundleFolderMenuItem.Size = new Size(315, 34);
        _generateBundleFolderMenuItem.Text = "&Generate Bundle Folder...";
        // 
        // _toolsMenuItem
        // 
        _toolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _optionsMenuItem });
        _toolsMenuItem.Name = "_toolsMenuItem";
        _toolsMenuItem.Size = new Size(69, 29);
        _toolsMenuItem.Text = "&Tools";
        // 
        // _optionsMenuItem
        // 
        _optionsMenuItem.Name = "_optionsMenuItem";
        _optionsMenuItem.Size = new Size(178, 34);
        _optionsMenuItem.Text = "&Options";
        // 
        // _helpMenuItem
        // 
        _helpMenuItem.Name = "_helpMenuItem";
        _helpMenuItem.Size = new Size(65, 29);
        _helpMenuItem.Text = "&Help";
        // 
        // _mainToolStrip
        // 
        _mainToolStrip.ImageScalingSize = new Size(36, 36);
        _mainToolStrip.Items.AddRange(new ToolStripItem[] { _newToolStripButton, _addAppToolStripButton, _removeAppToolStripButton, _exportToolStripButton, _applyNowToolStripButton });
        _mainToolStrip.Location = new Point(0, 33);
        _mainToolStrip.Margin = new Padding(0, 2, 0, 2);
        _mainToolStrip.Name = "_mainToolStrip";
        _mainToolStrip.Size = new Size(1348, 34);
        _mainToolStrip.TabIndex = 1;
        // 
        // _newToolStripButton
        // 
        _newToolStripButton.Name = "_newToolStripButton";
        _newToolStripButton.Size = new Size(51, 29);
        _newToolStripButton.Text = "New";
        _newToolStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
        // 
        // _addAppToolStripButton
        // 
        _addAppToolStripButton.Name = "_addAppToolStripButton";
        _addAppToolStripButton.Size = new Size(89, 29);
        _addAppToolStripButton.Text = "Add App";
        _addAppToolStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
        // 
        // _removeAppToolStripButton
        // 
        _removeAppToolStripButton.Name = "_removeAppToolStripButton";
        _removeAppToolStripButton.Size = new Size(119, 29);
        _removeAppToolStripButton.Text = "Remove App";
        _removeAppToolStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
        // 
        // _exportToolStripButton
        // 
        _exportToolStripButton.Name = "_exportToolStripButton";
        _exportToolStripButton.Size = new Size(67, 29);
        _exportToolStripButton.Text = "Export";
        _exportToolStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
        // 
        // _applyNowToolStripButton
        // 
        _applyNowToolStripButton.Name = "_applyNowToolStripButton";
        _applyNowToolStripButton.Size = new Size(63, 29);
        _applyNowToolStripButton.Text = "Apply";
        _applyNowToolStripButton.TextImageRelation = TextImageRelation.ImageAboveText;
        // 
        // _mainSplitContainer
        // 
        _mainSplitContainer.Dock = DockStyle.Fill;
        _mainSplitContainer.Location = new Point(0, 67);
        _mainSplitContainer.Margin = new Padding(4);
        _mainSplitContainer.Name = "_mainSplitContainer";
        // 
        // _mainSplitContainer.Panel1
        // 
        _mainSplitContainer.Panel1.Controls.Add(_packageTreeView);
        // 
        // _mainSplitContainer.Panel2
        // 
        _mainSplitContainer.Panel2.Controls.Add(_rightSplitContainer);
        _mainSplitContainer.Size = new Size(1348, 735);
        _mainSplitContainer.SplitterDistance = 375;
        _mainSplitContainer.SplitterWidth = 5;
        _mainSplitContainer.TabIndex = 2;
        // 
        // _packageTreeView
        // 
        _packageTreeView.Dock = DockStyle.Fill;
        _packageTreeView.HideSelection = false;
        _packageTreeView.Location = new Point(0, 0);
        _packageTreeView.Margin = new Padding(4);
        _packageTreeView.Name = "_packageTreeView";
        _packageTreeView.Size = new Size(375, 735);
        _packageTreeView.TabIndex = 0;
        // 
        // _rightSplitContainer
        // 
        _rightSplitContainer.Dock = DockStyle.Fill;
        _rightSplitContainer.Location = new Point(0, 0);
        _rightSplitContainer.Margin = new Padding(4);
        _rightSplitContainer.Name = "_rightSplitContainer";
        _rightSplitContainer.Orientation = Orientation.Horizontal;
        // 
        // _rightSplitContainer.Panel1
        // 
        _rightSplitContainer.Panel1.Controls.Add(_gridHostPanel);
        // 
        // _rightSplitContainer.Panel2
        // 
        _rightSplitContainer.Panel2.Controls.Add(_consoleControl);
        _rightSplitContainer.Size = new Size(968, 735);
        _rightSplitContainer.SplitterDistance = 444;
        _rightSplitContainer.SplitterWidth = 5;
        _rightSplitContainer.TabIndex = 0;
        // 
        // _gridHostPanel
        // 
        _gridHostPanel.Controls.Add(_appsWarpDataGridView);
        _gridHostPanel.Dock = DockStyle.Fill;
        _gridHostPanel.Location = new Point(0, 0);
        _gridHostPanel.Margin = new Padding(4);
        _gridHostPanel.Name = "_gridHostPanel";
        _gridHostPanel.Size = new Size(968, 444);
        _gridHostPanel.TabIndex = 0;
        // 
        // _appsWarpDataGridView
        // 
        _appsWarpDataGridView.AllowUserToAddRows = false;
        _appsWarpDataGridView.AllowUserToDeleteRows = false;
        dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 245, 245);
        dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
        _appsWarpDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
        _appsWarpDataGridView.BackgroundColor = SystemColors.Window;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = SystemColors.Control;
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Control;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        _appsWarpDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        _appsWarpDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle3.BackColor = SystemColors.Window;
        dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        _appsWarpDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
        _appsWarpDataGridView.Dock = DockStyle.Fill;
        _appsWarpDataGridView.EnableHeadersVisualStyles = false;
        _appsWarpDataGridView.GridColor = SystemColors.ControlDark;
        _appsWarpDataGridView.Location = new Point(0, 0);
        _appsWarpDataGridView.Margin = new Padding(4);
        _appsWarpDataGridView.MultiSelect = false;
        _appsWarpDataGridView.Name = "_appsWarpDataGridView";
        dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle4.BackColor = SystemColors.Control;
        dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
        _appsWarpDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
        _appsWarpDataGridView.RowHeadersWidth = 62;
        _appsWarpDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _appsWarpDataGridView.Size = new Size(968, 444);
        _appsWarpDataGridView.TabIndex = 1;
        // 
        // _consoleControl
        // 
        _consoleControl.Dock = DockStyle.Fill;
        _consoleControl.Location = new Point(0, 0);
        _consoleControl.Margin = new Padding(4);
        _consoleControl.Name = "_consoleControl";
        _consoleControl.ReadOnly = true;
        _consoleControl.Size = new Size(968, 286);
        _consoleControl.TabIndex = 0;
        _consoleControl.Text = "";
        // 
        // _statusStrip
        // 
        _statusStrip.ImageScalingSize = new Size(24, 24);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _statusLabel });
        _statusStrip.Location = new Point(0, 802);
        _statusStrip.Margin = new Padding(0, 2, 0, 2);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Padding = new Padding(1, 0, 18, 0);
        _statusStrip.Size = new Size(1348, 32);
        _statusStrip.TabIndex = 3;
        // 
        // _statusLabel
        // 
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(60, 25);
        _statusLabel.Text = "Ready";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1348, 834);
        Controls.Add(_mainSplitContainer);
        Controls.Add(_statusStrip);
        Controls.Add(_mainToolStrip);
        Controls.Add(_mainMenuStrip);
        MainMenuStrip = _mainMenuStrip;
        Margin = new Padding(4);
        Name = "MainForm";
        Text = "WinGet Package Editor";
        _mainMenuStrip.ResumeLayout(false);
        _mainMenuStrip.PerformLayout();
        _mainToolStrip.ResumeLayout(false);
        _mainToolStrip.PerformLayout();
        _mainSplitContainer.Panel1.ResumeLayout(false);
        _mainSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_mainSplitContainer).EndInit();
        _mainSplitContainer.ResumeLayout(false);
        _rightSplitContainer.Panel1.ResumeLayout(false);
        _rightSplitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_rightSplitContainer).EndInit();
        _rightSplitContainer.ResumeLayout(false);
        _gridHostPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_appsWarpDataGridView).EndInit();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip _mainMenuStrip = null!;
    private ToolStripMenuItem _fileMenuItem = null!;
    private ToolStripMenuItem _newMenuItem = null!;
    private ToolStripMenuItem _openMenuItem = null!;
    private ToolStripMenuItem _exportMenuItem = null!;
    private ToolStripMenuItem _quitMenuItem = null!;
    private ToolStripMenuItem _editMenuItem = null!;
    private ToolStripMenuItem _addAppMenuItem = null!;
    private ToolStripMenuItem _removeAppMenuItem = null!;
    private ToolStripMenuItem _propertiesMenuItem = null!;
    private ToolStripMenuItem _viewMenuItem = null!;
    private ToolStripMenuItem _expandNodesMenuItem = null!;
    private ToolStripMenuItem _collapseNodeMenuItem = null!;
    private ToolStripMenuItem _expandSelectedMenuItem = null!;
    private ToolStripMenuItem _actionMenuItem = null!;
    private ToolStripMenuItem _applyNowMenuItem = null!;
    private ToolStripMenuItem _generateBundleFolderMenuItem = null!;
    private ToolStripMenuItem _toolsMenuItem = null!;
    private ToolStripMenuItem _optionsMenuItem = null!;
    private ToolStripMenuItem _helpMenuItem = null!;
    private ToolStrip _mainToolStrip = null!;
    private ToolStripButton _newToolStripButton = null!;
    private ToolStripButton _addAppToolStripButton = null!;
    private ToolStripButton _removeAppToolStripButton = null!;
    private ToolStripButton _exportToolStripButton = null!;
    private ToolStripButton _applyNowToolStripButton = null!;
    private SplitContainer _mainSplitContainer = null!;
    private TreeView _packageTreeView = null!;
    private SplitContainer _rightSplitContainer = null!;
    private Panel _gridHostPanel = null!;
    private WarpDataGridView _appsWarpDataGridView = null!;
    private ConsoleControl _consoleControl = null!;
    private StatusStrip _statusStrip = null!;
    private ToolStripStatusLabel _statusLabel = null!;
    private ToolStripMenuItem _newFromExistingMenuItem = null!;
    private ToolStripMenuItem _updatePackageMenuItem = null!;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem _removePackageMenuItem = null!;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripSeparator toolStripSeparator3;
}
