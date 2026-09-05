using DrumMachine.Demo;

namespace SplitFlap.Tests;

public sealed class DrumDemoInfrastructureTests
{
    [Theory]
    [InlineData("kit", "Kit")]
    [InlineData("score", "Score")]
    [InlineData("spectrum", "Spectrum")]
    [InlineData("document", "Document")]
    [InlineData("ALL", "All")]
    public void Options_ParseKnownScenarios(string value, string expected)
    {
        Assert.True(StartupOptions.TryParse(
            ["--scenario", value, "--run-for", "2.5"], out StartupOptions options, out string? error));
        Assert.Null(error);
        Assert.Equal(expected, options.Scenario.ToString());
        Assert.Equal(TimeSpan.FromSeconds(2.5), options.RunFor);
    }

    [Theory]
    [InlineData("--scenario", "1")]
    [InlineData("--scenario", "none")]
    [InlineData("--scenario", "kit,score")]
    [InlineData("--run-for", "NaN")]
    [InlineData("--run-for", "Infinity")]
    [InlineData("--run-for", "-1")]
    [InlineData("--run-for", "3601")]
    [InlineData("--unknown", "anything")]
    public void Options_RejectInvalidArguments(string option, string value)
    {
        Assert.False(StartupOptions.TryParse([option, value], out _, out string? error));
        Assert.NotNull(error);
    }

    [Fact]
    public void Options_RejectMissingValues()
    {
        Assert.False(StartupOptions.TryParse(["--scenario"], out _, out _));
        Assert.False(StartupOptions.TryParse(["--run-for"], out _, out _));
    }

    [Fact]
    public void Options_CanIsolateUserPreferences()
    {
        Assert.True(StartupOptions.TryParse(["--no-settings"], out StartupOptions options, out _));
        Assert.True(options.NoSettings);
    }

    [Fact]
    public void OriginalScore_UsesTwoBarsAndIsEditableWithoutMutatingThePreset()
    {
        var score = DemoScores.OriginalBallad;
        Assert.Equal(2, score.BarCount);
        var changed = score.WithStep(0, SplitFlap.Audio.Percussion.Cr78Instrument.Cowbell, 1, true);

        Assert.False(score.HasHit(0, SplitFlap.Audio.Percussion.Cr78Instrument.Cowbell, 1));
        Assert.True(changed.HasHit(0, SplitFlap.Audio.Percussion.Cr78Instrument.Cowbell, 1));
    }

    [Fact]
    public void Diagnostics_DoNotShareTheDepartureBoardsDirectory()
    {
        Assert.NotEqual(SplitFlap.Demo.AppPaths.LogDirectory, AppPaths.LogDirectory);
        Assert.EndsWith(Path.Combine("DrumMachine.Demo", "Logs"), AppPaths.LogDirectory);
        Assert.EndsWith(Path.Combine("SplitFlap.Demo", "Logs"), SplitFlap.Demo.AppPaths.LogDirectory);
    }
}
