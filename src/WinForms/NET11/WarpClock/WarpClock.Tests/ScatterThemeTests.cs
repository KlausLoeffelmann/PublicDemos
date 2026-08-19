using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.Tests;

public sealed class ScatterThemeTests
{
    [Fact]
    public void ScatterTheme_PublishesOledVariantsAndAuxiliaryElements()
    {
        ScatterTheme theme = new();

        Assert.Equal(ClockThemeVariants.DayNightOled, theme.SupportedVariants);

        IReadOnlyList<ClockElementDescriptor> elements = theme.CreateElements();
        Assert.Contains(elements, element => element.Id == ClockElementId.TimeZone && element.RedrawPerFrame);
        Assert.Contains(elements, element => element.Id == ClockElementId.Day && element.RedrawPerFrame);
        Assert.Contains(elements, element => element.Id == ClockElementId.Weekday && element.RedrawPerFrame);
        Assert.Equal(20, elements.Count);
    }

    [Fact]
    public void ScatterPalette_UsesDarkBlueAndPitchBlackOledFaces()
    {
        ScatterThemePalette day = ScatterTheme.CreatePalette(ClockThemeVariantKind.Day);
        ScatterThemePalette oledDay = ScatterTheme.CreatePalette(ClockThemeVariantKind.OledDay);
        ScatterThemePalette oledNight = ScatterTheme.CreatePalette(ClockThemeVariantKind.OledNight);

        Assert.NotEqual(Color.Black.ToArgb(), day.Face.ToArgb());
        Assert.Equal(Color.FromArgb(12, 32, 86).ToArgb(), oledDay.Face.ToArgb());
        Assert.True(oledDay.Face.B > oledDay.Face.R);
        Assert.Equal(Color.Black.ToArgb(), oledNight.Face.ToArgb());
    }

    [Fact]
    public void ScatterTheme_CustomPropertiesExposeRequestedDisplayNames_AndFlowAcrossVariants()
    {
        ScatterTheme theme = new();
        PropertyInfo[] properties = theme.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<BrowsableAttribute>()?.Browsable == true)
            .ToArray();

        Assert.Equal(
        [
            "Clock-Face Background",
            "Hands",
            "Numeral Background",
            "Numeral Border",
            "Numeral Foreground",
        ],
        properties.Select(property => property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray());

        theme.ClockFaceBackground = Color.MidnightBlue;
        theme.Hands = Color.LightGoldenrodYellow;
        theme.NumeralForeground = Color.HotPink;

        ScatterTheme night = Assert.IsType<ScatterTheme>(theme.ResolveVariant(ClockThemeVariantKind.Night));
        ScatterThemePalette expectedNightDefaults = ScatterTheme.CreatePalette(ClockThemeVariantKind.Night);

        Assert.Equal(Color.MidnightBlue.ToArgb(), night.ClockFaceBackground.ToArgb());
        Assert.Equal(Color.LightGoldenrodYellow.ToArgb(), night.Hands.ToArgb());
        Assert.Equal(Color.HotPink.ToArgb(), night.NumeralForeground.ToArgb());
        Assert.Equal(expectedNightDefaults.MagnetRim.ToArgb(), night.NumeralBorder.ToArgb());
    }

    [Fact]
    public void ScatterRenderer_TimeZoneLabelPrefersAmbientAliasAndDesignation()
    {
        TestRenderContext context = new()
        {
            Ambient = new ClockAmbientSnapshot
            {
                IndexedImages = Array.Empty<ClockIndexedImageSnapshot>(),
                TimeZoneAlias = "Pacific",
                TimeZoneDesignation = "PDT",
            },
            TimeZone = ClockTimeZoneSnapshot.Create(
                TimeZoneInfo.CreateCustomTimeZone("Fallback", TimeSpan.FromHours(-7), "Fallback Name", "Fallback Standard"),
                new DateTime(2026, 8, 19, 10, 0, 0, DateTimeKind.Unspecified)),
        };

        Assert.Equal("Pacific · PDT", ScatterRenderer.ComposeTimeZoneLabel(context));
    }

    [Fact]
    public void ScatterAnimator_TimeZoneShiftMovesNumeralsToFractionalCounterpartPositions()
    {
        ScatterTheme theme = new();
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements())
        {
            Time = CreateTime(2026, 8, 19, 9, 15, 0),
            TimeZone = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Utc, new DateTime(2026, 8, 19, 9, 15, 0, DateTimeKind.Unspecified)),
        };

        animator.Initialize(context);
        Advance(animator, context, seconds: 24f, stepSeconds: 0.2f);

        PointF[] beforeOffsets = CaptureOffsets(context);
        PointF[] beforeWorld = Enumerable.Range(0, 12)
            .Select(index => Add(ScatterTheme.GetHomePosition(index), beforeOffsets[index]))
            .ToArray();

        ClockTimeZoneSnapshot previous = ClockTimeZoneSnapshot.Create(TimeZoneInfo.Utc, context.Time.Now);
        ClockTimeZoneSnapshot current = ClockTimeZoneSnapshot.Create(
            TimeZoneInfo.CreateCustomTimeZone("UTC+05:30", TimeSpan.FromMinutes(330), "UTC+05:30", "UTC+05:30"),
            context.Time.Now.AddHours(5.5));

        animator.OnTimeZoneChanged(context, previous, current);
        Advance(animator, context, seconds: 2.05f, stepSeconds: 0.05f);

        Assert.Equal(ClockHandTargetMode.FreeFloating, context.GetParameters(ClockElementId.HourHand).HandTargetMode);
        Assert.Equal(ClockHandTargetMode.FreeFloating, context.GetParameters(ClockElementId.MinuteHand).HandTargetMode);

        PointF[] afterOffsets = CaptureOffsets(context);
        for (int i = 0; i < afterOffsets.Length; i++)
        {
            PointF expectedWorld = SampleWorldPosition(beforeWorld, i - 5.5f);
            PointF expectedOffset = Subtract(expectedWorld, ScatterTheme.GetHomePosition(i));

            Assert.Equal(expectedOffset.X, afterOffsets[i].X, 2);
            Assert.Equal(expectedOffset.Y, afterOffsets[i].Y, 2);
        }
    }

    private static PointF Add(PointF left, PointF right) => new(left.X + right.X, left.Y + right.Y);

    private static void Advance(IThemeAnimator animator, TestTickContext context, float seconds, float stepSeconds)
    {
        int steps = (int)MathF.Ceiling(seconds / stepSeconds);
        TimeSpan delta = TimeSpan.FromSeconds(stepSeconds);

        for (int i = 0; i < steps; i++)
        {
            context.FrameDelta = delta;
            context.Time = AdvanceTime(context.Time, delta);
            animator.OnTick(context);
        }
    }

    private static ClockTimeSnapshot AdvanceTime(ClockTimeSnapshot snapshot, TimeSpan delta)
    {
        DateTime now = snapshot.Now + delta;
        return CreateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }

    private static PointF[] CaptureOffsets(TestTickContext context)
        => Enumerable.Range(0, 12)
            .Select(index => context.GetParameters(ClockElementId.HourMarker(index)).AnchorOffset)
            .ToArray();

    private static ClockTimeSnapshot CreateTime(int year, int month, int day, int hour, int minute, int second, int millisecond = 0)
    {
        DateTime now = new(year, month, day, hour, minute, second, millisecond, DateTimeKind.Unspecified);
        float totalSeconds = second + (millisecond / 1000f);
        float minuteAngle = ((minute + (totalSeconds / 60f)) % 60f) * 6f;
        float hourAngle = (((hour % 12) + (minute / 60f) + (totalSeconds / 3600f)) % 12f) * 30f;
        float secondAngle = (totalSeconds % 60f) * 6f;
        float subSecondAngle = (millisecond / 1000f) * 360f;

        return new ClockTimeSnapshot
        {
            Now = now,
            HourAngle = hourAngle,
            MinuteAngle = minuteAngle,
            SecondAngle = secondAngle,
            SubSecondAngle = subSecondAngle,
        };
    }

    private static PointF Lerp(PointF a, PointF b, float t)
        => new(
            a.X + ((b.X - a.X) * t),
            a.Y + ((b.Y - a.Y) * t));

    private static PointF SampleWorldPosition(PointF[] positions, float fractionalIndex)
    {
        float wrapped = fractionalIndex % positions.Length;
        if (wrapped < 0f)
        {
            wrapped += positions.Length;
        }

        int lower = (int)MathF.Floor(wrapped) % positions.Length;
        int upper = (lower + 1) % positions.Length;
        float t = wrapped - MathF.Floor(wrapped);
        return Lerp(positions[lower], positions[upper], t);
    }

    private static PointF Subtract(PointF left, PointF right) => new(left.X - right.X, left.Y - right.Y);

    private sealed class TestRenderContext : IClockRenderContext
    {
        public ClockElementId Id { get; init; } = ClockElementId.TimeZone;

        public SizeF ContentSize { get; init; } = new(250f, 78f);

        public PointF Pivot { get; init; } = new(125f, 39f);

        public ClockElementParameters Parameters { get; } = new();

        public ClockTimeSnapshot Time { get; set; } = CreateTime(2026, 8, 19, 10, 0, 0);

        public ClockTimeZoneSnapshot TimeZone { get; set; }

        public ClockAmbientSnapshot Ambient { get; set; } = ClockAmbientSnapshot.Empty;

        public float Scale { get; init; } = 1f;
    }

    private sealed class TestTickContext(IReadOnlyList<ClockElementDescriptor> elements) : IClockTickContext
    {
        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];

        public ClockTimeSnapshot Time { get; set; } = CreateTime(2026, 8, 19, 9, 15, 0);

        public ClockTimeZoneSnapshot TimeZone { get; set; }

        public ClockAmbientSnapshot Ambient { get; set; } = new()
        {
            IndexedImages = Array.Empty<ClockIndexedImageSnapshot>(),
        };

        public TimeSpan FrameDelta { get; set; } = TimeSpan.Zero;

        public IReadOnlyList<ClockElementDescriptor> Elements { get; } = elements;

        public SizeF SurfaceSize { get; set; } = new(1000f, 1000f);

        public float FaceRotationDegrees { get; set; }

        public ClockElementParameters GetParameters(ClockElementId id)
        {
            if (!_parameters.TryGetValue(id, out ClockElementParameters? parameters))
            {
                parameters = new ClockElementParameters();
                _parameters[id] = parameters;
            }

            return parameters;
        }
    }
}
