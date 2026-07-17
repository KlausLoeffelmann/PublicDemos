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
        _introLabel = new Label();
        _statusLabel = new Label();
        _matrixScrollPanel = new Panel();
        _animationTableLayoutPanel = new TableLayoutPanel();
        _animationTimer = new System.Windows.Forms.Timer(components);
        _rootLayoutPanel.SuspendLayout();
        _matrixScrollPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _rootLayoutPanel
        // 
        _rootLayoutPanel.ColumnCount = 1;
        _rootLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayoutPanel.Controls.Add(_introLabel, 0, 0);
        _rootLayoutPanel.Controls.Add(_statusLabel, 0, 1);
        _rootLayoutPanel.Controls.Add(_matrixScrollPanel, 0, 2);
        _rootLayoutPanel.Dock = DockStyle.Fill;
        _rootLayoutPanel.Location = new Point(0, 0);
        _rootLayoutPanel.Name = "_rootLayoutPanel";
        _rootLayoutPanel.Padding = new Padding(8);
        _rootLayoutPanel.RowCount = 3;
        _rootLayoutPanel.RowStyles.Add(new RowStyle());
        _rootLayoutPanel.RowStyles.Add(new RowStyle());
        _rootLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayoutPanel.Size = new Size(1200, 850);
        _rootLayoutPanel.TabIndex = 0;
        // 
        // _introLabel
        // 
        _introLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _introLabel.AutoSize = true;
        _introLabel.Location = new Point(11, 11);
        _introLabel.Margin = new Padding(3, 3, 3, 8);
        _introLabel.Name = "_introLabel";
        _introLabel.Size = new Size(1178, 40);
        _introLabel.TabIndex = 0;
        _introLabel.Text = "A deterministic 12 x 13 matrix drives hover, pressed, and checked transitions on the same timer tick. The animation runs only while this view is visible.";
        // 
        // _statusLabel
        // 
        _statusLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _statusLabel.AutoSize = true;
        _statusLabel.Location = new Point(11, 59);
        _statusLabel.Margin = new Padding(3, 0, 3, 8);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new Size(1178, 20);
        _statusLabel.TabIndex = 1;
        // 
        // _matrixScrollPanel
        // 
        _matrixScrollPanel.AutoScroll = true;
        _matrixScrollPanel.Controls.Add(_animationTableLayoutPanel);
        _matrixScrollPanel.Dock = DockStyle.Fill;
        _matrixScrollPanel.Location = new Point(11, 90);
        _matrixScrollPanel.Name = "_matrixScrollPanel";
        _matrixScrollPanel.Size = new Size(1178, 749);
        _matrixScrollPanel.TabIndex = 2;
        // 
        // _animationTableLayoutPanel
        // 
        _animationTableLayoutPanel.AutoSize = true;
        _animationTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _animationTableLayoutPanel.ColumnCount = 12;
        _animationTableLayoutPanel.Location = new Point(0, 0);
        _animationTableLayoutPanel.Name = "_animationTableLayoutPanel";
        _animationTableLayoutPanel.RowCount = 13;
        _animationTableLayoutPanel.Size = new Size(1392, 806);
        _animationTableLayoutPanel.TabIndex = 0;
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
        _matrixScrollPanel.ResumeLayout(false);
        _matrixScrollPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _rootLayoutPanel;
    private Label _introLabel;
    private Label _statusLabel;
    private Panel _matrixScrollPanel;
    private TableLayoutPanel _animationTableLayoutPanel;
    private System.Windows.Forms.Timer _animationTimer;
}
