namespace LargeFormSmokeTest.Forms
{
    partial class DeclarationForm
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
            _menu = new System.Windows.Forms.MenuStrip();
            _fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            _menuExport = new System.Windows.Forms.ToolStripMenuItem();
            _menuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            _editMenu = new System.Windows.Forms.ToolStripMenuItem();
            _menuEditTaxForm = new System.Windows.Forms.ToolStripMenuItem();
            _menuSaveChanges = new System.Windows.Forms.ToolStripMenuItem();
            _menuSaveAndClose = new System.Windows.Forms.ToolStripMenuItem();
            _menuCloseWithoutSaving = new System.Windows.Forms.ToolStripMenuItem();
            _viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            _toolStrip = new System.Windows.Forms.ToolStrip();
            _btnEdit = new System.Windows.Forms.ToolStripButton();
            _btnSave = new System.Windows.Forms.ToolStripButton();
            _btnExport = new System.Windows.Forms.ToolStripButton();
            _btnClose = new System.Windows.Forms.ToolStripButton();
            _banner = new System.Windows.Forms.Label();
            _host = new System.Windows.Forms.FlowLayoutPanel();
            _menu.SuspendLayout();
            _toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _menu
            // 
            _menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _fileMenu, _editMenu, _viewMenu });
            _menu.Location = new System.Drawing.Point(0, 0);
            _menu.Name = "_menu";
            _menu.Size = new System.Drawing.Size(820, 24);
            // 
            // _fileMenu
            // 
            _fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { _menuExport, _menuFileClose });
            _fileMenu.Name = "_fileMenu";
            _fileMenu.Text = "File";
            // 
            // _menuExport
            // 
            _menuExport.Name = "_menuExport";
            _menuExport.Text = "Export";
            // 
            // _menuFileClose
            // 
            _menuFileClose.Name = "_menuFileClose";
            _menuFileClose.Text = "Close";
            // 
            // _editMenu
            // 
            _editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { _menuEditTaxForm, _menuSaveChanges, _menuSaveAndClose, _menuCloseWithoutSaving });
            _editMenu.Name = "_editMenu";
            _editMenu.Text = "Edit";
            // 
            // _menuEditTaxForm
            // 
            _menuEditTaxForm.Name = "_menuEditTaxForm";
            _menuEditTaxForm.Text = "Edit tax form";
            // 
            // _menuSaveChanges
            // 
            _menuSaveChanges.Name = "_menuSaveChanges";
            _menuSaveChanges.Text = "Save changes";
            // 
            // _menuSaveAndClose
            // 
            _menuSaveAndClose.Name = "_menuSaveAndClose";
            _menuSaveAndClose.Text = "Save and close";
            // 
            // _menuCloseWithoutSaving
            // 
            _menuCloseWithoutSaving.Name = "_menuCloseWithoutSaving";
            _menuCloseWithoutSaving.Text = "Close without saving";
            // 
            // _viewMenu
            // 
            _viewMenu.Name = "_viewMenu";
            _viewMenu.Text = "Go to section";
            // 
            // _toolStrip
            // 
            _toolStrip.ImageScalingSize = new System.Drawing.Size(36, 36);
            _toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _btnEdit, _btnSave, _btnExport, _btnClose });
            _toolStrip.Location = new System.Drawing.Point(0, 24);
            _toolStrip.Name = "_toolStrip";
            _toolStrip.Size = new System.Drawing.Size(820, 43);
            // 
            // _btnEdit
            // 
            _btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _btnEdit.Name = "_btnEdit";
            _btnEdit.Size = new System.Drawing.Size(40, 40);
            _btnEdit.Text = "Edit tax form";
            // 
            // _btnSave
            // 
            _btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new System.Drawing.Size(40, 40);
            _btnSave.Text = "Save changes";
            // 
            // _btnExport
            // 
            _btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _btnExport.Name = "_btnExport";
            _btnExport.Size = new System.Drawing.Size(40, 40);
            _btnExport.Text = "Export";
            // 
            // _btnClose
            // 
            _btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new System.Drawing.Size(40, 40);
            _btnClose.Text = "Close";
            // 
            // _banner
            // 
            _banner.Dock = System.Windows.Forms.DockStyle.Top;
            _banner.Location = new System.Drawing.Point(0, 67);
            _banner.Name = "_banner";
            _banner.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            _banner.Size = new System.Drawing.Size(820, 28);
            _banner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _host
            // 
            _host.AutoScroll = true;
            _host.Dock = System.Windows.Forms.DockStyle.Fill;
            _host.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            _host.Location = new System.Drawing.Point(0, 95);
            _host.Name = "_host";
            _host.Padding = new System.Windows.Forms.Padding(12);
            _host.Size = new System.Drawing.Size(820, 555);
            _host.WrapContents = false;
            // 
            // DeclarationForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(820, 650);
            Controls.Add(_host);
            Controls.Add(_banner);
            Controls.Add(_toolStrip);
            Controls.Add(_menu);
            MainMenuStrip = _menu;
            MinimumSize = new System.Drawing.Size(700, 480);
            Name = "DeclarationForm";
            StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            Text = "DeclarationForm";
            _menu.ResumeLayout(false);
            _menu.PerformLayout();
            _toolStrip.ResumeLayout(false);
            _toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip _menu;
        private System.Windows.Forms.ToolStripMenuItem _fileMenu;
        private System.Windows.Forms.ToolStripMenuItem _menuExport;
        private System.Windows.Forms.ToolStripMenuItem _menuFileClose;
        private System.Windows.Forms.ToolStripMenuItem _editMenu;
        private System.Windows.Forms.ToolStripMenuItem _menuEditTaxForm;
        private System.Windows.Forms.ToolStripMenuItem _menuSaveChanges;
        private System.Windows.Forms.ToolStripMenuItem _menuSaveAndClose;
        private System.Windows.Forms.ToolStripMenuItem _menuCloseWithoutSaving;
        private System.Windows.Forms.ToolStripMenuItem _viewMenu;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripButton _btnEdit;
        private System.Windows.Forms.ToolStripButton _btnSave;
        private System.Windows.Forms.ToolStripButton _btnExport;
        private System.Windows.Forms.ToolStripButton _btnClose;
        private System.Windows.Forms.Label _banner;
        private System.Windows.Forms.FlowLayoutPanel _host;
    }
}
