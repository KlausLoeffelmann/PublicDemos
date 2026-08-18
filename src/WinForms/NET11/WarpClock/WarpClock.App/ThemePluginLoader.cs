using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;
using WarpClock.Abstractions;

namespace WarpClock.App;

/// <summary>
///  Discovers <see cref="IClockTheme"/> implementations in drop-in assemblies under a
///  plug-ins directory. Each assembly is loaded into its own collectible
///  <see cref="AssemblyLoadContext"/> that defers shared contract / runtime assemblies
///  (for example <c>WarpClock.Abstractions</c>) to the default context, so theme types
///  share identity with the host.
/// </summary>
public sealed class ThemePluginLoader
{
    private readonly string _pluginDirectory;
    private readonly ILogger<ThemePluginLoader> _logger;
    private readonly Dictionary<string, DateTime> _loadedWriteTimesUtc = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<PluginLoadContext> _contexts = [];

    public ThemePluginLoader(AppPaths paths, ILogger<ThemePluginLoader> logger)
        : this(paths.PluginDirectory, logger)
    {
    }

    public ThemePluginLoader(string pluginDirectory, ILogger<ThemePluginLoader> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginDirectory);
        ArgumentNullException.ThrowIfNull(logger);

        _pluginDirectory = pluginDirectory;
        _logger = logger;
    }

    /// <summary>The directory scanned for plug-in assemblies.</summary>
    public string PluginDirectory => _pluginDirectory;

    /// <summary>
    ///  Loads any plug-in assemblies not yet loaded or whose file timestamp changed and
    ///  returns the themes they contribute.
    /// </summary>
    public IReadOnlyList<DiscoveredTheme> LoadNew()
    {
        var discovered = new List<DiscoveredTheme>();

        if (!Directory.Exists(_pluginDirectory))
        {
            return discovered;
        }

        foreach (string path in Directory.EnumerateFiles(_pluginDirectory, "*.dll"))
        {
            DateTime lastWriteUtc;
            try
            {
                lastWriteUtc = File.GetLastWriteTimeUtc(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _logger.LogWarning(ex, "Skipping unreadable plug-in path metadata for {Path}.", path);
                continue;
            }

            if (_loadedWriteTimesUtc.TryGetValue(path, out DateTime loadedWriteUtc)
                && loadedWriteUtc == lastWriteUtc)
            {
                continue;
            }

            try
            {
                discovered.AddRange(LoadAssembly(path, lastWriteUtc));
                _loadedWriteTimesUtc[path] = lastWriteUtc;
            }
            catch (Exception ex) when (IsRecoverableAssemblyLoadFailure(ex))
            {
                _logger.LogWarning(ex, "Skipping unreadable plug-in assembly {Path}. {Details}", path, DescribeLoadFailure(ex));
            }
        }

        return discovered;
    }

    private IEnumerable<DiscoveredTheme> LoadAssembly(string path, DateTime lastWriteUtc)
    {
        PluginLoadContext context = new();
        string shadowPath = CreateShadowCopy(path, lastWriteUtc);
        Assembly assembly = context.LoadFromAssemblyPath(shadowPath);
        _contexts.Add(context);

        var results = new List<DiscoveredTheme>();
        foreach (Type type in GetLoadableThemeTypes(assembly, path))
        {
            try
            {
                ClockThemeExportAttribute? export = type.GetCustomAttribute<ClockThemeExportAttribute>();
                if (export is { Discoverable: false })
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is IClockTheme theme)
                {
                    string display = export?.DisplayName ?? theme.Name;
                    results.Add(new DiscoveredTheme(theme, display, Path.GetFileName(path)));
                }
            }
            catch (Exception ex) when (IsRecoverableThemeActivationFailure(ex))
            {
                _logger.LogWarning(
                    ex,
                    "Skipping plug-in theme type {ThemeType} from {Path}. {Details}",
                    type.FullName ?? type.Name,
                    path,
                    DescribeLoadFailure(ex));
            }
        }

        return results;
    }

    private string CreateShadowCopy(string path, DateTime lastWriteUtc)
    {
        string shadowDirectory = Path.Combine(_pluginDirectory, ".shadow-cache");
        Directory.CreateDirectory(shadowDirectory);

        string safeName = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path);
        string shadowPath = Path.Combine(
            shadowDirectory,
            $"{safeName}.{lastWriteUtc.Ticks}.{Guid.NewGuid():N}{extension}");

        File.Copy(path, shadowPath, overwrite: false);
        return shadowPath;
    }

    private IEnumerable<Type> GetLoadableThemeTypes(Assembly assembly, string path)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            _logger.LogWarning(ex, "Some plug-in types could not be reflected from {Path}. {Details}", path, DescribeLoadFailure(ex));
            types = ex.Types.Where(type => type is not null).Cast<Type>().ToArray();
        }

        foreach (Type type in types)
        {
            if (typeof(IClockTheme).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                yield return type;
            }
        }
    }

    private static bool IsRecoverableAssemblyLoadFailure(Exception exception)
        => exception is BadImageFormatException
            or FileLoadException
            or FileNotFoundException
            or IOException
            or UnauthorizedAccessException
            or ReflectionTypeLoadException
            or TypeLoadException;

    private static bool IsRecoverableThemeActivationFailure(Exception exception)
        => exception is FileLoadException
            or FileNotFoundException
            or InvalidOperationException
            or MemberAccessException
            or MissingMethodException
            or TargetInvocationException
            or TypeLoadException;

    private static string DescribeLoadFailure(Exception exception)
        => exception is ReflectionTypeLoadException reflectionException
            ? string.Join(
                " | ",
                reflectionException.LoaderExceptions
                    .Where(loaderException => loaderException is not null)
                    .Select(loaderException => loaderException!.Message))
            : exception is TargetInvocationException { InnerException: Exception inner }
                ? inner.Message
                : exception.Message;

    private sealed class PluginLoadContext : AssemblyLoadContext
    {
        public PluginLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName) => null;
    }
}

/// <summary>A theme discovered from a plug-in assembly.</summary>
/// <param name="Theme">The theme instance.</param>
/// <param name="DisplayName">The name to show in the UI.</param>
/// <param name="SourceFile">The plug-in file name the theme came from.</param>
public sealed record DiscoveredTheme(IClockTheme Theme, string DisplayName, string SourceFile);
