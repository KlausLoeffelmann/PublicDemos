// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace VisualStylesModeDemo.Controls;

internal sealed class FontTemplateConverter : ExpandableObjectConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(string)
        || destinationType == typeof(InstanceDescriptor)
        || (destinationType is not null && base.CanConvertTo(context, destinationType));

    public override object? ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        if (value is not string text)
        {
            return base.ConvertFrom(context, culture, value);
        }

        string[] parts = text.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
        {
            throw new FormatException("A relative font must contain a size delta, added style, and removed style.");
        }

        return new FontTemplate(
            float.Parse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture),
            Enum.Parse<FontStyle>(parts[1]),
            Enum.Parse<FontStyle>(parts[2]));
    }

    public override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
    {
        if (value is FontTemplate relativeFont)
        {
            if (destinationType == typeof(string))
            {
                return FormattableString.Invariant(
                    $"{relativeFont.SizeDeltaInPoints:R}, {relativeFont.AddedStyle}, {relativeFont.RemovedStyle}");
            }

            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo constructor = typeof(FontTemplate).GetConstructor(
                    [typeof(float), typeof(FontStyle), typeof(FontStyle)])!;

                return new InstanceDescriptor(
                    constructor,
                    new object[]
                    {
                        relativeFont.SizeDeltaInPoints,
                        relativeFont.AddedStyle,
                        relativeFont.RemovedStyle,
                    });
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
