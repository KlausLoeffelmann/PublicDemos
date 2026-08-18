using WarpClock.Abstractions;
using WarpClock.Themes.Builtin;

namespace WarpClock.App;

public partial class FormMain
{
    private const string StockThemeSource = "stock";

    private static readonly Func<IClockTheme>[] s_stockThemeFactories =
    [
        BuiltInThemes.RailwayClassic,
        BuiltInThemes.ModernMinimal,
        BuiltInThemes.AntiqueWorn,
        BuiltInThemes.Nerd,
        BuiltInThemes.Scatter,
        BuiltInThemes.Logical,
    ];

    // ── Theme catalog (dynamic: stock families + discovered plug-ins) ──

    private sealed class ThemeEntry
    {
        public ThemeEntry(IClockTheme familyTheme, ThemeCatalogInfo catalog)
        {
            ArgumentNullException.ThrowIfNull(familyTheme);
            ArgumentNullException.ThrowIfNull(catalog);

            FamilyTheme = familyTheme;
            Catalog = catalog;
        }

        public IClockTheme FamilyTheme { get; private set; }

        public ThemeCatalogInfo Catalog { get; private set; }

        public void Update(IClockTheme familyTheme, ThemeCatalogInfo catalog)
        {
            ArgumentNullException.ThrowIfNull(familyTheme);
            ArgumentNullException.ThrowIfNull(catalog);

            FamilyTheme = familyTheme;
            Catalog = catalog;
        }

        public IClockTheme ResolveTheme(ClockThemeVariantKind? requestedVariant, ThemeSchedulePeriod period, bool preferOledVariants)
            => FamilyTheme.ResolveVariant(Catalog.ResolveVariant(requestedVariant, period, preferOledVariants));
    }

    private sealed class ThemeSelection
    {
        public ThemeSelection(ThemeEntry entry, ClockThemeVariantKind? explicitVariant)
        {
            ArgumentNullException.ThrowIfNull(entry);

            Entry = entry;
            ExplicitVariant = explicitVariant;
        }

        public ThemeEntry Entry { get; }

        public ClockThemeVariantKind? ExplicitVariant { get; }

        public ThemeReference ToReference()
            => new()
            {
                ThemeKey = Entry.Catalog.ThemeKey,
                Variant = ExplicitVariant,
            };
    }

    private sealed class ThemeMenuBinding
    {
        public ThemeMenuBinding(ThemeSelection selection, ToolStripMenuItem item)
        {
            ArgumentNullException.ThrowIfNull(selection);
            ArgumentNullException.ThrowIfNull(item);

            Selection = selection;
            Item = item;
        }

        public ThemeSelection Selection { get; }

        public ToolStripMenuItem Item { get; }
    }
}
