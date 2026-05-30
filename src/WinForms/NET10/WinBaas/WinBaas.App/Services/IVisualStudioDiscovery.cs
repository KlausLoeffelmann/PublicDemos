using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Discovers locally installed Visual Studio SKUs, hives, and extensions.
/// </summary>
public interface IVisualStudioDiscovery
{
    /// <summary>Discovers installed Visual Studio SKUs.</summary>
    Task<IReadOnlyList<VsSku>> DiscoverAsync(CancellationToken cancellationToken = default);
}
