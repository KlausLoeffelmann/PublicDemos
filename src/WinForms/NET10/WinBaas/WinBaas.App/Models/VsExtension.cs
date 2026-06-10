namespace WinBaas.Models;

/// <summary>
///  One installed Visual Studio extension.
/// </summary>
public sealed class VsExtension
{
    /// <summary>The display name.</summary>
    public required string Name { get; init; }

    /// <summary>The publisher, if known.</summary>
    public string Publisher { get; init; } = string.Empty;

    /// <summary>The version string, if known.</summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>The absolute installation path.</summary>
    public required string InstallPath { get; init; }

    /// <summary>The folder name used for backup layout.</summary>
    public string FolderName => Path.GetFileName(InstallPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
}
