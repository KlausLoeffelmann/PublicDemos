using System.ComponentModel;
using System.Drawing;
using Microsoft.Extensions.Logging;
using WarpClock.Abstractions;
using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.App.Tests;

public sealed class ThemeCustomPropertiesTests
{
    [Fact]
    public void CustomProperties_RoundTripAcrossVariantsUsingInvariantStrings()
    {
        ThemeCustomPropertyStore store = new();
        store.Load(new PersistedThemeState());

        VariantTheme dayTheme = new(ClockThemeVariantKind.Day);
        ThemeCustomPropertySession daySession = ThemeCustomPropertySession.Create(
            "variant-theme",
            dayTheme,
            store,
            NullTestLogger.Instance);

        dayTheme.Intensity = 7;
        dayTheme.Mode = VariantMode.NightOnly;
        dayTheme.AccentColor = Color.MediumPurple;

        store.CaptureValue(daySession, nameof(VariantTheme.Intensity), NullTestLogger.Instance);
        store.CaptureValue(daySession, nameof(VariantTheme.Mode), NullTestLogger.Instance);
        store.CaptureValue(daySession, nameof(VariantTheme.AccentColor), NullTestLogger.Instance);

        VariantTheme nightTheme = new(ClockThemeVariantKind.Night);
        ThemeCustomPropertySession nightSession = ThemeCustomPropertySession.Create(
            "variant-theme",
            nightTheme,
            store,
            NullTestLogger.Instance);

        Assert.Equal(7, nightTheme.Intensity);
        Assert.Equal(VariantMode.NightOnly, nightTheme.Mode);
        Assert.Equal(Color.MediumPurple.ToArgb(), nightTheme.AccentColor.ToArgb());
        Assert.Equal(
            ["AccentColor", "Intensity", "Mode"],
            nightSession.Properties.Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void Discovery_OmitsInvalidAndReadOnlyProperties_AndLogsWarnings()
    {
        ListLogger logger = new();
        InvalidPropertyTheme theme = new();

        ThemeCustomPropertySession session = ThemeCustomPropertySession.Create(
            "invalid-theme",
            theme,
            new ThemeCustomPropertyStore(),
            logger);

        ThemeCustomPropertyDefinition property = Assert.Single(session.Properties);
        Assert.Equal(nameof(InvalidPropertyTheme.ValidProperty), property.Name);
        Assert.Contains(logger.Messages, message => message.Contains(nameof(InvalidPropertyTheme.ReadOnlyProperty), StringComparison.Ordinal));
        Assert.Contains(logger.Messages, message => message.Contains(nameof(InvalidPropertyTheme.MissingDescriptionProperty), StringComparison.Ordinal));
        Assert.Contains(logger.Messages, message => message.Contains(nameof(InvalidPropertyTheme.WrongCategoryProperty), StringComparison.Ordinal));
        Assert.Contains(logger.Messages, message => message.Contains(nameof(InvalidPropertyTheme.UnsupportedProperty), StringComparison.Ordinal));
    }

    [Fact]
    public void PersistedValues_OmitInvalidAndMissingProperties_AndPreserveRemovedPluginEntries()
    {
        PersistedThemeState persisted = new()
        {
            CustomPropertyValues =
            [
                new PersistedThemeCustomPropertyValue
                {
                    ThemeKey = "removed-plugin",
                    PropertyName = "AccentColor",
                    Value = "Red",
                },
                new PersistedThemeCustomPropertyValue
                {
                    ThemeKey = "variant-theme",
                    PropertyName = "MissingProperty",
                    Value = "123",
                },
                new PersistedThemeCustomPropertyValue
                {
                    ThemeKey = "variant-theme",
                    PropertyName = nameof(VariantTheme.Intensity),
                    Value = "not-an-int",
                },
            ],
        };
        persisted.Normalize();

        ThemeCustomPropertyStore store = new();
        store.Load(persisted);

        ListLogger logger = new();
        VariantTheme theme = new(ClockThemeVariantKind.Day);
        ThemeCustomPropertySession.Create("variant-theme", theme, store, logger);

        PersistedThemeCustomPropertyValue removedPluginEntry = Assert.Single(
            store.ExportValues(),
            value => string.Equals(value.ThemeKey, "removed-plugin", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("AccentColor", removedPluginEntry.PropertyName);
        Assert.DoesNotContain(
            store.ExportValues(),
            value => string.Equals(value.ThemeKey, "variant-theme", StringComparison.OrdinalIgnoreCase)
                && string.Equals(value.PropertyName, "MissingProperty", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            store.ExportValues(),
            value => string.Equals(value.ThemeKey, "variant-theme", StringComparison.OrdinalIgnoreCase)
                && string.Equals(value.PropertyName, nameof(VariantTheme.Intensity), StringComparison.OrdinalIgnoreCase));
        Assert.Contains(logger.Messages, message => message.Contains("MissingProperty", StringComparison.Ordinal));
        Assert.Contains(logger.Messages, message => message.Contains(nameof(VariantTheme.Intensity), StringComparison.Ordinal));
    }

    [Fact]
    public void PropertyGridAdapter_MergesStaticAndThemePropertiesWithoutExposingTheThemeControl()
    {
        ThemePropertyGridAdapter adapter = new(new BaseSettings());
        ThemeCustomPropertySession session = ThemeCustomPropertySession.Create(
            "variant-theme",
            new VariantTheme(ClockThemeVariantKind.Day),
            new ThemeCustomPropertyStore(),
            NullTestLogger.Instance);
        adapter.SetThemeSession(session);

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(adapter);

        Assert.NotNull(properties[nameof(BaseSettings.BaseSetting)]);
        Assert.NotNull(properties[nameof(VariantTheme.AccentColor)]);
        Assert.False(typeof(Control).IsAssignableFrom(adapter.GetType()));
        Assert.IsType<BaseSettings>(
            ((ICustomTypeDescriptor)adapter).GetPropertyOwner(properties[nameof(BaseSettings.BaseSetting)]));
        Assert.Same(adapter, ((ICustomTypeDescriptor)adapter).GetPropertyOwner(properties[nameof(VariantTheme.AccentColor)]));
    }

    private sealed class BaseSettings
    {
        [Category("Rendering")]
        public bool BaseSetting { get; set; }
    }

    private enum VariantMode
    {
        Normal,
        NightOnly,
    }

    [ClockThemeExport(Discoverable = false)]
    private sealed class VariantTheme(ClockThemeVariantKind variant) : IClockTheme
    {
        public string Name => ClockThemeVariants.FormatDisplayName("Variant Theme", variant);

        public string Description => "Test theme with custom properties.";

        public string Author => "Tests";

        public ThemeCapabilities Capabilities => ThemeCapabilities.Default;

        public IReadOnlyList<ClockThemeVariantKind> SupportedVariants => ClockThemeVariants.DayNight;

        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("Accent color.")]
        public Color AccentColor { get; set; } = Color.DarkOrange;

        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("Intensity level.")]
        public int Intensity { get; set; } = variant == ClockThemeVariantKind.Day ? 1 : 2;

        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("Variant mode.")]
        public VariantMode Mode { get; set; } = VariantMode.Normal;

        public IClockTheme ResolveVariant(ClockThemeVariantKind requestedVariant) => new VariantTheme(requestedVariant);

        public IReadOnlyList<ClockElementDescriptor> CreateElements() => [];

        public IClockLayout CreateLayout() => new TestLayout();

        public IClockElementRenderer CreateRenderer() => new TestRenderer();

        public IThemeAnimator? CreateAnimator() => null;
    }

    [ClockThemeExport(Discoverable = false)]
    private sealed class InvalidPropertyTheme : IClockTheme
    {
        public string Name => "Invalid Theme";

        public string Description => "Theme with malformed custom properties.";

        public string Author => "Tests";

        public ThemeCapabilities Capabilities => ThemeCapabilities.Default;

        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("Valid property.")]
        public int ValidProperty { get; set; } = 5;

        [Browsable(true)]
        [Category("Custom Properties")]
        public int MissingDescriptionProperty { get; set; }

        [Browsable(true)]
        [Description("Wrong category.")]
        [Category("Other")]
        public int WrongCategoryProperty { get; set; }

        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("Read only.")]
        public int ReadOnlyProperty => 10;

        [Browsable(true)]
        [Category("Custom Properties")]
        [Description("Unsupported property.")]
        public UnsupportedValue UnsupportedProperty { get; set; } = new();

        public IReadOnlyList<ClockElementDescriptor> CreateElements() => [];

        public IClockLayout CreateLayout() => new TestLayout();

        public IClockElementRenderer CreateRenderer() => new TestRenderer();

        public IThemeAnimator? CreateAnimator() => null;
    }

    private sealed class UnsupportedValue;

    private sealed class TestLayout : IClockLayout
    {
        public bool TryGetAnchor(ClockElementId id, SizeF surface, out PointF anchor)
        {
            anchor = default;
            return false;
        }
    }

    private sealed class TestRenderer : IClockElementRenderer
    {
        public void DrawElement(ID2DGraphics graphics, IClockRenderContext context)
        {
        }
    }

    private sealed class NullTestLogger : ILogger
    {
        public static readonly NullTestLogger Instance = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
        }
    }

    private sealed class ListLogger : ILogger
    {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Messages.Add(formatter(state, exception));
    }
}
