// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using VisualStylesModeDemo.Components;
using VisualStylesModeDemo.Controls;

namespace VisualStylesModeDemo.Views;

partial class CustomerEntryView
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomerEntryView));
        _contentLayoutPanel = new TableLayoutPanel();
        _identityGroupBox = new GroupBox();
        _identityLayoutPanel = new TableLayoutPanel();
        _customerNumberLabel = new Label();
        _customerNumberTextBox = new TextBox();
        _titleLabel = new Label();
        _titleComboBox = new ComboBox();
        _firstNameLabel = new Label();
        _firstNameTextBox = new TextBox();
        _middleNameLabel = new Label();
        _middleNameTextBox = new TextBox();
        _lastNameLabel = new Label();
        _lastNameTextBox = new TextBox();
        _birthDateLabel = new Label();
        _birthDatePicker = new DateTimePicker();
        _customerSinceLabel = new Label();
        _customerSincePicker = new DateTimePicker();
        _activeCustomerLabel = new Label();
        _activeCustomerCheckBox = new CheckBox();
        _contactGroupBox = new GroupBox();
        _contactLayoutPanel = new TableLayoutPanel();
        _emailLabel = new Label();
        _emailTextBox = new TextBox();
        _phoneLabel = new Label();
        _phoneMaskedTextBox = new MaskedTextBox();
        _mobileLabel = new Label();
        _mobileMaskedTextBox = new MaskedTextBox();
        _preferredContactLabel = new Label();
        _contactPermissionsLabel = new Label();
        _contactPermissionsFlowPanel = new FlowLayoutPanel();
        _emailPermissionCheckBox = new CheckBox();
        _smsPermissionCheckBox = new CheckBox();
        _preferredContactComboBox = new ComboBox();
        _addressGroupBox = new GroupBox();
        _addressLayoutPanel = new TableLayoutPanel();
        _addressLine1Label = new Label();
        _addressLine1TextBox = new TextBox();
        _addressLine2Label = new Label();
        _addressLine2TextBox = new TextBox();
        _streetLabel = new Label();
        _streetTextBox = new TextBox();
        _cityZipStateLabel = new Label();
        _cityZipStateLayoutPanel = new TableLayoutPanel();
        _cityTextBox = new TextBox();
        _zipMaskedTextBox = new MaskedTextBox();
        _stateTextBox = new TextBox();
        _countryLabel = new Label();
        _countryComboBox = new ComboBox();
        _preferencesGroupBox = new GroupBox();
        _preferencesLayoutPanel = new TableLayoutPanel();
        _customerTypeLabel = new Label();
        _customerTypeComboBox = new ComboBox();
        _accountStatusLabel = new Label();
        _accountStatusComboBox = new ComboBox();
        _creditLimitLabel = new Label();
        _creditLimitNumericUpDown = new NumericUpDown();
        _discountLabel = new Label();
        _discountNumericUpDown = new NumericUpDown();
        _languageLabel = new Label();
        _languageComboBox = new ComboBox();
        _timeZoneLabel = new Label();
        _timeZoneComboBox = new ComboBox();
        _accountOptionsLabel = new Label();
        _accountOptionsFlowPanel = new FlowLayoutPanel();
        _paperlessCheckBox = new CheckBox();
        _priorityCheckBox = new CheckBox();
        _notesGroupBox = new GroupBox();
        _notesRichTextBox = new RichTextBox();
        _notesToolStrip = new ToolStrip();
        _cutToolStripButton = new ToolStripButton();
        _copyToolStripButton = new ToolStripButton();
        _pasteToolStripButton = new ToolStripButton();
        toolStripSeparator1 = new ToolStripSeparator();
        _boldToolStripButton = new ToolStripButton();
        _italicToolStripButton = new ToolStripButton();
        _underlineToolStripButton = new ToolStripButton();
        _iconFactoryComponent = new IconFactoryComponent(components);
        _contentLayoutPanel.SuspendLayout();
        _identityGroupBox.SuspendLayout();
        _identityLayoutPanel.SuspendLayout();
        _contactGroupBox.SuspendLayout();
        _contactLayoutPanel.SuspendLayout();
        _contactPermissionsFlowPanel.SuspendLayout();
        _addressGroupBox.SuspendLayout();
        _addressLayoutPanel.SuspendLayout();
        _cityZipStateLayoutPanel.SuspendLayout();
        _preferencesGroupBox.SuspendLayout();
        _preferencesLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_creditLimitNumericUpDown).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_discountNumericUpDown).BeginInit();
        _accountOptionsFlowPanel.SuspendLayout();
        _notesGroupBox.SuspendLayout();
        _notesRichTextBox.SuspendLayout();
        _notesToolStrip.SuspendLayout();
        SuspendLayout();
        // 
        // _contentLayoutPanel
        // 
        _contentLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _contentLayoutPanel.ColumnCount = 2;
        _contentLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _contentLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _contentLayoutPanel.Controls.Add(_identityGroupBox, 0, 0);
        _contentLayoutPanel.Controls.Add(_contactGroupBox, 1, 0);
        _contentLayoutPanel.Controls.Add(_addressGroupBox, 0, 1);
        _contentLayoutPanel.Controls.Add(_preferencesGroupBox, 1, 1);
        _contentLayoutPanel.Controls.Add(_notesGroupBox, 0, 2);
        _contentLayoutPanel.Dock = DockStyle.Fill;
        _contentLayoutPanel.Location = new Point(15, 15);
        _contentLayoutPanel.Name = "_contentLayoutPanel";
        _contentLayoutPanel.RowCount = 3;
        _contentLayoutPanel.RowStyles.Add(new RowStyle());
        _contentLayoutPanel.RowStyles.Add(new RowStyle());
        _contentLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _contentLayoutPanel.Size = new Size(2012, 1079);
        _contentLayoutPanel.TabIndex = 0;
        // 
        // _identityGroupBox
        // 
        _identityGroupBox.AutoSize = true;
        _identityGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _identityGroupBox.Controls.Add(_identityLayoutPanel);
        _identityGroupBox.Dock = DockStyle.Fill;
        _identityGroupBox.FlatStyle = FlatStyle.Popup;
        _identityGroupBox.Location = new Point(3, 3);
        _identityGroupBox.Margin = new Padding(3, 3, 10, 10);
        _identityGroupBox.Name = "_identityGroupBox";
        _identityGroupBox.Size = new Size(993, 384);
        _identityGroupBox.TabIndex = 0;
        _identityGroupBox.TabStop = false;
        _identityGroupBox.Text = "Customer identity";
        _identityGroupBox.Enter += _identityGroupBox_Enter;
        // 
        // _identityLayoutPanel
        // 
        _identityLayoutPanel.AutoSize = true;
        _identityLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _identityLayoutPanel.ColumnCount = 4;
        _identityLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _identityLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _identityLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _identityLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _identityLayoutPanel.Controls.Add(_customerNumberLabel, 0, 0);
        _identityLayoutPanel.Controls.Add(_customerNumberTextBox, 1, 0);
        _identityLayoutPanel.Controls.Add(_titleLabel, 2, 0);
        _identityLayoutPanel.Controls.Add(_titleComboBox, 3, 0);
        _identityLayoutPanel.Controls.Add(_firstNameLabel, 0, 1);
        _identityLayoutPanel.Controls.Add(_firstNameTextBox, 1, 1);
        _identityLayoutPanel.Controls.Add(_middleNameLabel, 2, 1);
        _identityLayoutPanel.Controls.Add(_middleNameTextBox, 3, 1);
        _identityLayoutPanel.Controls.Add(_lastNameLabel, 0, 2);
        _identityLayoutPanel.Controls.Add(_lastNameTextBox, 1, 2);
        _identityLayoutPanel.Controls.Add(_birthDateLabel, 2, 2);
        _identityLayoutPanel.Controls.Add(_birthDatePicker, 3, 2);
        _identityLayoutPanel.Controls.Add(_customerSinceLabel, 0, 3);
        _identityLayoutPanel.Controls.Add(_customerSincePicker, 1, 3);
        _identityLayoutPanel.Controls.Add(_activeCustomerLabel, 2, 3);
        _identityLayoutPanel.Controls.Add(_activeCustomerCheckBox, 3, 3);
        _identityLayoutPanel.Dock = DockStyle.Fill;
        _identityLayoutPanel.Location = new Point(7, 65);
        _identityLayoutPanel.Name = "_identityLayoutPanel";
        _identityLayoutPanel.RowCount = 5;
        _identityLayoutPanel.RowStyles.Add(new RowStyle());
        _identityLayoutPanel.RowStyles.Add(new RowStyle());
        _identityLayoutPanel.RowStyles.Add(new RowStyle());
        _identityLayoutPanel.RowStyles.Add(new RowStyle());
        _identityLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _identityLayoutPanel.Size = new Size(979, 312);
        _identityLayoutPanel.TabIndex = 0;
        // 
        // _customerNumberLabel
        // 
        _customerNumberLabel.Anchor = AnchorStyles.Left;
        _customerNumberLabel.AutoSize = true;
        _customerNumberLabel.Location = new Point(0, 16);
        _customerNumberLabel.Margin = new Padding(0);
        _customerNumberLabel.Name = "_customerNumberLabel";
        _customerNumberLabel.Size = new Size(173, 35);
        _customerNumberLabel.TabIndex = 0;
        _customerNumberLabel.Text = "Customer &No.:";
        // 
        // _customerNumberTextBox
        // 
        _customerNumberTextBox.AccessibleName = "Customer number";
        _customerNumberTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _customerNumberTextBox.Location = new Point(195, 7);
        _customerNumberTextBox.Margin = new Padding(5);
        _customerNumberTextBox.Name = "_customerNumberTextBox";
        _customerNumberTextBox.Size = new Size(318, 54);
        _customerNumberTextBox.TabIndex = 1;
        _customerNumberTextBox.Text = "C-10427";
        // 
        // _titleLabel
        // 
        _titleLabel.Anchor = AnchorStyles.Left;
        _titleLabel.AutoSize = true;
        _titleLabel.Location = new Point(521, 16);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.Size = new Size(66, 35);
        _titleLabel.TabIndex = 2;
        _titleLabel.Text = "&Title:";
        // 
        // _titleComboBox
        // 
        _titleComboBox.AccessibleName = "Title";
        _titleComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _titleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _titleComboBox.FormattingEnabled = true;
        _titleComboBox.Location = new Point(656, 5);
        _titleComboBox.Margin = new Padding(5);
        _titleComboBox.Name = "_titleComboBox";
        _titleComboBox.Size = new Size(318, 58);
        _titleComboBox.TabIndex = 3;
        // 
        // _firstNameLabel
        // 
        _firstNameLabel.Anchor = AnchorStyles.Left;
        _firstNameLabel.AutoSize = true;
        _firstNameLabel.Location = new Point(0, 82);
        _firstNameLabel.Margin = new Padding(0);
        _firstNameLabel.Name = "_firstNameLabel";
        _firstNameLabel.Size = new Size(135, 35);
        _firstNameLabel.TabIndex = 4;
        _firstNameLabel.Text = "&First name:";
        // 
        // _firstNameTextBox
        // 
        _firstNameTextBox.AccessibleName = "First name";
        _firstNameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _firstNameTextBox.Location = new Point(195, 73);
        _firstNameTextBox.Margin = new Padding(5);
        _firstNameTextBox.Name = "_firstNameTextBox";
        _firstNameTextBox.Size = new Size(318, 54);
        _firstNameTextBox.TabIndex = 5;
        _firstNameTextBox.Text = "Alex";
        // 
        // _middleNameLabel
        // 
        _middleNameLabel.Anchor = AnchorStyles.Left;
        _middleNameLabel.AutoSize = true;
        _middleNameLabel.Location = new Point(521, 82);
        _middleNameLabel.Name = "_middleNameLabel";
        _middleNameLabel.Size = new Size(97, 35);
        _middleNameLabel.TabIndex = 6;
        _middleNameLabel.Text = "&Middle:";
        // 
        // _middleNameTextBox
        // 
        _middleNameTextBox.AccessibleName = "Middle name";
        _middleNameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _middleNameTextBox.Location = new Point(656, 73);
        _middleNameTextBox.Margin = new Padding(5);
        _middleNameTextBox.Name = "_middleNameTextBox";
        _middleNameTextBox.Size = new Size(318, 54);
        _middleNameTextBox.TabIndex = 7;
        // 
        // _lastNameLabel
        // 
        _lastNameLabel.Anchor = AnchorStyles.Left;
        _lastNameLabel.AutoSize = true;
        _lastNameLabel.Location = new Point(0, 146);
        _lastNameLabel.Margin = new Padding(0);
        _lastNameLabel.Name = "_lastNameLabel";
        _lastNameLabel.Size = new Size(133, 35);
        _lastNameLabel.TabIndex = 8;
        _lastNameLabel.Text = "&Last name:";
        // 
        // _lastNameTextBox
        // 
        _lastNameTextBox.AccessibleName = "Last name";
        _lastNameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _lastNameTextBox.Location = new Point(195, 137);
        _lastNameTextBox.Margin = new Padding(5);
        _lastNameTextBox.Name = "_lastNameTextBox";
        _lastNameTextBox.Size = new Size(318, 54);
        _lastNameTextBox.TabIndex = 9;
        _lastNameTextBox.Text = "Morgan";
        // 
        // _birthDateLabel
        // 
        _birthDateLabel.Anchor = AnchorStyles.Left;
        _birthDateLabel.AutoSize = true;
        _birthDateLabel.Location = new Point(521, 146);
        _birthDateLabel.Name = "_birthDateLabel";
        _birthDateLabel.Size = new Size(127, 35);
        _birthDateLabel.TabIndex = 10;
        _birthDateLabel.Text = "&Birth date:";
        // 
        // _birthDatePicker
        // 
        _birthDatePicker.AccessibleName = "Birth date";
        _birthDatePicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _birthDatePicker.Format = DateTimePickerFormat.Short;
        _birthDatePicker.Location = new Point(656, 144);
        _birthDatePicker.Margin = new Padding(5);
        _birthDatePicker.Name = "_birthDatePicker";
        _birthDatePicker.Size = new Size(318, 39);
        _birthDatePicker.TabIndex = 11;
        _birthDatePicker.Value = new DateTime(1987, 6, 15, 0, 0, 0, 0);
        // 
        // _customerSinceLabel
        // 
        _customerSinceLabel.Anchor = AnchorStyles.Left;
        _customerSinceLabel.AutoSize = true;
        _customerSinceLabel.Location = new Point(0, 205);
        _customerSinceLabel.Margin = new Padding(0);
        _customerSinceLabel.Name = "_customerSinceLabel";
        _customerSinceLabel.Size = new Size(190, 35);
        _customerSinceLabel.TabIndex = 12;
        _customerSinceLabel.Text = "Customer &since:";
        // 
        // _customerSincePicker
        // 
        _customerSincePicker.AccessibleName = "Customer since";
        _customerSincePicker.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _customerSincePicker.Format = DateTimePickerFormat.Short;
        _customerSincePicker.Location = new Point(195, 203);
        _customerSincePicker.Margin = new Padding(5);
        _customerSincePicker.Name = "_customerSincePicker";
        _customerSincePicker.Size = new Size(318, 39);
        _customerSincePicker.TabIndex = 13;
        // 
        // _activeCustomerLabel
        // 
        _activeCustomerLabel.Anchor = AnchorStyles.Left;
        _activeCustomerLabel.AutoSize = true;
        _activeCustomerLabel.Location = new Point(521, 205);
        _activeCustomerLabel.Name = "_activeCustomerLabel";
        _activeCustomerLabel.Size = new Size(86, 35);
        _activeCustomerLabel.TabIndex = 14;
        _activeCustomerLabel.Text = "Status:";
        // 
        // _activeCustomerCheckBox
        // 
        _activeCustomerCheckBox.AccessibleName = "Active customer";
        _activeCustomerCheckBox.Anchor = AnchorStyles.Left;
        _activeCustomerCheckBox.Appearance = Appearance.ToggleSwitch;
        _activeCustomerCheckBox.AutoSize = true;
        _activeCustomerCheckBox.Checked = true;
        _activeCustomerCheckBox.CheckState = CheckState.Checked;
        _activeCustomerCheckBox.Location = new Point(656, 201);
        _activeCustomerCheckBox.Margin = new Padding(5);
        _activeCustomerCheckBox.Name = "_activeCustomerCheckBox";
        _activeCustomerCheckBox.Padding = new Padding(4);
        _activeCustomerCheckBox.Size = new Size(155, 43);
        _activeCustomerCheckBox.TabIndex = 15;
        _activeCustomerCheckBox.Text = "&Active";
        _activeCustomerCheckBox.UseVisualStyleBackColor = true;
        // 
        // _contactGroupBox
        // 
        _contactGroupBox.AutoSize = true;
        _contactGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _contactGroupBox.Controls.Add(_contactLayoutPanel);
        _contactGroupBox.Dock = DockStyle.Fill;
        _contactGroupBox.FlatStyle = FlatStyle.Popup;
        _contactGroupBox.Location = new Point(1016, 0);
        _contactGroupBox.Margin = new Padding(10, 0, 0, 10);
        _contactGroupBox.Name = "_contactGroupBox";
        _contactGroupBox.Size = new Size(996, 387);
        _contactGroupBox.TabIndex = 1;
        _contactGroupBox.TabStop = false;
        _contactGroupBox.Text = "Contact details";
        // 
        // _contactLayoutPanel
        // 
        _contactLayoutPanel.AutoSize = true;
        _contactLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _contactLayoutPanel.ColumnCount = 2;
        _contactLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _contactLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _contactLayoutPanel.Controls.Add(_emailLabel, 0, 0);
        _contactLayoutPanel.Controls.Add(_emailTextBox, 1, 0);
        _contactLayoutPanel.Controls.Add(_phoneLabel, 0, 1);
        _contactLayoutPanel.Controls.Add(_phoneMaskedTextBox, 1, 1);
        _contactLayoutPanel.Controls.Add(_mobileLabel, 0, 2);
        _contactLayoutPanel.Controls.Add(_mobileMaskedTextBox, 1, 2);
        _contactLayoutPanel.Controls.Add(_preferredContactLabel, 0, 3);
        _contactLayoutPanel.Controls.Add(_contactPermissionsLabel, 0, 4);
        _contactLayoutPanel.Controls.Add(_contactPermissionsFlowPanel, 1, 4);
        _contactLayoutPanel.Controls.Add(_preferredContactComboBox, 1, 3);
        _contactLayoutPanel.Dock = DockStyle.Fill;
        _contactLayoutPanel.Location = new Point(7, 65);
        _contactLayoutPanel.Name = "_contactLayoutPanel";
        _contactLayoutPanel.RowCount = 5;
        _contactLayoutPanel.RowStyles.Add(new RowStyle());
        _contactLayoutPanel.RowStyles.Add(new RowStyle());
        _contactLayoutPanel.RowStyles.Add(new RowStyle());
        _contactLayoutPanel.RowStyles.Add(new RowStyle());
        _contactLayoutPanel.RowStyles.Add(new RowStyle());
        _contactLayoutPanel.Size = new Size(982, 315);
        _contactLayoutPanel.TabIndex = 0;
        // 
        // _emailLabel
        // 
        _emailLabel.Anchor = AnchorStyles.Left;
        _emailLabel.AutoSize = true;
        _emailLabel.Location = new Point(0, 14);
        _emailLabel.Margin = new Padding(0);
        _emailLabel.Name = "_emailLabel";
        _emailLabel.Size = new Size(80, 35);
        _emailLabel.TabIndex = 0;
        _emailLabel.Text = "&Email:";
        // 
        // _emailTextBox
        // 
        _emailTextBox.AccessibleName = "Email address";
        _emailTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _emailTextBox.Location = new Point(217, 5);
        _emailTextBox.Margin = new Padding(5);
        _emailTextBox.Name = "_emailTextBox";
        _emailTextBox.Size = new Size(760, 54);
        _emailTextBox.TabIndex = 1;
        _emailTextBox.Text = "alex.morgan@example.com";
        // 
        // _phoneLabel
        // 
        _phoneLabel.Anchor = AnchorStyles.Left;
        _phoneLabel.AutoSize = true;
        _phoneLabel.Location = new Point(0, 78);
        _phoneLabel.Margin = new Padding(0);
        _phoneLabel.Name = "_phoneLabel";
        _phoneLabel.Size = new Size(90, 35);
        _phoneLabel.TabIndex = 2;
        _phoneLabel.Text = "&Phone:";
        // 
        // _phoneMaskedTextBox
        // 
        _phoneMaskedTextBox.AccessibleName = "Phone number";
        _phoneMaskedTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _phoneMaskedTextBox.Location = new Point(217, 69);
        _phoneMaskedTextBox.Margin = new Padding(5);
        _phoneMaskedTextBox.Mask = "(999) 000-0000";
        _phoneMaskedTextBox.Name = "_phoneMaskedTextBox";
        _phoneMaskedTextBox.Size = new Size(760, 54);
        _phoneMaskedTextBox.TabIndex = 3;
        _phoneMaskedTextBox.Text = "2065550142";
        // 
        // _mobileLabel
        // 
        _mobileLabel.Anchor = AnchorStyles.Left;
        _mobileLabel.AutoSize = true;
        _mobileLabel.Location = new Point(0, 142);
        _mobileLabel.Margin = new Padding(0);
        _mobileLabel.Name = "_mobileLabel";
        _mobileLabel.Size = new Size(97, 35);
        _mobileLabel.TabIndex = 4;
        _mobileLabel.Text = "&Mobile:";
        // 
        // _mobileMaskedTextBox
        // 
        _mobileMaskedTextBox.AccessibleName = "Mobile phone number";
        _mobileMaskedTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _mobileMaskedTextBox.Location = new Point(217, 133);
        _mobileMaskedTextBox.Margin = new Padding(5);
        _mobileMaskedTextBox.Mask = "(999) 000-0000";
        _mobileMaskedTextBox.Name = "_mobileMaskedTextBox";
        _mobileMaskedTextBox.Size = new Size(760, 54);
        _mobileMaskedTextBox.TabIndex = 5;
        _mobileMaskedTextBox.Text = "2065550188";
        // 
        // _preferredContactLabel
        // 
        _preferredContactLabel.Anchor = AnchorStyles.Left;
        _preferredContactLabel.AutoSize = true;
        _preferredContactLabel.Location = new Point(0, 206);
        _preferredContactLabel.Margin = new Padding(0);
        _preferredContactLabel.Name = "_preferredContactLabel";
        _preferredContactLabel.Size = new Size(212, 35);
        _preferredContactLabel.TabIndex = 6;
        _preferredContactLabel.Text = "Preferred &contact:";
        // 
        // _contactPermissionsLabel
        // 
        _contactPermissionsLabel.Anchor = AnchorStyles.Left;
        _contactPermissionsLabel.AutoSize = true;
        _contactPermissionsLabel.Location = new Point(0, 268);
        _contactPermissionsLabel.Margin = new Padding(0);
        _contactPermissionsLabel.Name = "_contactPermissionsLabel";
        _contactPermissionsLabel.Size = new Size(151, 35);
        _contactPermissionsLabel.TabIndex = 8;
        _contactPermissionsLabel.Text = "Permissions:";
        // 
        // _contactPermissionsFlowPanel
        // 
        _contactPermissionsFlowPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _contactPermissionsFlowPanel.AutoSize = true;
        _contactPermissionsFlowPanel.Controls.Add(_emailPermissionCheckBox);
        _contactPermissionsFlowPanel.Controls.Add(_smsPermissionCheckBox);
        _contactPermissionsFlowPanel.Location = new Point(215, 259);
        _contactPermissionsFlowPanel.Name = "_contactPermissionsFlowPanel";
        _contactPermissionsFlowPanel.Size = new Size(764, 53);
        _contactPermissionsFlowPanel.TabIndex = 9;
        _contactPermissionsFlowPanel.WrapContents = false;
        // 
        // _emailPermissionCheckBox
        // 
        _emailPermissionCheckBox.Appearance = Appearance.ToggleSwitch;
        _emailPermissionCheckBox.AutoSize = true;
        _emailPermissionCheckBox.Checked = true;
        _emailPermissionCheckBox.CheckState = CheckState.Checked;
        _emailPermissionCheckBox.Location = new Point(5, 5);
        _emailPermissionCheckBox.Margin = new Padding(5);
        _emailPermissionCheckBox.Name = "_emailPermissionCheckBox";
        _emailPermissionCheckBox.Padding = new Padding(4);
        _emailPermissionCheckBox.Size = new Size(216, 43);
        _emailPermissionCheckBox.TabIndex = 0;
        _emailPermissionCheckBox.Text = "Allow &email";
        _emailPermissionCheckBox.UseVisualStyleBackColor = true;
        // 
        // _smsPermissionCheckBox
        // 
        _smsPermissionCheckBox.Appearance = Appearance.ToggleSwitch;
        _smsPermissionCheckBox.AutoSize = true;
        _smsPermissionCheckBox.Location = new Point(231, 5);
        _smsPermissionCheckBox.Margin = new Padding(5);
        _smsPermissionCheckBox.Name = "_smsPermissionCheckBox";
        _smsPermissionCheckBox.Padding = new Padding(4);
        _smsPermissionCheckBox.Size = new Size(204, 43);
        _smsPermissionCheckBox.TabIndex = 1;
        _smsPermissionCheckBox.Text = "Allow &SMS";
        _smsPermissionCheckBox.UseVisualStyleBackColor = true;
        // 
        // _preferredContactComboBox
        // 
        _preferredContactComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _preferredContactComboBox.FlatStyle = FlatStyle.Popup;
        _preferredContactComboBox.FormattingEnabled = true;
        _preferredContactComboBox.Location = new Point(215, 195);
        _preferredContactComboBox.Name = "_preferredContactComboBox";
        _preferredContactComboBox.Size = new Size(764, 58);
        _preferredContactComboBox.TabIndex = 10;
        // 
        // _addressGroupBox
        // 
        _addressGroupBox.AutoSize = true;
        _addressGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _addressGroupBox.Controls.Add(_addressLayoutPanel);
        _addressGroupBox.Dock = DockStyle.Fill;
        _addressGroupBox.FlatStyle = FlatStyle.Popup;
        _addressGroupBox.Location = new Point(0, 397);
        _addressGroupBox.Margin = new Padding(0, 0, 10, 10);
        _addressGroupBox.MinimumSize = new Size(0, 300);
        _addressGroupBox.Name = "_addressGroupBox";
        _addressGroupBox.Padding = new Padding(0);
        _addressGroupBox.Size = new Size(996, 386);
        _addressGroupBox.TabIndex = 2;
        _addressGroupBox.TabStop = false;
        _addressGroupBox.Text = "Address";
        _addressGroupBox.Enter += _addressGroupBox_Enter;
        // 
        // _addressLayoutPanel
        // 
        _addressLayoutPanel.AutoSize = true;
        _addressLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _addressLayoutPanel.ColumnCount = 2;
        _addressLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _addressLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _addressLayoutPanel.Controls.Add(_addressLine1Label, 0, 0);
        _addressLayoutPanel.Controls.Add(_addressLine1TextBox, 1, 0);
        _addressLayoutPanel.Controls.Add(_addressLine2Label, 0, 1);
        _addressLayoutPanel.Controls.Add(_addressLine2TextBox, 1, 1);
        _addressLayoutPanel.Controls.Add(_streetLabel, 0, 2);
        _addressLayoutPanel.Controls.Add(_streetTextBox, 1, 2);
        _addressLayoutPanel.Controls.Add(_cityZipStateLabel, 0, 3);
        _addressLayoutPanel.Controls.Add(_cityZipStateLayoutPanel, 1, 3);
        _addressLayoutPanel.Controls.Add(_countryLabel, 0, 4);
        _addressLayoutPanel.Controls.Add(_countryComboBox, 1, 4);
        _addressLayoutPanel.Dock = DockStyle.Fill;
        _addressLayoutPanel.Location = new Point(4, 62);
        _addressLayoutPanel.Margin = new Padding(0);
        _addressLayoutPanel.Name = "_addressLayoutPanel";
        _addressLayoutPanel.RowCount = 5;
        _addressLayoutPanel.RowStyles.Add(new RowStyle());
        _addressLayoutPanel.RowStyles.Add(new RowStyle());
        _addressLayoutPanel.RowStyles.Add(new RowStyle());
        _addressLayoutPanel.RowStyles.Add(new RowStyle());
        _addressLayoutPanel.RowStyles.Add(new RowStyle());
        _addressLayoutPanel.Size = new Size(988, 320);
        _addressLayoutPanel.TabIndex = 0;
        // 
        // _addressLine1Label
        // 
        _addressLine1Label.Anchor = AnchorStyles.Left;
        _addressLine1Label.AutoSize = true;
        _addressLine1Label.Location = new Point(0, 14);
        _addressLine1Label.Margin = new Padding(0);
        _addressLine1Label.Name = "_addressLine1Label";
        _addressLine1Label.Size = new Size(182, 35);
        _addressLine1Label.TabIndex = 0;
        _addressLine1Label.Text = "Address Line &1:";
        // 
        // _addressLine1TextBox
        // 
        _addressLine1TextBox.AccessibleName = "Address line 1";
        _addressLine1TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _addressLine1TextBox.Location = new Point(203, 5);
        _addressLine1TextBox.Margin = new Padding(5);
        _addressLine1TextBox.Name = "_addressLine1TextBox";
        _addressLine1TextBox.Size = new Size(780, 54);
        _addressLine1TextBox.TabIndex = 1;
        _addressLine1TextBox.Text = "Suite 420";
        // 
        // _addressLine2Label
        // 
        _addressLine2Label.Anchor = AnchorStyles.Left;
        _addressLine2Label.AutoSize = true;
        _addressLine2Label.Location = new Point(0, 78);
        _addressLine2Label.Margin = new Padding(0);
        _addressLine2Label.Name = "_addressLine2Label";
        _addressLine2Label.Size = new Size(182, 35);
        _addressLine2Label.TabIndex = 2;
        _addressLine2Label.Text = "Address Line &2:";
        // 
        // _addressLine2TextBox
        // 
        _addressLine2TextBox.AccessibleName = "Address line 2";
        _addressLine2TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _addressLine2TextBox.Location = new Point(203, 69);
        _addressLine2TextBox.Margin = new Padding(5);
        _addressLine2TextBox.Name = "_addressLine2TextBox";
        _addressLine2TextBox.Size = new Size(780, 54);
        _addressLine2TextBox.TabIndex = 3;
        _addressLine2TextBox.Text = "North Building";
        // 
        // _streetLabel
        // 
        _streetLabel.Anchor = AnchorStyles.Left;
        _streetLabel.AutoSize = true;
        _streetLabel.Location = new Point(0, 142);
        _streetLabel.Margin = new Padding(0);
        _streetLabel.Name = "_streetLabel";
        _streetLabel.Size = new Size(83, 35);
        _streetLabel.TabIndex = 4;
        _streetLabel.Text = "&Street:";
        // 
        // _streetTextBox
        // 
        _streetTextBox.AccessibleName = "Street";
        _streetTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _streetTextBox.Location = new Point(203, 133);
        _streetTextBox.Margin = new Padding(5);
        _streetTextBox.Name = "_streetTextBox";
        _streetTextBox.Size = new Size(780, 54);
        _streetTextBox.TabIndex = 5;
        _streetTextBox.Text = "1234 Market Street";
        // 
        // _cityZipStateLabel
        // 
        _cityZipStateLabel.Anchor = AnchorStyles.Left;
        _cityZipStateLabel.AutoSize = true;
        _cityZipStateLabel.Location = new Point(0, 206);
        _cityZipStateLabel.Margin = new Padding(0);
        _cityZipStateLabel.Name = "_cityZipStateLabel";
        _cityZipStateLabel.Size = new Size(198, 35);
        _cityZipStateLabel.TabIndex = 6;
        _cityZipStateLabel.Text = "&City / &ZIP / &State:";
        // 
        // _cityZipStateLayoutPanel
        // 
        _cityZipStateLayoutPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _cityZipStateLayoutPanel.AutoSize = true;
        _cityZipStateLayoutPanel.ColumnCount = 3;
        _cityZipStateLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _cityZipStateLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _cityZipStateLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _cityZipStateLayoutPanel.Controls.Add(_cityTextBox, 0, 0);
        _cityZipStateLayoutPanel.Controls.Add(_zipMaskedTextBox, 1, 0);
        _cityZipStateLayoutPanel.Controls.Add(_stateTextBox, 2, 0);
        _cityZipStateLayoutPanel.Location = new Point(198, 192);
        _cityZipStateLayoutPanel.Margin = new Padding(0);
        _cityZipStateLayoutPanel.Name = "_cityZipStateLayoutPanel";
        _cityZipStateLayoutPanel.RowCount = 1;
        _cityZipStateLayoutPanel.RowStyles.Add(new RowStyle());
        _cityZipStateLayoutPanel.Size = new Size(790, 64);
        _cityZipStateLayoutPanel.TabIndex = 7;
        // 
        // _cityTextBox
        // 
        _cityTextBox.AccessibleName = "City";
        _cityTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _cityTextBox.Location = new Point(5, 5);
        _cityTextBox.Margin = new Padding(5);
        _cityTextBox.Name = "_cityTextBox";
        _cityTextBox.Size = new Size(531, 54);
        _cityTextBox.TabIndex = 0;
        _cityTextBox.Text = "Seattle";
        // 
        // _zipMaskedTextBox
        // 
        _zipMaskedTextBox.AccessibleName = "ZIP code";
        _zipMaskedTextBox.Dock = DockStyle.Fill;
        _zipMaskedTextBox.Location = new Point(546, 5);
        _zipMaskedTextBox.Margin = new Padding(5);
        _zipMaskedTextBox.Mask = "00000-9999";
        _zipMaskedTextBox.Name = "_zipMaskedTextBox";
        _zipMaskedTextBox.Size = new Size(148, 54);
        _zipMaskedTextBox.TabIndex = 1;
        _zipMaskedTextBox.Text = "98101";
        // 
        // _stateTextBox
        // 
        _stateTextBox.AccessibleName = "State";
        _stateTextBox.CharacterCasing = CharacterCasing.Upper;
        _stateTextBox.Dock = DockStyle.Fill;
        _stateTextBox.Location = new Point(704, 5);
        _stateTextBox.Margin = new Padding(5);
        _stateTextBox.MaxLength = 2;
        _stateTextBox.Name = "_stateTextBox";
        _stateTextBox.Size = new Size(81, 54);
        _stateTextBox.TabIndex = 2;
        _stateTextBox.Text = "WA";
        // 
        // _countryLabel
        // 
        _countryLabel.Anchor = AnchorStyles.Left;
        _countryLabel.AutoSize = true;
        _countryLabel.Location = new Point(0, 270);
        _countryLabel.Margin = new Padding(0);
        _countryLabel.Name = "_countryLabel";
        _countryLabel.Size = new Size(107, 35);
        _countryLabel.TabIndex = 8;
        _countryLabel.Text = "C&ountry:";
        // 
        // _countryComboBox
        // 
        _countryComboBox.AccessibleName = "Country";
        _countryComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _countryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _countryComboBox.FormattingEnabled = true;
        _countryComboBox.Location = new Point(201, 259);
        _countryComboBox.Name = "_countryComboBox";
        _countryComboBox.Size = new Size(784, 58);
        _countryComboBox.TabIndex = 9;
        _countryComboBox.VisualStylesMode = VisualStylesMode.Net11;
        // 
        // _preferencesGroupBox
        // 
        _preferencesGroupBox.AutoSize = true;
        _preferencesGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _preferencesGroupBox.Controls.Add(_preferencesLayoutPanel);
        _preferencesGroupBox.Dock = DockStyle.Fill;
        _preferencesGroupBox.FlatStyle = FlatStyle.Popup;
        _preferencesGroupBox.Location = new Point(1016, 397);
        _preferencesGroupBox.Margin = new Padding(10, 0, 0, 10);
        _preferencesGroupBox.MinimumSize = new Size(0, 300);
        _preferencesGroupBox.Name = "_preferencesGroupBox";
        _preferencesGroupBox.Size = new Size(996, 386);
        _preferencesGroupBox.TabIndex = 3;
        _preferencesGroupBox.TabStop = false;
        _preferencesGroupBox.Text = "Account and preferences";
        // 
        // _preferencesLayoutPanel
        // 
        _preferencesLayoutPanel.AutoSize = true;
        _preferencesLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _preferencesLayoutPanel.ColumnCount = 4;
        _preferencesLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _preferencesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _preferencesLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _preferencesLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _preferencesLayoutPanel.Controls.Add(_customerTypeLabel, 0, 0);
        _preferencesLayoutPanel.Controls.Add(_customerTypeComboBox, 1, 0);
        _preferencesLayoutPanel.Controls.Add(_accountStatusLabel, 2, 0);
        _preferencesLayoutPanel.Controls.Add(_accountStatusComboBox, 3, 0);
        _preferencesLayoutPanel.Controls.Add(_creditLimitLabel, 0, 1);
        _preferencesLayoutPanel.Controls.Add(_creditLimitNumericUpDown, 1, 1);
        _preferencesLayoutPanel.Controls.Add(_discountLabel, 2, 1);
        _preferencesLayoutPanel.Controls.Add(_discountNumericUpDown, 3, 1);
        _preferencesLayoutPanel.Controls.Add(_languageLabel, 0, 2);
        _preferencesLayoutPanel.Controls.Add(_languageComboBox, 1, 2);
        _preferencesLayoutPanel.Controls.Add(_timeZoneLabel, 2, 2);
        _preferencesLayoutPanel.Controls.Add(_timeZoneComboBox, 3, 2);
        _preferencesLayoutPanel.Controls.Add(_accountOptionsLabel, 0, 3);
        _preferencesLayoutPanel.Controls.Add(_accountOptionsFlowPanel, 1, 3);
        _preferencesLayoutPanel.Dock = DockStyle.Fill;
        _preferencesLayoutPanel.Location = new Point(7, 65);
        _preferencesLayoutPanel.Margin = new Padding(0);
        _preferencesLayoutPanel.Name = "_preferencesLayoutPanel";
        _preferencesLayoutPanel.RowCount = 5;
        _preferencesLayoutPanel.RowStyles.Add(new RowStyle());
        _preferencesLayoutPanel.RowStyles.Add(new RowStyle());
        _preferencesLayoutPanel.RowStyles.Add(new RowStyle());
        _preferencesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 66.6666641F));
        _preferencesLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
        _preferencesLayoutPanel.Size = new Size(982, 314);
        _preferencesLayoutPanel.TabIndex = 0;
        // 
        // _customerTypeLabel
        // 
        _customerTypeLabel.Anchor = AnchorStyles.Left;
        _customerTypeLabel.AutoSize = true;
        _customerTypeLabel.Location = new Point(0, 16);
        _customerTypeLabel.Margin = new Padding(0);
        _customerTypeLabel.Name = "_customerTypeLabel";
        _customerTypeLabel.Size = new Size(72, 35);
        _customerTypeLabel.TabIndex = 0;
        _customerTypeLabel.Text = "T&ype:";
        // 
        // _customerTypeComboBox
        // 
        _customerTypeComboBox.AccessibleName = "Customer type";
        _customerTypeComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _customerTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _customerTypeComboBox.FormattingEnabled = true;
        _customerTypeComboBox.Location = new Point(146, 5);
        _customerTypeComboBox.Margin = new Padding(5);
        _customerTypeComboBox.Name = "_customerTypeComboBox";
        _customerTypeComboBox.Size = new Size(289, 58);
        _customerTypeComboBox.TabIndex = 1;
        // 
        // _accountStatusLabel
        // 
        _accountStatusLabel.Anchor = AnchorStyles.Left;
        _accountStatusLabel.AutoSize = true;
        _accountStatusLabel.Location = new Point(443, 12);
        _accountStatusLabel.Name = "_accountStatusLabel";
        _accountStatusLabel.Padding = new Padding(4);
        _accountStatusLabel.Size = new Size(94, 43);
        _accountStatusLabel.TabIndex = 2;
        _accountStatusLabel.Text = "&Status:";
        // 
        // _accountStatusComboBox
        // 
        _accountStatusComboBox.AccessibleName = "Account status";
        _accountStatusComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _accountStatusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _accountStatusComboBox.FormattingEnabled = true;
        _accountStatusComboBox.Location = new Point(688, 5);
        _accountStatusComboBox.Margin = new Padding(5);
        _accountStatusComboBox.Name = "_accountStatusComboBox";
        _accountStatusComboBox.Size = new Size(289, 58);
        _accountStatusComboBox.TabIndex = 3;
        // 
        // _creditLimitLabel
        // 
        _creditLimitLabel.Anchor = AnchorStyles.Left;
        _creditLimitLabel.AutoSize = true;
        _creditLimitLabel.Location = new Point(0, 78);
        _creditLimitLabel.Margin = new Padding(0);
        _creditLimitLabel.Name = "_creditLimitLabel";
        _creditLimitLabel.Size = new Size(141, 35);
        _creditLimitLabel.TabIndex = 4;
        _creditLimitLabel.Text = "&Credit limit:";
        // 
        // _creditLimitNumericUpDown
        // 
        _creditLimitNumericUpDown.AccessibleName = "Credit limit";
        _creditLimitNumericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _creditLimitNumericUpDown.AutoSize = true;
        _creditLimitNumericUpDown.DecimalPlaces = 2;
        _creditLimitNumericUpDown.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        _creditLimitNumericUpDown.Location = new Point(146, 73);
        _creditLimitNumericUpDown.Margin = new Padding(5);
        _creditLimitNumericUpDown.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        _creditLimitNumericUpDown.Name = "_creditLimitNumericUpDown";
        _creditLimitNumericUpDown.Size = new Size(289, 46);
        _creditLimitNumericUpDown.TabIndex = 5;
        _creditLimitNumericUpDown.ThousandsSeparator = true;
        _creditLimitNumericUpDown.Value = new decimal(new int[] { 7500, 0, 0, 0 });
        // 
        // _discountLabel
        // 
        _discountLabel.Anchor = AnchorStyles.Left;
        _discountLabel.AutoSize = true;
        _discountLabel.Location = new Point(443, 74);
        _discountLabel.Name = "_discountLabel";
        _discountLabel.Padding = new Padding(4);
        _discountLabel.Size = new Size(237, 43);
        _discountLabel.TabIndex = 6;
        _discountLabel.Text = "Default &discount %:";
        // 
        // _discountNumericUpDown
        // 
        _discountNumericUpDown.AccessibleName = "Default discount percent";
        _discountNumericUpDown.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _discountNumericUpDown.AutoSize = true;
        _discountNumericUpDown.DecimalPlaces = 1;
        _discountNumericUpDown.Location = new Point(688, 73);
        _discountNumericUpDown.Margin = new Padding(5);
        _discountNumericUpDown.Name = "_discountNumericUpDown";
        _discountNumericUpDown.Size = new Size(289, 46);
        _discountNumericUpDown.TabIndex = 7;
        _discountNumericUpDown.Value = new decimal(new int[] { 5, 0, 0, 0 });
        // 
        // _languageLabel
        // 
        _languageLabel.Anchor = AnchorStyles.Left;
        _languageLabel.AutoSize = true;
        _languageLabel.Location = new Point(0, 140);
        _languageLabel.Margin = new Padding(0);
        _languageLabel.Name = "_languageLabel";
        _languageLabel.Size = new Size(129, 35);
        _languageLabel.TabIndex = 8;
        _languageLabel.Text = "&Language:";
        // 
        // _languageComboBox
        // 
        _languageComboBox.AccessibleName = "Preferred language";
        _languageComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _languageComboBox.FormattingEnabled = true;
        _languageComboBox.Location = new Point(146, 129);
        _languageComboBox.Margin = new Padding(5);
        _languageComboBox.Name = "_languageComboBox";
        _languageComboBox.Size = new Size(289, 58);
        _languageComboBox.TabIndex = 9;
        // 
        // _timeZoneLabel
        // 
        _timeZoneLabel.Anchor = AnchorStyles.Left;
        _timeZoneLabel.AutoSize = true;
        _timeZoneLabel.Location = new Point(443, 136);
        _timeZoneLabel.Name = "_timeZoneLabel";
        _timeZoneLabel.Padding = new Padding(4);
        _timeZoneLabel.Size = new Size(142, 43);
        _timeZoneLabel.TabIndex = 10;
        _timeZoneLabel.Text = "Time &zone:";
        // 
        // _timeZoneComboBox
        // 
        _timeZoneComboBox.AccessibleName = "Time zone";
        _timeZoneComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _timeZoneComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _timeZoneComboBox.FormattingEnabled = true;
        _timeZoneComboBox.Location = new Point(688, 129);
        _timeZoneComboBox.Margin = new Padding(5);
        _timeZoneComboBox.Name = "_timeZoneComboBox";
        _timeZoneComboBox.Size = new Size(289, 58);
        _timeZoneComboBox.TabIndex = 11;
        // 
        // _accountOptionsLabel
        // 
        _accountOptionsLabel.Anchor = AnchorStyles.Left;
        _accountOptionsLabel.AutoSize = true;
        _accountOptionsLabel.Location = new Point(0, 215);
        _accountOptionsLabel.Margin = new Padding(0);
        _accountOptionsLabel.Name = "_accountOptionsLabel";
        _accountOptionsLabel.Size = new Size(108, 35);
        _accountOptionsLabel.TabIndex = 12;
        _accountOptionsLabel.Text = "Options:";
        // 
        // _accountOptionsFlowPanel
        // 
        _accountOptionsFlowPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        _accountOptionsFlowPanel.AutoSize = true;
        _preferencesLayoutPanel.SetColumnSpan(_accountOptionsFlowPanel, 3);
        _accountOptionsFlowPanel.Controls.Add(_paperlessCheckBox);
        _accountOptionsFlowPanel.Controls.Add(_priorityCheckBox);
        _accountOptionsFlowPanel.Location = new Point(144, 206);
        _accountOptionsFlowPanel.Name = "_accountOptionsFlowPanel";
        _accountOptionsFlowPanel.Size = new Size(835, 53);
        _accountOptionsFlowPanel.TabIndex = 13;
        _accountOptionsFlowPanel.WrapContents = false;
        // 
        // _paperlessCheckBox
        // 
        _paperlessCheckBox.Appearance = Appearance.ToggleSwitch;
        _paperlessCheckBox.AutoSize = true;
        _paperlessCheckBox.Checked = true;
        _paperlessCheckBox.CheckState = CheckState.Checked;
        _paperlessCheckBox.Location = new Point(5, 5);
        _paperlessCheckBox.Margin = new Padding(5);
        _paperlessCheckBox.Name = "_paperlessCheckBox";
        _paperlessCheckBox.Padding = new Padding(4);
        _paperlessCheckBox.Size = new Size(267, 43);
        _paperlessCheckBox.TabIndex = 0;
        _paperlessCheckBox.Text = "&Paperless billing";
        _paperlessCheckBox.UseVisualStyleBackColor = true;
        // 
        // _priorityCheckBox
        // 
        _priorityCheckBox.Appearance = Appearance.ToggleSwitch;
        _priorityCheckBox.AutoSize = true;
        _priorityCheckBox.Location = new Point(282, 5);
        _priorityCheckBox.Margin = new Padding(5);
        _priorityCheckBox.Name = "_priorityCheckBox";
        _priorityCheckBox.Padding = new Padding(4);
        _priorityCheckBox.Size = new Size(278, 43);
        _priorityCheckBox.TabIndex = 1;
        _priorityCheckBox.Text = "Priority &customer";
        _priorityCheckBox.UseVisualStyleBackColor = true;
        // 
        // _notesGroupBox
        // 
        _contentLayoutPanel.SetColumnSpan(_notesGroupBox, 2);
        _notesGroupBox.Controls.Add(_notesRichTextBox);
        _notesGroupBox.Dock = DockStyle.Fill;
        _notesGroupBox.FlatStyle = FlatStyle.Popup;
        _notesGroupBox.Location = new Point(4, 797);
        _notesGroupBox.Margin = new Padding(4);
        _notesGroupBox.Name = "_notesGroupBox";
        _notesGroupBox.Size = new Size(2004, 278);
        _notesGroupBox.TabIndex = 4;
        _notesGroupBox.TabStop = false;
        _notesGroupBox.Text = "Customer notes";
        // 
        // _notesRichTextBox
        // 
        _notesRichTextBox.AccessibleName = "Customer notes";
        _notesRichTextBox.Controls.Add(_notesToolStrip);
        _notesRichTextBox.Dock = DockStyle.Fill;
        _notesRichTextBox.Location = new Point(7, 65);
        _notesRichTextBox.Name = "_notesRichTextBox";
        _notesRichTextBox.Padding = new Padding(4, 55, 4, 4);
        _notesRichTextBox.Size = new Size(1990, 206);
        _notesRichTextBox.TabIndex = 0;
        _notesRichTextBox.Text = "Prefers email contact. Interested in the premium support plan.";
        _notesRichTextBox.SelectionChanged += NotesRichTextBox_SelectionChanged;
        _notesRichTextBox.PaddingChanged += NotesRichTextBox_LayoutChanged;
        _notesRichTextBox.HandleCreated += NotesRichTextBox_HandleCreated;
        _notesRichTextBox.Resize += NotesRichTextBox_LayoutChanged;
        // 
        // _notesToolStrip
        // 
        _notesToolStrip.BackColor = SystemColors.Control;
        _notesToolStrip.Dock = DockStyle.None;
        _notesToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        _notesToolStrip.ImageScalingSize = new Size(48, 48);
        _notesToolStrip.Items.AddRange(new ToolStripItem[] { _cutToolStripButton, _copyToolStripButton, _pasteToolStripButton, toolStripSeparator1, _boldToolStripButton, _italicToolStripButton, _underlineToolStripButton });
        _notesToolStrip.Location = new Point(0, -56);
        _notesToolStrip.Name = "_notesToolStrip";
        _notesToolStrip.Size = new Size(322, 58);
        _notesToolStrip.TabIndex = 0;
        // 
        // _cutToolStripButton
        // 
        _cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _cutToolStripButton.Image = (Image)resources.GetObject("_cutToolStripButton.Image");
        _cutToolStripButton.ImageTransparentColor = Color.Magenta;
        _cutToolStripButton.Name = "_cutToolStripButton";
        _cutToolStripButton.Size = new Size(52, 52);
        _cutToolStripButton.Text = "toolStripButton1";
        // 
        // _copyToolStripButton
        // 
        _copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _copyToolStripButton.Image = (Image)resources.GetObject("_copyToolStripButton.Image");
        _copyToolStripButton.ImageTransparentColor = Color.Magenta;
        _copyToolStripButton.Name = "_copyToolStripButton";
        _copyToolStripButton.Size = new Size(52, 52);
        _copyToolStripButton.Text = "toolStripButton1";
        // 
        // _pasteToolStripButton
        // 
        _pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _pasteToolStripButton.Image = (Image)resources.GetObject("_pasteToolStripButton.Image");
        _pasteToolStripButton.ImageTransparentColor = Color.Magenta;
        _pasteToolStripButton.Name = "_pasteToolStripButton";
        _pasteToolStripButton.Size = new Size(52, 52);
        _pasteToolStripButton.Text = "toolStripButton1";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(6, 58);
        // 
        // _boldToolStripButton
        // 
        _boldToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _boldToolStripButton.Image = (Image)resources.GetObject("_boldToolStripButton.Image");
        _boldToolStripButton.ImageTransparentColor = Color.Magenta;
        _boldToolStripButton.Name = "_boldToolStripButton";
        _boldToolStripButton.Size = new Size(52, 52);
        _boldToolStripButton.Text = "toolStripButton1";
        // 
        // _italicToolStripButton
        // 
        _italicToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _italicToolStripButton.Image = (Image)resources.GetObject("_italicToolStripButton.Image");
        _italicToolStripButton.ImageTransparentColor = Color.Magenta;
        _italicToolStripButton.Name = "_italicToolStripButton";
        _italicToolStripButton.Size = new Size(52, 52);
        _italicToolStripButton.Text = "toolStripButton2";
        // 
        // _underlineToolStripButton
        // 
        _underlineToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        _underlineToolStripButton.Image = (Image)resources.GetObject("_underlineToolStripButton.Image");
        _underlineToolStripButton.ImageTransparentColor = Color.Magenta;
        _underlineToolStripButton.Name = "_underlineToolStripButton";
        _underlineToolStripButton.Size = new Size(52, 52);
        _underlineToolStripButton.Text = "toolStripButton3";
        // 
        // CustomerEntryView
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        Controls.Add(_contentLayoutPanel);
        Name = "CustomerEntryView";
        Padding = new Padding(15);
        Size = new Size(2042, 1109);
        VisualStylesMode = VisualStylesMode.Latest;
        _contentLayoutPanel.ResumeLayout(false);
        _contentLayoutPanel.PerformLayout();
        _identityGroupBox.ResumeLayout(false);
        _identityGroupBox.PerformLayout();
        _identityLayoutPanel.ResumeLayout(false);
        _identityLayoutPanel.PerformLayout();
        _contactGroupBox.ResumeLayout(false);
        _contactGroupBox.PerformLayout();
        _contactLayoutPanel.ResumeLayout(false);
        _contactLayoutPanel.PerformLayout();
        _contactPermissionsFlowPanel.ResumeLayout(false);
        _contactPermissionsFlowPanel.PerformLayout();
        _addressGroupBox.ResumeLayout(false);
        _addressGroupBox.PerformLayout();
        _addressLayoutPanel.ResumeLayout(false);
        _addressLayoutPanel.PerformLayout();
        _cityZipStateLayoutPanel.ResumeLayout(false);
        _cityZipStateLayoutPanel.PerformLayout();
        _preferencesGroupBox.ResumeLayout(false);
        _preferencesGroupBox.PerformLayout();
        _preferencesLayoutPanel.ResumeLayout(false);
        _preferencesLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_creditLimitNumericUpDown).EndInit();
        ((System.ComponentModel.ISupportInitialize)_discountNumericUpDown).EndInit();
        _accountOptionsFlowPanel.ResumeLayout(false);
        _accountOptionsFlowPanel.PerformLayout();
        _notesGroupBox.ResumeLayout(false);
        _notesRichTextBox.ResumeLayout(false);
        _notesRichTextBox.PerformLayout();
        _notesToolStrip.ResumeLayout(false);
        _notesToolStrip.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel _contentLayoutPanel;
    private GroupBox _identityGroupBox;
    private TableLayoutPanel _identityLayoutPanel;
    private Label _customerNumberLabel;
    private TextBox _customerNumberTextBox;
    private Label _titleLabel;
    private ComboBox _titleComboBox;
    private Label _firstNameLabel;
    private TextBox _firstNameTextBox;
    private Label _middleNameLabel;
    private TextBox _middleNameTextBox;
    private Label _lastNameLabel;
    private TextBox _lastNameTextBox;
    private Label _birthDateLabel;
    private DateTimePicker _birthDatePicker;
    private Label _customerSinceLabel;
    private DateTimePicker _customerSincePicker;
    private Label _activeCustomerLabel;
    private CheckBox _activeCustomerCheckBox;
    private GroupBox _contactGroupBox;
    private TableLayoutPanel _contactLayoutPanel;
    private Label _emailLabel;
    private TextBox _emailTextBox;
    private Label _phoneLabel;
    private MaskedTextBox _phoneMaskedTextBox;
    private Label _mobileLabel;
    private MaskedTextBox _mobileMaskedTextBox;
    private Label _preferredContactLabel;
    private Label _contactPermissionsLabel;
    private FlowLayoutPanel _contactPermissionsFlowPanel;
    private CheckBox _emailPermissionCheckBox;
    private CheckBox _smsPermissionCheckBox;
    private GroupBox _addressGroupBox;
    private TableLayoutPanel _addressLayoutPanel;
    private Label _addressLine1Label;
    private TextBox _addressLine1TextBox;
    private Label _addressLine2Label;
    private TextBox _addressLine2TextBox;
    private Label _streetLabel;
    private TextBox _streetTextBox;
    private Label _cityZipStateLabel;
    private TableLayoutPanel _cityZipStateLayoutPanel;
    private TextBox _cityTextBox;
    private MaskedTextBox _zipMaskedTextBox;
    private TextBox _stateTextBox;
    private Label _countryLabel;
    private ComboBox _countryComboBox;
    private GroupBox _preferencesGroupBox;
    private TableLayoutPanel _preferencesLayoutPanel;
    private Label _customerTypeLabel;
    private ComboBox _customerTypeComboBox;
    private Label _accountStatusLabel;
    private ComboBox _accountStatusComboBox;
    private Label _creditLimitLabel;
    private NumericUpDown _creditLimitNumericUpDown;
    private Label _discountLabel;
    private NumericUpDown _discountNumericUpDown;
    private Label _languageLabel;
    private ComboBox _languageComboBox;
    private Label _timeZoneLabel;
    private ComboBox _timeZoneComboBox;
    private GroupBox _notesGroupBox;
    private RichTextBox _notesRichTextBox;
    private ToolStrip _notesToolStrip;
    private Label _accountOptionsLabel;
    private FlowLayoutPanel _accountOptionsFlowPanel;
    private CheckBox _paperlessCheckBox;
    private CheckBox _priorityCheckBox;
    private TextBox textBox1;
    private ToolStripButton _pasteToolStripButton;
    private ToolStripButton _cutToolStripButton;
    private ToolStripButton _copyToolStripButton;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton _boldToolStripButton;
    private ToolStripButton _italicToolStripButton;
    private ToolStripButton _underlineToolStripButton;
    private ComboBox _preferredContactComboBox;
    private IconFactoryComponent _iconFactoryComponent;
}
