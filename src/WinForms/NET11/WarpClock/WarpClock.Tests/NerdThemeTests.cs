using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.Tests;

public sealed class NerdThemeTests
{
    [Fact]
    public void NerdPaletteUsesDistinctHourMinuteAndSecondLedBanks()
    {
        NerdThemePalette day = NerdTheme.CreatePalette(ClockThemeVariantKind.Day);
        NerdThemePalette night = NerdTheme.CreatePalette(ClockThemeVariantKind.Night);

        Assert.NotEqual(day.HourOn.ToArgb(), day.MinuteOn.ToArgb());
        Assert.NotEqual(day.MinuteOn.ToArgb(), day.SecondOn.ToArgb());
        Assert.NotEqual(night.HourOn.ToArgb(), night.MinuteOn.ToArgb());
        Assert.NotEqual(night.MinuteOn.ToArgb(), night.SecondOn.ToArgb());
        Assert.NotEqual(day.HourOn.ToArgb(), day.HourOff.ToArgb());
        Assert.NotEqual(day.MinuteOn.ToArgb(), day.MinuteOff.ToArgb());
        Assert.NotEqual(day.SecondOn.ToArgb(), day.SecondOff.ToArgb());
    }

    [Fact]
    public void NerdPublishesOneBinaryHandAndFourIndependentSleds()
    {
        IReadOnlyList<ClockElementDescriptor> elements = new NerdTheme().CreateElements();
        ClockElementDescriptor secondHand = Assert.Single(
            elements,
            element => element.Id == ClockElementId.SecondHand);
        ClockElementDescriptor[] slides = elements
            .Where(element => element.Id.Kind == ClockElementKind.Custom)
            .ToArray();

        Assert.Equal(7, elements.Count);
        Assert.Equal(4, slides.Length);
        Assert.DoesNotContain(elements, element => element.Id.Kind == ClockElementKind.HourMarker);
        Assert.Equal(ClockHandKind.Second, secondHand.Hand);
        Assert.Equal(NerdThemeGeometry.SecondHandContentSize, secondHand.ContentSize);
        Assert.Equal(NerdThemeGeometry.SecondHandPivot, secondHand.Pivot);
        Assert.All(slides, slide =>
        {
            Assert.Equal(ClockHandKind.None, slide.Hand);
            Assert.True(slide.RedrawPerFrame);
            Assert.Equal(NerdThemeGeometry.SledContentSize, slide.ContentSize);
            Assert.Equal(NerdThemeGeometry.SledPivot, slide.Pivot);
        });
    }

    [Fact]
    public void NerdAnimatorSpawnsAtMostFourSlidesThenReturnsToSolo()
    {
        NerdTheme theme = new()
        {
            AddSlideEveryMin = 1,
            SoloRecoveryMin = 1,
            MaximumSlides = 4,
        };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());

        animator.Initialize(context);
        Assert.Equal(1, ActiveSlides(context));

        Advance(animator, context, TimeSpan.FromMinutes(3.1), TimeSpan.FromSeconds(1));
        Assert.Equal(4, ActiveSlides(context));

        Advance(animator, context, TimeSpan.FromSeconds(54), TimeSpan.FromSeconds(1));
        Advance(animator, context, TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(0.2));
        float beamOutOpacity = context.GetParameters(ClockElementId.CustomElement(1)).Opacity;
        Assert.InRange(beamOutOpacity, 0.01f, 0.99f);

        Advance(animator, context, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.2));
        Assert.Equal(1, ActiveSlides(context));

        Advance(animator, context, TimeSpan.FromSeconds(50), TimeSpan.FromSeconds(1));
        Assert.Equal(1, ActiveSlides(context));
    }

    [Fact]
    public void NerdSledAlwaysGlidesWhileSecondHandUsesConfiguredMotion()
    {
        NerdTheme theme = new() { SecondHandMotion = ClockHandMotion.Tick };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);
        Advance(animator, context, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5));

        Assert.Equal(3f, context.GetParameters(ClockElementId.CustomElement(0)).ExtraRotationDegrees, 3);
        Assert.Equal(ClockHandMotion.Tick, context.GetParameters(ClockElementId.SecondHand).HandMotion);
    }

    [Fact]
    public void NerdCompanionSledUsesAnIndependentSpeed()
    {
        NerdTheme theme = new()
        {
            AddSlideEveryMin = 1,
            SpeedUpAfterMin = 5,
        };
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);
        Advance(animator, context, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1));

        float primaryBefore = context.GetParameters(ClockElementId.CustomElement(0)).ExtraRotationDegrees;
        float companionBefore = context.GetParameters(ClockElementId.CustomElement(1)).ExtraRotationDegrees;
        Advance(animator, context, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        float primaryDelta = AngularDelta(
            primaryBefore,
            context.GetParameters(ClockElementId.CustomElement(0)).ExtraRotationDegrees);
        float companionDelta = AngularDelta(
            companionBefore,
            context.GetParameters(ClockElementId.CustomElement(1)).ExtraRotationDegrees);

        Assert.NotEqual(primaryDelta, companionDelta, precision: 2);
    }

    [Fact]
    public void NerdBackgroundAnimationAdvancesContinuously()
    {
        NerdTheme theme = new();
        IThemeAnimator animator = theme.CreateAnimator();
        TestTickContext context = new(theme.CreateElements());
        animator.Initialize(context);

        Advance(animator, context, TimeSpan.FromSeconds(2.5), TimeSpan.FromSeconds(0.5));

        Assert.Equal(2.5f, context.GetParameters(ClockElementId.Face).Progress, 3);
    }

    [Fact]
    public void NerdPropertiesClampAndFlowToSiblingVariant()
    {
        NerdTheme theme = new()
        {
            SpeedUpAfterMin = 0,
            FastDurationMin = 0,
            AddSlideEveryMin = 0,
            SoloRecoveryMin = 0,
            MaximumSlides = 9,
            MinimumFastMultiplier = 1f,
            MaximumFastMultiplier = 9f,
            SecondHandMotion = ClockHandMotion.Sweep,
        };

        NerdTheme night = Assert.IsType<NerdTheme>(theme.ResolveVariant(ClockThemeVariantKind.Night));

        Assert.Equal(1, night.SpeedUpAfterMin);
        Assert.Equal(1, night.FastDurationMin);
        Assert.Equal(1, night.AddSlideEveryMin);
        Assert.Equal(1, night.SoloRecoveryMin);
        Assert.Equal(4, night.MaximumSlides);
        Assert.Equal(1.5f, night.MinimumFastMultiplier);
        Assert.Equal(5f, night.MaximumFastMultiplier);
        Assert.Equal(ClockHandMotion.Sweep, night.SecondHandMotion);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(14, true)]
    [InlineData(15, false)]
    [InlineData(44, false)]
    [InlineData(45, true)]
    [InlineData(59, true)]
    public void SecondsBitOrderFlipsAtThreeAndNine(int second, bool expectedLsbFirst)
        => Assert.Equal(
            expectedLsbFirst,
            NerdBinaryLayout.SecondsUseLeastSignificantBitFirst(second));

    [Fact]
    public void BinarySlotsSupportAllThreeBanks()
    {
        Assert.Equal(
            [false, true, false, false, false, false],
            Slots(value: 2, bitCount: 6, lsbFirst: true));
        Assert.Equal(
            [false, false, false, false, false, true],
            Slots(value: 32, bitCount: 6, lsbFirst: true));
        Assert.Equal(
            [true, true, true, false, true],
            Slots(value: 23, bitCount: 5, lsbFirst: true));
        Assert.Equal(
            [true, true, false, true, true, true],
            Slots(value: 59, bitCount: 6, lsbFirst: true));
        Assert.Equal(
            [true, false, false, false, false, false],
            Slots(value: 32, bitCount: 6, lsbFirst: false));
    }

    [Fact]
    public void NerdGeometryKeepsLedsSeparatedAndClearOfArbour()
    {
        float minimumCenterDistance =
            NerdThemeGeometry.ArbourRadius
            + NerdThemeGeometry.ArbourClearance
            + NerdThemeGeometry.LedRadius;
        float outermostHourCenter =
            NerdThemeGeometry.HourBankInnerRadius
            + ((NerdThemeGeometry.HourBitCount - 1) * NerdThemeGeometry.BladeLedPitch);
        float outermostMinuteCenter =
            NerdThemeGeometry.MinuteBankInnerRadius
            + ((NerdThemeGeometry.MinuteBitCount - 1) * NerdThemeGeometry.BladeLedPitch);
        float sledPitch = 2f
            * NerdThemeGeometry.SledRadius
            * MathF.Sin(
                (NerdThemeGeometry.SledLedHalfSpanDegrees
                    * 2f
                    / (NerdThemeGeometry.SecondBitCount - 1))
                * (MathF.PI / 360f));

        Assert.True(NerdThemeGeometry.HourBankInnerRadius >= minimumCenterDistance);
        Assert.True(NerdThemeGeometry.BladeLedPitch >= NerdThemeGeometry.LedRadius * 2f);
        Assert.True(
            NerdThemeGeometry.MinuteBankInnerRadius - outermostHourCenter
            >= NerdThemeGeometry.LedRadius * 2f);
        Assert.True(
            outermostMinuteCenter + NerdThemeGeometry.LedRadius
            <= NerdThemeGeometry.BladeTopRadius);
        Assert.True(sledPitch >= NerdThemeGeometry.LedRadius * 2f);
        Assert.True(NerdThemeGeometry.SledHalfSpanDegrees * 2f < 30f);
    }

    private static bool[] Slots(int value, int bitCount, bool lsbFirst)
        => Enumerable.Range(0, bitCount)
            .Select(slot => NerdBinaryLayout.IsBitOn(value, slot, bitCount, lsbFirst))
            .ToArray();

    private static float AngularDelta(float before, float after)
    {
        float delta = (after - before) % 360f;
        return delta < 0f ? delta + 360f : delta;
    }

    private static int ActiveSlides(TestTickContext context)
        => Enumerable.Range(0, 4)
            .Count(index => context.GetParameters(ClockElementId.CustomElement(index)).Visible);

    private static void Advance(
        IThemeAnimator animator,
        TestTickContext context,
        TimeSpan duration,
        TimeSpan step)
    {
        int count = (int)Math.Ceiling(duration.TotalSeconds / step.TotalSeconds);
        for (int i = 0; i < count; i++)
        {
            context.FrameDelta = step;
            DateTime now = context.Time.Now + step;
            float second = now.Second + (now.Millisecond / 1000f);
            context.Time = context.Time with
            {
                Now = now,
                SecondAngle = second * 6f,
            };
            animator.OnTick(context);
        }
    }

    private sealed class TestTickContext(IReadOnlyList<ClockElementDescriptor> elements) : IClockTickContext
    {
        private readonly Dictionary<ClockElementId, ClockElementParameters> _parameters = [];

        public ClockTimeSnapshot Time { get; set; } = new()
        {
            Now = new DateTime(2026, 8, 25, 12, 0, 0, DateTimeKind.Unspecified),
            HourAngle = 0f,
            MinuteAngle = 0f,
            SecondAngle = 0f,
            SubSecondAngle = 0f,
        };

        public ClockTimeZoneSnapshot TimeZone { get; set; }

        public ClockAmbientSnapshot Ambient { get; set; } = ClockAmbientSnapshot.Empty;

        public TimeSpan FrameDelta { get; set; }

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
