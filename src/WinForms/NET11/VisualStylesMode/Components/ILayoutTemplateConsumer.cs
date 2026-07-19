namespace VisualStylesModeDemo.Components;

/// <summary>
///  Identifies a control that consumes a complete <see cref="LayoutTemplateItem"/>
///  instead of having the provider copy individual template values to it.
/// </summary>
/// <remarks>
///  <para>
///   Implement this interface on a <see cref="Control"/>-derived class when the control
///   needs to interpret a template itself. An explicit interface implementation keeps
///   the provider's string-valued extender property as the PropertyGrid-facing selector.
///  </para>
/// </remarks>
public interface ILayoutTemplateConsumer
{
    /// <summary>
    ///  Gets or sets the layout template currently assigned by a
    ///  <see cref="LayoutTemplateProvider"/>.
    /// </summary>
    LayoutTemplateItem? LayoutTemplate { get; set; }
}
