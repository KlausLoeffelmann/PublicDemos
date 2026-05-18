namespace Northwind.App
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support – do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            _menuStrip = new MenuStrip();
            _tsmiFile = new ToolStripMenuItem();
            _tsmiExportCsv = new ToolStripMenuItem();
            _tsmiFileSep1 = new ToolStripSeparator();
            _tsmiQuit = new ToolStripMenuItem();
            _tsmiEdit = new ToolStripMenuItem();
            _tsmiAddCustomer = new ToolStripMenuItem();
            _tsmiEditCustomer = new ToolStripMenuItem();
            _tsmiEditCancel = new ToolStripMenuItem();
            _tsmiEditSep1 = new ToolStripSeparator();
            _tsmiSaveChanges = new ToolStripMenuItem();
            _toolStrip = new ToolStrip();
            _tsbAdd = new ToolStripButton();
            _tsbEdit = new ToolStripButton();
            _tsbCancel = new ToolStripButton();
            _tsbSaveChanges = new ToolStripButton();
            _splitContainer = new SplitContainer();
            _dataGridView = new ThemedDataGridView();
            _pnlDetail = new Panel();
            _grpFields = new GroupBox();
            _pnlFields = new Panel();
            _tlpDetail = new TableLayoutPanel();
            _lblCustomerIdField = new Label();
            _txtCustomerId = new TextBox();
            _lblCompanyNameField = new Label();
            _txtCompanyName = new TextBox();
            _lblContactNameField = new Label();
            _txtContactName = new TextBox();
            _lblContactTitleField = new Label();
            _txtContactTitle = new TextBox();
            _lblAddressField = new Label();
            _txtAddress = new TextBox();
            _lblCityField = new Label();
            _txtCity = new TextBox();
            _lblRegionField = new Label();
            _txtRegion = new TextBox();
            _lblPostalCodeField = new Label();
            _txtPostalCode = new TextBox();
            _lblCountryField = new Label();
            _txtCountry = new TextBox();
            _lblPhoneField = new Label();
            _txtPhone = new TextBox();
            _lblFaxField = new Label();
            _txtFax = new TextBox();
            _pnlHeader = new Panel();
            _lblCustomerHeader = new Label();
            _picCustomer = new PictureBox();
            _statusStrip = new StatusStrip();
            _tsslCustomersLabel = new ToolStripStatusLabel();
            _tsslCustomerCount = new ToolStripStatusLabel();
            _tsslLastChangedLabel = new ToolStripStatusLabel();
            _tsslLastChangedInfo = new ToolStripStatusLabel();
            _tssbSelect = new ToolStripButton();
            _tsslDateTime = new ToolStripStatusLabel();
            _menuStrip.SuspendLayout();
            _toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.Panel2.SuspendLayout();
            _splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_dataGridView).BeginInit();
            _pnlDetail.SuspendLayout();
            _grpFields.SuspendLayout();
            _pnlFields.SuspendLayout();
            _tlpDetail.SuspendLayout();
            _pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_picCustomer).BeginInit();
            _statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _menuStrip
            // 
            _menuStrip.Font = new Font("Segoe UI", 11F);
            _menuStrip.ImageScalingSize = new Size(36, 36);
            _menuStrip.Items.AddRange(new ToolStripItem[] { _tsmiFile, _tsmiEdit });
            _menuStrip.Location = new Point(0, 0);
            _menuStrip.Name = "_menuStrip";
            _menuStrip.Padding = new Padding(5, 2, 0, 2);
            _menuStrip.Size = new Size(1365, 53);
            _menuStrip.TabIndex = 0;
            // 
            // _tsmiFile
            // 
            _tsmiFile.DropDownItems.AddRange(new ToolStripItem[] { _tsmiExportCsv, _tsmiFileSep1, _tsmiQuit });
            _tsmiFile.Name = "_tsmiFile";
            _tsmiFile.Size = new Size(91, 49);
            _tsmiFile.Text = "&File";
            // 
            // _tsmiExportCsv
            // 
            _tsmiExportCsv.Name = "_tsmiExportCsv";
            _tsmiExportCsv.Size = new Size(394, 54);
            _tsmiExportCsv.Text = "Export as CSV...";
            _tsmiExportCsv.Click += TsmiExportCsv_Click;
            // 
            // _tsmiFileSep1
            // 
            _tsmiFileSep1.Name = "_tsmiFileSep1";
            _tsmiFileSep1.Size = new Size(391, 6);
            // 
            // _tsmiQuit
            // 
            _tsmiQuit.Name = "_tsmiQuit";
            _tsmiQuit.Size = new Size(394, 54);
            _tsmiQuit.Text = "&Quit";
            _tsmiQuit.Click += TsmiQuit_Click;
            // 
            // _tsmiEdit
            // 
            _tsmiEdit.DropDownItems.AddRange(new ToolStripItem[] { _tsmiAddCustomer, _tsmiEditCustomer, _tsmiEditCancel, _tsmiEditSep1, _tsmiSaveChanges });
            _tsmiEdit.Name = "_tsmiEdit";
            _tsmiEdit.Size = new Size(97, 49);
            _tsmiEdit.Text = "&Edit";
            // 
            // _tsmiAddCustomer
            // 
            _tsmiAddCustomer.Name = "_tsmiAddCustomer";
            _tsmiAddCustomer.Size = new Size(503, 54);
            _tsmiAddCustomer.Text = "Add new Customer";
            _tsmiAddCustomer.Click += TsbAdd_Click;
            // 
            // _tsmiEditCustomer
            // 
            _tsmiEditCustomer.Name = "_tsmiEditCustomer";
            _tsmiEditCustomer.Size = new Size(503, 54);
            _tsmiEditCustomer.Text = "Edit selected Customer";
            _tsmiEditCustomer.Click += TsbEdit_Click;
            // 
            // _tsmiEditCancel
            // 
            _tsmiEditCancel.Name = "_tsmiEditCancel";
            _tsmiEditCancel.Size = new Size(503, 54);
            _tsmiEditCancel.Text = "Cancel";
            _tsmiEditCancel.Click += TsbCancel_Click;
            // 
            // _tsmiEditSep1
            // 
            _tsmiEditSep1.Name = "_tsmiEditSep1";
            _tsmiEditSep1.Size = new Size(500, 6);
            // 
            // _tsmiSaveChanges
            // 
            _tsmiSaveChanges.Name = "_tsmiSaveChanges";
            _tsmiSaveChanges.Size = new Size(503, 54);
            _tsmiSaveChanges.Text = "Save changes";
            _tsmiSaveChanges.Click += TsbSaveChanges_Click;
            // 
            // _toolStrip
            // 
            _toolStrip.ImageScalingSize = new Size(36, 36);
            _toolStrip.Items.AddRange(new ToolStripItem[] { _tsbAdd, _tsbEdit, _tsbCancel, _tsbSaveChanges });
            _toolStrip.Location = new Point(0, 53);
            _toolStrip.Name = "_toolStrip";
            _toolStrip.Size = new Size(1365, 47);
            _toolStrip.TabIndex = 1;
            // 
            // _tsbAdd
            // 
            _tsbAdd.AccessibleName = "Add new Customer";
            _tsbAdd.Name = "_tsbAdd";
            _tsbAdd.Size = new Size(70, 41);
            _tsbAdd.Text = "Add";
            _tsbAdd.TextImageRelation = TextImageRelation.ImageAboveText;
            _tsbAdd.Click += TsbAdd_Click;
            // 
            // _tsbEdit
            // 
            _tsbEdit.AccessibleName = "Edit selected Customer";
            _tsbEdit.Name = "_tsbEdit";
            _tsbEdit.Size = new Size(67, 41);
            _tsbEdit.Text = "Edit";
            _tsbEdit.TextImageRelation = TextImageRelation.ImageAboveText;
            _tsbEdit.Click += TsbEdit_Click;
            // 
            // _tsbCancel
            // 
            _tsbCancel.AccessibleName = "Cancel editing";
            _tsbCancel.Name = "_tsbCancel";
            _tsbCancel.Size = new Size(100, 41);
            _tsbCancel.Text = "Cancel";
            _tsbCancel.TextImageRelation = TextImageRelation.ImageAboveText;
            _tsbCancel.Click += TsbCancel_Click;
            // 
            // _tsbSaveChanges
            // 
            _tsbSaveChanges.AccessibleName = "Save changes";
            _tsbSaveChanges.Name = "_tsbSaveChanges";
            _tsbSaveChanges.Size = new Size(180, 41);
            _tsbSaveChanges.Text = "Save changes";
            _tsbSaveChanges.TextImageRelation = TextImageRelation.ImageAboveText;
            _tsbSaveChanges.Click += TsbSaveChanges_Click;
            // 
            // _splitContainer
            // 
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Font = new Font("Segoe UI", 10F);
            _splitContainer.Location = new Point(0, 100);
            _splitContainer.Name = "_splitContainer";
            _splitContainer.Orientation = Orientation.Horizontal;
            // 
            // _splitContainer.Panel1
            // 
            _splitContainer.Panel1.Controls.Add(_dataGridView);
            // 
            // _splitContainer.Panel2
            // 
            _splitContainer.Panel2.Controls.Add(_pnlDetail);
            _splitContainer.Size = new Size(1365, 740);
            _splitContainer.SplitterDistance = 525;
            _splitContainer.TabIndex = 2;
            // 
            // _dataGridView
            // 
            _dataGridView.AllowUserToAddRows = false;
            _dataGridView.AllowUserToDeleteRows = false;
            _dataGridView.BackgroundColor = SystemColors.Window;
            _dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            _dataGridView.ColumnHeadersHeight = 52;
            _dataGridView.Dock = DockStyle.Fill;
            _dataGridView.GridColor = SystemColors.ControlDark;
            _dataGridView.Location = new Point(0, 0);
            _dataGridView.Name = "_dataGridView";
            _dataGridView.ReadOnly = true;
            _dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            _dataGridView.RowHeadersVisible = false;
            _dataGridView.RowHeadersWidth = 92;
            _dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dataGridView.Size = new Size(1365, 525);
            _dataGridView.TabIndex = 0;
            _dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            // 
            // _pnlDetail
            // 
            _pnlDetail.Controls.Add(_grpFields);
            _pnlDetail.Controls.Add(_pnlHeader);
            _pnlDetail.Dock = DockStyle.Fill;
            _pnlDetail.Location = new Point(0, 0);
            _pnlDetail.Name = "_pnlDetail";
            _pnlDetail.Padding = new Padding(7, 5, 7, 5);
            _pnlDetail.Size = new Size(1365, 211);
            _pnlDetail.TabIndex = 0;
            // 
            // _grpFields
            // 
            _grpFields.Controls.Add(_pnlFields);
            _grpFields.Dock = DockStyle.Fill;
            _grpFields.Location = new Point(7, 221);
            _grpFields.Name = "_grpFields";
            _grpFields.Padding = new Padding(8, 6, 8, 8);
            _grpFields.Size = new Size(1351, 0);
            _grpFields.TabIndex = 1;
            _grpFields.TabStop = false;
            _grpFields.Text = "Customer details";
            // 
            // _pnlFields
            // 
            _pnlFields.AutoScroll = true;
            _pnlFields.Controls.Add(_tlpDetail);
            _pnlFields.Dock = DockStyle.Fill;
            _pnlFields.Location = new Point(8, 46);
            _pnlFields.Name = "_pnlFields";
            _pnlFields.Size = new Size(1335, 0);
            _pnlFields.TabIndex = 0;
            // 
            // _tlpDetail
            // 
            _tlpDetail.AutoSize = true;
            _tlpDetail.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpDetail.ColumnCount = 2;
            _tlpDetail.ColumnStyles.Add(new ColumnStyle());
            _tlpDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _tlpDetail.Controls.Add(_lblCustomerIdField, 0, 0);
            _tlpDetail.Controls.Add(_txtCustomerId, 1, 0);
            _tlpDetail.Controls.Add(_lblCompanyNameField, 0, 1);
            _tlpDetail.Controls.Add(_txtCompanyName, 1, 1);
            _tlpDetail.Controls.Add(_lblContactNameField, 0, 2);
            _tlpDetail.Controls.Add(_txtContactName, 1, 2);
            _tlpDetail.Controls.Add(_lblContactTitleField, 0, 3);
            _tlpDetail.Controls.Add(_txtContactTitle, 1, 3);
            _tlpDetail.Controls.Add(_lblAddressField, 0, 4);
            _tlpDetail.Controls.Add(_txtAddress, 1, 4);
            _tlpDetail.Controls.Add(_lblCityField, 0, 5);
            _tlpDetail.Controls.Add(_txtCity, 1, 5);
            _tlpDetail.Controls.Add(_lblRegionField, 0, 6);
            _tlpDetail.Controls.Add(_txtRegion, 1, 6);
            _tlpDetail.Controls.Add(_lblPostalCodeField, 0, 7);
            _tlpDetail.Controls.Add(_txtPostalCode, 1, 7);
            _tlpDetail.Controls.Add(_lblCountryField, 0, 8);
            _tlpDetail.Controls.Add(_txtCountry, 1, 8);
            _tlpDetail.Controls.Add(_lblPhoneField, 0, 9);
            _tlpDetail.Controls.Add(_txtPhone, 1, 9);
            _tlpDetail.Controls.Add(_lblFaxField, 0, 10);
            _tlpDetail.Controls.Add(_txtFax, 1, 10);
            _tlpDetail.Dock = DockStyle.Top;
            _tlpDetail.Location = new Point(0, 0);
            _tlpDetail.Name = "_tlpDetail";
            _tlpDetail.RowCount = 11;
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.RowStyles.Add(new RowStyle());
            _tlpDetail.Size = new Size(1335, 737);
            _tlpDetail.TabIndex = 0;
            // 
            // _lblCustomerIdField
            // 
            _lblCustomerIdField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblCustomerIdField.AutoSize = true;
            _lblCustomerIdField.Location = new Point(3, 16);
            _lblCustomerIdField.Margin = new Padding(3, 16, 12, 10);
            _lblCustomerIdField.Name = "_lblCustomerIdField";
            _lblCustomerIdField.Size = new Size(240, 41);
            _lblCustomerIdField.TabIndex = 1;
            _lblCustomerIdField.Text = "Customer ID:";
            // 
            // _txtCustomerId
            // 
            _txtCustomerId.Dock = DockStyle.Fill;
            _txtCustomerId.Location = new Point(258, 3);
            _txtCustomerId.MaxLength = 5;
            _txtCustomerId.Name = "_txtCustomerId";
            _txtCustomerId.Size = new Size(1074, 47);
            _txtCustomerId.TabIndex = 0;
            _txtCustomerId.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblCompanyNameField
            // 
            _lblCompanyNameField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblCompanyNameField.AutoSize = true;
            _lblCompanyNameField.Location = new Point(3, 83);
            _lblCompanyNameField.Margin = new Padding(3, 16, 12, 10);
            _lblCompanyNameField.Name = "_lblCompanyNameField";
            _lblCompanyNameField.Size = new Size(240, 41);
            _lblCompanyNameField.TabIndex = 2;
            _lblCompanyNameField.Text = "Company Name:";
            // 
            // _txtCompanyName
            // 
            _txtCompanyName.Dock = DockStyle.Fill;
            _txtCompanyName.Location = new Point(258, 70);
            _txtCompanyName.MaxLength = 40;
            _txtCompanyName.Name = "_txtCompanyName";
            _txtCompanyName.Size = new Size(1074, 47);
            _txtCompanyName.TabIndex = 1;
            _txtCompanyName.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblContactNameField
            // 
            _lblContactNameField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblContactNameField.AutoSize = true;
            _lblContactNameField.Location = new Point(3, 150);
            _lblContactNameField.Margin = new Padding(3, 16, 12, 10);
            _lblContactNameField.Name = "_lblContactNameField";
            _lblContactNameField.Size = new Size(240, 41);
            _lblContactNameField.TabIndex = 3;
            _lblContactNameField.Text = "Contact Name:";
            // 
            // _txtContactName
            // 
            _txtContactName.Dock = DockStyle.Fill;
            _txtContactName.Location = new Point(258, 137);
            _txtContactName.MaxLength = 30;
            _txtContactName.Name = "_txtContactName";
            _txtContactName.Size = new Size(1074, 47);
            _txtContactName.TabIndex = 2;
            _txtContactName.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblContactTitleField
            // 
            _lblContactTitleField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblContactTitleField.AutoSize = true;
            _lblContactTitleField.Location = new Point(3, 217);
            _lblContactTitleField.Margin = new Padding(3, 16, 12, 10);
            _lblContactTitleField.Name = "_lblContactTitleField";
            _lblContactTitleField.Size = new Size(240, 41);
            _lblContactTitleField.TabIndex = 4;
            _lblContactTitleField.Text = "Contact Title:";
            // 
            // _txtContactTitle
            // 
            _txtContactTitle.Dock = DockStyle.Fill;
            _txtContactTitle.Location = new Point(258, 204);
            _txtContactTitle.MaxLength = 30;
            _txtContactTitle.Name = "_txtContactTitle";
            _txtContactTitle.Size = new Size(1074, 47);
            _txtContactTitle.TabIndex = 3;
            _txtContactTitle.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblAddressField
            // 
            _lblAddressField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblAddressField.AutoSize = true;
            _lblAddressField.Location = new Point(3, 284);
            _lblAddressField.Margin = new Padding(3, 16, 12, 10);
            _lblAddressField.Name = "_lblAddressField";
            _lblAddressField.Size = new Size(240, 41);
            _lblAddressField.TabIndex = 5;
            _lblAddressField.Text = "Address:";
            // 
            // _txtAddress
            // 
            _txtAddress.Dock = DockStyle.Fill;
            _txtAddress.Location = new Point(258, 271);
            _txtAddress.MaxLength = 60;
            _txtAddress.Name = "_txtAddress";
            _txtAddress.Size = new Size(1074, 47);
            _txtAddress.TabIndex = 4;
            _txtAddress.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblCityField
            // 
            _lblCityField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblCityField.AutoSize = true;
            _lblCityField.Location = new Point(3, 351);
            _lblCityField.Margin = new Padding(3, 16, 12, 10);
            _lblCityField.Name = "_lblCityField";
            _lblCityField.Size = new Size(240, 41);
            _lblCityField.TabIndex = 6;
            _lblCityField.Text = "City:";
            // 
            // _txtCity
            // 
            _txtCity.Dock = DockStyle.Fill;
            _txtCity.Location = new Point(258, 338);
            _txtCity.MaxLength = 15;
            _txtCity.Name = "_txtCity";
            _txtCity.Size = new Size(1074, 47);
            _txtCity.TabIndex = 5;
            _txtCity.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblRegionField
            // 
            _lblRegionField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblRegionField.AutoSize = true;
            _lblRegionField.Location = new Point(3, 418);
            _lblRegionField.Margin = new Padding(3, 16, 12, 10);
            _lblRegionField.Name = "_lblRegionField";
            _lblRegionField.Size = new Size(240, 41);
            _lblRegionField.TabIndex = 7;
            _lblRegionField.Text = "Region:";
            // 
            // _txtRegion
            // 
            _txtRegion.Dock = DockStyle.Fill;
            _txtRegion.Location = new Point(258, 405);
            _txtRegion.MaxLength = 15;
            _txtRegion.Name = "_txtRegion";
            _txtRegion.Size = new Size(1074, 47);
            _txtRegion.TabIndex = 6;
            _txtRegion.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblPostalCodeField
            // 
            _lblPostalCodeField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblPostalCodeField.AutoSize = true;
            _lblPostalCodeField.Location = new Point(3, 485);
            _lblPostalCodeField.Margin = new Padding(3, 16, 12, 10);
            _lblPostalCodeField.Name = "_lblPostalCodeField";
            _lblPostalCodeField.Size = new Size(240, 41);
            _lblPostalCodeField.TabIndex = 8;
            _lblPostalCodeField.Text = "Postal Code:";
            // 
            // _txtPostalCode
            // 
            _txtPostalCode.Dock = DockStyle.Fill;
            _txtPostalCode.Location = new Point(258, 472);
            _txtPostalCode.MaxLength = 10;
            _txtPostalCode.Name = "_txtPostalCode";
            _txtPostalCode.Size = new Size(1074, 47);
            _txtPostalCode.TabIndex = 7;
            _txtPostalCode.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblCountryField
            // 
            _lblCountryField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblCountryField.AutoSize = true;
            _lblCountryField.Location = new Point(3, 552);
            _lblCountryField.Margin = new Padding(3, 16, 12, 10);
            _lblCountryField.Name = "_lblCountryField";
            _lblCountryField.Size = new Size(240, 41);
            _lblCountryField.TabIndex = 9;
            _lblCountryField.Text = "Country:";
            // 
            // _txtCountry
            // 
            _txtCountry.Dock = DockStyle.Fill;
            _txtCountry.Location = new Point(258, 539);
            _txtCountry.MaxLength = 15;
            _txtCountry.Name = "_txtCountry";
            _txtCountry.Size = new Size(1074, 47);
            _txtCountry.TabIndex = 8;
            _txtCountry.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblPhoneField
            // 
            _lblPhoneField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblPhoneField.AutoSize = true;
            _lblPhoneField.Location = new Point(3, 619);
            _lblPhoneField.Margin = new Padding(3, 16, 12, 10);
            _lblPhoneField.Name = "_lblPhoneField";
            _lblPhoneField.Size = new Size(240, 41);
            _lblPhoneField.TabIndex = 10;
            _lblPhoneField.Text = "Phone:";
            // 
            // _txtPhone
            // 
            _txtPhone.Dock = DockStyle.Fill;
            _txtPhone.Location = new Point(258, 606);
            _txtPhone.MaxLength = 24;
            _txtPhone.Name = "_txtPhone";
            _txtPhone.Size = new Size(1074, 47);
            _txtPhone.TabIndex = 9;
            _txtPhone.TextChanged += DetailTextBox_TextChanged;
            // 
            // _lblFaxField
            // 
            _lblFaxField.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _lblFaxField.AutoSize = true;
            _lblFaxField.Location = new Point(3, 686);
            _lblFaxField.Margin = new Padding(3, 16, 12, 10);
            _lblFaxField.Name = "_lblFaxField";
            _lblFaxField.Size = new Size(240, 41);
            _lblFaxField.TabIndex = 11;
            _lblFaxField.Text = "Fax:";
            // 
            // _txtFax
            // 
            _txtFax.Dock = DockStyle.Fill;
            _txtFax.Location = new Point(258, 673);
            _txtFax.MaxLength = 24;
            _txtFax.Name = "_txtFax";
            _txtFax.Size = new Size(1074, 47);
            _txtFax.TabIndex = 10;
            _txtFax.TextChanged += DetailTextBox_TextChanged;
            // 
            // _pnlHeader
            // 
            _pnlHeader.Controls.Add(_lblCustomerHeader);
            _pnlHeader.Controls.Add(_picCustomer);
            _pnlHeader.Dock = DockStyle.Top;
            _pnlHeader.Location = new Point(7, 5);
            _pnlHeader.Name = "_pnlHeader";
            _pnlHeader.Padding = new Padding(0, 0, 0, 12);
            _pnlHeader.Size = new Size(1351, 216);
            _pnlHeader.TabIndex = 0;
            // 
            // _lblCustomerHeader
            // 
            _lblCustomerHeader.Dock = DockStyle.Fill;
            _lblCustomerHeader.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
            _lblCustomerHeader.Location = new Point(200, 0);
            _lblCustomerHeader.Margin = new Padding(0);
            _lblCustomerHeader.Name = "_lblCustomerHeader";
            _lblCustomerHeader.Padding = new Padding(18, 0, 8, 0);
            _lblCustomerHeader.Size = new Size(1151, 204);
            _lblCustomerHeader.TabIndex = 1;
            _lblCustomerHeader.Text = "(none)";
            _lblCustomerHeader.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _picCustomer
            // 
            _picCustomer.Dock = DockStyle.Left;
            _picCustomer.Location = new Point(0, 0);
            _picCustomer.Margin = new Padding(0);
            _picCustomer.Name = "_picCustomer";
            _picCustomer.Size = new Size(200, 204);
            _picCustomer.SizeMode = PictureBoxSizeMode.Zoom;
            _picCustomer.TabIndex = 0;
            _picCustomer.TabStop = false;
            // 
            // _statusStrip
            // 
            _statusStrip.Font = new Font("Segoe UI", 11F);
            _statusStrip.ImageScalingSize = new Size(36, 36);
            _statusStrip.Items.AddRange(new ToolStripItem[] { _tsslCustomersLabel, _tsslCustomerCount, _tsslLastChangedLabel, _tsslLastChangedInfo, _tssbSelect, _tsslDateTime });
            _statusStrip.Location = new Point(0, 840);
            _statusStrip.Name = "_statusStrip";
            _statusStrip.Padding = new Padding(1, 0, 12, 0);
            _statusStrip.Size = new Size(1365, 60);
            _statusStrip.TabIndex = 3;
            // 
            // _tsslCustomersLabel
            // 
            _tsslCustomersLabel.Name = "_tsslCustomersLabel";
            _tsslCustomersLabel.Size = new Size(180, 49);
            _tsslCustomersLabel.Text = "Customers:";
            // 
            // _tsslCustomerCount
            // 
            _tsslCustomerCount.Name = "_tsslCustomerCount";
            _tsslCustomerCount.Size = new Size(38, 49);
            _tsslCustomerCount.Text = "0";
            // 
            // _tsslLastChangedLabel
            // 
            _tsslLastChangedLabel.BorderSides = ToolStripStatusLabelBorderSides.Left;
            _tsslLastChangedLabel.Name = "_tsslLastChangedLabel";
            _tsslLastChangedLabel.Size = new Size(371, 49);
            _tsslLastChangedLabel.Text = "Last changed Customer:";
            // 
            // _tsslLastChangedInfo
            // 
            _tsslLastChangedInfo.Name = "_tsslLastChangedInfo";
            _tsslLastChangedInfo.Size = new Size(114, 49);
            _tsslLastChangedInfo.Text = "(none)";
            // 
            // _tssbSelect
            // 
            _tssbSelect.AccessibleName = "Select last changed customer in grid";
            _tssbSelect.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _tssbSelect.Name = "_tssbSelect";
            _tssbSelect.Size = new Size(110, 56);
            _tssbSelect.Text = "Select";
            _tssbSelect.Click += TssbSelect_Click;
            // 
            // _tsslDateTime
            // 
            _tsslDateTime.Name = "_tsslDateTime";
            _tsslDateTime.Size = new Size(539, 49);
            _tsslDateTime.Spring = true;
            _tsslDateTime.TextAlign = ContentAlignment.MiddleRight;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1365, 900);
            Controls.Add(_splitContainer);
            Controls.Add(_toolStrip);
            Controls.Add(_menuStrip);
            Controls.Add(_statusStrip);
            MainMenuStrip = _menuStrip;
            MinimumSize = new Size(709, 549);
            Name = "FrmMain";
            Text = "Northwind Customer Editor";
            _menuStrip.ResumeLayout(false);
            _menuStrip.PerformLayout();
            _toolStrip.ResumeLayout(false);
            _toolStrip.PerformLayout();
            _splitContainer.Panel1.ResumeLayout(false);
            _splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
            _splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_dataGridView).EndInit();
            _pnlDetail.ResumeLayout(false);
            _grpFields.ResumeLayout(false);
            _pnlFields.ResumeLayout(false);
            _pnlFields.PerformLayout();
            _tlpDetail.ResumeLayout(false);
            _tlpDetail.PerformLayout();
            _pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_picCustomer).EndInit();
            _statusStrip.ResumeLayout(false);
            _statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // ── 7. Backing fields ───────────────────────────────────────────────────

        private MenuStrip _menuStrip;
        private ToolStripMenuItem _tsmiFile;
        private ToolStripMenuItem _tsmiExportCsv;
        private ToolStripSeparator _tsmiFileSep1;
        private ToolStripMenuItem _tsmiQuit;
        private ToolStripMenuItem _tsmiEdit;
        private ToolStripMenuItem _tsmiAddCustomer;
        private ToolStripMenuItem _tsmiEditCustomer;
        private ToolStripMenuItem _tsmiEditCancel;
        private ToolStripSeparator _tsmiEditSep1;
        private ToolStripMenuItem _tsmiSaveChanges;

        private ToolStrip _toolStrip;
        private ToolStripButton _tsbAdd;
        private ToolStripButton _tsbEdit;
        private ToolStripButton _tsbCancel;
        private ToolStripButton _tsbSaveChanges;

        private SplitContainer _splitContainer;
        private ThemedDataGridView _dataGridView;

        private Panel _pnlDetail;
        private Panel _pnlHeader;
        private PictureBox _picCustomer;
        private GroupBox _grpFields;
        private Panel _pnlFields;
        private TableLayoutPanel _tlpDetail;
        private Label _lblCustomerHeader;
        private Label _lblCustomerIdField;
        private Label _lblCompanyNameField;
        private Label _lblContactNameField;
        private Label _lblContactTitleField;
        private Label _lblAddressField;
        private Label _lblCityField;
        private Label _lblRegionField;
        private Label _lblPostalCodeField;
        private Label _lblCountryField;
        private Label _lblPhoneField;
        private Label _lblFaxField;

        private TextBox _txtCustomerId;
        private TextBox _txtCompanyName;
        private TextBox _txtContactName;
        private TextBox _txtContactTitle;
        private TextBox _txtAddress;
        private TextBox _txtCity;
        private TextBox _txtRegion;
        private TextBox _txtPostalCode;
        private TextBox _txtCountry;
        private TextBox _txtPhone;
        private TextBox _txtFax;

        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _tsslCustomersLabel;
        private ToolStripStatusLabel _tsslCustomerCount;
        private ToolStripStatusLabel _tsslLastChangedLabel;
        private ToolStripStatusLabel _tsslLastChangedInfo;
        private ToolStripButton _tssbSelect;
        private ToolStripStatusLabel _tsslDateTime;
    }
}
