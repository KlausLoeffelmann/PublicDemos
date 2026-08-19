namespace WarpClock.App;

public partial class TimeZonesOptionsView : UserControl
{
    private readonly ComboBox[] _timeZoneComboBoxes;
    private readonly TextBox[] _aliasTextBoxes;
    private readonly RadioButton[] _defaultRadioButtons;

    public TimeZonesOptionsView()
    {
        InitializeComponent();

        AutoScroll = true;
        AutoScrollMinSize = Size;

        _timeZoneComboBoxes =
        [
            _timeZone1ComboBox,
            _timeZone2ComboBox,
            _timeZone3ComboBox,
            _timeZone4ComboBox,
            _timeZone5ComboBox,
            _timeZone6ComboBox,
        ];
        _aliasTextBoxes =
        [
            _alias1TextBox,
            _alias2TextBox,
            _alias3TextBox,
            _alias4TextBox,
            _alias5TextBox,
            _alias6TextBox,
        ];
        _defaultRadioButtons =
        [
            _default1RadioButton,
            _default2RadioButton,
            _default3RadioButton,
            _default4RadioButton,
            _default5RadioButton,
            _default6RadioButton,
        ];

        BindTimeZoneChoices();
        ConfigureAccessibility();
        UpdateEnabledState();
        UpdateClockFaceState();
    }

    public void LoadFrom(TimeZoneOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _enabledCheckBox.Checked = options.Enabled;
        _changeEveryNumericUpDown.Value = Math.Clamp(options.ChangeToNextSeconds, (int)_changeEveryNumericUpDown.Minimum, (int)_changeEveryNumericUpDown.Maximum);
        _returnNumericUpDown.Value = Math.Clamp(options.ReturnToDefaultSeconds, (int)_returnNumericUpDown.Minimum, (int)_returnNumericUpDown.Maximum);
        _showOnClockFaceCheckBox.Checked = options.ShowOnClockFace;
        _showOnlyWhenAlternateCheckBox.Checked = options.ShowOnlyWhenAlternate;
        _showHeadlineFallbackCheckBox.Checked = options.ShowHeadlineFallback;

        IReadOnlyList<TimeZoneEditorRow> rows = OptionsDialogModelMapper.CreateTimeZoneRows(options);
        for (int i = 0; i < rows.Count; i++)
        {
            _timeZoneComboBoxes[i].SelectedValue = rows[i].TimeZoneId;
            _aliasTextBoxes[i].Text = rows[i].DisplayName;
            _defaultRadioButtons[i].Checked = rows[i].IsDefault;
        }

        UpdateEnabledState();
        UpdateClockFaceState();
    }

    public bool TryCreateOptions(out TimeZoneOptions options, out string? validationMessage)
    {
        List<TimeZoneEditorRow> rows = [];
        for (int i = 0; i < _timeZoneComboBoxes.Length; i++)
        {
            rows.Add(new TimeZoneEditorRow
            {
                TimeZoneId = _timeZoneComboBoxes[i].SelectedValue as string ?? string.Empty,
                DisplayName = _aliasTextBoxes[i].Text,
                IsDefault = _defaultRadioButtons[i].Checked,
            });
        }

        return OptionsDialogModelMapper.TryCreateTimeZoneOptions(
            _enabledCheckBox.Checked,
            (int)_changeEveryNumericUpDown.Value,
            (int)_returnNumericUpDown.Value,
            _showOnClockFaceCheckBox.Checked,
            _showOnlyWhenAlternateCheckBox.Checked,
            _showHeadlineFallbackCheckBox.Checked,
            rows,
            out options,
            out validationMessage);
    }

    private void BindTimeZoneChoices()
    {
        List<TimeZoneChoice> choices =
        [
            new TimeZoneChoice
            {
                Id = string.Empty,
                DisplayName = "(empty)",
            },
            .. TimeZoneInfo.GetSystemTimeZones()
                .Select(zone => new TimeZoneChoice
                {
                    Id = zone.Id,
                    DisplayName = $"{zone.DisplayName} [{zone.Id}]",
                }),
        ];

        foreach (ComboBox comboBox in _timeZoneComboBoxes)
        {
            comboBox.DisplayMember = nameof(TimeZoneChoice.DisplayName);
            comboBox.ValueMember = nameof(TimeZoneChoice.Id);
            comboBox.DataSource = choices.ToList();
        }
    }

    private void ConfigureAccessibility()
    {
        for (int i = 0; i < _timeZoneComboBoxes.Length; i++)
        {
            int rowNumber = i + 1;
            _defaultRadioButtons[i].AccessibleName = $"Default time zone row {rowNumber}";
            _timeZoneComboBoxes[i].AccessibleName = $"Time zone row {rowNumber}";
            _aliasTextBoxes[i].AccessibleName = $"Time zone alias row {rowNumber}";
        }

        _enabledCheckBox.AccessibleName = "Enable time zone rotation";
        _changeEveryNumericUpDown.AccessibleName = "Change to next time zone seconds";
        _returnNumericUpDown.AccessibleName = "Return to default time zone seconds";
    }

    private void OnEnabledCheckBoxCheckedChanged(object? sender, EventArgs e)
        => UpdateEnabledState();

    private void OnShowOnClockFaceCheckedChanged(object? sender, EventArgs e)
        => UpdateClockFaceState();

    private void UpdateEnabledState()
    {
        bool enabled = _enabledCheckBox.Checked;
        _changeEveryLabel.Enabled = enabled;
        _changeEveryNumericUpDown.Enabled = enabled;
        _changeEverySuffixLabel.Enabled = enabled;
        _returnLabel.Enabled = enabled;
        _returnNumericUpDown.Enabled = enabled;
        _returnSuffixLabel.Enabled = enabled;
    }

    private void UpdateClockFaceState()
    {
        _showOnlyWhenAlternateCheckBox.Enabled = _showOnClockFaceCheckBox.Checked;
        if (!_showOnlyWhenAlternateCheckBox.Enabled)
        {
            _showOnlyWhenAlternateCheckBox.Checked = false;
        }
    }

    private sealed class TimeZoneChoice
    {
        public string Id { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
    }
}
