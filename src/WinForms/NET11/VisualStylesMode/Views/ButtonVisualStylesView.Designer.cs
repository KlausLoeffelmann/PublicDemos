// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

partial class ButtonVisualStylesView
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

    #region Component Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _rootTableLayoutPanel = new TableLayoutPanel();
        _flatStyleGroupBox = new GroupBox();
        _flatStyleTableLayoutPanel = new TableLayoutPanel();
        _flatStyleStandardCheckBox = new CheckBox();
        _flatStyleStandardButton = new Button();
        _flatStylePopupCheckBox = new CheckBox();
        _flatStylePopupButton = new Button();
        _flatStyleFlatCheckBox = new CheckBox();
        _flatStyleFlatButton = new Button();
        _flatStyleSystemCheckBox = new CheckBox();
        _flatStyleSystemButton = new Button();
        _visualStylesGroupBox = new GroupBox();
        _visualStylesTableLayoutPanel = new TableLayoutPanel();
        _visualStylesClassicCheckBox = new CheckBox();
        _visualStylesClassicButton = new Button();
        _visualStylesNet11CheckBox = new CheckBox();
        _visualStylesNet11Button = new Button();
        _visualStylesLatestCheckBox = new CheckBox();
        _visualStylesLatestButton = new Button();
        _enabledStateGroupBox = new GroupBox();
        _enabledStateTableLayoutPanel = new TableLayoutPanel();
        _enabledButtonCheckBox = new CheckBox();
        _enabledButton = new Button();
        _disabledButtonCheckBox = new CheckBox();
        _disabledButton = new Button();
        _commandGroupBox = new GroupBox();
        _commandTableLayoutPanel = new TableLayoutPanel();
        _commandAlphaCheckBox = new CheckBox();
        _commandAlphaButton = new Button();
        _commandBetaCheckBox = new CheckBox();
        _commandBetaButton = new Button();
        _commandToggleEnabledCheckBox = new CheckBox();
        _commandToggleEnabledButton = new Button();
        _commandResultLabel = new Label();
        _backgroundImageGroupBox = new GroupBox();
        _backgroundImageTableLayoutPanel = new TableLayoutPanel();
        _backgroundImageTileCheckBox = new CheckBox();
        _backgroundImageStretchCheckBox = new CheckBox();
        _backgroundImageZoomCheckBox = new CheckBox();
        _backgroundImageCenterCheckBox = new CheckBox();
        _backgroundImageTileButton = new Button();
        _backgroundImageStretchButton = new Button();
        _backgroundImageZoomButton = new Button();
        _backgroundImageCenterButton = new Button();
        _rootTableLayoutPanel.SuspendLayout();
        _flatStyleGroupBox.SuspendLayout();
        _flatStyleTableLayoutPanel.SuspendLayout();
        _visualStylesGroupBox.SuspendLayout();
        _visualStylesTableLayoutPanel.SuspendLayout();
        _enabledStateGroupBox.SuspendLayout();
        _enabledStateTableLayoutPanel.SuspendLayout();
        _commandGroupBox.SuspendLayout();
        _commandTableLayoutPanel.SuspendLayout();
        _backgroundImageGroupBox.SuspendLayout();
        _backgroundImageTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _rootTableLayoutPanel
        // 
        _rootTableLayoutPanel.AutoSize = true;
        _rootTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootTableLayoutPanel.ColumnCount = 2;
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _rootTableLayoutPanel.Controls.Add(_flatStyleGroupBox, 0, 0);
        _rootTableLayoutPanel.Controls.Add(_visualStylesGroupBox, 1, 0);
        _rootTableLayoutPanel.Controls.Add(_enabledStateGroupBox, 0, 1);
        _rootTableLayoutPanel.Controls.Add(_commandGroupBox, 1, 1);
        _rootTableLayoutPanel.Controls.Add(_backgroundImageGroupBox, 0, 2);
        _rootTableLayoutPanel.Dock = DockStyle.Fill;
        _rootTableLayoutPanel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _rootTableLayoutPanel.Location = new Point(0, 0);
        _rootTableLayoutPanel.Name = "_rootTableLayoutPanel";
        _rootTableLayoutPanel.RowCount = 3;
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.Size = new Size(922, 610);
        _rootTableLayoutPanel.TabIndex = 0;
        // 
        // _flatStyleGroupBox
        // 
        _flatStyleGroupBox.AutoSize = true;
        _flatStyleGroupBox.Controls.Add(_flatStyleTableLayoutPanel);
        _flatStyleGroupBox.Dock = DockStyle.Fill;
        _flatStyleGroupBox.Location = new Point(3, 3);
        _flatStyleGroupBox.Name = "_flatStyleGroupBox";
        _flatStyleGroupBox.Padding = new Padding(8);
        _flatStyleGroupBox.Size = new Size(520, 229);
        _flatStyleGroupBox.TabIndex = 0;
        _flatStyleGroupBox.TabStop = false;
        _flatStyleGroupBox.Text = "FlatStyle variations";
        // 
        // _flatStyleTableLayoutPanel
        // 
        _flatStyleTableLayoutPanel.AutoSize = true;
        _flatStyleTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleTableLayoutPanel.ColumnCount = 2;
        _flatStyleTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _flatStyleTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleStandardCheckBox, 0, 0);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleStandardButton, 1, 0);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStylePopupCheckBox, 0, 1);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStylePopupButton, 1, 1);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleFlatCheckBox, 0, 2);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleFlatButton, 1, 2);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleSystemCheckBox, 0, 3);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleSystemButton, 1, 3);
        _flatStyleTableLayoutPanel.Dock = DockStyle.Fill;
        _flatStyleTableLayoutPanel.Location = new Point(8, 38);
        _flatStyleTableLayoutPanel.Name = "_flatStyleTableLayoutPanel";
        _flatStyleTableLayoutPanel.RowCount = 4;
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.Size = new Size(504, 183);
        _flatStyleTableLayoutPanel.TabIndex = 0;
        // 
        // _flatStyleStandardCheckBox
        // 
        _flatStyleStandardCheckBox.Anchor = AnchorStyles.Left;
        _flatStyleStandardCheckBox.AutoSize = true;
        _flatStyleStandardCheckBox.Location = new Point(3, 7);
        _flatStyleStandardCheckBox.Margin = new Padding(3, 6, 3, 3);
        _flatStyleStandardCheckBox.Name = "_flatStyleStandardCheckBox";
        _flatStyleStandardCheckBox.Size = new Size(125, 34);
        _flatStyleStandardCheckBox.TabIndex = 0;
        _flatStyleStandardCheckBox.Text = "Standard";
        // 
        // _flatStyleStandardButton
        // 
        _flatStyleStandardButton.Anchor = AnchorStyles.Left;
        _flatStyleStandardButton.AutoSize = true;
        _flatStyleStandardButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleStandardButton.DialogResult = DialogResult.OK;
        _flatStyleStandardButton.Location = new Point(134, 3);
        _flatStyleStandardButton.Name = "_flatStyleStandardButton";
        _flatStyleStandardButton.Size = new Size(109, 40);
        _flatStyleStandardButton.TabIndex = 1;
        _flatStyleStandardButton.Text = "Standard";
        _flatStyleStandardButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStylePopupCheckBox
        // 
        _flatStylePopupCheckBox.Anchor = AnchorStyles.Left;
        _flatStylePopupCheckBox.AutoSize = true;
        _flatStylePopupCheckBox.Location = new Point(3, 53);
        _flatStylePopupCheckBox.Margin = new Padding(3, 6, 3, 3);
        _flatStylePopupCheckBox.Name = "_flatStylePopupCheckBox";
        _flatStylePopupCheckBox.Size = new Size(101, 34);
        _flatStylePopupCheckBox.TabIndex = 2;
        _flatStylePopupCheckBox.Text = "Popup";
        // 
        // _flatStylePopupButton
        // 
        _flatStylePopupButton.Anchor = AnchorStyles.Left;
        _flatStylePopupButton.AutoSize = true;
        _flatStylePopupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStylePopupButton.FlatStyle = FlatStyle.Popup;
        _flatStylePopupButton.Location = new Point(134, 49);
        _flatStylePopupButton.Name = "_flatStylePopupButton";
        _flatStylePopupButton.Size = new Size(85, 40);
        _flatStylePopupButton.TabIndex = 3;
        _flatStylePopupButton.Text = "Popup";
        _flatStylePopupButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStyleFlatCheckBox
        // 
        _flatStyleFlatCheckBox.Anchor = AnchorStyles.Left;
        _flatStyleFlatCheckBox.AutoSize = true;
        _flatStyleFlatCheckBox.Location = new Point(3, 99);
        _flatStyleFlatCheckBox.Margin = new Padding(3, 6, 3, 3);
        _flatStyleFlatCheckBox.Name = "_flatStyleFlatCheckBox";
        _flatStyleFlatCheckBox.Size = new Size(73, 34);
        _flatStyleFlatCheckBox.TabIndex = 4;
        _flatStyleFlatCheckBox.Text = "Flat";
        // 
        // _flatStyleFlatButton
        // 
        _flatStyleFlatButton.Anchor = AnchorStyles.Left;
        _flatStyleFlatButton.AutoSize = true;
        _flatStyleFlatButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleFlatButton.FlatStyle = FlatStyle.Flat;
        _flatStyleFlatButton.Location = new Point(134, 95);
        _flatStyleFlatButton.Name = "_flatStyleFlatButton";
        _flatStyleFlatButton.Size = new Size(57, 40);
        _flatStyleFlatButton.TabIndex = 5;
        _flatStyleFlatButton.Text = "Flat";
        _flatStyleFlatButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStyleSystemCheckBox
        // 
        _flatStyleSystemCheckBox.Anchor = AnchorStyles.Left;
        _flatStyleSystemCheckBox.AutoSize = true;
        _flatStyleSystemCheckBox.Location = new Point(3, 145);
        _flatStyleSystemCheckBox.Margin = new Padding(3, 6, 3, 3);
        _flatStyleSystemCheckBox.Name = "_flatStyleSystemCheckBox";
        _flatStyleSystemCheckBox.Size = new Size(108, 34);
        _flatStyleSystemCheckBox.TabIndex = 6;
        _flatStyleSystemCheckBox.Text = "System";
        // 
        // _flatStyleSystemButton
        // 
        _flatStyleSystemButton.Anchor = AnchorStyles.Left;
        _flatStyleSystemButton.AutoSize = true;
        _flatStyleSystemButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleSystemButton.FlatStyle = FlatStyle.System;
        _flatStyleSystemButton.Location = new Point(134, 141);
        _flatStyleSystemButton.Name = "_flatStyleSystemButton";
        _flatStyleSystemButton.Size = new Size(96, 39);
        _flatStyleSystemButton.TabIndex = 7;
        _flatStyleSystemButton.Text = "System";
        _flatStyleSystemButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _visualStylesGroupBox
        // 
        _visualStylesGroupBox.AutoSize = true;
        _visualStylesGroupBox.Controls.Add(_visualStylesTableLayoutPanel);
        _visualStylesGroupBox.Dock = DockStyle.Fill;
        _visualStylesGroupBox.Location = new Point(529, 3);
        _visualStylesGroupBox.Name = "_visualStylesGroupBox";
        _visualStylesGroupBox.Padding = new Padding(8);
        _visualStylesGroupBox.Size = new Size(390, 229);
        _visualStylesGroupBox.TabIndex = 1;
        _visualStylesGroupBox.TabStop = false;
        _visualStylesGroupBox.Text = "Per-control VisualStylesMode";
        // 
        // _visualStylesTableLayoutPanel
        // 
        _visualStylesTableLayoutPanel.AutoSize = true;
        _visualStylesTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _visualStylesTableLayoutPanel.ColumnCount = 2;
        _visualStylesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _visualStylesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesClassicCheckBox, 0, 0);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesClassicButton, 1, 0);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesNet11CheckBox, 0, 1);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesNet11Button, 1, 1);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesLatestCheckBox, 0, 2);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesLatestButton, 1, 2);
        _visualStylesTableLayoutPanel.Dock = DockStyle.Fill;
        _visualStylesTableLayoutPanel.Location = new Point(8, 38);
        _visualStylesTableLayoutPanel.Name = "_visualStylesTableLayoutPanel";
        _visualStylesTableLayoutPanel.RowCount = 3;
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.Size = new Size(374, 183);
        _visualStylesTableLayoutPanel.TabIndex = 0;
        // 
        // _visualStylesClassicCheckBox
        // 
        _visualStylesClassicCheckBox.Anchor = AnchorStyles.Left;
        _visualStylesClassicCheckBox.AutoSize = true;
        _visualStylesClassicCheckBox.Location = new Point(3, 7);
        _visualStylesClassicCheckBox.Margin = new Padding(3, 6, 3, 3);
        _visualStylesClassicCheckBox.Name = "_visualStylesClassicCheckBox";
        _visualStylesClassicCheckBox.Size = new Size(102, 34);
        _visualStylesClassicCheckBox.TabIndex = 0;
        _visualStylesClassicCheckBox.Text = "Classic";
        // 
        // _visualStylesClassicButton
        // 
        _visualStylesClassicButton.Anchor = AnchorStyles.Left;
        _visualStylesClassicButton.AutoSize = true;
        _visualStylesClassicButton.Location = new Point(111, 3);
        _visualStylesClassicButton.Name = "_visualStylesClassicButton";
        _visualStylesClassicButton.Size = new Size(260, 40);
        _visualStylesClassicButton.TabIndex = 1;
        _visualStylesClassicButton.Text = "VisualStylesMode.Classic";
        _visualStylesClassicButton.VisualStylesMode = VisualStylesMode.Classic;
        // 
        // _visualStylesNet11CheckBox
        // 
        _visualStylesNet11CheckBox.Anchor = AnchorStyles.Left;
        _visualStylesNet11CheckBox.AutoSize = true;
        _visualStylesNet11CheckBox.Location = new Point(3, 53);
        _visualStylesNet11CheckBox.Margin = new Padding(3, 6, 3, 3);
        _visualStylesNet11CheckBox.Name = "_visualStylesNet11CheckBox";
        _visualStylesNet11CheckBox.Size = new Size(98, 34);
        _visualStylesNet11CheckBox.TabIndex = 2;
        _visualStylesNet11CheckBox.Text = "Net11";
        // 
        // _visualStylesNet11Button
        // 
        _visualStylesNet11Button.Anchor = AnchorStyles.Left;
        _visualStylesNet11Button.AutoSize = true;
        _visualStylesNet11Button.Location = new Point(111, 49);
        _visualStylesNet11Button.Name = "_visualStylesNet11Button";
        _visualStylesNet11Button.Size = new Size(256, 40);
        _visualStylesNet11Button.TabIndex = 3;
        _visualStylesNet11Button.Text = "VisualStylesMode.Net11";
        _visualStylesNet11Button.VisualStylesMode = VisualStylesMode.Net11;
        // 
        // _visualStylesLatestCheckBox
        // 
        _visualStylesLatestCheckBox.Anchor = AnchorStyles.Left;
        _visualStylesLatestCheckBox.AutoSize = true;
        _visualStylesLatestCheckBox.Location = new Point(3, 122);
        _visualStylesLatestCheckBox.Margin = new Padding(3, 6, 3, 3);
        _visualStylesLatestCheckBox.Name = "_visualStylesLatestCheckBox";
        _visualStylesLatestCheckBox.Size = new Size(95, 34);
        _visualStylesLatestCheckBox.TabIndex = 4;
        _visualStylesLatestCheckBox.Text = "Latest";
        // 
        // _visualStylesLatestButton
        // 
        _visualStylesLatestButton.Anchor = AnchorStyles.Left;
        _visualStylesLatestButton.AutoSize = true;
        _visualStylesLatestButton.Location = new Point(111, 117);
        _visualStylesLatestButton.Name = "_visualStylesLatestButton";
        _visualStylesLatestButton.Size = new Size(253, 40);
        _visualStylesLatestButton.TabIndex = 5;
        _visualStylesLatestButton.Text = "VisualStylesMode.Latest";
        _visualStylesLatestButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _enabledStateGroupBox
        // 
        _enabledStateGroupBox.AutoSize = true;
        _enabledStateGroupBox.Controls.Add(_enabledStateTableLayoutPanel);
        _enabledStateGroupBox.Dock = DockStyle.Fill;
        _enabledStateGroupBox.Location = new Point(3, 238);
        _enabledStateGroupBox.Name = "_enabledStateGroupBox";
        _enabledStateGroupBox.Padding = new Padding(8);
        _enabledStateGroupBox.Size = new Size(520, 214);
        _enabledStateGroupBox.TabIndex = 2;
        _enabledStateGroupBox.TabStop = false;
        _enabledStateGroupBox.Text = "Enabled / Disabled";
        // 
        // _enabledStateTableLayoutPanel
        // 
        _enabledStateTableLayoutPanel.AutoSize = true;
        _enabledStateTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _enabledStateTableLayoutPanel.ColumnCount = 2;
        _enabledStateTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _enabledStateTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _enabledStateTableLayoutPanel.Controls.Add(_enabledButtonCheckBox, 0, 0);
        _enabledStateTableLayoutPanel.Controls.Add(_enabledButton, 1, 0);
        _enabledStateTableLayoutPanel.Controls.Add(_disabledButtonCheckBox, 0, 1);
        _enabledStateTableLayoutPanel.Controls.Add(_disabledButton, 1, 1);
        _enabledStateTableLayoutPanel.Dock = DockStyle.Fill;
        _enabledStateTableLayoutPanel.Location = new Point(8, 38);
        _enabledStateTableLayoutPanel.Name = "_enabledStateTableLayoutPanel";
        _enabledStateTableLayoutPanel.RowCount = 2;
        _enabledStateTableLayoutPanel.RowStyles.Add(new RowStyle());
        _enabledStateTableLayoutPanel.RowStyles.Add(new RowStyle());
        _enabledStateTableLayoutPanel.Size = new Size(504, 168);
        _enabledStateTableLayoutPanel.TabIndex = 0;
        // 
        // _enabledButtonCheckBox
        // 
        _enabledButtonCheckBox.Anchor = AnchorStyles.Left;
        _enabledButtonCheckBox.AutoSize = true;
        _enabledButtonCheckBox.Location = new Point(3, 6);
        _enabledButtonCheckBox.Margin = new Padding(3, 6, 3, 3);
        _enabledButtonCheckBox.Name = "_enabledButtonCheckBox";
        _enabledButtonCheckBox.Size = new Size(116, 34);
        _enabledButtonCheckBox.TabIndex = 0;
        _enabledButtonCheckBox.Text = "Enabled";
        // 
        // _enabledButton
        // 
        _enabledButton.Anchor = AnchorStyles.Left;
        _enabledButton.Location = new Point(131, 3);
        _enabledButton.Name = "_enabledButton";
        _enabledButton.Size = new Size(140, 38);
        _enabledButton.TabIndex = 1;
        _enabledButton.Text = "Enabled button";
        // 
        // _disabledButtonCheckBox
        // 
        _disabledButtonCheckBox.Anchor = AnchorStyles.Left;
        _disabledButtonCheckBox.AutoSize = true;
        _disabledButtonCheckBox.Location = new Point(3, 90);
        _disabledButtonCheckBox.Margin = new Padding(3, 6, 3, 3);
        _disabledButtonCheckBox.Name = "_disabledButtonCheckBox";
        _disabledButtonCheckBox.Size = new Size(122, 34);
        _disabledButtonCheckBox.TabIndex = 2;
        _disabledButtonCheckBox.Text = "Disabled";
        // 
        // _disabledButton
        // 
        _disabledButton.Anchor = AnchorStyles.Left;
        _disabledButton.Enabled = false;
        _disabledButton.Location = new Point(131, 87);
        _disabledButton.Name = "_disabledButton";
        _disabledButton.Size = new Size(140, 38);
        _disabledButton.TabIndex = 3;
        _disabledButton.Text = "Disabled button";
        // 
        // _commandGroupBox
        // 
        _commandGroupBox.AutoSize = true;
        _commandGroupBox.Controls.Add(_commandTableLayoutPanel);
        _commandGroupBox.Dock = DockStyle.Fill;
        _commandGroupBox.Location = new Point(529, 238);
        _commandGroupBox.Name = "_commandGroupBox";
        _commandGroupBox.Padding = new Padding(8);
        _commandGroupBox.Size = new Size(390, 214);
        _commandGroupBox.TabIndex = 3;
        _commandGroupBox.TabStop = false;
        _commandGroupBox.Text = "Command / CommandParameter";
        // 
        // _commandTableLayoutPanel
        // 
        _commandTableLayoutPanel.AutoSize = true;
        _commandTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _commandTableLayoutPanel.ColumnCount = 2;
        _commandTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _commandTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _commandTableLayoutPanel.Controls.Add(_commandAlphaCheckBox, 0, 0);
        _commandTableLayoutPanel.Controls.Add(_commandAlphaButton, 1, 0);
        _commandTableLayoutPanel.Controls.Add(_commandBetaCheckBox, 0, 1);
        _commandTableLayoutPanel.Controls.Add(_commandBetaButton, 1, 1);
        _commandTableLayoutPanel.Controls.Add(_commandToggleEnabledCheckBox, 0, 2);
        _commandTableLayoutPanel.Controls.Add(_commandToggleEnabledButton, 1, 2);
        _commandTableLayoutPanel.Controls.Add(_commandResultLabel, 0, 3);
        _commandTableLayoutPanel.Dock = DockStyle.Fill;
        _commandTableLayoutPanel.Location = new Point(8, 38);
        _commandTableLayoutPanel.Name = "_commandTableLayoutPanel";
        _commandTableLayoutPanel.RowCount = 4;
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.Size = new Size(374, 168);
        _commandTableLayoutPanel.TabIndex = 0;
        // 
        // _commandAlphaCheckBox
        // 
        _commandAlphaCheckBox.Anchor = AnchorStyles.Left;
        _commandAlphaCheckBox.AutoSize = true;
        _commandAlphaCheckBox.Location = new Point(3, 6);
        _commandAlphaCheckBox.Margin = new Padding(3, 6, 3, 3);
        _commandAlphaCheckBox.Name = "_commandAlphaCheckBox";
        _commandAlphaCheckBox.Size = new Size(94, 34);
        _commandAlphaCheckBox.TabIndex = 0;
        _commandAlphaCheckBox.Text = "Alpha";
        // 
        // _commandAlphaButton
        // 
        _commandAlphaButton.Anchor = AnchorStyles.Left;
        _commandAlphaButton.Location = new Point(179, 5);
        _commandAlphaButton.Name = "_commandAlphaButton";
        _commandAlphaButton.Size = new Size(160, 32);
        _commandAlphaButton.TabIndex = 1;
        _commandAlphaButton.Text = "Run Command (Alpha)";
        // 
        // _commandBetaCheckBox
        // 
        _commandBetaCheckBox.Anchor = AnchorStyles.Left;
        _commandBetaCheckBox.AutoSize = true;
        _commandBetaCheckBox.Location = new Point(3, 49);
        _commandBetaCheckBox.Margin = new Padding(3, 6, 3, 3);
        _commandBetaCheckBox.Name = "_commandBetaCheckBox";
        _commandBetaCheckBox.Size = new Size(82, 34);
        _commandBetaCheckBox.TabIndex = 2;
        _commandBetaCheckBox.Text = "Beta";
        // 
        // _commandBetaButton
        // 
        _commandBetaButton.Anchor = AnchorStyles.Left;
        _commandBetaButton.Location = new Point(179, 48);
        _commandBetaButton.Name = "_commandBetaButton";
        _commandBetaButton.Size = new Size(160, 32);
        _commandBetaButton.TabIndex = 3;
        _commandBetaButton.Text = "Run Command (Beta)";
        // 
        // _commandToggleEnabledCheckBox
        // 
        _commandToggleEnabledCheckBox.Anchor = AnchorStyles.Left;
        _commandToggleEnabledCheckBox.AutoSize = true;
        _commandToggleEnabledCheckBox.Location = new Point(3, 92);
        _commandToggleEnabledCheckBox.Margin = new Padding(3, 6, 3, 3);
        _commandToggleEnabledCheckBox.Name = "_commandToggleEnabledCheckBox";
        _commandToggleEnabledCheckBox.Size = new Size(170, 34);
        _commandToggleEnabledCheckBox.TabIndex = 4;
        _commandToggleEnabledCheckBox.Text = "Toggle switch";
        // 
        // _commandToggleEnabledButton
        // 
        _commandToggleEnabledButton.Anchor = AnchorStyles.Left;
        _commandToggleEnabledButton.Location = new Point(179, 91);
        _commandToggleEnabledButton.Name = "_commandToggleEnabledButton";
        _commandToggleEnabledButton.Size = new Size(160, 32);
        _commandToggleEnabledButton.TabIndex = 5;
        _commandToggleEnabledButton.Text = "Toggle CanExecute";
        // 
        // _commandResultLabel
        // 
        _commandResultLabel.Anchor = AnchorStyles.Left;
        _commandResultLabel.AutoSize = true;
        _commandTableLayoutPanel.SetColumnSpan(_commandResultLabel, 2);
        _commandResultLabel.Location = new Point(3, 135);
        _commandResultLabel.Margin = new Padding(3, 6, 3, 3);
        _commandResultLabel.Name = "_commandResultLabel";
        _commandResultLabel.Size = new Size(286, 30);
        _commandResultLabel.TabIndex = 6;
        _commandResultLabel.Text = "Last command result: (none)";
        // 
        // _backgroundImageGroupBox
        // 
        _backgroundImageGroupBox.AutoSize = true;
        _backgroundImageGroupBox.Controls.Add(_backgroundImageTableLayoutPanel);
        _backgroundImageGroupBox.Dock = DockStyle.Fill;
        _backgroundImageGroupBox.Location = new Point(3, 458);
        _backgroundImageGroupBox.Name = "_backgroundImageGroupBox";
        _backgroundImageGroupBox.Padding = new Padding(8);
        _backgroundImageGroupBox.Size = new Size(520, 149);
        _backgroundImageGroupBox.TabIndex = 4;
        _backgroundImageGroupBox.TabStop = false;
        _backgroundImageGroupBox.Text = "BackgroundImage / BackgroundImageLayout";
        // 
        // _backgroundImageTableLayoutPanel
        // 
        _backgroundImageTableLayoutPanel.AutoSize = true;
        _backgroundImageTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _backgroundImageTableLayoutPanel.ColumnCount = 4;
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageTileCheckBox, 0, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageStretchCheckBox, 1, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageZoomCheckBox, 2, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageCenterCheckBox, 3, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageTileButton, 0, 1);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageStretchButton, 1, 1);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageZoomButton, 2, 1);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageCenterButton, 3, 1);
        _backgroundImageTableLayoutPanel.Dock = DockStyle.Fill;
        _backgroundImageTableLayoutPanel.Location = new Point(8, 38);
        _backgroundImageTableLayoutPanel.Name = "_backgroundImageTableLayoutPanel";
        _backgroundImageTableLayoutPanel.RowCount = 2;
        _backgroundImageTableLayoutPanel.RowStyles.Add(new RowStyle());
        _backgroundImageTableLayoutPanel.RowStyles.Add(new RowStyle());
        _backgroundImageTableLayoutPanel.Size = new Size(504, 103);
        _backgroundImageTableLayoutPanel.TabIndex = 0;
        // 
        // _backgroundImageTileCheckBox
        // 
        _backgroundImageTileCheckBox.Anchor = AnchorStyles.Left;
        _backgroundImageTileCheckBox.AutoSize = true;
        _backgroundImageTileCheckBox.Location = new Point(3, 3);
        _backgroundImageTileCheckBox.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageTileCheckBox.Name = "_backgroundImageTileCheckBox";
        _backgroundImageTileCheckBox.Size = new Size(73, 34);
        _backgroundImageTileCheckBox.TabIndex = 0;
        _backgroundImageTileCheckBox.Text = "Tile";
        // 
        // _backgroundImageStretchCheckBox
        // 
        _backgroundImageStretchCheckBox.Anchor = AnchorStyles.Left;
        _backgroundImageStretchCheckBox.AutoSize = true;
        _backgroundImageStretchCheckBox.Location = new Point(129, 3);
        _backgroundImageStretchCheckBox.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageStretchCheckBox.Name = "_backgroundImageStretchCheckBox";
        _backgroundImageStretchCheckBox.Size = new Size(106, 34);
        _backgroundImageStretchCheckBox.TabIndex = 1;
        _backgroundImageStretchCheckBox.Text = "Stretch";
        // 
        // _backgroundImageZoomCheckBox
        // 
        _backgroundImageZoomCheckBox.Anchor = AnchorStyles.Left;
        _backgroundImageZoomCheckBox.AutoSize = true;
        _backgroundImageZoomCheckBox.Location = new Point(255, 3);
        _backgroundImageZoomCheckBox.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageZoomCheckBox.Name = "_backgroundImageZoomCheckBox";
        _backgroundImageZoomCheckBox.Size = new Size(97, 34);
        _backgroundImageZoomCheckBox.TabIndex = 2;
        _backgroundImageZoomCheckBox.Text = "Zoom";
        // 
        // _backgroundImageCenterCheckBox
        // 
        _backgroundImageCenterCheckBox.Anchor = AnchorStyles.Left;
        _backgroundImageCenterCheckBox.AutoSize = true;
        _backgroundImageCenterCheckBox.Location = new Point(381, 3);
        _backgroundImageCenterCheckBox.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageCenterCheckBox.Name = "_backgroundImageCenterCheckBox";
        _backgroundImageCenterCheckBox.Size = new Size(104, 34);
        _backgroundImageCenterCheckBox.TabIndex = 3;
        _backgroundImageCenterCheckBox.Text = "Center";
        // 
        // _backgroundImageTileButton
        // 
        _backgroundImageTileButton.Anchor = AnchorStyles.Left;
        _backgroundImageTileButton.AutoSize = true;
        _backgroundImageTileButton.FlatStyle = FlatStyle.Flat;
        _backgroundImageTileButton.ForeColor = Color.White;
        _backgroundImageTileButton.Location = new Point(3, 40);
        _backgroundImageTileButton.Name = "_backgroundImageTileButton";
        _backgroundImageTileButton.Size = new Size(120, 60);
        _backgroundImageTileButton.TabIndex = 4;
        _backgroundImageTileButton.Text = "Tile";
        // 
        // _backgroundImageStretchButton
        // 
        _backgroundImageStretchButton.Anchor = AnchorStyles.Left;
        _backgroundImageStretchButton.AutoSize = true;
        _backgroundImageStretchButton.BackgroundImageLayout = ImageLayout.Stretch;
        _backgroundImageStretchButton.FlatStyle = FlatStyle.Flat;
        _backgroundImageStretchButton.ForeColor = Color.White;
        _backgroundImageStretchButton.Location = new Point(129, 40);
        _backgroundImageStretchButton.Name = "_backgroundImageStretchButton";
        _backgroundImageStretchButton.Size = new Size(120, 60);
        _backgroundImageStretchButton.TabIndex = 5;
        _backgroundImageStretchButton.Text = "Stretch";
        // 
        // _backgroundImageZoomButton
        // 
        _backgroundImageZoomButton.Anchor = AnchorStyles.Left;
        _backgroundImageZoomButton.AutoSize = true;
        _backgroundImageZoomButton.BackgroundImageLayout = ImageLayout.Zoom;
        _backgroundImageZoomButton.FlatStyle = FlatStyle.Flat;
        _backgroundImageZoomButton.ForeColor = Color.White;
        _backgroundImageZoomButton.Location = new Point(255, 40);
        _backgroundImageZoomButton.Name = "_backgroundImageZoomButton";
        _backgroundImageZoomButton.Size = new Size(120, 60);
        _backgroundImageZoomButton.TabIndex = 6;
        _backgroundImageZoomButton.Text = "Zoom";
        // 
        // _backgroundImageCenterButton
        // 
        _backgroundImageCenterButton.Anchor = AnchorStyles.Left;
        _backgroundImageCenterButton.AutoSize = true;
        _backgroundImageCenterButton.BackgroundImageLayout = ImageLayout.Center;
        _backgroundImageCenterButton.FlatStyle = FlatStyle.Flat;
        _backgroundImageCenterButton.ForeColor = Color.White;
        _backgroundImageCenterButton.Location = new Point(381, 40);
        _backgroundImageCenterButton.Name = "_backgroundImageCenterButton";
        _backgroundImageCenterButton.Size = new Size(120, 60);
        _backgroundImageCenterButton.TabIndex = 7;
        _backgroundImageCenterButton.Text = "Center";
        // 
        // ButtonVisualStylesView
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_rootTableLayoutPanel);
        Name = "ButtonVisualStylesView";
        Size = new Size(922, 610);
        _rootTableLayoutPanel.ResumeLayout(false);
        _rootTableLayoutPanel.PerformLayout();
        _flatStyleGroupBox.ResumeLayout(false);
        _flatStyleGroupBox.PerformLayout();
        _flatStyleTableLayoutPanel.ResumeLayout(false);
        _flatStyleTableLayoutPanel.PerformLayout();
        _visualStylesGroupBox.ResumeLayout(false);
        _visualStylesGroupBox.PerformLayout();
        _visualStylesTableLayoutPanel.ResumeLayout(false);
        _visualStylesTableLayoutPanel.PerformLayout();
        _enabledStateGroupBox.ResumeLayout(false);
        _enabledStateGroupBox.PerformLayout();
        _enabledStateTableLayoutPanel.ResumeLayout(false);
        _enabledStateTableLayoutPanel.PerformLayout();
        _commandGroupBox.ResumeLayout(false);
        _commandGroupBox.PerformLayout();
        _commandTableLayoutPanel.ResumeLayout(false);
        _commandTableLayoutPanel.PerformLayout();
        _backgroundImageGroupBox.ResumeLayout(false);
        _backgroundImageGroupBox.PerformLayout();
        _backgroundImageTableLayoutPanel.ResumeLayout(false);
        _backgroundImageTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _rootTableLayoutPanel;
    private GroupBox _flatStyleGroupBox;
    private TableLayoutPanel _flatStyleTableLayoutPanel;
    private CheckBox _flatStyleStandardCheckBox;
    private Button _flatStyleStandardButton;
    private CheckBox _flatStylePopupCheckBox;
    private Button _flatStylePopupButton;
    private CheckBox _flatStyleFlatCheckBox;
    private Button _flatStyleFlatButton;
    private CheckBox _flatStyleSystemCheckBox;
    private Button _flatStyleSystemButton;
    private GroupBox _visualStylesGroupBox;
    private TableLayoutPanel _visualStylesTableLayoutPanel;
    private CheckBox _visualStylesClassicCheckBox;
    private Button _visualStylesClassicButton;
    private CheckBox _visualStylesNet11CheckBox;
    private Button _visualStylesNet11Button;
    private CheckBox _visualStylesLatestCheckBox;
    private Button _visualStylesLatestButton;
    private GroupBox _enabledStateGroupBox;
    private TableLayoutPanel _enabledStateTableLayoutPanel;
    private CheckBox _enabledButtonCheckBox;
    private Button _enabledButton;
    private CheckBox _disabledButtonCheckBox;
    private Button _disabledButton;
    private GroupBox _commandGroupBox;
    private TableLayoutPanel _commandTableLayoutPanel;
    private CheckBox _commandAlphaCheckBox;
    private Button _commandAlphaButton;
    private CheckBox _commandBetaCheckBox;
    private Button _commandBetaButton;
    private CheckBox _commandToggleEnabledCheckBox;
    private Button _commandToggleEnabledButton;
    private Label _commandResultLabel;
    private GroupBox _backgroundImageGroupBox;
    private TableLayoutPanel _backgroundImageTableLayoutPanel;
    private CheckBox _backgroundImageTileCheckBox;
    private Button _backgroundImageTileButton;
    private CheckBox _backgroundImageStretchCheckBox;
    private Button _backgroundImageStretchButton;
    private CheckBox _backgroundImageZoomCheckBox;
    private Button _backgroundImageZoomButton;
    private CheckBox _backgroundImageCenterCheckBox;
    private Button _backgroundImageCenterButton;
}
