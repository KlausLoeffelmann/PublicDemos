using System.Drawing;
using Microsoft.Extensions.Logging.Abstractions;
using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.App.Tests;

public sealed class ThemePluginLoaderTests
{
    [Fact]
    public void LoadNew_IsolatesPerTypeFailuresAndReloadsWhenTheAssemblyChanges()
    {
        string workDirectory = CreateWorkDirectory();

        try
        {
            string pluginPath = Path.Combine(workDirectory, "LoaderPlugin.dll");
            CopyCurrentTestAssembly(pluginPath);

            ThemePluginLoader loader = new(workDirectory, NullLogger<ThemePluginLoader>.Instance);

            IReadOnlyList<DiscoveredTheme> first = loader.LoadNew();
            DiscoveredTheme discovered = Assert.Single(first);
            Assert.Equal("Loader Happy Theme", discovered.Theme.Name);

            IReadOnlyList<DiscoveredTheme> second = loader.LoadNew();
            Assert.Empty(second);

            CopyCurrentTestAssembly(pluginPath);
            File.SetLastWriteTimeUtc(pluginPath, File.GetLastWriteTimeUtc(pluginPath).AddSeconds(5));
            IReadOnlyList<DiscoveredTheme> reloaded = loader.LoadNew();

            Assert.Single(reloaded);
        }
        finally
        {
            DeleteDirectory(workDirectory);
        }
    }

    [Fact]
    public void LoadNew_RetriesAfterTransientDllFailure()
    {
        string workDirectory = CreateWorkDirectory();

        try
        {
            string pluginPath = Path.Combine(workDirectory, "TransientPlugin.dll");
            File.WriteAllText(pluginPath, "not a valid assembly");

            ThemePluginLoader loader = new(workDirectory, NullLogger<ThemePluginLoader>.Instance);

            Assert.Empty(loader.LoadNew());

            CopyCurrentTestAssembly(pluginPath);
            IReadOnlyList<DiscoveredTheme> discovered = loader.LoadNew();

            Assert.Single(discovered);
        }
        finally
        {
            DeleteDirectory(workDirectory);
        }
    }

    private static void CopyCurrentTestAssembly(string destinationPath)
    {
        string sourcePath = typeof(ThemePluginLoaderTests).Assembly.Location;
        File.Copy(sourcePath, destinationPath, overwrite: true);
    }

    private static string CreateWorkDirectory()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", nameof(ThemePluginLoaderTests), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (Exception) when (Directory.Exists(path))
        {
        }
    }
}

[ClockThemeExport]
public sealed class LoaderHappyTheme : IClockTheme
{
    public string Name => "Loader Happy Theme";

    public string Description => "A test-only theme used to validate plug-in discovery.";

    public string Author => "Tests";

    public ThemeCapabilities Capabilities => ThemeCapabilities.Default;

    public IReadOnlyList<ClockElementDescriptor> CreateElements() => [];

    public IClockLayout CreateLayout() => new LoaderHappyLayout();

    public IClockElementRenderer CreateRenderer() => new LoaderHappyRenderer();

    public IThemeAnimator? CreateAnimator() => null;

    private sealed class LoaderHappyLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            anchor = default;
            return false;
        }
    }

    private sealed class LoaderHappyRenderer : IClockElementRenderer
    {
        public void DrawElement(ID2DGraphics graphics, IClockRenderContext context)
        {
        }
    }
}

[ClockThemeExport]
public sealed class LoaderBrokenTheme : IClockTheme
{
    public LoaderBrokenTheme()
    {
        throw new InvalidOperationException("Synthetic constructor failure for loader coverage.");
    }

    public string Name => "Loader Broken Theme";

    public string Description => "A test-only theme that must never be constructed successfully.";

    public string Author => "Tests";

    public ThemeCapabilities Capabilities => ThemeCapabilities.Default;

    public IReadOnlyList<ClockElementDescriptor> CreateElements() => [];

    public IClockLayout CreateLayout() => throw new NotSupportedException();

    public IClockElementRenderer CreateRenderer() => throw new NotSupportedException();

    public IThemeAnimator? CreateAnimator() => null;
}
