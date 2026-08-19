using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Tests;

public sealed class ClockContextCompatibilityTests
{
    [Fact]
    public void LegacyRenderContext_GainsDefaultAmbientAndTimeZoneSnapshots()
    {
        IClockRenderContext context = new LegacyRenderContext(CreateTimeSnapshot(new DateTime(2024, 08, 19, 12, 34, 56, 789)));

        Assert.Equal(TimeZoneInfo.Local.Id, context.TimeZone.Id);
        Assert.Empty(context.Ambient.IndexedImages);
        Assert.Null(context.Ambient.OverlayMessage);
        Assert.Null(context.Ambient.TickerText);
        Assert.Null(context.Ambient.TimeZoneAlias);
        Assert.Null(context.Ambient.TimeZoneDesignation);
        Assert.Equal(ClockAmbientPresentationState.Default, context.Ambient.PresentationState);
    }

    [Fact]
    public void LegacyTickContext_GainsDefaultAmbientAndTimeZoneSnapshots()
    {
        IClockTickContext context = new LegacyTickContext(CreateTimeSnapshot(new DateTime(2024, 08, 19, 12, 34, 56, 789)));

        Assert.Equal(TimeZoneInfo.Local.Id, context.TimeZone.Id);
        Assert.Empty(context.Ambient.IndexedImages);
        Assert.Null(context.Ambient.OverlayMessage);
        Assert.Null(context.Ambient.TickerText);
        Assert.Null(context.Ambient.TimeZoneAlias);
        Assert.Null(context.Ambient.TimeZoneDesignation);
        Assert.Equal(ClockAmbientPresentationState.Default, context.Ambient.PresentationState);
    }

    [Fact]
    public void AuxiliaryVisibility_DefaultsIncludeFractionSecondGate()
    {
        ClockAuxiliaryVisibility visibility = ClockAuxiliaryVisibility.Default;

        Assert.True(visibility.ShowFractionSecond);
    }

    [Fact]
    public void LegacyAnimator_CanIgnoreTimeZoneChangeCallback()
    {
        IThemeAnimator animator = new LegacyAnimator();
        IClockTickContext context = new LegacyTickContext(CreateTimeSnapshot(new DateTime(2024, 08, 19, 12, 34, 56, 789)));
        ClockTimeZoneSnapshot previous = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Utc, context.Time.Now);
        ClockTimeZoneSnapshot current = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Local, context.Time.Now);

        animator.OnTimeZoneChanged(context, previous, current);
        animator.OnTick(context);
    }

    [Fact]
    public void AuxiliaryElementIdsExposeExplicitKinds()
    {
        Assert.Equal(ClockElementKind.TimeZone, ClockElementId.TimeZone.Kind);
        Assert.Equal(ClockElementKind.Day, ClockElementId.Day.Kind);
        Assert.Equal(ClockElementKind.Weekday, ClockElementId.Weekday.Kind);
        Assert.Equal(ClockElementKind.OverlayMessage, ClockElementId.OverlayMessage.Kind);
        Assert.Equal(ClockElementKind.FractionSecondDial, ClockElementId.FractionSecondDial.Kind);
        Assert.Equal("IndexedImage[3]", ClockElementId.IndexedImage(3).ToString());
    }

    private static ClockTimeSnapshot CreateTimeSnapshot(DateTime now)
        => new()
        {
            Now = now,
            HourAngle = 0f,
            MinuteAngle = 0f,
            SecondAngle = 0f,
            SubSecondAngle = 0f,
        };

    private sealed class LegacyRenderContext(ClockTimeSnapshot time) : IClockRenderContext
    {
        public ClockElementId Id => ClockElementId.Face;

        public SizeF ContentSize => new(100f, 100f);

        public PointF Pivot => new(50f, 50f);

        public ClockElementParameters Parameters { get; } = new();

        public ClockTimeSnapshot Time { get; } = time;

        public float Scale => 1f;
    }

    private sealed class LegacyTickContext(ClockTimeSnapshot time) : IClockTickContext
    {
        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];

        public ClockTimeSnapshot Time { get; } = time;

        public TimeSpan FrameDelta => TimeSpan.Zero;

        public IReadOnlyList<ClockElementDescriptor> Elements => [];

        public float FaceRotationDegrees { get; set; }

        public ClockElementParameters GetParameters(ClockElementId id)
        {
            if (!_parameters.TryGetValue(id, out ClockElementParameters? parameters))
            {
                parameters = new ClockElementParameters();
                _parameters.Add(id, parameters);
            }

            return parameters;
        }
    }

    private sealed class LegacyAnimator : IThemeAnimator
    {
        public void OnTick(IClockTickContext context)
        {
        }
    }
}
