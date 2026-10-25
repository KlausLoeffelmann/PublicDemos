using System.Text.Json.Nodes;
using DrumMachine.Demo;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Percussion;

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
        AppSettings expected = new AppSettings
        {
            Theme = (AppTheme)theme,
            IconSize = (ToolbarIconSize)iconSize,
            FontSize = (AppFontSize)fontSize,
            DefaultFolder = files.DirectoryPath,
            BarsPerView = 2,
            RecentFiles = Enumerable.Range(1, 5).Select(index => files.File($"{index}.drumloop.json")).ToArray()
        }.WithTrackPreset(1, "Drums", [Cr78Instrument.SnareDrum, Cr78Instrument.BassDrum]);

        await AppSettingsStore.SaveAsync(expected, path, TestContext.Current.CancellationToken);
        AppSettings actual = AppSettingsStore.Load(path);

        Assert.Equal(expected.Theme, actual.Theme);
        Assert.Equal(expected.IconSize, actual.IconSize);
        Assert.Equal(expected.FontSize, actual.FontSize);
        Assert.Equal(expected.DefaultFolder, actual.DefaultFolder);
        Assert.Equal(expected.RecentFiles, actual.RecentFiles);
        Assert.Equal(expected.BarsPerView, actual.BarsPerView);
        Assert.Equal("Drums", actual.TrackPresets[1].Name);
        Assert.Equal([Cr78Instrument.BassDrum, Cr78Instrument.SnareDrum], actual.TrackPresets[1].Instruments);
        Assert.All(actual.TrackPresets.Where((_, index) => index != 1), preset => Assert.Empty(preset.Instruments));
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
    [InlineData("version", "4")]
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
        Assert.Equal(8, actual.TrackPresets.Count);
        Assert.All(actual.TrackPresets, preset => Assert.Empty(preset.Instruments));
    }

    /// <summary>
    ///  Migrates a version-two file that has no preset array without discarding its other choices.
    /// </summary>
    [Fact]
    public void Settings_VersionTwoKeepsChoicesAndStartsWithEmptyPresets()
    {
        using LoopTestFiles files = new();
        string path = files.File("settings.json");
        System.IO.File.WriteAllText(path, CreateValidJson(files.DirectoryPath).ToJsonString());

        AppSettings actual = AppSettingsStore.Load(path);

        Assert.Equal(AppTheme.Dark, actual.Theme);
        Assert.Equal(ToolbarIconSize.Large, actual.IconSize);
        Assert.Equal(AppFontSize.Xxl, actual.FontSize);
        Assert.Equal(2, actual.BarsPerView);
        Assert.Equal(8, actual.TrackPresets.Count);
        Assert.All(actual.TrackPresets, preset =>
        {
            Assert.Equal("", preset.Name);
            Assert.Empty(preset.Instruments);
        });
    }

    /// <summary>
    ///  Rejects a version-three preset list that is the wrong length or contains an unusable selection.
    /// </summary>
    [Theory]
    [InlineData("count")]
    [InlineData("unknown")]
    [InlineData("numeric")]
    [InlineData("metallic")]
    [InlineData("duplicate")]
    [InlineData("name")]
    public void Settings_InvalidTrackPresetsUseCompleteDefaults(string problem)
    {
        using LoopTestFiles files = new();
        JsonObject json = CreateValidJson(files.DirectoryPath);
        json["version"] = 3;
        JsonArray presets = [];
        int count = problem == "count" ? 7 : 8;
        for (int slot = 0; slot < count; slot++)
        {
            JsonArray instruments = [];
            if (slot == 0)
            {
                foreach (string token in problem switch
                {
                    "unknown" => ["NotADrum"],
                    "numeric" => ["0"],
                    "metallic" => ["MetallicBeat"],
                    "duplicate" => ["BassDrum", "BassDrum"],
                    _ => Array.Empty<string>()
                })
                {
                    instruments.Add(token);
                }
            }
            presets.Add(new JsonObject
            {
                ["name"] = problem == "name" && slot == 0 ? new string('a', 41) : "",
                ["instruments"] = instruments
            });
        }

        json["trackPresets"] = presets;
        string path = files.File("settings.json");
        System.IO.File.WriteAllText(path, json.ToJsonString());
        AssertDefaults(AppSettingsStore.Load(path));
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
    [InlineData("preset-count")]
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
            "preset-count" => valid with { TrackPresets = [] },
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
        Assert.Equal(8, settings.TrackPresets.Count);
        Assert.All(settings.TrackPresets, preset =>
        {
            Assert.Equal("", preset.Name);
            Assert.Empty(preset.Instruments);
        });
        Assert.False(string.IsNullOrWhiteSpace(settings.RecordingsFolder));
        Assert.False(string.IsNullOrWhiteSpace(settings.SoundPresetFolder));
        Assert.False(string.IsNullOrWhiteSpace(settings.LastWavImportFolder));
        Assert.False(settings.Window.HasBounds);
        Assert.Equal(0, settings.SpectrumSplitterDistanceDip);
        Assert.Empty(settings.StepGridColumns);
    }

    /// <summary>
    ///  Migrates retired track tokens and supplies the new folder defaults without discarding version-three choices.
    /// </summary>
    [Fact]
    public void Settings_VersionThreeMigratesLegacyTrackSelections()
    {
        using LoopTestFiles files = new();
        JsonObject json = CreateValidJson(files.DirectoryPath);
        json["version"] = 3;
        JsonArray presets = [];
        for (int slot = 0; slot < 8; slot++)
        {
            JsonArray instruments = [];
            if (slot == 0)
            {
                instruments.Add("Maracas");
                instruments.Add("Claves");
            }
            presets.Add(new JsonObject { ["name"] = slot == 0 ? "Legacy" : "", ["instruments"] = instruments });
        }

        json["trackPresets"] = presets;
        string path = files.File("settings.json");
        System.IO.File.WriteAllText(path, json.ToJsonString());

        AppSettings actual = AppSettingsStore.Load(path);

        Assert.Equal(["Tambourine2", "Guiro2"], actual.TrackPresets[0].Instruments.Select(item => item.ToString()));
        Assert.Equal("Legacy", actual.TrackPresets[0].Name);
        Assert.Equal(AppTheme.Dark, actual.Theme);
        Assert.True(Directory.Exists(actual.RecordingsFolder));
        Assert.True(Directory.Exists(actual.SoundPresetFolder));
        Assert.False(actual.Window.HasBounds);
    }

    /// <summary>
    ///  Round-trips recordings, preset, and import folders plus window and column layout.
    /// </summary>
    [Fact]
    public async Task Settings_RoundTripFoldersWindowAndColumns()
    {
        using LoopTestFiles files = new();
        string path = Path.Combine(files.DirectoryPath, "app-data", "settings.json");
        AppSettings expected = new AppSettings
        {
            DefaultFolder = files.DirectoryPath,
            RecordingsFolder = files.DirectoryPath,
            SoundPresetFolder = files.DirectoryPath,
            LastWavImportFolder = files.DirectoryPath,
            Window = new WindowPlacement
            {
                X = 40,
                Y = 50,
                Width = 1100,
                Height = 760,
                IsMaximized = true,
                SavedDpi = 144
            },
            SpectrumSplitterDistanceDip = 180,
            StepGridColumns = StepGridColumnPreference.Names
                .Select((name, index) => new StepGridColumnPreference
                {
                    Name = name,
                    DisplayIndex = index,
                    WidthDip = 40
                })
                .ToArray()
        };

        await AppSettingsStore.SaveAsync(expected, path, TestContext.Current.CancellationToken);
        AppSettings actual = AppSettingsStore.Load(path);

        Assert.Equal(files.DirectoryPath, actual.RecordingsFolder);
        Assert.Equal(files.DirectoryPath, actual.SoundPresetFolder);
        Assert.Equal(files.DirectoryPath, actual.LastWavImportFolder);
        Assert.Equal(expected.Window, actual.Window);
        Assert.Equal(180, actual.SpectrumSplitterDistanceDip);
        Assert.Equal(expected.StepGridColumns, actual.StepGridColumns);
        string json = await System.IO.File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
        Assert.Contains("\"version\": 4", json, StringComparison.Ordinal);
    }

    /// <summary>
    ///  Rejects a version-four file that is missing one of the new required fields.
    /// </summary>
    [Theory]
    [InlineData("recordingsFolder")]
    [InlineData("soundPresetFolder")]
    [InlineData("lastWavImportFolder")]
    [InlineData("windowPlacement")]
    [InlineData("spectrumSplitterDip")]
    [InlineData("stepGridColumns")]
    public async Task Settings_VersionFourMissingFieldsUseCompleteDefaults(string property)
    {
        using LoopTestFiles files = new();
        string path = files.File("settings.json");
        await AppSettingsStore.SaveAsync(
            new AppSettings { DefaultFolder = files.DirectoryPath, Theme = AppTheme.Dark },
            path,
            TestContext.Current.CancellationToken);
        JsonObject json = JsonNode.Parse(System.IO.File.ReadAllText(path))!.AsObject();
        LoopTestFiles.RemoveJsonProperty(json, property);
        System.IO.File.WriteAllText(path, json.ToJsonString());

        AssertDefaults(AppSettingsStore.Load(path));
    }

    [Theory]
    [InlineData(96, 800, 600)]
    [InlineData(144, 1200, 900)]
    [InlineData(192, 1600, 1200)]
    public void Placement_ScalesSaved96DpiSizeTo144And192(int dpi, int width, int height)
    {
        WindowPlacement saved = new() { X = 12, Y = 18, Width = 800, Height = 600, SavedDpi = 96 };
        Rectangle scaled = saved.ScaleToDpi(dpi);
        Assert.Equal(new Rectangle(12, 18, width, height), scaled);
        Assert.Equal(36, WindowPlacement.ScaleFromDip(36, 96));
        Assert.Equal(dpi == 96 ? 36 : dpi == 144 ? 54 : 72, WindowPlacement.ScaleFromDip(36, dpi));
        Assert.False(WindowPlacement.IsUsableOnScreen(new Rectangle(-20_000, -20_000, 900, 700)));
    }
}
