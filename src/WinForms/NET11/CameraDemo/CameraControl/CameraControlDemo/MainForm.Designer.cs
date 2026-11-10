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
            _controlBar = new ToolStrip();
            _cameraLabel = new ToolStripLabel();
            _cameraComboBox = new ToolStripComboBox();
            _formatLabel = new ToolStripLabel();
            _formatComboBox = new ToolStripComboBox();
            _captureSeparator = new ToolStripSeparator();
            _keepAspectRatioButton = new ToolStripButton();
            _refreshButton = new ToolStripButton();
            _viewSeparator = new ToolStripSeparator();
            _fullScreenButton = new ToolStripButton();
            _cameraView = new CameraView();
            components = new System.ComponentModel.Container();
            _imageFactory = new ToolbarImageFactory(components);
            _kioskModeManager = new KioskModeManager(components);
            SuspendLayout();
            _rootLayout.SuspendLayout();
            _controlBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_kioskModeManager).BeginInit();
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
            _controlBar.AccessibleName = "Camera controls";
            _controlBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _controlBar.Dock = DockStyle.None;
            _controlBar.GripStyle = ToolStripGripStyle.Hidden;
            _controlBar.Items.AddRange(new ToolStripItem[] { _cameraLabel, _cameraComboBox, _formatLabel, _formatComboBox, _captureSeparator, _keepAspectRatioButton, _refreshButton, _viewSeparator, _fullScreenButton });
            _controlBar.Location = new Point(11, 11);
            _controlBar.Margin = new Padding(3);
            _controlBar.Name = "_controlBar";
            _controlBar.Padding = new Padding(4);
            _controlBar.Size = new Size(1217, 41);
            _controlBar.TabIndex = 0;
            _controlBar.TabStop = true;
            // 
            // _cameraLabel
            // 
            _cameraLabel.ImageScaling = ToolStripItemImageScaling.None;
            _cameraLabel.Name = "_cameraLabel";
            _cameraLabel.Text = "&Camera:";
            // 
            // _cameraComboBox
            // 
            _cameraComboBox.AccessibleName = "Camera";
            _cameraComboBox.AutoSize = false;
            _cameraComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _cameraComboBox.DropDownWidth = 400;
            _cameraComboBox.Name = "_cameraComboBox";
            _cameraComboBox.Size = new Size(250, 33);
            _cameraComboBox.ToolTipText = "Select the camera to preview";
            _cameraComboBox.SelectedIndexChanged += CameraComboBox_SelectedIndexChanged;
            // 
            // _formatLabel
            // 
            _formatLabel.ImageScaling = ToolStripItemImageScaling.None;
            _formatLabel.Margin = new Padding(12, 1, 0, 2);
            _formatLabel.Name = "_formatLabel";
            _formatLabel.Text = "&Resolution:";
            // 
            // _formatComboBox
            // 
            _formatComboBox.AccessibleName = "Capture resolution and frame rate";
            _formatComboBox.AutoSize = false;
            _formatComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _formatComboBox.DropDownWidth = 400;
            _formatComboBox.Name = "_formatComboBox";
            _formatComboBox.Size = new Size(260, 33);
            _formatComboBox.ToolTipText = "Select the capture resolution, frame rate, and pixel format";
            _formatComboBox.SelectedIndexChanged += FormatComboBox_SelectedIndexChanged;
            //
            // _captureSeparator
            //
            _captureSeparator.Name = "_captureSeparator";
            //
            // _keepAspectRatioButton
            //
            _keepAspectRatioButton.Checked = true;
            _keepAspectRatioButton.CheckOnClick = true;
            _keepAspectRatioButton.CheckState = CheckState.Checked;
            _keepAspectRatioButton.ImageScaling = ToolStripItemImageScaling.None;
            _keepAspectRatioButton.Name = "_keepAspectRatioButton";
            _keepAspectRatioButton.Text = "&Fit frame";
            _keepAspectRatioButton.ToolTipText = "Fit the frame while preserving its aspect ratio; turn off for unscaled 1:1 pixels";
            _keepAspectRatioButton.CheckedChanged += KeepAspectRatioButton_CheckedChanged;
            // 
            // _refreshButton
            // 
            _refreshButton.ImageScaling = ToolStripItemImageScaling.None;
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Text = "Re&fresh";
            _refreshButton.ToolTipText = "Refresh the camera list and restart the preview";
            _refreshButton.Click += RefreshButton_Click;
            //
            // _viewSeparator
            //
            _viewSeparator.Name = "_viewSeparator";
            // 
            // _fullScreenButton
            // 
            _fullScreenButton.Alignment = ToolStripItemAlignment.Right;
            _fullScreenButton.ImageScaling = ToolStripItemImageScaling.None;
            _fullScreenButton.Name = "_fullScreenButton";
            _fullScreenButton.Overflow = ToolStripItemOverflow.Never;
            _fullScreenButton.Text = "F&ullscreen";
            _fullScreenButton.ToolTipText = "Enter fullscreen (F11); press Esc to return to the window";
            _fullScreenButton.Click += FullScreenButton_Click;
            // 
            // _cameraView
            // 
            _cameraView.BackColor = Color.Black;
            _cameraView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _cameraView.ForeColor = Color.Gainsboro;
            _cameraView.Location = new Point(11, 58);
            _cameraView.Name = "_cameraView";
            _cameraView.Size = new Size(1217, 634);
            _cameraView.TabIndex = 1;
            _cameraView.TabStop = false;
            // 
            // _kioskModeManager
            // 
            _kioskModeManager.ContainerControl = this;
            _kioskModeManager.EscapeExitsFullScreen = true;
            _kioskModeManager.ToggleFullScreenKeys = Keys.F11;
            _kioskModeManager.FullScreenChanged += KioskModeManager_FullScreenChanged;
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
            ((System.ComponentModel.ISupportInitialize)_kioskModeManager).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private ToolStrip _controlBar;
        private ToolStripLabel _cameraLabel;
        private ToolStripComboBox _cameraComboBox;
        private ToolStripLabel _formatLabel;
        private ToolStripComboBox _formatComboBox;
        private ToolStripSeparator _captureSeparator;
        private ToolStripButton _keepAspectRatioButton;
        private ToolStripButton _refreshButton;
        private ToolStripSeparator _viewSeparator;
        private ToolStripButton _fullScreenButton;
        private CameraView _cameraView;
        private KioskModeManager _kioskModeManager;
        private ToolbarImageFactory _imageFactory;
    }
}
