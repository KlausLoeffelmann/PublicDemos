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
            _tlpDialogResultButtons.SuspendLayout();
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
            _btnOK.Size = new Size(117, 51);
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
            _btnCancel.Location = new Point(130, 5);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Padding = new Padding(14, 0, 14, 0);
            _btnCancel.Size = new Size(117, 51);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            // 
            // _tlpDialogResultButtons
            // 
            _tlpDialogResultButtons.AutoSize = true;
            _tlpDialogResultButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlpDialogResultButtons.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            _tlpDialogResultButtons.ColumnCount = 2;
            _tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _tlpDialogResultButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _tlpDialogResultButtons.Controls.Add(_btnCancel, 1, 0);
            _tlpDialogResultButtons.Controls.Add(_btnOK, 0, 0);
            _tlpDialogResultButtons.Location = new Point(733, 513);
            _tlpDialogResultButtons.Name = "_tlpDialogResultButtons";
            _tlpDialogResultButtons.RowCount = 1;
            _tlpDialogResultButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpDialogResultButtons.Size = new Size(252, 61);
            _tlpDialogResultButtons.TabIndex = 3;
            // 
            // ScratchView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_tlpDialogResultButtons);
            Margin = new Padding(2);
            Name = "ScratchView";
            Size = new Size(1013, 601);
            VisualStylesMode = VisualStylesMode.Net11;
            _tlpDialogResultButtons.ResumeLayout(false);
            _tlpDialogResultButtons.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button _btnOK;
        private Button _btnCancel;
        private TableLayoutPanel _tlpDialogResultButtons;
    }
}
