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
        _flatStyleStandardLabel = new Label();
        _flatStyleStandardButton = new Button();
        _flatStylePopupLabel = new Label();
        _flatStylePopupButton = new Button();
        _flatStyleFlatLabel = new Label();
        _flatStyleFlatButton = new Button();
        _flatStyleSystemLabel = new Label();
        _flatStyleSystemButton = new Button();
        _visualStylesGroupBox = new GroupBox();
        _visualStylesTableLayoutPanel = new TableLayoutPanel();
        _visualStylesClassicLabel = new Label();
        _visualStylesClassicButton = new Button();
        _visualStylesNet11Label = new Label();
        _visualStylesNet11Button = new Button();
        _visualStylesLatestLabel = new Label();
        _visualStylesLatestButton = new Button();
        _enabledStateGroupBox = new GroupBox();
        _enabledStateTableLayoutPanel = new TableLayoutPanel();
        _enabledButtonLabel = new Label();
        _enabledButton = new Button();
        _disabledButtonLabel = new Label();
        _disabledButton = new Button();
        _commandGroupBox = new GroupBox();
        _commandTableLayoutPanel = new TableLayoutPanel();
        _commandAlphaLabel = new Label();
        _commandAlphaButton = new Button();
        _commandBetaLabel = new Label();
        _commandBetaButton = new Button();
        _commandToggleEnabledLabel = new Label();
        _commandToggleEnabledButton = new Button();
        _commandResultLabel = new Label();
        _backgroundImageGroupBox = new GroupBox();
        _backgroundImageTableLayoutPanel = new TableLayoutPanel();
        _backgroundImageTileLabel = new Label();
        _backgroundImageStretchLabel = new Label();
        _backgroundImageZoomLabel = new Label();
        _backgroundImageCenterLabel = new Label();
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
        _rootTableLayoutPanel.Size = new Size(922, 613);
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
        _flatStyleGroupBox.Size = new Size(520, 232);
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
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleStandardLabel, 0, 0);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleStandardButton, 1, 0);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStylePopupLabel, 0, 1);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStylePopupButton, 1, 1);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleFlatLabel, 0, 2);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleFlatButton, 1, 2);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleSystemLabel, 0, 3);
        _flatStyleTableLayoutPanel.Controls.Add(_flatStyleSystemButton, 1, 3);
        _flatStyleTableLayoutPanel.Dock = DockStyle.Fill;
        _flatStyleTableLayoutPanel.Location = new Point(8, 38);
        _flatStyleTableLayoutPanel.Name = "_flatStyleTableLayoutPanel";
        _flatStyleTableLayoutPanel.RowCount = 4;
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.Size = new Size(504, 186);
        _flatStyleTableLayoutPanel.TabIndex = 0;
        // 
        // _flatStyleStandardLabel
        // 
        _flatStyleStandardLabel.Anchor = AnchorStyles.Left;
        _flatStyleStandardLabel.AutoSize = true;
        _flatStyleStandardLabel.Location = new Point(3, 7);
        _flatStyleStandardLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStyleStandardLabel.Name = "_flatStyleStandardLabel";
        _flatStyleStandardLabel.Size = new Size(133, 34);
        _flatStyleStandardLabel.TabIndex = 0;
        _flatStyleStandardLabel.Text = "Standard";
        // 
        // _flatStyleStandardButton
        // 
        _flatStyleStandardButton.Anchor = AnchorStyles.Left;
        _flatStyleStandardButton.AutoSize = true;
        _flatStyleStandardButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleStandardButton.DialogResult = DialogResult.OK;
        _flatStyleStandardButton.Location = new Point(142, 3);
        _flatStyleStandardButton.Name = "_flatStyleStandardButton";
        _flatStyleStandardButton.Size = new Size(109, 40);
        _flatStyleStandardButton.TabIndex = 1;
        _flatStyleStandardButton.Text = "Standard";
        _flatStyleStandardButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStylePopupLabel
        // 
        _flatStylePopupLabel.Anchor = AnchorStyles.Left;
        _flatStylePopupLabel.AutoSize = true;
        _flatStylePopupLabel.Location = new Point(3, 53);
        _flatStylePopupLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStylePopupLabel.Name = "_flatStylePopupLabel";
        _flatStylePopupLabel.Size = new Size(109, 34);
        _flatStylePopupLabel.TabIndex = 2;
        _flatStylePopupLabel.Text = "Popup";
        // 
        // _flatStylePopupButton
        // 
        _flatStylePopupButton.Anchor = AnchorStyles.Left;
        _flatStylePopupButton.AutoSize = true;
        _flatStylePopupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStylePopupButton.FlatStyle = FlatStyle.Popup;
        _flatStylePopupButton.Location = new Point(142, 49);
        _flatStylePopupButton.Name = "_flatStylePopupButton";
        _flatStylePopupButton.Size = new Size(85, 40);
        _flatStylePopupButton.TabIndex = 3;
        _flatStylePopupButton.Text = "Popup";
        _flatStylePopupButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStyleFlatLabel
        // 
        _flatStyleFlatLabel.Anchor = AnchorStyles.Left;
        _flatStyleFlatLabel.AutoSize = true;
        _flatStyleFlatLabel.Location = new Point(3, 98);
        _flatStyleFlatLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStyleFlatLabel.Name = "_flatStyleFlatLabel";
        _flatStyleFlatLabel.Size = new Size(57, 34);
        _flatStyleFlatLabel.TabIndex = 4;
        _flatStyleFlatLabel.Text = "Flat";
        // 
        // _flatStyleFlatButton
        // 
        _flatStyleFlatButton.Anchor = AnchorStyles.Left;
        _flatStyleFlatButton.AutoSize = true;
        _flatStyleFlatButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleFlatButton.FlatStyle = FlatStyle.Flat;
        _flatStyleFlatButton.Location = new Point(142, 96);
        _flatStyleFlatButton.Name = "_flatStyleFlatButton";
        _flatStyleFlatButton.Size = new Size(57, 40);
        _flatStyleFlatButton.TabIndex = 5;
        _flatStyleFlatButton.Text = "Flat";
        _flatStyleFlatButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStyleSystemLabel
        // 
        _flatStyleSystemLabel.Anchor = AnchorStyles.Left;
        _flatStyleSystemLabel.AutoSize = true;
        _flatStyleSystemLabel.Location = new Point(3, 148);
        _flatStyleSystemLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStyleSystemLabel.Name = "_flatStyleSystemLabel";
        _flatStyleSystemLabel.Size = new Size(108, 34);
        _flatStyleSystemLabel.TabIndex = 6;
        _flatStyleSystemLabel.Text = "System";
        // 
        // _flatStyleSystemButton
        // 
        _flatStyleSystemButton.Anchor = AnchorStyles.Left;
        _flatStyleSystemButton.AutoSize = true;
        _flatStyleSystemButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleSystemButton.FlatStyle = FlatStyle.System;
        _flatStyleSystemButton.Location = new Point(142, 144);
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
        _visualStylesGroupBox.Size = new Size(390, 232);
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
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesClassicLabel, 0, 0);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesClassicButton, 1, 0);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesNet11Label, 0, 1);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesNet11Button, 1, 1);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesLatestLabel, 0, 2);
        _visualStylesTableLayoutPanel.Controls.Add(_visualStylesLatestButton, 1, 2);
        _visualStylesTableLayoutPanel.Dock = DockStyle.Fill;
        _visualStylesTableLayoutPanel.Location = new Point(8, 38);
        _visualStylesTableLayoutPanel.Name = "_visualStylesTableLayoutPanel";
        _visualStylesTableLayoutPanel.RowCount = 3;
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.Size = new Size(374, 186);
        _visualStylesTableLayoutPanel.TabIndex = 0;
        // 
        // _visualStylesClassicLabel
        // 
        _visualStylesClassicLabel.Anchor = AnchorStyles.Left;
        _visualStylesClassicLabel.AutoSize = true;
        _visualStylesClassicLabel.Location = new Point(3, 7);
        _visualStylesClassicLabel.Margin = new Padding(3, 6, 3, 3);
        _visualStylesClassicLabel.Name = "_visualStylesClassicLabel";
        _visualStylesClassicLabel.Size = new Size(102, 34);
        _visualStylesClassicLabel.TabIndex = 0;
        _visualStylesClassicLabel.Text = "Classic";
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
        // _visualStylesNet11Label
        // 
        _visualStylesNet11Label.Anchor = AnchorStyles.Left;
        _visualStylesNet11Label.AutoSize = true;
        _visualStylesNet11Label.Location = new Point(3, 53);
        _visualStylesNet11Label.Margin = new Padding(3, 6, 3, 3);
        _visualStylesNet11Label.Name = "_visualStylesNet11Label";
        _visualStylesNet11Label.Size = new Size(98, 34);
        _visualStylesNet11Label.TabIndex = 2;
        _visualStylesNet11Label.Text = "Net11";
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
        // _visualStylesLatestLabel
        // 
        _visualStylesLatestLabel.Anchor = AnchorStyles.Left;
        _visualStylesLatestLabel.AutoSize = true;
        _visualStylesLatestLabel.Location = new Point(3, 123);
        _visualStylesLatestLabel.Margin = new Padding(3, 6, 3, 3);
        _visualStylesLatestLabel.Name = "_visualStylesLatestLabel";
        _visualStylesLatestLabel.Size = new Size(95, 34);
        _visualStylesLatestLabel.TabIndex = 4;
        _visualStylesLatestLabel.Text = "Latest";
        // 
        // _visualStylesLatestButton
        // 
        _visualStylesLatestButton.Anchor = AnchorStyles.Left;
        _visualStylesLatestButton.AutoSize = true;
        _visualStylesLatestButton.Location = new Point(111, 119);
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
        _enabledStateGroupBox.Location = new Point(3, 241);
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
        _enabledStateTableLayoutPanel.Controls.Add(_enabledButtonLabel, 0, 0);
        _enabledStateTableLayoutPanel.Controls.Add(_enabledButton, 1, 0);
        _enabledStateTableLayoutPanel.Controls.Add(_disabledButtonLabel, 0, 1);
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
        // _enabledButtonLabel
        // 
        _enabledButtonLabel.Anchor = AnchorStyles.Left;
        _enabledButtonLabel.AutoSize = true;
        _enabledButtonLabel.Location = new Point(3, 6);
        _enabledButtonLabel.Margin = new Padding(3, 6, 3, 3);
        _enabledButtonLabel.Name = "_enabledButtonLabel";
        _enabledButtonLabel.Size = new Size(116, 34);
        _enabledButtonLabel.TabIndex = 0;
        _enabledButtonLabel.Text = "Enabled";
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
        // _disabledButtonLabel
        // 
        _disabledButtonLabel.Anchor = AnchorStyles.Left;
        _disabledButtonLabel.AutoSize = true;
        _disabledButtonLabel.Location = new Point(3, 90);
        _disabledButtonLabel.Margin = new Padding(3, 6, 3, 3);
        _disabledButtonLabel.Name = "_disabledButtonLabel";
        _disabledButtonLabel.Size = new Size(122, 34);
        _disabledButtonLabel.TabIndex = 2;
        _disabledButtonLabel.Text = "Disabled";
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
        _commandGroupBox.Location = new Point(529, 241);
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
        _commandTableLayoutPanel.Controls.Add(_commandAlphaLabel, 0, 0);
        _commandTableLayoutPanel.Controls.Add(_commandAlphaButton, 1, 0);
        _commandTableLayoutPanel.Controls.Add(_commandBetaLabel, 0, 1);
        _commandTableLayoutPanel.Controls.Add(_commandBetaButton, 1, 1);
        _commandTableLayoutPanel.Controls.Add(_commandToggleEnabledLabel, 0, 2);
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
        // _commandAlphaLabel
        // 
        _commandAlphaLabel.Anchor = AnchorStyles.Left;
        _commandAlphaLabel.AutoSize = true;
        _commandAlphaLabel.Location = new Point(3, 6);
        _commandAlphaLabel.Margin = new Padding(3, 6, 3, 3);
        _commandAlphaLabel.Name = "_commandAlphaLabel";
        _commandAlphaLabel.Size = new Size(94, 34);
        _commandAlphaLabel.TabIndex = 0;
        _commandAlphaLabel.Text = "Alpha";
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
        // _commandBetaLabel
        // 
        _commandBetaLabel.Anchor = AnchorStyles.Left;
        _commandBetaLabel.AutoSize = true;
        _commandBetaLabel.Location = new Point(3, 49);
        _commandBetaLabel.Margin = new Padding(3, 6, 3, 3);
        _commandBetaLabel.Name = "_commandBetaLabel";
        _commandBetaLabel.Size = new Size(82, 34);
        _commandBetaLabel.TabIndex = 2;
        _commandBetaLabel.Text = "Beta";
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
        // _commandToggleEnabledLabel
        // 
        _commandToggleEnabledLabel.Anchor = AnchorStyles.Left;
        _commandToggleEnabledLabel.AutoSize = true;
        _commandToggleEnabledLabel.Location = new Point(3, 92);
        _commandToggleEnabledLabel.Margin = new Padding(3, 6, 3, 3);
        _commandToggleEnabledLabel.Name = "_commandToggleEnabledLabel";
        _commandToggleEnabledLabel.Size = new Size(170, 34);
        _commandToggleEnabledLabel.TabIndex = 4;
        _commandToggleEnabledLabel.Text = "Toggle switch";
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
        _backgroundImageGroupBox.Location = new Point(3, 461);
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
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageTileLabel, 0, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageStretchLabel, 1, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageZoomLabel, 2, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageCenterLabel, 3, 0);
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
        // _backgroundImageTileLabel
        // 
        _backgroundImageTileLabel.Anchor = AnchorStyles.Left;
        _backgroundImageTileLabel.AutoSize = true;
        _backgroundImageTileLabel.Location = new Point(3, 3);
        _backgroundImageTileLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageTileLabel.Name = "_backgroundImageTileLabel";
        _backgroundImageTileLabel.Size = new Size(73, 34);
        _backgroundImageTileLabel.TabIndex = 0;
        _backgroundImageTileLabel.Text = "Tile";
        // 
        // _backgroundImageStretchLabel
        // 
        _backgroundImageStretchLabel.Anchor = AnchorStyles.Left;
        _backgroundImageStretchLabel.AutoSize = true;
        _backgroundImageStretchLabel.Location = new Point(129, 3);
        _backgroundImageStretchLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageStretchLabel.Name = "_backgroundImageStretchLabel";
        _backgroundImageStretchLabel.Size = new Size(106, 34);
        _backgroundImageStretchLabel.TabIndex = 1;
        _backgroundImageStretchLabel.Text = "Stretch";
        // 
        // _backgroundImageZoomLabel
        // 
        _backgroundImageZoomLabel.Anchor = AnchorStyles.Left;
        _backgroundImageZoomLabel.AutoSize = true;
        _backgroundImageZoomLabel.Location = new Point(255, 3);
        _backgroundImageZoomLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageZoomLabel.Name = "_backgroundImageZoomLabel";
        _backgroundImageZoomLabel.Size = new Size(97, 34);
        _backgroundImageZoomLabel.TabIndex = 2;
        _backgroundImageZoomLabel.Text = "Zoom";
        // 
        // _backgroundImageCenterLabel
        // 
        _backgroundImageCenterLabel.Anchor = AnchorStyles.Left;
        _backgroundImageCenterLabel.AutoSize = true;
        _backgroundImageCenterLabel.Location = new Point(381, 3);
        _backgroundImageCenterLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageCenterLabel.Name = "_backgroundImageCenterLabel";
        _backgroundImageCenterLabel.Size = new Size(104, 34);
        _backgroundImageCenterLabel.TabIndex = 3;
        _backgroundImageCenterLabel.Text = "Center";
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
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_rootTableLayoutPanel);
        Name = "ButtonVisualStylesView";
        Size = new Size(922, 613);
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
    private Label _flatStyleStandardLabel;
    private Button _flatStyleStandardButton;
    private Label _flatStylePopupLabel;
    private Button _flatStylePopupButton;
    private Label _flatStyleFlatLabel;
    private Button _flatStyleFlatButton;
    private Label _flatStyleSystemLabel;
    private Button _flatStyleSystemButton;
    private GroupBox _visualStylesGroupBox;
    private TableLayoutPanel _visualStylesTableLayoutPanel;
    private Label _visualStylesClassicLabel;
    private Button _visualStylesClassicButton;
    private Label _visualStylesNet11Label;
    private Button _visualStylesNet11Button;
    private Label _visualStylesLatestLabel;
    private Button _visualStylesLatestButton;
    private GroupBox _enabledStateGroupBox;
    private TableLayoutPanel _enabledStateTableLayoutPanel;
    private Label _enabledButtonLabel;
    private Button _enabledButton;
    private Label _disabledButtonLabel;
    private Button _disabledButton;
    private GroupBox _commandGroupBox;
    private TableLayoutPanel _commandTableLayoutPanel;
    private Label _commandAlphaLabel;
    private Button _commandAlphaButton;
    private Label _commandBetaLabel;
    private Button _commandBetaButton;
    private Label _commandToggleEnabledLabel;
    private Button _commandToggleEnabledButton;
    private Label _commandResultLabel;
    private GroupBox _backgroundImageGroupBox;
    private TableLayoutPanel _backgroundImageTableLayoutPanel;
    private Label _backgroundImageTileLabel;
    private Label _backgroundImageStretchLabel;
    private Label _backgroundImageZoomLabel;
    private Label _backgroundImageCenterLabel;
    private Button _backgroundImageTileButton;
    private Button _backgroundImageStretchButton;
    private Button _backgroundImageZoomButton;
    private Button _backgroundImageCenterButton;
}
