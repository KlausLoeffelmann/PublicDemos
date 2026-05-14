namespace Northwind.App
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private MenuStrip menuStripMain;
        private ToolStripMenuItem menuItemFile;
        private ToolStripMenuItem menuItemExportCsv;
        private ToolStripMenuItem menuItemQuit;
        private ToolStripMenuItem menuItemEdit;
        private ToolStripMenuItem menuItemAdd;
        private ToolStripMenuItem menuItemEditSelected;
        private ToolStripMenuItem menuItemCancel;
        private ToolStripMenuItem menuItemSave;
        private ToolStrip toolStripMain;
        private ToolStripButton toolStripButtonAdd;
        private ToolStripButton toolStripButtonEdit;
        private ToolStripButton toolStripButtonCancel;
        private ToolStripButton toolStripButtonSave;
        private SplitContainer splitContainerMain;
        private DataGridView dataGridCustomers;
        private Panel panelDetails;
        private Label labelHeader;
        private TableLayoutPanel tableLayoutDetails;
        private Label labelCustomerId;
        private Label labelCompanyName;
        private Label labelContactName;
        private Label labelContactTitle;
        private Label labelAddress;
        private Label labelCity;
        private Label labelRegion;
        private Label labelPostalCode;
        private Label labelCountry;
        private Label labelPhone;
        private Label labelFax;
        private TextBox textBoxCustomerId;
        private TextBox textBoxCompanyName;
        private TextBox textBoxContactName;
        private TextBox textBoxContactTitle;
        private TextBox textBoxAddress;
        private TextBox textBoxCity;
        private TextBox textBoxRegion;
        private TextBox textBoxPostalCode;
        private TextBox textBoxCountry;
        private TextBox textBoxPhone;
        private TextBox textBoxFax;
        private StatusStrip statusStripMain;
        private ToolStripStatusLabel statusLabelCustomers;
        private ToolStripStatusLabel statusLabelCustomerCount;
        private ToolStripStatusLabel statusLabelLastChanged;
        private ToolStripStatusLabel statusLabelLastChangedValue;
        private ToolStripStatusLabel statusLabelSpring;
        private ToolStripStatusLabel statusLabelDateTime;
        private ToolStripStatusLabel statusLabelSpacer;
        private ToolStripSplitButton statusButtonSelect;

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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStripMain = new MenuStrip();
            menuItemFile = new ToolStripMenuItem();
            menuItemExportCsv = new ToolStripMenuItem();
            menuItemQuit = new ToolStripMenuItem();
            menuItemEdit = new ToolStripMenuItem();
            menuItemAdd = new ToolStripMenuItem();
            menuItemEditSelected = new ToolStripMenuItem();
            menuItemCancel = new ToolStripMenuItem();
            menuItemSave = new ToolStripMenuItem();
            toolStripMain = new ToolStrip();
            toolStripButtonAdd = new ToolStripButton();
            toolStripButtonEdit = new ToolStripButton();
            toolStripButtonCancel = new ToolStripButton();
            toolStripButtonSave = new ToolStripButton();
            splitContainerMain = new SplitContainer();
            dataGridCustomers = new DataGridView();
            panelDetails = new Panel();
            tableLayoutDetails = new TableLayoutPanel();
            labelCustomerId = new Label();
            textBoxCustomerId = new TextBox();
            labelCompanyName = new Label();
            textBoxCompanyName = new TextBox();
            labelContactName = new Label();
            textBoxContactName = new TextBox();
            labelContactTitle = new Label();
            textBoxContactTitle = new TextBox();
            labelAddress = new Label();
            textBoxAddress = new TextBox();
            labelCity = new Label();
            textBoxCity = new TextBox();
            labelRegion = new Label();
            textBoxRegion = new TextBox();
            labelPostalCode = new Label();
            textBoxPostalCode = new TextBox();
            labelCountry = new Label();
            textBoxCountry = new TextBox();
            labelPhone = new Label();
            textBoxPhone = new TextBox();
            labelFax = new Label();
            textBoxFax = new TextBox();
            labelHeader = new Label();
            statusStripMain = new StatusStrip();
            statusLabelCustomers = new ToolStripStatusLabel();
            statusLabelCustomerCount = new ToolStripStatusLabel();
            statusLabelLastChanged = new ToolStripStatusLabel();
            statusLabelLastChangedValue = new ToolStripStatusLabel();
            statusLabelSpacer = new ToolStripStatusLabel();
            statusButtonSelect = new ToolStripSplitButton();
            statusLabelSpring = new ToolStripStatusLabel();
            statusLabelDateTime = new ToolStripStatusLabel();
            menuStripMain.SuspendLayout();
            toolStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridCustomers).BeginInit();
            panelDetails.SuspendLayout();
            tableLayoutDetails.SuspendLayout();
            statusStripMain.SuspendLayout();
            SuspendLayout();
            // 
            // menuStripMain
            // 
            menuStripMain.Font = new Font("Segoe UI", 11F);
            menuStripMain.ImageScalingSize = new Size(36, 36);
            menuStripMain.Items.AddRange(new ToolStripItem[] { menuItemFile, menuItemEdit });
            menuStripMain.Location = new Point(0, 0);
            menuStripMain.Name = "menuStripMain";
            menuStripMain.Padding = new Padding(5, 2, 0, 2);
            menuStripMain.Size = new Size(1445, 53);
            menuStripMain.TabIndex = 0;
            menuStripMain.Text = "menuStripMain";
            // 
            // menuItemFile
            // 
            menuItemFile.DropDownItems.AddRange(new ToolStripItem[] { menuItemExportCsv, menuItemQuit });
            menuItemFile.Name = "menuItemFile";
            menuItemFile.Size = new Size(91, 49);
            menuItemFile.Text = "File";
            // 
            // menuItemExportCsv
            // 
            menuItemExportCsv.Name = "menuItemExportCsv";
            menuItemExportCsv.Size = new Size(394, 54);
            menuItemExportCsv.Text = "Export as CSV...";
            menuItemExportCsv.Click += MenuItemExportCsv_Click;
            // 
            // menuItemQuit
            // 
            menuItemQuit.Name = "menuItemQuit";
            menuItemQuit.Size = new Size(394, 54);
            menuItemQuit.Text = "Quit";
            menuItemQuit.Click += MenuItemQuit_Click;
            // 
            // menuItemEdit
            // 
            menuItemEdit.DropDownItems.AddRange(new ToolStripItem[] { menuItemAdd, menuItemEditSelected, menuItemCancel, menuItemSave });
            menuItemEdit.Name = "menuItemEdit";
            menuItemEdit.Size = new Size(97, 49);
            menuItemEdit.Text = "Edit";
            // 
            // menuItemAdd
            // 
            menuItemAdd.Name = "menuItemAdd";
            menuItemAdd.Size = new Size(503, 54);
            menuItemAdd.Text = "Add new Customer";
            menuItemAdd.Click += MenuItemAdd_Click;
            // 
            // menuItemEditSelected
            // 
            menuItemEditSelected.Name = "menuItemEditSelected";
            menuItemEditSelected.Size = new Size(503, 54);
            menuItemEditSelected.Text = "Edit selected Customer";
            menuItemEditSelected.Click += MenuItemEditSelected_Click;
            // 
            // menuItemCancel
            // 
            menuItemCancel.Name = "menuItemCancel";
            menuItemCancel.Size = new Size(503, 54);
            menuItemCancel.Text = "Cancel";
            menuItemCancel.Click += MenuItemCancel_Click;
            // 
            // menuItemSave
            // 
            menuItemSave.Name = "menuItemSave";
            menuItemSave.Size = new Size(503, 54);
            menuItemSave.Text = "Save changes";
            menuItemSave.Click += MenuItemSave_Click;
            // 
            // toolStripMain
            // 
            toolStripMain.ImageScalingSize = new Size(36, 36);
            toolStripMain.Items.AddRange(new ToolStripItem[] { toolStripButtonAdd, toolStripButtonEdit, toolStripButtonCancel, toolStripButtonSave });
            toolStripMain.Location = new Point(0, 53);
            toolStripMain.Name = "toolStripMain";
            toolStripMain.Size = new Size(1445, 47);
            toolStripMain.TabIndex = 1;
            toolStripMain.Text = "toolStripMain";
            toolStripMain.ItemClicked += ToolStripMain_ItemClicked;
            // 
            // toolStripButtonAdd
            // 
            toolStripButtonAdd.Image = ImageFactory.CreateAddIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonAdd.Name = "toolStripButtonAdd";
            toolStripButtonAdd.Size = new Size(70, 41);
            toolStripButtonAdd.Text = "Add";
            toolStripButtonAdd.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonAdd.Click += ToolStripButtonAdd_Click;
            // 
            // toolStripButtonEdit
            // 
            toolStripButtonEdit.Image = ImageFactory.CreateEditIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonEdit.Name = "toolStripButtonEdit";
            toolStripButtonEdit.Size = new Size(67, 41);
            toolStripButtonEdit.Text = "Edit";
            toolStripButtonEdit.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonEdit.Click += ToolStripButtonEdit_Click;
            // 
            // toolStripButtonCancel
            // 
            toolStripButtonCancel.Image = ImageFactory.CreateCancelIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonCancel.Name = "toolStripButtonCancel";
            toolStripButtonCancel.Size = new Size(100, 41);
            toolStripButtonCancel.Text = "Cancel";
            toolStripButtonCancel.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonCancel.Click += ToolStripButtonCancel_Click;
            // 
            // toolStripButtonSave
            // 
            toolStripButtonSave.Image = ImageFactory.CreateSaveIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonSave.Name = "toolStripButtonSave";
            toolStripButtonSave.Size = new Size(180, 41);
            toolStripButtonSave.Text = "Save changes";
            toolStripButtonSave.TextImageRelation = TextImageRelation.ImageAboveText;
            toolStripButtonSave.Click += ToolStripButtonSave_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Font = new Font("Segoe UI", 10F);
            splitContainerMain.Location = new Point(0, 100);
            splitContainerMain.Name = "splitContainerMain";
            splitContainerMain.Orientation = Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(dataGridCustomers);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(panelDetails);
            splitContainerMain.Size = new Size(1445, 749);
            splitContainerMain.SplitterDistance = 365;
            splitContainerMain.TabIndex = 2;
            // 
            // dataGridCustomers
            // 
            dataGridCustomers.AllowUserToAddRows = false;
            dataGridCustomers.AllowUserToDeleteRows = false;
            dataGridCustomers.AllowUserToResizeRows = false;
            dataGridCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridCustomers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridCustomers.Dock = DockStyle.Fill;
            dataGridCustomers.Location = new Point(0, 0);
            dataGridCustomers.Name = "dataGridCustomers";
            dataGridCustomers.ReadOnly = true;
            dataGridCustomers.RowHeadersVisible = false;
            dataGridCustomers.RowHeadersWidth = 92;
            dataGridCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridCustomers.Size = new Size(1445, 365);
            dataGridCustomers.TabIndex = 0;
            dataGridCustomers.SelectionChanged += DataGridCustomers_SelectionChanged;
            // 
            // panelDetails
            // 
            panelDetails.Controls.Add(tableLayoutDetails);
            panelDetails.Controls.Add(labelHeader);
            panelDetails.Dock = DockStyle.Fill;
            panelDetails.Location = new Point(0, 0);
            panelDetails.Name = "panelDetails";
            panelDetails.Padding = new Padding(11);
            panelDetails.Size = new Size(1445, 380);
            panelDetails.TabIndex = 0;
            // 
            // tableLayoutDetails
            // 
            tableLayoutDetails.AutoSize = true;
            tableLayoutDetails.ColumnCount = 2;
            tableLayoutDetails.ColumnStyles.Add(new ColumnStyle());
            tableLayoutDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutDetails.Controls.Add(labelCustomerId, 0, 0);
            tableLayoutDetails.Controls.Add(textBoxCustomerId, 1, 0);
            tableLayoutDetails.Controls.Add(labelCompanyName, 0, 1);
            tableLayoutDetails.Controls.Add(textBoxCompanyName, 1, 1);
            tableLayoutDetails.Controls.Add(labelContactName, 0, 2);
            tableLayoutDetails.Controls.Add(textBoxContactName, 1, 2);
            tableLayoutDetails.Controls.Add(labelContactTitle, 0, 3);
            tableLayoutDetails.Controls.Add(textBoxContactTitle, 1, 3);
            tableLayoutDetails.Controls.Add(labelAddress, 0, 4);
            tableLayoutDetails.Controls.Add(textBoxAddress, 1, 4);
            tableLayoutDetails.Controls.Add(labelCity, 0, 5);
            tableLayoutDetails.Controls.Add(textBoxCity, 1, 5);
            tableLayoutDetails.Controls.Add(labelRegion, 0, 6);
            tableLayoutDetails.Controls.Add(textBoxRegion, 1, 6);
            tableLayoutDetails.Controls.Add(labelPostalCode, 0, 7);
            tableLayoutDetails.Controls.Add(textBoxPostalCode, 1, 7);
            tableLayoutDetails.Controls.Add(labelCountry, 0, 8);
            tableLayoutDetails.Controls.Add(textBoxCountry, 1, 8);
            tableLayoutDetails.Controls.Add(labelPhone, 0, 9);
            tableLayoutDetails.Controls.Add(textBoxPhone, 1, 9);
            tableLayoutDetails.Controls.Add(labelFax, 0, 10);
            tableLayoutDetails.Controls.Add(textBoxFax, 1, 10);
            tableLayoutDetails.Dock = DockStyle.Top;
            tableLayoutDetails.Location = new Point(11, 68);
            tableLayoutDetails.Name = "tableLayoutDetails";
            tableLayoutDetails.RowCount = 11;
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            tableLayoutDetails.Size = new Size(1423, 319);
            tableLayoutDetails.TabIndex = 1;
            // 
            // labelCustomerId
            // 
            labelCustomerId.AutoSize = true;
            labelCustomerId.Location = new Point(3, 0);
            labelCustomerId.Name = "labelCustomerId";
            labelCustomerId.Size = new Size(184, 29);
            labelCustomerId.TabIndex = 0;
            labelCustomerId.Text = "Customer ID";
            // 
            // textBoxCustomerId
            // 
            textBoxCustomerId.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxCustomerId.Location = new Point(242, 3);
            textBoxCustomerId.Name = "textBoxCustomerId";
            textBoxCustomerId.Size = new Size(1178, 47);
            textBoxCustomerId.TabIndex = 1;
            textBoxCustomerId.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelCompanyName
            // 
            labelCompanyName.AutoSize = true;
            labelCompanyName.Location = new Point(3, 29);
            labelCompanyName.Name = "labelCompanyName";
            labelCompanyName.Size = new Size(233, 29);
            labelCompanyName.TabIndex = 2;
            labelCompanyName.Text = "Company Name";
            // 
            // textBoxCompanyName
            // 
            textBoxCompanyName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxCompanyName.Location = new Point(242, 32);
            textBoxCompanyName.Name = "textBoxCompanyName";
            textBoxCompanyName.Size = new Size(1178, 47);
            textBoxCompanyName.TabIndex = 3;
            textBoxCompanyName.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelContactName
            // 
            labelContactName.AutoSize = true;
            labelContactName.Location = new Point(3, 58);
            labelContactName.Name = "labelContactName";
            labelContactName.Size = new Size(208, 29);
            labelContactName.TabIndex = 4;
            labelContactName.Text = "Contact Name";
            // 
            // textBoxContactName
            // 
            textBoxContactName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxContactName.Location = new Point(242, 61);
            textBoxContactName.Name = "textBoxContactName";
            textBoxContactName.Size = new Size(1178, 47);
            textBoxContactName.TabIndex = 5;
            textBoxContactName.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelContactTitle
            // 
            labelContactTitle.AutoSize = true;
            labelContactTitle.Location = new Point(3, 87);
            labelContactTitle.Name = "labelContactTitle";
            labelContactTitle.Size = new Size(185, 29);
            labelContactTitle.TabIndex = 6;
            labelContactTitle.Text = "Contact Title";
            // 
            // textBoxContactTitle
            // 
            textBoxContactTitle.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxContactTitle.Location = new Point(242, 90);
            textBoxContactTitle.Name = "textBoxContactTitle";
            textBoxContactTitle.Size = new Size(1178, 47);
            textBoxContactTitle.TabIndex = 7;
            textBoxContactTitle.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelAddress
            // 
            labelAddress.AutoSize = true;
            labelAddress.Location = new Point(3, 116);
            labelAddress.Name = "labelAddress";
            labelAddress.Size = new Size(125, 29);
            labelAddress.TabIndex = 8;
            labelAddress.Text = "Address";
            // 
            // textBoxAddress
            // 
            textBoxAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxAddress.Location = new Point(242, 119);
            textBoxAddress.Name = "textBoxAddress";
            textBoxAddress.Size = new Size(1178, 47);
            textBoxAddress.TabIndex = 9;
            textBoxAddress.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelCity
            // 
            labelCity.AutoSize = true;
            labelCity.Location = new Point(3, 145);
            labelCity.Name = "labelCity";
            labelCity.Size = new Size(69, 29);
            labelCity.TabIndex = 10;
            labelCity.Text = "City";
            // 
            // textBoxCity
            // 
            textBoxCity.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxCity.Location = new Point(242, 148);
            textBoxCity.Name = "textBoxCity";
            textBoxCity.Size = new Size(1178, 47);
            textBoxCity.TabIndex = 11;
            textBoxCity.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelRegion
            // 
            labelRegion.AutoSize = true;
            labelRegion.Location = new Point(3, 174);
            labelRegion.Name = "labelRegion";
            labelRegion.Size = new Size(111, 29);
            labelRegion.TabIndex = 12;
            labelRegion.Text = "Region";
            // 
            // textBoxRegion
            // 
            textBoxRegion.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxRegion.Location = new Point(242, 177);
            textBoxRegion.Name = "textBoxRegion";
            textBoxRegion.Size = new Size(1178, 47);
            textBoxRegion.TabIndex = 13;
            textBoxRegion.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelPostalCode
            // 
            labelPostalCode.AutoSize = true;
            labelPostalCode.Location = new Point(3, 203);
            labelPostalCode.Name = "labelPostalCode";
            labelPostalCode.Size = new Size(176, 29);
            labelPostalCode.TabIndex = 14;
            labelPostalCode.Text = "Postal Code";
            // 
            // textBoxPostalCode
            // 
            textBoxPostalCode.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxPostalCode.Location = new Point(242, 206);
            textBoxPostalCode.Name = "textBoxPostalCode";
            textBoxPostalCode.Size = new Size(1178, 47);
            textBoxPostalCode.TabIndex = 15;
            textBoxPostalCode.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelCountry
            // 
            labelCountry.AutoSize = true;
            labelCountry.Location = new Point(3, 232);
            labelCountry.Name = "labelCountry";
            labelCountry.Size = new Size(124, 29);
            labelCountry.TabIndex = 16;
            labelCountry.Text = "Country";
            // 
            // textBoxCountry
            // 
            textBoxCountry.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxCountry.Location = new Point(242, 235);
            textBoxCountry.Name = "textBoxCountry";
            textBoxCountry.Size = new Size(1178, 47);
            textBoxCountry.TabIndex = 17;
            textBoxCountry.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelPhone
            // 
            labelPhone.AutoSize = true;
            labelPhone.Location = new Point(3, 261);
            labelPhone.Name = "labelPhone";
            labelPhone.Size = new Size(103, 29);
            labelPhone.TabIndex = 18;
            labelPhone.Text = "Phone";
            // 
            // textBoxPhone
            // 
            textBoxPhone.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxPhone.Location = new Point(242, 264);
            textBoxPhone.Name = "textBoxPhone";
            textBoxPhone.Size = new Size(1178, 47);
            textBoxPhone.TabIndex = 19;
            textBoxPhone.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelFax
            // 
            labelFax.AutoSize = true;
            labelFax.Location = new Point(3, 290);
            labelFax.Name = "labelFax";
            labelFax.Size = new Size(61, 29);
            labelFax.TabIndex = 20;
            labelFax.Text = "Fax";
            // 
            // textBoxFax
            // 
            textBoxFax.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBoxFax.Location = new Point(242, 293);
            textBoxFax.Name = "textBoxFax";
            textBoxFax.Size = new Size(1178, 47);
            textBoxFax.TabIndex = 21;
            textBoxFax.TextChanged += DetailTextBox_TextChanged;
            // 
            // labelHeader
            // 
            labelHeader.AutoSize = true;
            labelHeader.Dock = DockStyle.Top;
            labelHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            labelHeader.Location = new Point(11, 11);
            labelHeader.Name = "labelHeader";
            labelHeader.Size = new Size(550, 57);
            labelHeader.TabIndex = 0;
            labelHeader.Text = "{CustomerID} - {Company}";
            // 
            // statusStripMain
            // 
            statusStripMain.Font = new Font("Segoe UI", 11F);
            statusStripMain.ImageScalingSize = new Size(36, 36);
            statusStripMain.Items.AddRange(new ToolStripItem[] { statusLabelCustomers, statusLabelCustomerCount, statusLabelLastChanged, statusLabelLastChangedValue, statusLabelSpacer, statusButtonSelect, statusLabelSpring, statusLabelDateTime });
            statusStripMain.Location = new Point(0, 849);
            statusStripMain.Name = "statusStripMain";
            statusStripMain.Padding = new Padding(1, 0, 12, 0);
            statusStripMain.Size = new Size(1445, 56);
            statusStripMain.TabIndex = 3;
            statusStripMain.Text = "statusStripMain";
            // 
            // statusLabelCustomers
            // 
            statusLabelCustomers.Name = "statusLabelCustomers";
            statusLabelCustomers.Size = new Size(180, 45);
            statusLabelCustomers.Text = "Customers:";
            // 
            // statusLabelCustomerCount
            // 
            statusLabelCustomerCount.Name = "statusLabelCustomerCount";
            statusLabelCustomerCount.Size = new Size(38, 45);
            statusLabelCustomerCount.Text = "0";
            // 
            // statusLabelLastChanged
            // 
            statusLabelLastChanged.Name = "statusLabelLastChanged";
            statusLabelLastChanged.Size = new Size(367, 45);
            statusLabelLastChanged.Text = "Last changed Customer:";
            // 
            // statusLabelLastChangedValue
            // 
            statusLabelLastChangedValue.Name = "statusLabelLastChangedValue";
            statusLabelLastChangedValue.Size = new Size(313, 45);
            statusLabelLastChangedValue.Text = "{id} {name} {contact}";
            // 
            // statusLabelSpacer
            // 
            statusLabelSpacer.Name = "statusLabelSpacer";
            statusLabelSpacer.Size = new Size(0, 45);
            // 
            // statusButtonSelect
            // 
            statusButtonSelect.DisplayStyle = ToolStripItemDisplayStyle.Text;
            statusButtonSelect.Name = "statusButtonSelect";
            statusButtonSelect.Size = new Size(136, 52);
            statusButtonSelect.Text = "Select";
            statusButtonSelect.ButtonClick += StatusButtonSelect_ButtonClick;
            // 
            // statusLabelSpring
            // 
            statusLabelSpring.Name = "statusLabelSpring";
            statusLabelSpring.Size = new Size(191, 45);
            statusLabelSpring.Spring = true;
            // 
            // statusLabelDateTime
            // 
            statusLabelDateTime.Name = "statusLabelDateTime";
            statusLabelDateTime.Size = new Size(207, 45);
            statusLabelDateTime.Text = "{Date} {Time}";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1445, 905);
            Controls.Add(splitContainerMain);
            Controls.Add(statusStripMain);
            Controls.Add(toolStripMain);
            Controls.Add(menuStripMain);
            MainMenuStrip = menuStripMain;
            Name = "FrmMain";
            Text = "Northwind Customers";
            Load += FrmMain_Load;
            menuStripMain.ResumeLayout(false);
            menuStripMain.PerformLayout();
            toolStripMain.ResumeLayout(false);
            toolStripMain.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridCustomers).EndInit();
            panelDetails.ResumeLayout(false);
            panelDetails.PerformLayout();
            tableLayoutDetails.ResumeLayout(false);
            tableLayoutDetails.PerformLayout();
            statusStripMain.ResumeLayout(false);
            statusStripMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
