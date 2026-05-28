using System.Text.Json;
using BranchComposer.App.Models;

namespace BranchComposer.App.Services;

public sealed class AppStateStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _stateFilePath;

    public AppStateStore()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _stateFilePath = Path.Combine(appDataPath, "BranchComposer", "appstate.json");
    }

    public async Task<AppState> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_stateFilePath))
        {
            return new AppState();
        }

        await using FileStream stream = File.OpenRead(_stateFilePath);
        return await JsonSerializer.DeserializeAsync<AppState>(stream, JsonOptions, cancellationToken).ConfigureAwait(false)
            ?? new AppState();
    }

    public async Task SaveAsync(AppState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        Directory.CreateDirectory(Path.GetDirectoryName(_stateFilePath)!);

        await using FileStream stream = File.Create(_stateFilePath);
        await JsonSerializer.SerializeAsync(stream, state, JsonOptions, cancellationToken).ConfigureAwait(false);
    }
}

