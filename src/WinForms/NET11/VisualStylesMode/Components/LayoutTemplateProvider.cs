using System.ComponentModel;
using System.ComponentModel.Design;

namespace VisualStylesModeDemo.Components;

/// <summary>
///  Provides named layout templates as an extender property for WinForms controls.
/// </summary>
/// <remarks>
///  <para>
///   Controls select a template by name through the extended <c>LayoutTemplate</c>
///   property. The provider retains the resolved template item so a later rename updates
///   existing assignments without changing the selected template identity.
///  </para>
///  <para>
///   Controls that implement <see cref="ILayoutTemplateConsumer"/> receive the complete
///   template. Other controls receive only the non-empty values defined by the template.
///  </para>
/// </remarks>
[ProvideProperty("LayoutTemplate", typeof(Control))]
public partial class LayoutTemplateProvider : Component, IExtenderProvider
{
    private readonly BindingList<LayoutTemplateItem> _layoutTemplates = [];
    private readonly Dictionary<Control, TemplateAssignment> _assignments = [];
    private readonly Dictionary<LayoutTemplateItem, string> _knownTemplateNames = [];

    private ContainerControl? _templateSourceContainer;
    private bool _suppressTemplateListChanges;

    /// <summary>
    ///  Initializes an unsited layout template provider.
    /// </summary>
    public LayoutTemplateProvider()
    {
        InitializeComponent();
        InitializeProvider();
    }

    /// <summary>
    ///  Initializes a layout template provider and adds it to
    ///  <paramref name="container"/>.
    /// </summary>
    /// <param name="container">The component container that owns the provider.</param>
    public LayoutTemplateProvider(IContainer container)
        : this()
    {
        ArgumentNullException.ThrowIfNull(container);
        container.Add(this);
    }

    /// <inheritdoc/>
    public override ISite? Site
    {
        get => base.Site;
        set
        {
            base.Site = value;

            if (TemplateSourceContainer is null
                && value?.GetService(typeof(IDesignerHost)) is IDesignerHost designerHost
                && designerHost.RootComponent is ContainerControl containerControl)
            {
                TemplateSourceContainer = containerControl;
            }
        }
    }

    /// <summary>
    ///  Gets or sets the container whose font is the baseline for all
    ///  <see cref="Controls.FontTemplate"/> calculations.
    /// </summary>
    /// <remarks>
    ///  When the component is dropped on a Form or UserControl designer, this property
    ///  is initialized from the designer root and serialized into
    ///  <c>InitializeComponent</c> for runtime use.
    /// </remarks>
    [Category("Layout")]
    [DefaultValue(null)]
    public ContainerControl? TemplateSourceContainer
    {
        get => _templateSourceContainer;
        set
        {
            if (ReferenceEquals(_templateSourceContainer, value))
            {
                return;
            }

            if (_templateSourceContainer is not null)
            {
                _templateSourceContainer.FontChanged -= TemplateSourceContainer_FontChanged;
            }

            _templateSourceContainer = value;

            if (_templateSourceContainer is not null)
            {
                _templateSourceContainer.FontChanged += TemplateSourceContainer_FontChanged;
            }

            RefreshAllAssignments();
        }
    }

    /// <summary>
    ///  Gets the templates offered by the extended PropertyGrid dropdown.
    /// </summary>
    /// <remarks>
    ///  Template names must be non-empty and unique, ignoring case. The collection is
    ///  exposed as content so the WinForms designer serializes individual
    ///  <c>Add</c> calls instead of replacing the collection instance.
    /// </remarks>
    [Category("Layout")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BindingList<LayoutTemplateItem> LayoutTemplates => _layoutTemplates;

    /// <inheritdoc/>
    public bool CanExtend(object extendee) => extendee is Control;

    /// <summary>
    ///  Gets the name of the layout template assigned to <paramref name="control"/>.
    /// </summary>
    /// <param name="control">The control whose extended value is requested.</param>
    /// <returns>
    ///  The selected template name, or <see cref="string.Empty"/> when no template is
    ///  assigned.
    /// </returns>
    [Category("Layout")]
    [DefaultValue("")]
    [TypeConverter("VisualStylesModeDemo.Components.LayoutTemplateNameConverter, VisualStylesModeDemo")]
    public string GetLayoutTemplate(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        return _assignments.TryGetValue(control, out TemplateAssignment? assignment)
            ? assignment.Item?.Name ?? assignment.Name
            : string.Empty;
    }

    /// <summary>
    ///  Assigns the named layout template to <paramref name="control"/>.
    /// </summary>
    /// <param name="control">The control to extend.</param>
    /// <param name="templateName">
    ///  The template name, or <see langword="null"/> or an empty string to clear the
    ///  assignment.
    /// </param>
    public void SetLayoutTemplate(Control control, string? templateName)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (string.IsNullOrWhiteSpace(templateName))
        {
            ClearAssignment(control);
            return;
        }

        if (!_assignments.TryGetValue(control, out TemplateAssignment? assignment))
        {
            assignment = new TemplateAssignment(templateName);
            _assignments.Add(control, assignment);
            control.Disposed += ExtendedControl_Disposed;
        }
        else
        {
            assignment.Name = templateName;
            assignment.Item = null;
        }

        ResolveAndApply(control, assignment);
        TypeDescriptor.Refresh(control);
    }

    private static LayoutTemplateItem? FindTemplate(
        IEnumerable<LayoutTemplateItem> templates,
        string name) =>
        templates.FirstOrDefault(
            template => string.Equals(template.Name, name, StringComparison.OrdinalIgnoreCase));

    private void ApplyTemplate(Control control, LayoutTemplateItem template)
    {
        if (control is ILayoutTemplateConsumer consumer)
        {
            consumer.LayoutTemplate = template;
            return;
        }

        if (template.Margin != Padding.Empty)
        {
            control.Margin = template.Margin;
        }

        if (template.Padding != Padding.Empty)
        {
            control.Padding = template.Padding;
        }

        if (template.ForeColor != Color.Empty)
        {
            control.ForeColor = template.ForeColor;
        }

        if (template.BackColor != Color.Empty)
        {
            control.BackColor = template.BackColor;
        }

        if (TemplateSourceContainer is not null)
        {
            control.Font = template.FontTemplate.GetFont(TemplateSourceContainer.Font);
        }
    }

    private void ClearAssignment(Control control)
    {
        if (!_assignments.Remove(control))
        {
            return;
        }

        control.Disposed -= ExtendedControl_Disposed;

        if (control is ILayoutTemplateConsumer consumer)
        {
            consumer.LayoutTemplate = null;
        }

        TypeDescriptor.Refresh(control);
    }

    private void ExtendedControl_Disposed(object? sender, EventArgs e)
    {
        if (sender is Control control)
        {
            control.Disposed -= ExtendedControl_Disposed;
            _assignments.Remove(control);
        }
    }

    private void InitializeProvider()
    {
        _layoutTemplates.ListChanged += LayoutTemplates_ListChanged;
        Disposed += LayoutTemplateProvider_Disposed;
    }

    private void LayoutTemplateProvider_Disposed(object? sender, EventArgs e)
    {
        Disposed -= LayoutTemplateProvider_Disposed;
        _layoutTemplates.ListChanged -= LayoutTemplates_ListChanged;

        if (_templateSourceContainer is not null)
        {
            _templateSourceContainer.FontChanged -= TemplateSourceContainer_FontChanged;
        }

        foreach (Control control in _assignments.Keys)
        {
            control.Disposed -= ExtendedControl_Disposed;
        }

        _assignments.Clear();
        _knownTemplateNames.Clear();
    }

    private void LayoutTemplates_ListChanged(object? sender, ListChangedEventArgs e)
    {
        if (_suppressTemplateListChanges)
        {
            return;
        }

        ValidateTemplateNames(e);
        RefreshAllAssignments();
    }

    private void RefreshAllAssignments()
    {
        foreach ((Control control, TemplateAssignment assignment) in _assignments.ToArray())
        {
            if (control.IsDisposed)
            {
                ExtendedControl_Disposed(control, EventArgs.Empty);
                continue;
            }

            ResolveAndApply(control, assignment);
            TypeDescriptor.Refresh(control);
        }
    }

    private void ResolveAndApply(Control control, TemplateAssignment assignment)
    {
        if (assignment.Item is not null && _layoutTemplates.Contains(assignment.Item))
        {
            assignment.Name = assignment.Item.Name;
            ApplyTemplate(control, assignment.Item);
            return;
        }

        assignment.Item = FindTemplate(_layoutTemplates, assignment.Name);

        if (assignment.Item is not null)
        {
            assignment.Name = assignment.Item.Name;
            ApplyTemplate(control, assignment.Item);
        }
        else if (control is ILayoutTemplateConsumer consumer)
        {
            consumer.LayoutTemplate = null;
        }
    }

    private void TemplateSourceContainer_FontChanged(object? sender, EventArgs e) =>
        RefreshAllAssignments();

    private void ValidateTemplateNames(ListChangedEventArgs e)
    {
        Dictionary<string, LayoutTemplateItem> templatesByName =
            new(StringComparer.OrdinalIgnoreCase);

        foreach (LayoutTemplateItem template in _layoutTemplates)
        {
            if (templatesByName.TryAdd(template.Name, template))
            {
                continue;
            }

            LayoutTemplateItem changedTemplate =
                e.NewIndex >= 0 && e.NewIndex < _layoutTemplates.Count
                    ? _layoutTemplates[e.NewIndex]
                    : template;

            RollBackInvalidTemplateChange(e, changedTemplate);

            throw new InvalidOperationException(
                $"The layout template name '{template.Name}' is already in use.");
        }

        _knownTemplateNames.Clear();

        foreach (LayoutTemplateItem template in _layoutTemplates)
        {
            _knownTemplateNames.Add(template, template.Name);
        }
    }

    private void RollBackInvalidTemplateChange(
        ListChangedEventArgs e,
        LayoutTemplateItem changedTemplate)
    {
        _suppressTemplateListChanges = true;

        try
        {
            if (_knownTemplateNames.TryGetValue(changedTemplate, out string? previousName))
            {
                changedTemplate.Name = previousName;
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded
                && e.NewIndex >= 0
                && e.NewIndex < _layoutTemplates.Count)
            {
                _layoutTemplates.RemoveAt(e.NewIndex);
            }
        }
        finally
        {
            _suppressTemplateListChanges = false;
        }
    }

    private sealed class TemplateAssignment(string name)
    {
        public string Name { get; set; } = name;

        public LayoutTemplateItem? Item { get; set; }
    }
}
