// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using VisualStylesModeDemo.Controls;

namespace VisualStylesModeDemo.Views;

partial class CashRegisterView
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
        _rootLayout = new TableLayoutPanel();
        _registerLayout = new TableLayoutPanel();
        _tlp7SegmentContainer = new TableLayoutPanel();
        _display = new SevenSegmentDisplay();
        tableLayoutPanel1 = new TableLayoutPanel();
        checkBox1 = new CheckBox();
        checkBox2 = new CheckBox();
        _keyBodyLayout = new TableLayoutPanel();
        _denominationGrid = new TableLayoutPanel();
        _thousandsHeaderLabel = new Label();
        _hundredsHeaderLabel = new Label();
        _tensHeaderLabel = new Label();
        _onesHeaderLabel = new Label();
        _tenthsHeaderLabel = new Label();
        _hundredthsHeaderLabel = new Label();
        _thousands9Button = new CashRegisterKeyButton();
        _thousands8Button = new CashRegisterKeyButton();
        _thousands7Button = new CashRegisterKeyButton();
        _thousands6Button = new CashRegisterKeyButton();
        _thousands5Button = new CashRegisterKeyButton();
        _thousands4Button = new CashRegisterKeyButton();
        _thousands3Button = new CashRegisterKeyButton();
        _thousands2Button = new CashRegisterKeyButton();
        _thousands1Button = new CashRegisterKeyButton();
        _hundreds9Button = new CashRegisterKeyButton();
        _hundreds8Button = new CashRegisterKeyButton();
        _hundreds7Button = new CashRegisterKeyButton();
        _hundreds6Button = new CashRegisterKeyButton();
        _hundreds5Button = new CashRegisterKeyButton();
        _hundreds4Button = new CashRegisterKeyButton();
        _hundreds3Button = new CashRegisterKeyButton();
        _hundreds2Button = new CashRegisterKeyButton();
        _hundreds1Button = new CashRegisterKeyButton();
        _tens9Button = new CashRegisterKeyButton();
        _tens8Button = new CashRegisterKeyButton();
        _tens7Button = new CashRegisterKeyButton();
        _tens6Button = new CashRegisterKeyButton();
        _tens5Button = new CashRegisterKeyButton();
        _tens4Button = new CashRegisterKeyButton();
        _tens3Button = new CashRegisterKeyButton();
        _tens2Button = new CashRegisterKeyButton();
        _tens1Button = new CashRegisterKeyButton();
        _ones9Button = new CashRegisterKeyButton();
        _ones8Button = new CashRegisterKeyButton();
        _ones7Button = new CashRegisterKeyButton();
        _ones6Button = new CashRegisterKeyButton();
        _ones5Button = new CashRegisterKeyButton();
        _ones4Button = new CashRegisterKeyButton();
        _ones3Button = new CashRegisterKeyButton();
        _ones2Button = new CashRegisterKeyButton();
        _ones1Button = new CashRegisterKeyButton();
        _tenths9Button = new CashRegisterKeyButton();
        _tenths8Button = new CashRegisterKeyButton();
        _tenths7Button = new CashRegisterKeyButton();
        _tenths6Button = new CashRegisterKeyButton();
        _tenths5Button = new CashRegisterKeyButton();
        _tenths4Button = new CashRegisterKeyButton();
        _tenths3Button = new CashRegisterKeyButton();
        _tenths2Button = new CashRegisterKeyButton();
        _tenths1Button = new CashRegisterKeyButton();
        _hundredths9Button = new CashRegisterKeyButton();
        _hundredths8Button = new CashRegisterKeyButton();
        _hundredths7Button = new CashRegisterKeyButton();
        _hundredths6Button = new CashRegisterKeyButton();
        _hundredths5Button = new CashRegisterKeyButton();
        _hundredths4Button = new CashRegisterKeyButton();
        _hundredths3Button = new CashRegisterKeyButton();
        _hundredths2Button = new CashRegisterKeyButton();
        _hundredths1Button = new CashRegisterKeyButton();
        _departmentAndActionLayout = new TableLayoutPanel();
        _departmentGrid = new TableLayoutPanel();
        _taxButton = new CashRegisterKeyButton();
        _department01Button = new CashRegisterKeyButton();
        _department02Button = new CashRegisterKeyButton();
        _department03Button = new CashRegisterKeyButton();
        _department04Button = new CashRegisterKeyButton();
        _department05Button = new CashRegisterKeyButton();
        _department06Button = new CashRegisterKeyButton();
        _department07Button = new CashRegisterKeyButton();
        _department08Button = new CashRegisterKeyButton();
        _department09Button = new CashRegisterKeyButton();
        _department10Button = new CashRegisterKeyButton();
        _department11Button = new CashRegisterKeyButton();
        _department12Button = new CashRegisterKeyButton();
        _department13Button = new CashRegisterKeyButton();
        _department14Button = new CashRegisterKeyButton();
        _department15Button = new CashRegisterKeyButton();
        _department16Button = new CashRegisterKeyButton();
        _department17Button = new CashRegisterKeyButton();
        _department18Button = new CashRegisterKeyButton();
        _actionGrid = new TableLayoutPanel();
        _subtotalButton = new CashRegisterKeyButton();
        _voidButton = new CashRegisterKeyButton();
        _totalButton = new CashRegisterKeyButton();
        _receiptGroupBox = new GroupBox();
        _receiptTextBox = new RichTextBox();
        _rootLayout.SuspendLayout();
        _registerLayout.SuspendLayout();
        _tlp7SegmentContainer.SuspendLayout();
        tableLayoutPanel1.SuspendLayout();
        _keyBodyLayout.SuspendLayout();
        _denominationGrid.SuspendLayout();
        _departmentAndActionLayout.SuspendLayout();
        _departmentGrid.SuspendLayout();
        _actionGrid.SuspendLayout();
        _receiptGroupBox.SuspendLayout();
        SuspendLayout();
        // 
        // _rootLayout
        // 
        _rootLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _rootLayout.AutoSize = true;
        _rootLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _rootLayout.ColumnCount = 2;
        _rootLayout.ColumnStyles.Add(new ColumnStyle());
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootLayout.Controls.Add(_registerLayout, 0, 0);
        _rootLayout.Controls.Add(_receiptGroupBox, 1, 0);
        _rootLayout.Location = new Point(0, 0);
        _rootLayout.Name = "_rootLayout";
        _rootLayout.Padding = new Padding(8);
        _rootLayout.RowCount = 1;
        _rootLayout.RowStyles.Add(new RowStyle());
        _rootLayout.Size = new Size(1517, 977);
        _rootLayout.TabIndex = 0;
        // 
        // _registerLayout
        // 
        _registerLayout.AutoSize = true;
        _registerLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _registerLayout.ColumnCount = 1;
        _registerLayout.ColumnStyles.Add(new ColumnStyle());
        _registerLayout.Controls.Add(_tlp7SegmentContainer, 0, 0);
        _registerLayout.Controls.Add(_keyBodyLayout, 0, 1);
        _registerLayout.Dock = DockStyle.Fill;
        _registerLayout.Location = new Point(11, 11);
        _registerLayout.Name = "_registerLayout";
        _registerLayout.RowCount = 2;
        _registerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _registerLayout.RowStyles.Add(new RowStyle());
        _registerLayout.Size = new Size(1118, 955);
        _registerLayout.TabIndex = 0;
        // 
        // _tlp7SegmentContainer
        // 
        _tlp7SegmentContainer.AutoSize = true;
        _tlp7SegmentContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tlp7SegmentContainer.ColumnCount = 2;
        _tlp7SegmentContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _tlp7SegmentContainer.ColumnStyles.Add(new ColumnStyle());
        _tlp7SegmentContainer.Controls.Add(_display, 1, 0);
        _tlp7SegmentContainer.Controls.Add(tableLayoutPanel1, 0, 0);
        _tlp7SegmentContainer.Dock = DockStyle.Fill;
        _tlp7SegmentContainer.Location = new Point(3, 3);
        _tlp7SegmentContainer.Name = "_tlp7SegmentContainer";
        _tlp7SegmentContainer.RowCount = 1;
        _tlp7SegmentContainer.RowStyles.Add(new RowStyle());
        _tlp7SegmentContainer.Size = new Size(1112, 166);
        _tlp7SegmentContainer.TabIndex = 2;
        // 
        // _display
        // 
        _display.AccessibleName = "Cash register amount display";
        _display.AccessibleRole = AccessibleRole.StaticText;
        _display.BackColor = Color.FromArgb(18, 24, 22);
        _display.Dock = DockStyle.Fill;
        _display.ForeColor = Color.FromArgb(255, 118, 35);
        _display.Location = new Point(306, 6);
        _display.Margin = new Padding(6);
        _display.Name = "_display";
        _display.Size = new Size(800, 154);
        _display.TabIndex = 1;
        _display.TabStop = false;
        // 
        // _tlpDialogResultButtons
        // 
        tableLayoutPanel1.AutoSize = true;
        tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tableLayoutPanel1.ColumnCount = 1;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.Controls.Add(checkBox1, 0, 1);
        tableLayoutPanel1.Controls.Add(checkBox2, 0, 0);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(3, 3);
        tableLayoutPanel1.Name = "_tlpDialogResultButtons";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel1.Size = new Size(294, 160);
        tableLayoutPanel1.TabIndex = 2;
        // 
        // checkBox1
        // 
        checkBox1.Anchor = AnchorStyles.Left;
        checkBox1.Appearance = Appearance.ToggleSwitch;
        checkBox1.AutoSize = true;
        checkBox1.Location = new Point(11, 105);
        checkBox1.Margin = new Padding(11, 3, 3, 3);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(204, 29);
        checkBox1.TabIndex = 3;
        checkBox1.Text = "Use real cash register";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // checkBox2
        // 
        checkBox2.Anchor = AnchorStyles.Left;
        checkBox2.Appearance = Appearance.ToggleSwitch;
        checkBox2.AutoSize = true;
        checkBox2.Location = new Point(11, 25);
        checkBox2.Margin = new Padding(11, 3, 3, 3);
        checkBox2.Name = "checkBox2";
        checkBox2.Size = new Size(215, 29);
        checkBox2.TabIndex = 2;
        checkBox2.Text = "Use real receipt printer";
        checkBox2.UseVisualStyleBackColor = true;
        // 
        // _keyBodyLayout
        // 
        _keyBodyLayout.AutoSize = true;
        _keyBodyLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _keyBodyLayout.ColumnCount = 2;
        _keyBodyLayout.ColumnStyles.Add(new ColumnStyle());
        _keyBodyLayout.ColumnStyles.Add(new ColumnStyle());
        _keyBodyLayout.Controls.Add(_denominationGrid, 0, 0);
        _keyBodyLayout.Controls.Add(_departmentAndActionLayout, 1, 0);
        _keyBodyLayout.Dock = DockStyle.Fill;
        _keyBodyLayout.Location = new Point(3, 175);
        _keyBodyLayout.Name = "_keyBodyLayout";
        _keyBodyLayout.RowCount = 1;
        _keyBodyLayout.RowStyles.Add(new RowStyle());
        _keyBodyLayout.Size = new Size(1112, 777);
        _keyBodyLayout.TabIndex = 1;
        // 
        // _denominationGrid
        // 
        _denominationGrid.AutoSize = true;
        _denominationGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _denominationGrid.ColumnCount = 6;
        _denominationGrid.ColumnStyles.Add(new ColumnStyle());
        _denominationGrid.ColumnStyles.Add(new ColumnStyle());
        _denominationGrid.ColumnStyles.Add(new ColumnStyle());
        _denominationGrid.ColumnStyles.Add(new ColumnStyle());
        _denominationGrid.ColumnStyles.Add(new ColumnStyle());
        _denominationGrid.ColumnStyles.Add(new ColumnStyle());
        _denominationGrid.Controls.Add(_thousandsHeaderLabel, 0, 0);
        _denominationGrid.Controls.Add(_hundredsHeaderLabel, 1, 0);
        _denominationGrid.Controls.Add(_tensHeaderLabel, 2, 0);
        _denominationGrid.Controls.Add(_onesHeaderLabel, 3, 0);
        _denominationGrid.Controls.Add(_tenthsHeaderLabel, 4, 0);
        _denominationGrid.Controls.Add(_hundredthsHeaderLabel, 5, 0);
        _denominationGrid.Controls.Add(_thousands9Button, 0, 1);
        _denominationGrid.Controls.Add(_thousands8Button, 0, 2);
        _denominationGrid.Controls.Add(_thousands7Button, 0, 3);
        _denominationGrid.Controls.Add(_thousands6Button, 0, 4);
        _denominationGrid.Controls.Add(_thousands5Button, 0, 5);
        _denominationGrid.Controls.Add(_thousands4Button, 0, 6);
        _denominationGrid.Controls.Add(_thousands3Button, 0, 7);
        _denominationGrid.Controls.Add(_thousands2Button, 0, 8);
        _denominationGrid.Controls.Add(_thousands1Button, 0, 9);
        _denominationGrid.Controls.Add(_hundreds9Button, 1, 1);
        _denominationGrid.Controls.Add(_hundreds8Button, 1, 2);
        _denominationGrid.Controls.Add(_hundreds7Button, 1, 3);
        _denominationGrid.Controls.Add(_hundreds6Button, 1, 4);
        _denominationGrid.Controls.Add(_hundreds5Button, 1, 5);
        _denominationGrid.Controls.Add(_hundreds4Button, 1, 6);
        _denominationGrid.Controls.Add(_hundreds3Button, 1, 7);
        _denominationGrid.Controls.Add(_hundreds2Button, 1, 8);
        _denominationGrid.Controls.Add(_hundreds1Button, 1, 9);
        _denominationGrid.Controls.Add(_tens9Button, 2, 1);
        _denominationGrid.Controls.Add(_tens8Button, 2, 2);
        _denominationGrid.Controls.Add(_tens7Button, 2, 3);
        _denominationGrid.Controls.Add(_tens6Button, 2, 4);
        _denominationGrid.Controls.Add(_tens5Button, 2, 5);
        _denominationGrid.Controls.Add(_tens4Button, 2, 6);
        _denominationGrid.Controls.Add(_tens3Button, 2, 7);
        _denominationGrid.Controls.Add(_tens2Button, 2, 8);
        _denominationGrid.Controls.Add(_tens1Button, 2, 9);
        _denominationGrid.Controls.Add(_ones9Button, 3, 1);
        _denominationGrid.Controls.Add(_ones8Button, 3, 2);
        _denominationGrid.Controls.Add(_ones7Button, 3, 3);
        _denominationGrid.Controls.Add(_ones6Button, 3, 4);
        _denominationGrid.Controls.Add(_ones5Button, 3, 5);
        _denominationGrid.Controls.Add(_ones4Button, 3, 6);
        _denominationGrid.Controls.Add(_ones3Button, 3, 7);
        _denominationGrid.Controls.Add(_ones2Button, 3, 8);
        _denominationGrid.Controls.Add(_ones1Button, 3, 9);
        _denominationGrid.Controls.Add(_tenths9Button, 4, 1);
        _denominationGrid.Controls.Add(_tenths8Button, 4, 2);
        _denominationGrid.Controls.Add(_tenths7Button, 4, 3);
        _denominationGrid.Controls.Add(_tenths6Button, 4, 4);
        _denominationGrid.Controls.Add(_tenths5Button, 4, 5);
        _denominationGrid.Controls.Add(_tenths4Button, 4, 6);
        _denominationGrid.Controls.Add(_tenths3Button, 4, 7);
        _denominationGrid.Controls.Add(_tenths2Button, 4, 8);
        _denominationGrid.Controls.Add(_tenths1Button, 4, 9);
        _denominationGrid.Controls.Add(_hundredths9Button, 5, 1);
        _denominationGrid.Controls.Add(_hundredths8Button, 5, 2);
        _denominationGrid.Controls.Add(_hundredths7Button, 5, 3);
        _denominationGrid.Controls.Add(_hundredths6Button, 5, 4);
        _denominationGrid.Controls.Add(_hundredths5Button, 5, 5);
        _denominationGrid.Controls.Add(_hundredths4Button, 5, 6);
        _denominationGrid.Controls.Add(_hundredths3Button, 5, 7);
        _denominationGrid.Controls.Add(_hundredths2Button, 5, 8);
        _denominationGrid.Controls.Add(_hundredths1Button, 5, 9);
        _denominationGrid.Dock = DockStyle.Fill;
        _denominationGrid.Location = new Point(3, 3);
        _denominationGrid.Name = "_denominationGrid";
        _denominationGrid.RowCount = 10;
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.Size = new Size(570, 771);
        _denominationGrid.TabIndex = 0;
        // 
        // _thousandsHeaderLabel
        // 
        _thousandsHeaderLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousandsHeaderLabel.AutoSize = true;
        _thousandsHeaderLabel.Location = new Point(10, 10);
        _thousandsHeaderLabel.Margin = new Padding(10, 10, 10, 5);
        _thousandsHeaderLabel.Name = "_thousandsHeaderLabel";
        _thousandsHeaderLabel.Padding = new Padding(10);
        _thousandsHeaderLabel.Size = new Size(80, 45);
        _thousandsHeaderLabel.TabIndex = 0;
        _thousandsHeaderLabel.Text = "THOU";
        _thousandsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _hundredsHeaderLabel
        // 
        _hundredsHeaderLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredsHeaderLabel.AutoSize = true;
        _hundredsHeaderLabel.Location = new Point(110, 10);
        _hundredsHeaderLabel.Margin = new Padding(10, 10, 10, 5);
        _hundredsHeaderLabel.Name = "_hundredsHeaderLabel";
        _hundredsHeaderLabel.Padding = new Padding(10);
        _hundredsHeaderLabel.Size = new Size(76, 45);
        _hundredsHeaderLabel.TabIndex = 0;
        _hundredsHeaderLabel.Text = "HUN";
        _hundredsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _tensHeaderLabel
        // 
        _tensHeaderLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tensHeaderLabel.AutoSize = true;
        _tensHeaderLabel.Location = new Point(206, 10);
        _tensHeaderLabel.Margin = new Padding(10, 10, 10, 5);
        _tensHeaderLabel.Name = "_tensHeaderLabel";
        _tensHeaderLabel.Padding = new Padding(10);
        _tensHeaderLabel.Size = new Size(76, 45);
        _tensHeaderLabel.TabIndex = 0;
        _tensHeaderLabel.Text = "TENS";
        _tensHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _onesHeaderLabel
        // 
        _onesHeaderLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _onesHeaderLabel.AutoSize = true;
        _onesHeaderLabel.Location = new Point(302, 10);
        _onesHeaderLabel.Margin = new Padding(10, 10, 10, 5);
        _onesHeaderLabel.Name = "_onesHeaderLabel";
        _onesHeaderLabel.Padding = new Padding(10);
        _onesHeaderLabel.Size = new Size(78, 45);
        _onesHeaderLabel.TabIndex = 0;
        _onesHeaderLabel.Text = "ONES";
        _onesHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _tenthsHeaderLabel
        // 
        _tenthsHeaderLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenthsHeaderLabel.AutoSize = true;
        _tenthsHeaderLabel.Location = new Point(400, 10);
        _tenthsHeaderLabel.Margin = new Padding(10, 10, 10, 5);
        _tenthsHeaderLabel.Name = "_tenthsHeaderLabel";
        _tenthsHeaderLabel.Padding = new Padding(10);
        _tenthsHeaderLabel.Size = new Size(70, 45);
        _tenthsHeaderLabel.TabIndex = 0;
        _tenthsHeaderLabel.Text = "10c";
        _tenthsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _hundredthsHeaderLabel
        // 
        _hundredthsHeaderLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredthsHeaderLabel.AutoSize = true;
        _hundredthsHeaderLabel.Location = new Point(490, 10);
        _hundredthsHeaderLabel.Margin = new Padding(10, 10, 10, 5);
        _hundredthsHeaderLabel.Name = "_hundredthsHeaderLabel";
        _hundredthsHeaderLabel.Padding = new Padding(10);
        _hundredthsHeaderLabel.Size = new Size(70, 45);
        _hundredthsHeaderLabel.TabIndex = 0;
        _hundredthsHeaderLabel.Text = "1c";
        _hundredthsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _thousands9Button
        // 
        _thousands9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands9Button.AutoSize = true;
        _thousands9Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands9Button.Location = new Point(8, 68);
        _thousands9Button.Margin = new Padding(8);
        _thousands9Button.Name = "_thousands9Button";
        _thousands9Button.Padding = new Padding(14);
        _thousands9Button.Size = new Size(84, 63);
        _thousands9Button.TabIndex = 1;
        _thousands9Button.Text = "9K";
        _thousands9Button.UseVisualStyleBackColor = false;
        _thousands9Button.Click += DenominationButton_Click;
        // 
        // _thousands8Button
        // 
        _thousands8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands8Button.AutoSize = true;
        _thousands8Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands8Button.Location = new Point(8, 147);
        _thousands8Button.Margin = new Padding(8);
        _thousands8Button.Name = "_thousands8Button";
        _thousands8Button.Padding = new Padding(14);
        _thousands8Button.Size = new Size(84, 63);
        _thousands8Button.TabIndex = 7;
        _thousands8Button.Text = "8K";
        _thousands8Button.UseVisualStyleBackColor = false;
        _thousands8Button.Click += DenominationButton_Click;
        // 
        // _thousands7Button
        // 
        _thousands7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands7Button.AutoSize = true;
        _thousands7Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands7Button.Location = new Point(8, 226);
        _thousands7Button.Margin = new Padding(8);
        _thousands7Button.Name = "_thousands7Button";
        _thousands7Button.Padding = new Padding(14);
        _thousands7Button.Size = new Size(84, 63);
        _thousands7Button.TabIndex = 13;
        _thousands7Button.Text = "7K";
        _thousands7Button.UseVisualStyleBackColor = false;
        _thousands7Button.Click += DenominationButton_Click;
        // 
        // _thousands6Button
        // 
        _thousands6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands6Button.AutoSize = true;
        _thousands6Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands6Button.Location = new Point(8, 305);
        _thousands6Button.Margin = new Padding(8);
        _thousands6Button.Name = "_thousands6Button";
        _thousands6Button.Padding = new Padding(14);
        _thousands6Button.Size = new Size(84, 63);
        _thousands6Button.TabIndex = 19;
        _thousands6Button.Text = "6K";
        _thousands6Button.UseVisualStyleBackColor = false;
        _thousands6Button.Click += DenominationButton_Click;
        // 
        // _thousands5Button
        // 
        _thousands5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands5Button.AutoSize = true;
        _thousands5Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands5Button.Location = new Point(8, 384);
        _thousands5Button.Margin = new Padding(8);
        _thousands5Button.Name = "_thousands5Button";
        _thousands5Button.Padding = new Padding(14);
        _thousands5Button.Size = new Size(84, 63);
        _thousands5Button.TabIndex = 25;
        _thousands5Button.Text = "5K";
        _thousands5Button.UseVisualStyleBackColor = false;
        _thousands5Button.Click += DenominationButton_Click;
        // 
        // _thousands4Button
        // 
        _thousands4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands4Button.AutoSize = true;
        _thousands4Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands4Button.Location = new Point(8, 463);
        _thousands4Button.Margin = new Padding(8);
        _thousands4Button.Name = "_thousands4Button";
        _thousands4Button.Padding = new Padding(14);
        _thousands4Button.Size = new Size(84, 63);
        _thousands4Button.TabIndex = 31;
        _thousands4Button.Text = "4K";
        _thousands4Button.UseVisualStyleBackColor = false;
        _thousands4Button.Click += DenominationButton_Click;
        // 
        // _thousands3Button
        // 
        _thousands3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands3Button.AutoSize = true;
        _thousands3Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands3Button.Location = new Point(8, 542);
        _thousands3Button.Margin = new Padding(8);
        _thousands3Button.Name = "_thousands3Button";
        _thousands3Button.Padding = new Padding(14);
        _thousands3Button.Size = new Size(84, 63);
        _thousands3Button.TabIndex = 37;
        _thousands3Button.Text = "3K";
        _thousands3Button.UseVisualStyleBackColor = false;
        _thousands3Button.Click += DenominationButton_Click;
        // 
        // _thousands2Button
        // 
        _thousands2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands2Button.AutoSize = true;
        _thousands2Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands2Button.Location = new Point(8, 621);
        _thousands2Button.Margin = new Padding(8);
        _thousands2Button.Name = "_thousands2Button";
        _thousands2Button.Padding = new Padding(14);
        _thousands2Button.Size = new Size(84, 63);
        _thousands2Button.TabIndex = 43;
        _thousands2Button.Text = "2K";
        _thousands2Button.UseVisualStyleBackColor = false;
        _thousands2Button.Click += DenominationButton_Click;
        // 
        // _thousands1Button
        // 
        _thousands1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands1Button.AutoSize = true;
        _thousands1Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _thousands1Button.Location = new Point(8, 700);
        _thousands1Button.Margin = new Padding(8);
        _thousands1Button.Name = "_thousands1Button";
        _thousands1Button.Padding = new Padding(14);
        _thousands1Button.Size = new Size(84, 63);
        _thousands1Button.TabIndex = 49;
        _thousands1Button.Text = "1K";
        _thousands1Button.UseVisualStyleBackColor = false;
        _thousands1Button.Click += DenominationButton_Click;
        // 
        // _hundreds9Button
        // 
        _hundreds9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds9Button.AutoSize = true;
        _hundreds9Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds9Button.Location = new Point(108, 68);
        _hundreds9Button.Margin = new Padding(8);
        _hundreds9Button.Name = "_hundreds9Button";
        _hundreds9Button.Padding = new Padding(14);
        _hundreds9Button.Size = new Size(80, 63);
        _hundreds9Button.TabIndex = 2;
        _hundreds9Button.Text = "900";
        _hundreds9Button.UseVisualStyleBackColor = false;
        _hundreds9Button.Click += DenominationButton_Click;
        // 
        // _hundreds8Button
        // 
        _hundreds8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds8Button.AutoSize = true;
        _hundreds8Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds8Button.Location = new Point(108, 147);
        _hundreds8Button.Margin = new Padding(8);
        _hundreds8Button.Name = "_hundreds8Button";
        _hundreds8Button.Padding = new Padding(14);
        _hundreds8Button.Size = new Size(80, 63);
        _hundreds8Button.TabIndex = 8;
        _hundreds8Button.Text = "800";
        _hundreds8Button.UseVisualStyleBackColor = false;
        _hundreds8Button.Click += DenominationButton_Click;
        // 
        // _hundreds7Button
        // 
        _hundreds7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds7Button.AutoSize = true;
        _hundreds7Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds7Button.Location = new Point(108, 226);
        _hundreds7Button.Margin = new Padding(8);
        _hundreds7Button.Name = "_hundreds7Button";
        _hundreds7Button.Padding = new Padding(14);
        _hundreds7Button.Size = new Size(80, 63);
        _hundreds7Button.TabIndex = 14;
        _hundreds7Button.Text = "700";
        _hundreds7Button.UseVisualStyleBackColor = false;
        _hundreds7Button.Click += DenominationButton_Click;
        // 
        // _hundreds6Button
        // 
        _hundreds6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds6Button.AutoSize = true;
        _hundreds6Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds6Button.Location = new Point(108, 305);
        _hundreds6Button.Margin = new Padding(8);
        _hundreds6Button.Name = "_hundreds6Button";
        _hundreds6Button.Padding = new Padding(14);
        _hundreds6Button.Size = new Size(80, 63);
        _hundreds6Button.TabIndex = 20;
        _hundreds6Button.Text = "600";
        _hundreds6Button.UseVisualStyleBackColor = false;
        _hundreds6Button.Click += DenominationButton_Click;
        // 
        // _hundreds5Button
        // 
        _hundreds5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds5Button.AutoSize = true;
        _hundreds5Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds5Button.Location = new Point(108, 384);
        _hundreds5Button.Margin = new Padding(8);
        _hundreds5Button.Name = "_hundreds5Button";
        _hundreds5Button.Padding = new Padding(14);
        _hundreds5Button.Size = new Size(80, 63);
        _hundreds5Button.TabIndex = 26;
        _hundreds5Button.Text = "500";
        _hundreds5Button.UseVisualStyleBackColor = false;
        _hundreds5Button.Click += DenominationButton_Click;
        // 
        // _hundreds4Button
        // 
        _hundreds4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds4Button.AutoSize = true;
        _hundreds4Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds4Button.Location = new Point(108, 463);
        _hundreds4Button.Margin = new Padding(8);
        _hundreds4Button.Name = "_hundreds4Button";
        _hundreds4Button.Padding = new Padding(14);
        _hundreds4Button.Size = new Size(80, 63);
        _hundreds4Button.TabIndex = 32;
        _hundreds4Button.Text = "400";
        _hundreds4Button.UseVisualStyleBackColor = false;
        _hundreds4Button.Click += DenominationButton_Click;
        // 
        // _hundreds3Button
        // 
        _hundreds3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds3Button.AutoSize = true;
        _hundreds3Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds3Button.Location = new Point(108, 542);
        _hundreds3Button.Margin = new Padding(8);
        _hundreds3Button.Name = "_hundreds3Button";
        _hundreds3Button.Padding = new Padding(14);
        _hundreds3Button.Size = new Size(80, 63);
        _hundreds3Button.TabIndex = 38;
        _hundreds3Button.Text = "300";
        _hundreds3Button.UseVisualStyleBackColor = false;
        _hundreds3Button.Click += DenominationButton_Click;
        // 
        // _hundreds2Button
        // 
        _hundreds2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds2Button.AutoSize = true;
        _hundreds2Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds2Button.Location = new Point(108, 621);
        _hundreds2Button.Margin = new Padding(8);
        _hundreds2Button.Name = "_hundreds2Button";
        _hundreds2Button.Padding = new Padding(14);
        _hundreds2Button.Size = new Size(80, 63);
        _hundreds2Button.TabIndex = 44;
        _hundreds2Button.Text = "200";
        _hundreds2Button.UseVisualStyleBackColor = false;
        _hundreds2Button.Click += DenominationButton_Click;
        // 
        // _hundreds1Button
        // 
        _hundreds1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds1Button.AutoSize = true;
        _hundreds1Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundreds1Button.Location = new Point(108, 700);
        _hundreds1Button.Margin = new Padding(8);
        _hundreds1Button.Name = "_hundreds1Button";
        _hundreds1Button.Padding = new Padding(14);
        _hundreds1Button.Size = new Size(80, 63);
        _hundreds1Button.TabIndex = 50;
        _hundreds1Button.Text = "100";
        _hundreds1Button.UseVisualStyleBackColor = false;
        _hundreds1Button.Click += DenominationButton_Click;
        // 
        // _tens9Button
        // 
        _tens9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens9Button.AutoSize = true;
        _tens9Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens9Button.Location = new Point(204, 68);
        _tens9Button.Margin = new Padding(8);
        _tens9Button.Name = "_tens9Button";
        _tens9Button.Padding = new Padding(14);
        _tens9Button.Size = new Size(80, 63);
        _tens9Button.TabIndex = 3;
        _tens9Button.Text = "$90";
        _tens9Button.UseVisualStyleBackColor = false;
        _tens9Button.Click += DenominationButton_Click;
        // 
        // _tens8Button
        // 
        _tens8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens8Button.AutoSize = true;
        _tens8Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens8Button.Location = new Point(204, 147);
        _tens8Button.Margin = new Padding(8);
        _tens8Button.Name = "_tens8Button";
        _tens8Button.Padding = new Padding(14);
        _tens8Button.Size = new Size(80, 63);
        _tens8Button.TabIndex = 9;
        _tens8Button.Text = "$80";
        _tens8Button.UseVisualStyleBackColor = false;
        _tens8Button.Click += DenominationButton_Click;
        // 
        // _tens7Button
        // 
        _tens7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens7Button.AutoSize = true;
        _tens7Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens7Button.Location = new Point(204, 226);
        _tens7Button.Margin = new Padding(8);
        _tens7Button.Name = "_tens7Button";
        _tens7Button.Padding = new Padding(14);
        _tens7Button.Size = new Size(80, 63);
        _tens7Button.TabIndex = 15;
        _tens7Button.Text = "$70";
        _tens7Button.UseVisualStyleBackColor = false;
        _tens7Button.Click += DenominationButton_Click;
        // 
        // _tens6Button
        // 
        _tens6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens6Button.AutoSize = true;
        _tens6Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens6Button.Location = new Point(204, 305);
        _tens6Button.Margin = new Padding(8);
        _tens6Button.Name = "_tens6Button";
        _tens6Button.Padding = new Padding(14);
        _tens6Button.Size = new Size(80, 63);
        _tens6Button.TabIndex = 21;
        _tens6Button.Text = "$60";
        _tens6Button.UseVisualStyleBackColor = false;
        _tens6Button.Click += DenominationButton_Click;
        // 
        // _tens5Button
        // 
        _tens5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens5Button.AutoSize = true;
        _tens5Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens5Button.Location = new Point(204, 384);
        _tens5Button.Margin = new Padding(8);
        _tens5Button.Name = "_tens5Button";
        _tens5Button.Padding = new Padding(14);
        _tens5Button.Size = new Size(80, 63);
        _tens5Button.TabIndex = 27;
        _tens5Button.Text = "$50";
        _tens5Button.UseVisualStyleBackColor = false;
        _tens5Button.Click += DenominationButton_Click;
        // 
        // _tens4Button
        // 
        _tens4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens4Button.AutoSize = true;
        _tens4Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens4Button.Location = new Point(204, 463);
        _tens4Button.Margin = new Padding(8);
        _tens4Button.Name = "_tens4Button";
        _tens4Button.Padding = new Padding(14);
        _tens4Button.Size = new Size(80, 63);
        _tens4Button.TabIndex = 33;
        _tens4Button.Text = "$40";
        _tens4Button.UseVisualStyleBackColor = false;
        _tens4Button.Click += DenominationButton_Click;
        // 
        // _tens3Button
        // 
        _tens3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens3Button.AutoSize = true;
        _tens3Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens3Button.Location = new Point(204, 542);
        _tens3Button.Margin = new Padding(8);
        _tens3Button.Name = "_tens3Button";
        _tens3Button.Padding = new Padding(14);
        _tens3Button.Size = new Size(80, 63);
        _tens3Button.TabIndex = 39;
        _tens3Button.Text = "$30";
        _tens3Button.UseVisualStyleBackColor = false;
        _tens3Button.Click += DenominationButton_Click;
        // 
        // _tens2Button
        // 
        _tens2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens2Button.AutoSize = true;
        _tens2Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens2Button.Location = new Point(204, 621);
        _tens2Button.Margin = new Padding(8);
        _tens2Button.Name = "_tens2Button";
        _tens2Button.Padding = new Padding(14);
        _tens2Button.Size = new Size(80, 63);
        _tens2Button.TabIndex = 45;
        _tens2Button.Text = "$20";
        _tens2Button.UseVisualStyleBackColor = false;
        _tens2Button.Click += DenominationButton_Click;
        // 
        // _tens1Button
        // 
        _tens1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens1Button.AutoSize = true;
        _tens1Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tens1Button.Location = new Point(204, 700);
        _tens1Button.Margin = new Padding(8);
        _tens1Button.Name = "_tens1Button";
        _tens1Button.Padding = new Padding(14);
        _tens1Button.Size = new Size(80, 63);
        _tens1Button.TabIndex = 51;
        _tens1Button.Text = "$10";
        _tens1Button.UseVisualStyleBackColor = false;
        _tens1Button.Click += DenominationButton_Click;
        // 
        // _ones9Button
        // 
        _ones9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones9Button.AutoSize = true;
        _ones9Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones9Button.Location = new Point(300, 68);
        _ones9Button.Margin = new Padding(8);
        _ones9Button.Name = "_ones9Button";
        _ones9Button.Padding = new Padding(14);
        _ones9Button.Size = new Size(82, 63);
        _ones9Button.TabIndex = 4;
        _ones9Button.Text = "$9";
        _ones9Button.UseVisualStyleBackColor = false;
        _ones9Button.Click += DenominationButton_Click;
        // 
        // _ones8Button
        // 
        _ones8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones8Button.AutoSize = true;
        _ones8Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones8Button.Location = new Point(300, 147);
        _ones8Button.Margin = new Padding(8);
        _ones8Button.Name = "_ones8Button";
        _ones8Button.Padding = new Padding(14);
        _ones8Button.Size = new Size(82, 63);
        _ones8Button.TabIndex = 10;
        _ones8Button.Text = "$8";
        _ones8Button.UseVisualStyleBackColor = false;
        _ones8Button.Click += DenominationButton_Click;
        // 
        // _ones7Button
        // 
        _ones7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones7Button.AutoSize = true;
        _ones7Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones7Button.Location = new Point(300, 226);
        _ones7Button.Margin = new Padding(8);
        _ones7Button.Name = "_ones7Button";
        _ones7Button.Padding = new Padding(14);
        _ones7Button.Size = new Size(82, 63);
        _ones7Button.TabIndex = 16;
        _ones7Button.Text = "$7";
        _ones7Button.UseVisualStyleBackColor = false;
        _ones7Button.Click += DenominationButton_Click;
        // 
        // _ones6Button
        // 
        _ones6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones6Button.AutoSize = true;
        _ones6Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones6Button.Location = new Point(300, 305);
        _ones6Button.Margin = new Padding(8);
        _ones6Button.Name = "_ones6Button";
        _ones6Button.Padding = new Padding(14);
        _ones6Button.Size = new Size(82, 63);
        _ones6Button.TabIndex = 22;
        _ones6Button.Text = "$6";
        _ones6Button.UseVisualStyleBackColor = false;
        _ones6Button.Click += DenominationButton_Click;
        // 
        // _ones5Button
        // 
        _ones5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones5Button.AutoSize = true;
        _ones5Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones5Button.Location = new Point(300, 384);
        _ones5Button.Margin = new Padding(8);
        _ones5Button.Name = "_ones5Button";
        _ones5Button.Padding = new Padding(14);
        _ones5Button.Size = new Size(82, 63);
        _ones5Button.TabIndex = 28;
        _ones5Button.Text = "$5";
        _ones5Button.UseVisualStyleBackColor = false;
        _ones5Button.Click += DenominationButton_Click;
        // 
        // _ones4Button
        // 
        _ones4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones4Button.AutoSize = true;
        _ones4Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones4Button.Location = new Point(300, 463);
        _ones4Button.Margin = new Padding(8);
        _ones4Button.Name = "_ones4Button";
        _ones4Button.Padding = new Padding(14);
        _ones4Button.Size = new Size(82, 63);
        _ones4Button.TabIndex = 34;
        _ones4Button.Text = "$4";
        _ones4Button.UseVisualStyleBackColor = false;
        _ones4Button.Click += DenominationButton_Click;
        // 
        // _ones3Button
        // 
        _ones3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones3Button.AutoSize = true;
        _ones3Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones3Button.Location = new Point(300, 542);
        _ones3Button.Margin = new Padding(8);
        _ones3Button.Name = "_ones3Button";
        _ones3Button.Padding = new Padding(14);
        _ones3Button.Size = new Size(82, 63);
        _ones3Button.TabIndex = 40;
        _ones3Button.Text = "$3";
        _ones3Button.UseVisualStyleBackColor = false;
        _ones3Button.Click += DenominationButton_Click;
        // 
        // _ones2Button
        // 
        _ones2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones2Button.AutoSize = true;
        _ones2Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones2Button.Location = new Point(300, 621);
        _ones2Button.Margin = new Padding(8);
        _ones2Button.Name = "_ones2Button";
        _ones2Button.Padding = new Padding(14);
        _ones2Button.Size = new Size(82, 63);
        _ones2Button.TabIndex = 46;
        _ones2Button.Text = "$2";
        _ones2Button.UseVisualStyleBackColor = false;
        _ones2Button.Click += DenominationButton_Click;
        // 
        // _ones1Button
        // 
        _ones1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones1Button.AutoSize = true;
        _ones1Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _ones1Button.Location = new Point(300, 700);
        _ones1Button.Margin = new Padding(8);
        _ones1Button.Name = "_ones1Button";
        _ones1Button.Padding = new Padding(14);
        _ones1Button.Size = new Size(82, 63);
        _ones1Button.TabIndex = 52;
        _ones1Button.Text = "$1";
        _ones1Button.UseVisualStyleBackColor = false;
        _ones1Button.Click += DenominationButton_Click;
        // 
        // _tenths9Button
        // 
        _tenths9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths9Button.AutoSize = true;
        _tenths9Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths9Button.Location = new Point(398, 68);
        _tenths9Button.Margin = new Padding(8);
        _tenths9Button.Name = "_tenths9Button";
        _tenths9Button.Padding = new Padding(14);
        _tenths9Button.Size = new Size(74, 63);
        _tenths9Button.TabIndex = 5;
        _tenths9Button.Text = ".90";
        _tenths9Button.UseVisualStyleBackColor = false;
        _tenths9Button.Click += DenominationButton_Click;
        // 
        // _tenths8Button
        // 
        _tenths8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths8Button.AutoSize = true;
        _tenths8Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths8Button.Location = new Point(398, 147);
        _tenths8Button.Margin = new Padding(8);
        _tenths8Button.Name = "_tenths8Button";
        _tenths8Button.Padding = new Padding(14);
        _tenths8Button.Size = new Size(74, 63);
        _tenths8Button.TabIndex = 11;
        _tenths8Button.Text = ".80";
        _tenths8Button.UseVisualStyleBackColor = false;
        _tenths8Button.Click += DenominationButton_Click;
        // 
        // _tenths7Button
        // 
        _tenths7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths7Button.AutoSize = true;
        _tenths7Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths7Button.Location = new Point(398, 226);
        _tenths7Button.Margin = new Padding(8);
        _tenths7Button.Name = "_tenths7Button";
        _tenths7Button.Padding = new Padding(14);
        _tenths7Button.Size = new Size(74, 63);
        _tenths7Button.TabIndex = 17;
        _tenths7Button.Text = ".70";
        _tenths7Button.UseVisualStyleBackColor = false;
        _tenths7Button.Click += DenominationButton_Click;
        // 
        // _tenths6Button
        // 
        _tenths6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths6Button.AutoSize = true;
        _tenths6Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths6Button.Location = new Point(398, 305);
        _tenths6Button.Margin = new Padding(8);
        _tenths6Button.Name = "_tenths6Button";
        _tenths6Button.Padding = new Padding(14);
        _tenths6Button.Size = new Size(74, 63);
        _tenths6Button.TabIndex = 23;
        _tenths6Button.Text = ".60";
        _tenths6Button.UseVisualStyleBackColor = false;
        _tenths6Button.Click += DenominationButton_Click;
        // 
        // _tenths5Button
        // 
        _tenths5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths5Button.AutoSize = true;
        _tenths5Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths5Button.Location = new Point(398, 384);
        _tenths5Button.Margin = new Padding(8);
        _tenths5Button.Name = "_tenths5Button";
        _tenths5Button.Padding = new Padding(14);
        _tenths5Button.Size = new Size(74, 63);
        _tenths5Button.TabIndex = 29;
        _tenths5Button.Text = ".50";
        _tenths5Button.UseVisualStyleBackColor = false;
        _tenths5Button.Click += DenominationButton_Click;
        // 
        // _tenths4Button
        // 
        _tenths4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths4Button.AutoSize = true;
        _tenths4Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths4Button.Location = new Point(398, 463);
        _tenths4Button.Margin = new Padding(8);
        _tenths4Button.Name = "_tenths4Button";
        _tenths4Button.Padding = new Padding(14);
        _tenths4Button.Size = new Size(74, 63);
        _tenths4Button.TabIndex = 35;
        _tenths4Button.Text = ".40";
        _tenths4Button.UseVisualStyleBackColor = false;
        _tenths4Button.Click += DenominationButton_Click;
        // 
        // _tenths3Button
        // 
        _tenths3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths3Button.AutoSize = true;
        _tenths3Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths3Button.Location = new Point(398, 542);
        _tenths3Button.Margin = new Padding(8);
        _tenths3Button.Name = "_tenths3Button";
        _tenths3Button.Padding = new Padding(14);
        _tenths3Button.Size = new Size(74, 63);
        _tenths3Button.TabIndex = 41;
        _tenths3Button.Text = ".30";
        _tenths3Button.UseVisualStyleBackColor = false;
        _tenths3Button.Click += DenominationButton_Click;
        // 
        // _tenths2Button
        // 
        _tenths2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths2Button.AutoSize = true;
        _tenths2Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths2Button.Location = new Point(398, 621);
        _tenths2Button.Margin = new Padding(8);
        _tenths2Button.Name = "_tenths2Button";
        _tenths2Button.Padding = new Padding(14);
        _tenths2Button.Size = new Size(74, 63);
        _tenths2Button.TabIndex = 47;
        _tenths2Button.Text = ".20";
        _tenths2Button.UseVisualStyleBackColor = false;
        _tenths2Button.Click += DenominationButton_Click;
        // 
        // _tenths1Button
        // 
        _tenths1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths1Button.AutoSize = true;
        _tenths1Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _tenths1Button.Location = new Point(398, 700);
        _tenths1Button.Margin = new Padding(8);
        _tenths1Button.Name = "_tenths1Button";
        _tenths1Button.Padding = new Padding(14);
        _tenths1Button.Size = new Size(74, 63);
        _tenths1Button.TabIndex = 53;
        _tenths1Button.Text = ".10";
        _tenths1Button.UseVisualStyleBackColor = false;
        _tenths1Button.Click += DenominationButton_Click;
        // 
        // _hundredths9Button
        // 
        _hundredths9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths9Button.AutoSize = true;
        _hundredths9Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths9Button.Location = new Point(488, 68);
        _hundredths9Button.Margin = new Padding(8);
        _hundredths9Button.Name = "_hundredths9Button";
        _hundredths9Button.Padding = new Padding(14);
        _hundredths9Button.Size = new Size(74, 63);
        _hundredths9Button.TabIndex = 6;
        _hundredths9Button.Text = ".09";
        _hundredths9Button.UseVisualStyleBackColor = false;
        _hundredths9Button.Click += DenominationButton_Click;
        // 
        // _hundredths8Button
        // 
        _hundredths8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths8Button.AutoSize = true;
        _hundredths8Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths8Button.Location = new Point(488, 147);
        _hundredths8Button.Margin = new Padding(8);
        _hundredths8Button.Name = "_hundredths8Button";
        _hundredths8Button.Padding = new Padding(14);
        _hundredths8Button.Size = new Size(74, 63);
        _hundredths8Button.TabIndex = 12;
        _hundredths8Button.Text = ".08";
        _hundredths8Button.UseVisualStyleBackColor = false;
        _hundredths8Button.Click += DenominationButton_Click;
        // 
        // _hundredths7Button
        // 
        _hundredths7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths7Button.AutoSize = true;
        _hundredths7Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths7Button.Location = new Point(488, 226);
        _hundredths7Button.Margin = new Padding(8);
        _hundredths7Button.Name = "_hundredths7Button";
        _hundredths7Button.Padding = new Padding(14);
        _hundredths7Button.Size = new Size(74, 63);
        _hundredths7Button.TabIndex = 18;
        _hundredths7Button.Text = ".07";
        _hundredths7Button.UseVisualStyleBackColor = false;
        _hundredths7Button.Click += DenominationButton_Click;
        // 
        // _hundredths6Button
        // 
        _hundredths6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths6Button.AutoSize = true;
        _hundredths6Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths6Button.Location = new Point(488, 305);
        _hundredths6Button.Margin = new Padding(8);
        _hundredths6Button.Name = "_hundredths6Button";
        _hundredths6Button.Padding = new Padding(14);
        _hundredths6Button.Size = new Size(74, 63);
        _hundredths6Button.TabIndex = 24;
        _hundredths6Button.Text = ".06";
        _hundredths6Button.UseVisualStyleBackColor = false;
        _hundredths6Button.Click += DenominationButton_Click;
        // 
        // _hundredths5Button
        // 
        _hundredths5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths5Button.AutoSize = true;
        _hundredths5Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths5Button.Location = new Point(488, 384);
        _hundredths5Button.Margin = new Padding(8);
        _hundredths5Button.Name = "_hundredths5Button";
        _hundredths5Button.Padding = new Padding(14);
        _hundredths5Button.Size = new Size(74, 63);
        _hundredths5Button.TabIndex = 30;
        _hundredths5Button.Text = ".05";
        _hundredths5Button.UseVisualStyleBackColor = false;
        _hundredths5Button.Click += DenominationButton_Click;
        // 
        // _hundredths4Button
        // 
        _hundredths4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths4Button.AutoSize = true;
        _hundredths4Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths4Button.Location = new Point(488, 463);
        _hundredths4Button.Margin = new Padding(8);
        _hundredths4Button.Name = "_hundredths4Button";
        _hundredths4Button.Padding = new Padding(14);
        _hundredths4Button.Size = new Size(74, 63);
        _hundredths4Button.TabIndex = 36;
        _hundredths4Button.Text = ".04";
        _hundredths4Button.UseVisualStyleBackColor = false;
        _hundredths4Button.Click += DenominationButton_Click;
        // 
        // _hundredths3Button
        // 
        _hundredths3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths3Button.AutoSize = true;
        _hundredths3Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths3Button.Location = new Point(488, 542);
        _hundredths3Button.Margin = new Padding(8);
        _hundredths3Button.Name = "_hundredths3Button";
        _hundredths3Button.Padding = new Padding(14);
        _hundredths3Button.Size = new Size(74, 63);
        _hundredths3Button.TabIndex = 42;
        _hundredths3Button.Text = ".03";
        _hundredths3Button.UseVisualStyleBackColor = false;
        _hundredths3Button.Click += DenominationButton_Click;
        // 
        // _hundredths2Button
        // 
        _hundredths2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths2Button.AutoSize = true;
        _hundredths2Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths2Button.Location = new Point(488, 621);
        _hundredths2Button.Margin = new Padding(8);
        _hundredths2Button.Name = "_hundredths2Button";
        _hundredths2Button.Padding = new Padding(14);
        _hundredths2Button.Size = new Size(74, 63);
        _hundredths2Button.TabIndex = 48;
        _hundredths2Button.Text = ".02";
        _hundredths2Button.UseVisualStyleBackColor = false;
        _hundredths2Button.Click += DenominationButton_Click;
        // 
        // _hundredths1Button
        // 
        _hundredths1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths1Button.AutoSize = true;
        _hundredths1Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _hundredths1Button.Location = new Point(488, 700);
        _hundredths1Button.Margin = new Padding(8);
        _hundredths1Button.Name = "_hundredths1Button";
        _hundredths1Button.Padding = new Padding(14);
        _hundredths1Button.Size = new Size(74, 63);
        _hundredths1Button.TabIndex = 54;
        _hundredths1Button.Text = ".01";
        _hundredths1Button.UseVisualStyleBackColor = false;
        _hundredths1Button.Click += DenominationButton_Click;
        // 
        // _departmentAndActionLayout
        // 
        _departmentAndActionLayout.AutoSize = true;
        _departmentAndActionLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _departmentAndActionLayout.ColumnCount = 1;
        _departmentAndActionLayout.ColumnStyles.Add(new ColumnStyle());
        _departmentAndActionLayout.Controls.Add(_departmentGrid, 0, 0);
        _departmentAndActionLayout.Controls.Add(_actionGrid, 0, 1);
        _departmentAndActionLayout.Dock = DockStyle.Fill;
        _departmentAndActionLayout.Location = new Point(579, 3);
        _departmentAndActionLayout.Name = "_departmentAndActionLayout";
        _departmentAndActionLayout.RowCount = 2;
        _departmentAndActionLayout.RowStyles.Add(new RowStyle());
        _departmentAndActionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _departmentAndActionLayout.Size = new Size(530, 771);
        _departmentAndActionLayout.TabIndex = 1;
        // 
        // _departmentGrid
        // 
        _departmentGrid.AutoSize = true;
        _departmentGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _departmentGrid.ColumnCount = 4;
        _departmentGrid.ColumnStyles.Add(new ColumnStyle());
        _departmentGrid.ColumnStyles.Add(new ColumnStyle());
        _departmentGrid.ColumnStyles.Add(new ColumnStyle());
        _departmentGrid.ColumnStyles.Add(new ColumnStyle());
        _departmentGrid.Controls.Add(_taxButton, 0, 4);
        _departmentGrid.Controls.Add(_department01Button, 0, 0);
        _departmentGrid.Controls.Add(_department02Button, 1, 0);
        _departmentGrid.Controls.Add(_department03Button, 2, 0);
        _departmentGrid.Controls.Add(_department04Button, 3, 0);
        _departmentGrid.Controls.Add(_department05Button, 0, 1);
        _departmentGrid.Controls.Add(_department06Button, 1, 1);
        _departmentGrid.Controls.Add(_department07Button, 2, 1);
        _departmentGrid.Controls.Add(_department08Button, 3, 1);
        _departmentGrid.Controls.Add(_department09Button, 0, 2);
        _departmentGrid.Controls.Add(_department10Button, 1, 2);
        _departmentGrid.Controls.Add(_department11Button, 2, 2);
        _departmentGrid.Controls.Add(_department12Button, 3, 2);
        _departmentGrid.Controls.Add(_department13Button, 0, 3);
        _departmentGrid.Controls.Add(_department14Button, 1, 3);
        _departmentGrid.Controls.Add(_department15Button, 2, 3);
        _departmentGrid.Controls.Add(_department16Button, 3, 3);
        _departmentGrid.Controls.Add(_department17Button, 2, 4);
        _departmentGrid.Controls.Add(_department18Button, 3, 4);
        _departmentGrid.Dock = DockStyle.Fill;
        _departmentGrid.Location = new Point(3, 3);
        _departmentGrid.Name = "_departmentGrid";
        _departmentGrid.RowCount = 5;
        _departmentGrid.RowStyles.Add(new RowStyle());
        _departmentGrid.RowStyles.Add(new RowStyle());
        _departmentGrid.RowStyles.Add(new RowStyle());
        _departmentGrid.RowStyles.Add(new RowStyle());
        _departmentGrid.RowStyles.Add(new RowStyle());
        _departmentGrid.Size = new Size(524, 435);
        _departmentGrid.TabIndex = 0;
        // 
        // _taxButton
        // 
        _taxButton.AutoSize = true;
        _taxButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _departmentGrid.SetColumnSpan(_taxButton, 2);
        _taxButton.Dock = DockStyle.Fill;
        _taxButton.Location = new Point(10, 358);
        _taxButton.Margin = new Padding(10);
        _taxButton.Name = "_taxButton";
        _taxButton.Padding = new Padding(16);
        _taxButton.Size = new Size(242, 67);
        _taxButton.TabIndex = 75;
        _taxButton.Text = "TAX 8.25%";
        _taxButton.UseVisualStyleBackColor = false;
        _taxButton.Click += TaxButton_Click;
        // 
        // _department01Button
        // 
        _department01Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department01Button.AutoSize = true;
        _department01Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department01Button.Location = new Point(10, 10);
        _department01Button.Margin = new Padding(10);
        _department01Button.Name = "_department01Button";
        _department01Button.Padding = new Padding(16);
        _department01Button.Size = new Size(111, 67);
        _department01Button.TabIndex = 55;
        _department01Button.Text = "DEP 01";
        _department01Button.UseVisualStyleBackColor = false;
        _department01Button.Click += DepartmentButton_Click;
        // 
        // _department02Button
        // 
        _department02Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department02Button.AutoSize = true;
        _department02Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department02Button.Location = new Point(141, 10);
        _department02Button.Margin = new Padding(10);
        _department02Button.Name = "_department02Button";
        _department02Button.Padding = new Padding(16);
        _department02Button.Size = new Size(111, 67);
        _department02Button.TabIndex = 56;
        _department02Button.Text = "DEP 02";
        _department02Button.UseVisualStyleBackColor = false;
        _department02Button.Click += DepartmentButton_Click;
        // 
        // _department03Button
        // 
        _department03Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department03Button.AutoSize = true;
        _department03Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department03Button.Location = new Point(272, 10);
        _department03Button.Margin = new Padding(10);
        _department03Button.Name = "_department03Button";
        _department03Button.Padding = new Padding(16);
        _department03Button.Size = new Size(111, 67);
        _department03Button.TabIndex = 57;
        _department03Button.Text = "DEP 03";
        _department03Button.UseVisualStyleBackColor = false;
        _department03Button.Click += DepartmentButton_Click;
        // 
        // _department04Button
        // 
        _department04Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department04Button.AutoSize = true;
        _department04Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department04Button.Location = new Point(403, 10);
        _department04Button.Margin = new Padding(10);
        _department04Button.Name = "_department04Button";
        _department04Button.Padding = new Padding(16);
        _department04Button.Size = new Size(111, 67);
        _department04Button.TabIndex = 58;
        _department04Button.Text = "DEP 04";
        _department04Button.UseVisualStyleBackColor = false;
        _department04Button.Click += DepartmentButton_Click;
        // 
        // _department05Button
        // 
        _department05Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department05Button.AutoSize = true;
        _department05Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department05Button.Location = new Point(10, 97);
        _department05Button.Margin = new Padding(10);
        _department05Button.Name = "_department05Button";
        _department05Button.Padding = new Padding(16);
        _department05Button.Size = new Size(111, 67);
        _department05Button.TabIndex = 59;
        _department05Button.Text = "DEP 05";
        _department05Button.UseVisualStyleBackColor = false;
        _department05Button.Click += DepartmentButton_Click;
        // 
        // _department06Button
        // 
        _department06Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department06Button.AutoSize = true;
        _department06Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department06Button.Location = new Point(141, 97);
        _department06Button.Margin = new Padding(10);
        _department06Button.Name = "_department06Button";
        _department06Button.Padding = new Padding(16);
        _department06Button.Size = new Size(111, 67);
        _department06Button.TabIndex = 60;
        _department06Button.Text = "DEP 06";
        _department06Button.UseVisualStyleBackColor = false;
        _department06Button.Click += DepartmentButton_Click;
        // 
        // _department07Button
        // 
        _department07Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department07Button.AutoSize = true;
        _department07Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department07Button.Location = new Point(272, 97);
        _department07Button.Margin = new Padding(10);
        _department07Button.Name = "_department07Button";
        _department07Button.Padding = new Padding(16);
        _department07Button.Size = new Size(111, 67);
        _department07Button.TabIndex = 61;
        _department07Button.Text = "DEP 07";
        _department07Button.UseVisualStyleBackColor = false;
        _department07Button.Click += DepartmentButton_Click;
        // 
        // _department08Button
        // 
        _department08Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department08Button.AutoSize = true;
        _department08Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department08Button.Location = new Point(403, 97);
        _department08Button.Margin = new Padding(10);
        _department08Button.Name = "_department08Button";
        _department08Button.Padding = new Padding(16);
        _department08Button.Size = new Size(111, 67);
        _department08Button.TabIndex = 62;
        _department08Button.Text = "DEP 08";
        _department08Button.UseVisualStyleBackColor = false;
        _department08Button.Click += DepartmentButton_Click;
        // 
        // _department09Button
        // 
        _department09Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department09Button.AutoSize = true;
        _department09Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department09Button.Location = new Point(10, 184);
        _department09Button.Margin = new Padding(10);
        _department09Button.Name = "_department09Button";
        _department09Button.Padding = new Padding(16);
        _department09Button.Size = new Size(111, 67);
        _department09Button.TabIndex = 63;
        _department09Button.Text = "DEP 09";
        _department09Button.UseVisualStyleBackColor = false;
        _department09Button.Click += DepartmentButton_Click;
        // 
        // _department10Button
        // 
        _department10Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department10Button.AutoSize = true;
        _department10Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department10Button.Location = new Point(141, 184);
        _department10Button.Margin = new Padding(10);
        _department10Button.Name = "_department10Button";
        _department10Button.Padding = new Padding(16);
        _department10Button.Size = new Size(111, 67);
        _department10Button.TabIndex = 64;
        _department10Button.Text = "DEP 10";
        _department10Button.UseVisualStyleBackColor = false;
        _department10Button.Click += DepartmentButton_Click;
        // 
        // _department11Button
        // 
        _department11Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department11Button.AutoSize = true;
        _department11Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department11Button.Location = new Point(272, 184);
        _department11Button.Margin = new Padding(10);
        _department11Button.Name = "_department11Button";
        _department11Button.Padding = new Padding(16);
        _department11Button.Size = new Size(111, 67);
        _department11Button.TabIndex = 65;
        _department11Button.Text = "DEP 11";
        _department11Button.UseVisualStyleBackColor = false;
        _department11Button.Click += DepartmentButton_Click;
        // 
        // _department12Button
        // 
        _department12Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department12Button.AutoSize = true;
        _department12Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department12Button.Location = new Point(403, 184);
        _department12Button.Margin = new Padding(10);
        _department12Button.Name = "_department12Button";
        _department12Button.Padding = new Padding(16);
        _department12Button.Size = new Size(111, 67);
        _department12Button.TabIndex = 66;
        _department12Button.Text = "DEP 12";
        _department12Button.UseVisualStyleBackColor = false;
        _department12Button.Click += DepartmentButton_Click;
        // 
        // _department13Button
        // 
        _department13Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department13Button.AutoSize = true;
        _department13Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department13Button.Location = new Point(10, 271);
        _department13Button.Margin = new Padding(10);
        _department13Button.Name = "_department13Button";
        _department13Button.Padding = new Padding(16);
        _department13Button.Size = new Size(111, 67);
        _department13Button.TabIndex = 67;
        _department13Button.Text = "DEP 13";
        _department13Button.UseVisualStyleBackColor = false;
        _department13Button.Click += DepartmentButton_Click;
        // 
        // _department14Button
        // 
        _department14Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department14Button.AutoSize = true;
        _department14Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department14Button.Location = new Point(141, 271);
        _department14Button.Margin = new Padding(10);
        _department14Button.Name = "_department14Button";
        _department14Button.Padding = new Padding(16);
        _department14Button.Size = new Size(111, 67);
        _department14Button.TabIndex = 68;
        _department14Button.Text = "DEP 14";
        _department14Button.UseVisualStyleBackColor = false;
        _department14Button.Click += DepartmentButton_Click;
        // 
        // _department15Button
        // 
        _department15Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department15Button.AutoSize = true;
        _department15Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department15Button.Location = new Point(272, 271);
        _department15Button.Margin = new Padding(10);
        _department15Button.Name = "_department15Button";
        _department15Button.Padding = new Padding(16);
        _department15Button.Size = new Size(111, 67);
        _department15Button.TabIndex = 69;
        _department15Button.Text = "DEP 15";
        _department15Button.UseVisualStyleBackColor = false;
        _department15Button.Click += DepartmentButton_Click;
        // 
        // _department16Button
        // 
        _department16Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department16Button.AutoSize = true;
        _department16Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department16Button.Location = new Point(403, 271);
        _department16Button.Margin = new Padding(10);
        _department16Button.Name = "_department16Button";
        _department16Button.Padding = new Padding(16);
        _department16Button.Size = new Size(111, 67);
        _department16Button.TabIndex = 70;
        _department16Button.Text = "DEP 16";
        _department16Button.UseVisualStyleBackColor = false;
        _department16Button.Click += DepartmentButton_Click;
        // 
        // _department17Button
        // 
        _department17Button.AutoSize = true;
        _department17Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department17Button.Dock = DockStyle.Fill;
        _department17Button.Location = new Point(272, 358);
        _department17Button.Margin = new Padding(10);
        _department17Button.Name = "_department17Button";
        _department17Button.Padding = new Padding(16);
        _department17Button.Size = new Size(111, 67);
        _department17Button.TabIndex = 73;
        _department17Button.Text = "DEP 17";
        _department17Button.UseVisualStyleBackColor = false;
        _department17Button.Click += DepartmentButton_Click;
        // 
        // _department18Button
        // 
        _department18Button.AutoSize = true;
        _department18Button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _department18Button.Dock = DockStyle.Fill;
        _department18Button.Location = new Point(403, 358);
        _department18Button.Margin = new Padding(10);
        _department18Button.Name = "_department18Button";
        _department18Button.Padding = new Padding(16);
        _department18Button.Size = new Size(111, 67);
        _department18Button.TabIndex = 74;
        _department18Button.Text = "DEP 18";
        _department18Button.UseVisualStyleBackColor = false;
        _department18Button.Click += DepartmentButton_Click;
        // 
        // _actionGrid
        // 
        _actionGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _actionGrid.ColumnCount = 2;
        _actionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        _actionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.6666641F));
        _actionGrid.Controls.Add(_subtotalButton, 0, 1);
        _actionGrid.Controls.Add(_voidButton, 0, 0);
        _actionGrid.Controls.Add(_totalButton, 1, 0);
        _actionGrid.Location = new Point(3, 444);
        _actionGrid.Name = "_actionGrid";
        _actionGrid.RowCount = 2;
        _actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _actionGrid.Size = new Size(524, 324);
        _actionGrid.TabIndex = 1;
        // 
        // _subtotalButton
        // 
        _subtotalButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _subtotalButton.AutoSize = true;
        _subtotalButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _subtotalButton.Location = new Point(10, 172);
        _subtotalButton.Margin = new Padding(10);
        _subtotalButton.Name = "_subtotalButton";
        _subtotalButton.Size = new Size(154, 142);
        _subtotalButton.TabIndex = 77;
        _subtotalButton.Text = "SUBTOTAL";
        _subtotalButton.UseVisualStyleBackColor = false;
        _subtotalButton.Click += SubtotalButton_Click;
        // 
        // _voidButton
        // 
        _voidButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _voidButton.AutoSize = true;
        _voidButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _voidButton.Location = new Point(10, 10);
        _voidButton.Margin = new Padding(10);
        _voidButton.Name = "_voidButton";
        _voidButton.Size = new Size(154, 142);
        _voidButton.TabIndex = 76;
        _voidButton.Text = "VOID";
        _voidButton.UseVisualStyleBackColor = false;
        _voidButton.Click += VoidButton_Click;
        // 
        // _totalButton
        // 
        _totalButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _totalButton.AutoSize = true;
        _totalButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _totalButton.Location = new Point(184, 10);
        _totalButton.Margin = new Padding(10);
        _totalButton.Name = "_totalButton";
        _actionGrid.SetRowSpan(_totalButton, 2);
        _totalButton.Size = new Size(330, 304);
        _totalButton.TabIndex = 78;
        _totalButton.Text = "TOTAL";
        _totalButton.UseVisualStyleBackColor = false;
        _totalButton.Click += TotalButton_Click;
        // 
        // _receiptGroupBox
        // 
        _receiptGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _receiptGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _receiptGroupBox.Controls.Add(_receiptTextBox);
        _receiptGroupBox.Location = new Point(1135, 11);
        _receiptGroupBox.Name = "_receiptGroupBox";
        _receiptGroupBox.Padding = new Padding(10);
        _receiptGroupBox.Size = new Size(371, 955);
        _receiptGroupBox.TabIndex = 1;
        _receiptGroupBox.TabStop = false;
        _receiptGroupBox.Text = "Receipt printer";
        // 
        // _receiptTextBox
        // 
        _receiptTextBox.AccessibleName = "Printed receipt";
        _receiptTextBox.BackColor = Color.FromArgb(255, 253, 240);
        _receiptTextBox.BorderStyle = BorderStyle.None;
        _receiptTextBox.DetectUrls = false;
        _receiptTextBox.Dock = DockStyle.Fill;
        _receiptTextBox.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        _receiptTextBox.ForeColor = Color.FromArgb(28, 28, 24);
        _receiptTextBox.Location = new Point(10, 34);
        _receiptTextBox.Name = "_receiptTextBox";
        _receiptTextBox.Padding = new Padding(14);
        _receiptTextBox.ReadOnly = true;
        _receiptTextBox.ScrollBars = RichTextBoxScrollBars.None;
        _receiptTextBox.Size = new Size(351, 911);
        _receiptTextBox.TabIndex = 0;
        _receiptTextBox.Text = "";
        _receiptTextBox.WordWrap = false;
        // 
        // CashRegisterView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoScroll = true;
        Controls.Add(_rootLayout);
        Name = "CashRegisterView";
        Size = new Size(1520, 981);
        _rootLayout.ResumeLayout(false);
        _rootLayout.PerformLayout();
        _registerLayout.ResumeLayout(false);
        _registerLayout.PerformLayout();
        _tlp7SegmentContainer.ResumeLayout(false);
        _tlp7SegmentContainer.PerformLayout();
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        _keyBodyLayout.ResumeLayout(false);
        _keyBodyLayout.PerformLayout();
        _denominationGrid.ResumeLayout(false);
        _denominationGrid.PerformLayout();
        _departmentAndActionLayout.ResumeLayout(false);
        _departmentAndActionLayout.PerformLayout();
        _departmentGrid.ResumeLayout(false);
        _departmentGrid.PerformLayout();
        _actionGrid.ResumeLayout(false);
        _actionGrid.PerformLayout();
        _receiptGroupBox.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel _rootLayout;
    private TableLayoutPanel _registerLayout;
    private TableLayoutPanel _keyBodyLayout;
    private TableLayoutPanel _denominationGrid;
    private Label _thousandsHeaderLabel;
    private Label _hundredsHeaderLabel;
    private Label _tensHeaderLabel;
    private Label _onesHeaderLabel;
    private Label _tenthsHeaderLabel;
    private Label _hundredthsHeaderLabel;
    private TableLayoutPanel _departmentAndActionLayout;
    private TableLayoutPanel _departmentGrid;
    private TableLayoutPanel _actionGrid;
    private GroupBox _receiptGroupBox;
    private RichTextBox _receiptTextBox;
    private CashRegisterKeyButton _thousands9Button;
    private CashRegisterKeyButton _thousands8Button;
    private CashRegisterKeyButton _thousands7Button;
    private CashRegisterKeyButton _thousands6Button;
    private CashRegisterKeyButton _thousands5Button;
    private CashRegisterKeyButton _thousands4Button;
    private CashRegisterKeyButton _thousands3Button;
    private CashRegisterKeyButton _thousands2Button;
    private CashRegisterKeyButton _thousands1Button;
    private CashRegisterKeyButton _hundreds9Button;
    private CashRegisterKeyButton _hundreds8Button;
    private CashRegisterKeyButton _hundreds7Button;
    private CashRegisterKeyButton _hundreds6Button;
    private CashRegisterKeyButton _hundreds5Button;
    private CashRegisterKeyButton _hundreds4Button;
    private CashRegisterKeyButton _hundreds3Button;
    private CashRegisterKeyButton _hundreds2Button;
    private CashRegisterKeyButton _hundreds1Button;
    private CashRegisterKeyButton _tens9Button;
    private CashRegisterKeyButton _tens8Button;
    private CashRegisterKeyButton _tens7Button;
    private CashRegisterKeyButton _tens6Button;
    private CashRegisterKeyButton _tens5Button;
    private CashRegisterKeyButton _tens4Button;
    private CashRegisterKeyButton _tens3Button;
    private CashRegisterKeyButton _tens2Button;
    private CashRegisterKeyButton _tens1Button;
    private CashRegisterKeyButton _ones9Button;
    private CashRegisterKeyButton _ones8Button;
    private CashRegisterKeyButton _ones7Button;
    private CashRegisterKeyButton _ones6Button;
    private CashRegisterKeyButton _ones5Button;
    private CashRegisterKeyButton _ones4Button;
    private CashRegisterKeyButton _ones3Button;
    private CashRegisterKeyButton _ones2Button;
    private CashRegisterKeyButton _ones1Button;
    private CashRegisterKeyButton _tenths9Button;
    private CashRegisterKeyButton _tenths8Button;
    private CashRegisterKeyButton _tenths7Button;
    private CashRegisterKeyButton _tenths6Button;
    private CashRegisterKeyButton _tenths5Button;
    private CashRegisterKeyButton _tenths4Button;
    private CashRegisterKeyButton _tenths3Button;
    private CashRegisterKeyButton _tenths2Button;
    private CashRegisterKeyButton _tenths1Button;
    private CashRegisterKeyButton _hundredths9Button;
    private CashRegisterKeyButton _hundredths8Button;
    private CashRegisterKeyButton _hundredths7Button;
    private CashRegisterKeyButton _hundredths6Button;
    private CashRegisterKeyButton _hundredths5Button;
    private CashRegisterKeyButton _hundredths4Button;
    private CashRegisterKeyButton _hundredths3Button;
    private CashRegisterKeyButton _hundredths2Button;
    private CashRegisterKeyButton _hundredths1Button;
    private CashRegisterKeyButton _department01Button;
    private CashRegisterKeyButton _department02Button;
    private CashRegisterKeyButton _department03Button;
    private CashRegisterKeyButton _department04Button;
    private CashRegisterKeyButton _department05Button;
    private CashRegisterKeyButton _department06Button;
    private CashRegisterKeyButton _department07Button;
    private CashRegisterKeyButton _department08Button;
    private CashRegisterKeyButton _department09Button;
    private CashRegisterKeyButton _department10Button;
    private CashRegisterKeyButton _department11Button;
    private CashRegisterKeyButton _department12Button;
    private CashRegisterKeyButton _department13Button;
    private CashRegisterKeyButton _department14Button;
    private CashRegisterKeyButton _department15Button;
    private CashRegisterKeyButton _department16Button;
    private CashRegisterKeyButton _department17Button;
    private CashRegisterKeyButton _department18Button;
    private CashRegisterKeyButton _taxButton;
    private CashRegisterKeyButton _voidButton;
    private CashRegisterKeyButton _subtotalButton;
    private CashRegisterKeyButton _totalButton;
    private TableLayoutPanel _tlp7SegmentContainer;
    private SevenSegmentDisplay _display;
    private TableLayoutPanel tableLayoutPanel1;
    private CheckBox checkBox1;
    private CheckBox checkBox2;
}
