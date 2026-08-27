using System.Drawing;
using System.ComponentModel;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed record NerdThemePalette(
    Color FaceBlue,
    Color FaceRed,
    Color FaceGreen,
    Color Grid,
    Color Blade,
    Color HourOn,
    Color HourOff,
    Color MinuteOn,
    Color MinuteOff,
    Color SecondOn,
    Color SecondOff);

internal static class NerdThemeGeometry
{
    public static readonly SizeF SecondHandContentSize = new(250f, 530f);
    public static readonly PointF SecondHandPivot = new(125f, 490f);
    public static readonly SizeF SledContentSize = new(280f, 540f);
    public static readonly PointF SledPivot = new(140f, 500f);

    public const int HourBitCount = 5;
    public const int MinuteBitCount = 6;
    public const int SecondBitCount = 6;

    public const float ArbourRadius = 30f;
    public const float ArbourClearance = 6f;
    public const float LedRadius = 9f;
    public const float BladeTopRadius = 420f;
    public const float BladeHalfWidth = 30f;
    public const float BladeTailDepth = 24f;
    public const float HourBankInnerRadius = 58f;
    public const float MinuteBankInnerRadius = 218f;
    public const float BladeLedPitch = 27f;
    public const float SledRadius = 455f;
    public const float SledHalfSpanDegrees = 12f;
    public const float SledHalfThickness = 18f;
    public const float SledLedHalfSpanDegrees = 10f;
    public const int SledTrackCount = 4;
    public const float SledTrackSpacing = 48f;
    public const float SledTrackTransitionSeconds = 0.65f;
    public const float SledMaximumTrackTransitionSeconds =
        SledTrackTransitionSeconds * (SledTrackCount - 1);
    public const float SledAngularSafetyGap = 4f;
    public const float SledRadialSafetyGap = 6f;

    public const float SledCollisionAngularSpan =
        (SledHalfSpanDegrees * 2f) + SledAngularSafetyGap;

    public const float SledCollisionRadialSpan =
        (SledHalfThickness * 2f) + SledRadialSafetyGap;

    public static float GetSledTrackRadius(float track)
        => SledRadius
            - (Math.Clamp(track, 0f, SledTrackCount - 1f) * SledTrackSpacing);
}

/// <summary>
///  A binary second hand with independently gliding curved seconds sleds.
/// </summary>
public sealed class NerdTheme : IClockTheme
{
    private const string BaseName = "NERD";
    private const string BaseDescription =
        "Curved binary seconds sled with blue hour LEDs and red minute LEDs on its rotating blade.";

    private readonly ClockThemeVariantKind _variant;
    private readonly NerdThemePalette _palette;
    private int _speedUpAfterMin = 1;
    private int _fastDurationMin = 1;
    private int _addSlideEveryMin = 2;
    private int _soloRecoveryMin = 3;
    private int _maximumSlides = 4;
    private float _minimumFastMultiplier = 1.5f;
    private float _maximumFastMultiplier = 5f;

    public NerdTheme()
        : this(ClockThemeVariantKind.Day)
    {
    }

    internal NerdTheme(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(ClockThemeVariants.DayNight, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant);
        }

        _variant = variant;
        _palette = CreatePalette(variant);
    }

    /// <inheritdoc/>
    public string Name => ClockThemeVariants.FormatDisplayName(BaseName, _variant);

    /// <inheritdoc/>
    public string Description => BaseDescription;

    /// <inheritdoc/>
    public string Author => "stock theme";

    /// <inheritdoc/>
    public ThemeCapabilities Capabilities { get; } = ThemeCapabilities.Default;

    /// <inheritdoc/>
    public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => ClockThemeVariants.DayNight;

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Second Hand Motion")]
    [Description("Movement of the binary hour/minute hand. Sleds always glide independently.")]
    public ClockHandMotion SecondHandMotion { get; set; } = ClockHandMotion.Tick;

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Cheat Mode")]
    [Description("Shows decimal hour, minute, and position-derived sled values every 30 seconds.")]
    public bool CheatMode { get; set; }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Speed Up After (min)")]
    [Description("Minutes at normal speed before each sled enters its faster phase.")]
    public int SpeedUpAfterMin
    {
        get => _speedUpAfterMin;
        set => _speedUpAfterMin = Math.Max(1, value);
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Fast Duration (min)")]
    [Description("Minutes each sled remains in its faster phase before restarting its speed cycle.")]
    public int FastDurationMin
    {
        get => _fastDurationMin;
        set => _fastDurationMin = Math.Max(1, value);
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Add Slide Every (min)")]
    [Description("Minutes between Enterprise-style companion sled appearances.")]
    public int AddSlideEveryMin
    {
        get => _addSlideEveryMin;
        set => _addSlideEveryMin = Math.Max(1, value);
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Solo Recovery (min)")]
    [Description("Minutes with one sled after companions beam out.")]
    public int SoloRecoveryMin
    {
        get => _soloRecoveryMin;
        set => _soloRecoveryMin = Math.Max(1, value);
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Maximum Slides")]
    [Description("Maximum simultaneous sled count, from one through four.")]
    public int MaximumSlides
    {
        get => _maximumSlides;
        set => _maximumSlides = Math.Clamp(value, 1, 4);
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Minimum Fast Speed")]
    [Description("Lowest randomly selected fast-phase multiplier.")]
    public float MinimumFastMultiplier
    {
        get => _minimumFastMultiplier;
        set
        {
            _minimumFastMultiplier = Math.Clamp(value, 1.5f, 5f);
            _maximumFastMultiplier = Math.Max(_maximumFastMultiplier, _minimumFastMultiplier);
        }
    }

    [Browsable(true)]
    [Category("Custom Properties")]
    [DisplayName("Maximum Fast Speed")]
    [Description("Highest randomly selected fast-phase multiplier.")]
    public float MaximumFastMultiplier
    {
        get => _maximumFastMultiplier;
        set
        {
            _maximumFastMultiplier = Math.Clamp(value, 1.5f, 5f);
            _minimumFastMultiplier = Math.Min(_minimumFastMultiplier, _maximumFastMultiplier);
        }
    }

    /// <inheritdoc/>
    public IClockTheme ResolveVariant(ClockThemeVariantKind variant)
    {
        if (!ClockThemeVariants.Supports(SupportedVariants, variant))
        {
            throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, SupportedVariants, variant);
        }

        if (variant == _variant)
        {
            return this;
        }

        return new NerdTheme(variant)
        {
            SecondHandMotion = SecondHandMotion,
            CheatMode = CheatMode,
            SpeedUpAfterMin = SpeedUpAfterMin,
            FastDurationMin = FastDurationMin,
            AddSlideEveryMin = AddSlideEveryMin,
            SoloRecoveryMin = SoloRecoveryMin,
            MaximumSlides = MaximumSlides,
            MinimumFastMultiplier = MinimumFastMultiplier,
            MaximumFastMultiplier = MaximumFastMultiplier,
        };
    }

    /// <inheritdoc/>
    public IReadOnlyList<ClockElementDescriptor> CreateElements()
    {
        return
        [
            new()
            {
                Id = ClockElementId.Face,
                ContentSize = new SizeF(1000, 1000),
                Pivot = new PointF(500, 500),
                ZOrder = 0,
                RedrawPerFrame = true,
            },
            new()
            {
                Id = ClockElementId.SecondHand,
                ContentSize = NerdThemeGeometry.SecondHandContentSize,
                Pivot = NerdThemeGeometry.SecondHandPivot,
                Hand = ClockHandKind.Second,
                ZOrder = 30,
                RedrawPerFrame = true,
            },
            .. Enumerable.Range(0, 4).Select(index => new ClockElementDescriptor
            {
                Id = ClockElementId.CustomElement(index),
                ContentSize = NerdThemeGeometry.SledContentSize,
                Pivot = NerdThemeGeometry.SledPivot,
                ZOrder = 24 + index,
                RedrawPerFrame = true,
            }),
            new()
            {
                Id = ClockElementId.Arbour,
                ContentSize = new SizeF(60, 60),
                Pivot = new PointF(30, 30),
                ZOrder = 40,
            },
        ];
    }

    /// <inheritdoc/>
    public IClockLayout CreateLayout() => new RadialLayout();

    /// <inheritdoc/>
    public IClockElementRenderer CreateRenderer() => new NerdRenderer(_palette);

    /// <inheritdoc/>
    public IThemeAnimator CreateAnimator()
        => new NerdAnimator(
            SecondHandMotion,
            SpeedUpAfterMin,
            FastDurationMin,
            AddSlideEveryMin,
            SoloRecoveryMin,
            MaximumSlides,
            MinimumFastMultiplier,
            MaximumFastMultiplier,
            CheatMode);

    internal static NerdThemePalette CreatePalette(ClockThemeVariantKind variant)
        => variant switch
        {
            ClockThemeVariantKind.Day => new NerdThemePalette(
                FaceBlue: Color.FromArgb(76, 172, 238),
                FaceRed: Color.FromArgb(226, 92, 112),
                FaceGreen: Color.FromArgb(72, 198, 146),
                Grid: Color.FromArgb(25, 39, 61),
                Blade: Color.FromArgb(150, 38, 48, 70),
                HourOn: Color.FromArgb(12, 58, 112),
                HourOff: Color.FromArgb(206, 229, 244),
                MinuteOn: Color.FromArgb(122, 18, 34),
                MinuteOff: Color.FromArgb(241, 210, 210),
                SecondOn: Color.FromArgb(12, 82, 52),
                SecondOff: Color.FromArgb(207, 228, 216)),
            ClockThemeVariantKind.Night => new NerdThemePalette(
                FaceBlue: Color.FromArgb(18, 56, 91),
                FaceRed: Color.FromArgb(82, 28, 43),
                FaceGreen: Color.FromArgb(18, 72, 55),
                Grid: Color.FromArgb(112, 122, 136),
                Blade: Color.FromArgb(108, 48, 56, 68),
                HourOn: Color.FromArgb(102, 176, 216),
                HourOff: Color.FromArgb(36, 55, 68),
                MinuteOn: Color.FromArgb(204, 122, 122),
                MinuteOff: Color.FromArgb(70, 41, 45),
                SecondOn: Color.FromArgb(92, 168, 123),
                SecondOff: Color.FromArgb(35, 62, 48)),
            _ => throw ClockThemeVariants.CreateUnsupportedVariantException(BaseName, ClockThemeVariants.DayNight, variant),
        };
}

internal static class NerdBinaryLayout
{
    public static int SecondAtAngle(float angleDegrees)
    {
        float normalized = angleDegrees % 360f;
        if (normalized < 0f)
        {
            normalized += 360f;
        }

        return Math.Min(59, (int)MathF.Floor(normalized / 6f));
    }

    public static bool SecondsUseLeastSignificantBitFirst(int second)
        => second < 15 || second >= 45;

    public static bool IsBitOn(int value, int slot, int bitCount, bool leastSignificantBitFirst)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(slot);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(slot, bitCount);

        int bit = leastSignificantBitFirst ? slot : bitCount - 1 - slot;
        return (value & (1 << bit)) != 0;
    }
}
