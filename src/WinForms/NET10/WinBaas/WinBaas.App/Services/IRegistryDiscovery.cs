using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Discovers the observed values for the curated registry catalog.
/// </summary>
public interface IRegistryDiscovery
{
    /// <summary>Discovers the current state of the curated registry values.</summary>
    Task<IReadOnlyList<RegistryDiscoveredItem>> DiscoverAsync(CancellationToken cancellationToken = default);
}
