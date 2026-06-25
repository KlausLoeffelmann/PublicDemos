namespace LargeFormSmokeTest.Forms
{
    partial class PersonForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            _root = new System.Windows.Forms.TableLayoutPanel();
            _personalGroup = new System.Windows.Forms.GroupBox();
            _personalTable = new System.Windows.Forms.TableLayoutPanel();
            _capTitle = new System.Windows.Forms.Label();
            _txtTitle = new System.Windows.Forms.TextBox();
            _capFirstName = new System.Windows.Forms.Label();
            _txtFirstName = new System.Windows.Forms.TextBox();
            _capLastName = new System.Windows.Forms.Label();
            _txtLastName = new System.Windows.Forms.TextBox();
            _capMaiden = new System.Windows.Forms.Label();
            _txtMaiden = new System.Windows.Forms.TextBox();
            _capBirthDate = new System.Windows.Forms.Label();
            _dtBirthDate = new System.Windows.Forms.DateTimePicker();
            _capBirthPlace = new System.Windows.Forms.Label();
            _txtBirthPlace = new System.Windows.Forms.TextBox();
            _addressGroup = new System.Windows.Forms.GroupBox();
            _addressTable = new System.Windows.Forms.TableLayoutPanel();
            _capStreet = new System.Windows.Forms.Label();
            _txtStreet = new System.Windows.Forms.TextBox();
            _capHouseNumber = new System.Windows.Forms.Label();
            _txtHouseNumber = new System.Windows.Forms.TextBox();
            _capPostalCode = new System.Windows.Forms.Label();
            _txtPostalCode = new System.Windows.Forms.TextBox();
            _capCity = new System.Windows.Forms.Label();
            _txtCity = new System.Windows.Forms.TextBox();
            _capCountry = new System.Windows.Forms.Label();
            _txtCountry = new System.Windows.Forms.TextBox();
            _parentsGroup = new System.Windows.Forms.GroupBox();
            _parentsTable = new System.Windows.Forms.TableLayoutPanel();
            _capMother = new System.Windows.Forms.Label();
            _txtMother = new System.Windows.Forms.TextBox();
            _capFather = new System.Windows.Forms.Label();
            _txtFather = new System.Windows.Forms.TextBox();
            _buttons = new System.Windows.Forms.FlowLayoutPanel();
            _btnSave = new System.Windows.Forms.Button();
            _btnCancel = new System.Windows.Forms.Button();
            _root.SuspendLayout();
            _personalGroup.SuspendLayout();
            _personalTable.SuspendLayout();
            _addressGroup.SuspendLayout();
            _addressTable.SuspendLayout();
            _parentsGroup.SuspendLayout();
            _parentsTable.SuspendLayout();
            _buttons.SuspendLayout();
            SuspendLayout();
            // 
            // _root
            // 
            _root.ColumnCount = 1;
            _root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _root.Controls.Add(_personalGroup, 0, 0);
            _root.Controls.Add(_addressGroup, 0, 1);
            _root.Controls.Add(_parentsGroup, 0, 2);
            _root.Controls.Add(_buttons, 0, 3);
            _root.Dock = System.Windows.Forms.DockStyle.Fill;
            _root.Name = "_root";
            _root.Padding = new System.Windows.Forms.Padding(10);
            _root.RowCount = 4;
            _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            _root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            // 
            // _personalGroup
            // 
            _personalGroup.Controls.Add(_personalTable);
            _personalGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            _personalGroup.Name = "_personalGroup";
            _personalGroup.Padding = new System.Windows.Forms.Padding(8);
            _personalGroup.Text = "Personal data";
            // 
            // _personalTable
            // 
            _personalTable.ColumnCount = 2;
            _personalTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            _personalTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _personalTable.Controls.Add(_capTitle, 0, 0);
            _personalTable.Controls.Add(_txtTitle, 1, 0);
            _personalTable.Controls.Add(_capFirstName, 0, 1);
            _personalTable.Controls.Add(_txtFirstName, 1, 1);
            _personalTable.Controls.Add(_capLastName, 0, 2);
            _personalTable.Controls.Add(_txtLastName, 1, 2);
            _personalTable.Controls.Add(_capMaiden, 0, 3);
            _personalTable.Controls.Add(_txtMaiden, 1, 3);
            _personalTable.Controls.Add(_capBirthDate, 0, 4);
            _personalTable.Controls.Add(_dtBirthDate, 1, 4);
            _personalTable.Controls.Add(_capBirthPlace, 0, 5);
            _personalTable.Controls.Add(_txtBirthPlace, 1, 5);
            _personalTable.Dock = System.Windows.Forms.DockStyle.Fill;
            _personalTable.Name = "_personalTable";
            _personalTable.RowCount = 6;
            _personalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _personalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _personalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _personalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _personalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _personalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _capTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            _capTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capFirstName.Dock = System.Windows.Forms.DockStyle.Fill;
            _capFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capLastName.Dock = System.Windows.Forms.DockStyle.Fill;
            _capLastName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capMaiden.Dock = System.Windows.Forms.DockStyle.Fill;
            _capMaiden.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capBirthDate.Dock = System.Windows.Forms.DockStyle.Fill;
            _capBirthDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capBirthPlace.Dock = System.Windows.Forms.DockStyle.Fill;
            _capBirthPlace.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _txtTitle.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtFirstName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtLastName.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtMaiden.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtBirthPlace.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _dtBirthDate.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _dtBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            _dtBirthDate.Name = "_dtBirthDate";
            // 
            // _addressGroup
            // 
            _addressGroup.Controls.Add(_addressTable);
            _addressGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            _addressGroup.Name = "_addressGroup";
            _addressGroup.Padding = new System.Windows.Forms.Padding(8);
            _addressGroup.Text = "Current address";
            // 
            // _addressTable
            // 
            _addressTable.ColumnCount = 2;
            _addressTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            _addressTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _addressTable.Controls.Add(_capStreet, 0, 0);
            _addressTable.Controls.Add(_txtStreet, 1, 0);
            _addressTable.Controls.Add(_capHouseNumber, 0, 1);
            _addressTable.Controls.Add(_txtHouseNumber, 1, 1);
            _addressTable.Controls.Add(_capPostalCode, 0, 2);
            _addressTable.Controls.Add(_txtPostalCode, 1, 2);
            _addressTable.Controls.Add(_capCity, 0, 3);
            _addressTable.Controls.Add(_txtCity, 1, 3);
            _addressTable.Controls.Add(_capCountry, 0, 4);
            _addressTable.Controls.Add(_txtCountry, 1, 4);
            _addressTable.Dock = System.Windows.Forms.DockStyle.Fill;
            _addressTable.Name = "_addressTable";
            _addressTable.RowCount = 5;
            _addressTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _addressTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _addressTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _addressTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _addressTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _capStreet.Dock = System.Windows.Forms.DockStyle.Fill;
            _capStreet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capHouseNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            _capHouseNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capPostalCode.Dock = System.Windows.Forms.DockStyle.Fill;
            _capPostalCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capCity.Dock = System.Windows.Forms.DockStyle.Fill;
            _capCity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capCountry.Dock = System.Windows.Forms.DockStyle.Fill;
            _capCountry.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _txtStreet.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtHouseNumber.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtPostalCode.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtCity.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtCountry.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            // 
            // _parentsGroup
            // 
            _parentsGroup.Controls.Add(_parentsTable);
            _parentsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            _parentsGroup.Name = "_parentsGroup";
            _parentsGroup.Padding = new System.Windows.Forms.Padding(8);
            _parentsGroup.Text = "Parents";
            // 
            // _parentsTable
            // 
            _parentsTable.ColumnCount = 2;
            _parentsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            _parentsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _parentsTable.Controls.Add(_capMother, 0, 0);
            _parentsTable.Controls.Add(_txtMother, 1, 0);
            _parentsTable.Controls.Add(_capFather, 0, 1);
            _parentsTable.Controls.Add(_txtFather, 1, 1);
            _parentsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            _parentsTable.Name = "_parentsTable";
            _parentsTable.RowCount = 2;
            _parentsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _parentsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _capMother.Dock = System.Windows.Forms.DockStyle.Fill;
            _capMother.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _capFather.Dock = System.Windows.Forms.DockStyle.Fill;
            _capFather.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _txtMother.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _txtFather.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            // 
            // _buttons
            // 
            _buttons.Controls.Add(_btnCancel);
            _buttons.Controls.Add(_btnSave);
            _buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            _buttons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            _buttons.Name = "_buttons";
            // 
            // _btnSave
            // 
            _btnSave.AutoSize = true;
            _btnSave.MinimumSize = new System.Drawing.Size(100, 30);
            _btnSave.Name = "_btnSave";
            _btnSave.Text = "Save";
            // 
            // _btnCancel
            // 
            _btnCancel.AutoSize = true;
            _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _btnCancel.MinimumSize = new System.Drawing.Size(100, 30);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Text = "Cancel";
            // 
            // PersonForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = _btnCancel;
            ClientSize = new System.Drawing.Size(520, 560);
            Controls.Add(_root);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PersonForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit person";
            _root.ResumeLayout(false);
            _personalGroup.ResumeLayout(false);
            _personalTable.ResumeLayout(false);
            _personalTable.PerformLayout();
            _addressGroup.ResumeLayout(false);
            _addressTable.ResumeLayout(false);
            _addressTable.PerformLayout();
            _parentsGroup.ResumeLayout(false);
            _parentsTable.ResumeLayout(false);
            _parentsTable.PerformLayout();
            _buttons.ResumeLayout(false);
            _buttons.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _root;
        private System.Windows.Forms.GroupBox _personalGroup;
        private System.Windows.Forms.TableLayoutPanel _personalTable;
        private System.Windows.Forms.Label _capTitle;
        private System.Windows.Forms.TextBox _txtTitle;
        private System.Windows.Forms.Label _capFirstName;
        private System.Windows.Forms.TextBox _txtFirstName;
        private System.Windows.Forms.Label _capLastName;
        private System.Windows.Forms.TextBox _txtLastName;
        private System.Windows.Forms.Label _capMaiden;
        private System.Windows.Forms.TextBox _txtMaiden;
        private System.Windows.Forms.Label _capBirthDate;
        private System.Windows.Forms.DateTimePicker _dtBirthDate;
        private System.Windows.Forms.Label _capBirthPlace;
        private System.Windows.Forms.TextBox _txtBirthPlace;
        private System.Windows.Forms.GroupBox _addressGroup;
        private System.Windows.Forms.TableLayoutPanel _addressTable;
        private System.Windows.Forms.Label _capStreet;
        private System.Windows.Forms.TextBox _txtStreet;
        private System.Windows.Forms.Label _capHouseNumber;
        private System.Windows.Forms.TextBox _txtHouseNumber;
        private System.Windows.Forms.Label _capPostalCode;
        private System.Windows.Forms.TextBox _txtPostalCode;
        private System.Windows.Forms.Label _capCity;
        private System.Windows.Forms.TextBox _txtCity;
        private System.Windows.Forms.Label _capCountry;
        private System.Windows.Forms.TextBox _txtCountry;
        private System.Windows.Forms.GroupBox _parentsGroup;
        private System.Windows.Forms.TableLayoutPanel _parentsTable;
        private System.Windows.Forms.Label _capMother;
        private System.Windows.Forms.TextBox _txtMother;
        private System.Windows.Forms.Label _capFather;
        private System.Windows.Forms.TextBox _txtFather;
        private System.Windows.Forms.FlowLayoutPanel _buttons;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnCancel;
    }
}
