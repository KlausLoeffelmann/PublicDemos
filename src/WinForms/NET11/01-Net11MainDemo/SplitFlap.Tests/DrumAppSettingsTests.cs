using System.Text.Json.Nodes;
using DrumMachine.Demo;
using DrumMachine.Demo.Documents;

namespace SplitFlap.Tests;

/// <summary>
///  Verifies separate application defaults, bounded Windows recent paths, and failure-safe preference persistence.
/// </summary>
public sealed class DrumAppSettingsTests
{
    /// <summary>
    ///  Uses System, the WinForms standard font, thirty-two-pixel icons, Documents, and a one-bar viewport as defaults.
    /// </summary>
    [Fact]
    public void Defaults_AreIndependentOfMusicalDocuments()
    {
        AssertDefaults(new AppSettings());
        Assert.Equal(32, (int)ToolbarIconSize.Small);
        Assert.Equal(48, (int)ToolbarIconSize.Medium);
        Assert.Equal(64, (int)ToolbarIconSize.Large);
        Assert.Equal(0f, AppFontSizing.GetPointIncrement(AppFontSize.Small));
        Assert.Equal(2f, AppFontSizing.GetPointIncrement(AppFontSize.Normal));
        Assert.Equal(4f, AppFontSizing.GetPointIncrement(AppFontSize.Large));
        Assert.Equal(6f, AppFontSizing.GetPointIncrement(AppFontSize.Xxl));
        string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        Assert.Equal(Path.Combine(localData, "DrumMachine.Demo", "settings.json"), AppPaths.SettingsFile);
        Assert.Equal(Path.Combine(localData, "DrumMachine.Demo", "Logs"), AppPaths.LogDirectory);
        Assert.NotEqual(SplitFlap.Demo.AppPaths.SettingsFile, AppPaths.SettingsFile);
        Assert.NotEqual(SplitFlap.Demo.AppPaths.LogDirectory, AppPaths.LogDirectory);
    }

    /// <summary>
    ///  Normalizes relative components, deduplicates Windows case variants, and orders only the latest five paths.
    /// </summary>
    [Fact]
    public void Recents_NormalizeDeduplicateAndRetainFiveInSuccessfulOperationOrder()
    {
        using LoopTestFiles files = new();
        AppSettings original = new() { DefaultFolder = files.DirectoryPath, BarsPerView = 2 };
        AppSettings settings = original;
        for (int index = 1; index <= 6; index++)
        {
            settings = settings.WithRecentFile(files.File($"loop-{index}.drumloop.json"));
        }

        string normalizedThird = files.File("loop-3.drumloop.json").ToUpperInvariant();
        string equivalentThird = Path.Combine(files.DirectoryPath.ToUpperInvariant(), "unused", "..", "LOOP-3.DRUMLOOP.JSON");
        settings = settings.WithRecentFile(equivalentThird);

        Assert.Equal(
            [
                normalizedThird,
                files.File("loop-6.drumloop.json"),
                files.File("loop-5.drumloop.json"),
                files.File("loop-4.drumloop.json"),
                files.File("loop-2.drumloop.json")
            ],
            settings.RecentFiles);
        Assert.Empty(original.RecentFiles);
        Assert.Equal(files.DirectoryPath, settings.DefaultFolder);
        Assert.Equal(2, settings.BarsPerView);

        AppSettings removed = settings.WithRemovedRecentFile(files.File("loop-3.drumloop.json"));
        Assert.Equal(4, removed.RecentFiles.Count);
        Assert.Equal(files.File("loop-6.drumloop.json"), removed.RecentFiles[0]);
        Assert.Equal(5, settings.RecentFiles.Count);
        Assert.Equal(removed.RecentFiles,
            removed.WithRemovedRecentFile(files.File("never-opened.drumloop.json")).RecentFiles);
        Assert.Empty(Directory.EnumerateFileSystemEntries(files.DirectoryPath));
    }

    /// <summary>
    ///  Defensively copies recent paths so editing caller-owned arrays cannot mutate an in-flight settings save.
    /// </summary>
    [Fact]
    public void Recents_DoNotRetainMutableCallerCollections()
    {
        using LoopTestFiles files = new();
        string path = files.File("first.drumloop.json");
        string[] source = [path];
        AppSettings settings = new() { RecentFiles = source };
        source[0] = files.File("changed.drumloop.json");
        Assert.Equal(path, Assert.Single(settings.RecentFiles));
        Assert.Throws<NotSupportedException>(() => ((IList<string>)settings.RecentFiles)[0] = source[0]);
        Assert.Throws<ArgumentException>(() => settings.WithRecentFile(" "));
        Assert.Throws<ArgumentException>(() => settings.WithRecentFile(files.File("invalid*.drumloop.json")));
        Assert.Throws<ArgumentException>(() =>
            settings.WithRecentFile(files.DirectoryPath + Path.DirectorySeparatorChar));
    }

    /// <summary>
    ///  Round-trips supported theme, icon-size, and relative font-size choices in an isolated preferences folder.
    /// </summary>
    [Theory]
    [InlineData(0, 32, 0)]
    [InlineData(1, 48, 1)]
    [InlineData(2, 64, 3)]
    public async Task Settings_RoundTripAllPreferencesAndRecentPaths(int theme, int iconSize, int fontSize)
    {
        using LoopTestFiles files = new();
        string path = Path.Combine(files.DirectoryPath, "app-data", "settings.json");
        AppSettings expected = new()
        {
            Theme = (AppTheme)theme,
            IconSize = (ToolbarIconSize)iconSize,
            FontSize = (AppFontSize)fontSize,
            DefaultFolder = files.DirectoryPath,
            BarsPerView = 2,
            RecentFiles = Enumerable.Range(1, 5).Select(index => files.File($"{index}.drumloop.json")).ToArray()
        };

        await AppSettingsStore.SaveAsync(expected, path, TestContext.Current.CancellationToken);
        AppSettings actual = AppSettingsStore.Load(path);

        Assert.Equal(expected.Theme, actual.Theme);
        Assert.Equal(expected.IconSize, actual.IconSize);
        Assert.Equal(expected.FontSize, actual.FontSize);
        Assert.Equal(expected.DefaultFolder, actual.DefaultFolder);
        Assert.Equal(expected.RecentFiles, actual.RecentFiles);
        Assert.Equal(expected.BarsPerView, actual.BarsPerView);
        string json = await System.IO.File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
        Assert.Contains($"\"{expected.Theme}\"", json, StringComparison.Ordinal);
        Assert.Contains($"\"{expected.IconSize}\"", json, StringComparison.Ordinal);
        Assert.Contains($"\"{expected.FontSize}\"", json, StringComparison.Ordinal);
        Assert.Empty(Directory.EnumerateFiles(Path.GetDirectoryName(path)!, "*.tmp"));
    }

    /// <summary>
    ///  Uses defaults for absent, empty, malformed, unsupported, duplicate, and oversized preference files.
    /// </summary>
    [Theory]
    [InlineData("missing")]
    [InlineData("empty")]
    [InlineData("null")]
    [InlineData("malformed")]
    [InlineData("missing-fields")]
    [InlineData("duplicate-property")]
    [InlineData("unknown-property")]
    [InlineData("oversized")]
    public void Settings_InvalidOrAbsentDataUsesExplicitDefaults(string problem)
    {
        using LoopTestFiles files = new();
        string path = files.File("settings.json");
        if (problem == "oversized")
        {
            using FileStream stream = new(path, FileMode.CreateNew, FileAccess.Write);
            stream.SetLength(AppSettingsStore.MaximumFileBytes + 1L);
        }
        else if (problem != "missing")
        {
            string json = CreateValidJson(files.DirectoryPath).ToJsonString();
            string invalid = problem switch
            {
                "empty" => "",
                "null" => "null",
                "malformed" => "{broken",
                "missing-fields" => "{}",
                "duplicate-property" => json.Insert(1, "\"version\":1,"),
                "unknown-property" => json.Insert(1, "\"unexpected\":true,"),
                _ => throw new ArgumentException(nameof(problem))
            };
            System.IO.File.WriteAllText(path, invalid);
        }

        AssertDefaults(AppSettingsStore.Load(path));
        if (problem == "missing")
        {
            Assert.False(System.IO.File.Exists(path));
        }
    }

    /// <summary>
    ///  Validates enum strings, view size, folder syntax, and bounded recent entries instead of keeping partial choices.
    /// </summary>
    [Theory]
    [InlineData("version", "3")]
    [InlineData("theme", "\"Unknown\"")]
    [InlineData("theme", "1")]
    [InlineData("theme", "\"1\"")]
    [InlineData("iconSize", "\"Huge\"")]
    [InlineData("iconSize", "32")]
    [InlineData("iconSize", "\"32\"")]
    [InlineData("fontSize", "\"Huge\"")]
    [InlineData("fontSize", "2")]
    [InlineData("defaultFolder", "\"\"")]
    [InlineData("defaultFolder", "\"relative-folder\"")]
    [InlineData("defaultFolder", "null")]
    [InlineData("defaultFolder", "\"C:\\\\invalid*folder\"")]
    [InlineData("barsPerView", "0")]
    [InlineData("barsPerView", "3")]
    [InlineData("barsPerView", "\"1\"")]
    [InlineData("recentFiles", "null")]
    [InlineData("recentFiles", "[null]")]
    [InlineData("recentFiles", "[1]")]
    [InlineData("recentFiles", "[\"relative.drumloop.json\"]")]
    [InlineData("recentFiles", "[\"C:\\\\a.json\",\"c:\\\\A.json\"]")]
    [InlineData("recentFiles", "[\"C:\\\\1.json\",\"C:\\\\2.json\",\"C:\\\\3.json\",\"C:\\\\4.json\",\"C:\\\\5.json\",\"C:\\\\6.json\"]")]
    public void Settings_InvalidPreferenceValuesUseCompleteDefaults(string propertyPath, string value)
    {
        using LoopTestFiles files = new();
        JsonObject json = CreateValidJson(files.DirectoryPath);
        LoopTestFiles.SetJsonValue(json, propertyPath, JsonNode.Parse(value));
        string path = files.File("settings.json");
        System.IO.File.WriteAllText(path, json.ToJsonString());
        AssertDefaults(AppSettingsStore.Load(path));
    }

    /// <summary>
    ///  Rejects missing preference fields explicitly rather than retaining an accidental mixture of defaults.
    /// </summary>
    [Theory]
    [InlineData("version")]
    [InlineData("theme")]
    [InlineData("iconSize")]
    [InlineData("fontSize")]
    [InlineData("defaultFolder")]
    [InlineData("recentFiles")]
    [InlineData("barsPerView")]
    public void Settings_MissingPreferenceFieldsUseCompleteDefaults(string property)
    {
        using LoopTestFiles files = new();
        JsonObject json = CreateValidJson(files.DirectoryPath);
        LoopTestFiles.RemoveJsonProperty(json, property);
        string path = files.File("settings.json");
        System.IO.File.WriteAllText(path, json.ToJsonString());
        AssertDefaults(AppSettingsStore.Load(path));
    }

    /// <summary>
    ///  Migrates version-one preferences to the WinForms-standard font without discarding other choices.
    /// </summary>
    [Fact]
    public void Settings_VersionOneUsesTheWinFormsStandardFont()
    {
        using LoopTestFiles files = new();
        JsonObject json = CreateValidJson(files.DirectoryPath);
        json["version"] = 1;
        LoopTestFiles.RemoveJsonProperty(json, "fontSize");
        string path = files.File("settings.json");
        System.IO.File.WriteAllText(path, json.ToJsonString());

        AppSettings actual = AppSettingsStore.Load(path);

        Assert.Equal(AppTheme.Dark, actual.Theme);
        Assert.Equal(ToolbarIconSize.Large, actual.IconSize);
        Assert.Equal(AppFontSize.Small, actual.FontSize);
        Assert.Equal(files.DirectoryPath, actual.DefaultFolder);
        Assert.Equal(2, actual.BarsPerView);
    }

    /// <summary>
    ///  Derives each selection from the supplied base font so repeated choices never accumulate point changes.
    /// </summary>
    [Fact]
    public void FontSizing_PreservesFontIdentityAndUsesNonCumulativePointIncrements()
    {
        using Font baseline = new(
            SystemFonts.DefaultFont.FontFamily,
            11f,
            FontStyle.Bold | FontStyle.Italic,
            GraphicsUnit.Point,
            SystemFonts.DefaultFont.GdiCharSet,
            SystemFonts.DefaultFont.GdiVerticalFont);
        using Font normal = AppFontSizing.CreateFont(baseline, AppFontSize.Normal);
        using Font large = AppFontSizing.CreateFont(baseline, AppFontSize.Large);
        using Font repeatedNormal = AppFontSizing.CreateFont(baseline, AppFontSize.Normal);

        Assert.Equal(baseline.FontFamily.Name, normal.FontFamily.Name);
        Assert.Equal(baseline.Style, normal.Style);
        Assert.Equal(baseline.GdiCharSet, normal.GdiCharSet);
        Assert.Equal(13f, normal.SizeInPoints, 3);
        Assert.Equal(15f, large.SizeInPoints, 3);
        Assert.Equal(normal.SizeInPoints, repeatedNormal.SizeInPoints, 3);
    }

    /// <summary>
    ///  Leaves the old preferences untouched when an in-memory Options result fails validation.
    /// </summary>
    [Theory]
    [InlineData("theme")]
    [InlineData("icon-size")]
    [InlineData("font-size")]
    [InlineData("view")]
    [InlineData("folder")]
    [InlineData("relative-recent")]
    [InlineData("duplicate-recent")]
    [InlineData("too-many-recents")]
    public async Task Settings_InvalidSavePropagatesAndPreservesThePreviousFile(string problem)
    {
        using LoopTestFiles files = new();
        string path = files.File("settings.json");
        AppSettings valid = new() { DefaultFolder = files.DirectoryPath, Theme = AppTheme.Dark };
        await AppSettingsStore.SaveAsync(valid, path, TestContext.Current.CancellationToken);
        byte[] previous = await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        AppSettings invalid = problem switch
        {
            "theme" => valid with { Theme = (AppTheme)99 },
            "icon-size" => valid with { IconSize = (ToolbarIconSize)16 },
            "font-size" => valid with { FontSize = (AppFontSize)99 },
            "view" => valid with { BarsPerView = 4 },
            "folder" => valid with { DefaultFolder = "" },
            "relative-recent" => valid with { RecentFiles = ["relative.json"] },
            "duplicate-recent" => valid with { RecentFiles = [files.File("same.json"), files.File("SAME.json")] },
            "too-many-recents" => valid with
            {
                RecentFiles = Enumerable.Range(1, 6).Select(index => files.File($"{index}.json")).ToArray()
            },
            _ => throw new ArgumentException(nameof(problem))
        };

        await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            AppSettingsStore.SaveAsync(invalid, path, TestContext.Current.CancellationToken));
        Assert.Equal(previous, await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken));
        Assert.Empty(Directory.EnumerateFiles(files.DirectoryPath, "*.tmp"));
    }

    /// <summary>
    ///  Propagates a locked destination and cancellation without claiming success or replacing the last good options.
    /// </summary>
    [Fact]
    public async Task Settings_FailedPublicationAndCancellationPreservePreviousPreferences()
    {
        using LoopTestFiles files = new();
        string path = files.File("settings.json");
        AppSettings original = new() { DefaultFolder = files.DirectoryPath };
        await AppSettingsStore.SaveAsync(original, path, TestContext.Current.CancellationToken);
        byte[] previous = await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        using (FileStream heldOpen = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            await Assert.ThrowsAnyAsync<IOException>(() =>
                AppSettingsStore.SaveAsync(original with { Theme = AppTheme.Dark }, path, TestContext.Current.CancellationToken));
        }

        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            AppSettingsStore.SaveAsync(original with { IconSize = ToolbarIconSize.Large }, path, cancellation.Token));
        Assert.Equal(previous, await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken));
        Assert.Empty(Directory.EnumerateFiles(files.DirectoryPath, "*.tmp"));
    }

    /// <summary>
    ///  Does not confuse preferences with musical documents even when users choose a misleading file extension.
    /// </summary>
    [Fact]
    public async Task Settings_AndLoopFilesAreNotInterchangeable()
    {
        using LoopTestFiles files = new();
        string preferences = files.File("preferences.drumloop.json");
        string loop = files.File("loop-settings.json");
        await AppSettingsStore.SaveAsync(
            new AppSettings { DefaultFolder = files.DirectoryPath }, preferences, TestContext.Current.CancellationToken);
        await LoopDocumentStore.SaveAsync(LoopDocument.CreateEmpty(4), loop, TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(preferences, TestContext.Current.CancellationToken));
        AssertDefaults(AppSettingsStore.Load(loop));
    }

    private static JsonObject CreateValidJson(string folder)
        => new()
        {
            ["version"] = 2,
            ["theme"] = "Dark",
            ["iconSize"] = "Large",
            ["fontSize"] = "Xxl",
            ["defaultFolder"] = folder,
            ["recentFiles"] = new JsonArray(),
            ["barsPerView"] = 2
        };

    private static void AssertDefaults(AppSettings settings)
    {
        Assert.Equal(AppTheme.System, settings.Theme);
        Assert.Equal(ToolbarIconSize.Small, settings.IconSize);
        Assert.Equal(AppFontSize.Small, settings.FontSize);
        Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), settings.DefaultFolder);
        Assert.Empty(settings.RecentFiles);
        Assert.Equal(1, settings.BarsPerView);
    }
}
