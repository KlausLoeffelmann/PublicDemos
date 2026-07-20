// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using VisualStylesModeDemo.Controls;

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
        components = new System.ComponentModel.Container();
        Components.LayoutTemplateItem layoutTemplateItem1 = new Components.LayoutTemplateItem();
        _rootTableLayoutPanel = new TableLayoutPanel();
        _flatStyleGroupBox = new GroupBoxEx();
        _flatStyleTableLayoutPanel = new TableLayoutPanel();
        _flatStyleStandardLabel = new Label();
        _flatStyleStandardButton = new Button();
        _flatStylePopupLabel = new Label();
        _flatStylePopupButton = new Button();
        _flatStyleFlatLabel = new Label();
        _flatStyleFlatButton = new Button();
        _flatStyleSystemLabel = new Label();
        _flatStyleSystemButton = new Button();
        _visualStylesGroupBox = new GroupBoxEx();
        _visualStylesTableLayoutPanel = new TableLayoutPanel();
        _visualStylesClassicLabel = new Label();
        _visualStylesClassicButton = new Button();
        _visualStylesNet11Label = new Label();
        _visualStylesNet11Button = new Button();
        _visualStylesLatestLabel = new Label();
        _visualStylesLatestButton = new Button();
        _enabledStateGroupBox = new GroupBoxEx();
        _enabledStateTableLayoutPanel = new TableLayoutPanel();
        _enabledButtonLabel = new Label();
        _enabledButton = new Button();
        _disabledButtonLabel = new Label();
        _disabledButton = new Button();
        _commandGroupBox = new GroupBoxEx();
        _commandTableLayoutPanel = new TableLayoutPanel();
        _commandAlphaLabel = new Label();
        _commandAlphaButton = new Button();
        _commandBetaLabel = new Label();
        _commandBetaButton = new Button();
        _commandToggleEnabledLabel = new Label();
        _commandToggleEnabledButton = new Button();
        _commandResultLabel = new Label();
        _backgroundImageGroupBox = new GroupBoxEx();
        _backgroundImageTableLayoutPanel = new TableLayoutPanel();
        _backgroundImageTileLabel = new Label();
        _backgroundImageStretchLabel = new Label();
        _backgroundImageZoomLabel = new Label();
        _backgroundImageCenterLabel = new Label();
        _backgroundImageTileButton = new Button();
        _backgroundImageStretchButton = new Button();
        _backgroundImageZoomButton = new Button();
        _backgroundImageCenterButton = new Button();
        layoutTemplateProvider1 = new VisualStylesModeDemo.Components.LayoutTemplateProvider(components);
        _rootTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_flatStyleGroupBox).BeginInit();
        _flatStyleGroupBox.SuspendLayout();
        _flatStyleTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_visualStylesGroupBox).BeginInit();
        _visualStylesGroupBox.SuspendLayout();
        _visualStylesTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_enabledStateGroupBox).BeginInit();
        _enabledStateGroupBox.SuspendLayout();
        _enabledStateTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_commandGroupBox).BeginInit();
        _commandGroupBox.SuspendLayout();
        _commandTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_backgroundImageGroupBox).BeginInit();
        _backgroundImageGroupBox.SuspendLayout();
        _backgroundImageTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _rootTableLayoutPanel
        // 
        _rootTableLayoutPanel.AutoSize = true;
        _rootTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootTableLayoutPanel.ColumnCount = 2;
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _rootTableLayoutPanel.Controls.Add(_flatStyleGroupBox, 0, 0);
        _rootTableLayoutPanel.Controls.Add(_visualStylesGroupBox, 1, 0);
        _rootTableLayoutPanel.Controls.Add(_enabledStateGroupBox, 0, 1);
        _rootTableLayoutPanel.Controls.Add(_commandGroupBox, 1, 1);
        _rootTableLayoutPanel.Controls.Add(_backgroundImageGroupBox, 0, 2);
        _rootTableLayoutPanel.Dock = DockStyle.Fill;
        _rootTableLayoutPanel.Location = new Point(0, 0);
        _rootTableLayoutPanel.Name = "_rootTableLayoutPanel";
        _rootTableLayoutPanel.RowCount = 3;
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootTableLayoutPanel.Size = new Size(1622, 966);
        _rootTableLayoutPanel.TabIndex = 0;
        // 
        // _flatStyleGroupBox
        // 
        _flatStyleGroupBox.AutoSize = true;
        _flatStyleGroupBox.CaptionFontTemplate = new FontTemplate(1F, FontStyle.Bold, FontStyle.Regular);
        _flatStyleGroupBox.Controls.Add(_flatStyleTableLayoutPanel);
        _flatStyleGroupBox.Dock = DockStyle.Fill;
        _flatStyleGroupBox.FlatStyle = FlatStyle.Popup;
        _flatStyleGroupBox.Location = new Point(3, 3);
        _flatStyleGroupBox.Name = "_flatStyleGroupBox";
        _flatStyleGroupBox.Size = new Size(805, 345);
        _flatStyleGroupBox.TabIndex = 0;
        _flatStyleGroupBox.TabStop = false;
        _flatStyleGroupBox.Text = "FlatStyle variations";
        _flatStyleGroupBox.VisualStylesMode = VisualStylesMode.Net11;
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
        _flatStyleTableLayoutPanel.Location = new Point(8, 48);
        _flatStyleTableLayoutPanel.Name = "_flatStyleTableLayoutPanel";
        _flatStyleTableLayoutPanel.RowCount = 4;
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.RowStyles.Add(new RowStyle());
        _flatStyleTableLayoutPanel.Size = new Size(789, 289);
        _flatStyleTableLayoutPanel.TabIndex = 0;
        // 
        // _flatStyleStandardLabel
        // 
        _flatStyleStandardLabel.Anchor = AnchorStyles.Left;
        _flatStyleStandardLabel.AutoSize = true;
        _flatStyleStandardLabel.Location = new Point(3, 29);
        _flatStyleStandardLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStyleStandardLabel.Name = "_flatStyleStandardLabel";
        _flatStyleStandardLabel.Size = new Size(83, 25);
        _flatStyleStandardLabel.TabIndex = 0;
        _flatStyleStandardLabel.Text = "Standard";
        // 
        // _flatStyleStandardButton
        // 
        _flatStyleStandardButton.Anchor = AnchorStyles.Left;
        _flatStyleStandardButton.AutoSize = true;
        _flatStyleStandardButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleStandardButton.DialogResult = DialogResult.OK;
        _flatStyleStandardButton.Location = new Point(99, 10);
        _flatStyleStandardButton.Margin = new Padding(10);
        _flatStyleStandardButton.Name = "_flatStyleStandardButton";
        _flatStyleStandardButton.Padding = new Padding(5);
        _flatStyleStandardButton.Size = new Size(119, 61);
        _flatStyleStandardButton.TabIndex = 1;
        _flatStyleStandardButton.Text = "Standard";
        // 
        // _flatStylePopupLabel
        // 
        _flatStylePopupLabel.Anchor = AnchorStyles.Left;
        _flatStylePopupLabel.AutoSize = true;
        _flatStylePopupLabel.Location = new Point(3, 114);
        _flatStylePopupLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStylePopupLabel.Name = "_flatStylePopupLabel";
        _flatStylePopupLabel.Size = new Size(64, 25);
        _flatStylePopupLabel.TabIndex = 2;
        _flatStylePopupLabel.Text = "Popup";
        // 
        // _flatStylePopupButton
        // 
        _flatStylePopupButton.Anchor = AnchorStyles.Left;
        _flatStylePopupButton.AutoSize = true;
        _flatStylePopupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStylePopupButton.FlatStyle = FlatStyle.Popup;
        _flatStylePopupButton.Location = new Point(99, 91);
        _flatStylePopupButton.Margin = new Padding(10);
        _flatStylePopupButton.Name = "_flatStylePopupButton";
        _flatStylePopupButton.Padding = new Padding(10);
        _flatStylePopupButton.Size = new Size(166, 69);
        _flatStylePopupButton.TabIndex = 3;
        _flatStylePopupButton.Text = "Popup-Button";
        _flatStylePopupButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStyleFlatLabel
        // 
        _flatStyleFlatLabel.Anchor = AnchorStyles.Left;
        _flatStyleFlatLabel.AutoSize = true;
        _flatStyleFlatLabel.Location = new Point(3, 194);
        _flatStyleFlatLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStyleFlatLabel.Name = "_flatStyleFlatLabel";
        _flatStyleFlatLabel.Size = new Size(40, 25);
        _flatStyleFlatLabel.TabIndex = 4;
        _flatStyleFlatLabel.Text = "Flat";
        // 
        // _flatStyleFlatButton
        // 
        _flatStyleFlatButton.Anchor = AnchorStyles.Left;
        _flatStyleFlatButton.AutoSize = true;
        _flatStyleFlatButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleFlatButton.FlatStyle = FlatStyle.Flat;
        _flatStyleFlatButton.Location = new Point(99, 180);
        _flatStyleFlatButton.Margin = new Padding(10);
        _flatStyleFlatButton.Name = "_flatStyleFlatButton";
        _flatStyleFlatButton.Padding = new Padding(5);
        _flatStyleFlatButton.Size = new Size(66, 51);
        _flatStyleFlatButton.TabIndex = 5;
        _flatStyleFlatButton.Text = "Flat";
        _flatStyleFlatButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _flatStyleSystemLabel
        // 
        _flatStyleSystemLabel.Anchor = AnchorStyles.Left;
        _flatStyleSystemLabel.AutoSize = true;
        _flatStyleSystemLabel.Location = new Point(3, 262);
        _flatStyleSystemLabel.Margin = new Padding(3, 6, 3, 3);
        _flatStyleSystemLabel.Name = "_flatStyleSystemLabel";
        _flatStyleSystemLabel.Size = new Size(69, 25);
        _flatStyleSystemLabel.TabIndex = 6;
        _flatStyleSystemLabel.Text = "System";
        // 
        // _flatStyleSystemButton
        // 
        _flatStyleSystemButton.Anchor = AnchorStyles.Left;
        _flatStyleSystemButton.AutoSize = true;
        _flatStyleSystemButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _flatStyleSystemButton.FlatStyle = FlatStyle.System;
        _flatStyleSystemButton.Location = new Point(99, 251);
        _flatStyleSystemButton.Margin = new Padding(10);
        _flatStyleSystemButton.Name = "_flatStyleSystemButton";
        _flatStyleSystemButton.Padding = new Padding(5);
        _flatStyleSystemButton.Size = new Size(93, 44);
        _flatStyleSystemButton.TabIndex = 7;
        _flatStyleSystemButton.Text = "System";
        _flatStyleSystemButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _visualStylesGroupBox
        // 
        _visualStylesGroupBox.AutoSize = true;
        _visualStylesGroupBox.CaptionFontTemplate = new FontTemplate(1F, FontStyle.Bold, FontStyle.Regular);
        _visualStylesGroupBox.Controls.Add(_visualStylesTableLayoutPanel);
        _visualStylesGroupBox.Dock = DockStyle.Fill;
        _visualStylesGroupBox.Location = new Point(814, 3);
        _visualStylesGroupBox.Name = "_visualStylesGroupBox";
        _visualStylesGroupBox.Size = new Size(805, 345);
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
        _visualStylesTableLayoutPanel.Location = new Point(8, 32);
        _visualStylesTableLayoutPanel.Name = "_visualStylesTableLayoutPanel";
        _visualStylesTableLayoutPanel.RowCount = 3;
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.RowStyles.Add(new RowStyle());
        _visualStylesTableLayoutPanel.Size = new Size(789, 305);
        _visualStylesTableLayoutPanel.TabIndex = 0;
        // 
        // _visualStylesClassicLabel
        // 
        _visualStylesClassicLabel.Anchor = AnchorStyles.Left;
        _visualStylesClassicLabel.AutoSize = true;
        _visualStylesClassicLabel.Location = new Point(3, 24);
        _visualStylesClassicLabel.Margin = new Padding(3, 6, 3, 3);
        _visualStylesClassicLabel.Name = "_visualStylesClassicLabel";
        _visualStylesClassicLabel.Size = new Size(64, 25);
        _visualStylesClassicLabel.TabIndex = 0;
        _visualStylesClassicLabel.Text = "Classic";
        // 
        // _visualStylesClassicButton
        // 
        _visualStylesClassicButton.Anchor = AnchorStyles.Left;
        _visualStylesClassicButton.AutoSize = true;
        _visualStylesClassicButton.Location = new Point(80, 10);
        _visualStylesClassicButton.Margin = new Padding(10);
        _visualStylesClassicButton.Name = "_visualStylesClassicButton";
        _visualStylesClassicButton.Padding = new Padding(5);
        _visualStylesClassicButton.Size = new Size(297, 50);
        _visualStylesClassicButton.TabIndex = 1;
        _visualStylesClassicButton.Text = "VisualStylesMode.Classic";
        _visualStylesClassicButton.VisualStylesMode = VisualStylesMode.Classic;
        // 
        // _visualStylesNet11Label
        // 
        _visualStylesNet11Label.Anchor = AnchorStyles.Left;
        _visualStylesNet11Label.AutoSize = true;
        _visualStylesNet11Label.Location = new Point(3, 101);
        _visualStylesNet11Label.Margin = new Padding(3, 6, 3, 3);
        _visualStylesNet11Label.Name = "_visualStylesNet11Label";
        _visualStylesNet11Label.Size = new Size(60, 25);
        _visualStylesNet11Label.TabIndex = 2;
        _visualStylesNet11Label.Text = "Net11";
        // 
        // _visualStylesNet11Button
        // 
        _visualStylesNet11Button.Anchor = AnchorStyles.Left;
        _visualStylesNet11Button.AutoSize = true;
        _visualStylesNet11Button.Location = new Point(80, 80);
        _visualStylesNet11Button.Margin = new Padding(10);
        _visualStylesNet11Button.Name = "_visualStylesNet11Button";
        _visualStylesNet11Button.Padding = new Padding(5);
        _visualStylesNet11Button.Size = new Size(309, 65);
        _visualStylesNet11Button.TabIndex = 3;
        _visualStylesNet11Button.Text = "VisualStylesMode.Net11";
        _visualStylesNet11Button.VisualStylesMode = VisualStylesMode.Net11;
        // 
        // _visualStylesLatestLabel
        // 
        _visualStylesLatestLabel.Anchor = AnchorStyles.Left;
        _visualStylesLatestLabel.AutoSize = true;
        _visualStylesLatestLabel.Location = new Point(3, 219);
        _visualStylesLatestLabel.Margin = new Padding(3, 6, 3, 3);
        _visualStylesLatestLabel.Name = "_visualStylesLatestLabel";
        _visualStylesLatestLabel.Size = new Size(58, 25);
        _visualStylesLatestLabel.TabIndex = 4;
        _visualStylesLatestLabel.Text = "Latest";
        // 
        // _visualStylesLatestButton
        // 
        _visualStylesLatestButton.Anchor = AnchorStyles.Left;
        _visualStylesLatestButton.AutoSize = true;
        _visualStylesLatestButton.Location = new Point(80, 197);
        _visualStylesLatestButton.Margin = new Padding(10);
        _visualStylesLatestButton.Name = "_visualStylesLatestButton";
        _visualStylesLatestButton.Padding = new Padding(5);
        _visualStylesLatestButton.Size = new Size(308, 65);
        _visualStylesLatestButton.TabIndex = 5;
        _visualStylesLatestButton.Text = "VisualStylesMode.Latest";
        _visualStylesLatestButton.VisualStylesMode = VisualStylesMode.Latest;
        // 
        // _enabledStateGroupBox
        // 
        _enabledStateGroupBox.AutoSize = true;
        _enabledStateGroupBox.CaptionFontTemplate = new FontTemplate(1F, FontStyle.Bold, FontStyle.Regular);
        _enabledStateGroupBox.Controls.Add(_enabledStateTableLayoutPanel);
        _enabledStateGroupBox.Dock = DockStyle.Fill;
        _enabledStateGroupBox.Location = new Point(3, 354);
        _enabledStateGroupBox.Name = "_enabledStateGroupBox";
        _enabledStateGroupBox.Size = new Size(805, 284);
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
        _enabledStateTableLayoutPanel.Location = new Point(8, 32);
        _enabledStateTableLayoutPanel.Name = "_enabledStateTableLayoutPanel";
        _enabledStateTableLayoutPanel.RowCount = 2;
        _enabledStateTableLayoutPanel.RowStyles.Add(new RowStyle());
        _enabledStateTableLayoutPanel.RowStyles.Add(new RowStyle());
        _enabledStateTableLayoutPanel.Size = new Size(789, 244);
        _enabledStateTableLayoutPanel.TabIndex = 0;
        // 
        // _enabledButtonLabel
        // 
        _enabledButtonLabel.Anchor = AnchorStyles.Left;
        _enabledButtonLabel.AutoSize = true;
        _enabledButtonLabel.Location = new Point(3, 41);
        _enabledButtonLabel.Margin = new Padding(3, 6, 3, 3);
        _enabledButtonLabel.Name = "_enabledButtonLabel";
        _enabledButtonLabel.Size = new Size(75, 25);
        _enabledButtonLabel.TabIndex = 0;
        _enabledButtonLabel.Text = "Enabled";
        // 
        // _enabledButton
        // 
        _enabledButton.Anchor = AnchorStyles.Left;
        _enabledButton.Font = new Font("Segoe UI Semibold", 13F);
        layoutTemplateProvider1.SetLayoutTemplate(_enabledButton, "Headertext");
        _enabledButton.Location = new Point(97, 10);
        _enabledButton.Margin = new Padding(10);
        _enabledButton.Name = "_enabledButton";
        _enabledButton.Size = new Size(140, 85);
        _enabledButton.TabIndex = 1;
        _enabledButton.Text = "Enabled button";
        // 
        // _disabledButtonLabel
        // 
        _disabledButtonLabel.Anchor = AnchorStyles.Left;
        _disabledButtonLabel.AutoSize = true;
        _disabledButtonLabel.Location = new Point(3, 163);
        _disabledButtonLabel.Margin = new Padding(3, 6, 3, 3);
        _disabledButtonLabel.Name = "_disabledButtonLabel";
        _disabledButtonLabel.Size = new Size(81, 25);
        _disabledButtonLabel.TabIndex = 2;
        _disabledButtonLabel.Text = "Disabled";
        // 
        // _disabledButton
        // 
        _disabledButton.Anchor = AnchorStyles.Left;
        _disabledButton.Enabled = false;
        _disabledButton.Font = new Font("Segoe UI Semibold", 13F);
        layoutTemplateProvider1.SetLayoutTemplate(_disabledButton, "Headertext");
        _disabledButton.Location = new Point(97, 124);
        _disabledButton.Margin = new Padding(10);
        _disabledButton.Name = "_disabledButton";
        _disabledButton.Size = new Size(140, 101);
        _disabledButton.TabIndex = 3;
        _disabledButton.Text = "Disabled button";
        // 
        // _commandGroupBox
        // 
        _commandGroupBox.AutoSize = true;
        _commandGroupBox.CaptionFontTemplate = new FontTemplate(1F, FontStyle.Bold, FontStyle.Regular);
        _commandGroupBox.Controls.Add(_commandTableLayoutPanel);
        _commandGroupBox.Dock = DockStyle.Fill;
        _commandGroupBox.Location = new Point(814, 354);
        _commandGroupBox.Name = "_commandGroupBox";
        _commandGroupBox.Size = new Size(805, 284);
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
        _commandTableLayoutPanel.Location = new Point(8, 32);
        _commandTableLayoutPanel.Name = "_commandTableLayoutPanel";
        _commandTableLayoutPanel.RowCount = 4;
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.RowStyles.Add(new RowStyle());
        _commandTableLayoutPanel.Size = new Size(789, 244);
        _commandTableLayoutPanel.TabIndex = 0;
        // 
        // _commandAlphaLabel
        // 
        _commandAlphaLabel.Anchor = AnchorStyles.Left;
        _commandAlphaLabel.AutoSize = true;
        _commandAlphaLabel.Location = new Point(3, 24);
        _commandAlphaLabel.Margin = new Padding(3, 6, 3, 3);
        _commandAlphaLabel.Name = "_commandAlphaLabel";
        _commandAlphaLabel.Size = new Size(58, 25);
        _commandAlphaLabel.TabIndex = 0;
        _commandAlphaLabel.Text = "Alpha";
        // 
        // _commandAlphaButton
        // 
        _commandAlphaButton.Anchor = AnchorStyles.Left;
        _commandAlphaButton.Location = new Point(135, 10);
        _commandAlphaButton.Margin = new Padding(10);
        _commandAlphaButton.Name = "_commandAlphaButton";
        _commandAlphaButton.Padding = new Padding(5);
        _commandAlphaButton.Size = new Size(249, 50);
        _commandAlphaButton.TabIndex = 1;
        _commandAlphaButton.Text = "Run Command (Alpha)";
        // 
        // _commandBetaLabel
        // 
        _commandBetaLabel.Anchor = AnchorStyles.Left;
        _commandBetaLabel.AutoSize = true;
        _commandBetaLabel.Location = new Point(3, 94);
        _commandBetaLabel.Margin = new Padding(3, 6, 3, 3);
        _commandBetaLabel.Name = "_commandBetaLabel";
        _commandBetaLabel.Size = new Size(46, 25);
        _commandBetaLabel.TabIndex = 2;
        _commandBetaLabel.Text = "Beta";
        // 
        // _commandBetaButton
        // 
        _commandBetaButton.Anchor = AnchorStyles.Left;
        _commandBetaButton.Location = new Point(135, 80);
        _commandBetaButton.Margin = new Padding(10);
        _commandBetaButton.Name = "_commandBetaButton";
        _commandBetaButton.Padding = new Padding(5);
        _commandBetaButton.Size = new Size(249, 50);
        _commandBetaButton.TabIndex = 3;
        _commandBetaButton.Text = "Run Command (Beta)";
        // 
        // _commandToggleEnabledLabel
        // 
        _commandToggleEnabledLabel.Anchor = AnchorStyles.Left;
        _commandToggleEnabledLabel.AutoSize = true;
        _commandToggleEnabledLabel.Location = new Point(3, 164);
        _commandToggleEnabledLabel.Margin = new Padding(3, 6, 3, 3);
        _commandToggleEnabledLabel.Name = "_commandToggleEnabledLabel";
        _commandToggleEnabledLabel.Size = new Size(119, 25);
        _commandToggleEnabledLabel.TabIndex = 4;
        _commandToggleEnabledLabel.Text = "Toggle switch";
        // 
        // _commandToggleEnabledButton
        // 
        _commandToggleEnabledButton.Anchor = AnchorStyles.Left;
        _commandToggleEnabledButton.Location = new Point(135, 150);
        _commandToggleEnabledButton.Margin = new Padding(10);
        _commandToggleEnabledButton.Name = "_commandToggleEnabledButton";
        _commandToggleEnabledButton.Padding = new Padding(5);
        _commandToggleEnabledButton.Size = new Size(249, 50);
        _commandToggleEnabledButton.TabIndex = 5;
        _commandToggleEnabledButton.Text = "Toggle CanExecute";
        // 
        // _commandResultLabel
        // 
        _commandResultLabel.Anchor = AnchorStyles.Left;
        _commandResultLabel.AutoSize = true;
        _commandTableLayoutPanel.SetColumnSpan(_commandResultLabel, 2);
        _commandResultLabel.Location = new Point(3, 216);
        _commandResultLabel.Margin = new Padding(3, 6, 3, 3);
        _commandResultLabel.Name = "_commandResultLabel";
        _commandResultLabel.Size = new Size(236, 25);
        _commandResultLabel.TabIndex = 6;
        _commandResultLabel.Text = "Last command result: (none)";
        // 
        // _backgroundImageGroupBox
        // 
        _backgroundImageGroupBox.CaptionFontTemplate = new FontTemplate(1F, FontStyle.Bold, FontStyle.Regular);
        _rootTableLayoutPanel.SetColumnSpan(_backgroundImageGroupBox, 2);
        _backgroundImageGroupBox.Controls.Add(_backgroundImageTableLayoutPanel);
        _backgroundImageGroupBox.Dock = DockStyle.Fill;
        _backgroundImageGroupBox.Location = new Point(3, 644);
        _backgroundImageGroupBox.Name = "_backgroundImageGroupBox";
        _backgroundImageGroupBox.Size = new Size(1616, 319);
        _backgroundImageGroupBox.TabIndex = 4;
        _backgroundImageGroupBox.TabStop = false;
        _backgroundImageGroupBox.Text = "BackgroundImage / BackgroundImageLayout";
        // 
        // _backgroundImageTableLayoutPanel
        // 
        _backgroundImageTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _backgroundImageTableLayoutPanel.ColumnCount = 4;
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _backgroundImageTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageTileLabel, 0, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageStretchLabel, 1, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageZoomLabel, 2, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageCenterLabel, 3, 0);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageTileButton, 0, 1);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageStretchButton, 1, 1);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageZoomButton, 2, 1);
        _backgroundImageTableLayoutPanel.Controls.Add(_backgroundImageCenterButton, 3, 1);
        _backgroundImageTableLayoutPanel.Dock = DockStyle.Fill;
        _backgroundImageTableLayoutPanel.Location = new Point(8, 32);
        _backgroundImageTableLayoutPanel.Name = "_backgroundImageTableLayoutPanel";
        _backgroundImageTableLayoutPanel.RowCount = 2;
        _backgroundImageTableLayoutPanel.RowStyles.Add(new RowStyle());
        _backgroundImageTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _backgroundImageTableLayoutPanel.Size = new Size(1600, 279);
        _backgroundImageTableLayoutPanel.TabIndex = 0;
        // 
        // _backgroundImageTileLabel
        // 
        _backgroundImageTileLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageTileLabel.AutoSize = true;
        _backgroundImageTileLabel.Font = new Font("Segoe UI Semibold", 9.857143F, FontStyle.Bold);
        _backgroundImageTileLabel.Location = new Point(3, 3);
        _backgroundImageTileLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageTileLabel.Name = "_backgroundImageTileLabel";
        _backgroundImageTileLabel.Size = new Size(394, 28);
        _backgroundImageTileLabel.TabIndex = 0;
        _backgroundImageTileLabel.Text = "Tile";
        _backgroundImageTileLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _backgroundImageStretchLabel
        // 
        _backgroundImageStretchLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageStretchLabel.AutoSize = true;
        _backgroundImageStretchLabel.Font = new Font("Segoe UI Semibold", 9.857143F, FontStyle.Bold);
        _backgroundImageStretchLabel.Location = new Point(403, 3);
        _backgroundImageStretchLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageStretchLabel.Name = "_backgroundImageStretchLabel";
        _backgroundImageStretchLabel.Size = new Size(394, 28);
        _backgroundImageStretchLabel.TabIndex = 1;
        _backgroundImageStretchLabel.Text = "Stretch";
        _backgroundImageStretchLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _backgroundImageZoomLabel
        // 
        _backgroundImageZoomLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageZoomLabel.AutoSize = true;
        _backgroundImageZoomLabel.Font = new Font("Segoe UI Semibold", 9.857143F, FontStyle.Bold);
        _backgroundImageZoomLabel.Location = new Point(803, 3);
        _backgroundImageZoomLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageZoomLabel.Name = "_backgroundImageZoomLabel";
        _backgroundImageZoomLabel.Size = new Size(394, 28);
        _backgroundImageZoomLabel.TabIndex = 2;
        _backgroundImageZoomLabel.Text = "Zoom";
        _backgroundImageZoomLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _backgroundImageCenterLabel
        // 
        _backgroundImageCenterLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageCenterLabel.AutoSize = true;
        _backgroundImageCenterLabel.Font = new Font("Segoe UI Semibold", 9.857143F, FontStyle.Bold);
        _backgroundImageCenterLabel.Location = new Point(1203, 3);
        _backgroundImageCenterLabel.Margin = new Padding(3, 3, 3, 0);
        _backgroundImageCenterLabel.Name = "_backgroundImageCenterLabel";
        _backgroundImageCenterLabel.Size = new Size(394, 28);
        _backgroundImageCenterLabel.TabIndex = 3;
        _backgroundImageCenterLabel.Text = "Center";
        _backgroundImageCenterLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _backgroundImageTileButton
        // 
        _backgroundImageTileButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageTileButton.AutoSize = true;
        _backgroundImageTileButton.Location = new Point(14, 45);
        _backgroundImageTileButton.Margin = new Padding(14);
        _backgroundImageTileButton.Name = "_backgroundImageTileButton";
        _backgroundImageTileButton.Size = new Size(372, 220);
        _backgroundImageTileButton.TabIndex = 4;
        _backgroundImageTileButton.Text = "Tile";
        // 
        // _backgroundImageStretchButton
        // 
        _backgroundImageStretchButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageStretchButton.AutoSize = true;
        _backgroundImageStretchButton.BackgroundImageLayout = ImageLayout.Stretch;
        _backgroundImageStretchButton.Location = new Point(414, 45);
        _backgroundImageStretchButton.Margin = new Padding(14);
        _backgroundImageStretchButton.Name = "_backgroundImageStretchButton";
        _backgroundImageStretchButton.Size = new Size(372, 220);
        _backgroundImageStretchButton.TabIndex = 5;
        _backgroundImageStretchButton.Text = "Stretch";
        // 
        // _backgroundImageZoomButton
        // 
        _backgroundImageZoomButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageZoomButton.AutoSize = true;
        _backgroundImageZoomButton.BackgroundImageLayout = ImageLayout.Zoom;
        _backgroundImageZoomButton.Location = new Point(814, 45);
        _backgroundImageZoomButton.Margin = new Padding(14);
        _backgroundImageZoomButton.Name = "_backgroundImageZoomButton";
        _backgroundImageZoomButton.Size = new Size(372, 220);
        _backgroundImageZoomButton.TabIndex = 6;
        _backgroundImageZoomButton.Text = "Zoom";
        // 
        // _backgroundImageCenterButton
        // 
        _backgroundImageCenterButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _backgroundImageCenterButton.AutoSize = true;
        _backgroundImageCenterButton.BackgroundImageLayout = ImageLayout.Center;
        _backgroundImageCenterButton.Location = new Point(1214, 45);
        _backgroundImageCenterButton.Margin = new Padding(14);
        _backgroundImageCenterButton.Name = "_backgroundImageCenterButton";
        _backgroundImageCenterButton.Size = new Size(372, 220);
        _backgroundImageCenterButton.TabIndex = 7;
        _backgroundImageCenterButton.Text = "Center";
        // 
        // layoutTemplateProvider1
        // 
        layoutTemplateItem1.BackColor = Color.Empty;
        layoutTemplateItem1.FontTemplate = new FontTemplate(4F, FontStyle.Bold, FontStyle.Regular);
        layoutTemplateItem1.ForeColor = Color.Empty;
        layoutTemplateItem1.Margin = new Padding(0, 0, 0, 0);
        layoutTemplateItem1.Name = "Headertext";
        layoutTemplateItem1.Padding = new Padding(0, 0, 0, 0);
        layoutTemplateProvider1.LayoutTemplates.Add(layoutTemplateItem1);
        layoutTemplateProvider1.TemplateSourceContainer = this;
        // 
        // ButtonVisualStylesView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_rootTableLayoutPanel);
        Name = "ButtonVisualStylesView";
        Size = new Size(1622, 966);
        _rootTableLayoutPanel.ResumeLayout(false);
        _rootTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_flatStyleGroupBox).EndInit();
        _flatStyleGroupBox.ResumeLayout(false);
        _flatStyleGroupBox.PerformLayout();
        _flatStyleTableLayoutPanel.ResumeLayout(false);
        _flatStyleTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_visualStylesGroupBox).EndInit();
        _visualStylesGroupBox.ResumeLayout(false);
        _visualStylesGroupBox.PerformLayout();
        _visualStylesTableLayoutPanel.ResumeLayout(false);
        _visualStylesTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_enabledStateGroupBox).EndInit();
        _enabledStateGroupBox.ResumeLayout(false);
        _enabledStateGroupBox.PerformLayout();
        _enabledStateTableLayoutPanel.ResumeLayout(false);
        _enabledStateTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_commandGroupBox).EndInit();
        _commandGroupBox.ResumeLayout(false);
        _commandGroupBox.PerformLayout();
        _commandTableLayoutPanel.ResumeLayout(false);
        _commandTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_backgroundImageGroupBox).EndInit();
        _backgroundImageGroupBox.ResumeLayout(false);
        _backgroundImageTableLayoutPanel.ResumeLayout(false);
        _backgroundImageTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _rootTableLayoutPanel;
    private GroupBoxEx _flatStyleGroupBox;
    private TableLayoutPanel _flatStyleTableLayoutPanel;
    private Label _flatStyleStandardLabel;
    private Button _flatStyleStandardButton;
    private Label _flatStylePopupLabel;
    private Button _flatStylePopupButton;
    private Label _flatStyleFlatLabel;
    private Button _flatStyleFlatButton;
    private Label _flatStyleSystemLabel;
    private Button _flatStyleSystemButton;
    private GroupBoxEx _visualStylesGroupBox;
    private TableLayoutPanel _visualStylesTableLayoutPanel;
    private Label _visualStylesClassicLabel;
    private Button _visualStylesClassicButton;
    private Label _visualStylesNet11Label;
    private Button _visualStylesNet11Button;
    private Label _visualStylesLatestLabel;
    private Button _visualStylesLatestButton;
    private GroupBoxEx _enabledStateGroupBox;
    private TableLayoutPanel _enabledStateTableLayoutPanel;
    private Label _enabledButtonLabel;
    private Button _enabledButton;
    private Label _disabledButtonLabel;
    private Button _disabledButton;
    private GroupBoxEx _commandGroupBox;
    private TableLayoutPanel _commandTableLayoutPanel;
    private Label _commandAlphaLabel;
    private Button _commandAlphaButton;
    private Label _commandBetaLabel;
    private Button _commandBetaButton;
    private Label _commandToggleEnabledLabel;
    private Button _commandToggleEnabledButton;
    private Label _commandResultLabel;
    private GroupBoxEx _backgroundImageGroupBox;
    private TableLayoutPanel _backgroundImageTableLayoutPanel;
    private Label _backgroundImageTileLabel;
    private Label _backgroundImageStretchLabel;
    private Label _backgroundImageZoomLabel;
    private Label _backgroundImageCenterLabel;
    private Button _backgroundImageTileButton;
    private Button _backgroundImageStretchButton;
    private Button _backgroundImageZoomButton;
    private Button _backgroundImageCenterButton;
    private Components.LayoutTemplateProvider layoutTemplateProvider1;
}
