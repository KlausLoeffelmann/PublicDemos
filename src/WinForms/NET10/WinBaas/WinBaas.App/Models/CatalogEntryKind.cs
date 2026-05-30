namespace WinBaas.Models;

/// <summary>
///  The kind of backup source a catalog entry describes.
/// </summary>
public enum CatalogEntryKind
{
    /// <summary>A directory on disk.</summary>
    Folder,

    /// <summary>A single file on disk.</summary>
    File,

    /// <summary>An environment variable value.</summary>
    EnvironmentVariable,

    /// <summary>A SQL Server instance (LocalDB or SQL Express) and its databases.</summary>
    SqlServer,

    /// <summary>A curated set of Windows registry values.</summary>
    Registry,

    /// <summary>The locally installed Visual Studio SKUs.</summary>
    VisualStudio,
}
