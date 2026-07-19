using System.ComponentModel;

namespace VisualStylesModeDemo.Components;

/// <summary>
///  Supplies the template names belonging to a specific
///  <see cref="LayoutTemplateProvider"/> as PropertyGrid standard values.
/// </summary>
public sealed class LayoutTemplateNameConverter : StringConverter
{
    /// <inheritdoc/>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

    /// <inheritdoc/>
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

    /// <inheritdoc/>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        LayoutTemplateProvider? provider =
            context?.PropertyDescriptor?.Attributes[typeof(ExtenderProvidedPropertyAttribute)]
                is ExtenderProvidedPropertyAttribute extenderAttribute
                    ? extenderAttribute.Provider as LayoutTemplateProvider
                    : null;

        string[] templateNames = provider is null
            ? []
            : provider.LayoutTemplates.Select(template => template.Name).ToArray();

        return new StandardValuesCollection(templateNames);
    }
}
