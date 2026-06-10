namespace WinBaas.Models;

/// <summary>
///  One installed Visual Studio SKU together with its user hive and extension metadata.
/// </summary>
public sealed class VsSku
{
    /// <summary>The VS display name shown in the overview grid.</summary>
    public required string DisplayName { get; init; }

    /// <summary>The release year, e.g. <c>2022</c> or <c>2026</c>.</summary>
    public required string Year { get; init; }

    /// <summary>The edition, e.g. Community / Professional / Enterprise.</summary>
    public required string Edition { get; init; }

    /// <summary>The release ring, e.g. Release / Preview / Int.Preview / Main.</summary>
    public required string Ring { get; init; }

    /// <summary>The installation version string.</summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>The installation date, if known.</summary>
    public DateTime? InstallDate { get; init; }

    /// <summary>The installation root path, if known.</summary>
    public string InstallationPath { get; init; } = string.Empty;

    /// <summary>The main settings-file path for the SKU, or the hive root when unknown.</summary>
    public string SettingsPath { get; init; } = string.Empty;

    /// <summary>The base hive root for the SKU, or an empty string when unavailable.</summary>
    public string HiveRootPath { get; init; } = string.Empty;

    /// <summary>The discovered hive folders for this SKU.</summary>
    public IReadOnlyList<VsHive> Hives { get; init; } = [];

    /// <summary>The discovered extensions for this SKU.</summary>
    public IReadOnlyList<VsExtension> Extensions { get; init; } = [];

    /// <summary>True when the user selected this SKU for backup.</summary>
    public bool IsChecked { get; set; }

    /// <summary>The tree-node label.</summary>
    public string NodeLabel => $"VS {Year} — {Edition} — {Ring}";

    /// <summary>The stable persistence key / backup folder label.</summary>
    public string Key
        => $"{Year}|{Edition}|{Ring}|{HiveRootPath}".ToLowerInvariant();
}
