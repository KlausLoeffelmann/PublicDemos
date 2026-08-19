using WarpClock.Abstractions;

namespace WarpClock.Engine.Tests;

public sealed class ClockTimeModelTests
{
    [Fact]
    public void CreateSnapshotAtUtc_UsesDisplayedTimeZoneWithDaylightSavingRules()
    {
        var model = new ClockTimeModel
        {
            DisplayedTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"),
        };

        ClockTimeSnapshot snapshot = model.CreateSnapshotAtUtc(
            new DateTime(2024, 03, 10, 07, 30, 00, DateTimeKind.Utc),
            0d);
        ClockTimeZoneSnapshot timeZone = model.CreateTimeZoneSnapshot(snapshot.Now);

        Assert.Equal(new DateTime(2024, 03, 10, 03, 30, 00), snapshot.Now);
        Assert.Equal(TimeSpan.FromHours(-4), timeZone.UtcOffset);
        Assert.True(timeZone.IsDaylightSavingTime);
    }

    [Fact]
    public void CreateSnapshotAtUtc_SupportsNonIntegralUtcOffsets()
    {
        var model = new ClockTimeModel
        {
            DisplayedTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Nepal Standard Time"),
        };

        ClockTimeSnapshot snapshot = model.CreateSnapshotAtUtc(
            new DateTime(2024, 01, 15, 06, 00, 00, DateTimeKind.Utc),
            0d);
        ClockTimeZoneSnapshot timeZone = model.CreateTimeZoneSnapshot(snapshot.Now);

        Assert.Equal(new DateTime(2024, 01, 15, 11, 45, 00), snapshot.Now);
        Assert.Equal(TimeSpan.FromMinutes(345), timeZone.UtcOffset);
        Assert.False(timeZone.IsDaylightSavingTime);
    }

    [Fact]
    public void CreateSnapshotAtUtc_RetainsDemoOffsetAndSpeedMultiplier()
    {
        var model = new ClockTimeModel
        {
            DisplayedTimeZone = TimeZoneInfo.Utc,
            TimeOffset = TimeSpan.FromHours(1),
            SpeedMultiplier = 3d,
        };

        ClockTimeSnapshot first = model.CreateSnapshotAtUtc(
            new DateTime(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc),
            0d);
        ClockTimeSnapshot second = model.CreateSnapshotAtUtc(
            new DateTime(2024, 01, 01, 00, 00, 10, DateTimeKind.Utc),
            10d);

        Assert.Equal(new DateTime(2024, 01, 01, 01, 00, 00, DateTimeKind.Utc), first.Now);
        Assert.Equal(new DateTime(2024, 01, 01, 01, 00, 30, DateTimeKind.Utc), second.Now);
    }
}
