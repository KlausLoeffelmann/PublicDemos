// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace VisualStylesModeDemo.Views;

partial class CheckBoxRadioButtonVisualStylesView
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
        _introLabel = new Label();
        _matrixTableLayoutPanel = new TableLayoutPanel();
        _rootTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _rootTableLayoutPanel
        // 
        _rootTableLayoutPanel.AutoSize = true;
        _rootTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootTableLayoutPanel.ColumnCount = 1;
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootTableLayoutPanel.Controls.Add(_introLabel, 0, 0);
        _rootTableLayoutPanel.Controls.Add(_matrixTableLayoutPanel, 0, 1);
        _rootTableLayoutPanel.Dock = DockStyle.Top;
        _rootTableLayoutPanel.Location = new Point(0, 0);
        _rootTableLayoutPanel.Name = "_rootTableLayoutPanel";
        _rootTableLayoutPanel.RowCount = 2;
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle());
        _rootTableLayoutPanel.Size = new Size(1000, 400);
        _rootTableLayoutPanel.TabIndex = 0;
        // 
        // _introLabel
        // 
        _introLabel.AutoSize = true;
        _introLabel.Margin = new Padding(3, 6, 3, 12);
        _introLabel.Name = "_introLabel";
        _introLabel.Size = new Size(400, 30);
        _introLabel.TabIndex = 0;
        _introLabel.Text = "Enable Edit mode, then double-click a control to select it. Hold Ctrl to add or remove controls.";
        // 
        // _matrixTableLayoutPanel
        // 
        // Seven columns (row-header + six appearance/flat-style variants) and five rows
        // (column-header + four control/VisualStylesMode combinations) are declared here; the
        // header labels and the 24 scenario controls are populated in BuildMatrix() so the
        // repetitive matrix stays in one readable, well-commented place.
        _matrixTableLayoutPanel.AutoSize = true;
        _matrixTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _matrixTableLayoutPanel.ColumnCount = 7;
        for (int column = 0; column < 7; column++)
        {
            _matrixTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        }

        _matrixTableLayoutPanel.RowCount = 5;
        for (int row = 0; row < 5; row++)
        {
            _matrixTableLayoutPanel.RowStyles.Add(new RowStyle());
        }

        _matrixTableLayoutPanel.Location = new Point(3, 51);
        _matrixTableLayoutPanel.Name = "_matrixTableLayoutPanel";
        _matrixTableLayoutPanel.Size = new Size(900, 340);
        _matrixTableLayoutPanel.TabIndex = 1;
        // 
        // CheckBoxRadioButtonVisualStylesView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_rootTableLayoutPanel);
        Name = "CheckBoxRadioButtonVisualStylesView";
        Size = new Size(1000, 400);
        _rootTableLayoutPanel.ResumeLayout(false);
        _rootTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _rootTableLayoutPanel;
    private Label _introLabel;
    private TableLayoutPanel _matrixTableLayoutPanel;
}
