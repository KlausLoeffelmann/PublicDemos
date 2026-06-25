namespace LargeFormSmokeTest.Sections
{
    partial class MantelbogenSection
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
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
            _groupBox = new System.Windows.Forms.GroupBox();
            _table = new System.Windows.Forms.TableLayoutPanel();
            _lbl0 = new System.Windows.Forms.Label();
            _inp0 = new System.Windows.Forms.TextBox();
            _lbl1 = new System.Windows.Forms.Label();
            _inp1 = new System.Windows.Forms.TextBox();
            _lbl2 = new System.Windows.Forms.Label();
            _inp2 = new System.Windows.Forms.NumericUpDown();
            _lbl3 = new System.Windows.Forms.Label();
            _inp3 = new System.Windows.Forms.ComboBox();
            _lbl4 = new System.Windows.Forms.Label();
            _inp4 = new System.Windows.Forms.ComboBox();
            _lbl5 = new System.Windows.Forms.Label();
            _inp5 = new System.Windows.Forms.TextBox();
            _lbl6 = new System.Windows.Forms.Label();
            _inp6 = new System.Windows.Forms.TextBox();
            _lbl7 = new System.Windows.Forms.Label();
            _inp7 = new System.Windows.Forms.TextBox();
            _lbl8 = new System.Windows.Forms.Label();
            _inp8 = new System.Windows.Forms.TextBox();
            _lbl9 = new System.Windows.Forms.Label();
            _inp9 = new System.Windows.Forms.TextBox();
            _lbl10 = new System.Windows.Forms.Label();
            _inp10 = new System.Windows.Forms.DateTimePicker();
            _inp11 = new System.Windows.Forms.CheckBox();
            _table.SuspendLayout();
            _groupBox.SuspendLayout();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_inp2).BeginInit();
            // 
            // _table
            // 
            _table.ColumnCount = 2;
            _table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            _table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _table.RowCount = 13;
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _table.Dock = System.Windows.Forms.DockStyle.Fill;
            _table.Padding = new System.Windows.Forms.Padding(4);
            _table.Name = "_table";
            _lbl0.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl0.AutoEllipsis = true;
            _lbl0.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl0.Name = "_lbl0";
            _lbl0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl0.Text = "Steuernummer";
            _table.Controls.Add(_lbl0, 0, 0);
            _inp0.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp0.Name = "_inp0";
            _table.Controls.Add(_inp0, 1, 0);
            _lbl1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl1.AutoEllipsis = true;
            _lbl1.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl1.Name = "_lbl1";
            _lbl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl1.Text = "Finanzamt";
            _table.Controls.Add(_lbl1, 0, 1);
            _inp1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp1.Name = "_inp1";
            _table.Controls.Add(_inp1, 1, 1);
            _lbl2.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl2.AutoEllipsis = true;
            _lbl2.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl2.Name = "_lbl2";
            _lbl2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl2.Text = "Veranlagungsjahr";
            _table.Controls.Add(_lbl2, 0, 2);
            _inp2.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp2.Name = "_inp2";
            _inp2.DecimalPlaces = 2;
            _inp2.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            _inp2.ThousandsSeparator = true;
            _table.Controls.Add(_inp2, 1, 2);
            _lbl3.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl3.AutoEllipsis = true;
            _lbl3.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl3.Name = "_lbl3";
            _lbl3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl3.Text = "Familienstand";
            _table.Controls.Add(_lbl3, 0, 3);
            _inp3.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp3.Name = "_inp3";
            _inp3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _inp3.Items.AddRange(new object[] { "ledig", "verheiratet", "geschieden", "verwitwet" });
            _table.Controls.Add(_inp3, 1, 3);
            _lbl4.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl4.AutoEllipsis = true;
            _lbl4.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl4.Name = "_lbl4";
            _lbl4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl4.Text = "Religionszugehörigkeit";
            _table.Controls.Add(_lbl4, 0, 4);
            _inp4.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp4.Name = "_inp4";
            _inp4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _inp4.Items.AddRange(new object[] { "keine", "rk", "ev", "sonstige" });
            _table.Controls.Add(_inp4, 1, 4);
            _lbl5.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl5.AutoEllipsis = true;
            _lbl5.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl5.Name = "_lbl5";
            _lbl5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl5.Text = "Identifikationsnummer";
            _table.Controls.Add(_lbl5, 0, 5);
            _inp5.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp5.Name = "_inp5";
            _table.Controls.Add(_inp5, 1, 5);
            _lbl6.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl6.AutoEllipsis = true;
            _lbl6.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl6.Name = "_lbl6";
            _lbl6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl6.Text = "Telefon";
            _table.Controls.Add(_lbl6, 0, 6);
            _inp6.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp6.Name = "_inp6";
            _table.Controls.Add(_inp6, 1, 6);
            _lbl7.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl7.AutoEllipsis = true;
            _lbl7.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl7.Name = "_lbl7";
            _lbl7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl7.Text = "IBAN";
            _table.Controls.Add(_lbl7, 0, 7);
            _inp7.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp7.Name = "_inp7";
            _table.Controls.Add(_inp7, 1, 7);
            _lbl8.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl8.AutoEllipsis = true;
            _lbl8.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl8.Name = "_lbl8";
            _lbl8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl8.Text = "BIC";
            _table.Controls.Add(_lbl8, 0, 8);
            _inp8.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp8.Name = "_inp8";
            _table.Controls.Add(_inp8, 1, 8);
            _lbl9.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl9.AutoEllipsis = true;
            _lbl9.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl9.Name = "_lbl9";
            _lbl9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl9.Text = "Steuerberater";
            _table.Controls.Add(_lbl9, 0, 9);
            _inp9.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp9.Name = "_inp9";
            _table.Controls.Add(_inp9, 1, 9);
            _lbl10.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _lbl10.AutoEllipsis = true;
            _lbl10.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl10.Name = "_lbl10";
            _lbl10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _lbl10.Text = "Einreichungsdatum";
            _table.Controls.Add(_lbl10, 0, 10);
            _inp10.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp10.Name = "_inp10";
            _inp10.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            _table.Controls.Add(_inp10, 1, 10);
            _inp11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            _inp11.AutoSize = true;
            _inp11.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            _inp11.Name = "_inp11";
            _inp11.Text = "Elektronisch übermittelt (ELSTER)";
            _table.Controls.Add(_inp11, 0, 11);
            _table.SetColumnSpan(_inp11, 2);
            // 
            // _groupBox
            // 
            _groupBox.Controls.Add(_table);
            _groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _groupBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
            _groupBox.Name = "_groupBox";
            _groupBox.Text = "";
            // 
            // MantelbogenSection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_groupBox);
            Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            Name = "MantelbogenSection";
            Size = new System.Drawing.Size(720, 428);
            ((System.ComponentModel.ISupportInitialize)_inp2).EndInit();
            _table.ResumeLayout(false);
            _table.PerformLayout();
            _groupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox _groupBox;
        private System.Windows.Forms.TableLayoutPanel _table;
        private System.Windows.Forms.Label _lbl0;
        private System.Windows.Forms.TextBox _inp0;
        private System.Windows.Forms.Label _lbl1;
        private System.Windows.Forms.TextBox _inp1;
        private System.Windows.Forms.Label _lbl2;
        private System.Windows.Forms.NumericUpDown _inp2;
        private System.Windows.Forms.Label _lbl3;
        private System.Windows.Forms.ComboBox _inp3;
        private System.Windows.Forms.Label _lbl4;
        private System.Windows.Forms.ComboBox _inp4;
        private System.Windows.Forms.Label _lbl5;
        private System.Windows.Forms.TextBox _inp5;
        private System.Windows.Forms.Label _lbl6;
        private System.Windows.Forms.TextBox _inp6;
        private System.Windows.Forms.Label _lbl7;
        private System.Windows.Forms.TextBox _inp7;
        private System.Windows.Forms.Label _lbl8;
        private System.Windows.Forms.TextBox _inp8;
        private System.Windows.Forms.Label _lbl9;
        private System.Windows.Forms.TextBox _inp9;
        private System.Windows.Forms.Label _lbl10;
        private System.Windows.Forms.DateTimePicker _inp10;
        private System.Windows.Forms.CheckBox _inp11;
    }
}
