// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo;

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
        _menuStrip = new MenuStrip();
        _fileToolStripMenuItem = new ToolStripMenuItem();
        _saveSettingsToolStripMenuItem = new ToolStripMenuItem();
        _loadSettingsToolStripMenuItem = new ToolStripMenuItem();
        _editToolStripMenuItem = new ToolStripMenuItem();
        _selectAllToolStripMenuItem = new ToolStripMenuItem();
        _clearSelectionToolStripMenuItem = new ToolStripMenuItem();
        _viewToolStripMenuItem = new ToolStripMenuItem();
        _selectionMarginToolStripMenuItem = new ToolStripMenuItem();
        _statusStrip = new StatusStrip();
        _formSizeStatusLabel = new ToolStripStatusLabel();
        _formClientSizeStatusLabel = new ToolStripStatusLabel();
        _selectedControlStatusLabel = new ToolStripStatusLabel();
        _splitContainer = new SplitContainer();
        _propertyGrid = new PropertyGrid();
        _menuStrip.SuspendLayout();
        _statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel2.SuspendLayout();
        _splitContainer.SuspendLayout();
        SuspendLayout();
        // 
        // _menuStrip
        // 
        _menuStrip.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _menuStrip.ImageScalingSize = new Size(24, 24);
        _menuStrip.Items.AddRange(new ToolStripItem[] { _fileToolStripMenuItem, _editToolStripMenuItem, _viewToolStripMenuItem });
        _menuStrip.Location = new Point(0, 0);
        _menuStrip.Name = "_menuStrip";
        _menuStrip.Padding = new Padding(7, 2, 0, 2);
        _menuStrip.Size = new Size(1405, 38);
        _menuStrip.TabIndex = 0;
        // 
        // _fileToolStripMenuItem
        // 
        _fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _saveSettingsToolStripMenuItem, _loadSettingsToolStripMenuItem });
        _fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
        _fileToolStripMenuItem.Size = new Size(62, 34);
        _fileToolStripMenuItem.Text = "&File";
        // 
        // _saveSettingsToolStripMenuItem
        // 
        _saveSettingsToolStripMenuItem.Name = "_saveSettingsToolStripMenuItem";
        _saveSettingsToolStripMenuItem.Size = new Size(349, 38);
        _saveSettingsToolStripMenuItem.Text = "&Save property settings...";
        _saveSettingsToolStripMenuItem.Click += SaveSettingsToolStripMenuItem_Click;
        // 
        // _loadSettingsToolStripMenuItem
        // 
        _loadSettingsToolStripMenuItem.Name = "_loadSettingsToolStripMenuItem";
        _loadSettingsToolStripMenuItem.Size = new Size(349, 38);
        _loadSettingsToolStripMenuItem.Text = "&Load property settings...";
        _loadSettingsToolStripMenuItem.Click += LoadSettingsToolStripMenuItem_Click;
        // 
        // _editToolStripMenuItem
        // 
        _editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _selectAllToolStripMenuItem, _clearSelectionToolStripMenuItem });
        _editToolStripMenuItem.Name = "_editToolStripMenuItem";
        _editToolStripMenuItem.Size = new Size(65, 34);
        _editToolStripMenuItem.Text = "&Edit";
        // 
        // _selectAllToolStripMenuItem
        // 
        _selectAllToolStripMenuItem.Name = "_selectAllToolStripMenuItem";
        _selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
        _selectAllToolStripMenuItem.Size = new Size(280, 38);
        _selectAllToolStripMenuItem.Text = "Select &All";
        _selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
        // 
        // _clearSelectionToolStripMenuItem
        // 
        _clearSelectionToolStripMenuItem.Name = "_clearSelectionToolStripMenuItem";
        _clearSelectionToolStripMenuItem.Size = new Size(280, 38);
        _clearSelectionToolStripMenuItem.Text = "&Clear Selection";
        _clearSelectionToolStripMenuItem.Click += ClearSelectionToolStripMenuItem_Click;
        // 
        // _viewToolStripMenuItem
        // 
        _viewToolStripMenuItem.Name = "_viewToolStripMenuItem";
        _viewToolStripMenuItem.Size = new Size(76, 34);
        _viewToolStripMenuItem.Text = "&View";
        // 
        // _selectionMarginToolStripMenuItem
        // 
        _selectionMarginToolStripMenuItem.Name = "_selectionMarginToolStripMenuItem";
        _selectionMarginToolStripMenuItem.Size = new Size(280, 38);
        _selectionMarginToolStripMenuItem.Text = "Selection &Margin";
        // 
        // _statusStrip
        // 
        _statusStrip.ImageScalingSize = new Size(24, 24);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _formSizeStatusLabel, _formClientSizeStatusLabel, _selectedControlStatusLabel });
        _statusStrip.Location = new Point(0, 676);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Padding = new Padding(1, 0, 26, 0);
        _statusStrip.Size = new Size(1405, 37);
        _statusStrip.TabIndex = 2;
        // 
        // _formSizeStatusLabel
        // 
        _formSizeStatusLabel.Font = new Font("Segoe UI", 11F);
        _formSizeStatusLabel.Name = "_formSizeStatusLabel";
        _formSizeStatusLabel.Size = new Size(111, 30);
        _formSizeStatusLabel.Text = "Form size:";
        // 
        // _formClientSizeStatusLabel
        // 
        _formClientSizeStatusLabel.Font = new Font("Segoe UI", 11F);
        _formClientSizeStatusLabel.Name = "_formClientSizeStatusLabel";
        _formClientSizeStatusLabel.Size = new Size(168, 30);
        _formClientSizeStatusLabel.Text = "Form client size:";
        // 
        // _selectedControlStatusLabel
        // 
        _selectedControlStatusLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _selectedControlStatusLabel.Name = "_selectedControlStatusLabel";
        _selectedControlStatusLabel.Size = new Size(1053, 30);
        _selectedControlStatusLabel.Spring = true;
        _selectedControlStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _splitContainer
        // 
        _splitContainer.Dock = DockStyle.Fill;
        _splitContainer.Location = new Point(0, 38);
        _splitContainer.Margin = new Padding(4, 4, 4, 4);
        _splitContainer.Name = "_splitContainer";
        // 
        // _splitContainer.Panel1
        // 
        _splitContainer.Panel1.AutoScroll = true;
        _splitContainer.Panel1.Padding = new Padding(14, 14, 14, 14);
        // 
        // _splitContainer.Panel2
        // 
        _splitContainer.Panel2.Controls.Add(_propertyGrid);
        _splitContainer.Size = new Size(1405, 638);
        _splitContainer.SplitterDistance = 956;
        _splitContainer.SplitterWidth = 5;
        _splitContainer.TabIndex = 1;
        // 
        // _propertyGrid
        // 
        _propertyGrid.BackColor = SystemColors.Control;
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.Location = new Point(0, 0);
        _propertyGrid.Margin = new Padding(4, 4, 4, 4);
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.Size = new Size(444, 638);
        _propertyGrid.TabIndex = 0;
        _propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(12F, 30F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1405, 713);
        Controls.Add(_splitContainer);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        MainMenuStrip = _menuStrip;
        Margin = new Padding(4, 4, 4, 4);
        MinimumSize = new Size(1196, 769);
        Name = "MainForm";
        Text = "VisualStylesModeDemo - WinForms NET11 API scratchpad";
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _statusStrip.ResumeLayout(false);
        _statusStrip.PerformLayout();
        _splitContainer.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
        _splitContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip _menuStrip;
    private ToolStripMenuItem _fileToolStripMenuItem;
    private ToolStripMenuItem _saveSettingsToolStripMenuItem;
    private ToolStripMenuItem _loadSettingsToolStripMenuItem;
    private ToolStripMenuItem _editToolStripMenuItem;
    private ToolStripMenuItem _selectAllToolStripMenuItem;
    private ToolStripMenuItem _clearSelectionToolStripMenuItem;
    private ToolStripMenuItem _viewToolStripMenuItem;
    private ToolStripMenuItem _selectionMarginToolStripMenuItem;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _formSizeStatusLabel;
    private ToolStripStatusLabel _formClientSizeStatusLabel;
    private ToolStripStatusLabel _selectedControlStatusLabel;
    private SplitContainer _splitContainer;
    private PropertyGrid _propertyGrid;
}
