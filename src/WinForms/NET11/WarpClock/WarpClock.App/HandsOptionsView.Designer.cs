using System.ComponentModel;

namespace WarpClock.App;

partial class HandsOptionsView
{
    private IContainer components;
    private TableLayoutPanel _layoutPanel;
    private GroupBox _hourGroupBox;
    private TableLayoutPanel _hourLayoutPanel;
    private RadioButton _hourCrawlingRadioButton;
    private RadioButton _hourSweepRadioButton;
    private RadioButton _hourFastTickRadioButton;
    private RadioButton _hourTickRadioButton;
    private GroupBox _minuteGroupBox;
    private TableLayoutPanel _minuteLayoutPanel;
    private RadioButton _minuteCrawlingRadioButton;
    private RadioButton _minuteSweepRadioButton;
    private RadioButton _minuteFastTickRadioButton;
    private RadioButton _minuteTickRadioButton;
    private GroupBox _secondGroupBox;
    private TableLayoutPanel _secondLayoutPanel;
    private RadioButton _secondCrawlingRadioButton;
    private RadioButton _secondSweepRadioButton;
    private RadioButton _secondFastTickRadioButton;
    private RadioButton _secondTickRadioButton;
    private GroupBox _globalGroupBox;
    private TableLayoutPanel _globalLayoutPanel;
    private Label _graceLabel;
    private NumericUpDown _graceNumericUpDown;
    private Label _graceSuffixLabel;
    private Label _hintLabel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        _layoutPanel = new TableLayoutPanel();
        _hourGroupBox = new GroupBox();
        _hourLayoutPanel = new TableLayoutPanel();
        _hourCrawlingRadioButton = new RadioButton();
        _hourSweepRadioButton = new RadioButton();
        _hourFastTickRadioButton = new RadioButton();
        _hourTickRadioButton = new RadioButton();
        _minuteGroupBox = new GroupBox();
        _minuteLayoutPanel = new TableLayoutPanel();
        _minuteCrawlingRadioButton = new RadioButton();
        _minuteSweepRadioButton = new RadioButton();
        _minuteFastTickRadioButton = new RadioButton();
        _minuteTickRadioButton = new RadioButton();
        _secondGroupBox = new GroupBox();
        _secondLayoutPanel = new TableLayoutPanel();
        _secondCrawlingRadioButton = new RadioButton();
        _secondSweepRadioButton = new RadioButton();
        _secondFastTickRadioButton = new RadioButton();
        _secondTickRadioButton = new RadioButton();
        _globalGroupBox = new GroupBox();
        _globalLayoutPanel = new TableLayoutPanel();
        _graceLabel = new Label();
        _graceNumericUpDown = new NumericUpDown();
        _graceSuffixLabel = new Label();
        _hintLabel = new Label();
        _layoutPanel.SuspendLayout();
        _hourGroupBox.SuspendLayout();
        _hourLayoutPanel.SuspendLayout();
        _minuteGroupBox.SuspendLayout();
        _minuteLayoutPanel.SuspendLayout();
        _secondGroupBox.SuspendLayout();
        _secondLayoutPanel.SuspendLayout();
        _globalGroupBox.SuspendLayout();
        _globalLayoutPanel.SuspendLayout();
        ((ISupportInitialize)_graceNumericUpDown).BeginInit();
        SuspendLayout();
        // 
        // _layoutPanel
        // 
        _layoutPanel.ColumnCount = 2;
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _layoutPanel.Controls.Add(_hourGroupBox, 0, 0);
        _layoutPanel.Controls.Add(_minuteGroupBox, 1, 0);
        _layoutPanel.Controls.Add(_secondGroupBox, 0, 1);
        _layoutPanel.Controls.Add(_globalGroupBox, 1, 1);
        _layoutPanel.Dock = DockStyle.Top;
        _layoutPanel.Location = new Point(12, 12);
        _layoutPanel.Name = "_layoutPanel";
        _layoutPanel.RowCount = 2;
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layoutPanel.Size = new Size(736, 430);
        _layoutPanel.TabIndex = 0;
        // 
        // _hourGroupBox
        // 
        _hourGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _hourGroupBox.Controls.Add(_hourLayoutPanel);
        _hourGroupBox.Location = new Point(3, 3);
        _hourGroupBox.Name = "_hourGroupBox";
        _hourGroupBox.Padding = new Padding(12);
        _hourGroupBox.Size = new Size(362, 178);
        _hourGroupBox.TabIndex = 0;
        _hourGroupBox.TabStop = false;
        _hourGroupBox.Text = "Hour hand";
        // 
        // _hourLayoutPanel
        // 
        _hourLayoutPanel.AutoSize = true;
        _hourLayoutPanel.ColumnCount = 1;
        _hourLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _hourLayoutPanel.Controls.Add(_hourCrawlingRadioButton, 0, 0);
        _hourLayoutPanel.Controls.Add(_hourSweepRadioButton, 0, 1);
        _hourLayoutPanel.Controls.Add(_hourFastTickRadioButton, 0, 2);
        _hourLayoutPanel.Controls.Add(_hourTickRadioButton, 0, 3);
        _hourLayoutPanel.Dock = DockStyle.Fill;
        _hourLayoutPanel.Location = new Point(12, 28);
        _hourLayoutPanel.Name = "_hourLayoutPanel";
        _hourLayoutPanel.RowCount = 4;
        _hourLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _hourLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _hourLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _hourLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _hourLayoutPanel.Size = new Size(338, 138);
        _hourLayoutPanel.TabIndex = 0;
        // 
        // _hourCrawlingRadioButton
        // 
        _hourCrawlingRadioButton.AutoSize = true;
        _hourCrawlingRadioButton.Location = new Point(3, 3);
        _hourCrawlingRadioButton.Name = "_hourCrawlingRadioButton";
        _hourCrawlingRadioButton.Size = new Size(78, 19);
        _hourCrawlingRadioButton.TabIndex = 0;
        _hourCrawlingRadioButton.TabStop = true;
        _hourCrawlingRadioButton.Text = "Crawl (move and pause)";
        _hourCrawlingRadioButton.UseVisualStyleBackColor = true;
        // 
        // _hourSweepRadioButton
        // 
        _hourSweepRadioButton.AutoSize = true;
        _hourSweepRadioButton.Location = new Point(3, 28);
        _hourSweepRadioButton.Name = "_hourSweepRadioButton";
        _hourSweepRadioButton.Size = new Size(109, 19);
        _hourSweepRadioButton.TabIndex = 1;
        _hourSweepRadioButton.TabStop = true;
        _hourSweepRadioButton.Text = "Glide continuously";
        _hourSweepRadioButton.UseVisualStyleBackColor = true;
        // 
        // _hourFastTickRadioButton
        // 
        _hourFastTickRadioButton.AutoSize = true;
        _hourFastTickRadioButton.Location = new Point(3, 53);
        _hourFastTickRadioButton.Name = "_hourFastTickRadioButton";
        _hourFastTickRadioButton.Size = new Size(72, 19);
        _hourFastTickRadioButton.TabIndex = 2;
        _hourFastTickRadioButton.TabStop = true;
        _hourFastTickRadioButton.Text = "Fast tick";
        _hourFastTickRadioButton.UseVisualStyleBackColor = true;
        // 
        // _hourTickRadioButton
        // 
        _hourTickRadioButton.AutoSize = true;
        _hourTickRadioButton.Location = new Point(3, 78);
        _hourTickRadioButton.Name = "_hourTickRadioButton";
        _hourTickRadioButton.Size = new Size(46, 19);
        _hourTickRadioButton.TabIndex = 3;
        _hourTickRadioButton.TabStop = true;
        _hourTickRadioButton.Text = "Tick";
        _hourTickRadioButton.UseVisualStyleBackColor = true;
        // 
        // _minuteGroupBox
        // 
        _minuteGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _minuteGroupBox.Controls.Add(_minuteLayoutPanel);
        _minuteGroupBox.Location = new Point(371, 3);
        _minuteGroupBox.Name = "_minuteGroupBox";
        _minuteGroupBox.Padding = new Padding(12);
        _minuteGroupBox.Size = new Size(362, 178);
        _minuteGroupBox.TabIndex = 1;
        _minuteGroupBox.TabStop = false;
        _minuteGroupBox.Text = "Minute hand";
        // 
        // _minuteLayoutPanel
        // 
        _minuteLayoutPanel.AutoSize = true;
        _minuteLayoutPanel.ColumnCount = 1;
        _minuteLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _minuteLayoutPanel.Controls.Add(_minuteCrawlingRadioButton, 0, 0);
        _minuteLayoutPanel.Controls.Add(_minuteSweepRadioButton, 0, 1);
        _minuteLayoutPanel.Controls.Add(_minuteFastTickRadioButton, 0, 2);
        _minuteLayoutPanel.Controls.Add(_minuteTickRadioButton, 0, 3);
        _minuteLayoutPanel.Dock = DockStyle.Fill;
        _minuteLayoutPanel.Location = new Point(12, 28);
        _minuteLayoutPanel.Name = "_minuteLayoutPanel";
        _minuteLayoutPanel.RowCount = 4;
        _minuteLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _minuteLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _minuteLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _minuteLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _minuteLayoutPanel.Size = new Size(338, 138);
        _minuteLayoutPanel.TabIndex = 0;
        // 
        // _minuteCrawlingRadioButton
        // 
        _minuteCrawlingRadioButton.AutoSize = true;
        _minuteCrawlingRadioButton.Location = new Point(3, 3);
        _minuteCrawlingRadioButton.Name = "_minuteCrawlingRadioButton";
        _minuteCrawlingRadioButton.Size = new Size(78, 19);
        _minuteCrawlingRadioButton.TabIndex = 0;
        _minuteCrawlingRadioButton.TabStop = true;
        _minuteCrawlingRadioButton.Text = "Crawl (move and pause)";
        _minuteCrawlingRadioButton.UseVisualStyleBackColor = true;
        // 
        // _minuteSweepRadioButton
        // 
        _minuteSweepRadioButton.AutoSize = true;
        _minuteSweepRadioButton.Location = new Point(3, 28);
        _minuteSweepRadioButton.Name = "_minuteSweepRadioButton";
        _minuteSweepRadioButton.Size = new Size(109, 19);
        _minuteSweepRadioButton.TabIndex = 1;
        _minuteSweepRadioButton.TabStop = true;
        _minuteSweepRadioButton.Text = "Glide continuously";
        _minuteSweepRadioButton.UseVisualStyleBackColor = true;
        // 
        // _minuteFastTickRadioButton
        // 
        _minuteFastTickRadioButton.AutoSize = true;
        _minuteFastTickRadioButton.Location = new Point(3, 53);
        _minuteFastTickRadioButton.Name = "_minuteFastTickRadioButton";
        _minuteFastTickRadioButton.Size = new Size(72, 19);
        _minuteFastTickRadioButton.TabIndex = 2;
        _minuteFastTickRadioButton.TabStop = true;
        _minuteFastTickRadioButton.Text = "Fast tick";
        _minuteFastTickRadioButton.UseVisualStyleBackColor = true;
        // 
        // _minuteTickRadioButton
        // 
        _minuteTickRadioButton.AutoSize = true;
        _minuteTickRadioButton.Location = new Point(3, 78);
        _minuteTickRadioButton.Name = "_minuteTickRadioButton";
        _minuteTickRadioButton.Size = new Size(46, 19);
        _minuteTickRadioButton.TabIndex = 3;
        _minuteTickRadioButton.TabStop = true;
        _minuteTickRadioButton.Text = "Tick";
        _minuteTickRadioButton.UseVisualStyleBackColor = true;
        // 
        // _secondGroupBox
        // 
        _secondGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _secondGroupBox.Controls.Add(_secondLayoutPanel);
        _secondGroupBox.Location = new Point(3, 187);
        _secondGroupBox.Name = "_secondGroupBox";
        _secondGroupBox.Padding = new Padding(12);
        _secondGroupBox.Size = new Size(362, 178);
        _secondGroupBox.TabIndex = 2;
        _secondGroupBox.TabStop = false;
        _secondGroupBox.Text = "Second hand";
        // 
        // _secondLayoutPanel
        // 
        _secondLayoutPanel.AutoSize = true;
        _secondLayoutPanel.ColumnCount = 1;
        _secondLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _secondLayoutPanel.Controls.Add(_secondCrawlingRadioButton, 0, 0);
        _secondLayoutPanel.Controls.Add(_secondSweepRadioButton, 0, 1);
        _secondLayoutPanel.Controls.Add(_secondFastTickRadioButton, 0, 2);
        _secondLayoutPanel.Controls.Add(_secondTickRadioButton, 0, 3);
        _secondLayoutPanel.Dock = DockStyle.Fill;
        _secondLayoutPanel.Location = new Point(12, 28);
        _secondLayoutPanel.Name = "_secondLayoutPanel";
        _secondLayoutPanel.RowCount = 4;
        _secondLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _secondLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _secondLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _secondLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _secondLayoutPanel.Size = new Size(338, 138);
        _secondLayoutPanel.TabIndex = 0;
        // 
        // _secondCrawlingRadioButton
        // 
        _secondCrawlingRadioButton.AutoSize = true;
        _secondCrawlingRadioButton.Location = new Point(3, 3);
        _secondCrawlingRadioButton.Name = "_secondCrawlingRadioButton";
        _secondCrawlingRadioButton.Size = new Size(78, 19);
        _secondCrawlingRadioButton.TabIndex = 0;
        _secondCrawlingRadioButton.TabStop = true;
        _secondCrawlingRadioButton.Text = "Crawl (move and pause)";
        _secondCrawlingRadioButton.UseVisualStyleBackColor = true;
        // 
        // _secondSweepRadioButton
        // 
        _secondSweepRadioButton.AutoSize = true;
        _secondSweepRadioButton.Location = new Point(3, 28);
        _secondSweepRadioButton.Name = "_secondSweepRadioButton";
        _secondSweepRadioButton.Size = new Size(109, 19);
        _secondSweepRadioButton.TabIndex = 1;
        _secondSweepRadioButton.TabStop = true;
        _secondSweepRadioButton.Text = "Glide continuously";
        _secondSweepRadioButton.UseVisualStyleBackColor = true;
        // 
        // _secondFastTickRadioButton
        // 
        _secondFastTickRadioButton.AutoSize = true;
        _secondFastTickRadioButton.Location = new Point(3, 53);
        _secondFastTickRadioButton.Name = "_secondFastTickRadioButton";
        _secondFastTickRadioButton.Size = new Size(72, 19);
        _secondFastTickRadioButton.TabIndex = 2;
        _secondFastTickRadioButton.TabStop = true;
        _secondFastTickRadioButton.Text = "Fast tick";
        _secondFastTickRadioButton.UseVisualStyleBackColor = true;
        // 
        // _secondTickRadioButton
        // 
        _secondTickRadioButton.AutoSize = true;
        _secondTickRadioButton.Location = new Point(3, 78);
        _secondTickRadioButton.Name = "_secondTickRadioButton";
        _secondTickRadioButton.Size = new Size(46, 19);
        _secondTickRadioButton.TabIndex = 3;
        _secondTickRadioButton.TabStop = true;
        _secondTickRadioButton.Text = "Tick";
        _secondTickRadioButton.UseVisualStyleBackColor = true;
        // 
        // _globalGroupBox
        // 
        _globalGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _globalGroupBox.Controls.Add(_globalLayoutPanel);
        _globalGroupBox.Location = new Point(371, 187);
        _globalGroupBox.Name = "_globalGroupBox";
        _globalGroupBox.Padding = new Padding(12);
        _globalGroupBox.Size = new Size(362, 178);
        _globalGroupBox.TabIndex = 3;
        _globalGroupBox.TabStop = false;
        _globalGroupBox.Text = "Global grace";
        // 
        // _globalLayoutPanel
        // 
        _globalLayoutPanel.ColumnCount = 3;
        _globalLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _globalLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _globalLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _globalLayoutPanel.Controls.Add(_graceLabel, 0, 0);
        _globalLayoutPanel.Controls.Add(_graceNumericUpDown, 1, 0);
        _globalLayoutPanel.Controls.Add(_graceSuffixLabel, 2, 0);
        _globalLayoutPanel.Controls.Add(_hintLabel, 0, 1);
        _globalLayoutPanel.Dock = DockStyle.Fill;
        _globalLayoutPanel.Location = new Point(12, 28);
        _globalLayoutPanel.Name = "_globalLayoutPanel";
        _globalLayoutPanel.RowCount = 2;
        _globalLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _globalLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _globalLayoutPanel.Size = new Size(338, 138);
        _globalLayoutPanel.TabIndex = 0;
        // 
        // _graceLabel
        // 
        _graceLabel.Anchor = AnchorStyles.Left;
        _graceLabel.AutoSize = true;
        _graceLabel.Location = new Point(3, 6);
        _graceLabel.Name = "_graceLabel";
        _graceLabel.Size = new Size(113, 15);
        _graceLabel.TabIndex = 0;
        _graceLabel.Text = "Catch-up window:";
        // 
        // _graceNumericUpDown
        // 
        _graceNumericUpDown.Anchor = AnchorStyles.Left;
        _graceNumericUpDown.Location = new Point(122, 3);
        _graceNumericUpDown.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
        _graceNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _graceNumericUpDown.Name = "_graceNumericUpDown";
        _graceNumericUpDown.Size = new Size(72, 23);
        _graceNumericUpDown.TabIndex = 1;
        _graceNumericUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
        // 
        // _graceSuffixLabel
        // 
        _graceSuffixLabel.Anchor = AnchorStyles.Left;
        _graceSuffixLabel.AutoSize = true;
        _graceSuffixLabel.Location = new Point(200, 7);
        _graceSuffixLabel.Name = "_graceSuffixLabel";
        _graceSuffixLabel.Size = new Size(49, 15);
        _graceSuffixLabel.TabIndex = 2;
        _graceSuffixLabel.Text = "seconds";
        // 
        // _hintLabel
        // 
        _hintLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _hintLabel.AutoSize = true;
        _globalLayoutPanel.SetColumnSpan(_hintLabel, 3);
        _hintLabel.Location = new Point(3, 32);
        _hintLabel.Margin = new Padding(3, 3, 3, 0);
        _hintLabel.Name = "_hintLabel";
        _hintLabel.Size = new Size(332, 45);
        _hintLabel.TabIndex = 3;
        _hintLabel.Text = "This shared grace period controls how long a hand may take to converge on the authoritative target after a motion change.";
        // 
        // HandsOptionsView
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_layoutPanel);
        Name = "HandsOptionsView";
        Padding = new Padding(12);
        Size = new Size(760, 480);
        _layoutPanel.ResumeLayout(false);
        _hourGroupBox.ResumeLayout(false);
        _hourGroupBox.PerformLayout();
        _hourLayoutPanel.ResumeLayout(false);
        _hourLayoutPanel.PerformLayout();
        _minuteGroupBox.ResumeLayout(false);
        _minuteGroupBox.PerformLayout();
        _minuteLayoutPanel.ResumeLayout(false);
        _minuteLayoutPanel.PerformLayout();
        _secondGroupBox.ResumeLayout(false);
        _secondGroupBox.PerformLayout();
        _secondLayoutPanel.ResumeLayout(false);
        _secondLayoutPanel.PerformLayout();
        _globalGroupBox.ResumeLayout(false);
        _globalLayoutPanel.ResumeLayout(false);
        _globalLayoutPanel.PerformLayout();
        ((ISupportInitialize)_graceNumericUpDown).EndInit();
        ResumeLayout(false);
    }
}
