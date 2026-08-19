using System.ComponentModel;
using WarpClock.Abstractions;

namespace WarpClock.App;

public partial class ThemeSetEditorDialog : Form
{
    private readonly BindingList<ThemeSetRow> _rows = new();
    private IReadOnlyList<ThemeCatalogInfo> _catalog = Array.Empty<ThemeCatalogInfo>();
    private string? _currentPath;

    public ThemeSetEditorDialog()
    {
        InitializeComponent();

        _themeGrid.AutoGenerateColumns = false;
        _themeGrid.DataSource = _rows;
        _themeGrid.CellFormatting += ThemeGrid_CellFormatting;
        _themeGrid.DataBindingComplete += ThemeGrid_DataBindingComplete;
        _dayStartPicker.Format = DateTimePickerFormat.Custom;
        _dayStartPicker.CustomFormat = "HH:mm";
        _dayStartPicker.ShowUpDown = true;
        _nightStartPicker.Format = DateTimePickerFormat.Custom;
        _nightStartPicker.CustomFormat = "HH:mm";
        _nightStartPicker.ShowUpDown = true;
        _rotationMinutesUpDown.Enabled = _autoRotateCheckBox.Checked;
        _rotationSuffixLabel.Enabled = _autoRotateCheckBox.Checked;
    }

    public ThemeSetEditorDialog(
        ThemeScheduleDocument document,
        IReadOnlyList<ThemeCatalogInfo> catalog,
        string? currentPath,
        string? defaultPath)
        : this()
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(catalog);

        _catalog = catalog;
        _currentPath = currentPath;
        LoadDocument(document, defaultPath);
    }

    public ThemeScheduleDocument EditedDocument { get; private set; } = new();

    public bool UseAsDefaultOnStartup { get; private set; }

    private void LoadDocument(ThemeScheduleDocument document, string? defaultPath)
    {
        document.Normalize();

        _nameTextBox.Text = document.Name;
        _autoRotateCheckBox.Checked = document.AutoRotate;
        _dayStartPicker.Value = DateTime.Today.Add((document.DayStartsAt ?? ThemeScheduleDocument.DefaultDayStartsAt).ToTimeSpan());
        _nightStartPicker.Value = DateTime.Today.Add((document.NightStartsAt ?? ThemeScheduleDocument.DefaultNightStartsAt).ToTimeSpan());
        _rotationMinutesUpDown.Value = Math.Clamp(document.RotationMinutes ?? ThemeScheduleDocument.DefaultRotationMinutes, (int)_rotationMinutesUpDown.Minimum, (int)_rotationMinutesUpDown.Maximum);
        _useAsDefaultPathCheckBox.Checked = !string.IsNullOrWhiteSpace(_currentPath)
            && string.Equals(_currentPath, defaultPath, StringComparison.OrdinalIgnoreCase);

        FillRows(BuildRows(document, _catalog));
        UpdateDisplayedPath();
        ClearValidation();
    }

    private List<ThemeSetRow> BuildRows(ThemeScheduleDocument document, IReadOnlyList<ThemeCatalogInfo> catalog)
    {
        Dictionary<string, ThemeCatalogInfo> catalogByKey = catalog
            .GroupBy(item => ThemeCatalogInfo.NormalizeThemeKey(item.ThemeKey), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);

        var rows = new List<ThemeSetRow>(document.Entries.Count + catalog.Count);
        var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (ThemeScheduleEntry entry in document.Entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Theme.ThemeKey))
            {
                continue;
            }

            string themeKey = ThemeCatalogInfo.NormalizeThemeKey(entry.Theme.ThemeKey);
            seenKeys.Add(themeKey);

            if (catalogByKey.TryGetValue(themeKey, out ThemeCatalogInfo? info))
            {
                bool supportsDay = info.SupportsPeriod(ThemeSchedulePeriod.Day, entry.Theme.Variant);
                bool supportsNight = info.SupportsPeriod(ThemeSchedulePeriod.Night, entry.Theme.Variant);

                rows.Add(new ThemeSetRow
                {
                    ThemeKey = themeKey,
                    Variant = info.SupportsVariant(entry.Theme.Variant ?? ClockThemeVariantKind.Day)
                        ? entry.Theme.Variant
                        : null,
                    DisplayName = info.FamilyName,
                    Source = info.Source,
                    Enabled = entry.Enabled,
                    EligibleDuringDay = supportsDay && entry.EligibleDuringDay,
                    EligibleDuringNight = supportsNight && entry.EligibleDuringNight,
                    SupportsDay = supportsDay,
                    SupportsNight = supportsNight,
                    Status = "Configured",
                });
            }
            else
            {
                rows.Add(new ThemeSetRow
                {
                    ThemeKey = themeKey,
                    Variant = entry.Theme.Variant,
                    DisplayName = string.IsNullOrWhiteSpace(entry.DisplayName) ? themeKey : entry.DisplayName,
                    Source = string.IsNullOrWhiteSpace(entry.Source) ? "missing" : entry.Source,
                    Enabled = entry.Enabled,
                    EligibleDuringDay = entry.EligibleDuringDay,
                    EligibleDuringNight = entry.EligibleDuringNight,
                    SupportsDay = true,
                    SupportsNight = true,
                    Status = "Missing",
                });
            }
        }

        foreach (ThemeCatalogInfo info in catalog)
        {
            if (seenKeys.Contains(info.ThemeKey))
            {
                continue;
            }

            ThemeScheduleEntry defaults = ThemeSetDefaults.CreateDefaultEntry(info);
            defaults.Enabled = false;

            rows.Add(new ThemeSetRow
            {
                ThemeKey = info.ThemeKey,
                DisplayName = info.FamilyName,
                Source = info.Source,
                Enabled = defaults.Enabled,
                EligibleDuringDay = defaults.EligibleDuringDay,
                EligibleDuringNight = defaults.EligibleDuringNight,
                SupportsDay = info.SupportsPeriod(ThemeSchedulePeriod.Day),
                SupportsNight = info.SupportsPeriod(ThemeSchedulePeriod.Night),
                Status = "New",
            });
        }

        return rows;
    }

    private void FillRows(IEnumerable<ThemeSetRow> rows)
    {
        _rows.RaiseListChangedEvents = false;
        _rows.Clear();

        foreach (ThemeSetRow row in rows)
        {
            _rows.Add(row);
        }

        _rows.RaiseListChangedEvents = true;
        _rows.ResetBindings();
        ApplyEligibilityReadOnlyState();
    }

    private void OnResetDefaultsClick(object? sender, EventArgs e)
    {
        ThemeScheduleDocument defaults = ThemeSetDefaults.CreateDefault(_catalog);
        _nameTextBox.Text = defaults.Name;
        FillRows(BuildRows(defaults, _catalog));
        _autoRotateCheckBox.Checked = true;
        _dayStartPicker.Value = DateTime.Today.Add(ThemeScheduleDocument.DefaultDayStartsAt.ToTimeSpan());
        _nightStartPicker.Value = DateTime.Today.Add(ThemeScheduleDocument.DefaultNightStartsAt.ToTimeSpan());
        _rotationMinutesUpDown.Value = ThemeScheduleDocument.DefaultRotationMinutes;
        ClearValidation();
    }

    private void OnAutoRotateCheckedChanged(object? sender, EventArgs e)
    {
        _rotationMinutesUpDown.Enabled = _autoRotateCheckBox.Checked;
        _rotationSuffixLabel.Enabled = _autoRotateCheckBox.Checked;
        ClearValidation();
    }

    private void OnOkClick(object? sender, EventArgs e)
    {
        TimeOnly dayStart = TimeOnly.FromDateTime(_dayStartPicker.Value);
        TimeOnly nightStart = TimeOnly.FromDateTime(_nightStartPicker.Value);

        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            SetValidation("Enter a themeset name.");
            return;
        }

        if (dayStart == nightStart)
        {
            SetValidation("The day and night start times must be different.");
            return;
        }

        if (_autoRotateCheckBox.Checked
            && !_rows.Any(row => row.Enabled && (row.EligibleDuringDay || row.EligibleDuringNight)))
        {
            SetValidation("Enable at least one eligible theme before turning on automatic rotation.");
            return;
        }

        EditedDocument = new ThemeScheduleDocument
        {
            Name = _nameTextBox.Text.Trim(),
            AutoRotate = _autoRotateCheckBox.Checked,
            DayStartsAt = dayStart,
            NightStartsAt = nightStart,
            RotationMinutes = _autoRotateCheckBox.Checked ? (int)_rotationMinutesUpDown.Value : null,
            Entries = _rows.Select(row => new ThemeScheduleEntry
            {
                Theme = new ThemeReference
                {
                    ThemeKey = row.ThemeKey,
                    Variant = row.Variant,
                },
                DisplayName = row.DisplayName,
                Source = row.Source,
                Enabled = row.Enabled,
                EligibleDuringDay = row.SupportsDay && row.EligibleDuringDay,
                EligibleDuringNight = row.SupportsNight && row.EligibleDuringNight,
            }).ToList(),
        };

        EditedDocument.Normalize();
        UseAsDefaultOnStartup = _useAsDefaultPathCheckBox.Checked;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ThemeGrid_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        => ApplyEligibilityReadOnlyState();

    private void ThemeGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count)
        {
            return;
        }

        ThemeSetRow row = _rows[e.RowIndex];
        if (e.ColumnIndex == _statusColumn.Index)
        {
            if (!row.SupportsDay && !row.SupportsNight && !string.Equals(row.Status, "Missing", StringComparison.OrdinalIgnoreCase))
            {
                e.Value = "Unsupported";
                e.FormattingApplied = true;
            }
            else if (!row.SupportsDay || !row.SupportsNight)
            {
                e.Value = row.SupportsDay ? "Day only" : "Night only";
                e.FormattingApplied = true;
            }
        }
    }

    private void ApplyEligibilityReadOnlyState()
    {
        for (int index = 0; index < _themeGrid.Rows.Count && index < _rows.Count; index++)
        {
            DataGridViewRow gridRow = _themeGrid.Rows[index];
            ThemeSetRow row = _rows[index];

            ApplyEligibilityCellState(gridRow.Cells[_dayColumn.Index], row.SupportsDay);
            ApplyEligibilityCellState(gridRow.Cells[_nightColumn.Index], row.SupportsNight);
        }
    }

    private static void ApplyEligibilityCellState(DataGridViewCell cell, bool isSupported)
    {
        cell.ReadOnly = !isSupported;
        cell.Style.BackColor = isSupported ? SystemColors.Window : SystemColors.ControlLight;
        cell.Style.SelectionBackColor = isSupported ? SystemColors.Highlight : SystemColors.ControlLight;
        cell.Style.SelectionForeColor = isSupported ? SystemColors.HighlightText : SystemColors.GrayText;
        cell.Style.ForeColor = isSupported ? SystemColors.ControlText : SystemColors.GrayText;
    }

    private void UpdateDisplayedPath()
    {
        _currentPathValueLabel.Text = string.IsNullOrWhiteSpace(_currentPath)
            ? "Unsaved new themeset (choose a file when saving)."
            : _currentPath;
    }

    private void SetValidation(string message)
    {
        _validationLabel.Text = message;
        _validationLabel.Visible = true;
    }

    private void ClearValidation()
    {
        _validationLabel.Text = string.Empty;
        _validationLabel.Visible = false;
    }

    private sealed class ThemeSetRow
    {
        public string ThemeKey { get; set; } = string.Empty;

        public ClockThemeVariantKind? Variant { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public bool Enabled { get; set; }

        public bool EligibleDuringDay { get; set; } = true;

        public bool EligibleDuringNight { get; set; } = true;

        public bool SupportsDay { get; set; } = true;

        public bool SupportsNight { get; set; } = true;

        public string Status { get; set; } = string.Empty;
    }
}
