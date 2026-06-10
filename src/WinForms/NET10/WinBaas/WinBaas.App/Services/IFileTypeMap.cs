using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Maps file extensions to human-friendly type labels for display in the
///  WinBaas grid.
/// </summary>
public interface IFileTypeMap
{
    /// <summary>
    ///  Gets a friendly label for the file represented by <paramref name="path"/>,
    ///  qualified by the owning <paramref name="source"/> entry where helpful
    ///  (e.g. <c>"VS Code · User Settings"</c>).
    /// </summary>
    string GetLabel(string path, CatalogEntry? source = null);
}

