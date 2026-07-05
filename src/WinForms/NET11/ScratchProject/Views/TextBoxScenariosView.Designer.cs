// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject.Views;

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
        _textBoxGroupBox = new GroupBox();
        _textBoxTableLayoutPanel = new TableLayoutPanel();
        _textBoxDefaultCheckBox = new CheckBox();
        _textBoxDefault = new TextBox();
        _textBoxFixedSingleCheckBox = new CheckBox();
        _textBoxFixedSingle = new TextBox();
        _textBoxMultilineCheckBox = new CheckBox();
        _textBoxMultiline = new TextBox();
        _textBoxNoBorderReadOnlyCheckBox = new CheckBox();
        _textBoxNoBorderReadOnly = new TextBox();
        _richTextBoxGroupBox = new GroupBox();
        _richTextBoxTableLayoutPanel = new TableLayoutPanel();
        _richTextBoxDefaultCheckBox = new CheckBox();
        _richTextBoxDefault = new RichTextBox();
        _richTextBoxFixedSingleCheckBox = new CheckBox();
        _richTextBoxFixedSingle = new RichTextBox();
        _richTextBoxNoWordWrapCheckBox = new CheckBox();
        _richTextBoxNoWordWrap = new RichTextBox();
        _richTextBoxReadOnlyCheckBox = new CheckBox();
        _richTextBoxReadOnly = new RichTextBox();
        _rootTableLayoutPanel.SuspendLayout();
        _textBoxGroupBox.SuspendLayout();
        _textBoxTableLayoutPanel.SuspendLayout();
        _richTextBoxGroupBox.SuspendLayout();
        _richTextBoxTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        //
        // _rootTableLayoutPanel
        //
        _rootTableLayoutPanel.AutoSize = true;
        _rootTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootTableLayoutPanel.ColumnCount = 2;
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _rootTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _rootTableLayoutPanel.Controls.Add(_textBoxGroupBox, 0, 0);
        _rootTableLayoutPanel.Controls.Add(_richTextBoxGroupBox, 1, 0);
        _rootTableLayoutPanel.Dock = DockStyle.Top;
        _rootTableLayoutPanel.Name = "_rootTableLayoutPanel";
        _rootTableLayoutPanel.RowCount = 1;
        _rootTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _rootTableLayoutPanel.TabIndex = 0;
        //
        // _textBoxGroupBox
        //
        _textBoxGroupBox.AutoSize = true;
        _textBoxGroupBox.AutoSizeMode = AutoSizeMode.GrowOnly;
        _textBoxGroupBox.Controls.Add(_textBoxTableLayoutPanel);
        _textBoxGroupBox.Dock = DockStyle.Fill;
        _textBoxGroupBox.Name = "_textBoxGroupBox";
        _textBoxGroupBox.Padding = new Padding(8);
        _textBoxGroupBox.TabIndex = 0;
        _textBoxGroupBox.TabStop = false;
        _textBoxGroupBox.Text = "TextBox scenarios (NC-paint / hover repro)";
        //
        // _textBoxTableLayoutPanel
        //
        _textBoxTableLayoutPanel.AutoSize = true;
        _textBoxTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _textBoxTableLayoutPanel.ColumnCount = 2;
        _textBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _textBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _textBoxTableLayoutPanel.Controls.Add(_textBoxDefaultCheckBox, 0, 0);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxDefault, 1, 0);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxFixedSingleCheckBox, 0, 1);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxFixedSingle, 1, 1);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxMultilineCheckBox, 0, 2);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxMultiline, 1, 2);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxNoBorderReadOnlyCheckBox, 0, 3);
        _textBoxTableLayoutPanel.Controls.Add(_textBoxNoBorderReadOnly, 1, 3);
        _textBoxTableLayoutPanel.Dock = DockStyle.Fill;
        _textBoxTableLayoutPanel.Location = new Point(8, 23);
        _textBoxTableLayoutPanel.Name = "_textBoxTableLayoutPanel";
        _textBoxTableLayoutPanel.RowCount = 4;
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _textBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _textBoxTableLayoutPanel.TabIndex = 0;
        //
        // _textBoxDefaultCheckBox
        //
        _textBoxDefaultCheckBox.Anchor = AnchorStyles.Left;
        _textBoxDefaultCheckBox.AutoSize = true;
        _textBoxDefaultCheckBox.Margin = new Padding(3, 6, 3, 3);
        _textBoxDefaultCheckBox.Name = "_textBoxDefaultCheckBox";
        _textBoxDefaultCheckBox.TabIndex = 0;
        _textBoxDefaultCheckBox.Text = "Default (Fixed3D)";
        //
        // _textBoxDefault
        //
        _textBoxDefault.Anchor = AnchorStyles.Left;
        _textBoxDefault.Margin = new Padding(3);
        _textBoxDefault.Name = "_textBoxDefault";
        _textBoxDefault.TabIndex = 1;
        _textBoxDefault.Text = "Hover over me";
        _textBoxDefault.Width = 220;
        //
        // _textBoxFixedSingleCheckBox
        //
        _textBoxFixedSingleCheckBox.Anchor = AnchorStyles.Left;
        _textBoxFixedSingleCheckBox.AutoSize = true;
        _textBoxFixedSingleCheckBox.Margin = new Padding(3, 6, 3, 3);
        _textBoxFixedSingleCheckBox.Name = "_textBoxFixedSingleCheckBox";
        _textBoxFixedSingleCheckBox.TabIndex = 2;
        _textBoxFixedSingleCheckBox.Text = "FixedSingle border";
        //
        // _textBoxFixedSingle
        //
        _textBoxFixedSingle.Anchor = AnchorStyles.Left;
        _textBoxFixedSingle.BorderStyle = BorderStyle.FixedSingle;
        _textBoxFixedSingle.Margin = new Padding(3);
        _textBoxFixedSingle.Name = "_textBoxFixedSingle";
        _textBoxFixedSingle.TabIndex = 3;
        _textBoxFixedSingle.Text = "Hover over me";
        _textBoxFixedSingle.Width = 220;
        //
        // _textBoxMultilineCheckBox
        //
        _textBoxMultilineCheckBox.Anchor = AnchorStyles.Left;
        _textBoxMultilineCheckBox.AutoSize = true;
        _textBoxMultilineCheckBox.Margin = new Padding(3, 6, 3, 3);
        _textBoxMultilineCheckBox.Name = "_textBoxMultilineCheckBox";
        _textBoxMultilineCheckBox.TabIndex = 4;
        _textBoxMultilineCheckBox.Text = "Multiline + FixedSingle";
        //
        // _textBoxMultiline
        //
        _textBoxMultiline.Anchor = AnchorStyles.Left;
        _textBoxMultiline.BorderStyle = BorderStyle.FixedSingle;
        _textBoxMultiline.Margin = new Padding(3);
        _textBoxMultiline.Multiline = true;
        _textBoxMultiline.Name = "_textBoxMultiline";
        _textBoxMultiline.Size = new Size(220, 60);
        _textBoxMultiline.TabIndex = 5;
        _textBoxMultiline.Text = "Hover over me\r\nMultiline text";
        //
        // _textBoxNoBorderReadOnlyCheckBox
        //
        _textBoxNoBorderReadOnlyCheckBox.Anchor = AnchorStyles.Left;
        _textBoxNoBorderReadOnlyCheckBox.AutoSize = true;
        _textBoxNoBorderReadOnlyCheckBox.Margin = new Padding(3, 6, 3, 3);
        _textBoxNoBorderReadOnlyCheckBox.Name = "_textBoxNoBorderReadOnlyCheckBox";
        _textBoxNoBorderReadOnlyCheckBox.TabIndex = 6;
        _textBoxNoBorderReadOnlyCheckBox.Text = "No border + ReadOnly";
        //
        // _textBoxNoBorderReadOnly
        //
        _textBoxNoBorderReadOnly.Anchor = AnchorStyles.Left;
        _textBoxNoBorderReadOnly.BorderStyle = BorderStyle.None;
        _textBoxNoBorderReadOnly.Margin = new Padding(3);
        _textBoxNoBorderReadOnly.Name = "_textBoxNoBorderReadOnly";
        _textBoxNoBorderReadOnly.ReadOnly = true;
        _textBoxNoBorderReadOnly.TabIndex = 7;
        _textBoxNoBorderReadOnly.Text = "Hover over me";
        _textBoxNoBorderReadOnly.Width = 220;
        //
        // _richTextBoxGroupBox
        //
        _richTextBoxGroupBox.AutoSize = true;
        _richTextBoxGroupBox.AutoSizeMode = AutoSizeMode.GrowOnly;
        _richTextBoxGroupBox.Controls.Add(_richTextBoxTableLayoutPanel);
        _richTextBoxGroupBox.Dock = DockStyle.Fill;
        _richTextBoxGroupBox.Name = "_richTextBoxGroupBox";
        _richTextBoxGroupBox.Padding = new Padding(8);
        _richTextBoxGroupBox.TabIndex = 1;
        _richTextBoxGroupBox.TabStop = false;
        _richTextBoxGroupBox.Text = "RichTextBox scenarios";
        //
        // _richTextBoxTableLayoutPanel
        //
        _richTextBoxTableLayoutPanel.AutoSize = true;
        _richTextBoxTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _richTextBoxTableLayoutPanel.ColumnCount = 2;
        _richTextBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _richTextBoxTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxDefaultCheckBox, 0, 0);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxDefault, 1, 0);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxFixedSingleCheckBox, 0, 1);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxFixedSingle, 1, 1);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxNoWordWrapCheckBox, 0, 2);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxNoWordWrap, 1, 2);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxReadOnlyCheckBox, 0, 3);
        _richTextBoxTableLayoutPanel.Controls.Add(_richTextBoxReadOnly, 1, 3);
        _richTextBoxTableLayoutPanel.Dock = DockStyle.Fill;
        _richTextBoxTableLayoutPanel.Location = new Point(8, 23);
        _richTextBoxTableLayoutPanel.Name = "_richTextBoxTableLayoutPanel";
        _richTextBoxTableLayoutPanel.RowCount = 4;
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _richTextBoxTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _richTextBoxTableLayoutPanel.TabIndex = 0;
        //
        // _richTextBoxDefaultCheckBox
        //
        _richTextBoxDefaultCheckBox.Anchor = AnchorStyles.Left;
        _richTextBoxDefaultCheckBox.AutoSize = true;
        _richTextBoxDefaultCheckBox.Margin = new Padding(3, 6, 3, 3);
        _richTextBoxDefaultCheckBox.Name = "_richTextBoxDefaultCheckBox";
        _richTextBoxDefaultCheckBox.TabIndex = 0;
        _richTextBoxDefaultCheckBox.Text = "Default";
        //
        // _richTextBoxDefault
        //
        _richTextBoxDefault.Anchor = AnchorStyles.Left;
        _richTextBoxDefault.Margin = new Padding(3);
        _richTextBoxDefault.Name = "_richTextBoxDefault";
        _richTextBoxDefault.Size = new Size(220, 60);
        _richTextBoxDefault.TabIndex = 1;
        _richTextBoxDefault.Text = "Hover over me";
        //
        // _richTextBoxFixedSingleCheckBox
        //
        _richTextBoxFixedSingleCheckBox.Anchor = AnchorStyles.Left;
        _richTextBoxFixedSingleCheckBox.AutoSize = true;
        _richTextBoxFixedSingleCheckBox.Margin = new Padding(3, 6, 3, 3);
        _richTextBoxFixedSingleCheckBox.Name = "_richTextBoxFixedSingleCheckBox";
        _richTextBoxFixedSingleCheckBox.TabIndex = 2;
        _richTextBoxFixedSingleCheckBox.Text = "FixedSingle border";
        //
        // _richTextBoxFixedSingle
        //
        _richTextBoxFixedSingle.Anchor = AnchorStyles.Left;
        _richTextBoxFixedSingle.BorderStyle = BorderStyle.FixedSingle;
        _richTextBoxFixedSingle.Margin = new Padding(3);
        _richTextBoxFixedSingle.Name = "_richTextBoxFixedSingle";
        _richTextBoxFixedSingle.Size = new Size(220, 60);
        _richTextBoxFixedSingle.TabIndex = 3;
        _richTextBoxFixedSingle.Text = "Hover over me";
        //
        // _richTextBoxNoWordWrapCheckBox
        //
        _richTextBoxNoWordWrapCheckBox.Anchor = AnchorStyles.Left;
        _richTextBoxNoWordWrapCheckBox.AutoSize = true;
        _richTextBoxNoWordWrapCheckBox.Margin = new Padding(3, 6, 3, 3);
        _richTextBoxNoWordWrapCheckBox.Name = "_richTextBoxNoWordWrapCheckBox";
        _richTextBoxNoWordWrapCheckBox.TabIndex = 4;
        _richTextBoxNoWordWrapCheckBox.Text = "WordWrap = false";
        //
        // _richTextBoxNoWordWrap
        //
        _richTextBoxNoWordWrap.Anchor = AnchorStyles.Left;
        _richTextBoxNoWordWrap.Margin = new Padding(3);
        _richTextBoxNoWordWrap.Name = "_richTextBoxNoWordWrap";
        _richTextBoxNoWordWrap.Size = new Size(220, 60);
        _richTextBoxNoWordWrap.TabIndex = 5;
        _richTextBoxNoWordWrap.Text = "Hover over me, this is a long line that would normally wrap.";
        _richTextBoxNoWordWrap.WordWrap = false;
        //
        // _richTextBoxReadOnlyCheckBox
        //
        _richTextBoxReadOnlyCheckBox.Anchor = AnchorStyles.Left;
        _richTextBoxReadOnlyCheckBox.AutoSize = true;
        _richTextBoxReadOnlyCheckBox.Margin = new Padding(3, 6, 3, 3);
        _richTextBoxReadOnlyCheckBox.Name = "_richTextBoxReadOnlyCheckBox";
        _richTextBoxReadOnlyCheckBox.TabIndex = 6;
        _richTextBoxReadOnlyCheckBox.Text = "ReadOnly";
        //
        // _richTextBoxReadOnly
        //
        _richTextBoxReadOnly.Anchor = AnchorStyles.Left;
        _richTextBoxReadOnly.Margin = new Padding(3);
        _richTextBoxReadOnly.Name = "_richTextBoxReadOnly";
        _richTextBoxReadOnly.ReadOnly = true;
        _richTextBoxReadOnly.Size = new Size(220, 60);
        _richTextBoxReadOnly.TabIndex = 7;
        _richTextBoxReadOnly.Text = "Hover over me";
        //
        // TextBoxScenariosView
        //
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_rootTableLayoutPanel);
        Name = "TextBoxScenariosView";
        Size = new Size(860, 320);
        _rootTableLayoutPanel.ResumeLayout(false);
        _rootTableLayoutPanel.PerformLayout();
        _textBoxGroupBox.ResumeLayout(false);
        _textBoxGroupBox.PerformLayout();
        _textBoxTableLayoutPanel.ResumeLayout(false);
        _textBoxTableLayoutPanel.PerformLayout();
        _richTextBoxGroupBox.ResumeLayout(false);
        _richTextBoxGroupBox.PerformLayout();
        _richTextBoxTableLayoutPanel.ResumeLayout(false);
        _richTextBoxTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _rootTableLayoutPanel;
    private GroupBox _textBoxGroupBox;
    private TableLayoutPanel _textBoxTableLayoutPanel;
    private CheckBox _textBoxDefaultCheckBox;
    private TextBox _textBoxDefault;
    private CheckBox _textBoxFixedSingleCheckBox;
    private TextBox _textBoxFixedSingle;
    private CheckBox _textBoxMultilineCheckBox;
    private TextBox _textBoxMultiline;
    private CheckBox _textBoxNoBorderReadOnlyCheckBox;
    private TextBox _textBoxNoBorderReadOnly;
    private GroupBox _richTextBoxGroupBox;
    private TableLayoutPanel _richTextBoxTableLayoutPanel;
    private CheckBox _richTextBoxDefaultCheckBox;
    private RichTextBox _richTextBoxDefault;
    private CheckBox _richTextBoxFixedSingleCheckBox;
    private RichTextBox _richTextBoxFixedSingle;
    private CheckBox _richTextBoxNoWordWrapCheckBox;
    private RichTextBox _richTextBoxNoWordWrap;
    private CheckBox _richTextBoxReadOnlyCheckBox;
    private RichTextBox _richTextBoxReadOnly;
}
