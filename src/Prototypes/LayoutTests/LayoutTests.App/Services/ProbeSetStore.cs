using System.Threading.Tasks;
using LayoutTests.App.Models;
using Microsoft.Extensions.Logging;
using WarpToolkit.ComponentModel;

namespace LayoutTests.App.Services;

public sealed class ProbeSetStore
{
    private const string LastPathSettingsKey = "LayoutTests.LastProbeSetPath";

    private readonly IUserSettingsService _settings;
    private readonly ILogger<ProbeSetStore> _logger;

    public ProbeSetStore(IUserSettingsService settings, ILogger<ProbeSetStore> logger)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(logger);

        _settings = settings;
        _logger = logger;
    }

    public string? LastOpenedPath
    {
        get => _settings.Get<string?>(LastPathSettingsKey, null);
        set => _settings.Set(LastPathSettingsKey, value);
    }

    public async Task<ProbeSet> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _logger.LogInformation("Loading probe set from {Path}", path);

        string json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        ProbeSet set = ProbeSetSerializer.Deserialize(json);
        LastOpenedPath = path;
        return set;
    }

    public async Task SaveAsync(ProbeSet set, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(set);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _logger.LogInformation("Saving probe set '{Name}' to {Path}", set.Name, path);

        string json = ProbeSetSerializer.Serialize(set);
        await File.WriteAllTextAsync(path, json, cancellationToken).ConfigureAwait(false);
        LastOpenedPath = path;
    }
}
