using System.ComponentModel;

using VisualStylesModeDemo.Controls;

namespace VisualStylesModeDemo.Components;

/// <summary>
///  Defines reusable layout, color, and relative-font values for WinForms controls.
/// </summary>
/// <remarks>
///  Empty layout and color values are intentional: they tell
///  <see cref="LayoutTemplateProvider"/> not to overwrite the corresponding control
///  property. The <see cref="FontTemplate"/> is always evaluated relative to the
///  provider's template source container.
/// </remarks>
public class LayoutTemplateItem : INotifyPropertyChanged
{
    private static int _nextId = 1;

    private string _name;
    private Padding _margin;
    private Padding _padding;
    private Color _foreColor;
    private Color _backColor;
    private FontTemplate _fontTemplate;

    /// <summary>
    ///  Initializes a template with a generated design-time name and empty overlay values.
    /// </summary>
    public LayoutTemplateItem()
    {
        _name = $"LayoutTemplate{_nextId++}";
        _fontTemplate = new FontTemplate();
        _fontTemplate.Changed += FontTemplate_Changed;
    }

    /// <summary>
    ///  Occurs when a template property or a nested <see cref="FontTemplate"/> value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///  Gets or sets the non-empty name used by controls and the PropertyGrid dropdown.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            SetField(ref _name, value);
        }
    }

    /// <summary>
    ///  Gets or sets the margin to overlay on a control.
    /// </summary>
    /// <remarks>
    ///  <see cref="Padding.Empty"/> leaves the control's existing margin unchanged.
    /// </remarks>
    [NotifyParentProperty(true)]
    public Padding Margin
    {
        get => _margin;
        set => SetField(ref _margin, value);
    }

    /// <summary>
    ///  Gets or sets the padding to overlay on a control.
    /// </summary>
    /// <remarks>
    ///  <see cref="Padding.Empty"/> leaves the control's existing padding unchanged.
    /// </remarks>
    [NotifyParentProperty(true)]
    public Padding Padding
    {
        get => _padding;
        set => SetField(ref _padding, value);
    }

    /// <summary>
    ///  Gets or sets the foreground color to overlay on a control.
    /// </summary>
    /// <remarks>
    ///  <see cref="Color.Empty"/> leaves the control's existing foreground color unchanged.
    /// </remarks>
    [NotifyParentProperty(true)]
    public Color ForeColor
    {
        get => _foreColor;
        set => SetField(ref _foreColor, value);
    }

    /// <summary>
    ///  Gets or sets the background color to overlay on a control.
    /// </summary>
    /// <remarks>
    ///  <see cref="Color.Empty"/> leaves the control's existing background color unchanged.
    /// </remarks>
    [NotifyParentProperty(true)]
    public Color BackColor
    {
        get => _backColor;
        set => SetField(ref _backColor, value);
    }

    /// <summary>
    ///  Gets or sets the relative font changes to apply to the provider host's font.
    /// </summary>
    [NotifyParentProperty(true)]
    public FontTemplate FontTemplate
    {
        get => _fontTemplate;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (ReferenceEquals(_fontTemplate, value))
            {
                return;
            }

            _fontTemplate.Changed -= FontTemplate_Changed;
            _fontTemplate = value;
            _fontTemplate.Changed += FontTemplate_Changed;
            OnPropertyChanged(nameof(FontTemplate));
        }
    }

    /// <inheritdoc/>
    public override string ToString() => Name;

    private void FontTemplate_Changed(object? sender, EventArgs e) =>
        OnPropertyChanged(nameof(FontTemplate));

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }
}
