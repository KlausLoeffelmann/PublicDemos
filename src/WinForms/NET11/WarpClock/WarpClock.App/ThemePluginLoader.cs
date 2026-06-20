using System.Reflection;
using System.Runtime.Loader;

using WarpClock.Abstractions;

namespace WarpClock.App;

/// <summary>
///  Discovers <see cref="IClockTheme"/> implementations in drop-in assemblies under a
///  plug-ins directory. Each assembly is loaded into its own collectible
///  <see cref="AssemblyLoadContext"/> that defers shared contract / runtime assemblies
///  (e.g. <c>WarpClock.Abstractions</c>, <c>WarpToolkit.WinForms.DirectX</c>) to the
///  default context, so theme types share identity with the host.
/// </summary>
public sealed class ThemePluginLoader
{
    private readonly string _pluginDirectory;
    private readonly HashSet<string> _loadedPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<PluginLoadContext> _contexts = [];

    public ThemePluginLoader(string pluginDirectory) => _pluginDirectory = pluginDirectory;

    /// <summary>The directory scanned for plug-in assemblies.</summary>
    public string PluginDirectory => _pluginDirectory;

    /// <summary>
    ///  Loads any plug-in assemblies not yet loaded and returns the themes they contribute.
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
            if (!_loadedPaths.Add(path))
            {
                continue;
            }

            try
            {
                discovered.AddRange(LoadAssembly(path));
            }
            catch (Exception ex) when (ex is BadImageFormatException or FileLoadException or ReflectionTypeLoadException)
            {
                // Skip assemblies that are not loadable plug-ins.
            }
        }

        return discovered;
    }

    private IEnumerable<DiscoveredTheme> LoadAssembly(string path)
    {
        var context = new PluginLoadContext();
        Assembly assembly = context.LoadFromAssemblyPath(path);
        _contexts.Add(context);

        var results = new List<DiscoveredTheme>();
        foreach (Type type in assembly.GetTypes())
        {
            if (!typeof(IClockTheme).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            var export = type.GetCustomAttribute<ClockThemeExportAttribute>();
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

        return results;
    }

    private sealed class PluginLoadContext : AssemblyLoadContext
    {
        public PluginLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName) => null; // defer to the default context
    }
}

/// <summary>A theme discovered from a plug-in assembly.</summary>
/// <param name="Theme">The theme instance.</param>
/// <param name="DisplayName">The name to show in the UI.</param>
/// <param name="SourceFile">The plug-in file name the theme came from.</param>
public sealed record DiscoveredTheme(IClockTheme Theme, string DisplayName, string SourceFile);
