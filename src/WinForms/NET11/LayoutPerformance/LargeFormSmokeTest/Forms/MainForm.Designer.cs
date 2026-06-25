namespace LargeFormSmokeTest.Forms
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _menu = new System.Windows.Forms.MenuStrip();
            _viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            _languageMenu = new System.Windows.Forms.ToolStripMenuItem();
            _menuEnglish = new System.Windows.Forms.ToolStripMenuItem();
            _menuGerman = new System.Windows.Forms.ToolStripMenuItem();
            _themeMenu = new System.Windows.Forms.ToolStripMenuItem();
            _menuClassic = new System.Windows.Forms.ToolStripMenuItem();
            _menuDark = new System.Windows.Forms.ToolStripMenuItem();
            _toolStrip = new System.Windows.Forms.ToolStrip();
            _btnEditPerson = new System.Windows.Forms.ToolStripButton();
            _btnOpenDeclaration = new System.Windows.Forms.ToolStripButton();
            _split = new System.Windows.Forms.SplitContainer();
            _payersGrid = new LargeFormSmokeTest.Controls.ThemedDataGridView();
            _bottomLayout = new System.Windows.Forms.TableLayoutPanel();
            _headerPanel = new System.Windows.Forms.Panel();
            _lblTaxNumber = new System.Windows.Forms.Label();
            _lblName = new System.Windows.Forms.Label();
            _detailGroup = new System.Windows.Forms.GroupBox();
            _detailTable = new System.Windows.Forms.TableLayoutPanel();
            _capName = new System.Windows.Forms.Label();
            _valName = new System.Windows.Forms.Label();
            _capBirth = new System.Windows.Forms.Label();
            _valBirth = new System.Windows.Forms.Label();
            _capAddress = new System.Windows.Forms.Label();
            _valAddress = new System.Windows.Forms.Label();
            _capPrevAddress = new System.Windows.Forms.Label();
            _valPrevAddress = new System.Windows.Forms.Label();
            _capMaiden = new System.Windows.Forms.Label();
            _valMaiden = new System.Windows.Forms.Label();
            _capMother = new System.Windows.Forms.Label();
            _valMother = new System.Windows.Forms.Label();
            _capFather = new System.Windows.Forms.Label();
            _valFather = new System.Windows.Forms.Label();
            _capContacts = new System.Windows.Forms.Label();
            _valContacts = new System.Windows.Forms.Label();
            _declarationsGroup = new System.Windows.Forms.GroupBox();
            _declarationsGrid = new LargeFormSmokeTest.Controls.ThemedDataGridView();
            _menu.SuspendLayout();
            _toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_split).BeginInit();
            _split.Panel1.SuspendLayout();
            _split.Panel2.SuspendLayout();
            _split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_payersGrid).BeginInit();
            _bottomLayout.SuspendLayout();
            _headerPanel.SuspendLayout();
            _detailGroup.SuspendLayout();
            _detailTable.SuspendLayout();
            _declarationsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_declarationsGrid).BeginInit();
            SuspendLayout();
            // 
            // _menu
            // 
            _menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _viewMenu });
            _menu.Location = new System.Drawing.Point(0, 0);
            _menu.Name = "_menu";
            _menu.Size = new System.Drawing.Size(1000, 24);
            // 
            // _viewMenu
            // 
            _viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { _languageMenu, _themeMenu });
            _viewMenu.Name = "_viewMenu";
            _viewMenu.Text = "View";
            // 
            // _languageMenu
            // 
            _languageMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { _menuEnglish, _menuGerman });
            _languageMenu.Name = "_languageMenu";
            _languageMenu.Text = "Language";
            // 
            // _menuEnglish
            // 
            _menuEnglish.Name = "_menuEnglish";
            _menuEnglish.Text = "English";
            // 
            // _menuGerman
            // 
            _menuGerman.Name = "_menuGerman";
            _menuGerman.Text = "German";
            // 
            // _themeMenu
            // 
            _themeMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { _menuClassic, _menuDark });
            _themeMenu.Name = "_themeMenu";
            _themeMenu.Text = "Theme";
            // 
            // _menuClassic
            // 
            _menuClassic.Name = "_menuClassic";
            _menuClassic.Text = "Classic";
            // 
            // _menuDark
            // 
            _menuDark.Name = "_menuDark";
            _menuDark.Text = "Dark";
            // 
            // _toolStrip
            // 
            _toolStrip.ImageScalingSize = new System.Drawing.Size(36, 36);
            _toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _btnEditPerson, _btnOpenDeclaration });
            _toolStrip.Location = new System.Drawing.Point(0, 24);
            _toolStrip.Name = "_toolStrip";
            _toolStrip.Size = new System.Drawing.Size(1000, 43);
            // 
            // _btnEditPerson
            // 
            _btnEditPerson.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _btnEditPerson.Name = "_btnEditPerson";
            _btnEditPerson.Size = new System.Drawing.Size(40, 40);
            _btnEditPerson.Text = "Edit person";
            // 
            // _btnOpenDeclaration
            // 
            _btnOpenDeclaration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _btnOpenDeclaration.Name = "_btnOpenDeclaration";
            _btnOpenDeclaration.Size = new System.Drawing.Size(40, 40);
            _btnOpenDeclaration.Text = "Open declaration";
            // 
            // _split
            // 
            _split.Dock = System.Windows.Forms.DockStyle.Fill;
            _split.Location = new System.Drawing.Point(0, 67);
            _split.Name = "_split";
            _split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _split.Panel1
            // 
            _split.Panel1.Controls.Add(_payersGrid);
            _split.Panel1.Padding = new System.Windows.Forms.Padding(8);
            // 
            // _split.Panel2
            // 
            _split.Panel2.Controls.Add(_bottomLayout);
            _split.Panel2.Padding = new System.Windows.Forms.Padding(8);
            _split.Size = new System.Drawing.Size(1000, 583);
            _split.SplitterDistance = 280;
            _split.TabIndex = 0;
            // 
            // _payersGrid
            // 
            _payersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            _payersGrid.Name = "_payersGrid";
            _payersGrid.TabIndex = 0;
            // 
            // _bottomLayout
            // 
            _bottomLayout.ColumnCount = 2;
            _bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42F));
            _bottomLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58F));
            _bottomLayout.Controls.Add(_headerPanel, 0, 0);
            _bottomLayout.Controls.Add(_detailGroup, 0, 1);
            _bottomLayout.Controls.Add(_declarationsGroup, 1, 1);
            _bottomLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            _bottomLayout.Name = "_bottomLayout";
            _bottomLayout.RowCount = 2;
            _bottomLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            _bottomLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // _headerPanel
            // 
            _bottomLayout.SetColumnSpan(_headerPanel, 2);
            _headerPanel.Controls.Add(_lblName);
            _headerPanel.Controls.Add(_lblTaxNumber);
            _headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _headerPanel.Name = "_headerPanel";
            // 
            // _lblTaxNumber
            // 
            _lblTaxNumber.Dock = System.Windows.Forms.DockStyle.Left;
            _lblTaxNumber.Name = "_lblTaxNumber";
            _lblTaxNumber.Size = new System.Drawing.Size(300, 40);
            _lblTaxNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblName
            // 
            _lblName.Dock = System.Windows.Forms.DockStyle.Right;
            _lblName.Name = "_lblName";
            _lblName.Size = new System.Drawing.Size(400, 40);
            _lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _detailGroup
            // 
            _detailGroup.Controls.Add(_detailTable);
            _detailGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            _detailGroup.Name = "_detailGroup";
            _detailGroup.Padding = new System.Windows.Forms.Padding(8);
            _detailGroup.Text = "Details";
            // 
            // _detailTable
            // 
            _detailTable.ColumnCount = 2;
            _detailTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            _detailTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _detailTable.Controls.Add(_capName, 0, 0);
            _detailTable.Controls.Add(_valName, 1, 0);
            _detailTable.Controls.Add(_capBirth, 0, 1);
            _detailTable.Controls.Add(_valBirth, 1, 1);
            _detailTable.Controls.Add(_capAddress, 0, 2);
            _detailTable.Controls.Add(_valAddress, 1, 2);
            _detailTable.Controls.Add(_capPrevAddress, 0, 3);
            _detailTable.Controls.Add(_valPrevAddress, 1, 3);
            _detailTable.Controls.Add(_capMaiden, 0, 4);
            _detailTable.Controls.Add(_valMaiden, 1, 4);
            _detailTable.Controls.Add(_capMother, 0, 5);
            _detailTable.Controls.Add(_valMother, 1, 5);
            _detailTable.Controls.Add(_capFather, 0, 6);
            _detailTable.Controls.Add(_valFather, 1, 6);
            _detailTable.Controls.Add(_capContacts, 0, 7);
            _detailTable.Controls.Add(_valContacts, 1, 7);
            _detailTable.Dock = System.Windows.Forms.DockStyle.Fill;
            _detailTable.Name = "_detailTable";
            _detailTable.RowCount = 9;
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            _detailTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _capName.Dock = System.Windows.Forms.DockStyle.Fill;
            _capName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valName.Dock = System.Windows.Forms.DockStyle.Fill;
            _valName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valName.AutoEllipsis = true;
            _capBirth.Dock = System.Windows.Forms.DockStyle.Fill;
            _capBirth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valBirth.Dock = System.Windows.Forms.DockStyle.Fill;
            _valBirth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valBirth.AutoEllipsis = true;
            _capAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            _capAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            _valAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valAddress.AutoEllipsis = true;
            _capPrevAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            _capPrevAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valPrevAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            _valPrevAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valPrevAddress.AutoEllipsis = true;
            _capMaiden.Dock = System.Windows.Forms.DockStyle.Fill;
            _capMaiden.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valMaiden.Dock = System.Windows.Forms.DockStyle.Fill;
            _valMaiden.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valMaiden.AutoEllipsis = true;
            _capMother.Dock = System.Windows.Forms.DockStyle.Fill;
            _capMother.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valMother.Dock = System.Windows.Forms.DockStyle.Fill;
            _valMother.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valMother.AutoEllipsis = true;
            _capFather.Dock = System.Windows.Forms.DockStyle.Fill;
            _capFather.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valFather.Dock = System.Windows.Forms.DockStyle.Fill;
            _valFather.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valFather.AutoEllipsis = true;
            _capContacts.Dock = System.Windows.Forms.DockStyle.Fill;
            _capContacts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valContacts.Dock = System.Windows.Forms.DockStyle.Fill;
            _valContacts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            _valContacts.AutoEllipsis = true;
            // 
            // _declarationsGroup
            // 
            _declarationsGroup.Controls.Add(_declarationsGrid);
            _declarationsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            _declarationsGroup.Name = "_declarationsGroup";
            _declarationsGroup.Padding = new System.Windows.Forms.Padding(8);
            _declarationsGroup.Text = "Declarations";
            // 
            // _declarationsGrid
            // 
            _declarationsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            _declarationsGrid.Name = "_declarationsGrid";
            _declarationsGrid.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1000, 650);
            Controls.Add(_split);
            Controls.Add(_toolStrip);
            Controls.Add(_menu);
            MainMenuStrip = _menu;
            MinimumSize = new System.Drawing.Size(820, 520);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "MainForm";
            _menu.ResumeLayout(false);
            _menu.PerformLayout();
            _toolStrip.ResumeLayout(false);
            _toolStrip.PerformLayout();
            _split.Panel1.ResumeLayout(false);
            _split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_split).EndInit();
            _split.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_payersGrid).EndInit();
            _bottomLayout.ResumeLayout(false);
            _headerPanel.ResumeLayout(false);
            _detailGroup.ResumeLayout(false);
            _detailTable.ResumeLayout(false);
            _declarationsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_declarationsGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip _menu;
        private System.Windows.Forms.ToolStripMenuItem _viewMenu;
        private System.Windows.Forms.ToolStripMenuItem _languageMenu;
        private System.Windows.Forms.ToolStripMenuItem _menuEnglish;
        private System.Windows.Forms.ToolStripMenuItem _menuGerman;
        private System.Windows.Forms.ToolStripMenuItem _themeMenu;
        private System.Windows.Forms.ToolStripMenuItem _menuClassic;
        private System.Windows.Forms.ToolStripMenuItem _menuDark;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripButton _btnEditPerson;
        private System.Windows.Forms.ToolStripButton _btnOpenDeclaration;
        private System.Windows.Forms.SplitContainer _split;
        private LargeFormSmokeTest.Controls.ThemedDataGridView _payersGrid;
        private System.Windows.Forms.TableLayoutPanel _bottomLayout;
        private System.Windows.Forms.Panel _headerPanel;
        private System.Windows.Forms.Label _lblTaxNumber;
        private System.Windows.Forms.Label _lblName;
        private System.Windows.Forms.GroupBox _detailGroup;
        private System.Windows.Forms.TableLayoutPanel _detailTable;
        private System.Windows.Forms.Label _capName;
        private System.Windows.Forms.Label _valName;
        private System.Windows.Forms.Label _capBirth;
        private System.Windows.Forms.Label _valBirth;
        private System.Windows.Forms.Label _capAddress;
        private System.Windows.Forms.Label _valAddress;
        private System.Windows.Forms.Label _capPrevAddress;
        private System.Windows.Forms.Label _valPrevAddress;
        private System.Windows.Forms.Label _capMaiden;
        private System.Windows.Forms.Label _valMaiden;
        private System.Windows.Forms.Label _capMother;
        private System.Windows.Forms.Label _valMother;
        private System.Windows.Forms.Label _capFather;
        private System.Windows.Forms.Label _valFather;
        private System.Windows.Forms.Label _capContacts;
        private System.Windows.Forms.Label _valContacts;
        private System.Windows.Forms.GroupBox _declarationsGroup;
        private LargeFormSmokeTest.Controls.ThemedDataGridView _declarationsGrid;
    }
}
