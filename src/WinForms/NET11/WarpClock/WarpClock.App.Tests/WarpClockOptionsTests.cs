using WarpClock.App;

namespace WarpClock.App.Tests;

public sealed class WarpClockOptionsTests
{
    [Fact]
    public void TimeZoneOptionsNormalizeCapsEntriesAndKeepsOneDefault()
    {
        var options = new TimeZoneOptions
        {
            Enabled = true,
            ChangeToNextSeconds = 64,
            ReturnToDefaultSeconds = 22,
            Entries = Enumerable.Range(0, 8)
                .Select(index => new ConfiguredTimeZone
                {
                    TimeZoneId = $"Zone-{index}",
                    DisplayName = $"Zone {index}",
                    IsDefault = index is 2 or 3,
                })
                .ToList(),
        };

        options.Normalize();

        Assert.Equal(TimeZoneOptions.MaximumTimeZoneCount, options.Entries.Count);
        Assert.Single(options.Entries, entry => entry.IsDefault);
        Assert.Equal(60, options.ChangeToNextSeconds);
        Assert.Equal(20, options.ReturnToDefaultSeconds);
    }

    [Fact]
    public void TimeZoneRotationReturnsToDefaultBetweenAlternates()
    {
        var options = new TimeZoneOptions
        {
            Enabled = true,
            ChangeToNextSeconds = 10,
            ReturnToDefaultSeconds = 5,
            Entries =
            [
                Zone("Local", isDefault: true),
                Zone("One"),
                Zone("Two"),
            ],
        };
        var controller = new TimeZoneRotationController();
        controller.Reset(options);

        Assert.Equal("Local", controller.Current.TimeZoneId);

        Assert.True(controller.Advance(TimeSpan.FromSeconds(10)));
        Assert.Equal("One", controller.Current.TimeZoneId);

        Assert.True(controller.Advance(TimeSpan.FromSeconds(5)));
        Assert.Equal("Local", controller.Current.TimeZoneId);

        Assert.True(controller.Advance(TimeSpan.FromSeconds(10)));
        Assert.Equal("Two", controller.Current.TimeZoneId);
    }

    [Fact]
    public void TimeZoneRotationProcessesEveryBoundaryAfterDelayedTick()
    {
        var options = new TimeZoneOptions
        {
            Enabled = true,
            ChangeToNextSeconds = 10,
            ReturnToDefaultSeconds = 5,
            Entries =
            [
                Zone("Local", isDefault: true),
                Zone("One"),
                Zone("Two"),
            ],
        };
        var controller = new TimeZoneRotationController();
        controller.Reset(options);

        Assert.True(controller.Advance(TimeSpan.FromSeconds(30)));

        Assert.Equal("Local", controller.Current.TimeZoneId);
        Assert.True(controller.Advance(TimeSpan.FromSeconds(10)));
        Assert.Equal("One", controller.Current.TimeZoneId);
    }

    [Fact]
    public void TickerComposerUsesConfiguredOrderAndDropsDuplicatesAndBlanks()
    {
        string result = TickerContentComposer.Compose(
            [
                TickerContentSource.ThemeName,
                TickerContentSource.CustomMessage,
                TickerContentSource.ThemeName,
                TickerContentSource.TimeZone,
            ],
            customMessage: "  Hello  ",
            displayedTime: new DateTime(2026, 8, 19),
            timeZoneDisplayName: " ",
            themeName: "Logical");

        Assert.Equal("Logical   |   Hello", result);
    }

    [Fact]
    public void DisplayOptionsNormalizePreservesDisabledBuiltInSources()
    {
        var options = new DisplayOptions
        {
            TickerContentOrder =
            [
                TickerContentSource.TimeZone,
                TickerContentSource.CustomMessage,
                TickerContentSource.TimeZone,
            ],
        };

        options.Normalize();

        Assert.Equal(
            [TickerContentSource.TimeZone, TickerContentSource.CustomMessage],
            options.TickerContentOrder);
    }

    [Fact]
    public void TimeZoneEditorRowsArePaddedToMaximumCount()
    {
        var options = new TimeZoneOptions
        {
            Entries =
            [
                Zone("Local", isDefault: true),
                Zone("Tokyo"),
            ],
        };

        IReadOnlyList<TimeZoneEditorRow> rows = OptionsDialogModelMapper.CreateTimeZoneRows(options);

        Assert.Equal(TimeZoneOptions.MaximumTimeZoneCount, rows.Count);
        Assert.Equal("Local", rows[0].TimeZoneId);
        Assert.Equal("Tokyo", rows[1].TimeZoneId);
        Assert.All(rows.Skip(2), row => Assert.True(string.IsNullOrEmpty(row.TimeZoneId)));
    }

    [Fact]
    public void CreatingTimeZoneOptionsRejectsDuplicateRows()
    {
        bool created = OptionsDialogModelMapper.TryCreateTimeZoneOptions(
            enabled: true,
            changeToNextSeconds: 60,
            returnToDefaultSeconds: 20,
            showOnClockFace: true,
            showOnlyWhenAlternate: false,
            showHeadlineFallback: true,
            rows:
            [
                new TimeZoneEditorRow { TimeZoneId = "Local", DisplayName = "Local", IsDefault = true },
                new TimeZoneEditorRow { TimeZoneId = "Local", DisplayName = "Again" },
            ],
            out _,
            out string? validationMessage);

        Assert.False(created);
        Assert.Contains("selected more than once", validationMessage);
    }

    [Fact]
    public void TickerEditorItemsKeepEnabledOrderAndAppendDisabledSources()
    {
        var options = new DisplayOptions
        {
            TickerContentOrder =
            [
                TickerContentSource.ThemeName,
                TickerContentSource.CustomMessage,
            ],
        };

        IReadOnlyList<TickerSourceEditorItem> items = OptionsDialogModelMapper.CreateTickerItems(options);

        Assert.Equal(TickerContentSource.ThemeName, items[0].Source);
        Assert.True(items[0].Enabled);
        Assert.Equal(TickerContentSource.CustomMessage, items[1].Source);
        Assert.True(items[1].Enabled);
        Assert.Contains(items, item => item.Source == TickerContentSource.CurrentDate && !item.Enabled);
        Assert.Contains(items, item => item.Source == TickerContentSource.TimeZone && !item.Enabled);
    }

    private static ConfiguredTimeZone Zone(string id, bool isDefault = false)
        => new()
        {
            TimeZoneId = id,
            DisplayName = id,
            IsDefault = isDefault,
        };
}
