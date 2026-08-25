using System.Text.Json;
using WarpClock.Abstractions;
using WarpClock.Engine;

namespace WarpClock.App.Tests;

public sealed class PersistedAppStateTests
{
    [Fact]
    public void Normalize_MigratesLegacyCurrentThemeKey()
    {
        PersistedAppState state = new()
        {
            Theme = new PersistedThemeState
            {
                CurrentTheme = new ThemeReference
                {
                    ThemeKey = "Scatter.dll|Scatter|WarpClock.Themes.Scatter.ScatterTheme",
                },
            },
        };

        state.Normalize();

        Assert.Equal("scatter", state.Theme.CurrentTheme?.ThemeKey);
    }

    [Fact]
    public void Normalize_MigratesLegacyCustomThemePropertyKeys()
    {
        PersistedAppState state = new()
        {
            Theme = new PersistedThemeState
            {
                CustomPropertyValues =
                [
                    new PersistedThemeCustomPropertyValue
                    {
                        ThemeKey = "Scatter.dll|Scatter|WarpClock.Themes.Scatter.ScatterTheme",
                        PropertyName = " AccentColor ",
                        Value = "Red",
                    },
                ],
            },
        };

        state.Normalize();

        PersistedThemeCustomPropertyValue property = Assert.Single(state.Theme.CustomPropertyValues);
        Assert.Equal("scatter", property.ThemeKey);
        Assert.Equal("AccentColor", property.PropertyName);
        Assert.Equal("Red", property.Value);
    }

    [Fact]
    public void Deserialize_MigratesLegacyThemeListPaths()
    {
        const string json = """
            {
              "Theme": {
                "CurrentThemeListPath": "C:\\themes\\current.json",
                "DefaultThemeListPath": "C:\\themes\\default.json"
              }
            }
            """;

        PersistedAppState state = JsonSerializer.Deserialize<PersistedAppState>(json)
            ?? throw new InvalidOperationException("Could not deserialize persisted state.");

        state.Normalize();

        Assert.Equal("C:\\themes\\current.json", state.Theme.CurrentThemeSetPath);
        Assert.Equal("C:\\themes\\default.json", state.Theme.DefaultThemeSetPath);
        Assert.DoesNotContain("ThemeListPath", JsonSerializer.Serialize(state));
    }

    [Fact]
    public void Normalize_MigratesLegacyHandSettingsIntoOptions()
    {
        PersistedAppState state = new()
        {
            SchemaVersion = 5,
            Clock = new PersistedClockSettings
            {
                HourMotion = ClockHandMotion.Tick,
                MinuteMotion = ClockHandMotion.Sweep,
                SecondMotion = ClockHandMotion.Crawling,
                GraceSeconds = 9,
            },
        };

        state.Normalize();

        Assert.Equal(PersistedAppState.CurrentSchemaVersion, state.SchemaVersion);
        Assert.Equal(ClockHandMotion.Tick, state.Options.Hands.HourMotion);
        Assert.Equal(ClockHandMotion.Sweep, state.Options.Hands.MinuteMotion);
        Assert.Equal(ClockHandMotion.Crawling, state.Options.Hands.SecondMotion);
        Assert.Equal(9, state.Options.Hands.GraceSeconds);
    }

    [Fact]
    public void Normalize_PreservesCurrentOptions()
    {
        PersistedAppState state = new()
        {
            SchemaVersion = PersistedAppState.CurrentSchemaVersion,
            Options = new WarpClockOptions
            {
                Hands = new HandOptions
                {
                    HourMotion = ClockHandMotion.Sweep,
                },
                Display = new DisplayOptions
                {
                    TickerEnabled = true,
                    CustomTickerMessage = "Status",
                },
            },
        };

        state.Normalize();

        Assert.Equal(ClockHandMotion.Sweep, state.Options.Hands.HourMotion);
        Assert.True(state.Options.Display.TickerEnabled);
        Assert.Equal("Status", state.Options.Display.CustomTickerMessage);
    }

    [Fact]
    public void HandTargetOverridesRoundTrip()
    {
        PersistedAppState expected = new()
        {
            Clock = new PersistedClockSettings
            {
                SecondTargetMode = ClockHandTargetMode.MagneticNumerals,
                MinuteTargetMode = ClockHandTargetMode.Radial,
                HourTargetMode = ClockHandTargetMode.FreeFloating,
            },
        };

        string json = JsonSerializer.Serialize(expected);
        PersistedAppState actual = JsonSerializer.Deserialize<PersistedAppState>(json)
            ?? throw new InvalidOperationException("Could not deserialize persisted state.");

        Assert.Equal(ClockHandTargetMode.MagneticNumerals, actual.Clock.SecondTargetMode);
        Assert.Equal(ClockHandTargetMode.Radial, actual.Clock.MinuteTargetMode);
        Assert.Equal(ClockHandTargetMode.FreeFloating, actual.Clock.HourTargetMode);
    }
}
