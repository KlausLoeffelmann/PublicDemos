using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using WarpClock.Abstractions;

namespace WarpClock.App.Tests;

public sealed class ThemeSetStoreTests
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(),
        },
    };

    [Fact]
    public void LoadFromPath_MigratesLegacyThemeKeysAndClearsInvalidNightEligibility()
    {
        string workDirectory = CreateWorkDirectory();

        try
        {
            string path = Path.Combine(workDirectory, "legacy-themelist.json");
            ThemeScheduleDocument legacy = new()
            {
                Name = "Legacy",
                Entries =
                [
                    new ThemeScheduleEntry
                    {
                        Theme = new ThemeReference
                        {
                            ThemeKey = "Nerd.dll|Nerd|WarpClock.Themes.Nerd.NerdTheme",
                            Variant = ClockThemeVariantKind.Day,
                        },
                        DisplayName = "Nerd",
                        Source = "Nerd.dll",
                        Enabled = true,
                        EligibleDuringDay = true,
                        EligibleDuringNight = true,
                    },
                ],
            };

            File.WriteAllText(path, JsonSerializer.Serialize(legacy, s_jsonOptions));

            ThemeSetStore store = new(new AppPaths(), NullLogger<ThemeSetStore>.Instance);
            ThemeCatalogInfo[] catalog =
            [
                new()
                {
                    ThemeKey = "nerd",
                    FamilyName = "Nerd",
                    Source = "stock",
                    SupportedVariants = ClockThemeVariants.DayNight,
                },
            ];

            ThemeScheduleDocument loaded = store.LoadFromPath(path, catalog);
            ThemeScheduleEntry entry = Assert.Single(loaded.Entries);

            Assert.Equal("nerd", entry.Theme.ThemeKey);
            Assert.True(entry.EligibleDuringDay);
            Assert.False(entry.EligibleDuringNight);
            Assert.Equal("stock", entry.Source);
        }
        finally
        {
            DeleteDirectory(workDirectory);
        }
    }

    [Fact]
    public void MigrateLegacyDefaultFile_CreatesCanonicalThemesetAndRemovesLegacyFile()
    {
        string workDirectory = CreateWorkDirectory();

        try
        {
            string legacyPath = Path.Combine(workDirectory, "themelist.json");
            string canonicalPath = Path.Combine(workDirectory, "default.themeset.json");
            ThemeScheduleDocument legacy = new()
            {
                Name = "Migrated Set",
                Entries =
                [
                    new ThemeScheduleEntry
                    {
                        Theme = new ThemeReference
                        {
                            ThemeKey = "scatter",
                        },
                        DisplayName = "Scatter",
                        Source = "stock",
                        Enabled = true,
                        EligibleDuringDay = true,
                        EligibleDuringNight = true,
                    },
                ],
            };

            File.WriteAllText(legacyPath, JsonSerializer.Serialize(legacy, s_jsonOptions));

            ThemeSetStore store = new(new AppPaths(), NullLogger<ThemeSetStore>.Instance);
            ThemeCatalogInfo[] catalog =
            [
                new()
                {
                    ThemeKey = "scatter",
                    FamilyName = "Scatter",
                    Source = "stock",
                    SupportedVariants = ClockThemeVariants.DayNight,
                },
            ];

            ThemeScheduleDocument migrated = store.MigrateLegacyDefaultFile(legacyPath, canonicalPath, catalog);

            Assert.Equal("Migrated Set", migrated.Name);
            Assert.True(File.Exists(canonicalPath));
            Assert.False(File.Exists(legacyPath));
        }
        finally
        {
            DeleteDirectory(workDirectory);
        }
    }

    private static string CreateWorkDirectory()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", nameof(ThemeSetStoreTests), Guid.NewGuid().ToString("N"));
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
