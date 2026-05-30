using WinBaas.Models;

namespace WinBaas.Services;

/// <summary>
///  Provides the curated set of Windows registry values shown by WinBaas.
/// </summary>
public interface IRegistryCatalog
{
    /// <summary>Gets the curated registry descriptors.</summary>
    IReadOnlyList<RegistryDescriptor> GetAll();
}
