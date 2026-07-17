// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using VisualStylesModeDemo.Controls;

namespace VisualStylesModeDemo.Views;

partial class TextBoxScenariosView
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
        _textBoxGroupBox = new GroupBoxEx();
        _textBoxTableLayoutPanel = new TableLayoutPanel();
        _textBoxDefaultLabel = new Label();
        _textBoxDefault = new TextBox();
        _textBoxFixedSingleLabel = new Label();
        _textBoxFixedSingle = new TextBox();
        _textBoxMultilineLabel = new Label();
        _textBoxMultiline = new TextBox();
        _textBoxNoBorderReadOnlyLabel = new Label();
        _textBoxNoBorderReadOnly = new TextBox();
        numericUpDown1 = new NumericUpDown();
        label1 = new Label();
        _richTextBoxGroupBox = new GroupBoxEx();
        _richTextBoxTableLayoutPanel = new TableLayoutPanel();
        _richTextBoxDefaultLabel = new Label();
        _richTextBoxDefault = new RichTextBox();
        _richTextBoxFixedSingleLabel = new Label();
        _richTextBoxFixedSingle = new RichTextBox();
        _richTextBoxNoWordWrapLabel = new Label();
        _richTextBoxNoWordWrap = new RichTextBox();
        _richTextBoxReadOnlyLabel = new Label();
        _richTextBoxReadOnly = new RichTextBox();
        _rootTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_textBoxGroupBox).BeginInit();
        _textBoxGroupBox.SuspendLayout();
        _textBoxTableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_richTextBoxGroupBox).BeginInit();
        _richTextBoxGroupBox.SuspendLayout();
        _richTextBoxTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // 
        // _rootTableLayoutPanel
        // 
        _rootTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootTableLayoutPanel.ColumnCount = 2;
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _rootTableLayoutPanel.Controls.Add(_textBoxGroupBox, 0, 0);
        _rootTableLayoutPanel.Controls.Add(_richTextBoxGroupBox, 1, 0);
        _rootTableLayoutPanel.Dock = DockStyle.Fill;
        _rootTableLayoutPanel.Location = new Point(0, 0);
        _rootTableLayoutPanel.Margin = new Padding(4);
        _rootTableLayoutPanel.Name = "_rootTableLayoutPanel";
        _rootTableLayoutPanel.Padding = new Padding(12);
        _rootTableLayoutPanel.RowCount = 1;
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootTableLayoutPanel.Size = new Size(1307, 803);
        _rootTableLayoutPanel.TabIndex = 0;
        // 
        // _textBoxGroupBox
        // 
        _textBoxGroupBox.Controls.Add(_textBoxTableLayoutPanel);
        _textBoxGroupBox.Dock = DockStyle.Fill;
        _textBoxGroupBox.Location = new Point(16, 16);
        _textBoxGroupBox.Margin = new Padding(4);
        _textBoxGroupBox.Name = "_textBoxGroupBox";
        _textBoxGroupBox.Padding = new Padding(10);
        _textBoxGroupBox.Size = new Size(633, 771);
        _textBoxGroupBox.TabIndex = 0;
        _textBoxGroupBox.TabStop = false;
        _textBoxGroupBox.Text = "TextBox scenarios (NC-paint / hover repro)";
        // 
        // _textBoxTableLayoutPanel
        // 
        _textBoxTableLayoutPanel.ColumnCount = 2;
        _textBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _textBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _textBoxTableLayoutPanel.Controls.Add(_textBoxDefaultLabel, 0, 0);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxDefault, 1, 0);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxFixedSingleLabel, 0, 1);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxFixedSingle, 1, 1);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxMultilineLabel, 0, 2);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxMultiline, 1, 2);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxNoBorderReadOnlyLabel, 0, 3);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxNoBorderReadOnly, 1, 3);
        _textBoxTableLayoutPanel.Controls.Add(numericUpDown1, 1, 4);
        _textBoxTableLayoutPanel.Controls.Add(label1, 0, 4);
        _textBoxTableLayoutPanel.Dock = DockStyle.Fill;
        _textBoxTableLayoutPanel.Location = new Point(10, 53);
        _textBoxTableLayoutPanel.Margin = new Padding(4);
        _textBoxTableLayoutPanel.Name = "_textBoxTableLayoutPanel";
        _textBoxTableLayoutPanel.RowCount = 5;
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle());
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle());
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle());
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle());
        _textBoxTableLayoutPanel.Size = new Size(613, 708);
        _textBoxTableLayoutPanel.TabIndex = 0;
        // 
        // _textBoxDefaultLabel
        // 
        _textBoxDefaultLabel.Anchor = AnchorStyles.Left;
        _textBoxDefaultLabel.AutoSize = true;
        _textBoxDefaultLabel.Location = new Point(4, 20);
        _textBoxDefaultLabel.Margin = new Padding(4, 7, 4, 4);
        _textBoxDefaultLabel.Name = "_textBoxDefaultLabel";
        _textBoxDefaultLabel.Size = new Size(173, 30);
        _textBoxDefaultLabel.TabIndex = 0;
        _textBoxDefaultLabel.Text = "Default (Fixed3D)";
        // 
        // _textBoxDefault
        // 
        _textBoxDefault.Anchor = AnchorStyles.Left;
        _textBoxDefault.Location = new Point(244, 12);
        _textBoxDefault.Margin = new Padding(12);
        _textBoxDefault.Name = "_textBoxDefault";
        _textBoxDefault.Size = new Size(239, 44);
        _textBoxDefault.TabIndex = 1;
        _textBoxDefault.Text = "Hover over me";
        // 
        // _textBoxFixedSingleLabel
        // 
        _textBoxFixedSingleLabel.Anchor = AnchorStyles.Left;
        _textBoxFixedSingleLabel.AutoSize = true;
        _textBoxFixedSingleLabel.Location = new Point(4, 91);
        _textBoxFixedSingleLabel.Margin = new Padding(4, 7, 4, 4);
        _textBoxFixedSingleLabel.Name = "_textBoxFixedSingleLabel";
        _textBoxFixedSingleLabel.Size = new Size(184, 30);
        _textBoxFixedSingleLabel.TabIndex = 2;
        _textBoxFixedSingleLabel.Text = "FixedSingle border";
        // 
        // _textBoxFixedSingle
        // 
        _textBoxFixedSingle.Anchor = AnchorStyles.Left;
        _textBoxFixedSingle.BorderStyle = BorderStyle.FixedSingle;
        _textBoxFixedSingle.Location = new Point(244, 80);
        _textBoxFixedSingle.Margin = new Padding(12);
        _textBoxFixedSingle.MinimumSize = new Size(0, 50);
        _textBoxFixedSingle.Name = "_textBoxFixedSingle";
        _textBoxFixedSingle.Padding = new Padding(0, 1, 0, 0);
        _textBoxFixedSingle.Size = new Size(239, 50);
        _textBoxFixedSingle.TabIndex = 3;
        _textBoxFixedSingle.Text = "Hover over me";
        // 
        // _textBoxMultilineLabel
        // 
        _textBoxMultilineLabel.Anchor = AnchorStyles.Left;
        _textBoxMultilineLabel.AutoSize = true;
        _textBoxMultilineLabel.Location = new Point(4, 351);
        _textBoxMultilineLabel.Margin = new Padding(4, 7, 4, 4);
        _textBoxMultilineLabel.Name = "_textBoxMultilineLabel";
        _textBoxMultilineLabel.Size = new Size(224, 30);
        _textBoxMultilineLabel.TabIndex = 4;
        _textBoxMultilineLabel.Text = "Multiline + FixedSingle";
        // 
        // _textBoxMultiline
        // 
        _textBoxMultiline.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _textBoxMultiline.BorderStyle = BorderStyle.FixedSingle;
        _textBoxMultiline.Location = new Point(244, 154);
        _textBoxMultiline.Margin = new Padding(12);
        _textBoxMultiline.Multiline = true;
        _textBoxMultiline.Name = "_textBoxMultiline";
        _textBoxMultiline.Size = new Size(357, 422);
        _textBoxMultiline.TabIndex = 5;
        _textBoxMultiline.Text = "Hover over me\r\nMultiline text";
        // 
        // _textBoxNoBorderReadOnlyLabel
        // 
        _textBoxNoBorderReadOnlyLabel.Anchor = AnchorStyles.Left;
        _textBoxNoBorderReadOnlyLabel.AutoSize = true;
        _textBoxNoBorderReadOnlyLabel.Location = new Point(4, 606);
        _textBoxNoBorderReadOnlyLabel.Margin = new Padding(4, 7, 4, 4);
        _textBoxNoBorderReadOnlyLabel.Name = "_textBoxNoBorderReadOnlyLabel";
        _textBoxNoBorderReadOnlyLabel.Size = new Size(223, 30);
        _textBoxNoBorderReadOnlyLabel.TabIndex = 6;
        _textBoxNoBorderReadOnlyLabel.Text = "No border + ReadOnly";
        // 
        // _textBoxNoBorderReadOnly
        // 
        _textBoxNoBorderReadOnly.Anchor = AnchorStyles.Left;
        _textBoxNoBorderReadOnly.BorderStyle = BorderStyle.None;
        _textBoxNoBorderReadOnly.Location = new Point(244, 600);
        _textBoxNoBorderReadOnly.Margin = new Padding(12);
        _textBoxNoBorderReadOnly.Name = "_textBoxNoBorderReadOnly";
        _textBoxNoBorderReadOnly.ReadOnly = true;
        _textBoxNoBorderReadOnly.Size = new Size(239, 40);
        _textBoxNoBorderReadOnly.TabIndex = 7;
        _textBoxNoBorderReadOnly.Text = "Hover over me";
        // 
        // numericUpDown1
        // 
        numericUpDown1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        numericUpDown1.AutoSize = true;
        numericUpDown1.Location = new Point(242, 662);
        numericUpDown1.Margin = new Padding(10);
        numericUpDown1.Maximum = new decimal(new int[] { -1530494977, 232830, 0, 0 });
        numericUpDown1.Name = "numericUpDown1";
        numericUpDown1.Size = new Size(361, 36);
        numericUpDown1.TabIndex = 8;
        // 
        // label1
        // 
        label1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        label1.AutoSize = true;
        label1.Location = new Point(4, 665);
        label1.Margin = new Padding(4, 0, 4, 0);
        label1.Name = "label1";
        label1.Size = new Size(224, 30);
        label1.TabIndex = 9;
        label1.Text = "Numeric UpDown";
        // 
        // _richTextBoxGroupBox
        // 
        _richTextBoxGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _richTextBoxGroupBox.Controls.Add(_richTextBoxTableLayoutPanel);
        _richTextBoxGroupBox.Dock = DockStyle.Fill;
        _richTextBoxGroupBox.Location = new Point(657, 16);
        _richTextBoxGroupBox.Margin = new Padding(4);
        _richTextBoxGroupBox.Name = "_richTextBoxGroupBox";
        _richTextBoxGroupBox.Padding = new Padding(10);
        _richTextBoxGroupBox.Size = new Size(634, 771);
        _richTextBoxGroupBox.TabIndex = 1;
        _richTextBoxGroupBox.TabStop = false;
        _richTextBoxGroupBox.Text = "RichTextBox scenarios";
        // 
        // _richTextBoxTableLayoutPanel
        // 
        _richTextBoxTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _richTextBoxTableLayoutPanel.ColumnCount = 2;
        _richTextBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _richTextBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxDefaultLabel, 0, 0);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxDefault, 1, 0);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxFixedSingleLabel, 0, 1);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxFixedSingle, 1, 1);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxNoWordWrapLabel, 0, 2);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxNoWordWrap, 1, 2);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxReadOnlyLabel, 0, 3);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxReadOnly, 1, 3);
        _richTextBoxTableLayoutPanel.Dock = DockStyle.Fill;
        _richTextBoxTableLayoutPanel.Location = new Point(10, 53);
        _richTextBoxTableLayoutPanel.Margin = new Padding(4);
        _richTextBoxTableLayoutPanel.Name = "_richTextBoxTableLayoutPanel";
        _richTextBoxTableLayoutPanel.RowCount = 4;
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        _richTextBoxTableLayoutPanel.Size = new Size(614, 708);
        _richTextBoxTableLayoutPanel.TabIndex = 0;
        // 
        // _richTextBoxDefaultLabel
        // 
        _richTextBoxDefaultLabel.Anchor = AnchorStyles.Left;
        _richTextBoxDefaultLabel.AutoSize = true;
        _richTextBoxDefaultLabel.Location = new Point(4, 75);
        _richTextBoxDefaultLabel.Margin = new Padding(4, 7, 4, 4);
        _richTextBoxDefaultLabel.Name = "_richTextBoxDefaultLabel";
        _richTextBoxDefaultLabel.Size = new Size(81, 30);
        _richTextBoxDefaultLabel.TabIndex = 0;
        _richTextBoxDefaultLabel.Text = "Default";
        // 
        // _richTextBoxDefault
        // 
        _richTextBoxDefault.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _richTextBoxDefault.Location = new Point(204, 12);
        _richTextBoxDefault.Margin = new Padding(12);
        _richTextBoxDefault.Name = "_richTextBoxDefault";
        _richTextBoxDefault.Size = new Size(398, 153);
        _richTextBoxDefault.TabIndex = 1;
        _richTextBoxDefault.Text = "Hover over me";
        // 
        // _richTextBoxFixedSingleLabel
        // 
        _richTextBoxFixedSingleLabel.Anchor = AnchorStyles.Left;
        _richTextBoxFixedSingleLabel.AutoSize = true;
        _richTextBoxFixedSingleLabel.Location = new Point(4, 252);
        _richTextBoxFixedSingleLabel.Margin = new Padding(4, 7, 4, 4);
        _richTextBoxFixedSingleLabel.Name = "_richTextBoxFixedSingleLabel";
        _richTextBoxFixedSingleLabel.Size = new Size(184, 30);
        _richTextBoxFixedSingleLabel.TabIndex = 2;
        _richTextBoxFixedSingleLabel.Text = "FixedSingle border";
        // 
        // _richTextBoxFixedSingle
        // 
        _richTextBoxFixedSingle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _richTextBoxFixedSingle.BorderStyle = BorderStyle.FixedSingle;
        _richTextBoxFixedSingle.Location = new Point(204, 189);
        _richTextBoxFixedSingle.Margin = new Padding(12);
        _richTextBoxFixedSingle.Name = "_richTextBoxFixedSingle";
        _richTextBoxFixedSingle.Padding = new Padding(1);
        _richTextBoxFixedSingle.Size = new Size(398, 153);
        _richTextBoxFixedSingle.TabIndex = 3;
        _richTextBoxFixedSingle.Text = "Hover over me";
        // 
        // _richTextBoxNoWordWrapLabel
        // 
        _richTextBoxNoWordWrapLabel.Anchor = AnchorStyles.Left;
        _richTextBoxNoWordWrapLabel.AutoSize = true;
        _richTextBoxNoWordWrapLabel.Location = new Point(4, 429);
        _richTextBoxNoWordWrapLabel.Margin = new Padding(4, 7, 4, 4);
        _richTextBoxNoWordWrapLabel.Name = "_richTextBoxNoWordWrapLabel";
        _richTextBoxNoWordWrapLabel.Size = new Size(182, 30);
        _richTextBoxNoWordWrapLabel.TabIndex = 4;
        _richTextBoxNoWordWrapLabel.Text = "WordWrap = false";
        // 
        // _richTextBoxNoWordWrap
        // 
        _richTextBoxNoWordWrap.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _richTextBoxNoWordWrap.Location = new Point(204, 366);
        _richTextBoxNoWordWrap.Margin = new Padding(12);
        _richTextBoxNoWordWrap.Name = "_richTextBoxNoWordWrap";
        _richTextBoxNoWordWrap.Size = new Size(398, 153);
        _richTextBoxNoWordWrap.TabIndex = 5;
        _richTextBoxNoWordWrap.Text = "Hover over me, this is a long line that would normally wrap.";
        _richTextBoxNoWordWrap.WordWrap = false;
        // 
        // _richTextBoxReadOnlyLabel
        // 
        _richTextBoxReadOnlyLabel.Anchor = AnchorStyles.Left;
        _richTextBoxReadOnlyLabel.AutoSize = true;
        _richTextBoxReadOnlyLabel.Location = new Point(4, 606);
        _richTextBoxReadOnlyLabel.Margin = new Padding(4, 7, 4, 4);
        _richTextBoxReadOnlyLabel.Name = "_richTextBoxReadOnlyLabel";
        _richTextBoxReadOnlyLabel.Size = new Size(102, 30);
        _richTextBoxReadOnlyLabel.TabIndex = 6;
        _richTextBoxReadOnlyLabel.Text = "ReadOnly";
        // 
        // _richTextBoxReadOnly
        // 
        _richTextBoxReadOnly.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _richTextBoxReadOnly.Location = new Point(204, 543);
        _richTextBoxReadOnly.Margin = new Padding(12);
        _richTextBoxReadOnly.Name = "_richTextBoxReadOnly";
        _richTextBoxReadOnly.ReadOnly = true;
        _richTextBoxReadOnly.Size = new Size(398, 153);
        _richTextBoxReadOnly.TabIndex = 7;
        _richTextBoxReadOnly.Text = "Hover over me";
        // 
        // TextBoxScenariosView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_rootTableLayoutPanel);
        DoubleBuffered = true;
        Margin = new Padding(4);
        Name = "TextBoxScenariosView";
        Size = new Size(1307, 803);
        VisualStylesMode = VisualStylesMode.Net11;
        _rootTableLayoutPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_textBoxGroupBox).EndInit();
        _textBoxGroupBox.ResumeLayout(false);
        _textBoxTableLayoutPanel.ResumeLayout(false);
        _textBoxTableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
        ((System.ComponentModel.ISupportInitialize)_richTextBoxGroupBox).EndInit();
        _richTextBoxGroupBox.ResumeLayout(false);
        _richTextBoxTableLayoutPanel.ResumeLayout(false);
        _richTextBoxTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _rootTableLayoutPanel;
    private GroupBoxEx _textBoxGroupBox;
    private TableLayoutPanel _textBoxTableLayoutPanel;
    private Label _textBoxDefaultLabel;
    private TextBox _textBoxDefault;
    private Label _textBoxFixedSingleLabel;
    private TextBox _textBoxFixedSingle;
    private Label _textBoxMultilineLabel;
    private TextBox _textBoxMultiline;
    private Label _textBoxNoBorderReadOnlyLabel;
    private TextBox _textBoxNoBorderReadOnly;
    private GroupBoxEx _richTextBoxGroupBox;
    private TableLayoutPanel _richTextBoxTableLayoutPanel;
    private Label _richTextBoxDefaultLabel;
    private RichTextBox _richTextBoxDefault;
    private Label _richTextBoxFixedSingleLabel;
    private RichTextBox _richTextBoxFixedSingle;
    private Label _richTextBoxNoWordWrapLabel;
    private RichTextBox _richTextBoxNoWordWrap;
    private Label _richTextBoxReadOnlyLabel;
    private RichTextBox _richTextBoxReadOnly;
    private NumericUpDown numericUpDown1;
    private Label label1;
}
