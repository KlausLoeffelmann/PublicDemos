using Northwind.DataLayer;
using System.ComponentModel;
using System.Text;

namespace Northwind.App
{
    public partial class FrmMain : Form
    {
        private enum EditMode { View, Add, Edit }

        private EditMode _editMode = EditMode.View;
        private Customer? _originalCustomer;
        private List<Customer> _customers = new();
        private Customer? _lastChangedCustomer;

        private readonly BindingSource _bindingSource = new();
        private readonly System.Windows.Forms.Timer _clockTimer;

        // Customer picture cache: CustomerID -> Image
        private readonly Dictionary<string, Image> _customerImages = new(StringComparer.OrdinalIgnoreCase);
        private Image? _placeholderImage;

        // Prevents dirty-tracking from firing while we programmatically populate fields.
        private bool _suppressDirtyTracking;

        public FrmMain()
        {
            InitializeComponent();

            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += ClockTimer_Tick;

            LoadCustomerImages();
            SetupDataGridView();
            SetupToolStripImages();
            UpdateClockLabel();
            UpdateCommandState();
        }

        // ── Lifecycle ───────────────────────────────────────────────────────────

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                await LoadCustomersAsync();
                _clockTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load customers:\n\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clockTimer.Stop();
            base.OnFormClosed(e);
        }

        // ── Setup ───────────────────────────────────────────────────────────────

        private void SetupDataGridView()
        {
            _dataGridView.AutoGenerateColumns = false;
            _dataGridView.DataSource = _bindingSource;

            // Owner-drawn "card" rows – no headers, single full-width cell per record.
            _dataGridView.ColumnHeadersVisible = false;
            _dataGridView.RowHeadersVisible = false;
            _dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            _dataGridView.AllowUserToResizeRows = false;
            _dataGridView.AllowUserToResizeColumns = false;
            _dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dataGridView.RowTemplate.Height = RowHeight;

            // Single full-width column – we paint everything ourselves.
            _dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerCard",
                DataPropertyName = nameof(Customer.CustomerId),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            });

            _dataGridView.CellPainting += DataGridView_CellPainting;
        }

        // ── Card layout constants ───────────────────────────────────────────────
        private const int RowHeight = 132;             // 100 picture + 16 top + 16 bottom
        private const int CardPadding = 16;
        private const int PictureMaxWidth = 250;
        private const int PictureHeight = 100;
        private const int NameColumnWidth = 400;
        private const int ColumnGap = 40;              // wider gap between logical columns

        private void DataGridView_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (_dataGridView.Rows[e.RowIndex].DataBoundItem is not Customer customer)
                return;

            bool selected = (e.State & DataGridViewElementStates.Selected) != 0;

            // Background
            using (var bgBrush = new SolidBrush(selected
                ? e.CellStyle!.SelectionBackColor
                : e.CellStyle!.BackColor))
            {
                e.Graphics!.FillRectangle(bgBrush, e.CellBounds);
            }

            Color foreColor = selected
                ? e.CellStyle!.SelectionForeColor
                : e.CellStyle!.ForeColor;

            var g = e.Graphics!;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            int x = e.CellBounds.Left + CardPadding;
            int yTop = e.CellBounds.Top + CardPadding;
            int picBottom = yTop + PictureHeight;

            // ── Picture column ──────────────────────────────────────────────────
            Image? image = GetCustomerImage(customer);

            if (image is not null)
            {
                // Scale by aspect ratio to the configured height
                double aspect = (double)image.Width / image.Height;
                int scaledWidth = (int)Math.Round(PictureHeight * aspect);

                int destWidth = Math.Min(scaledWidth, PictureMaxWidth);
                var destRect = new Rectangle(x, yTop, destWidth, PictureHeight);

                // If wider than 250 px after scaling, crop the source horizontally.
                int srcCropWidth = (int)Math.Round(destWidth / (double)PictureHeight * image.Height);
                srcCropWidth = Math.Min(srcCropWidth, image.Width);
                var srcRect = new Rectangle(0, 0, srcCropWidth, image.Height);

                g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
            }

            // ── Column 2: CustomerId (top, +2pt) and CompanyName (bottom) ───────
            int col2X = x + PictureMaxWidth + ColumnGap;

            Font baseFont = e.CellStyle!.Font;
            using var idFont = new Font(baseFont.FontFamily, baseFont.SizeInPoints + 2F, FontStyle.Regular);
            using var boldFont = new Font(baseFont, FontStyle.Bold);

            const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

            // CustomerId – top-aligned with the picture
            TextRenderer.DrawText(
                g,
                customer.CustomerId,
                idFont,
                new Point(col2X, yTop),
                foreColor,
                flags);

            // CompanyName – bottom-aligned with the picture (baseline at picBottom)
            Size companySize = TextRenderer.MeasureText(g, customer.CompanyName, baseFont, new Size(NameColumnWidth, int.MaxValue), flags);
            TextRenderer.DrawText(
                g,
                customer.CompanyName,
                baseFont,
                new Rectangle(col2X, picBottom - companySize.Height, NameColumnWidth, companySize.Height),
                foreColor,
                flags);

            // ── Column 3: ContactTitle (top, bold) and ContactName (bottom) ─────
            int col3X = col2X + NameColumnWidth + ColumnGap;
            int col3Width = Math.Max(0, e.CellBounds.Right - col3X - CardPadding);

            if (col3Width > 0)
            {
                if (!string.IsNullOrEmpty(customer.ContactTitle))
                {
                    TextRenderer.DrawText(
                        g,
                        customer.ContactTitle,
                        boldFont,
                        new Rectangle(col3X, yTop, col3Width, idFont.Height),
                        foreColor,
                        flags);
                }

                if (!string.IsNullOrEmpty(customer.ContactName))
                {
                    Size cnSize = TextRenderer.MeasureText(g, customer.ContactName, baseFont, new Size(col3Width, int.MaxValue), flags);
                    TextRenderer.DrawText(
                        g,
                        customer.ContactName,
                        baseFont,
                        new Rectangle(col3X, picBottom - cnSize.Height, col3Width, cnSize.Height),
                        foreColor,
                        flags);
                }
            }

            e.Handled = true;
        }

        private Image? GetCustomerImage(Customer customer)
        {
            if (_customerImages.TryGetValue(customer.CustomerId, out Image? img))
                return img;

            return _placeholderImage;
        }

        private void LoadCustomerImages()
        {
            string resourcesDir = Path.Combine(AppContext.BaseDirectory, "resources");

            if (!Directory.Exists(resourcesDir))
                return;

            // Load placeholder image
            string placeholder1Path = Path.Combine(resourcesDir, "placeholder1.png");
            string placeholder2Path = Path.Combine(resourcesDir, "placeholder2.png");

            if (File.Exists(placeholder1Path))
                _placeholderImage = Image.FromFile(placeholder1Path);
            else if (File.Exists(placeholder2Path))
                _placeholderImage = Image.FromFile(placeholder2Path);

            // Load customer-specific images (filenames like ALFKI.png)
            foreach (string filePath in Directory.GetFiles(resourcesDir, "*.png"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Skip placeholder files
                if (fileName.StartsWith("placeholder", StringComparison.OrdinalIgnoreCase))
                    continue;

                _customerImages[fileName] = Image.FromFile(filePath);
            }
        }

        private void SetupToolStripImages()
        {
            Color foreColor = SystemColors.ControlText;
            const int iconSize = 36;

            _tsbAdd.Image = SegoeFluentIconFactory.CreateIconBitmap(SegoeFluentIconFactory.AddGlyph, iconSize, foreColor);
            _tsbEdit.Image = SegoeFluentIconFactory.CreateIconBitmap(SegoeFluentIconFactory.EditGlyph, iconSize, foreColor);
            _tsbCancel.Image = SegoeFluentIconFactory.CreateIconBitmap(SegoeFluentIconFactory.CancelGlyph, iconSize, foreColor);
            _tsbSaveChanges.Image = SegoeFluentIconFactory.CreateIconBitmap(SegoeFluentIconFactory.SaveGlyph, iconSize, foreColor);
        }

        // ── Data loading ────────────────────────────────────────────────────────

        private async Task LoadCustomersAsync(CancellationToken ct = default)
        {
            _customers = await Task.Run(() =>
            {
                using var context = new NorthwindContext();

                return context.Customers
                    .OrderBy(c => c.CompanyName)
                    .ToList();
            }, ct);

            _bindingSource.DataSource = new BindingList<Customer>(_customers);
            _tsslCustomerCount.Text = _customers.Count.ToString();
        }

        // ── Edit mode ───────────────────────────────────────────────────────────

        private void SetEditMode(EditMode mode)
        {
            _editMode = mode;

            bool isEditing = mode != EditMode.View;

            _dataGridView.Enabled = !isEditing;

            // CustomerID is only editable when adding (it is the primary key)
            _txtCustomerId.Enabled = mode == EditMode.Add;
            _txtCompanyName.Enabled = isEditing;
            _txtContactName.Enabled = isEditing;
            _txtContactTitle.Enabled = isEditing;
            _txtAddress.Enabled = isEditing;
            _txtCity.Enabled = isEditing;
            _txtRegion.Enabled = isEditing;
            _txtPostalCode.Enabled = isEditing;
            _txtCountry.Enabled = isEditing;
            _txtPhone.Enabled = isEditing;
            _txtFax.Enabled = isEditing;

            UpdateCommandState();
        }

        // ── Command-state (enable/disable rules) ────────────────────────────────

        private void UpdateCommandState()
        {
            bool inViewMode = _editMode == EditMode.View;
            bool inEditOrAdd = !inViewMode;
            bool oneRowSelected = _dataGridView.SelectedRows.Count == 1;
            bool moreThanOneSelected = _dataGridView.SelectedRows.Count > 1;
            bool isDirty = IsDirty();

            // Add
            _tsbAdd.Enabled = inViewMode;
            _tsmiAddCustomer.Enabled = inViewMode;

            // Edit
            _tsbEdit.Enabled = inViewMode && oneRowSelected;
            _tsmiEditCustomer.Enabled = inViewMode && oneRowSelected;

            // Cancel
            _tsbCancel.Enabled = inEditOrAdd;
            _tsmiEditCancel.Enabled = inEditOrAdd;

            // Save changes
            _tsbSaveChanges.Enabled = inEditOrAdd && isDirty;
            _tsmiSaveChanges.Enabled = inEditOrAdd && isDirty;

            // Export CSV (> 1 selected rows, in View mode)
            _tsmiExportCsv.Enabled = inViewMode && moreThanOneSelected;

            // Select status-bar button
            _tssbSelect.Enabled = inViewMode && _lastChangedCustomer is not null;
        }

        // ── Dirty tracking ──────────────────────────────────────────────────────

        private bool IsDirty()
        {
            if (_editMode == EditMode.View)
                return false;

            if (_editMode == EditMode.Add)
            {
                return !string.IsNullOrWhiteSpace(_txtCustomerId.Text)
                    || !string.IsNullOrWhiteSpace(_txtCompanyName.Text);
            }

            // Edit mode: compare TextBox values against the stored original
            if (_originalCustomer is null)
                return false;

            return _txtCustomerId.Text != _originalCustomer.CustomerId
                || _txtCompanyName.Text != _originalCustomer.CompanyName
                || _txtContactName.Text != (_originalCustomer.ContactName ?? string.Empty)
                || _txtContactTitle.Text != (_originalCustomer.ContactTitle ?? string.Empty)
                || _txtAddress.Text != (_originalCustomer.Address ?? string.Empty)
                || _txtCity.Text != (_originalCustomer.City ?? string.Empty)
                || _txtRegion.Text != (_originalCustomer.Region ?? string.Empty)
                || _txtPostalCode.Text != (_originalCustomer.PostalCode ?? string.Empty)
                || _txtCountry.Text != (_originalCustomer.Country ?? string.Empty)
                || _txtPhone.Text != (_originalCustomer.Phone ?? string.Empty)
                || _txtFax.Text != (_originalCustomer.Fax ?? string.Empty);
        }

        // ── Detail view ─────────────────────────────────────────────────────────

        private void PopulateDetailView(Customer? customer)
        {
            _suppressDirtyTracking = true;

            try
            {
                if (customer is null)
                {
                    _lblCustomerHeader.Text = "(none)";
                    _picCustomer.Image = null;
                    _txtCustomerId.Text = string.Empty;
                    _txtCompanyName.Text = string.Empty;
                    _txtContactName.Text = string.Empty;
                    _txtContactTitle.Text = string.Empty;
                    _txtAddress.Text = string.Empty;
                    _txtCity.Text = string.Empty;
                    _txtRegion.Text = string.Empty;
                    _txtPostalCode.Text = string.Empty;
                    _txtCountry.Text = string.Empty;
                    _txtPhone.Text = string.Empty;
                    _txtFax.Text = string.Empty;
                }
                else
                {
                    _lblCustomerHeader.Text = customer.CustomerId;
                    _picCustomer.Image = GetCustomerImage(customer);
                    _txtCustomerId.Text = customer.CustomerId;
                    _txtCompanyName.Text = customer.CompanyName;
                    _txtContactName.Text = customer.ContactName ?? string.Empty;
                    _txtContactTitle.Text = customer.ContactTitle ?? string.Empty;
                    _txtAddress.Text = customer.Address ?? string.Empty;
                    _txtCity.Text = customer.City ?? string.Empty;
                    _txtRegion.Text = customer.Region ?? string.Empty;
                    _txtPostalCode.Text = customer.PostalCode ?? string.Empty;
                    _txtCountry.Text = customer.Country ?? string.Empty;
                    _txtPhone.Text = customer.Phone ?? string.Empty;
                    _txtFax.Text = customer.Fax ?? string.Empty;
                }
            }
            finally
            {
                _suppressDirtyTracking = false;
            }
        }

        private Customer GetCustomerFromForm()
        {
            return new Customer
            {
                CustomerId = _txtCustomerId.Text.Trim().ToUpperInvariant(),
                CompanyName = _txtCompanyName.Text.Trim(),
                ContactName = string.IsNullOrWhiteSpace(_txtContactName.Text) ? null : _txtContactName.Text.Trim(),
                ContactTitle = string.IsNullOrWhiteSpace(_txtContactTitle.Text) ? null : _txtContactTitle.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(_txtAddress.Text) ? null : _txtAddress.Text.Trim(),
                City = string.IsNullOrWhiteSpace(_txtCity.Text) ? null : _txtCity.Text.Trim(),
                Region = string.IsNullOrWhiteSpace(_txtRegion.Text) ? null : _txtRegion.Text.Trim(),
                PostalCode = string.IsNullOrWhiteSpace(_txtPostalCode.Text) ? null : _txtPostalCode.Text.Trim(),
                Country = string.IsNullOrWhiteSpace(_txtCountry.Text) ? null : _txtCountry.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(_txtPhone.Text) ? null : _txtPhone.Text.Trim(),
                Fax = string.IsNullOrWhiteSpace(_txtFax.Text) ? null : _txtFax.Text.Trim()
            };
        }

        // ── Event handlers ──────────────────────────────────────────────────────

        private void DataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            if (_editMode != EditMode.View)
                return;

            Customer? selected = null;

            if (_dataGridView.SelectedRows.Count == 1
                && _dataGridView.SelectedRows[0].DataBoundItem is Customer customer)
            {
                selected = customer;
            }

            PopulateDetailView(selected);
            UpdateCommandState();
        }

        private void TsbAdd_Click(object? sender, EventArgs e)
        {
            _originalCustomer = null;
            PopulateDetailView(null);
            SetEditMode(EditMode.Add);
            _txtCustomerId.Focus();
        }

        private void TsbEdit_Click(object? sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count != 1)
                return;

            if (_dataGridView.SelectedRows[0].DataBoundItem is not Customer customer)
                return;

            _originalCustomer = customer;
            PopulateDetailView(customer);
            SetEditMode(EditMode.Edit);
            _txtCompanyName.Focus();
        }

        private void TsbCancel_Click(object? sender, EventArgs e)
        {
            SetEditMode(EditMode.View);

            // Restore the detail view to the currently selected row (if any)
            Customer? selected = null;

            if (_dataGridView.SelectedRows.Count == 1
                && _dataGridView.SelectedRows[0].DataBoundItem is Customer customer)
            {
                selected = customer;
            }

            PopulateDetailView(selected);
            UpdateCommandState();
        }

        private async void TsbSaveChanges_Click(object? sender, EventArgs e)
        {
            try
            {
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to save changes:\n\n{ex.Message}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task SaveChangesAsync()
        {
            Customer formCustomer = GetCustomerFromForm();

            if (string.IsNullOrWhiteSpace(formCustomer.CustomerId))
            {
                MessageBox.Show(
                    "Customer ID is required.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                _txtCustomerId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(formCustomer.CompanyName))
            {
                MessageBox.Show(
                    "Company Name is required.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                _txtCompanyName.Focus();
                return;
            }

            EditMode savedMode = _editMode;

            await Task.Run(() =>
            {
                using var context = new NorthwindContext();

                if (savedMode == EditMode.Add)
                {
                    context.Customers.Add(formCustomer);
                }
                else
                {
                    Customer? existing = context.Customers.Find(formCustomer.CustomerId);

                    if (existing is not null)
                    {
                        existing.CompanyName = formCustomer.CompanyName;
                        existing.ContactName = formCustomer.ContactName;
                        existing.ContactTitle = formCustomer.ContactTitle;
                        existing.Address = formCustomer.Address;
                        existing.City = formCustomer.City;
                        existing.Region = formCustomer.Region;
                        existing.PostalCode = formCustomer.PostalCode;
                        existing.Country = formCustomer.Country;
                        existing.Phone = formCustomer.Phone;
                        existing.Fax = formCustomer.Fax;
                    }
                }

                context.SaveChanges();
            });

            _lastChangedCustomer = formCustomer;
            _tsslLastChangedInfo.Text =
                $"{formCustomer.CustomerId}  {formCustomer.CompanyName}  {formCustomer.ContactName ?? string.Empty}";

            await LoadCustomersAsync();
            SetEditMode(EditMode.View);
            SelectCustomerInGrid(formCustomer.CustomerId);
        }

        private void SelectCustomerInGrid(string customerId)
        {
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.DataBoundItem is Customer c && c.CustomerId == customerId)
                {
                    _dataGridView.ClearSelection();
                    row.Selected = true;
                    _dataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private void TsmiExportCsv_Click(object? sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count <= 1)
                return;

            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Export selected customers as CSV",
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = "customers"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                ExportSelectedRowsToCsv(dialog.FileName);

                MessageBox.Show(
                    $"Exported {_dataGridView.SelectedRows.Count} customer(s) to:\n{dialog.FileName}",
                    "Export Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Export failed:\n\n{ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ExportSelectedRowsToCsv(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("CustomerID,CompanyName,ContactName,ContactTitle");

            foreach (DataGridViewRow row in _dataGridView.SelectedRows)
            {
                if (row.DataBoundItem is not Customer c)
                    continue;

                sb.AppendLine(
                    $"{EscapeCsv(c.CustomerId)}," +
                    $"{EscapeCsv(c.CompanyName)}," +
                    $"{EscapeCsv(c.ContactName)}," +
                    $"{EscapeCsv(c.ContactTitle)}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }

        private void TsmiQuit_Click(object? sender, EventArgs e) => Close();

        private void TssbSelect_Click(object? sender, EventArgs e)
        {
            if (_lastChangedCustomer is null)
                return;

            SelectCustomerInGrid(_lastChangedCustomer.CustomerId);
        }

        private void DetailTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (_suppressDirtyTracking)
                return;

            UpdateCommandState();
        }

        private void ClockTimer_Tick(object? sender, EventArgs e) => UpdateClockLabel();

        private void UpdateClockLabel()
        {
            _tsslDateTime.Text = DateTime.Now.ToString("dd MMM yyyy  HH:mm:ss");
        }
    }
}

