namespace Northwind.App
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();

            // ── Fonts ──────────────────────────────────────────────────────────
            var menuFont      = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Point);
            var statusFont    = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Point);
            var splitPaneFont = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);

            // ── MenuStrip ──────────────────────────────────────────────────────
            _menuStrip = new MenuStrip { Font = menuFont };

            var mnuFile   = new ToolStripMenuItem("&File");
            _mnuExportCsv = new ToolStripMenuItem("Export as CSV...");
            _mnuQuit      = new ToolStripMenuItem("Quit");
            mnuFile.DropDownItems.AddRange(new ToolStripItem[]
            {
                _mnuExportCsv,
                new ToolStripSeparator(),
                _mnuQuit
            });

            var mnuEdit = new ToolStripMenuItem("&Edit");
            _mnuAdd     = new ToolStripMenuItem("Add new Customer");
            _mnuEdit    = new ToolStripMenuItem("Edit selected Customer");
            _mnuCancel  = new ToolStripMenuItem("Cancel");
            _mnuSave    = new ToolStripMenuItem("Save changes");
            mnuEdit.DropDownItems.AddRange(new ToolStripItem[]
            {
                _mnuAdd,
                _mnuEdit,
                _mnuCancel,
                new ToolStripSeparator(),
                _mnuSave
            });

            _menuStrip.Items.AddRange(new ToolStripItem[] { mnuFile, mnuEdit });

            // ── ToolStrip ──────────────────────────────────────────────────────
            _toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size(36, 36),
                Padding          = new Padding(2)
            };

            _tsbAdd    = CreateToolButton("Add",          SegoeFluentIconFactory.Glyphs.Add);
            _tsbEdit   = CreateToolButton("Edit",         SegoeFluentIconFactory.Glyphs.Edit);
            _tsbCancel = CreateToolButton("Cancel",       SegoeFluentIconFactory.Glyphs.Cancel);
            _tsbSave   = CreateToolButton("Save changes", SegoeFluentIconFactory.Glyphs.Save);

            _toolStrip.Items.AddRange(new ToolStripItem[] { _tsbAdd, _tsbEdit, _tsbCancel, _tsbSave });

            // ── SplitContainer ─────────────────────────────────────────────────
            _splitContainer = new SplitContainer
            {
                Dock             = DockStyle.Fill,
                Orientation      = Orientation.Horizontal,
                Font             = splitPaneFont,
                SplitterDistance = 320
            };

            // Panel1 — DataGridView
            _grid = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ReadOnly              = true,
                MultiSelect           = true,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle           = BorderStyle.None,
                BackgroundColor       = SystemColors.Window
            };
            _splitContainer.Panel1.Controls.Add(_grid);

            // Panel2 — Detail view
            _pnlDetail = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(8, 10, 8, 8)
            };

            _lblDetailHeader = new Label
            {
                Dock      = DockStyle.Top,
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold, GraphicsUnit.Point),
                Height    = 36,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var tblDetail = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 4,
                AutoSize    = false,
                Padding     = new Padding(0, 4, 0, 0)
            };
            tblDetail.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tblDetail.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tblDetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            // Build label/textbox pairs in the detail table
            _txtCustomerId   = AddDetailRow(tblDetail, 0, "Customer ID",   0);
            _txtCompanyName  = AddDetailRow(tblDetail, 0, "Company Name",  1);
            _txtContactName  = AddDetailRow(tblDetail, 1, "Contact Name",  0);
            _txtContactTitle = AddDetailRow(tblDetail, 1, "Contact Title", 2);
            _txtAddress      = AddDetailRow(tblDetail, 2, "Address",       0);
            _txtCity         = AddDetailRow(tblDetail, 2, "City",          2);
            _txtRegion       = AddDetailRow(tblDetail, 3, "Region",        0);
            _txtPostalCode   = AddDetailRow(tblDetail, 3, "Postal Code",   2);
            _txtCountry      = AddDetailRow(tblDetail, 4, "Country",       0);
            _txtPhone        = AddDetailRow(tblDetail, 4, "Phone",         2);
            _txtFax          = AddDetailRow(tblDetail, 5, "Fax",           0);

            _pnlDetail.Controls.Add(tblDetail);
            _pnlDetail.Controls.Add(_lblDetailHeader);
            _splitContainer.Panel2.Controls.Add(_pnlDetail);

            // ── StatusStrip ────────────────────────────────────────────────────
            _statusStrip = new StatusStrip { Font = statusFont };

            _ssLblCustomersCaption   = new ToolStripStatusLabel("Customers:");
            _ssLblCustomerCount      = new ToolStripStatusLabel("0");
            _ssLblLastChangedCaption = new ToolStripStatusLabel("Last changed Customer:");
            _ssLblLastChanged        = new ToolStripStatusLabel("");
            _ssBtnSelect             = new ToolStripButton("Select") { ToolTipText = "Jump to last changed customer in grid" };
            _ssLblDateTime           = new ToolStripStatusLabel("") { Spring = true, TextAlign = ContentAlignment.MiddleRight };

            _statusStrip.Items.AddRange(new ToolStripItem[]
            {
                _ssLblCustomersCaption,
                _ssLblCustomerCount,
                new ToolStripSeparator(),
                _ssLblLastChangedCaption,
                _ssLblLastChanged,
                _ssBtnSelect,
                _ssLblDateTime
            });

            // ── Timer for clock ────────────────────────────────────────────────
            _clockTimer = new System.Windows.Forms.Timer(components) { Interval = 1000 };

            // ── Wire up to form ────────────────────────────────────────────────
            Controls.Add(_splitContainer);
            Controls.Add(_toolStrip);
            Controls.Add(_menuStrip);
            Controls.Add(_statusStrip);
            MainMenuStrip = _menuStrip;

            AutoScaleDimensions = new SizeF(7f, 15f);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(1200, 750);
            Name                = "FrmMain";
            Text                = "Northwind – Customer Editor";
            Font                = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);

            ResumeLayout(false);
            PerformLayout();
        }

        // ── Layout helpers ────────────────────────────────────────────────────

        private static ToolStripButton CreateToolButton(string text, string glyph)
            => new()
            {
                Text              = text,
                Image             = SegoeFluentIconFactory.CreateIcon(glyph, 36),
                ImageScaling      = ToolStripItemImageScaling.SizeToFit,
                TextImageRelation = TextImageRelation.ImageAboveText,
                DisplayStyle      = ToolStripItemDisplayStyle.ImageAndText
            };

        /// <summary>
        /// Adds a Label + TextBox pair to <paramref name="table"/> at the given
        /// logical <paramref name="row"/> starting at <paramref name="startCol"/>
        /// (0 or 2) and returns the created TextBox.
        /// </summary>
        private static TextBox AddDetailRow(
            TableLayoutPanel table, int row, string labelText, int startCol)
        {
            var lbl = new Label
            {
                Text      = labelText + ":",
                AutoSize  = true,
                Anchor    = AnchorStyles.Left | AnchorStyles.Top,
                Padding   = new Padding(0, 6, 6, 0)
            };
            var tb = new TextBox
            {
                Dock    = DockStyle.Fill,
                Margin  = new Padding(0, 3, 8, 3),
                Enabled = false
            };
            table.Controls.Add(lbl, startCol,     row);
            table.Controls.Add(tb,  startCol + 1, row);
            return tb;
        }

        // ── Controls ──────────────────────────────────────────────────────────
        private MenuStrip         _menuStrip    = null!;
        private ToolStripMenuItem _mnuExportCsv = null!;
        private ToolStripMenuItem _mnuQuit      = null!;
        private ToolStripMenuItem _mnuAdd       = null!;
        private ToolStripMenuItem _mnuEdit      = null!;
        private ToolStripMenuItem _mnuCancel    = null!;
        private ToolStripMenuItem _mnuSave      = null!;

        private ToolStrip       _toolStrip = null!;
        private ToolStripButton _tsbAdd    = null!;
        private ToolStripButton _tsbEdit   = null!;
        private ToolStripButton _tsbCancel = null!;
        private ToolStripButton _tsbSave   = null!;

        private SplitContainer _splitContainer = null!;
        private DataGridView   _grid           = null!;

        private Panel   _pnlDetail       = null!;
        private Label   _lblDetailHeader = null!;
        private TextBox _txtCustomerId   = null!;
        private TextBox _txtCompanyName  = null!;
        private TextBox _txtContactName  = null!;
        private TextBox _txtContactTitle = null!;
        private TextBox _txtAddress      = null!;
        private TextBox _txtCity         = null!;
        private TextBox _txtRegion       = null!;
        private TextBox _txtPostalCode   = null!;
        private TextBox _txtCountry      = null!;
        private TextBox _txtPhone        = null!;
        private TextBox _txtFax          = null!;

        private StatusStrip          _statusStrip             = null!;
        private ToolStripStatusLabel _ssLblCustomersCaption   = null!;
        private ToolStripStatusLabel _ssLblCustomerCount      = null!;
        private ToolStripStatusLabel _ssLblLastChangedCaption = null!;
        private ToolStripStatusLabel _ssLblLastChanged        = null!;
        private ToolStripButton      _ssBtnSelect             = null!;
        private ToolStripStatusLabel _ssLblDateTime           = null!;

        private System.Windows.Forms.Timer _clockTimer = null!;

        #endregion
    }
}

