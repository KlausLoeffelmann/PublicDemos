using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Persists winget package definitions to disk, one JSON file per package.
/// </summary>
public interface IPackageStore
{
    /// <summary>
    ///  Loads every persisted package. Returns an empty list when no packages exist yet.
    ///  Implementations are best-effort and skip files that fail to deserialize.
    /// </summary>
    IReadOnlyList<WingetPackage> LoadAll();

    /// <summary>
    ///  Creates or overwrites the on-disk definition for <paramref name="package"/>.
    /// </summary>
    void Save(WingetPackage package);

    /// <summary>
    ///  Writes a timestamped backup of <paramref name="package"/> and deletes its on-disk definition.
    /// </summary>
    void Delete(WingetPackage package);
}
