using Microsoft.Win32;

namespace WinBaas.Models;

/// <summary>
///  Describes one curated Windows registry value that WinBaas can inspect and restore.
/// </summary>
public sealed class RegistryDescriptor
{
    /// <summary>The display label shown in the Registry grid.</summary>
    public required string Name { get; init; }

    /// <summary>The root hive that contains the value.</summary>
    public required RegistryHive Hive { get; init; }

    /// <summary>The hive-relative sub-key path.</summary>
    public required string SubKeyPath { get; init; }

    /// <summary>The registry value name. Empty means the default value.</summary>
    public required string ValueName { get; init; }

    /// <summary>The expected registry value kind.</summary>
    public required RegistryValueKind ValueKind { get; init; }

    /// <summary>A short description suitable for the grid.</summary>
    public required string ShortDescription { get; init; }

    /// <summary>A fuller description shown in the status strip.</summary>
    public required string FullDescription { get; init; }

    /// <summary>An optional default value used only for reference text.</summary>
    public object? DefaultValue { get; init; }

    /// <summary>True when the value is policy-only or otherwise informational.</summary>
    public bool InformationalOnly { get; init; }

    /// <summary>True when reading or restoring the value may require elevation.</summary>
    public bool RequiresElevation { get; init; }

    /// <summary>The display path shown in the UI and emitted into backup files.</summary>
    public string DisplayPath
        => string.IsNullOrEmpty(ValueName)
            ? $"{GetHiveDisplayName(Hive)}\\{SubKeyPath}\\(Default)"
            : $"{GetHiveDisplayName(Hive)}\\{SubKeyPath}\\{ValueName}";

    /// <summary>The registry-provider path used by PowerShell.</summary>
    public string ProviderPath => $"Registry::{GetHiveProviderName(Hive)}\\{SubKeyPath}";

    /// <summary>The .reg file key path.</summary>
    public string RegFileKeyPath => $"{GetHiveDisplayName(Hive)}\\{SubKeyPath}";

    /// <summary>Gets the .reg value token, e.g. <c>"ValueName"</c> or <c>@</c>.</summary>
    public string RegFileValueToken => string.IsNullOrEmpty(ValueName) ? "@" : $"\"{EscapeForReg(ValueName)}\"";

    private static string GetHiveDisplayName(RegistryHive hive) => hive switch
    {
        RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
        RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
        RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
        RegistryHive.Users => "HKEY_USERS",
        RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
        _ => hive.ToString(),
    };

    private static string GetHiveProviderName(RegistryHive hive) => hive switch
    {
        RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
        RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
        RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
        RegistryHive.Users => "HKEY_USERS",
        RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
        _ => hive.ToString(),
    };

    private static string EscapeForReg(string value)
        => value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
}
