using System.Text;
using Microsoft.EntityFrameworkCore;
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

        private readonly BindingSource _customersSource = new();
        private readonly System.Windows.Forms.Timer _clockTimer = new();
        private EditMode _editMode = EditMode.View;
        private Customer? _currentCustomer;
        private Customer? _originalCustomer;
        private string? _originalCustomerId;
        private string? _lastChangedCustomerId;
        private bool _isDirty;
        private bool _suppressDirtyCheck;

        private MenuStrip _menuStrip = null!;
        private ToolStrip _toolStrip = null!;
        private ToolStripButton _addButton = null!;
        private ToolStripButton _editButton = null!;
        private ToolStripButton _cancelButton = null!;
        private ToolStripButton _saveButton = null!;
        private ToolStripMenuItem _exportMenuItem = null!;
        private ToolStripMenuItem _addMenuItem = null!;
        private ToolStripMenuItem _editMenuItem = null!;
        private ToolStripMenuItem _cancelMenuItem = null!;
        private ToolStripMenuItem _saveMenuItem = null!;
        private SplitContainer _splitContainer = null!;
        private DataGridView _gridCustomers = null!;
        private Label _detailHeader = null!;
        private TextBox _customerIdTextBox = null!;
        private TextBox _companyNameTextBox = null!;
        private TextBox _contactNameTextBox = null!;
        private TextBox _contactTitleTextBox = null!;
        private TextBox _addressTextBox = null!;
        private TextBox _cityTextBox = null!;
        private TextBox _regionTextBox = null!;
        private TextBox _postalCodeTextBox = null!;
        private TextBox _countryTextBox = null!;
        private TextBox _phoneTextBox = null!;
        private TextBox _faxTextBox = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _customerCountValue = null!;
        private ToolStripStatusLabel _lastChangedValue = null!;
        private ToolStripStatusLabel _selectButton = null!;
        private ToolStripStatusLabel _dateTimeLabel = null!;

        public FrmMain()
        {
            InitializeComponent();
            InitializeUi();
            Load += FrmMain_Load;
        }

        private void InitializeUi()
        {
            Text = "Northwind Customer Editor";
            Name = "FrmMain";
            Width = 1400;
            Height = 900;

            _menuStrip = new MenuStrip
            {
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                Dock = DockStyle.Top
            };

            var fileMenu = new ToolStripMenuItem("File");
            _exportMenuItem = new ToolStripMenuItem("Export as CSV...")
            {
                Enabled = false
            };
            _exportMenuItem.Click += (_, _) => ExportSelected();
            var quitMenuItem = new ToolStripMenuItem("Quit");
            quitMenuItem.Click += (_, _) => Close();
            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                _exportMenuItem,
                new ToolStripSeparator(),
                quitMenuItem
            });

            var editMenu = new ToolStripMenuItem("Edit");
            _addMenuItem = new ToolStripMenuItem("Add new Customer");
            _addMenuItem.Click += (_, _) => EnterAddMode();
            _editMenuItem = new ToolStripMenuItem("Edit selected Customer");
            _editMenuItem.Click += (_, _) => EnterEditMode();
            _cancelMenuItem = new ToolStripMenuItem("Cancel");
            _cancelMenuItem.Click += (_, _) => CancelEdit();
            _saveMenuItem = new ToolStripMenuItem("Save changes");
            _saveMenuItem.Click += (_, _) => SaveChanges();
            editMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                _addMenuItem,
                _editMenuItem,
                _cancelMenuItem,
                new ToolStripSeparator(),
                _saveMenuItem
            });

            _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu });
            MainMenuStrip = _menuStrip;

            _toolStrip = new ToolStrip
            {
                Dock = DockStyle.Top,
                ImageScalingSize = new Size(36, 36),
                GripStyle = ToolStripGripStyle.Hidden
            };

            _addButton = CreateToolStripButton("Add", FluentIconImageFactory.CreateAdd());
            _addButton.Click += (_, _) => EnterAddMode();
            _editButton = CreateToolStripButton("Edit", FluentIconImageFactory.CreateEdit());
            _editButton.Click += (_, _) => EnterEditMode();
            _cancelButton = CreateToolStripButton("Cancel", FluentIconImageFactory.CreateCancel());
            _cancelButton.Click += (_, _) => CancelEdit();
            _saveButton = CreateToolStripButton("Save changes", FluentIconImageFactory.CreateSave());
            _saveButton.Click += (_, _) => SaveChanges();

            _toolStrip.Items.AddRange(new ToolStripItem[] { _addButton, _editButton, _cancelButton, _saveButton });

            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                SplitterDistance = 360
            };

            _gridCustomers = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                MultiSelect = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            _gridCustomers.SelectionChanged += (_, _) => UpdateSelection();

            _gridCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "CustomerId",
                DataPropertyName = "CustomerId",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });
            _gridCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "CompanyName",
                DataPropertyName = "CompanyName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            _gridCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ContactName",
                DataPropertyName = "ContactName",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });
            _gridCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ContactTitle",
                DataPropertyName = "ContactTitle",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            _gridCustomers.DataSource = _customersSource;
            _splitContainer.Panel1.Controls.Add(_gridCustomers);

            var detailPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var detailLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            _detailHeader = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                Text = ""
            };

            var detailTable = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                Dock = DockStyle.Top
            };
            detailTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            detailTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _customerIdTextBox = CreateDetailTextBox();
            _companyNameTextBox = CreateDetailTextBox();
            _contactNameTextBox = CreateDetailTextBox();
            _contactTitleTextBox = CreateDetailTextBox();
            _addressTextBox = CreateDetailTextBox();
            _cityTextBox = CreateDetailTextBox();
            _regionTextBox = CreateDetailTextBox();
            _postalCodeTextBox = CreateDetailTextBox();
            _countryTextBox = CreateDetailTextBox();
            _phoneTextBox = CreateDetailTextBox();
            _faxTextBox = CreateDetailTextBox();

            AddDetailRow(detailTable, "Customer ID", _customerIdTextBox);
            AddDetailRow(detailTable, "Company Name", _companyNameTextBox);
            AddDetailRow(detailTable, "Contact Name", _contactNameTextBox);
            AddDetailRow(detailTable, "Contact Title", _contactTitleTextBox);
            AddDetailRow(detailTable, "Address", _addressTextBox);
            AddDetailRow(detailTable, "City", _cityTextBox);
            AddDetailRow(detailTable, "Region", _regionTextBox);
            AddDetailRow(detailTable, "Postal Code", _postalCodeTextBox);
            AddDetailRow(detailTable, "Country", _countryTextBox);
            AddDetailRow(detailTable, "Phone", _phoneTextBox);
            AddDetailRow(detailTable, "Fax", _faxTextBox);

            detailLayout.Controls.Add(_detailHeader);
            detailLayout.Controls.Add(detailTable);
            detailPanel.Controls.Add(detailLayout);
            _splitContainer.Panel2.Controls.Add(detailPanel);

            _statusStrip = new StatusStrip
            {
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 11f, FontStyle.Regular)
            };

            var customerCountText = new ToolStripStatusLabel("Customers:");
            _customerCountValue = new ToolStripStatusLabel("0");
            var lastChangedText = new ToolStripStatusLabel("Last changed Customer:");
            _lastChangedValue = new ToolStripStatusLabel("-");
            _selectButton = new ToolStripStatusLabel("Select")
            {
                IsLink = true
            };
            _selectButton.Click += (_, _) => SelectLastChangedCustomer();
            _dateTimeLabel = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight
            };

            _statusStrip.Items.AddRange(new ToolStripItem[]
            {
                customerCountText,
                _customerCountValue,
                new ToolStripSeparator(),
                lastChangedText,
                _lastChangedValue,
                _selectButton,
                _dateTimeLabel
            });

            Controls.AddRange(new Control[] { _splitContainer, _statusStrip, _toolStrip, _menuStrip });

            _clockTimer.Interval = 1000;
            _clockTimer.Tick += (_, _) => UpdateDateTime();
            _clockTimer.Start();

            WireDetailChangeEvents();
            UpdateUiState();
            UpdateDateTime();
        }

        private ToolStripButton CreateToolStripButton(string text, Image image)
        {
            return new ToolStripButton(text, image)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                TextImageRelation = TextImageRelation.ImageAboveText
            };
        }

        private TextBox CreateDetailTextBox()
        {
            return new TextBox
            {
                Width = 360,
                Enabled = false
            };
        }

        private void AddDetailRow(TableLayoutPanel table, string labelText, Control editor)
        {
            var rowIndex = table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Padding = new Padding(0, 6, 12, 6)
            };
            editor.Margin = new Padding(0, 3, 0, 3);
            editor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(editor, 1, rowIndex);
        }

        private void FrmMain_Load(object? sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers(string? selectCustomerId = null)
        {
            using var context = new NorthwindContext();
            var customers = context.Customers
                .AsNoTracking()
                .OrderBy(c => c.CompanyName)
                .ToList();

            _customersSource.DataSource = customers;
            _customerCountValue.Text = customers.Count.ToString();

            if (customers.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(selectCustomerId))
                {
                    SelectCustomerById(selectCustomerId);
                }
                else
                {
                    _gridCustomers.ClearSelection();
                    _gridCustomers.Rows[0].Selected = true;
                }
            }
            else
            {
                _currentCustomer = null;
                UpdateDetailFields(null);
            }

            UpdateUiState();
        }

        private void UpdateSelection()
        {
            if (_editMode != EditMode.View)
            {
                UpdateUiState();
                return;
            }

            _currentCustomer = GetSelectedCustomer();
            UpdateDetailFields(_currentCustomer);
            UpdateUiState();
        }

        private Customer? GetSelectedCustomer()
        {
            if (_gridCustomers.SelectedRows.Count == 0)
            {
                return null;
            }

            return _gridCustomers.SelectedRows[0].DataBoundItem as Customer;
        }

        private void UpdateDetailFields(Customer? customer)
        {
            _suppressDirtyCheck = true;
            _customerIdTextBox.Text = customer?.CustomerId ?? string.Empty;
            _companyNameTextBox.Text = customer?.CompanyName ?? string.Empty;
            _contactNameTextBox.Text = customer?.ContactName ?? string.Empty;
            _contactTitleTextBox.Text = customer?.ContactTitle ?? string.Empty;
            _addressTextBox.Text = customer?.Address ?? string.Empty;
            _cityTextBox.Text = customer?.City ?? string.Empty;
            _regionTextBox.Text = customer?.Region ?? string.Empty;
            _postalCodeTextBox.Text = customer?.PostalCode ?? string.Empty;
            _countryTextBox.Text = customer?.Country ?? string.Empty;
            _phoneTextBox.Text = customer?.Phone ?? string.Empty;
            _faxTextBox.Text = customer?.Fax ?? string.Empty;
            _suppressDirtyCheck = false;

            UpdateHeaderLabel(customer);
            _isDirty = false;
        }

        private void UpdateHeaderLabel(Customer? customer)
        {
            if (_editMode == EditMode.Add)
            {
                _detailHeader.Text = "New Customer";
                return;
            }

            if (customer == null)
            {
                _detailHeader.Text = "";
                return;
            }

            _detailHeader.Text = $"{customer.CustomerId} - {customer.CompanyName}";
        }

        private void WireDetailChangeEvents()
        {
            foreach (var textBox in GetDetailTextBoxes())
            {
                textBox.TextChanged += (_, _) => OnDetailChanged();
            }
        }

        private IEnumerable<TextBox> GetDetailTextBoxes()
        {
            yield return _customerIdTextBox;
            yield return _companyNameTextBox;
            yield return _contactNameTextBox;
            yield return _contactTitleTextBox;
            yield return _addressTextBox;
            yield return _cityTextBox;
            yield return _regionTextBox;
            yield return _postalCodeTextBox;
            yield return _countryTextBox;
            yield return _phoneTextBox;
            yield return _faxTextBox;
        }

        private void OnDetailChanged()
        {
            if (_suppressDirtyCheck || _editMode == EditMode.View)
            {
                return;
            }

            _isDirty = HasChanges();
            UpdateUiState();
            UpdateHeaderLabel(GetCustomerFromInputs());
        }

        private bool HasChanges()
        {
            if (_editMode == EditMode.Add)
            {
                return GetDetailTextBoxes().Any(textBox => !string.IsNullOrWhiteSpace(textBox.Text));
            }

            if (_originalCustomer == null)
            {
                return false;
            }

            return !AreEquivalent(_originalCustomer, GetCustomerFromInputs());
        }

        private static bool AreEquivalent(Customer? original, Customer? current)
        {
            if (original == null || current == null)
            {
                return false;
            }

            return AreEqual(original.CustomerId, current.CustomerId)
                && AreEqual(original.CompanyName, current.CompanyName)
                && AreEqual(original.ContactName, current.ContactName)
                && AreEqual(original.ContactTitle, current.ContactTitle)
                && AreEqual(original.Address, current.Address)
                && AreEqual(original.City, current.City)
                && AreEqual(original.Region, current.Region)
                && AreEqual(original.PostalCode, current.PostalCode)
                && AreEqual(original.Country, current.Country)
                && AreEqual(original.Phone, current.Phone)
                && AreEqual(original.Fax, current.Fax);
        }

        private static bool AreEqual(string? left, string? right)
        {
            return string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);
        }

        private Customer GetCustomerFromInputs()
        {
            return new Customer
            {
                CustomerId = _customerIdTextBox.Text.Trim(),
                CompanyName = _companyNameTextBox.Text.Trim(),
                ContactName = NormalizeOptional(_contactNameTextBox.Text),
                ContactTitle = NormalizeOptional(_contactTitleTextBox.Text),
                Address = NormalizeOptional(_addressTextBox.Text),
                City = NormalizeOptional(_cityTextBox.Text),
                Region = NormalizeOptional(_regionTextBox.Text),
                PostalCode = NormalizeOptional(_postalCodeTextBox.Text),
                Country = NormalizeOptional(_countryTextBox.Text),
                Phone = NormalizeOptional(_phoneTextBox.Text),
                Fax = NormalizeOptional(_faxTextBox.Text)
            };
        }

        private static string? NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private void EnterAddMode()
        {
            _editMode = EditMode.Add;
            _originalCustomer = new Customer();
            _originalCustomerId = null;
            _suppressDirtyCheck = true;
            foreach (var textBox in GetDetailTextBoxes())
            {
                textBox.Text = string.Empty;
            }
            _suppressDirtyCheck = false;
            _isDirty = false;
            UpdateHeaderLabel(null);
            UpdateUiState();
        }

        private void EnterEditMode()
        {
            var selectedCustomer = GetSelectedCustomer();
            if (selectedCustomer == null)
            {
                return;
            }

            _editMode = EditMode.Edit;
            _originalCustomer = CloneCustomer(selectedCustomer);
            _originalCustomerId = selectedCustomer.CustomerId;
            _currentCustomer = selectedCustomer;
            UpdateDetailFields(selectedCustomer);
            _isDirty = false;
            UpdateUiState();
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
            _editMode = EditMode.View;
            _isDirty = false;
            _originalCustomer = null;
            _originalCustomerId = null;
            UpdateSelection();
            UpdateUiState();
        }

        private void SaveChanges()
        {
            if (!_isDirty)
            {
                return;
            }

            var customer = GetCustomerFromInputs();
            if (string.IsNullOrWhiteSpace(customer.CustomerId) || string.IsNullOrWhiteSpace(customer.CompanyName))
            {
                MessageBox.Show("Customer ID and Company Name are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using var context = new NorthwindContext();
                if (_editMode == EditMode.Add)
                {
                    context.Customers.Add(customer);
                }
                else if (_editMode == EditMode.Edit)
                {
                    var originalId = _originalCustomerId ?? customer.CustomerId;
                    var existing = context.Customers.SingleOrDefault(c => c.CustomerId == originalId);
                    if (existing == null)
                    {
                        MessageBox.Show("The selected customer could not be found.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    existing.CustomerId = customer.CustomerId;
                    existing.CompanyName = customer.CompanyName;
                    existing.ContactName = customer.ContactName;
                    existing.ContactTitle = customer.ContactTitle;
                    existing.Address = customer.Address;
                    existing.City = customer.City;
                    existing.Region = customer.Region;
                    existing.PostalCode = customer.PostalCode;
                    existing.Country = customer.Country;
                    existing.Phone = customer.Phone;
                    existing.Fax = customer.Fax;
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save changes. {ex.Message}", "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _editMode = EditMode.View;
            _isDirty = false;
            _lastChangedCustomerId = customer.CustomerId;
            _lastChangedValue.Text = $"{customer.CustomerId} {customer.CompanyName} {customer.ContactName}".Trim();
            LoadCustomers(customer.CustomerId);
            UpdateUiState();
        }

        private void ExportSelected()
        {
            if (_gridCustomers.SelectedRows.Count <= 1)
            {
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "customers.csv"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine("CustomerId,CompanyName,ContactName,ContactTitle");
            foreach (DataGridViewRow row in _gridCustomers.SelectedRows)
            {
                if (row.DataBoundItem is not Customer customer)
                {
                    continue;
                }

                builder.AppendLine(string.Join(',', new[]
                {
                    EscapeCsv(customer.CustomerId),
                    EscapeCsv(customer.CompanyName),
                    EscapeCsv(customer.ContactName),
                    EscapeCsv(customer.ContactTitle)
                }));
            }

            File.WriteAllText(dialog.FileName, builder.ToString());
        }

        private static string EscapeCsv(string? value)
        {
            var text = value ?? string.Empty;
            if (text.Contains('"'))
            {
                text = text.Replace("\"", "\"\"");
            }

            return text.Contains(',') || text.Contains('"') || text.Contains('\n')
                ? $"\"{text}\""
                : text;
        }

        private void SelectLastChangedCustomer()
        {
            if (_editMode != EditMode.View || string.IsNullOrWhiteSpace(_lastChangedCustomerId))
            {
                return;
            }

            SelectCustomerById(_lastChangedCustomerId);
        }

        private void SelectCustomerById(string customerId)
        {
            foreach (DataGridViewRow row in _gridCustomers.Rows)
            {
                if (row.DataBoundItem is Customer customer && customer.CustomerId == customerId)
                {
                    _gridCustomers.ClearSelection();
                    row.Selected = true;
                    _gridCustomers.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private void UpdateUiState()
        {
            var inViewMode = _editMode == EditMode.View;
            var selectedCount = _gridCustomers.SelectedRows.Count;

            _gridCustomers.Enabled = inViewMode;

            foreach (var textBox in GetDetailTextBoxes())
            {
                textBox.Enabled = !inViewMode;
            }

            _addButton.Enabled = inViewMode;
            _addMenuItem.Enabled = inViewMode;
            _editButton.Enabled = inViewMode && selectedCount == 1;
            _editMenuItem.Enabled = inViewMode && selectedCount == 1;
            _cancelButton.Enabled = !inViewMode;
            _cancelMenuItem.Enabled = !inViewMode;
            _saveButton.Enabled = !inViewMode && _isDirty;
            _saveMenuItem.Enabled = !inViewMode && _isDirty;
            _exportMenuItem.Enabled = inViewMode && selectedCount > 1;
            _selectButton.Enabled = inViewMode;
        }

        private void UpdateDateTime()
        {
            _dateTimeLabel.Text = DateTime.Now.ToString("g");
        }
    }
}
