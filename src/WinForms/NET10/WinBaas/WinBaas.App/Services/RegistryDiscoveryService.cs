using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using WinBaas.Models;

namespace WinBaas.Services;

/// <inheritdoc cref="IRegistryDiscovery"/>
public sealed class RegistryDiscoveryService(
    IRegistryCatalog catalog,
    ILogger<RegistryDiscoveryService> logger) : IRegistryDiscovery
{
    private readonly IRegistryCatalog _catalog = catalog;
    private readonly ILogger<RegistryDiscoveryService> _logger = logger;

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistryDiscoveredItem>> DiscoverAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => DiscoverCore(cancellationToken), cancellationToken);

    private IReadOnlyList<RegistryDiscoveredItem> DiscoverCore(CancellationToken cancellationToken)
    {
        var items = new List<RegistryDiscoveredItem>();
        foreach (RegistryDescriptor descriptor in _catalog.GetAll())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            items.Add(DiscoverOne(descriptor));
        }

        return items;
    }

    private RegistryDiscoveredItem DiscoverOne(RegistryDescriptor descriptor)
    {
        try
        {
            using RegistryKey baseKey = RegistryKey.OpenBaseKey(descriptor.Hive, RegistryView.Default);
            using RegistryKey? subKey = baseKey.OpenSubKey(descriptor.SubKeyPath, writable: false);
            if (subKey is null)
            {
                return new RegistryDiscoveredItem
                {
                    Descriptor = descriptor,
                    IsPresent = false,
                };
            }

            string[] names = subKey.GetValueNames();
            bool hasValue = string.IsNullOrEmpty(descriptor.ValueName)
                ? names.Length == 0 || names.Contains(string.Empty, StringComparer.Ordinal)
                : names.Contains(descriptor.ValueName, StringComparer.OrdinalIgnoreCase);

            if (!hasValue)
            {
                return new RegistryDiscoveredItem
                {
                    Descriptor = descriptor,
                    IsPresent = false,
                };
            }

            object? value = subKey.GetValue(descriptor.ValueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
            return new RegistryDiscoveredItem
            {
                Descriptor = descriptor,
                Value = value,
                IsPresent = true,
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogDebug(ex, "Access denied reading {Path}.", descriptor.DisplayPath);
            return new RegistryDiscoveredItem
            {
                Descriptor = descriptor,
                IsPresent = false,
                AccessDenied = true,
            };
        }
        catch (System.Security.SecurityException ex)
        {
            _logger.LogDebug(ex, "Security policy blocked reading {Path}.", descriptor.DisplayPath);
            return new RegistryDiscoveredItem
            {
                Descriptor = descriptor,
                IsPresent = false,
                AccessDenied = true,
            };
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Could not read registry value {Path}.", descriptor.DisplayPath);
            return new RegistryDiscoveredItem
            {
                Descriptor = descriptor,
                IsPresent = false,
            };
        }
    }
}
