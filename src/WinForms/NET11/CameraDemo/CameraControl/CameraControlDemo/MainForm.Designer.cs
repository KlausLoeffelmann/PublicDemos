namespace CameraControlDemo
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _rootLayout = new TableLayoutPanel();
            _controlBar = new FlowLayoutPanel();
            _cameraLabel = new Label();
            _cameraComboBox = new ComboBox();
            _formatLabel = new Label();
            _formatComboBox = new ComboBox();
            _keepAspectRatioCheckBox = new CheckBox();
            _refreshButton = new Button();
            button1 = new Button();
            _cameraView = new CameraView();
            kioskModeComponent1 = new KioskModeComponent();
            _rootLayout.SuspendLayout();
            _controlBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kioskModeComponent1).BeginInit();
            SuspendLayout();
            // 
            // _rootLayout
            // 
            _rootLayout.ColumnCount = 1;
            _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _rootLayout.Controls.Add(_controlBar, 0, 0);
            _rootLayout.Controls.Add(_cameraView, 0, 1);
            _rootLayout.Dock = DockStyle.Fill;
            _rootLayout.Location = new Point(0, 0);
            _rootLayout.Name = "_rootLayout";
            _rootLayout.Padding = new Padding(8);
            _rootLayout.RowCount = 2;
            _rootLayout.RowStyles.Add(new RowStyle());
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootLayout.Size = new Size(1239, 703);
            _rootLayout.TabIndex = 0;
            // 
            // _controlBar
            // 
            _controlBar.AutoSize = true;
            _controlBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _controlBar.Controls.Add(_cameraLabel);
            _controlBar.Controls.Add(_cameraComboBox);
            _controlBar.Controls.Add(_formatLabel);
            _controlBar.Controls.Add(_formatComboBox);
            _controlBar.Controls.Add(_keepAspectRatioCheckBox);
            _controlBar.Controls.Add(_refreshButton);
            _controlBar.Controls.Add(button1);
            _controlBar.Dock = DockStyle.Fill;
            _controlBar.Location = new Point(11, 11);
            _controlBar.Name = "_controlBar";
            _controlBar.Size = new Size(1217, 41);
            _controlBar.TabIndex = 0;
            // 
            // _cameraLabel
            // 
            _cameraLabel.Anchor = AnchorStyles.Left;
            _cameraLabel.AutoSize = true;
            _cameraLabel.Location = new Point(3, 12);
            _cameraLabel.Margin = new Padding(3, 8, 3, 0);
            _cameraLabel.Name = "_cameraLabel";
            _cameraLabel.Size = new Size(76, 25);
            _cameraLabel.TabIndex = 0;
            _cameraLabel.Text = "&Camera:";
            // 
            // _cameraComboBox
            // 
            _cameraComboBox.Anchor = AnchorStyles.Left;
            _cameraComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _cameraComboBox.Location = new Point(85, 4);
            _cameraComboBox.Name = "_cameraComboBox";
            _cameraComboBox.Size = new Size(320, 33);
            _cameraComboBox.TabIndex = 1;
            _cameraComboBox.SelectedIndexChanged += CameraComboBox_SelectedIndexChanged;
            // 
            // _formatLabel
            // 
            _formatLabel.Anchor = AnchorStyles.Left;
            _formatLabel.AutoSize = true;
            _formatLabel.Location = new Point(424, 12);
            _formatLabel.Margin = new Padding(16, 8, 3, 0);
            _formatLabel.Name = "_formatLabel";
            _formatLabel.Size = new Size(99, 25);
            _formatLabel.TabIndex = 2;
            _formatLabel.Text = "&Resolution:";
            // 
            // _formatComboBox
            // 
            _formatComboBox.Anchor = AnchorStyles.Left;
            _formatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _formatComboBox.Location = new Point(529, 4);
            _formatComboBox.Name = "_formatComboBox";
            _formatComboBox.Size = new Size(122, 33);
            _formatComboBox.TabIndex = 3;
            _formatComboBox.SelectedIndexChanged += FormatComboBox_SelectedIndexChanged;
            // 
            // _keepAspectRatioCheckBox
            // 
            _keepAspectRatioCheckBox.Anchor = AnchorStyles.Left;
            _keepAspectRatioCheckBox.AutoSize = true;
            _keepAspectRatioCheckBox.Checked = true;
            _keepAspectRatioCheckBox.CheckState = CheckState.Checked;
            _keepAspectRatioCheckBox.Location = new Point(670, 9);
            _keepAspectRatioCheckBox.Margin = new Padding(16, 6, 3, 0);
            _keepAspectRatioCheckBox.Name = "_keepAspectRatioCheckBox";
            _keepAspectRatioCheckBox.Size = new Size(174, 29);
            _keepAspectRatioCheckBox.TabIndex = 4;
            _keepAspectRatioCheckBox.Text = "&Keep aspect ratio";
            _keepAspectRatioCheckBox.UseVisualStyleBackColor = true;
            _keepAspectRatioCheckBox.CheckedChanged += KeepAspectRatioCheckBox_CheckedChanged;
            // 
            // _refreshButton
            // 
            _refreshButton.Anchor = AnchorStyles.Left;
            _refreshButton.AutoSize = true;
            _refreshButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _refreshButton.Location = new Point(863, 3);
            _refreshButton.Margin = new Padding(16, 3, 3, 3);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Padding = new Padding(8, 0, 8, 0);
            _refreshButton.Size = new Size(96, 35);
            _refreshButton.TabIndex = 5;
            _refreshButton.Text = "Re&fresh";
            _refreshButton.UseVisualStyleBackColor = true;
            _refreshButton.Click += RefreshButton_Click;
            // 
            // button1
            // 
            button1.Location = new Point(965, 3);
            button1.Name = "button1";
            button1.Size = new Size(180, 34);
            button1.TabIndex = 6;
            button1.Text = "Toogle Fullscreen";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // _cameraView
            // 
            _cameraView.BackColor = Color.Black;
            _cameraView.Dock = DockStyle.Fill;
            _cameraView.ForeColor = Color.Gainsboro;
            _cameraView.Location = new Point(11, 58);
            _cameraView.Name = "_cameraView";
            _cameraView.Size = new Size(1217, 634);
            _cameraView.TabIndex = 1;
            _cameraView.TabStop = false;
            // 
            // kioskModeComponent1
            // 
            kioskModeComponent1.ContainerControl = this;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1239, 703);
            Controls.Add(_rootLayout);
            KeyPreview = true;
            MinimumSize = new Size(640, 480);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Camera Control Demo";
            _rootLayout.ResumeLayout(false);
            _rootLayout.PerformLayout();
            _controlBar.ResumeLayout(false);
            _controlBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)kioskModeComponent1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private FlowLayoutPanel _controlBar;
        private Label _cameraLabel;
        private ComboBox _cameraComboBox;
        private Label _formatLabel;
        private ComboBox _formatComboBox;
        private CheckBox _keepAspectRatioCheckBox;
        private Button _refreshButton;
        private CameraView _cameraView;
        private Button button1;
        private KioskModeComponent kioskModeComponent1;
    }
}
