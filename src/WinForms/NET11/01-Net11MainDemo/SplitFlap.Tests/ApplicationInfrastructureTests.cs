using SplitFlap.Demo;

namespace SplitFlap.Tests;

public sealed class ApplicationInfrastructureTests
{
    [Fact]
    public void StartupOptions_ParsesScenarioTimerAndSettingsBypass()
    {
        bool success = StartupOptions.TryParse(
            ["--scenario", "all", "--run-for", "2.5", "--no-settings"],
            out StartupOptions options,
            out string? error);

        Assert.True(success, error);
        Assert.Equal(SmokeScenario.All, options.Scenario);
        Assert.Equal(TimeSpan.FromSeconds(2.5), options.RunFor);
        Assert.True(options.NoSettings);
    }

    [Theory]
    [InlineData("--scenario", "unknown")]
    [InlineData("--run-for", "0")]
    [InlineData("--unknown", "value")]
    public void StartupOptions_RejectsInvalidInput(string first, string second)
    {
        bool success = StartupOptions.TryParse(
            [first, second],
            out _,
            out string? error);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void Settings_RoundTripAndRecoverFromCorruption()
    {
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string path = Path.Combine(folder, "settings.json");

        try
        {
            AppSettings expected = new()
            {
                AutoSave = false,
                FontName = "Cascadia Mono",
                FontSize = 32,
                Rows = 12,
                Columns = 60,
                KeepAspectRatio = false,
                SoundEnabled = true,
                UpdateIntervalSeconds = 120
            };

            AppSettingsStore.Save(expected, path);
            AppSettings actual = AppSettingsStore.Load(path);

            Assert.Equal(expected.FontName, actual.FontName);
            Assert.Equal(expected.FontSize, actual.FontSize);
            Assert.Equal(expected.Rows, actual.Rows);
            Assert.Equal(expected.Columns, actual.Columns);
            Assert.False(actual.AutoSave);
            Assert.True(actual.SoundEnabled);
            Assert.Equal(120, actual.UpdateIntervalSeconds);

            File.WriteAllText(path, "{ not-json");
            AppSettings fallback = AppSettingsStore.Load(path);
            Assert.True(fallback.AutoSave);
        }
        finally
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
            }
        }
    }

    [Fact]
    public void ScaleToFit_PreservesAspectRatioWithinTarget()
    {
        Size fitted = DisplayLayoutCalculator.ScaleToFit(
            new Size(1600, 900),
            new Size(1000, 1000));

        Assert.Equal(new Size(1000, 562), fitted);
    }

    [Fact]
    public void CalculateGridFit_UsesMostOfTargetAtReadableScale()
    {
        DisplayLayoutCalculator.DisplayFit fit =
            DisplayLayoutCalculator.CalculateGridFit(
                currentRows: 9,
                currentColumns: 46,
                currentFontSize: 18,
                currentPreferredSize: new Size(1100, 400),
                targetSize: new Size(1760, 850));

        Assert.InRange(fit.Rows, 4, 16);
        Assert.InRange(fit.Columns, 32, 80);
        Assert.InRange(fit.FontSize, 4, 400);
        Assert.NotEqual((9, 46), (fit.Rows, fit.Columns));
    }

    [Fact]
    public void FlightBoard_SupportsHeaderOnlyGrid()
    {
        FlightBoard board = new(20);

        string first = board.Next(rows: 1);
        string second = board.Next(rows: 1);

        Assert.Equal(first, second);
        Assert.DoesNotContain(Environment.NewLine, first);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(10, 10)]
    [InlineData(14, 10)]
    [InlineData(15, 20)]
    [InlineData(127, 130)]
    [InlineData(999, 300)]
    public void UpdateInterval_NormalizesToTenSecondSteps(int value, int expected)
        => Assert.Equal(expected, UpdateInterval.Normalize(value));
}
