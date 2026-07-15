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
        components = new System.ComponentModel.Container();
        _menuStrip = new MenuStrip();
        _fileToolStripMenuItem = new ToolStripMenuItem();
        _saveSettingsToolStripMenuItem = new ToolStripMenuItem();
        _loadSettingsToolStripMenuItem = new ToolStripMenuItem();
        _editToolStripMenuItem = new ToolStripMenuItem();
        _editModeToolStripMenuItem = new ToolStripMenuItem();
        _editSelectionSeparator = new ToolStripSeparator();
        _selectAllToolStripMenuItem = new ToolStripMenuItem();
        _deselectAllToolStripMenuItem = new ToolStripMenuItem();
        _viewToolStripMenuItem = new ToolStripMenuItem();
        _viewAppearanceSeparator = new ToolStripSeparator();
        _classicVisualStylesToolStripMenuItem = new ToolStripMenuItem();
        _net11VisualStylesToolStripMenuItem = new ToolStripMenuItem();
        _flatStyleSeparator = new ToolStripSeparator();
        _standardFlatStyleToolStripMenuItem = new ToolStripMenuItem();
        _flatFlatStyleToolStripMenuItem = new ToolStripMenuItem();
        _popupFlatStyleToolStripMenuItem = new ToolStripMenuItem();
        _systemFlatStyleToolStripMenuItem = new ToolStripMenuItem();
        _toolStrip = new ToolStrip();
        _saveSettingsToolStripButton = new ToolStripButton();
        _loadSettingsToolStripButton = new ToolStripButton();
        _fileToolStripSeparator = new ToolStripSeparator();
        _editModeToolStripButton = new ToolStripButton();
        _editToolStripSeparator = new ToolStripSeparator();
        _selectAllToolStripButton = new ToolStripButton();
        _deselectAllToolStripButton = new ToolStripButton();
        _statusStrip = new StatusStrip();
        _formSizeStatusLabel = new ToolStripStatusLabel();
        _formClientSizeStatusLabel = new ToolStripStatusLabel();
        _selectedControlStatusLabel = new ToolStripStatusLabel();
        _displayScaleStatusLabel = new ToolStripStatusLabel();
        _textScaleStatusLabel = new ToolStripStatusLabel();
        _accentColorStatusLabel = new ToolStripStatusLabel();
        _accentColorSwatchStatusLabel = new ToolStripStatusLabel();
        _splitContainer = new SplitContainer();
        _propertyGrid = new PropertyGrid();
        _iconFactoryComponent = new VisualStylesModeDemo.Components.IconFactoryComponent(components);
        _systemAppearanceTimer = new System.Windows.Forms.Timer(components);
        _menuStrip.SuspendLayout();
        _toolStrip.SuspendLayout();
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
        _menuStrip.Size = new Size(1405, 44);
        _menuStrip.TabIndex = 0;
        // 
        // _fileToolStripMenuItem
        // 
        _fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _saveSettingsToolStripMenuItem, _loadSettingsToolStripMenuItem });
        _fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
        _fileToolStripMenuItem.Size = new Size(72, 40);
        _fileToolStripMenuItem.Text = "&File";
        // 
        // _saveSettingsToolStripMenuItem
        // 
        _saveSettingsToolStripMenuItem.Name = "_saveSettingsToolStripMenuItem";
        _saveSettingsToolStripMenuItem.Size = new Size(411, 44);
        _saveSettingsToolStripMenuItem.Text = "&Save property settings...";
        _saveSettingsToolStripMenuItem.Click += SaveSettingsToolStripMenuItem_Click;
        // 
        // _loadSettingsToolStripMenuItem
        // 
        _loadSettingsToolStripMenuItem.Name = "_loadSettingsToolStripMenuItem";
        _loadSettingsToolStripMenuItem.Size = new Size(411, 44);
        _loadSettingsToolStripMenuItem.Text = "&Load property settings...";
        _loadSettingsToolStripMenuItem.Click += LoadSettingsToolStripMenuItem_Click;
        // 
        // _editToolStripMenuItem
        // 
        _editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _editModeToolStripMenuItem, _editSelectionSeparator, _selectAllToolStripMenuItem, _deselectAllToolStripMenuItem });
        _editToolStripMenuItem.Name = "_editToolStripMenuItem";
        _editToolStripMenuItem.Size = new Size(76, 40);
        _editToolStripMenuItem.Text = "&Edit";
        // 
        // _editModeToolStripMenuItem
        // 
        _editModeToolStripMenuItem.Name = "_editModeToolStripMenuItem";
        _editModeToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;
        _editModeToolStripMenuItem.Size = new Size(429, 44);
        _editModeToolStripMenuItem.Text = "&Edit mode";
        _editModeToolStripMenuItem.Click += EditModeToolStripMenuItem_Click;
        // 
        // _editSelectionSeparator
        // 
        _editSelectionSeparator.Name = "_editSelectionSeparator";
        _editSelectionSeparator.Size = new Size(426, 6);
        // 
        // _selectAllToolStripMenuItem
        // 
        _selectAllToolStripMenuItem.Enabled = false;
        _selectAllToolStripMenuItem.Name = "_selectAllToolStripMenuItem";
        _selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
        _selectAllToolStripMenuItem.Size = new Size(429, 44);
        _selectAllToolStripMenuItem.Text = "Select &All";
        _selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItem_Click;
        // 
        // _deselectAllToolStripMenuItem
        // 
        _deselectAllToolStripMenuItem.Enabled = false;
        _deselectAllToolStripMenuItem.Name = "_deselectAllToolStripMenuItem";
        _deselectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.A;
        _deselectAllToolStripMenuItem.Size = new Size(429, 44);
        _deselectAllToolStripMenuItem.Text = "&Deselect All";
        _deselectAllToolStripMenuItem.Click += DeselectAllToolStripMenuItem_Click;
        // 
        // _viewToolStripMenuItem
        // 
        _viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _viewAppearanceSeparator, _classicVisualStylesToolStripMenuItem, _net11VisualStylesToolStripMenuItem, _flatStyleSeparator, _standardFlatStyleToolStripMenuItem, _flatFlatStyleToolStripMenuItem, _popupFlatStyleToolStripMenuItem, _systemFlatStyleToolStripMenuItem });
        _viewToolStripMenuItem.Name = "_viewToolStripMenuItem";
        _viewToolStripMenuItem.Size = new Size(88, 40);
        _viewToolStripMenuItem.Text = "&View";
        // 
        // _viewAppearanceSeparator
        // 
        _viewAppearanceSeparator.Name = "_viewAppearanceSeparator";
        _viewAppearanceSeparator.Size = new Size(312, 6);
        // 
        // _classicVisualStylesToolStripMenuItem
        // 
        _classicVisualStylesToolStripMenuItem.Margin = new Padding(0, 15, 0, 0);
        _classicVisualStylesToolStripMenuItem.Name = "_classicVisualStylesToolStripMenuItem";
        _classicVisualStylesToolStripMenuItem.Size = new Size(315, 44);
        _classicVisualStylesToolStripMenuItem.Text = "&Classic";
        _classicVisualStylesToolStripMenuItem.Click += ClassicVisualStylesToolStripMenuItem_Click;
        // 
        // _net11VisualStylesToolStripMenuItem
        // 
        _net11VisualStylesToolStripMenuItem.Checked = true;
        _net11VisualStylesToolStripMenuItem.CheckState = CheckState.Checked;
        _net11VisualStylesToolStripMenuItem.Name = "_net11VisualStylesToolStripMenuItem";
        _net11VisualStylesToolStripMenuItem.Size = new Size(315, 44);
        _net11VisualStylesToolStripMenuItem.Text = "&Net 11+";
        _net11VisualStylesToolStripMenuItem.Click += Net11VisualStylesToolStripMenuItem_Click;
        // 
        // _flatStyleSeparator
        // 
        _flatStyleSeparator.Name = "_flatStyleSeparator";
        _flatStyleSeparator.Size = new Size(312, 6);
        // 
        // _standardFlatStyleToolStripMenuItem
        // 
        _standardFlatStyleToolStripMenuItem.Checked = true;
        _standardFlatStyleToolStripMenuItem.CheckState = CheckState.Checked;
        _standardFlatStyleToolStripMenuItem.Enabled = false;
        _standardFlatStyleToolStripMenuItem.Margin = new Padding(0, 15, 0, 0);
        _standardFlatStyleToolStripMenuItem.Name = "_standardFlatStyleToolStripMenuItem";
        _standardFlatStyleToolStripMenuItem.Padding = new Padding(0, 5, 0, 2);
        _standardFlatStyleToolStripMenuItem.Size = new Size(315, 47);
        _standardFlatStyleToolStripMenuItem.Text = "&Standard";
        _standardFlatStyleToolStripMenuItem.Click += StandardFlatStyleToolStripMenuItem_Click;
        // 
        // _flatFlatStyleToolStripMenuItem
        // 
        _flatFlatStyleToolStripMenuItem.Enabled = false;
        _flatFlatStyleToolStripMenuItem.Name = "_flatFlatStyleToolStripMenuItem";
        _flatFlatStyleToolStripMenuItem.Size = new Size(315, 44);
        _flatFlatStyleToolStripMenuItem.Text = "&Flat";
        _flatFlatStyleToolStripMenuItem.Click += FlatFlatStyleToolStripMenuItem_Click;
        // 
        // _popupFlatStyleToolStripMenuItem
        // 
        _popupFlatStyleToolStripMenuItem.Enabled = false;
        _popupFlatStyleToolStripMenuItem.Name = "_popupFlatStyleToolStripMenuItem";
        _popupFlatStyleToolStripMenuItem.Size = new Size(315, 44);
        _popupFlatStyleToolStripMenuItem.Text = "&Popup";
        _popupFlatStyleToolStripMenuItem.Click += PopupFlatStyleToolStripMenuItem_Click;
        // 
        // _systemFlatStyleToolStripMenuItem
        // 
        _systemFlatStyleToolStripMenuItem.Enabled = false;
        _systemFlatStyleToolStripMenuItem.Name = "_systemFlatStyleToolStripMenuItem";
        _systemFlatStyleToolStripMenuItem.Size = new Size(315, 44);
        _systemFlatStyleToolStripMenuItem.Text = "S&ystem";
        _systemFlatStyleToolStripMenuItem.Click += SystemFlatStyleToolStripMenuItem_Click;
        // 
        // _toolStrip
        // 
        _toolStrip.ImageScalingSize = new Size(36, 36);
        _toolStrip.Items.AddRange(new ToolStripItem[] { _saveSettingsToolStripButton, _loadSettingsToolStripButton, _fileToolStripSeparator, _editModeToolStripButton, _editToolStripSeparator, _selectAllToolStripButton, _deselectAllToolStripButton });
        _toolStrip.Location = new Point(0, 44);
        _toolStrip.Name = "_toolStrip";
        _toolStrip.Size = new Size(1405, 25);
        _toolStrip.TabIndex = 1;
        // 
        // _saveSettingsToolStripButton
        // 
        _saveSettingsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _saveSettingsToolStripButton.Enabled = false;
        _saveSettingsToolStripButton.Name = "_saveSettingsToolStripButton";
        _saveSettingsToolStripButton.Size = new Size(40, 19);
        _saveSettingsToolStripButton.Text = "Save property settings";
        _saveSettingsToolStripButton.ToolTipText = "Save property settings";
        _saveSettingsToolStripButton.Click += SaveSettingsToolStripMenuItem_Click;
        // 
        // _loadSettingsToolStripButton
        // 
        _loadSettingsToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _loadSettingsToolStripButton.Name = "_loadSettingsToolStripButton";
        _loadSettingsToolStripButton.Size = new Size(40, 19);
        _loadSettingsToolStripButton.Text = "Load property settings";
        _loadSettingsToolStripButton.ToolTipText = "Load property settings";
        _loadSettingsToolStripButton.Click += LoadSettingsToolStripMenuItem_Click;
        // 
        // _fileToolStripSeparator
        // 
        _fileToolStripSeparator.Name = "_fileToolStripSeparator";
        _fileToolStripSeparator.Size = new Size(6, 25);
        // 
        // _editModeToolStripButton
        // 
        _editModeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _editModeToolStripButton.Name = "_editModeToolStripButton";
        _editModeToolStripButton.Size = new Size(40, 19);
        _editModeToolStripButton.Text = "Edit mode";
        _editModeToolStripButton.ToolTipText = "Toggle Edit mode (Ctrl+E)";
        _editModeToolStripButton.Click += EditModeToolStripMenuItem_Click;
        // 
        // _editToolStripSeparator
        // 
        _editToolStripSeparator.Name = "_editToolStripSeparator";
        _editToolStripSeparator.Size = new Size(6, 25);
        // 
        // _selectAllToolStripButton
        // 
        _selectAllToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _selectAllToolStripButton.Enabled = false;
        _selectAllToolStripButton.Name = "_selectAllToolStripButton";
        _selectAllToolStripButton.Size = new Size(40, 19);
        _selectAllToolStripButton.Text = "Select All";
        _selectAllToolStripButton.ToolTipText = "Select All (Ctrl+A)";
        _selectAllToolStripButton.Click += SelectAllToolStripMenuItem_Click;
        // 
        // _deselectAllToolStripButton
        // 
        _deselectAllToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _deselectAllToolStripButton.Enabled = false;
        _deselectAllToolStripButton.Name = "_deselectAllToolStripButton";
        _deselectAllToolStripButton.Size = new Size(40, 19);
        _deselectAllToolStripButton.Text = "Deselect All";
        _deselectAllToolStripButton.ToolTipText = "Deselect All (Ctrl+Shift+A)";
        _deselectAllToolStripButton.Click += DeselectAllToolStripMenuItem_Click;
        // 
        // _statusStrip
        // 
        _statusStrip.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _statusStrip.ImageScalingSize = new Size(24, 24);
        _statusStrip.Items.AddRange(new ToolStripItem[] { _formSizeStatusLabel, _formClientSizeStatusLabel, _selectedControlStatusLabel, _displayScaleStatusLabel, _textScaleStatusLabel, _accentColorStatusLabel, _accentColorSwatchStatusLabel });
        _statusStrip.Location = new Point(0, 668);
        _statusStrip.Name = "_statusStrip";
        _statusStrip.Size = new Size(1405, 45);
        _statusStrip.TabIndex = 2;
        // 
        // _formSizeStatusLabel
        // 
        _formSizeStatusLabel.Name = "_formSizeStatusLabel";
        _formSizeStatusLabel.Size = new Size(130, 36);
        _formSizeStatusLabel.Text = "Form size:";
        // 
        // _formClientSizeStatusLabel
        // 
        _formClientSizeStatusLabel.Name = "_formClientSizeStatusLabel";
        _formClientSizeStatusLabel.Size = new Size(199, 36);
        _formClientSizeStatusLabel.Text = "Form client size:";
        // 
        // _selectedControlStatusLabel
        // 
        _selectedControlStatusLabel.Name = "_selectedControlStatusLabel";
        _selectedControlStatusLabel.Size = new Size(471, 36);
        _selectedControlStatusLabel.Spring = true;
        _selectedControlStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // _displayScaleStatusLabel
        // 
        _displayScaleStatusLabel.Name = "_displayScaleStatusLabel";
        _displayScaleStatusLabel.Size = new Size(173, 36);
        _displayScaleStatusLabel.Text = "Display: 100%";
        // 
        // _textScaleStatusLabel
        // 
        _textScaleStatusLabel.Name = "_textScaleStatusLabel";
        _textScaleStatusLabel.Size = new Size(137, 36);
        _textScaleStatusLabel.Text = "Text: 100%";
        // 
        // _accentColorStatusLabel
        // 
        _accentColorStatusLabel.Name = "_accentColorStatusLabel";
        _accentColorStatusLabel.Size = new Size(232, 36);
        _accentColorStatusLabel.Text = "Accent: #FF000000";
        // 
        // _accentColorSwatchStatusLabel
        // 
        _accentColorSwatchStatusLabel.AutoSize = false;
        _accentColorSwatchStatusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
        _accentColorSwatchStatusLabel.Name = "_accentColorSwatchStatusLabel";
        _accentColorSwatchStatusLabel.Size = new Size(36, 36);
        _accentColorSwatchStatusLabel.ToolTipText = "Windows accent color";
        // 
        // _splitContainer
        // 
        _splitContainer.Dock = DockStyle.Fill;
        _splitContainer.Location = new Point(0, 69);
        _splitContainer.Margin = new Padding(4);
        _splitContainer.Name = "_splitContainer";
        // 
        // _splitContainer.Panel1
        // 
        _splitContainer.Panel1.AutoScroll = true;
        _splitContainer.Panel1.Padding = new Padding(14);
        // 
        // _splitContainer.Panel2
        // 
        _splitContainer.Panel2.Controls.Add(_propertyGrid);
        _splitContainer.Size = new Size(1405, 599);
        _splitContainer.SplitterDistance = 955;
        _splitContainer.SplitterWidth = 5;
        _splitContainer.TabIndex = 1;
        // 
        // _propertyGrid
        // 
        _propertyGrid.BackColor = SystemColors.Control;
        _propertyGrid.Dock = DockStyle.Fill;
        _propertyGrid.Location = new Point(0, 0);
        _propertyGrid.Margin = new Padding(4);
        _propertyGrid.Name = "_propertyGrid";
        _propertyGrid.Size = new Size(445, 599);
        _propertyGrid.TabIndex = 0;
        _propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        // 
        // _systemAppearanceTimer
        // 
        _systemAppearanceTimer.Interval = 5000;
        _systemAppearanceTimer.Tick += SystemAppearanceTimer_Tick;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(14F, 36F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1405, 713);
        Controls.Add(_splitContainer);
        Controls.Add(_toolStrip);
        Controls.Add(_statusStrip);
        Controls.Add(_menuStrip);
        Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        MainMenuStrip = _menuStrip;
        Margin = new Padding(4);
        MinimumSize = new Size(1196, 769);
        Name = "MainForm";
        Text = "VisualStylesModeDemo - WinForms NET11 API scratchpad";
        DpiChanged += MainForm_DpiChanged;
        SystemTextSizeChanged += MainForm_SystemTextSizeChanged;
        _menuStrip.ResumeLayout(false);
        _menuStrip.PerformLayout();
        _toolStrip.ResumeLayout(false);
        _toolStrip.PerformLayout();
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
    private ToolStripMenuItem _editModeToolStripMenuItem;
    private ToolStripSeparator _editSelectionSeparator;
    private ToolStripMenuItem _selectAllToolStripMenuItem;
    private ToolStripMenuItem _deselectAllToolStripMenuItem;
    private ToolStripMenuItem _viewToolStripMenuItem;
    private ToolStripSeparator _viewAppearanceSeparator;
    private ToolStripMenuItem _classicVisualStylesToolStripMenuItem;
    private ToolStripMenuItem _net11VisualStylesToolStripMenuItem;
    private ToolStripSeparator _flatStyleSeparator;
    private ToolStripMenuItem _standardFlatStyleToolStripMenuItem;
    private ToolStripMenuItem _flatFlatStyleToolStripMenuItem;
    private ToolStripMenuItem _popupFlatStyleToolStripMenuItem;
    private ToolStripMenuItem _systemFlatStyleToolStripMenuItem;
    private ToolStrip _toolStrip;
    private ToolStripButton _saveSettingsToolStripButton;
    private ToolStripButton _loadSettingsToolStripButton;
    private ToolStripSeparator _fileToolStripSeparator;
    private ToolStripButton _editModeToolStripButton;
    private ToolStripSeparator _editToolStripSeparator;
    private ToolStripButton _selectAllToolStripButton;
    private ToolStripButton _deselectAllToolStripButton;
    private StatusStrip _statusStrip;
    private ToolStripStatusLabel _formSizeStatusLabel;
    private ToolStripStatusLabel _formClientSizeStatusLabel;
    private ToolStripStatusLabel _selectedControlStatusLabel;
    private ToolStripStatusLabel _displayScaleStatusLabel;
    private ToolStripStatusLabel _textScaleStatusLabel;
    private ToolStripStatusLabel _accentColorStatusLabel;
    private ToolStripStatusLabel _accentColorSwatchStatusLabel;
    private SplitContainer _splitContainer;
    private PropertyGrid _propertyGrid;
    private Components.IconFactoryComponent _iconFactoryComponent;
    private System.Windows.Forms.Timer _systemAppearanceTimer;
}
