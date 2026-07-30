// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

partial class ParallelAnimationView
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
        _rootLayoutPanel = new TableLayoutPanel();
        _animationToolStrip = new ToolStrip();
        _maxConcurrentCaptionLabel = new ToolStripLabel();
        _maxConcurrentTrackBar = new TrackBar();
        _maxConcurrentTrackBarHost = new ToolStripControlHost(_maxConcurrentTrackBar);
        _maxConcurrentValueLabel = new ToolStripLabel();
        _introLabel = new Label();
        _statusLabel = new Label();
        _animationTableLayoutPanel = new TableLayoutPanel();
        _animationTimer = new System.Windows.Forms.Timer(components);
        _rootLayoutPanel.SuspendLayout();
        _animationToolStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_maxConcurrentTrackBar).BeginInit();
        SuspendLayout();
        // 
        // _rootLayoutPanel
        // 
        _rootLayoutPanel.ColumnCount = 1;
        _rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayoutPanel.Controls.Add(_animationToolStrip, 0, 0);
        _rootLayoutPanel.Controls.Add(_introLabel, 0, 1);
        _rootLayoutPanel.Controls.Add(_statusLabel, 0, 2);
        _rootLayoutPanel.Controls.Add(_animationTableLayoutPanel, 0, 3);
        _rootLayoutPanel.Dock = DockStyle.Fill;
        _rootLayoutPanel.Location = new Point(0, 0);
        _rootLayoutPanel.Name = "_rootLayoutPanel";
        _rootLayoutPanel.Padding = new Padding(8);
        _rootLayoutPanel.RowCount = 4;
        _rootLayoutPanel.RowStyles.Add(new RowStyle());
        _rootLayoutPanel.RowStyles.Add(new RowStyle());
        _rootLayoutPanel.RowStyles.Add(new RowStyle());
        _rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayoutPanel.Size = new Size(1200, 850);
        _rootLayoutPanel.TabIndex = 0;
        // 
        // _animationToolStrip
        // 
        _animationToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        _animationToolStrip.Items.Add(_maxConcurrentCaptionLabel);
        _animationToolStrip.Items.Add(_maxConcurrentTrackBarHost);
        _animationToolStrip.Items.Add(_maxConcurrentValueLabel);
        _animationToolStrip.Location = new Point(11, 11);
        _animationToolStrip.Name = "_animationToolStrip";
        _animationToolStrip.Size = new Size(1178, 25);
        _animationToolStrip.TabIndex = 0;
        _animationToolStrip.Text = "Animation Controls";
        // 
        // _maxConcurrentCaptionLabel
        // 
        _maxConcurrentCaptionLabel.Name = "_maxConcurrentCaptionLabel";
        _maxConcurrentCaptionLabel.Size = new Size(146, 22);
        _maxConcurrentCaptionLabel.Text = "Max concurrent animations:";
        // 
        // _maxConcurrentTrackBar
        // 
        _maxConcurrentTrackBar.AutoSize = false;
        _maxConcurrentTrackBar.LargeChange = 10;
        _maxConcurrentTrackBar.Maximum = 156;
        _maxConcurrentTrackBar.Minimum = 1;
        _maxConcurrentTrackBar.Name = "_maxConcurrentTrackBar";
        _maxConcurrentTrackBar.Size = new Size(220, 25);
        _maxConcurrentTrackBar.SmallChange = 1;
        _maxConcurrentTrackBar.TabIndex = 0;
        _maxConcurrentTrackBar.TickFrequency = 10;
        _maxConcurrentTrackBar.TickStyle = TickStyle.BottomRight;
        _maxConcurrentTrackBar.Value = 24;
        _maxConcurrentTrackBar.ValueChanged += MaxConcurrentTrackBar_ValueChanged;
        // 
        // _maxConcurrentTrackBarHost
        // 
        _maxConcurrentTrackBarHost.Name = "_maxConcurrentTrackBarHost";
        _maxConcurrentTrackBarHost.Size = new Size(220, 25);
        // 
        // _maxConcurrentValueLabel
        // 
        _maxConcurrentValueLabel.Name = "_maxConcurrentValueLabel";
        _maxConcurrentValueLabel.Size = new Size(36, 22);
        _maxConcurrentValueLabel.Text = "24 / 156";
        // 
        // _introLabel
        // 
        _introLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _introLabel.AutoSize = true;
        _introLabel.Location = new Point(11, 44);
        _introLabel.Margin = new Padding(3, 8, 3, 8);
        _introLabel.Name = "_introLabel";
        _introLabel.Size = new Size(1178, 40);
        _introLabel.TabIndex = 1;
        _introLabel.Text = "A 12 x 13 matrix of controls; only a random, independently timed subset animates at any moment, capped by the slider above. Each cell grows to fill the available space.";
        // 
        // _statusLabel
        // 
        _statusLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _statusLabel.AutoSize = true;
        _statusLabel.Location = new Point(11, 92);
        _statusLabel.Margin = new Padding(3, 0, 3, 8);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(1178, 20);
        _statusLabel.TabIndex = 2;
        // 
        // _animationTableLayoutPanel
        // 
        _animationTableLayoutPanel.ColumnCount = 12;
        _animationTableLayoutPanel.Dock = DockStyle.Fill;
        _animationTableLayoutPanel.Location = new Point(11, 123);
        _animationTableLayoutPanel.Name = "_animationTableLayoutPanel";
        _animationTableLayoutPanel.RowCount = 13;
        _animationTableLayoutPanel.Size = new Size(1178, 716);
        _animationTableLayoutPanel.TabIndex = 3;
        // 
        // _animationTimer
        // 
        _animationTimer.Interval = 350;
        _animationTimer.Tick += AnimationTimer_Tick;
        // 
        // ParallelAnimationView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        Controls.Add(_rootLayoutPanel);
        DoubleBuffered = true;
        Name = "ParallelAnimationView";
        Size = new Size(1200, 850);
        _rootLayoutPanel.ResumeLayout(false);
        _rootLayoutPanel.PerformLayout();
        _animationToolStrip.ResumeLayout(false);
        _animationToolStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_maxConcurrentTrackBar).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _rootLayoutPanel;
    private ToolStrip _animationToolStrip;
    private ToolStripLabel _maxConcurrentCaptionLabel;
    private TrackBar _maxConcurrentTrackBar;
    private ToolStripControlHost _maxConcurrentTrackBarHost;
    private ToolStripLabel _maxConcurrentValueLabel;
    private Label _introLabel;
    private Label _statusLabel;
    private TableLayoutPanel _animationTableLayoutPanel;
    private System.Windows.Forms.Timer _animationTimer;
}
