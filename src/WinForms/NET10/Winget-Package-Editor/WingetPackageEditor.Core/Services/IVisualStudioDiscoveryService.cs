using WingetPackageEditor.Core.Models;

namespace WingetPackageEditor.Core.Services;

/// <summary>
///  Discovers Visual Studio installations and their associated data hives.
/// </summary>
public interface IVisualStudioDiscoveryService
{
    /// <summary>
    ///  Discovers all installed Visual Studio instances (including prerelease), correlating
    ///  each with its local data and experimental hives. Implementations route any external
    ///  tool output to the console and never throw for the absence of Visual Studio.
    /// </summary>
    IReadOnlyList<VisualStudioInstanceInfo> DiscoverInstances();
}
