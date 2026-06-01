using WingetPackageEditor.Core.Models;
using WingetPackageEditor.Core.Services;

namespace WingetPackageEditor.Tests;

public sealed class JsonPackageStoreTests
{
    [Fact]
    public void SaveAndLoadAll_RoundtripsPackages()
    {
        using TempDirectory root = new();
        JsonPackageStore store = new(root.Path);
        WingetPackage package = new()
        {
            Name = "Roundtrip",
            Apps = [new GenericAppEntry { Id = "Git.Git", DisplayName = "Git" }]
        };

        store.Save(package);
        IReadOnlyList<WingetPackage> loaded = store.LoadAll();

        WingetPackage restored = Assert.Single(loaded);
        Assert.Equal("Roundtrip", restored.Name);
        Assert.Equal(package.Id, restored.Id);
        Assert.Single(restored.Apps);
    }

    [Fact]
    public void LoadAll_ReturnsEmpty_WhenNothingSaved()
    {
        using TempDirectory root = new();
        JsonPackageStore store = new(root.Path);

        Assert.Empty(store.LoadAll());
    }

    [Fact]
    public void Delete_WritesBackupAndRemovesPackageFile()
    {
        using TempDirectory root = new();
        JsonPackageStore store = new(root.Path);
        WingetPackage package = new() { Name = "ToRemove" };
        store.Save(package);

        store.Delete(package);

        Assert.Empty(store.LoadAll());
        string[] backups = Directory.GetFiles(Path.Combine(root.Path, "Backups"), "WPE*.bak");
        Assert.Single(backups);
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "wpe-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Path, recursive: true);
            }
            catch
            {
                // Best-effort cleanup.
            }
        }
    }
}
