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
///  same PropertyGrid, which is driven by whichever controls are checked (via CheckBoxes) in the
///  active view.
/// </summary>
/// <remarks>
///  <para>
///   To add another exploratory view (e.g. CheckBox scenarios, TreeView.NodeLeading,
///   Application.SystemTextSize live-update), create a new UserControl implementing
///   <see cref="IScenarioView"/> under Views\ and add one line to <see cref="CreateViews"/> below -
///   everything else (menu item, switching, selection wiring, disposal) is handled generically.
///  </para>
/// </remarks>
public partial class MainForm : Form
{
    private readonly List<(IScenarioView Scenario, ToolStripMenuItem MenuItem)> _views = [];
    private IScenarioView? _activeView;

    public MainForm()
    {
        InitializeComponent();

        // We will need the components container also for other means,
        // so, make sure, it got actually initialzed:
        components ??= new Container();

        CreateViews();

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

        // Future views, following the exact same pattern:
        //   RegisterView(new CheckBoxScenariosView());
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

    private void ResetSelectionToolStripMenuItem_Click(object sender, EventArgs e) => _activeView?.ClearSelection();

    private void PropertyGrid_PropertyValueChanged(object? sender, PropertyValueChangedEventArgs e) =>
        _splitContainer.Panel1.Refresh();

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateFormSizeStatusLabels();
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
            MessageBox.Show(this, "Check at least one control before saving its property settings.", Text);
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
