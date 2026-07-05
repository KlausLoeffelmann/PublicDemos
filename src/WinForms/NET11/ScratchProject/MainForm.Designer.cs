// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject;

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
        _menuStrip = new MenuStrip();
        _fileToolStripMenuItem = new ToolStripMenuItem();
        _saveSettingsToolStripMenuItem = new ToolStripMenuItem();
        _loadSettingsToolStripMenuItem = new ToolStripMenuItem();
        _editToolStripMenuItem = new ToolStripMenuItem();
        _selectAllToolStripMenuItem = new ToolStripMenuItem();
        _resetSelectionToolStripMenuItem = new ToolStripMenuItem();
        _viewToolStripMenuItem = new ToolStripMenuItem();
        _statusStrip = new StatusStrip();
        _formSizeStatusLabel = new ToolStripStatusLabel();
        _formClientSizeStatusLabel = new ToolStripStatusLabel();
        _selectedControlStatusLabel = new ToolStripStatusLabel();
        _splitContainer = new SplitContainer();
        _propertyGrid = new PropertyGrid();
        _menuStrip.SuspendLayout();
        _statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel1.SuspendLayout();
        _splitContainer.Panel2.SuspendLayout();
        _splitContainer.SuspendLayout();
        SuspendLayout();
        //
        // _menuStrip
        //
        _menuStrip.Dock = DockStyle.Top;
        _menuStrip.Items.AddRange(new ToolStripItem[] { _fileToolStripMenuItem, _editToolStripMenuItem, _viewToolStripMenuItem });
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Size = new Size(1440, 24);
        _menuStrip.TabIndex = 0;
        //
        // _fileToolStripMenuItem
        //
        _fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _saveSettingsToolStripMenuItem, _loadSettingsToolStripMenuItem });
        _fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
        _fileToolStripMenuItem.Size = new Size(40, 20);
        _fileToolStripMenuItem.Text = "&File";
        //
        // _saveSettingsToolStripMenuItem
        //
        _saveSettingsToolStripMenuItem.Name = "_saveSettingsToolStripMenuItem";
        _saveSettingsToolStripMenuItem.Size = new Size(226, 22);
        _saveSettingsToolStripMenuItem.Text = "&Save property settings...";
        _saveSettingsToolStripMenuItem.Click += SaveSettingsToolStripMenuItem_Click;
        //
        // _loadSettingsToolStripMenuItem
        //
        _loadSettingsToolStripMenuItem.Name = "_loadSettingsToolStripMenuItem";
        _loadSettingsToolStripMenuItem.Size = new Size(226, 22);
        _loadSettingsToolStripMenuItem.Text = "&Load property settings...";
        _loadSettingsToolStripMenuItem.Click += LoadSettingsToolStripMenuItem_Click;
        //
        // _editToolStripMenuItem
        //
        _editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _selectAllToolStripMenuItem, _resetSelectionToolStripMenuItem });
        _editToolStripMenuItem.Name = "_editToolStripMenuItem";
        _editToolStripMenuItem.Size = new Size(40, 20);
        _editToolStripMenuItem.Text = "&Edit";
        //
        // _selectAllToolStripMenuItem
        //
        _selectAllToolStripMenuItem.Name = "_selectAllToolStripMenuItem";
        _selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
        _selectAllToolStripMenuItem.Size = new Size(226, 22);
        _selectAllToolStripMenuItem.Text = "Select &All";
        _selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
        //
        // _resetSelectionToolStripMenuItem
        //
        _resetSelectionToolStripMenuItem.Name = "_resetSelectionToolStripMenuItem";
        _resetSelectionToolStripMenuItem.Size = new Size(226, 22);
        _resetSelectionToolStripMenuItem.Text = "&Reset Selection";
        _resetSelectionToolStripMenuItem.Click += ResetSelectionToolStripMenuItem_Click;
        //
        // _viewToolStripMenuItem
        //
        // DropDownItems are populated at runtime in MainForm.cs, one per registered IScenarioView.
        _viewToolStripMenuItem.Name = "_viewToolStripMenuItem";
        _viewToolStripMenuItem.Size = new Size(44, 20);
        _viewToolStripMenuItem.Text = "&View";
        //
        // _statusStrip
        //
        _statusStrip.Dock = DockStyle.Bottom;
        _statusStrip.Items.AddRange(new ToolStripItem[] { _formSizeStatusLabel, _formClientSizeStatusLabel, _selectedControlStatusLabel });
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Size = new Size(1440, 22);
        _statusStrip.TabIndex = 2;
        //
        // _formSizeStatusLabel
        //
        _formSizeStatusLabel.Name = "_formSizeStatusLabel";
        _formSizeStatusLabel.Size = new Size(70, 17);
        _formSizeStatusLabel.Text = "Form size:";
        //
        // _formClientSizeStatusLabel
        //
        _formClientSizeStatusLabel.Name = "_formClientSizeStatusLabel";
        _formClientSizeStatusLabel.Size = new Size(100, 17);
        _formClientSizeStatusLabel.Text = "Form client size:";
        //
        // _selectedControlStatusLabel
        //
        _selectedControlStatusLabel.Name = "_selectedControlStatusLabel";
        _selectedControlStatusLabel.Size = new Size(200, 17);
        _selectedControlStatusLabel.Spring = true;
        _selectedControlStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        //
        // _splitContainer
        //
        _splitContainer.Dock = DockStyle.Fill;
        _splitContainer.Location = new Point(0, 24);
        _splitContainer.Name = "_splitContainer";
        //
        // _splitContainer.Panel1
        //
        // Panel1 hosts whichever IScenarioView UserControl is currently active; its content is
        // switched at runtime (see MainForm.cs, SwitchToView), so Panel1 stays empty here.
        _splitContainer.Panel1.AutoScroll = true;
        _splitContainer.Panel1.Padding = new Padding(12);
        //
        // _splitContainer.Panel2
        //
        // Panel2 hosts the single, shared PropertyGrid, which stays static across all views.
        _splitContainer.Panel2.Controls.Add(_propertyGrid);
        _splitContainer.Size = new Size(1440, 814);
        _splitContainer.SplitterDistance = 980;
        _splitContainer.TabIndex = 1;
        //
        // _propertyGrid
        //
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.TabIndex = 0;
        _propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        //
        // MainForm
        //
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1440, 860);
        Controls.Add(_splitContainer);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        MainMenuStrip = _menuStrip;
        MinimumSize = new Size(1000, 650);
        Name = "MainForm";
        Text = "ScratchProject - WinForms NET11 API scratchpad";
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _splitContainer.Panel1.ResumeLayout(false);
        _splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
        _splitContainer.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private MenuStrip _menuStrip;
    private ToolStripMenuItem _fileToolStripMenuItem;
    private ToolStripMenuItem _saveSettingsToolStripMenuItem;
    private ToolStripMenuItem _loadSettingsToolStripMenuItem;
    private ToolStripMenuItem _editToolStripMenuItem;
    private ToolStripMenuItem _selectAllToolStripMenuItem;
    private ToolStripMenuItem _resetSelectionToolStripMenuItem;
    private ToolStripMenuItem _viewToolStripMenuItem;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _formSizeStatusLabel;
    private ToolStripStatusLabel _formClientSizeStatusLabel;
    private ToolStripStatusLabel _selectedControlStatusLabel;
    private SplitContainer _splitContainer;
    private PropertyGrid _propertyGrid;
}
