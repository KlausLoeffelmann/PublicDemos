using System.Drawing;

using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Tests;

public sealed class ThemeVariantTests
{
    [Fact]
    public void LegacyThemesRemainDayOnlyByDefault()
    {
        IClockTheme theme = new LegacyTheme();

        Assert.Equal([ClockThemeVariantKind.Day], theme.SupportedVariants);
        Assert.Same(theme, theme.ResolveVariant(ClockThemeVariantKind.Day));

        NotSupportedException ex = Assert.Throws<NotSupportedException>(
            () => theme.ResolveVariant(ClockThemeVariantKind.Night));

        Assert.Contains("Night", ex.Message);
    }

    [Fact]
    public void VariantHelpersProvideConsistentMetadata()
    {
        Assert.Equal(
            [ClockThemeVariantKind.Day, ClockThemeVariantKind.Night, ClockThemeVariantKind.OledDay, ClockThemeVariantKind.OledNight],
            ClockThemeVariants.DayNightOled);
        Assert.True(ClockThemeVariants.IsNight(ClockThemeVariantKind.Night));
        Assert.True(ClockThemeVariants.IsNight(ClockThemeVariantKind.OledNight));
        Assert.False(ClockThemeVariants.IsNight(ClockThemeVariantKind.Day));
        Assert.True(ClockThemeVariants.IsOled(ClockThemeVariantKind.OledDay));
        Assert.False(ClockThemeVariants.IsOled(ClockThemeVariantKind.Night));
        Assert.Equal(ClockThemeVariantKind.OledNight, ClockThemeVariants.Compose(night: true, oled: true));
        Assert.Equal("OLED-Day", ClockThemeVariants.GetLabel(ClockThemeVariantKind.OledDay));
        Assert.Equal("Railway Classic - Night", ClockThemeVariants.FormatDisplayName("Railway Classic", ClockThemeVariantKind.Night));
    }

    [Fact]
    public void StockCatalogPublishesOneDefaultEntryPerThemeFamily()
    {
        IReadOnlyList<IClockTheme> all = BuiltInThemes.All();

        Assert.Equal(
        [
            ClockThemeVariants.FormatDisplayName("Railway Classic", ClockThemeVariantKind.Day),
            ClockThemeVariants.FormatDisplayName("Modern Minimal", ClockThemeVariantKind.Day),
            ClockThemeVariants.FormatDisplayName("Antique Worn", ClockThemeVariantKind.Day),
            ClockThemeVariants.FormatDisplayName("NERD", ClockThemeVariantKind.Day),
            ClockThemeVariants.FormatDisplayName("Scatter (Magnetic)", ClockThemeVariantKind.Day),
            ClockThemeVariants.FormatDisplayName("Logical", ClockThemeVariantKind.Day),
        ],
        all.Select(theme => theme.Name));

        Assert.Equal(
        [
            ClockThemeVariants.DayNight,
            ClockThemeVariants.DayNight,
            ClockThemeVariants.DayNight,
            ClockThemeVariants.DayNight,
            ClockThemeVariants.DayNightOled,
            ClockThemeVariants.DayNightOled,
        ],
        all.Select(theme => theme.SupportedVariants));
    }

    [Fact]
    public void StockThemesResolveSiblingVariantsWithoutChangingBehavioralContracts()
    {
        ThemeExpectation[] expectations =
        [
            new("Railway Classic", BuiltInThemes.RailwayClassic, 77, ThemeCapabilities.Default, ClockThemeVariants.DayNight),
            new("Modern Minimal", BuiltInThemes.ModernMinimal, 69, ThemeCapabilities.Default, ClockThemeVariants.DayNight),
            new("Antique Worn", BuiltInThemes.AntiqueWorn, 77, ThemeCapabilities.Default, ClockThemeVariants.DayNight),
            new("NERD", BuiltInThemes.Nerd, 15, ThemeCapabilities.Default, ClockThemeVariants.DayNight),
            new("Scatter (Magnetic)", BuiltInThemes.Scatter, 20, new ThemeCapabilities
            {
                FreeFloating = true,
                HandsFollowFaceRotation = true,
                MagneticByDefault = true,
            }, ClockThemeVariants.DayNightOled),
            new("Logical", BuiltInThemes.Logical, 21, ThemeCapabilities.Default, ClockThemeVariants.DayNightOled),
        ];

        foreach (ThemeExpectation expectation in expectations)
        {
            IClockTheme defaultVariant = expectation.Factory();

            Assert.Equal(expectation.SupportedVariants, defaultVariant.SupportedVariants);
            Assert.Same(defaultVariant, defaultVariant.ResolveVariant(expectation.SupportedVariants[0]));
            Assert.Equal(
                ClockThemeVariants.FormatDisplayName(expectation.BaseName, expectation.SupportedVariants[0]),
                defaultVariant.Name);
            Assert.Equal(expectation.ElementCount, defaultVariant.CreateElements().Count);
            Assert.Equal(expectation.Capabilities, defaultVariant.Capabilities);

            foreach (ClockThemeVariantKind variant in expectation.SupportedVariants.Skip(1))
            {
                IClockTheme sibling = defaultVariant.ResolveVariant(variant);

                Assert.Equal(
                    ClockThemeVariants.FormatDisplayName(expectation.BaseName, variant),
                    sibling.Name);
                Assert.Equal(expectation.ElementCount, sibling.CreateElements().Count);
                Assert.Equal(expectation.Capabilities, sibling.Capabilities);
            }
        }
    }

    private sealed record ThemeExpectation(
        string BaseName,
        Func<IClockTheme> Factory,
        int ElementCount,
        ThemeCapabilities Capabilities,
        IReadOnlyList<ClockThemeVariantKind> SupportedVariants);

    private sealed class LegacyTheme : IClockTheme
    {
        public string Name => "Legacy";

        public string Description => "Legacy day-only theme.";

        public string Author => "test";

        public ThemeCapabilities Capabilities => ThemeCapabilities.Default;

        public IReadOnlyList<ClockElementDescriptor> CreateElements() => [];

        public IClockLayout CreateLayout() => new LegacyLayout();

        public IClockElementRenderer CreateRenderer() => new LegacyRenderer();

        public IThemeAnimator? CreateAnimator() => null;
    }

    private sealed class LegacyLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            anchor = default;
            return false;
        }
    }

    private sealed class LegacyRenderer : IClockElementRenderer
    {
        public void DrawElement(ID2DGraphics graphics, IClockRenderContext context)
        {
        }
    }
}
