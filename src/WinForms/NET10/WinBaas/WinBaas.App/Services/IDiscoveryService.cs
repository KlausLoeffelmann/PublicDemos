using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Discovers backup-candidate items for a given <see cref="CatalogEntry"/>.
/// </summary>
/// <remarks>
///  <para>
///   Implementations must be best-effort and non-fatal: missing folders,
///   inaccessible paths, missing SQL tools, and SQL connection failures must
///   never propagate as exceptions. Errors are logged and the call returns
///   whatever could be discovered.
///  </para>
/// </remarks>
public interface IDiscoveryService
{
    /// <summary>
    ///  Discovers items for the specified catalog entry.
    /// </summary>
    /// <param name="entry">The catalog entry to scan.</param>
    /// <param name="cancellationToken">Token to cancel discovery.</param>
    Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(
        CatalogEntry entry,
        CancellationToken cancellationToken = default);
}
