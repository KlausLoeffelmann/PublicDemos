namespace WarpClock.App.Tests;

public sealed class ThemeScheduleDocumentTests
{
    [Fact]
    public void Normalize_PreservesExplicitMidnightAndClearsRotationWhenAutoRotateIsDisabled()
    {
        ThemeScheduleDocument document = new()
        {
            AutoRotate = false,
            DayStartsAt = TimeOnly.MinValue,
            NightStartsAt = new TimeOnly(12, 0),
            RotationMinutes = 30,
        };

        document.Normalize();

        Assert.Equal(TimeOnly.MinValue, document.DayStartsAt);
        Assert.Equal(new TimeOnly(12, 0), document.NightStartsAt);
        Assert.Null(document.RotationMinutes);
    }

    [Fact]
    public void Normalize_AppliesDefaultsWhenBoundariesAreMissing()
    {
        ThemeScheduleDocument document = new()
        {
            DayStartsAt = null,
            NightStartsAt = null,
            RotationMinutes = null,
        };

        document.Normalize();

        Assert.Equal(ThemeScheduleDocument.DefaultDayStartsAt, document.DayStartsAt);
        Assert.Equal(ThemeScheduleDocument.DefaultNightStartsAt, document.NightStartsAt);
        Assert.Equal(ThemeScheduleDocument.DefaultRotationMinutes, document.RotationMinutes);
    }
}
