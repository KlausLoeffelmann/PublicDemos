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
        _display = new SevenSegmentDisplay();
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
        _department19Button = new CashRegisterKeyButton();
        _department20Button = new CashRegisterKeyButton();
        _actionGrid = new TableLayoutPanel();
        _taxButton = new CashRegisterKeyButton();
        _voidButton = new CashRegisterKeyButton();
        _subtotalButton = new CashRegisterKeyButton();
        _totalButton = new CashRegisterKeyButton();
        _receiptGroupBox = new GroupBox();
        _receiptTextBox = new RichTextBox();
        _rootLayout.SuspendLayout();
        _registerLayout.SuspendLayout();
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
        _rootLayout.ColumnCount = 2;
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        _rootLayout.Controls.Add(_registerLayout, 0, 0);
        _rootLayout.Controls.Add(_receiptGroupBox, 1, 0);
        _rootLayout.Location = new Point(0, 0);
        _rootLayout.Name = "_rootLayout";
        _rootLayout.Padding = new Padding(8);
        _rootLayout.RowCount = 1;
        _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootLayout.Size = new Size(1521, 1106);
        _rootLayout.TabIndex = 0;
        // 
        // _registerLayout
        // 
        _registerLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _registerLayout.ColumnCount = 1;
        _registerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _registerLayout.Controls.Add(_display, 0, 0);
        _registerLayout.Controls.Add(_keyBodyLayout, 0, 1);
        _registerLayout.Location = new Point(11, 11);
        _registerLayout.Name = "_registerLayout";
        _registerLayout.RowCount = 2;
        _registerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
        _registerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _registerLayout.Size = new Size(1047, 1084);
        _registerLayout.TabIndex = 0;
        // 
        // _display
        // 
        _display.AccessibleName = "Cash register amount display";
        _display.AccessibleRole = AccessibleRole.StaticText;
        _display.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _display.BackColor = Color.FromArgb(18, 24, 22);
        _display.ForeColor = Color.FromArgb(255, 118, 35);
        _display.Location = new Point(6, 6);
        _display.Margin = new Padding(6);
        _display.Name = "_display";
        _display.Size = new Size(1035, 188);
        _display.TabIndex = 0;
        _display.TabStop = false;
        // 
        // _keyBodyLayout
        // 
        _keyBodyLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _keyBodyLayout.ColumnCount = 2;
        _keyBodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
        _keyBodyLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
        _keyBodyLayout.Controls.Add(_denominationGrid, 0, 0);
        _keyBodyLayout.Controls.Add(_departmentAndActionLayout, 1, 0);
        _keyBodyLayout.Location = new Point(3, 203);
        _keyBodyLayout.Name = "_keyBodyLayout";
        _keyBodyLayout.RowCount = 1;
        _keyBodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _keyBodyLayout.Size = new Size(1041, 878);
        _keyBodyLayout.TabIndex = 1;
        // 
        // _denominationGrid
        // 
        _denominationGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _denominationGrid.ColumnCount = 6;
        _denominationGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        _denominationGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        _denominationGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        _denominationGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        _denominationGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
        _denominationGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
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
        _denominationGrid.Location = new Point(3, 3);
        _denominationGrid.MinimumSize = new Size(510, 620);
        _denominationGrid.Name = "_denominationGrid";
        _denominationGrid.RowCount = 10;
        _denominationGrid.RowStyles.Add(new RowStyle());
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.11111F));
        _denominationGrid.Size = new Size(639, 872);
        _denominationGrid.TabIndex = 0;
        // 
        // _thousandsHeaderLabel
        // 
        _thousandsHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _thousandsHeaderLabel.AutoSize = true;
        _thousandsHeaderLabel.Location = new Point(3, 0);
        _thousandsHeaderLabel.Name = "_thousandsHeaderLabel";
        _thousandsHeaderLabel.Size = new Size(100, 30);
        _thousandsHeaderLabel.TabIndex = 0;
        _thousandsHeaderLabel.Text = "THOU";
        _thousandsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _hundredsHeaderLabel
        // 
        _hundredsHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _hundredsHeaderLabel.AutoSize = true;
        _hundredsHeaderLabel.Location = new Point(109, 0);
        _hundredsHeaderLabel.Name = "_hundredsHeaderLabel";
        _hundredsHeaderLabel.Size = new Size(100, 30);
        _hundredsHeaderLabel.TabIndex = 0;
        _hundredsHeaderLabel.Text = "HUN";
        _hundredsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _tensHeaderLabel
        // 
        _tensHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _tensHeaderLabel.AutoSize = true;
        _tensHeaderLabel.Location = new Point(215, 0);
        _tensHeaderLabel.Name = "_tensHeaderLabel";
        _tensHeaderLabel.Size = new Size(100, 30);
        _tensHeaderLabel.TabIndex = 0;
        _tensHeaderLabel.Text = "TENS";
        _tensHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _onesHeaderLabel
        // 
        _onesHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _onesHeaderLabel.AutoSize = true;
        _onesHeaderLabel.Location = new Point(321, 0);
        _onesHeaderLabel.Name = "_onesHeaderLabel";
        _onesHeaderLabel.Size = new Size(100, 30);
        _onesHeaderLabel.TabIndex = 0;
        _onesHeaderLabel.Text = "ONES";
        _onesHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _tenthsHeaderLabel
        // 
        _tenthsHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _tenthsHeaderLabel.AutoSize = true;
        _tenthsHeaderLabel.Location = new Point(427, 0);
        _tenthsHeaderLabel.Name = "_tenthsHeaderLabel";
        _tenthsHeaderLabel.Size = new Size(100, 30);
        _tenthsHeaderLabel.TabIndex = 0;
        _tenthsHeaderLabel.Text = "10c";
        _tenthsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _hundredthsHeaderLabel
        // 
        _hundredthsHeaderLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _hundredthsHeaderLabel.AutoSize = true;
        _hundredthsHeaderLabel.Location = new Point(533, 0);
        _hundredthsHeaderLabel.Name = "_hundredthsHeaderLabel";
        _hundredthsHeaderLabel.Size = new Size(103, 30);
        _hundredthsHeaderLabel.TabIndex = 0;
        _hundredthsHeaderLabel.Text = "1c";
        _hundredthsHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _thousands9Button
        // 
        _thousands9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands9Button.Location = new Point(3, 33);
        _thousands9Button.Name = "_thousands9Button";
        _thousands9Button.Size = new Size(100, 87);
        _thousands9Button.TabIndex = 1;
        _thousands9Button.Text = "9K";
        _thousands9Button.UseVisualStyleBackColor = false;
        _thousands9Button.Click += DenominationButton_Click;
        // 
        // _thousands8Button
        // 
        _thousands8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands8Button.Location = new Point(3, 126);
        _thousands8Button.Name = "_thousands8Button";
        _thousands8Button.Size = new Size(100, 87);
        _thousands8Button.TabIndex = 7;
        _thousands8Button.Text = "8K";
        _thousands8Button.UseVisualStyleBackColor = false;
        _thousands8Button.Click += DenominationButton_Click;
        // 
        // _thousands7Button
        // 
        _thousands7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands7Button.Location = new Point(3, 219);
        _thousands7Button.Name = "_thousands7Button";
        _thousands7Button.Size = new Size(100, 87);
        _thousands7Button.TabIndex = 13;
        _thousands7Button.Text = "7K";
        _thousands7Button.UseVisualStyleBackColor = false;
        _thousands7Button.Click += DenominationButton_Click;
        // 
        // _thousands6Button
        // 
        _thousands6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands6Button.Location = new Point(3, 312);
        _thousands6Button.Name = "_thousands6Button";
        _thousands6Button.Size = new Size(100, 87);
        _thousands6Button.TabIndex = 19;
        _thousands6Button.Text = "6K";
        _thousands6Button.UseVisualStyleBackColor = false;
        _thousands6Button.Click += DenominationButton_Click;
        // 
        // _thousands5Button
        // 
        _thousands5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands5Button.Location = new Point(3, 405);
        _thousands5Button.Name = "_thousands5Button";
        _thousands5Button.Size = new Size(100, 87);
        _thousands5Button.TabIndex = 25;
        _thousands5Button.Text = "5K";
        _thousands5Button.UseVisualStyleBackColor = false;
        _thousands5Button.Click += DenominationButton_Click;
        // 
        // _thousands4Button
        // 
        _thousands4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands4Button.Location = new Point(3, 498);
        _thousands4Button.Name = "_thousands4Button";
        _thousands4Button.Size = new Size(100, 87);
        _thousands4Button.TabIndex = 31;
        _thousands4Button.Text = "4K";
        _thousands4Button.UseVisualStyleBackColor = false;
        _thousands4Button.Click += DenominationButton_Click;
        // 
        // _thousands3Button
        // 
        _thousands3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands3Button.Location = new Point(3, 591);
        _thousands3Button.Name = "_thousands3Button";
        _thousands3Button.Size = new Size(100, 87);
        _thousands3Button.TabIndex = 37;
        _thousands3Button.Text = "3K";
        _thousands3Button.UseVisualStyleBackColor = false;
        _thousands3Button.Click += DenominationButton_Click;
        // 
        // _thousands2Button
        // 
        _thousands2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands2Button.Location = new Point(3, 684);
        _thousands2Button.Name = "_thousands2Button";
        _thousands2Button.Size = new Size(100, 87);
        _thousands2Button.TabIndex = 43;
        _thousands2Button.Text = "2K";
        _thousands2Button.UseVisualStyleBackColor = false;
        _thousands2Button.Click += DenominationButton_Click;
        // 
        // _thousands1Button
        // 
        _thousands1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _thousands1Button.Location = new Point(3, 777);
        _thousands1Button.Name = "_thousands1Button";
        _thousands1Button.Size = new Size(100, 92);
        _thousands1Button.TabIndex = 49;
        _thousands1Button.Text = "1K";
        _thousands1Button.UseVisualStyleBackColor = false;
        _thousands1Button.Click += DenominationButton_Click;
        // 
        // _hundreds9Button
        // 
        _hundreds9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds9Button.Location = new Point(109, 33);
        _hundreds9Button.Name = "_hundreds9Button";
        _hundreds9Button.Size = new Size(100, 87);
        _hundreds9Button.TabIndex = 2;
        _hundreds9Button.Text = "900";
        _hundreds9Button.UseVisualStyleBackColor = false;
        _hundreds9Button.Click += DenominationButton_Click;
        // 
        // _hundreds8Button
        // 
        _hundreds8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds8Button.Location = new Point(109, 126);
        _hundreds8Button.Name = "_hundreds8Button";
        _hundreds8Button.Size = new Size(100, 87);
        _hundreds8Button.TabIndex = 8;
        _hundreds8Button.Text = "800";
        _hundreds8Button.UseVisualStyleBackColor = false;
        _hundreds8Button.Click += DenominationButton_Click;
        // 
        // _hundreds7Button
        // 
        _hundreds7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds7Button.Location = new Point(109, 219);
        _hundreds7Button.Name = "_hundreds7Button";
        _hundreds7Button.Size = new Size(100, 87);
        _hundreds7Button.TabIndex = 14;
        _hundreds7Button.Text = "700";
        _hundreds7Button.UseVisualStyleBackColor = false;
        _hundreds7Button.Click += DenominationButton_Click;
        // 
        // _hundreds6Button
        // 
        _hundreds6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds6Button.Location = new Point(109, 312);
        _hundreds6Button.Name = "_hundreds6Button";
        _hundreds6Button.Size = new Size(100, 87);
        _hundreds6Button.TabIndex = 20;
        _hundreds6Button.Text = "600";
        _hundreds6Button.UseVisualStyleBackColor = false;
        _hundreds6Button.Click += DenominationButton_Click;
        // 
        // _hundreds5Button
        // 
        _hundreds5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds5Button.Location = new Point(109, 405);
        _hundreds5Button.Name = "_hundreds5Button";
        _hundreds5Button.Size = new Size(100, 87);
        _hundreds5Button.TabIndex = 26;
        _hundreds5Button.Text = "500";
        _hundreds5Button.UseVisualStyleBackColor = false;
        _hundreds5Button.Click += DenominationButton_Click;
        // 
        // _hundreds4Button
        // 
        _hundreds4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds4Button.Location = new Point(109, 498);
        _hundreds4Button.Name = "_hundreds4Button";
        _hundreds4Button.Size = new Size(100, 87);
        _hundreds4Button.TabIndex = 32;
        _hundreds4Button.Text = "400";
        _hundreds4Button.UseVisualStyleBackColor = false;
        _hundreds4Button.Click += DenominationButton_Click;
        // 
        // _hundreds3Button
        // 
        _hundreds3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds3Button.Location = new Point(109, 591);
        _hundreds3Button.Name = "_hundreds3Button";
        _hundreds3Button.Size = new Size(100, 87);
        _hundreds3Button.TabIndex = 38;
        _hundreds3Button.Text = "300";
        _hundreds3Button.UseVisualStyleBackColor = false;
        _hundreds3Button.Click += DenominationButton_Click;
        // 
        // _hundreds2Button
        // 
        _hundreds2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds2Button.Location = new Point(109, 684);
        _hundreds2Button.Name = "_hundreds2Button";
        _hundreds2Button.Size = new Size(100, 87);
        _hundreds2Button.TabIndex = 44;
        _hundreds2Button.Text = "200";
        _hundreds2Button.UseVisualStyleBackColor = false;
        _hundreds2Button.Click += DenominationButton_Click;
        // 
        // _hundreds1Button
        // 
        _hundreds1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundreds1Button.Location = new Point(109, 777);
        _hundreds1Button.Name = "_hundreds1Button";
        _hundreds1Button.Size = new Size(100, 92);
        _hundreds1Button.TabIndex = 50;
        _hundreds1Button.Text = "100";
        _hundreds1Button.UseVisualStyleBackColor = false;
        _hundreds1Button.Click += DenominationButton_Click;
        // 
        // _tens9Button
        // 
        _tens9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens9Button.Location = new Point(215, 33);
        _tens9Button.Name = "_tens9Button";
        _tens9Button.Size = new Size(100, 87);
        _tens9Button.TabIndex = 3;
        _tens9Button.Text = "$90";
        _tens9Button.UseVisualStyleBackColor = false;
        _tens9Button.Click += DenominationButton_Click;
        // 
        // _tens8Button
        // 
        _tens8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens8Button.Location = new Point(215, 126);
        _tens8Button.Name = "_tens8Button";
        _tens8Button.Size = new Size(100, 87);
        _tens8Button.TabIndex = 9;
        _tens8Button.Text = "$80";
        _tens8Button.UseVisualStyleBackColor = false;
        _tens8Button.Click += DenominationButton_Click;
        // 
        // _tens7Button
        // 
        _tens7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens7Button.Location = new Point(215, 219);
        _tens7Button.Name = "_tens7Button";
        _tens7Button.Size = new Size(100, 87);
        _tens7Button.TabIndex = 15;
        _tens7Button.Text = "$70";
        _tens7Button.UseVisualStyleBackColor = false;
        _tens7Button.Click += DenominationButton_Click;
        // 
        // _tens6Button
        // 
        _tens6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens6Button.Location = new Point(215, 312);
        _tens6Button.Name = "_tens6Button";
        _tens6Button.Size = new Size(100, 87);
        _tens6Button.TabIndex = 21;
        _tens6Button.Text = "$60";
        _tens6Button.UseVisualStyleBackColor = false;
        _tens6Button.Click += DenominationButton_Click;
        // 
        // _tens5Button
        // 
        _tens5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens5Button.Location = new Point(215, 405);
        _tens5Button.Name = "_tens5Button";
        _tens5Button.Size = new Size(100, 87);
        _tens5Button.TabIndex = 27;
        _tens5Button.Text = "$50";
        _tens5Button.UseVisualStyleBackColor = false;
        _tens5Button.Click += DenominationButton_Click;
        // 
        // _tens4Button
        // 
        _tens4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens4Button.Location = new Point(215, 498);
        _tens4Button.Name = "_tens4Button";
        _tens4Button.Size = new Size(100, 87);
        _tens4Button.TabIndex = 33;
        _tens4Button.Text = "$40";
        _tens4Button.UseVisualStyleBackColor = false;
        _tens4Button.Click += DenominationButton_Click;
        // 
        // _tens3Button
        // 
        _tens3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens3Button.Location = new Point(215, 591);
        _tens3Button.Name = "_tens3Button";
        _tens3Button.Size = new Size(100, 87);
        _tens3Button.TabIndex = 39;
        _tens3Button.Text = "$30";
        _tens3Button.UseVisualStyleBackColor = false;
        _tens3Button.Click += DenominationButton_Click;
        // 
        // _tens2Button
        // 
        _tens2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens2Button.Location = new Point(215, 684);
        _tens2Button.Name = "_tens2Button";
        _tens2Button.Size = new Size(100, 87);
        _tens2Button.TabIndex = 45;
        _tens2Button.Text = "$20";
        _tens2Button.UseVisualStyleBackColor = false;
        _tens2Button.Click += DenominationButton_Click;
        // 
        // _tens1Button
        // 
        _tens1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tens1Button.Location = new Point(215, 777);
        _tens1Button.Name = "_tens1Button";
        _tens1Button.Size = new Size(100, 92);
        _tens1Button.TabIndex = 51;
        _tens1Button.Text = "$10";
        _tens1Button.UseVisualStyleBackColor = false;
        _tens1Button.Click += DenominationButton_Click;
        // 
        // _ones9Button
        // 
        _ones9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones9Button.Location = new Point(321, 33);
        _ones9Button.Name = "_ones9Button";
        _ones9Button.Size = new Size(100, 87);
        _ones9Button.TabIndex = 4;
        _ones9Button.Text = "$9";
        _ones9Button.UseVisualStyleBackColor = false;
        _ones9Button.Click += DenominationButton_Click;
        // 
        // _ones8Button
        // 
        _ones8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones8Button.Location = new Point(321, 126);
        _ones8Button.Name = "_ones8Button";
        _ones8Button.Size = new Size(100, 87);
        _ones8Button.TabIndex = 10;
        _ones8Button.Text = "$8";
        _ones8Button.UseVisualStyleBackColor = false;
        _ones8Button.Click += DenominationButton_Click;
        // 
        // _ones7Button
        // 
        _ones7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones7Button.Location = new Point(321, 219);
        _ones7Button.Name = "_ones7Button";
        _ones7Button.Size = new Size(100, 87);
        _ones7Button.TabIndex = 16;
        _ones7Button.Text = "$7";
        _ones7Button.UseVisualStyleBackColor = false;
        _ones7Button.Click += DenominationButton_Click;
        // 
        // _ones6Button
        // 
        _ones6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones6Button.Location = new Point(321, 312);
        _ones6Button.Name = "_ones6Button";
        _ones6Button.Size = new Size(100, 87);
        _ones6Button.TabIndex = 22;
        _ones6Button.Text = "$6";
        _ones6Button.UseVisualStyleBackColor = false;
        _ones6Button.Click += DenominationButton_Click;
        // 
        // _ones5Button
        // 
        _ones5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones5Button.Location = new Point(321, 405);
        _ones5Button.Name = "_ones5Button";
        _ones5Button.Size = new Size(100, 87);
        _ones5Button.TabIndex = 28;
        _ones5Button.Text = "$5";
        _ones5Button.UseVisualStyleBackColor = false;
        _ones5Button.Click += DenominationButton_Click;
        // 
        // _ones4Button
        // 
        _ones4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones4Button.Location = new Point(321, 498);
        _ones4Button.Name = "_ones4Button";
        _ones4Button.Size = new Size(100, 87);
        _ones4Button.TabIndex = 34;
        _ones4Button.Text = "$4";
        _ones4Button.UseVisualStyleBackColor = false;
        _ones4Button.Click += DenominationButton_Click;
        // 
        // _ones3Button
        // 
        _ones3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones3Button.Location = new Point(321, 591);
        _ones3Button.Name = "_ones3Button";
        _ones3Button.Size = new Size(100, 87);
        _ones3Button.TabIndex = 40;
        _ones3Button.Text = "$3";
        _ones3Button.UseVisualStyleBackColor = false;
        _ones3Button.Click += DenominationButton_Click;
        // 
        // _ones2Button
        // 
        _ones2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones2Button.Location = new Point(321, 684);
        _ones2Button.Name = "_ones2Button";
        _ones2Button.Size = new Size(100, 87);
        _ones2Button.TabIndex = 46;
        _ones2Button.Text = "$2";
        _ones2Button.UseVisualStyleBackColor = false;
        _ones2Button.Click += DenominationButton_Click;
        // 
        // _ones1Button
        // 
        _ones1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _ones1Button.Location = new Point(321, 777);
        _ones1Button.Name = "_ones1Button";
        _ones1Button.Size = new Size(100, 92);
        _ones1Button.TabIndex = 52;
        _ones1Button.Text = "$1";
        _ones1Button.UseVisualStyleBackColor = false;
        _ones1Button.Click += DenominationButton_Click;
        // 
        // _tenths9Button
        // 
        _tenths9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths9Button.Location = new Point(427, 33);
        _tenths9Button.Name = "_tenths9Button";
        _tenths9Button.Size = new Size(100, 87);
        _tenths9Button.TabIndex = 5;
        _tenths9Button.Text = ".90";
        _tenths9Button.UseVisualStyleBackColor = false;
        _tenths9Button.Click += DenominationButton_Click;
        // 
        // _tenths8Button
        // 
        _tenths8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths8Button.Location = new Point(427, 126);
        _tenths8Button.Name = "_tenths8Button";
        _tenths8Button.Size = new Size(100, 87);
        _tenths8Button.TabIndex = 11;
        _tenths8Button.Text = ".80";
        _tenths8Button.UseVisualStyleBackColor = false;
        _tenths8Button.Click += DenominationButton_Click;
        // 
        // _tenths7Button
        // 
        _tenths7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths7Button.Location = new Point(427, 219);
        _tenths7Button.Name = "_tenths7Button";
        _tenths7Button.Size = new Size(100, 87);
        _tenths7Button.TabIndex = 17;
        _tenths7Button.Text = ".70";
        _tenths7Button.UseVisualStyleBackColor = false;
        _tenths7Button.Click += DenominationButton_Click;
        // 
        // _tenths6Button
        // 
        _tenths6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths6Button.Location = new Point(427, 312);
        _tenths6Button.Name = "_tenths6Button";
        _tenths6Button.Size = new Size(100, 87);
        _tenths6Button.TabIndex = 23;
        _tenths6Button.Text = ".60";
        _tenths6Button.UseVisualStyleBackColor = false;
        _tenths6Button.Click += DenominationButton_Click;
        // 
        // _tenths5Button
        // 
        _tenths5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths5Button.Location = new Point(427, 405);
        _tenths5Button.Name = "_tenths5Button";
        _tenths5Button.Size = new Size(100, 87);
        _tenths5Button.TabIndex = 29;
        _tenths5Button.Text = ".50";
        _tenths5Button.UseVisualStyleBackColor = false;
        _tenths5Button.Click += DenominationButton_Click;
        // 
        // _tenths4Button
        // 
        _tenths4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths4Button.Location = new Point(427, 498);
        _tenths4Button.Name = "_tenths4Button";
        _tenths4Button.Size = new Size(100, 87);
        _tenths4Button.TabIndex = 35;
        _tenths4Button.Text = ".40";
        _tenths4Button.UseVisualStyleBackColor = false;
        _tenths4Button.Click += DenominationButton_Click;
        // 
        // _tenths3Button
        // 
        _tenths3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths3Button.Location = new Point(427, 591);
        _tenths3Button.Name = "_tenths3Button";
        _tenths3Button.Size = new Size(100, 87);
        _tenths3Button.TabIndex = 41;
        _tenths3Button.Text = ".30";
        _tenths3Button.UseVisualStyleBackColor = false;
        _tenths3Button.Click += DenominationButton_Click;
        // 
        // _tenths2Button
        // 
        _tenths2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths2Button.Location = new Point(427, 684);
        _tenths2Button.Name = "_tenths2Button";
        _tenths2Button.Size = new Size(100, 87);
        _tenths2Button.TabIndex = 47;
        _tenths2Button.Text = ".20";
        _tenths2Button.UseVisualStyleBackColor = false;
        _tenths2Button.Click += DenominationButton_Click;
        // 
        // _tenths1Button
        // 
        _tenths1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _tenths1Button.Location = new Point(427, 777);
        _tenths1Button.Name = "_tenths1Button";
        _tenths1Button.Size = new Size(100, 92);
        _tenths1Button.TabIndex = 53;
        _tenths1Button.Text = ".10";
        _tenths1Button.UseVisualStyleBackColor = false;
        _tenths1Button.Click += DenominationButton_Click;
        // 
        // _hundredths9Button
        // 
        _hundredths9Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths9Button.Location = new Point(533, 33);
        _hundredths9Button.Name = "_hundredths9Button";
        _hundredths9Button.Size = new Size(103, 87);
        _hundredths9Button.TabIndex = 6;
        _hundredths9Button.Text = ".09";
        _hundredths9Button.UseVisualStyleBackColor = false;
        _hundredths9Button.Click += DenominationButton_Click;
        // 
        // _hundredths8Button
        // 
        _hundredths8Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths8Button.Location = new Point(533, 126);
        _hundredths8Button.Name = "_hundredths8Button";
        _hundredths8Button.Size = new Size(103, 87);
        _hundredths8Button.TabIndex = 12;
        _hundredths8Button.Text = ".08";
        _hundredths8Button.UseVisualStyleBackColor = false;
        _hundredths8Button.Click += DenominationButton_Click;
        // 
        // _hundredths7Button
        // 
        _hundredths7Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths7Button.Location = new Point(533, 219);
        _hundredths7Button.Name = "_hundredths7Button";
        _hundredths7Button.Size = new Size(103, 87);
        _hundredths7Button.TabIndex = 18;
        _hundredths7Button.Text = ".07";
        _hundredths7Button.UseVisualStyleBackColor = false;
        _hundredths7Button.Click += DenominationButton_Click;
        // 
        // _hundredths6Button
        // 
        _hundredths6Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths6Button.Location = new Point(533, 312);
        _hundredths6Button.Name = "_hundredths6Button";
        _hundredths6Button.Size = new Size(103, 87);
        _hundredths6Button.TabIndex = 24;
        _hundredths6Button.Text = ".06";
        _hundredths6Button.UseVisualStyleBackColor = false;
        _hundredths6Button.Click += DenominationButton_Click;
        // 
        // _hundredths5Button
        // 
        _hundredths5Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths5Button.Location = new Point(533, 405);
        _hundredths5Button.Name = "_hundredths5Button";
        _hundredths5Button.Size = new Size(103, 87);
        _hundredths5Button.TabIndex = 30;
        _hundredths5Button.Text = ".05";
        _hundredths5Button.UseVisualStyleBackColor = false;
        _hundredths5Button.Click += DenominationButton_Click;
        // 
        // _hundredths4Button
        // 
        _hundredths4Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths4Button.Location = new Point(533, 498);
        _hundredths4Button.Name = "_hundredths4Button";
        _hundredths4Button.Size = new Size(103, 87);
        _hundredths4Button.TabIndex = 36;
        _hundredths4Button.Text = ".04";
        _hundredths4Button.UseVisualStyleBackColor = false;
        _hundredths4Button.Click += DenominationButton_Click;
        // 
        // _hundredths3Button
        // 
        _hundredths3Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths3Button.Location = new Point(533, 591);
        _hundredths3Button.Name = "_hundredths3Button";
        _hundredths3Button.Size = new Size(103, 87);
        _hundredths3Button.TabIndex = 42;
        _hundredths3Button.Text = ".03";
        _hundredths3Button.UseVisualStyleBackColor = false;
        _hundredths3Button.Click += DenominationButton_Click;
        // 
        // _hundredths2Button
        // 
        _hundredths2Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths2Button.Location = new Point(533, 684);
        _hundredths2Button.Name = "_hundredths2Button";
        _hundredths2Button.Size = new Size(103, 87);
        _hundredths2Button.TabIndex = 48;
        _hundredths2Button.Text = ".02";
        _hundredths2Button.UseVisualStyleBackColor = false;
        _hundredths2Button.Click += DenominationButton_Click;
        // 
        // _hundredths1Button
        // 
        _hundredths1Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _hundredths1Button.Location = new Point(533, 777);
        _hundredths1Button.Name = "_hundredths1Button";
        _hundredths1Button.Size = new Size(103, 92);
        _hundredths1Button.TabIndex = 54;
        _hundredths1Button.Text = ".01";
        _hundredths1Button.UseVisualStyleBackColor = false;
        _hundredths1Button.Click += DenominationButton_Click;
        // 
        // _departmentAndActionLayout
        // 
        _departmentAndActionLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _departmentAndActionLayout.ColumnCount = 1;
        _departmentAndActionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _departmentAndActionLayout.Controls.Add(_departmentGrid, 0, 0);
        _departmentAndActionLayout.Controls.Add(_actionGrid, 0, 1);
        _departmentAndActionLayout.Location = new Point(648, 3);
        _departmentAndActionLayout.Name = "_departmentAndActionLayout";
        _departmentAndActionLayout.RowCount = 2;
        _departmentAndActionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 73F));
        _departmentAndActionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 27F));
        _departmentAndActionLayout.Size = new Size(390, 872);
        _departmentAndActionLayout.TabIndex = 1;
        // 
        // _departmentGrid
        // 
        _departmentGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _departmentGrid.ColumnCount = 4;
        _departmentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _departmentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _departmentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        _departmentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
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
        _departmentGrid.Controls.Add(_department17Button, 0, 4);
        _departmentGrid.Controls.Add(_department18Button, 1, 4);
        _departmentGrid.Controls.Add(_department19Button, 2, 4);
        _departmentGrid.Controls.Add(_department20Button, 3, 4);
        _departmentGrid.Location = new Point(3, 3);
        _departmentGrid.Name = "_departmentGrid";
        _departmentGrid.RowCount = 5;
        _departmentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        _departmentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        _departmentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        _departmentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        _departmentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        _departmentGrid.Size = new Size(384, 630);
        _departmentGrid.TabIndex = 0;
        // 
        // _department01Button
        // 
        _department01Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department01Button.Location = new Point(3, 3);
        _department01Button.Name = "_department01Button";
        _department01Button.Size = new Size(90, 120);
        _department01Button.TabIndex = 55;
        _department01Button.Text = "DEP 01";
        _department01Button.UseVisualStyleBackColor = false;
        _department01Button.Click += DepartmentButton_Click;
        // 
        // _department02Button
        // 
        _department02Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department02Button.Location = new Point(99, 3);
        _department02Button.Name = "_department02Button";
        _department02Button.Size = new Size(90, 120);
        _department02Button.TabIndex = 56;
        _department02Button.Text = "DEP 02";
        _department02Button.UseVisualStyleBackColor = false;
        _department02Button.Click += DepartmentButton_Click;
        // 
        // _department03Button
        // 
        _department03Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department03Button.Location = new Point(195, 3);
        _department03Button.Name = "_department03Button";
        _department03Button.Size = new Size(90, 120);
        _department03Button.TabIndex = 57;
        _department03Button.Text = "DEP 03";
        _department03Button.UseVisualStyleBackColor = false;
        _department03Button.Click += DepartmentButton_Click;
        // 
        // _department04Button
        // 
        _department04Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department04Button.Location = new Point(291, 3);
        _department04Button.Name = "_department04Button";
        _department04Button.Size = new Size(90, 120);
        _department04Button.TabIndex = 58;
        _department04Button.Text = "DEP 04";
        _department04Button.UseVisualStyleBackColor = false;
        _department04Button.Click += DepartmentButton_Click;
        // 
        // _department05Button
        // 
        _department05Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department05Button.Location = new Point(3, 129);
        _department05Button.Name = "_department05Button";
        _department05Button.Size = new Size(90, 120);
        _department05Button.TabIndex = 59;
        _department05Button.Text = "DEP 05";
        _department05Button.UseVisualStyleBackColor = false;
        _department05Button.Click += DepartmentButton_Click;
        // 
        // _department06Button
        // 
        _department06Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department06Button.Location = new Point(99, 129);
        _department06Button.Name = "_department06Button";
        _department06Button.Size = new Size(90, 120);
        _department06Button.TabIndex = 60;
        _department06Button.Text = "DEP 06";
        _department06Button.UseVisualStyleBackColor = false;
        _department06Button.Click += DepartmentButton_Click;
        // 
        // _department07Button
        // 
        _department07Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department07Button.Location = new Point(195, 129);
        _department07Button.Name = "_department07Button";
        _department07Button.Size = new Size(90, 120);
        _department07Button.TabIndex = 61;
        _department07Button.Text = "DEP 07";
        _department07Button.UseVisualStyleBackColor = false;
        _department07Button.Click += DepartmentButton_Click;
        // 
        // _department08Button
        // 
        _department08Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department08Button.Location = new Point(291, 129);
        _department08Button.Name = "_department08Button";
        _department08Button.Size = new Size(90, 120);
        _department08Button.TabIndex = 62;
        _department08Button.Text = "DEP 08";
        _department08Button.UseVisualStyleBackColor = false;
        _department08Button.Click += DepartmentButton_Click;
        // 
        // _department09Button
        // 
        _department09Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department09Button.Location = new Point(3, 255);
        _department09Button.Name = "_department09Button";
        _department09Button.Size = new Size(90, 120);
        _department09Button.TabIndex = 63;
        _department09Button.Text = "DEP 09";
        _department09Button.UseVisualStyleBackColor = false;
        _department09Button.Click += DepartmentButton_Click;
        // 
        // _department10Button
        // 
        _department10Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department10Button.Location = new Point(99, 255);
        _department10Button.Name = "_department10Button";
        _department10Button.Size = new Size(90, 120);
        _department10Button.TabIndex = 64;
        _department10Button.Text = "DEP 10";
        _department10Button.UseVisualStyleBackColor = false;
        _department10Button.Click += DepartmentButton_Click;
        // 
        // _department11Button
        // 
        _department11Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department11Button.Location = new Point(195, 255);
        _department11Button.Name = "_department11Button";
        _department11Button.Size = new Size(90, 120);
        _department11Button.TabIndex = 65;
        _department11Button.Text = "DEP 11";
        _department11Button.UseVisualStyleBackColor = false;
        _department11Button.Click += DepartmentButton_Click;
        // 
        // _department12Button
        // 
        _department12Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department12Button.Location = new Point(291, 255);
        _department12Button.Name = "_department12Button";
        _department12Button.Size = new Size(90, 120);
        _department12Button.TabIndex = 66;
        _department12Button.Text = "DEP 12";
        _department12Button.UseVisualStyleBackColor = false;
        _department12Button.Click += DepartmentButton_Click;
        // 
        // _department13Button
        // 
        _department13Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department13Button.Location = new Point(3, 381);
        _department13Button.Name = "_department13Button";
        _department13Button.Size = new Size(90, 120);
        _department13Button.TabIndex = 67;
        _department13Button.Text = "DEP 13";
        _department13Button.UseVisualStyleBackColor = false;
        _department13Button.Click += DepartmentButton_Click;
        // 
        // _department14Button
        // 
        _department14Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department14Button.Location = new Point(99, 381);
        _department14Button.Name = "_department14Button";
        _department14Button.Size = new Size(90, 120);
        _department14Button.TabIndex = 68;
        _department14Button.Text = "DEP 14";
        _department14Button.UseVisualStyleBackColor = false;
        _department14Button.Click += DepartmentButton_Click;
        // 
        // _department15Button
        // 
        _department15Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department15Button.Location = new Point(195, 381);
        _department15Button.Name = "_department15Button";
        _department15Button.Size = new Size(90, 120);
        _department15Button.TabIndex = 69;
        _department15Button.Text = "DEP 15";
        _department15Button.UseVisualStyleBackColor = false;
        _department15Button.Click += DepartmentButton_Click;
        // 
        // _department16Button
        // 
        _department16Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department16Button.Location = new Point(291, 381);
        _department16Button.Name = "_department16Button";
        _department16Button.Size = new Size(90, 120);
        _department16Button.TabIndex = 70;
        _department16Button.Text = "DEP 16";
        _department16Button.UseVisualStyleBackColor = false;
        _department16Button.Click += DepartmentButton_Click;
        // 
        // _department17Button
        // 
        _department17Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department17Button.Location = new Point(3, 507);
        _department17Button.Name = "_department17Button";
        _department17Button.Size = new Size(90, 120);
        _department17Button.TabIndex = 71;
        _department17Button.Text = "DEP 17";
        _department17Button.UseVisualStyleBackColor = false;
        _department17Button.Click += DepartmentButton_Click;
        // 
        // _department18Button
        // 
        _department18Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department18Button.Location = new Point(99, 507);
        _department18Button.Name = "_department18Button";
        _department18Button.Size = new Size(90, 120);
        _department18Button.TabIndex = 72;
        _department18Button.Text = "DEP 18";
        _department18Button.UseVisualStyleBackColor = false;
        _department18Button.Click += DepartmentButton_Click;
        // 
        // _department19Button
        // 
        _department19Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department19Button.Location = new Point(195, 507);
        _department19Button.Name = "_department19Button";
        _department19Button.Size = new Size(90, 120);
        _department19Button.TabIndex = 73;
        _department19Button.Text = "DEP 19";
        _department19Button.UseVisualStyleBackColor = false;
        _department19Button.Click += DepartmentButton_Click;
        // 
        // _department20Button
        // 
        _department20Button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _department20Button.Location = new Point(291, 507);
        _department20Button.Name = "_department20Button";
        _department20Button.Size = new Size(90, 120);
        _department20Button.TabIndex = 74;
        _department20Button.Text = "DEP 20";
        _department20Button.UseVisualStyleBackColor = false;
        _department20Button.Click += DepartmentButton_Click;
        // 
        // _actionGrid
        // 
        _actionGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _actionGrid.ColumnCount = 2;
        _actionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _actionGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _actionGrid.Controls.Add(_taxButton, 0, 0);
        _actionGrid.Controls.Add(_voidButton, 1, 0);
        _actionGrid.Controls.Add(_subtotalButton, 0, 1);
        _actionGrid.Controls.Add(_totalButton, 1, 1);
        _actionGrid.Location = new Point(3, 639);
        _actionGrid.Name = "_actionGrid";
        _actionGrid.RowCount = 2;
        _actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _actionGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _actionGrid.Size = new Size(384, 230);
        _actionGrid.TabIndex = 1;
        // 
        // _taxButton
        // 
        _taxButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _taxButton.Location = new Point(3, 3);
        _taxButton.Name = "_taxButton";
        _taxButton.Size = new Size(186, 109);
        _taxButton.TabIndex = 75;
        _taxButton.Text = "TAX 8.25%";
        _taxButton.UseVisualStyleBackColor = false;
        _taxButton.Click += TaxButton_Click;
        // 
        // _voidButton
        // 
        _voidButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _voidButton.Location = new Point(195, 3);
        _voidButton.Name = "_voidButton";
        _voidButton.Size = new Size(186, 109);
        _voidButton.TabIndex = 76;
        _voidButton.Text = "VOID";
        _voidButton.UseVisualStyleBackColor = false;
        _voidButton.Click += VoidButton_Click;
        // 
        // _subtotalButton
        // 
        _subtotalButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _subtotalButton.Location = new Point(3, 118);
        _subtotalButton.Name = "_subtotalButton";
        _subtotalButton.Size = new Size(186, 109);
        _subtotalButton.TabIndex = 77;
        _subtotalButton.Text = "SUBTOTAL";
        _subtotalButton.UseVisualStyleBackColor = false;
        _subtotalButton.Click += SubtotalButton_Click;
        // 
        // _totalButton
        // 
        _totalButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _totalButton.Location = new Point(195, 118);
        _totalButton.Name = "_totalButton";
        _totalButton.Size = new Size(186, 109);
        _totalButton.TabIndex = 78;
        _totalButton.Text = "TOTAL";
        _totalButton.UseVisualStyleBackColor = false;
        _totalButton.Click += TotalButton_Click;
        // 
        // _receiptGroupBox
        // 
        _receiptGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _receiptGroupBox.Controls.Add(_receiptTextBox);
        _receiptGroupBox.Location = new Point(1064, 11);
        _receiptGroupBox.Name = "_receiptGroupBox";
        _receiptGroupBox.Padding = new Padding(10);
        _receiptGroupBox.Size = new Size(446, 1084);
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
        _receiptTextBox.Location = new Point(10, 38);
        _receiptTextBox.Name = "_receiptTextBox";
        _receiptTextBox.ReadOnly = true;
        _receiptTextBox.Size = new Size(426, 1036);
        _receiptTextBox.TabIndex = 0;
        _receiptTextBox.Text = "";
        _receiptTextBox.WordWrap = false;
        // 
        // CashRegisterView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        AutoScroll = true;
        AutoScrollMinSize = new Size(1200, 820);
        Controls.Add(_rootLayout);
        Name = "CashRegisterView";
        Size = new Size(1524, 1109);
        _rootLayout.ResumeLayout(false);
        _registerLayout.ResumeLayout(false);
        _keyBodyLayout.ResumeLayout(false);
        _denominationGrid.ResumeLayout(false);
        _denominationGrid.PerformLayout();
        _departmentAndActionLayout.ResumeLayout(false);
        _departmentGrid.ResumeLayout(false);
        _actionGrid.ResumeLayout(false);
        _receiptGroupBox.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _rootLayout;
    private TableLayoutPanel _registerLayout;
    private SevenSegmentDisplay _display;
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
    private CashRegisterKeyButton _department19Button;
    private CashRegisterKeyButton _department20Button;
    private CashRegisterKeyButton _taxButton;
    private CashRegisterKeyButton _voidButton;
    private CashRegisterKeyButton _subtotalButton;
    private CashRegisterKeyButton _totalButton;
}
