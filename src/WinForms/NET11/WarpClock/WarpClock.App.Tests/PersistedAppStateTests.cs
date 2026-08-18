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
}
