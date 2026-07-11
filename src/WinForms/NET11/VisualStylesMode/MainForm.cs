// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Text.Json;
using VisualStylesModeDemo.Views;

namespace VisualStylesModeDemo;

/// <summary>
///  Host shell for a growing set of exploratory .NET 11 WinForms scenarios (tracked in
///  https://github.com/dotnet/winforms/issues/14694). Panel1 of the SplitContainer swaps between
///  registered <see cref="IScenarioView"/> UserControls via the View menu; Panel2 always hosts the
///  same PropertyGrid, which is driven by whichever controls the user has double-clicked (selected)
///  in the active view.
/// </summary>
/// <remarks>
///  <para>
///   To add another exploratory view (e.g. TreeView.NodeLeading, Application.SystemTextSize
///   live-update), create a new UserControl implementing
///   <see cref="IScenarioView"/> under Views\ and add one line to <see cref="CreateViews"/> below -
///   everything else (menu item, switching, selection wiring, disposal) is handled generically.
///  </para>
/// </remarks>
public partial class MainForm : Form
{
    private readonly List<(IScenarioView Scenario, ToolStripMenuItem MenuItem)> _views = [];
    private IScenarioView? _activeView;

    // The selectable-margin (10..30px in 5px steps) shown in View > Selection Margin. Defaults to the
    // panel's own default gap and is persisted alongside the window position in window.json.
    private static readonly int[] s_selectionMarginSteps = [10, 15, 20, 25, 30];
    private readonly List<ToolStripMenuItem> _marginMenuItems = [];
    private int _selectionMargin = SelectablePanel.DefaultSelectionGap;

    public MainForm()
    {
        InitializeComponent();

        // We will need the components container also for other means,
        // so, make sure, it got actually initialzed:
        components ??= new Container();

        CreateViews();
        BuildSelectionMarginMenu();

        if (_views.Count > 0)
        {
            SwitchToView(_views[0].Scenario);
        }

        UpdateFormSizeStatusLabels();
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

        _viewToolStripMenuItem.DropDownItems.Add(menuItem);
        _views.Add((scenario, menuItem));

        // Let the shared IContainer own disposal of every view, whether active or not, so GDI
        // resources (e.g. the generated background-image Bitmaps) are cleaned up with the form.
        components.Add(view);
    }

    private void SwitchToView(IScenarioView scenario)
    {
        if (ReferenceEquals(_activeView, scenario))
        {
            return;
        }

        if (_activeView is not null)
        {
            _activeView.SelectionChanged -= ActiveView_SelectionChanged;
            ((Control)_activeView).Visible = false;
            _splitContainer.Panel1.Controls.Remove((Control)_activeView);
        }

        _activeView = scenario;
        _activeView.SelectionChanged += ActiveView_SelectionChanged;

        Control viewControl = (Control)scenario;
        viewControl.Dock = DockStyle.Top;
        _splitContainer.Panel1.Controls.Add(viewControl);
        viewControl.Visible = true;

        foreach ((IScenarioView candidate, ToolStripMenuItem menuItem) in _views)
        {
            menuItem.Checked = ReferenceEquals(candidate, scenario);
        }

        RefreshPropertyGridSelection();
    }

    private void ActiveView_SelectionChanged(object? sender, EventArgs e) => RefreshPropertyGridSelection();

    private void RefreshPropertyGridSelection()
    {
        Control[] selected = _activeView?.GetSelectedControls().ToArray() ?? [];
        _propertyGrid.SelectedObjects = selected;

        _selectedControlStatusLabel.Text = selected.Length switch
        {
            0 => $"{_activeView?.DisplayName}: no controls selected",
            1 => $"{_activeView?.DisplayName}: {((Control)selected[0]).Name} selected",
            _ => $"{_activeView?.DisplayName}: {selected.Length} controls selected",
        };
    }

    private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e) => _activeView?.SelectAll();

    private void ClearSelectionToolStripMenuItem_Click(object sender, EventArgs e) => _activeView?.ClearSelection();

    /// <summary>
    ///  Builds the "Selection Margin" submenu under View. The scenario view items were just added by
    ///  <see cref="CreateViews"/>, so we append a separator and then the submenu with one checkable
    ///  item per 5px step (10..30). The items behave like a radio group via <see cref="ApplySelectionMargin"/>.
    /// </summary>
    private void BuildSelectionMarginMenu()
    {
        _viewToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
        _viewToolStripMenuItem.DropDownItems.Add(_selectionMarginToolStripMenuItem);

        foreach (int step in s_selectionMarginSteps)
        {
            ToolStripMenuItem item = new()
            {
                Text = $"{step} px",
                Tag = step,
                Checked = step == _selectionMargin,
            };
            item.Click += SelectionMarginMenuItem_Click;

            _selectionMarginToolStripMenuItem.DropDownItems.Add(item);
            _marginMenuItems.Add(item);
        }
    }

    private void SelectionMarginMenuItem_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: int step })
        {
            ApplySelectionMargin(step);
        }
    }

    /// <summary>
    ///  Applies the selection margin (gap between a control's chrome and its selection frame) to every
    ///  registered view - all views are constructed up front, so switching views keeps a consistent
    ///  margin - and updates the submenu's radio-style check marks.
    /// </summary>
    private void ApplySelectionMargin(int gap)
    {
        _selectionMargin = gap;

        foreach ((IScenarioView scenario, _) in _views)
        {
            scenario.SetSelectionMargin(gap);
        }

        foreach (ToolStripMenuItem item in _marginMenuItems)
        {
            item.Checked = item.Tag is int step && step == gap;
        }
    }

    /// <summary>Snaps an arbitrary (e.g. hand-edited) value to the closest supported 5px step.</summary>
    private static int NormalizeMargin(int value)
    {
        int closest = s_selectionMarginSteps[0];
        foreach (int step in s_selectionMarginSteps)
        {
            if (Math.Abs(step - value) < Math.Abs(closest - value))
            {
                closest = step;
            }
        }

        return closest;
    }

    private void PropertyGrid_PropertyValueChanged(object? sender, PropertyValueChangedEventArgs e) =>
        _splitContainer.Panel1.Refresh();

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateFormSizeStatusLabels();
    }

    /// <summary>
    ///  Restores the persisted window position/size/state and selection margin. This runs in
    ///  <see cref="OnLoad"/> - after <c>base.OnLoad</c> - deliberately: by then the Font-based
    ///  AutoScale (DPI) layout pass has already sized the form, so our restored bounds are the final
    ///  word and won't be undone by a later scaling pass.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        WindowSettings? settings = TryLoadSettings();
        if (settings is null)
        {
            return;
        }

        // Restore the selection margin, snapped to a supported step in case window.json was edited.
        ApplySelectionMargin(NormalizeMargin(settings.SelectionMargin));

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

    /// <summary>Persists the window position/size/state and selection margin as the form closes.</summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        SaveSettings();
        base.OnFormClosing(e);
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
            SelectionMargin = _selectionMargin,
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

    /// <summary>Small JSON-serialized snapshot of the window's restore state and selection margin.</summary>
    private sealed record WindowSettings
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public bool Maximized { get; init; }
        public int SelectionMargin { get; init; }
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
        Control[] selected = _activeView?.GetSelectedControls().ToArray() ?? [];
        if (selected.Length == 0)
        {
            MessageBox.Show(this, "Double-click at least one control to select it before saving its property settings.", Text);
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
            Control[] matches = viewControl.Controls.Find(controlName, searchAllChildren: true);
            if (matches.Length == 0)
            {
                continue;
            }

            ApplyPropertyValues(matches[0], properties);
        }

        _splitContainer.Panel1.Refresh();
        _propertyGrid.Refresh();
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
}
