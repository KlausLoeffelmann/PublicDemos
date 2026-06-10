namespace NETC64.ConceptApp
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
            _memoryMapControl = new C64MemoryMapControl();
            components = new System.ComponentModel.Container();
            SuspendLayout();
            // 
            // _memoryMapControl
            // 
            _memoryMapControl.AccessibleDescription = "Displays the Commodore 64 screen memory as a 40 by 25 character grid.";
            _memoryMapControl.AccessibleName = "Commodore 64 screen memory";
            _memoryMapControl.Dock = DockStyle.Fill;
            _memoryMapControl.Location = new Point(0, 0);
            _memoryMapControl.Name = "_memoryMapControl";
            _memoryMapControl.Size = new Size(800, 450);
            _memoryMapControl.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(_memoryMapControl);
            Name = "MainForm";
            Text = "NET C64";
            ResumeLayout(false);
        }

        #endregion

        private C64MemoryMapControl _memoryMapControl;
    }
}
