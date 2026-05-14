using System.Data;
using System.Globalization;
using System.Text;
using Northwind.DataLayer;

namespace Northwind.App
{
    public partial class FrmMain : Form
    {
        private enum EditMode
        {
            View,
            Add,
            Edit
        }

        private readonly BindingSource _customerBindingSource = new();
        private readonly Dictionary<TextBox, Func<Customer, string?>> _textBoxGetters;
        private readonly Dictionary<TextBox, Action<Customer, string?>> _textBoxSetters;
        private Customer? _selectedCustomer;
        private Customer? _editCustomer;
        private Customer? _lastChangedCustomer;
        private EditMode _editMode = EditMode.View;
        private bool _isDirty;
        private bool _suppressChanges;
        private readonly System.Windows.Forms.Timer _clockTimer = new();
        private readonly ImageFactory _imageFactory = new();

        public FrmMain()
        {
            InitializeComponent();

            _textBoxGetters = new Dictionary<TextBox, Func<Customer, string?>>
            {
                [textBoxCustomerId] = customer => customer.CustomerId,
                [textBoxCompanyName] = customer => customer.CompanyName,
                [textBoxContactName] = customer => customer.ContactName,
                [textBoxContactTitle] = customer => customer.ContactTitle,
                [textBoxAddress] = customer => customer.Address,
                [textBoxCity] = customer => customer.City,
                [textBoxRegion] = customer => customer.Region,
                [textBoxPostalCode] = customer => customer.PostalCode,
                [textBoxCountry] = customer => customer.Country,
                [textBoxPhone] = customer => customer.Phone,
                [textBoxFax] = customer => customer.Fax
            };

            _textBoxSetters = new Dictionary<TextBox, Action<Customer, string?>>
            {
                [textBoxCustomerId] = (customer, value) => customer.CustomerId = value ?? string.Empty,
                [textBoxCompanyName] = (customer, value) => customer.CompanyName = value ?? string.Empty,
                [textBoxContactName] = (customer, value) => customer.ContactName = value,
                [textBoxContactTitle] = (customer, value) => customer.ContactTitle = value,
                [textBoxAddress] = (customer, value) => customer.Address = value,
                [textBoxCity] = (customer, value) => customer.City = value,
                [textBoxRegion] = (customer, value) => customer.Region = value,
                [textBoxPostalCode] = (customer, value) => customer.PostalCode = value,
                [textBoxCountry] = (customer, value) => customer.Country = value,
                [textBoxPhone] = (customer, value) => customer.Phone = value,
                [textBoxFax] = (customer, value) => customer.Fax = value
            };
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            ConfigureToolStrip();
            ConfigureClock();
            LoadCustomers();
            SetEditMode(EditMode.View);
        }

        private void ConfigureGrid()
        {
            dataGridCustomers.AutoGenerateColumns = false;
            dataGridCustomers.Columns.Clear();
            dataGridCustomers.Columns.Add(CreateTextColumn(nameof(Customer.CustomerId), "CustomerId"));
            dataGridCustomers.Columns.Add(CreateTextColumn(nameof(Customer.CompanyName), "CompanyName"));
            dataGridCustomers.Columns.Add(CreateTextColumn(nameof(Customer.ContactName), "ContactName"));
            dataGridCustomers.Columns.Add(CreateTextColumn(nameof(Customer.ContactTitle), "ContactTitle"));
            dataGridCustomers.DataSource = _customerBindingSource;
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(string dataPropertyName, string headerText)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
        }

        private void ConfigureToolStrip()
        {
            toolStripMain.Renderer = new ToolStripProfessionalRenderer();
            toolStripButtonAdd.Image = _imageFactory.CreateAddIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonEdit.Image = _imageFactory.CreateEditIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonCancel.Image = _imageFactory.CreateCancelIcon(toolStripMain.ImageScalingSize, 6);
            toolStripButtonSave.Image = _imageFactory.CreateSaveIcon(toolStripMain.ImageScalingSize, 6);
        }

        private void ConfigureClock()
        {
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (_, _) => UpdateDateTime();
            _clockTimer.Start();
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            statusLabelDateTime.Text = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
        }

        private void LoadCustomers()
        {
            using var context = new NorthwindContext();
            var customers = context.Customers
                .OrderBy(customer => customer.CompanyName)
                .ToList();
            _customerBindingSource.DataSource = customers;
            statusLabelCustomerCount.Text = customers.Count.ToString(CultureInfo.CurrentCulture);
            if (customers.Count > 0)
            {
                dataGridCustomers.ClearSelection();
                dataGridCustomers.Rows[0].Selected = true;
            }
        }

        private void DataGridCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (_editMode != EditMode.View || _suppressChanges)
            {
                return;
            }

            var customer = GetSelectedCustomer();
            _selectedCustomer = customer;
            DisplayCustomer(customer);
            UpdateEnabledState();
        }

        private Customer? GetSelectedCustomer()
        {
            if (dataGridCustomers.SelectedRows.Count != 1)
            {
                return null;
            }

            return dataGridCustomers.SelectedRows[0].DataBoundItem as Customer;
        }

        private void DisplayCustomer(Customer? customer)
        {
            _suppressChanges = true;
            foreach (var pair in _textBoxGetters)
            {
                pair.Key.Text = customer == null ? string.Empty : pair.Value(customer) ?? string.Empty;
            }

            labelHeader.Text = customer == null
                ? "{CustomerID} - {CompanyName}"
                : $"{customer.CustomerId} - {customer.CompanyName}";

            _suppressChanges = false;
            _isDirty = false;
            UpdateEnabledState();
        }

        private void SetEditMode(EditMode mode)
        {
            _editMode = mode;
            dataGridCustomers.Enabled = mode == EditMode.View;

            foreach (var textBox in _textBoxGetters.Keys)
            {
                textBox.Enabled = mode != EditMode.View;
            }

            if (mode == EditMode.View)
            {
                _editCustomer = null;
                DisplayCustomer(GetSelectedCustomer());
            }

            UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            var singleRowSelected = dataGridCustomers.SelectedRows.Count == 1;
            var multiRowSelected = dataGridCustomers.SelectedRows.Count > 1;
            var inViewMode = _editMode == EditMode.View;
            var inEditMode = _editMode != EditMode.View;

            toolStripButtonAdd.Enabled = inViewMode;
            menuItemAdd.Enabled = inViewMode;
            toolStripButtonEdit.Enabled = inViewMode && singleRowSelected;
            menuItemEditSelected.Enabled = inViewMode && singleRowSelected;
            toolStripButtonCancel.Enabled = inEditMode;
            menuItemCancel.Enabled = inEditMode;
            toolStripButtonSave.Enabled = inEditMode && _isDirty;
            menuItemSave.Enabled = inEditMode && _isDirty;
            menuItemExportCsv.Enabled = inViewMode && multiRowSelected;
            statusButtonSelect.Enabled = inViewMode;
        }

        private void BeginAdd()
        {
            _editCustomer = new Customer();
            _selectedCustomer = null;
            DisplayCustomer(_editCustomer);
            SetEditMode(EditMode.Add);
        }

        private void BeginEdit()
        {
            var customer = GetSelectedCustomer();
            if (customer == null)
            {
                return;
            }

            _editCustomer = CloneCustomer(customer);
            DisplayCustomer(_editCustomer);
            SetEditMode(EditMode.Edit);
        }

        private static Customer CloneCustomer(Customer customer)
        {
            return new Customer
            {
                CustomerId = customer.CustomerId,
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                ContactTitle = customer.ContactTitle,
                Address = customer.Address,
                City = customer.City,
                Region = customer.Region,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                Phone = customer.Phone,
                Fax = customer.Fax
            };
        }

        private void CancelEdit()
        {
            SetEditMode(EditMode.View);
        }

        private void DetailTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_suppressChanges || _editMode == EditMode.View)
            {
                return;
            }

            if (sender is TextBox textBox && _editCustomer != null)
            {
                if (_textBoxSetters.TryGetValue(textBox, out var setter))
                {
                    setter(_editCustomer, textBox.Text);
                }
            }

            _isDirty = _editMode == EditMode.Add || HasChanges();
            UpdateEnabledState();
        }

        private bool HasChanges()
        {
            if (_editMode == EditMode.Add)
            {
                return true;
            }

            var original = GetSelectedCustomer();
            if (original == null || _editCustomer == null)
            {
                return false;
            }

            foreach (var pair in _textBoxGetters)
            {
                var currentValue = pair.Value(_editCustomer) ?? string.Empty;
                var originalValue = pair.Value(original) ?? string.Empty;
                if (!string.Equals(currentValue, originalValue, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private void SaveChanges()
        {
            if (_editCustomer == null)
            {
                return;
            }

            using var context = new NorthwindContext();
            if (_editMode == EditMode.Add)
            {
                context.Customers.Add(_editCustomer);
            }
            else if (_editMode == EditMode.Edit)
            {
                context.Customers.Update(_editCustomer);
            }

            context.SaveChanges();
            _lastChangedCustomer = _editCustomer;
            UpdateLastChangedLabel();
            LoadCustomers();
            SetEditMode(EditMode.View);
            SelectCustomerById(_editCustomer.CustomerId);
        }

        private void UpdateLastChangedLabel()
        {
            if (_lastChangedCustomer == null)
            {
                statusLabelLastChangedValue.Text = "{id} {name} {contact}";
                return;
            }

            statusLabelLastChangedValue.Text = $"{_lastChangedCustomer.CustomerId} {_lastChangedCustomer.CompanyName} {_lastChangedCustomer.ContactName}";
        }

        private void SelectCustomerById(string customerId)
        {
            foreach (DataGridViewRow row in dataGridCustomers.Rows)
            {
                if (row.DataBoundItem is Customer customer &&
                    string.Equals(customer.CustomerId, customerId, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    dataGridCustomers.CurrentCell = row.Cells[0];
                    dataGridCustomers.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private void ExportSelectedToCsv()
        {
            if (dataGridCustomers.SelectedRows.Count <= 1)
            {
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FileName = "customers.csv"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("CustomerId,CompanyName,ContactName,ContactTitle");

            foreach (DataGridViewRow row in dataGridCustomers.SelectedRows)
            {
                if (row.DataBoundItem is not Customer customer)
                {
                    continue;
                }

                builder.AppendLine(string.Join(",",
                    EscapeCsv(customer.CustomerId),
                    EscapeCsv(customer.CompanyName),
                    EscapeCsv(customer.ContactName),
                    EscapeCsv(customer.ContactTitle)));
            }

            File.WriteAllText(dialog.FileName, builder.ToString(), Encoding.UTF8);
        }

        private static string EscapeCsv(string? value)
        {
            var safeValue = value ?? string.Empty;
            if (safeValue.Contains(',', StringComparison.Ordinal) || safeValue.Contains('"', StringComparison.Ordinal))
            {
                safeValue = safeValue.Replace("\"", "\"\"", StringComparison.Ordinal);
                return $"\"{safeValue}\"";
            }

            return safeValue;
        }

        private void MenuItemExportCsv_Click(object sender, EventArgs e) => ExportSelectedToCsv();

        private void MenuItemQuit_Click(object sender, EventArgs e) => Close();

        private void MenuItemAdd_Click(object sender, EventArgs e) => BeginAdd();

        private void MenuItemEditSelected_Click(object sender, EventArgs e) => BeginEdit();

        private void MenuItemCancel_Click(object sender, EventArgs e) => CancelEdit();

        private void MenuItemSave_Click(object sender, EventArgs e) => SaveChanges();

        private void ToolStripButtonAdd_Click(object sender, EventArgs e) => BeginAdd();

        private void ToolStripButtonEdit_Click(object sender, EventArgs e) => BeginEdit();

        private void ToolStripButtonCancel_Click(object sender, EventArgs e) => CancelEdit();

        private void ToolStripButtonSave_Click(object sender, EventArgs e) => SaveChanges();

        private void ToolStripMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void StatusButtonSelect_ButtonClick(object sender, EventArgs e)
        {
            if (_lastChangedCustomer == null)
            {
                return;
            }

            SelectCustomerById(_lastChangedCustomer.CustomerId);
        }

        private sealed class ImageFactory
        {
            private readonly Font _iconFont = new("Segoe Fluent Icons", 20, FontStyle.Regular, GraphicsUnit.Pixel);

            public Image CreateAddIcon(Size size, int padding) => CreateIcon("\uE710", size, padding);

            public Image CreateEditIcon(Size size, int padding) => CreateIcon("\uE70F", size, padding);

            public Image CreateCancelIcon(Size size, int padding) => CreateIcon("\uE711", size, padding);

            public Image CreateSaveIcon(Size size, int padding) => CreateIcon("\uE74E", size, padding);

            private Image CreateIcon(string glyph, Size size, int padding)
            {
                var bitmap = new Bitmap(size.Width, size.Height);
                using var graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                var rect = new Rectangle(padding, padding, size.Width - padding * 2, size.Height - padding * 2);
                using var brush = new SolidBrush(Color.DimGray);
                using var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(glyph, _iconFont, brush, rect, format);
                return bitmap;
            }
        }
    }
}
