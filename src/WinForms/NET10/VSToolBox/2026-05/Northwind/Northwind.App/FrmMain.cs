using Northwind.DataLayer;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Northwind.App;

public partial class FrmMain : Form
{
    // ── Edit-mode enum ────────────────────────────────────────────────────────
    private enum EditMode { View, Add, Edit }

    // ── State ─────────────────────────────────────────────────────────────────
    private EditMode          _mode            = EditMode.View;
    private List<Customer>    _customers       = [];
    private Customer?         _editingCustomer;           // original snapshot for dirty-check
    private Customer?         _lastChangedCustomer;
    private bool              _suppressDirty;

    // ── Construction ──────────────────────────────────────────────────────────
    public FrmMain()
    {
        InitializeComponent();
        WireEvents();
    }

    // ── Wire-up ───────────────────────────────────────────────────────────────
    private void WireEvents()
    {
        Load += FrmMain_Load;

        // Menu
        _mnuAdd.Click       += (_, _) => BeginAdd();
        _mnuEdit.Click      += (_, _) => BeginEdit();
        _mnuCancel.Click    += (_, _) => CancelEdit();
        _mnuSave.Click      += (_, _) => SaveChanges();
        _mnuExportCsv.Click += (_, _) => ExportCsv();
        _mnuQuit.Click      += (_, _) => Close();

        // ToolStrip
        _tsbAdd.Click    += (_, _) => BeginAdd();
        _tsbEdit.Click   += (_, _) => BeginEdit();
        _tsbCancel.Click += (_, _) => CancelEdit();
        _tsbSave.Click   += (_, _) => SaveChanges();

        // Grid selection
        _grid.SelectionChanged += Grid_SelectionChanged;

        // Detail TextBox dirty-tracking
        foreach (var tb in DetailTextBoxes())
            tb.TextChanged += Detail_TextChanged;

        // Status bar Select button
        _ssBtnSelect.Click += (_, _) => SelectLastChangedInGrid();

        // Clock
        _clockTimer.Tick += (_, _) => _ssLblDateTime.Text = DateTime.Now.ToString("d MMM yyyy   HH:mm:ss");
        _clockTimer.Start();
        _ssLblDateTime.Text = DateTime.Now.ToString("d MMM yyyy   HH:mm:ss");
    }

    // ── Load ──────────────────────────────────────────────────────────────────
    private void FrmMain_Load(object? sender, EventArgs e)
    {
        LoadCustomers();
        ApplyMode();
    }

    // ── Data ──────────────────────────────────────────────────────────────────
    private void LoadCustomers()
    {
        using var ctx = new NorthwindContext();
        _customers = [.. ctx.Customers
            .OrderBy(c => c.CompanyName)];

        BindGrid();
        _ssLblCustomerCount.Text = _customers.Count.ToString();
    }

    private void BindGrid()
    {
        var displayed = _customers.Select(c => new
        {
            c.CustomerId,
            c.CompanyName,
            c.ContactName,
            c.ContactTitle
        }).ToList();

        _grid.DataSource = null;
        _grid.DataSource = displayed;

        if (_grid.Columns.Count >= 4)
        {
            _grid.Columns[0].HeaderText = "Customer ID";
            _grid.Columns[1].HeaderText = "Company Name";
            _grid.Columns[2].HeaderText = "Contact Name";
            _grid.Columns[3].HeaderText = "Contact Title";
        }
    }

    // ── Grid selection ────────────────────────────────────────────────────────
    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_mode != EditMode.View) return;

        var customer = SelectedCustomer();
        PopulateDetail(customer);
        ApplyMode();
    }

    private Customer? SelectedCustomer()
    {
        if (_grid.SelectedRows.Count == 0) return null;
        var id = _grid.SelectedRows[0].Cells["CustomerId"].Value?.ToString();
        return _customers.FirstOrDefault(c => c.CustomerId == id);
    }

    // ── Detail panel ──────────────────────────────────────────────────────────
    private void PopulateDetail(Customer? c)
    {
        _suppressDirty = true;
        if (c is null)
        {
            _lblDetailHeader.Text = string.Empty;
            ClearDetailFields();
        }
        else
        {
            _lblDetailHeader.Text  = $"{c.CustomerId} – {c.CompanyName}";
            _txtCustomerId.Text    = c.CustomerId;
            _txtCompanyName.Text   = c.CompanyName;
            _txtContactName.Text   = c.ContactName   ?? string.Empty;
            _txtContactTitle.Text  = c.ContactTitle  ?? string.Empty;
            _txtAddress.Text       = c.Address       ?? string.Empty;
            _txtCity.Text          = c.City          ?? string.Empty;
            _txtRegion.Text        = c.Region        ?? string.Empty;
            _txtPostalCode.Text    = c.PostalCode     ?? string.Empty;
            _txtCountry.Text       = c.Country       ?? string.Empty;
            _txtPhone.Text         = c.Phone         ?? string.Empty;
            _txtFax.Text           = c.Fax           ?? string.Empty;
        }
        _suppressDirty = false;
    }

    private void ClearDetailFields()
    {
        _lblDetailHeader.Text = string.Empty;
        foreach (var tb in DetailTextBoxes()) tb.Text = string.Empty;
    }

    // ── Dirty tracking ────────────────────────────────────────────────────────
    private void Detail_TextChanged(object? sender, EventArgs e)
    {
        if (_suppressDirty || _mode == EditMode.View) return;
        var dirty = IsDirty();
        _mnuSave.Enabled = dirty;
        _tsbSave.Enabled = dirty;
    }

    private bool IsDirty()
    {
        if (_editingCustomer is null) return true;   // new record — any content counts
        return _txtCustomerId.Text   != _editingCustomer.CustomerId
            || _txtCompanyName.Text  != _editingCustomer.CompanyName
            || _txtContactName.Text  != (_editingCustomer.ContactName  ?? string.Empty)
            || _txtContactTitle.Text != (_editingCustomer.ContactTitle ?? string.Empty)
            || _txtAddress.Text      != (_editingCustomer.Address      ?? string.Empty)
            || _txtCity.Text         != (_editingCustomer.City         ?? string.Empty)
            || _txtRegion.Text       != (_editingCustomer.Region       ?? string.Empty)
            || _txtPostalCode.Text   != (_editingCustomer.PostalCode   ?? string.Empty)
            || _txtCountry.Text      != (_editingCustomer.Country      ?? string.Empty)
            || _txtPhone.Text        != (_editingCustomer.Phone        ?? string.Empty)
            || _txtFax.Text          != (_editingCustomer.Fax          ?? string.Empty);
    }

    // ── Add / Edit / Cancel ───────────────────────────────────────────────────
    private void BeginAdd()
    {
        _editingCustomer = null;
        _mode = EditMode.Add;
        ClearDetailFields();
        _txtCustomerId.Enabled = true;   // new record needs an ID
        ApplyMode();
        _txtCustomerId.Focus();
    }

    private void BeginEdit()
    {
        var c = SelectedCustomer();
        if (c is null) return;

        // Keep a snapshot for dirty-checking
        _editingCustomer = new Customer
        {
            CustomerId   = c.CustomerId,
            CompanyName  = c.CompanyName,
            ContactName  = c.ContactName,
            ContactTitle = c.ContactTitle,
            Address      = c.Address,
            City         = c.City,
            Region       = c.Region,
            PostalCode   = c.PostalCode,
            Country      = c.Country,
            Phone        = c.Phone,
            Fax          = c.Fax
        };

        _mode = EditMode.Edit;
        ApplyMode();
        _txtCompanyName.Focus();
    }

    private void CancelEdit()
    {
        _mode = EditMode.View;
        _editingCustomer = null;
        PopulateDetail(SelectedCustomer());
        ApplyMode();
    }

    // ── Save ──────────────────────────────────────────────────────────────────
    private void SaveChanges()
    {
        if (_txtCompanyName.Text.Trim().Length == 0)
        {
            MessageBox.Show("Company Name is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtCompanyName.Focus();
            return;
        }
        if (_txtCustomerId.Text.Trim().Length == 0)
        {
            MessageBox.Show("Customer ID is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtCustomerId.Focus();
            return;
        }

        try
        {
            using var ctx = new NorthwindContext();

            if (_mode == EditMode.Add)
            {
                var c = BuildCustomerFromFields();
                ctx.Customers.Add(c);
                ctx.SaveChanges();
                _lastChangedCustomer = c;
            }
            else // Edit
            {
                var id = _editingCustomer!.CustomerId;
                var c  = ctx.Customers.Find(id);
                if (c is null)
                {
                    MessageBox.Show("Customer not found — it may have been deleted.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ApplyFieldsToCustomer(c);
                ctx.SaveChanges();
                _lastChangedCustomer = c;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Save failed:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        UpdateLastChangedStatus();
        _mode = EditMode.View;
        _editingCustomer = null;
        LoadCustomers();          // refresh + rebind
        SelectLastChangedInGrid();
        ApplyMode();
    }

    private Customer BuildCustomerFromFields()
        => new()
        {
            CustomerId   = _txtCustomerId.Text.Trim().ToUpperInvariant(),
            CompanyName  = _txtCompanyName.Text.Trim(),
            ContactName  = NullIfEmpty(_txtContactName.Text),
            ContactTitle = NullIfEmpty(_txtContactTitle.Text),
            Address      = NullIfEmpty(_txtAddress.Text),
            City         = NullIfEmpty(_txtCity.Text),
            Region       = NullIfEmpty(_txtRegion.Text),
            PostalCode   = NullIfEmpty(_txtPostalCode.Text),
            Country      = NullIfEmpty(_txtCountry.Text),
            Phone        = NullIfEmpty(_txtPhone.Text),
            Fax          = NullIfEmpty(_txtFax.Text)
        };

    private void ApplyFieldsToCustomer(Customer c)
    {
        c.CompanyName  = _txtCompanyName.Text.Trim();
        c.ContactName  = NullIfEmpty(_txtContactName.Text);
        c.ContactTitle = NullIfEmpty(_txtContactTitle.Text);
        c.Address      = NullIfEmpty(_txtAddress.Text);
        c.City         = NullIfEmpty(_txtCity.Text);
        c.Region       = NullIfEmpty(_txtRegion.Text);
        c.PostalCode   = NullIfEmpty(_txtPostalCode.Text);
        c.Country      = NullIfEmpty(_txtCountry.Text);
        c.Phone        = NullIfEmpty(_txtPhone.Text);
        c.Fax          = NullIfEmpty(_txtFax.Text);
    }

    // ── Status bar ────────────────────────────────────────────────────────────
    private void UpdateLastChangedStatus()
    {
        if (_lastChangedCustomer is null) return;
        _ssLblLastChanged.Text = $"{_lastChangedCustomer.CustomerId}  {_lastChangedCustomer.CompanyName}  {_lastChangedCustomer.ContactName}";
    }

    private void SelectLastChangedInGrid()
    {
        if (_lastChangedCustomer is null || _mode != EditMode.View) return;

        for (int i = 0; i < _grid.Rows.Count; i++)
        {
            if (_grid.Rows[i].Cells["CustomerId"].Value?.ToString() == _lastChangedCustomer.CustomerId)
            {
                _grid.ClearSelection();
                _grid.Rows[i].Selected = true;
                _grid.FirstDisplayedScrollingRowIndex = i;
                break;
            }
        }
    }

    // ── Export CSV ────────────────────────────────────────────────────────────
    private void ExportCsv()
    {
        if (_grid.SelectedRows.Count <= 1) return;

        using var dlg = new SaveFileDialog
        {
            Title            = "Export selected customers",
            Filter           = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            DefaultExt       = "csv",
            FileName         = "customers.csv"
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var sb = new StringBuilder();
        sb.AppendLine("CustomerID,CompanyName,ContactName,ContactTitle");

        foreach (DataGridViewRow row in _grid.SelectedRows)
        {
            sb.AppendLine(string.Join(",",
                CsvEscape(row.Cells["CustomerId"].Value?.ToString()),
                CsvEscape(row.Cells["CompanyName"].Value?.ToString()),
                CsvEscape(row.Cells["ContactName"].Value?.ToString()),
                CsvEscape(row.Cells["ContactTitle"].Value?.ToString())));
        }

        File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
        MessageBox.Show($"Exported {_grid.SelectedRows.Count} rows.", "Export",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // ── Enable/Disable rules ──────────────────────────────────────────────────
    private void ApplyMode()
    {
        bool isView       = _mode == EditMode.View;
        bool isAddEdit    = !isView;
        bool oneSelected  = _grid.SelectedRows.Count == 1;
        bool manySelected = _grid.SelectedRows.Count > 1;

        // Grid
        _grid.Enabled = isView;

        // Detail TextBoxes
        foreach (var tb in DetailTextBoxes())
            tb.Enabled = isAddEdit;

        // Customer ID is only editable when adding
        _txtCustomerId.Enabled = _mode == EditMode.Add;

        bool dirty = isAddEdit && IsDirty();

        // Add
        _tsbAdd.Enabled  = isView;
        _mnuAdd.Enabled  = isView;

        // Edit
        _tsbEdit.Enabled  = isView && oneSelected;
        _mnuEdit.Enabled  = isView && oneSelected;

        // Cancel
        _tsbCancel.Enabled = isAddEdit;
        _mnuCancel.Enabled = isAddEdit;

        // Save
        _tsbSave.Enabled = dirty;
        _mnuSave.Enabled = dirty;

        // Export
        _mnuExportCsv.Enabled = isView && manySelected;

        // Status Select
        _ssBtnSelect.Enabled = isView;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private IEnumerable<TextBox> DetailTextBoxes()
    {
        yield return _txtCustomerId;
        yield return _txtCompanyName;
        yield return _txtContactName;
        yield return _txtContactTitle;
        yield return _txtAddress;
        yield return _txtCity;
        yield return _txtRegion;
        yield return _txtPostalCode;
        yield return _txtCountry;
        yield return _txtPhone;
        yield return _txtFax;
    }

    private static string? NullIfEmpty(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static string CsvEscape(string? value)
    {
        if (value is null) return string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

