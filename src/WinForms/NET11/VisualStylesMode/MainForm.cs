// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Text.Json;
using VisualStylesModeDemo.Components;
using VisualStylesModeDemo.Views;
using Windows.UI.ViewManagement;

namespace VisualStylesModeDemo;

/// <summary>
///  Host shell for a growing set of exploratory .NET 11 WinForms scenarios (tracked in
///  https://github.com/dotnet/winforms/issues/14694). Panel1 of the SplitContainer swaps between
///  registered <see cref="IScenarioView"/> UserControls via the View menu; Panel2 always hosts the
///  same PropertyGrid, which is driven by the shell's transparent Edit-mode adorner.
/// </summary>
/// <remarks>
///  <para>
///   To add another exploratory view (e.g. TreeView.NodeLeading, Application.SystemTextSize
///   live-update), create a new UserControl implementing <see cref="IScenarioView"/> under Views\
///   and add one line to <see cref="CreateViews"/> below.
///  </para>
/// </remarks>
public partial class MainForm : Form
{
    private const float BaseUiFontSize = 11F;

    private readonly List<(IScenarioView Scenario, ToolStripMenuItem MenuItem)> _views = [];
    private readonly ControlSelectionAdornerForm _selectionAdorner;
    private readonly UISettings _uiSettings = new();
    private IScenarioView? _activeView;
    private Font? _scaledUiFont;
    private bool _editModeEnabled;
    private Color _accentColor = SystemColors.Highlight;
    private VisualStylesMode _selectedVisualStylesMode = VisualStylesMode.Net11;
    private FlatStyle _selectedFlatStyle = FlatStyle.Standard;

    public MainForm()
    {
        InitializeComponent();

        _selectionAdorner = new ControlSelectionAdornerForm();
        _selectionAdorner.SelectionChanged += SelectionAdorner_SelectionChanged;
        components!.Add(_selectionAdorner);

        ApplySystemTextSize();
        UpdateFormSizeStatusLabels();
        UpdateSystemAppearance();

        CreateViews();

        if (_views.Count > 0)
        {
            SwitchToView(_views[0].Scenario);
        }

        UpdateSelectionUi();
        _systemAppearanceTimer.Start();
    }

    /// <summary>
    ///  Registers every exploratory scenario view and builds the corresponding View menu items.
    ///  Add new views here - one line per view.
    /// </summary>
    private void CreateViews()
    {
        RegisterView(new TextBoxScenariosView());
        RegisterView(new ButtonVisualStylesView());
        RegisterView(new CheckBoxRadioButtonVisualStylesView());
        RegisterView(new CashRegisterView());
        RegisterView(new CustomerEntryView());
        RegisterView(new ParallelAnimationView());

        // Future views, following the exact same pattern:
        //   RegisterView(new TreeViewNodeLeadingScenariosView());
        //   RegisterView(new SystemTextSizeScenariosView());
    }

    private void RegisterView(UserControl view)
    {
        if (view is not IScenarioView scenario)
        {
            throw new ArgumentException($"{view.GetType()} must implement {nameof(IScenarioView)}.", nameof(view));
        }

        ToolStripMenuItem menuItem = new()
        {
            Text = scenario.DisplayName,
            CheckOnClick = false,
        };

        menuItem.Click += (_, _) => SwitchToView(scenario);

        _viewToolStripMenuItem.DropDownItems.Insert(_views.Count, menuItem);
        _views.Add((scenario, menuItem));

        // Let the shared IContainer own disposal of every view, whether active or not, so GDI
        // resources (e.g. the generated background-image Bitmaps) are cleaned up with the form.
        components.Add(view);
    }

    private void SwitchToView(IScenarioView scenario)
    {
        try
        {
            if (ReferenceEquals(_activeView, scenario))
            {
                return;
            }

            using var scope = this.SuspendPainting(LayoutSuspendTraversal.Traverse);

            if (_activeView is not null)
            {
                if (_editModeEnabled)
                {
                    _selectionAdorner.DeactivateAndClear();
                }

                ((Control)_activeView).Visible = false;
                _splitContainer.Panel1.Controls.Remove((Control)_activeView);
            }

            _activeView = scenario;

            _splitContainer.Panel1.SuspendLayout();
            Control viewControl = (Control)scenario;
            _activeView.SuspendLayout();

            // Set AutoScaleMode to Inherited, if we're dealing with a container control
            // so we're not running the DPI layout logic twice.
            if (viewControl is ContainerControl viewAsContainer)
            {
                viewAsContainer.AutoScaleMode = AutoScaleMode.Inherit;
            }

            if (_scaledUiFont is not null)
            {
                viewControl.Font = _scaledUiFont;
            }

            foreach ((IScenarioView candidate, ToolStripMenuItem menuItem) in _views)
            {
                menuItem.Checked = ReferenceEquals(candidate, scenario);
            }

            viewControl.Dock = DockStyle.Fill;
            ApplyVisualStylesModeRecursively(viewControl, _selectedVisualStylesMode);

            if (scenario is IFlatStyleScenarioView flatStyleScenario)
            {
                flatStyleScenario.ApplyFlatStyle(_selectedFlatStyle);
            }

            _splitContainer.Panel1.Controls.Add(viewControl);
            viewControl.Visible = true;

            if (_editModeEnabled)
            {
                _selectionAdorner.Activate(this, viewControl, _splitContainer.Panel1);
            }
        }
        finally
        {
            _splitContainer.Panel1.ResumeLayout(true);
            _activeView?.ResumeLayout();
        }

        UpdateViewAppearanceMenu();
        UpdateSelectionUi();
    }

    private void ClassicVisualStylesToolStripMenuItem_Click(object sender, EventArgs e) 
        => SetVisualStylesMode(VisualStylesMode.Classic);

    private void Net11VisualStylesToolStripMenuItem_Click(object sender, EventArgs e) 
        => SetVisualStylesMode(VisualStylesMode.Net11);

    private void StandardFlatStyleToolStripMenuItem_Click(object sender, EventArgs e) 
        => SetFlatStyle(FlatStyle.Standard);

    private void FlatFlatStyleToolStripMenuItem_Click(object sender, EventArgs e) 
        => SetFlatStyle(FlatStyle.Flat);

    private void PopupFlatStyleToolStripMenuItem_Click(object sender, EventArgs e) 
        => SetFlatStyle(FlatStyle.Popup);

    private void SystemFlatStyleToolStripMenuItem_Click(object sender, EventArgs e) 
        => SetFlatStyle(FlatStyle.System);

    private void SetVisualStylesMode(VisualStylesMode visualStylesMode)
    {
        _selectedVisualStylesMode = visualStylesMode;
        if (_activeView is Control activeView)
        {
            ApplyVisualStylesModeRecursively(activeView, visualStylesMode);
        }

        if (_activeView is IFlatStyleScenarioView flatStyleScenario)
        {
            flatStyleScenario.ApplyFlatStyle(_selectedFlatStyle);
        }

        UpdateViewAppearanceMenu();
        _selectionAdorner.SynchronizeBoundsAndRender();
    }

    private void SetFlatStyle(FlatStyle flatStyle)
    {
        _selectedFlatStyle = flatStyle;

        if (_activeView is IFlatStyleScenarioView flatStyleScenario)
        {
            flatStyleScenario.ApplyFlatStyle(flatStyle);
        }

        UpdateViewAppearanceMenu();
        _selectionAdorner.SynchronizeBoundsAndRender();
    }

    private static void ApplyVisualStylesModeRecursively(Control control, VisualStylesMode visualStylesMode)
    {
        control.VisualStylesMode = visualStylesMode;
        foreach (Control child in control.Controls)
        {
            ApplyVisualStylesModeRecursively(child, visualStylesMode);
        }
    }

    private void UpdateViewAppearanceMenu()
    {
        _classicVisualStylesToolStripMenuItem.Checked = _selectedVisualStylesMode == VisualStylesMode.Classic;
        _net11VisualStylesToolStripMenuItem.Checked = _selectedVisualStylesMode == VisualStylesMode.Net11;

        bool supportsFlatStyle = _activeView is IFlatStyleScenarioView;
        _standardFlatStyleToolStripMenuItem.Enabled = supportsFlatStyle;
        _flatFlatStyleToolStripMenuItem.Enabled = supportsFlatStyle;
        _popupFlatStyleToolStripMenuItem.Enabled = supportsFlatStyle;
        _systemFlatStyleToolStripMenuItem.Enabled = supportsFlatStyle;

        _standardFlatStyleToolStripMenuItem.Checked = _selectedFlatStyle == FlatStyle.Standard;
        _flatFlatStyleToolStripMenuItem.Checked = _selectedFlatStyle == FlatStyle.Flat;
        _popupFlatStyleToolStripMenuItem.Checked = _selectedFlatStyle == FlatStyle.Popup;
        _systemFlatStyleToolStripMenuItem.Checked = _selectedFlatStyle == FlatStyle.System;
    }

    private void SelectionAdorner_SelectionChanged(object? sender, EventArgs e)
        => UpdateSelectionUi();

    private void EditModeToolStripMenuItem_Click(object sender, EventArgs e)
        => SetEditMode(!_editModeEnabled);

    private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        => _selectionAdorner.SelectAll();

    private void DeselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        => _selectionAdorner.ClearSelection();

    private void SetEditMode(bool enabled)
    {
        if (_editModeEnabled == enabled)
        {
            return;
        }

        _editModeEnabled = enabled;

        if (enabled)
        {
            _splitContainer.Panel2Collapsed = false;

            if (_activeView is Control viewControl)
            {
                _selectionAdorner.Activate(this, viewControl, _splitContainer.Panel1);
            }
        }
        else
        {
            _selectionAdorner.DeactivateAndClear();
            _propertyGrid.SelectedObjects = [];
            _splitContainer.Panel2Collapsed = true;
        }

        UpdateSelectionUi();
    }

    private void UpdateSelectionUi()
    {
        Control[] selected = [.. _selectionAdorner.SelectedControls];
        _propertyGrid.SelectedObjects = selected;

        _selectedControlStatusLabel.Text = !_editModeEnabled
            ? $"{_activeView?.DisplayName}: Edit mode off"
            : selected.Length switch
            {
                0 => $"{_activeView?.DisplayName}: no controls selected",
                1 => $"{_activeView?.DisplayName}: {GetControlDisplayName(selected[0])} selected",
                _ => $"{_activeView?.DisplayName}: {selected.Length} controls selected",
            };

        bool hasActiveView = _activeView is not null;
        bool hasSelection = selected.Length > 0;

        _editModeToolStripMenuItem.Checked = _editModeEnabled;
        _editModeToolStripButton.Checked = _editModeEnabled;
        _selectAllToolStripMenuItem.Enabled = _editModeEnabled && hasActiveView;
        _selectAllToolStripButton.Enabled = _editModeEnabled && hasActiveView;
        _deselectAllToolStripMenuItem.Enabled = _editModeEnabled && hasSelection;
        _deselectAllToolStripButton.Enabled = _editModeEnabled && hasSelection;
        _saveSettingsToolStripMenuItem.Enabled = hasSelection;
        _saveSettingsToolStripButton.Enabled = hasSelection;
    }

    private static string GetControlDisplayName(Control control)
        => string.IsNullOrEmpty(control.Name)
            ? control.GetType().Name
            : control.Name;

    private void ApplyToolStripImages()
    {
        Color iconColor = SystemColors.ControlText;
        _iconFactoryComponent.SetImage(_saveSettingsToolStripButton, SymbolGlyph.Save, 36, DeviceDpi, iconColor);
        _iconFactoryComponent.SetImage(_loadSettingsToolStripButton, SymbolGlyph.OpenFile, 36, DeviceDpi, iconColor);
        _iconFactoryComponent.SetImage(_editModeToolStripButton, SymbolGlyph.Edit, 36, DeviceDpi, iconColor);
        _iconFactoryComponent.SetImage(_selectAllToolStripButton, SymbolGlyph.SelectAll, 36, DeviceDpi, iconColor);
        _iconFactoryComponent.SetImage(_deselectAllToolStripButton, SymbolGlyph.ClearSelection, 36, DeviceDpi, iconColor);
    }

    private void PropertyGrid_PropertyValueChanged(object? sender, PropertyValueChangedEventArgs e)
    {
        _splitContainer.Panel1.Refresh();
        _selectionAdorner.SynchronizeBoundsAndRender();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateFormSizeStatusLabels();
        UpdateScaleStatusLabels();
    }

    /// <summary>
    ///  Restores the persisted window position/size/state. This runs in <see cref="OnLoad"/> after
    ///  <c>base.OnLoad</c> so the restored bounds are not replaced by a later AutoScale layout pass.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ApplyToolStripImages();

        WindowSettings? settings = TryLoadSettings();

        if (settings is null)
        {
            return;
        }

        Rectangle bounds = new(settings.X, settings.Y, settings.Width, settings.Height);

        if (IsOnScreen(bounds))
        {
            StartPosition = FormStartPosition.Manual;
            Bounds = bounds; // MinimumSize is enforced automatically by WinForms.

            if (settings.Maximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }
    }

    /// <summary>Persists the window position, size, and state as the form closes.</summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _systemAppearanceTimer.Stop();
        _selectionAdorner.DeactivateAndClear();
        SaveSettings();
        base.OnFormClosing(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        _scaledUiFont?.Dispose();
        _scaledUiFont = null;
    }

    /// <summary>The auto-managed settings file: <c>%APPDATA%\VisualStylesModeDemo\window.json</c>.</summary>
    private static string GetSettingsFilePath() =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "VisualStylesModeDemo",
            "window.json");

    private static WindowSettings? TryLoadSettings()
    {
        try
        {
            string path = GetSettingsFilePath();

            if (!File.Exists(path))
            {
                return null;
            }

            return JsonSerializer.Deserialize<WindowSettings>(File.ReadAllText(path));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            // A missing/corrupt/inaccessible settings file must never prevent the app from starting.
            return null;
        }
    }

    private void SaveSettings()
    {
        // When maximized, Bounds is the maximized rectangle while RestoreBounds is the "normal" size to
        // return to - persist the latter. Minimized is likewise normalized to its normal restore bounds.
        Rectangle bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;

        WindowSettings settings = new()
        {
            X = bounds.X,
            Y = bounds.Y,
            Width = bounds.Width,
            Height = bounds.Height,
            Maximized = WindowState == FormWindowState.Maximized,
        };

        try
        {
            string path = GetSettingsFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            // Persisting window state is best-effort for this scratch tool; never block app exit.
        }
    }

    /// <summary>
    ///  True if <paramref name="bounds"/> meaningfully overlaps some screen's working area, so a window
    ///  saved on a now-disconnected monitor doesn't get restored off-screen.
    /// </summary>
    private static bool IsOnScreen(Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return false;
        }

        foreach (Screen screen in Screen.AllScreens)
        {
            Rectangle overlap = Rectangle.Intersect(screen.WorkingArea, bounds);

            if (overlap.Width >= 100 && overlap.Height >= 50)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Small JSON-serialized snapshot of the window's restore state.</summary>
    private sealed record WindowSettings
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public bool Maximized { get; init; }
    }

    private void UpdateFormSizeStatusLabels()
    {
        _formSizeStatusLabel.Text = $"Form size: {Size.Width} x {Size.Height}";
        _formClientSizeStatusLabel.Text = $"Form client size: {ClientSize.Width} x {ClientSize.Height}";
    }

    /// <summary>
    ///  Saves the writable, string-convertible property values of every currently selected control
    ///  in the active view to a small JSON file, keyed by control name.
    /// </summary>
    private void SaveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Control[] selected = _selectionAdorner.SelectedControls.ToArray();
        if (selected.Length == 0)
        {
            MessageBox.Show(
                this,
                "Enable Edit mode and double-click at least one control before saving its property settings.",
                Text);
            return;
        }

        using SaveFileDialog dialog = new()
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = "VisualStylesModeDemo.settings.json",
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        Dictionary<string, Dictionary<string, string>> data = [];

        foreach (Control control in selected)
        {
            if (string.IsNullOrEmpty(control.Name))
            {
                continue;
            }

            data[control.Name] = CapturePropertyValues(control);
        }

        File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>
    ///  Loads property values previously written by <see cref="SaveSettingsToolStripMenuItem_Click"/>
    ///  and applies them to same-named controls found anywhere in the active view.
    /// </summary>
    private void LoadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_activeView is null)
        {
            return;
        }

        using OpenFileDialog dialog = new()
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        Dictionary<string, Dictionary<string, string>>? data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(
            File.ReadAllText(dialog.FileName));

        if (data is null)
        {
            return;
        }

        Control viewControl = (Control)_activeView;

        foreach ((string controlName, Dictionary<string, string> properties) in data)
        {
            Control[] matches = string.Equals(viewControl.Name, controlName, StringComparison.Ordinal)
                ? [viewControl]
                : viewControl.Controls.Find(controlName, searchAllChildren: true);

            if (matches.Length == 0)
            {
                continue;
            }

            ApplyPropertyValues(matches[0], properties);
        }

        _splitContainer.Panel1.Refresh();
        _propertyGrid.Refresh();
        _selectionAdorner.SynchronizeBoundsAndRender();
    }

    /// <summary>
    ///  Captures every browsable, read/write property of <paramref name="control"/> whose
    ///  <see cref="TypeConverter"/> can round-trip through a string (covers strings, numbers, bools,
    ///  enums, <see cref="Color"/>, <see cref="Size"/>, <see cref="Point"/>, <see cref="Padding"/>, ...).
    /// </summary>
    private static Dictionary<string, string> CapturePropertyValues(Control control)
    {
        Dictionary<string, string> values = [];
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(control))
        {
            if (property.IsReadOnly || !property.IsBrowsable)
            {
                continue;
            }

            TypeConverter converter = property.Converter;
            if (!converter.CanConvertTo(typeof(string)) || !converter.CanConvertFrom(typeof(string)))
            {
                continue;
            }

            object? value = property.GetValue(control);
            string? text = converter.ConvertToInvariantString(value);
            if (text is not null)
            {
                values[property.Name] = text;
            }
        }

        return values;
    }

    private static void ApplyPropertyValues(Control control, Dictionary<string, string> values)
    {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);

        foreach ((string propertyName, string text) in values)
        {
            PropertyDescriptor? property = properties[propertyName];

            if (property is null || property.IsReadOnly)
            {
                continue;
            }

            TypeConverter converter = property.Converter;
            if (!converter.CanConvertFrom(typeof(string)))
            {
                continue;
            }

            try
            {
                property.SetValue(control, converter.ConvertFromInvariantString(text));
            }
            catch (Exception ex) when (ex is FormatException or ArgumentException or NotSupportedException)
            {
                // Skip values that no longer round-trip (e.g. the saved file targeted a different
                // control type); this is a scratch/testing tool, not production settings storage.
            }
        }
    }

    private void MainForm_DpiChanged(object sender, DpiChangedEventArgs e)
    {
        ApplyToolStripImages();
        UpdateScaleStatusLabels();
        _selectionAdorner.SynchronizeBoundsAndRender();
    }

    private void SystemAppearanceTimer_Tick(object sender, EventArgs e) 
        => UpdateSystemAppearance();

    private void ApplySystemTextSize()
    {
        float scaledSize = BaseUiFontSize * (float)Application.SystemVisualSettings.TextScaleFactor;
        Font newFont = new("Segoe UI", scaledSize, FontStyle.Regular, GraphicsUnit.Point);
        Font? oldFont = _scaledUiFont;
        _scaledUiFont = newFont;

        _menuStrip.Font = newFont;
        _statusStrip.Font = newFont;

        foreach ((IScenarioView scenario, _) in _views)
        {
            ((Control)scenario).Font = newFont;
        }

        oldFont?.Dispose();
    }

    private void UpdateSystemAppearance()
    {
        Windows.UI.Color accent = _uiSettings.GetColorValue(UIColorType.Accent);
        _accentColor = Color.FromArgb(accent.A, accent.R, accent.G, accent.B);
        _accentColorStatusLabel.Text = $"Accent: #{_accentColor.ToArgb():X8}";
        _accentColorSwatchStatusLabel.BackColor = _accentColor;
        _accentColorSwatchStatusLabel.ToolTipText = $"Windows accent color #{_accentColor.ToArgb():X8}";
        _selectionAdorner.AccentColor = _accentColor;

        UpdateScaleStatusLabels();
    }

    private void UpdateScaleStatusLabels()
    {
        int displayPercent = (int)Math.Round(DeviceDpi / 96D * 100D);
        int textPercent = (int)Math.Round(Application.SystemVisualSettings.TextScaleFactor * 100D);
        _displayScaleStatusLabel.Text = $"Display: {displayPercent}% ({DeviceDpi} DPI)";
        _textScaleStatusLabel.Text = $"Text: {textPercent}%";
    }

    private void MainForm_SystemVisualSettingsChanged(object sender, SystemVisualSettingsChangedEventArgs e)
    {
        switch (e.Changed)
        {
            case SystemVisualSettingsCategories.AccentColor:
            UpdateSystemAppearance();
            break;

            case SystemVisualSettingsCategories.TextScale:
                ApplySystemTextSize();
                UpdateScaleStatusLabels();
                _selectionAdorner.SynchronizeBoundsAndRender();
                break;
        }
    }
}
