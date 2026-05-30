using System.Globalization;

namespace WinBaas.Models;

/// <summary>
///  The observed state of one curated registry value on the current machine.
/// </summary>
public sealed class RegistryDiscoveredItem
{
    /// <summary>The curated descriptor from which the discovery item was produced.</summary>
    public required RegistryDescriptor Descriptor { get; init; }

    /// <summary>The observed value, if present and readable.</summary>
    public object? Value { get; init; }

    /// <summary>True when the value exists in the registry.</summary>
    public bool IsPresent { get; init; }

    /// <summary>True when reading the value was denied by access policy.</summary>
    public bool AccessDenied { get; init; }

    /// <summary>True when the user selected this item for backup.</summary>
    public bool IsChecked { get; set; }

    /// <summary>Gets whether the user can select the item in the grid.</summary>
    public bool CanSelect => IsPresent && !AccessDenied;

    /// <summary>The display name used in the grid.</summary>
    public string Name => Descriptor.Name;

    /// <summary>The registry path shown in the grid.</summary>
    public string RegistryPath => Descriptor.DisplayPath;

    /// <summary>The short description shown in the grid.</summary>
    public string ShortDescription => Descriptor.ShortDescription;

    /// <summary>The full description shown in the status strip.</summary>
    public string FullDescription => Descriptor.FullDescription;

    /// <summary>The formatted value shown in the grid and .reg export.</summary>
    public string ValueText => FormatValue(Value);

    private static string FormatValue(object? value) => value switch
    {
        null => "(not present)",
        string text => text,
        string[] values => string.Join("; ", values),
        byte[] bytes => Convert.ToHexString(bytes),
        DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
    };
}
