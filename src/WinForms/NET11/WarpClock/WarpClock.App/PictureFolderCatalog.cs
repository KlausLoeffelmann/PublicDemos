using Microsoft.Extensions.Logging;

namespace WarpClock.App;

/// <summary>
///  Discovers supported local image files once for consumption by render-thread snapshots.
/// </summary>
public sealed class PictureFolderCatalog(ILogger<PictureFolderCatalog> logger)
{
    private static readonly HashSet<string> s_supportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".bmp",
            ".gif",
            ".jpeg",
            ".jpg",
            ".png",
            ".webp",
        };

    private readonly object _sync = new();
    private IReadOnlyList<string> _paths = [];

    public IReadOnlyList<string> Paths
    {
        get
        {
            lock (_sync)
            {
                return _paths;
            }
        }
    }

    public IReadOnlyList<string> Refresh(string? folder)
    {
        IReadOnlyList<string> discovered;
        if (string.IsNullOrWhiteSpace(folder))
        {
            discovered = [];
        }
        else
        {
            try
            {
                discovered = Directory.Exists(folder)
                    ? Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly)
                        .Where(path => s_supportedExtensions.Contains(Path.GetExtension(path)))
                        .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
                        .ThenBy(path => path, StringComparer.OrdinalIgnoreCase)
                        .ToArray()
                    : [];
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                logger.LogWarning(ex, "Could not refresh pictures from {Folder}.", folder);
                return Paths;
            }
        }

        lock (_sync)
        {
            _paths = discovered;
            return _paths;
        }
    }
}
