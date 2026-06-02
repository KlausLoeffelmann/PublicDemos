using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Manages the catalog of backup-source definitions.
/// </summary>
/// <remarks>
///  <para>
///   The catalog combines a built-in seed of common "easy to forget" backup
///   spots with user-defined entries. It is persisted through
///   <see cref="WarpToolkit.ComponentModel.IUserSettingsService"/>.
///  </para>
/// </remarks>
public interface ICatalogService
{
    /// <summary>Gets the current catalog entries (built-in + user-defined).</summary>
    IReadOnlyList<CatalogEntry> GetAll();

    /// <summary>Adds a user-defined entry.</summary>
    void Add(CatalogEntry entry);

    /// <summary>Removes a user-defined entry by id. Built-in entries cannot be removed.</summary>
    bool Remove(Guid id);

    /// <summary>Replaces the persisted catalog with the built-in seed.</summary>
    void RestoreDefaults();

    /// <summary>
    ///  Reloads the user-defined entries from the current backing store. Call
    ///  after the roaming catalog path option changes so the in-memory catalog
    ///  reflects the newly configured location instead of overwriting it.
    /// </summary>
    void Reload();

    /// <summary>Persists any pending changes to storage.</summary>
    void Save();
}
