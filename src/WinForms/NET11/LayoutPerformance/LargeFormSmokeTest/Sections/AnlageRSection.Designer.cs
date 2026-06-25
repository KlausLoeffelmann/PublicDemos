namespace LargeFormSmokeTest.Sections
{
    partial class AnlageRSection
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
            _inp0 = new System.Windows.Forms.NumericUpDown();
            _lbl1 = new System.Windows.Forms.Label();
            _inp1 = new System.Windows.Forms.NumericUpDown();
            _lbl2 = new System.Windows.Forms.Label();
            _rad2 = new System.Windows.Forms.FlowLayoutPanel();
            _rad2_0 = new System.Windows.Forms.RadioButton();
            _rad2_1 = new System.Windows.Forms.RadioButton();
            _rad2_2 = new System.Windows.Forms.RadioButton();
            _lbl3 = new System.Windows.Forms.Label();
            _inp3 = new System.Windows.Forms.NumericUpDown();
            _lbl4 = new System.Windows.Forms.Label();
            _inp4 = new System.Windows.Forms.NumericUpDown();
            _table.SuspendLayout();
            _groupBox.SuspendLayout();
            _rad2.SuspendLayout();
            SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_inp0).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_inp1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_inp3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_inp4).BeginInit();
            _table.ColumnCount = 2;
            _table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            _table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _table.RowCount = 5;
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _table.Dock = System.Windows.Forms.DockStyle.Top;
            _table.AutoSize = true;
            _table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _table.Padding = new System.Windows.Forms.Padding(4);
            _table.Name = "_table";
            _lbl0.Anchor = System.Windows.Forms.AnchorStyles.Left;
            _lbl0.AutoSize = true;
            _lbl0.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl0.Name = "_lbl0";
            _lbl0.Text = "Rentenbezüge";
            _table.Controls.Add(_lbl0, 0, 0);
            _inp0.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp0.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            _inp0.Name = "_inp0";
            _inp0.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            _inp0.DecimalPlaces = 2;
            _inp0.ThousandsSeparator = true;
            _table.Controls.Add(_inp0, 1, 0);
            _lbl1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            _lbl1.AutoSize = true;
            _lbl1.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl1.Name = "_lbl1";
            _lbl1.Text = "Beginn der Rente (Jahr)";
            _table.Controls.Add(_lbl1, 0, 1);
            _inp1.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            _inp1.Name = "_inp1";
            _inp1.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            _table.Controls.Add(_inp1, 1, 1);
            _lbl2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            _lbl2.AutoSize = true;
            _lbl2.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl2.Name = "_lbl2";
            _lbl2.Text = "Rentenart";
            _table.Controls.Add(_lbl2, 0, 2);
            _rad2.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _rad2.AutoSize = true;
            _rad2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _rad2.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            _rad2.WrapContents = true;
            _rad2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            _rad2.Name = "_rad2";
            _rad2_0.AutoSize = true;
            _rad2_0.Margin = new System.Windows.Forms.Padding(3, 3, 14, 3);
            _rad2_0.Name = "_rad2_0";
            _rad2_0.Text = "gesetzlich";
            _rad2.Controls.Add(_rad2_0);
            _rad2_1.AutoSize = true;
            _rad2_1.Margin = new System.Windows.Forms.Padding(3, 3, 14, 3);
            _rad2_1.Name = "_rad2_1";
            _rad2_1.Text = "betrieblich";
            _rad2.Controls.Add(_rad2_1);
            _rad2_2.AutoSize = true;
            _rad2_2.Margin = new System.Windows.Forms.Padding(3, 3, 14, 3);
            _rad2_2.Name = "_rad2_2";
            _rad2_2.Text = "privat";
            _rad2.Controls.Add(_rad2_2);
            _table.Controls.Add(_rad2, 1, 2);
            _lbl3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            _lbl3.AutoSize = true;
            _lbl3.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl3.Name = "_lbl3";
            _lbl3.Text = "Besteuerungsanteil";
            _table.Controls.Add(_lbl3, 0, 3);
            _inp3.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            _inp3.Name = "_inp3";
            _inp3.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            _inp3.DecimalPlaces = 2;
            _inp3.ThousandsSeparator = true;
            _table.Controls.Add(_inp3, 1, 3);
            _lbl4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            _lbl4.AutoSize = true;
            _lbl4.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            _lbl4.Name = "_lbl4";
            _lbl4.Text = "Anpassungsbetrag";
            _table.Controls.Add(_lbl4, 0, 4);
            _inp4.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _inp4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            _inp4.Name = "_inp4";
            _inp4.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            _inp4.DecimalPlaces = 2;
            _inp4.ThousandsSeparator = true;
            _table.Controls.Add(_inp4, 1, 4);
            _groupBox.Controls.Add(_table);
            _groupBox.Dock = System.Windows.Forms.DockStyle.Top;
            _groupBox.AutoSize = true;
            _groupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _groupBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 12);
            _groupBox.Name = "_groupBox";
            _groupBox.Text = "";
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            Controls.Add(_groupBox);
            Margin = new System.Windows.Forms.Padding(0);
            Name = "AnlageRSection";
            Size = new System.Drawing.Size(720, 320);
            ((System.ComponentModel.ISupportInitialize)_inp0).EndInit();
            ((System.ComponentModel.ISupportInitialize)_inp1).EndInit();
            ((System.ComponentModel.ISupportInitialize)_inp3).EndInit();
            ((System.ComponentModel.ISupportInitialize)_inp4).EndInit();
            _rad2.ResumeLayout(false);
            _rad2.PerformLayout();
            _table.ResumeLayout(false);
            _table.PerformLayout();
            _groupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox _groupBox;
        private System.Windows.Forms.TableLayoutPanel _table;
        private System.Windows.Forms.Label _lbl0;
        private System.Windows.Forms.NumericUpDown _inp0;
        private System.Windows.Forms.Label _lbl1;
        private System.Windows.Forms.NumericUpDown _inp1;
        private System.Windows.Forms.Label _lbl2;
        private System.Windows.Forms.FlowLayoutPanel _rad2;
        private System.Windows.Forms.RadioButton _rad2_0;
        private System.Windows.Forms.RadioButton _rad2_1;
        private System.Windows.Forms.RadioButton _rad2_2;
        private System.Windows.Forms.Label _lbl3;
        private System.Windows.Forms.NumericUpDown _inp3;
        private System.Windows.Forms.Label _lbl4;
        private System.Windows.Forms.NumericUpDown _inp4;
    }
}
