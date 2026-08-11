namespace KioskComponentFeatureTest
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            kioskModeManager = new KioskModeManager(components);
            ((System.ComponentModel.ISupportInitialize)kioskModeManager).BeginInit();
            SuspendLayout();
            // 
            // kioskModeManager
            // 
            kioskModeManager.ContainerControl = this;
            kioskModeManager.MousePointerAutoHideDelay = 10000;
            kioskModeManager.TopMostInFullScreen = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(946, 619);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)kioskModeManager).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private KioskModeManager kioskModeManager;
    }
}
