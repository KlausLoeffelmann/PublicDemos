using VisualStylesModeDemo.Controls;

namespace VisualStylesModeDemo.Views
{
    partial class ScratchView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _btnOK = new Button();
            _btnCancel = new Button();
            _tlpDialogResultButtons = new TableLayoutPanel();
            _contactGroupBox = new GroupBox();
            _contactLayoutPanel = new TableLayoutPanel();
            _emailLabel = new Label();
            _emailTextBox = new TextBox();
            _phoneLabel = new Label();
            _phoneMaskedTextBox = new MaskedTextBox();
            _mobileLabel = new Label();
            _mobileMaskedTextBox = new MaskedTextBox();
            _preferredContactLabel = new Label();
            _preferredContactComboBox = new ComboBox();
            _contactPermissionsLabel = new Label();
            _contactPermissionsFlowPanel = new FlowLayoutPanel();
            _emailPermissionCheckBox = new CheckBox();
            _smsPermissionCheckBox = new CheckBox();
            comboBox1 = new ComboBox();
            comboBox3 = new ComboBox();
            comboBox4 = new ComboBox();
            comboBox2 = new ComboBox();
            _tlpDialogResultButtons.SuspendLayout();
            _contactGroupBox.SuspendLayout();
            _contactLayoutPanel.SuspendLayout();
            _contactPermissionsFlowPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _btnOK
            // 
            _btnOK.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _btnOK.AutoSize = true;
            _btnOK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _btnOK.DialogResult = DialogResult.OK;
            _btnOK.Location = new Point(5, 5);
            _btnOK.Name = "_btnOK";
            _btnOK.Padding = new Padding(14, 0, 14, 0);
            _btnOK.Size = new Size(101, 35);
            _btnOK.TabIndex = 0;
            _btnOK.Text = "OK";
            _btnOK.UseVisualStyleBackColor = true;
            // 
            // _btnCancel
            // 
            _btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _btnCancel.AutoSize = true;
            _btnCancel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _btnCancel.DialogResult = DialogResult.OK;
            _btnCancel.Location = new Point(114, 5);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Padding = new Padding(14, 0, 14, 0);
            _btnCancel.Size = new Size(101, 35);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            // 
            // _tlpDialogResultButtons
            // 
            _tlpDialogResultButtons.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _tlpDialogResultButtons.AutoSize = true;
            _tlpDialogResultButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpDialogResultButtons.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            _tlpDialogResultButtons.ColumnCount = 2;
            _tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _tlpDialogResultButtons.Controls.Add(_btnCancel, 1, 0);
            _tlpDialogResultButtons.Controls.Add(_btnOK, 0, 0);
            _tlpDialogResultButtons.Location = new Point(891, 768);
            _tlpDialogResultButtons.Name = "_tlpDialogResultButtons";
            _tlpDialogResultButtons.RowCount = 1;
            _tlpDialogResultButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpDialogResultButtons.Size = new Size(220, 45);
            _tlpDialogResultButtons.TabIndex = 3;
            // 
            // _contactGroupBox
            // 
            _contactGroupBox.AutoSize = true;
            _contactGroupBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _contactGroupBox.Controls.Add(_contactLayoutPanel);
            _contactGroupBox.Location = new Point(9, 15);
            _contactGroupBox.Margin = new Padding(0);
            _contactGroupBox.Name = "_contactGroupBox";
            _contactGroupBox.Padding = new Padding(0);
            _contactGroupBox.Size = new Size(832, 235);
            _contactGroupBox.TabIndex = 4;
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
            _contactLayoutPanel.Controls.Add(_preferredContactComboBox, 1, 3);
            _contactLayoutPanel.Controls.Add(_contactPermissionsLabel, 0, 4);
            _contactLayoutPanel.Controls.Add(_contactPermissionsFlowPanel, 1, 4);
            _contactLayoutPanel.Dock = DockStyle.Fill;
            _contactLayoutPanel.Location = new Point(0, 24);
            _contactLayoutPanel.Margin = new Padding(0);
            _contactLayoutPanel.Name = "_contactLayoutPanel";
            _contactLayoutPanel.RowCount = 5;
            _contactLayoutPanel.RowStyles.Add(new RowStyle());
            _contactLayoutPanel.RowStyles.Add(new RowStyle());
            _contactLayoutPanel.RowStyles.Add(new RowStyle());
            _contactLayoutPanel.RowStyles.Add(new RowStyle());
            _contactLayoutPanel.RowStyles.Add(new RowStyle());
            _contactLayoutPanel.Size = new Size(832, 211);
            _contactLayoutPanel.TabIndex = 0;
            // 
            // _emailLabel
            // 
            _emailLabel.Anchor = AnchorStyles.Left;
            _emailLabel.AutoSize = true;
            _emailLabel.Location = new Point(3, 8);
            _emailLabel.Name = "_emailLabel";
            _emailLabel.Size = new Size(58, 25);
            _emailLabel.TabIndex = 0;
            _emailLabel.Text = "&Email:";
            // 
            // _emailTextBox
            // 
            _emailTextBox.AccessibleName = "Email address";
            _emailTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _emailTextBox.Location = new Point(162, 5);
            _emailTextBox.Margin = new Padding(5);
            _emailTextBox.Name = "_emailTextBox";
            _emailTextBox.Padding = new Padding(4);
            _emailTextBox.Size = new Size(665, 31);
            _emailTextBox.TabIndex = 1;
            _emailTextBox.Text = "alex.morgan@example.com";
            // 
            // _phoneLabel
            // 
            _phoneLabel.Anchor = AnchorStyles.Left;
            _phoneLabel.AutoSize = true;
            _phoneLabel.Location = new Point(3, 49);
            _phoneLabel.Name = "_phoneLabel";
            _phoneLabel.Size = new Size(66, 25);
            _phoneLabel.TabIndex = 2;
            _phoneLabel.Text = "&Phone:";
            // 
            // _phoneMaskedTextBox
            // 
            _phoneMaskedTextBox.AccessibleName = "Phone number";
            _phoneMaskedTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _phoneMaskedTextBox.Location = new Point(162, 46);
            _phoneMaskedTextBox.Margin = new Padding(5);
            _phoneMaskedTextBox.Mask = "(999) 000-0000";
            _phoneMaskedTextBox.Name = "_phoneMaskedTextBox";
            _phoneMaskedTextBox.Padding = new Padding(4);
            _phoneMaskedTextBox.Size = new Size(665, 31);
            _phoneMaskedTextBox.TabIndex = 3;
            _phoneMaskedTextBox.Text = "2065550142";
            // 
            // _mobileLabel
            // 
            _mobileLabel.Anchor = AnchorStyles.Left;
            _mobileLabel.AutoSize = true;
            _mobileLabel.Location = new Point(3, 90);
            _mobileLabel.Name = "_mobileLabel";
            _mobileLabel.Size = new Size(71, 25);
            _mobileLabel.TabIndex = 4;
            _mobileLabel.Text = "&Mobile:";
            // 
            // _mobileMaskedTextBox
            // 
            _mobileMaskedTextBox.AccessibleName = "Mobile phone number";
            _mobileMaskedTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _mobileMaskedTextBox.Location = new Point(162, 87);
            _mobileMaskedTextBox.Margin = new Padding(5);
            _mobileMaskedTextBox.Mask = "(999) 000-0000";
            _mobileMaskedTextBox.Name = "_mobileMaskedTextBox";
            _mobileMaskedTextBox.Padding = new Padding(4);
            _mobileMaskedTextBox.Size = new Size(665, 31);
            _mobileMaskedTextBox.TabIndex = 5;
            _mobileMaskedTextBox.Text = "2065550188";
            // 
            // _preferredContactLabel
            // 
            _preferredContactLabel.Anchor = AnchorStyles.Left;
            _preferredContactLabel.AutoSize = true;
            _preferredContactLabel.Location = new Point(3, 130);
            _preferredContactLabel.Name = "_preferredContactLabel";
            _preferredContactLabel.Size = new Size(151, 25);
            _preferredContactLabel.TabIndex = 6;
            _preferredContactLabel.Text = "Preferred &contact:";
            // 
            // _preferredContactComboBox
            // 
            _preferredContactComboBox.AccessibleName = "Preferred contact method";
            _preferredContactComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _preferredContactComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _preferredContactComboBox.FormattingEnabled = true;
            _preferredContactComboBox.Location = new Point(160, 126);
            _preferredContactComboBox.Name = "_preferredContactComboBox";
            _preferredContactComboBox.Size = new Size(669, 33);
            _preferredContactComboBox.TabIndex = 7;
            // 
            // _contactPermissionsLabel
            // 
            _contactPermissionsLabel.Anchor = AnchorStyles.Left;
            _contactPermissionsLabel.AutoSize = true;
            _contactPermissionsLabel.Location = new Point(3, 174);
            _contactPermissionsLabel.Name = "_contactPermissionsLabel";
            _contactPermissionsLabel.Size = new Size(109, 25);
            _contactPermissionsLabel.TabIndex = 8;
            _contactPermissionsLabel.Text = "Permissions:";
            // 
            // _contactPermissionsFlowPanel
            // 
            _contactPermissionsFlowPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _contactPermissionsFlowPanel.AutoSize = true;
            _contactPermissionsFlowPanel.Controls.Add(_emailPermissionCheckBox);
            _contactPermissionsFlowPanel.Controls.Add(_smsPermissionCheckBox);
            _contactPermissionsFlowPanel.Location = new Point(160, 165);
            _contactPermissionsFlowPanel.Name = "_contactPermissionsFlowPanel";
            _contactPermissionsFlowPanel.Size = new Size(669, 43);
            _contactPermissionsFlowPanel.TabIndex = 9;
            _contactPermissionsFlowPanel.WrapContents = false;
            // 
            // _emailPermissionCheckBox
            // 
            _emailPermissionCheckBox.Appearance = Appearance.ToggleSwitch;
            _emailPermissionCheckBox.AutoSize = true;
            _emailPermissionCheckBox.Checked = true;
            _emailPermissionCheckBox.CheckState = CheckState.Checked;
            _emailPermissionCheckBox.Location = new Point(3, 3);
            _emailPermissionCheckBox.Name = "_emailPermissionCheckBox";
            _emailPermissionCheckBox.Padding = new Padding(4);
            _emailPermissionCheckBox.Size = new Size(137, 37);
            _emailPermissionCheckBox.TabIndex = 0;
            _emailPermissionCheckBox.Text = "Allow &email";
            _emailPermissionCheckBox.UseVisualStyleBackColor = true;
            // 
            // _smsPermissionCheckBox
            // 
            _smsPermissionCheckBox.Appearance = Appearance.ToggleSwitch;
            _smsPermissionCheckBox.AutoSize = true;
            _smsPermissionCheckBox.Location = new Point(146, 3);
            _smsPermissionCheckBox.Name = "_smsPermissionCheckBox";
            _smsPermissionCheckBox.Padding = new Padding(4);
            _smsPermissionCheckBox.Size = new Size(131, 37);
            _smsPermissionCheckBox.TabIndex = 1;
            _smsPermissionCheckBox.Text = "Allow &SMS";
            _smsPermissionCheckBox.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.AccessibleName = "Preferred contact method";
            comboBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(181, 286);
            comboBox1.Margin = new Padding(10);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(582, 33);
            comboBox1.TabIndex = 8;
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(258, 427);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(317, 33);
            comboBox3.TabIndex = 10;
            // 
            // comboBox4
            // 
            comboBox4.FlatStyle = FlatStyle.Popup;
            comboBox4.FormattingEnabled = true;
            comboBox4.Location = new Point(258, 477);
            comboBox4.Name = "comboBox4";
            comboBox4.Size = new Size(317, 33);
            comboBox4.TabIndex = 11;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(326, 626);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(397, 44);
            comboBox2.TabIndex = 12;
            comboBox2.VisualStylesMode = VisualStylesMode.Net11;
            // 
            // ScratchView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(comboBox2);
            Controls.Add(comboBox4);
            Controls.Add(comboBox3);
            Controls.Add(comboBox1);
            Controls.Add(_contactGroupBox);
            Controls.Add(_tlpDialogResultButtons);
            Margin = new Padding(2);
            Name = "ScratchView";
            Size = new Size(1128, 832);
            Load += ScratchView_Load;
            _tlpDialogResultButtons.ResumeLayout(false);
            _tlpDialogResultButtons.PerformLayout();
            _contactGroupBox.ResumeLayout(false);
            _contactGroupBox.PerformLayout();
            _contactLayoutPanel.ResumeLayout(false);
            _contactLayoutPanel.PerformLayout();
            _contactPermissionsFlowPanel.ResumeLayout(false);
            _contactPermissionsFlowPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button _btnOK;
        private Button _btnCancel;
        private TableLayoutPanel _tlpDialogResultButtons;
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
        private ComboBox _preferredContactComboBox;
        private ComboBox comboBox1;
        private ComboBox comboBox3;
        private ComboBox comboBox4;
        private ComboBox comboBox2;
    }
}
