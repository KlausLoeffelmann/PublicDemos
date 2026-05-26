namespace WinBaas.Services;

/// <summary>
///  Maps file extensions to human-friendly type labels for display in the
///  WinBaas grid.
/// </summary>
public interface IFileTypeMap
{
    /// <summary>
    ///  Gets a friendly label for the file represented by <paramref name="path"/>,
    ///  or its extension if no specific label exists.
    /// </summary>
    string GetLabel(string path);
}
