namespace SplitFlap.WinForms;

/// <summary>
///  Finds installed fixed-pitch font families. GDI+ doesn't expose the pitch flag, so we do what
///  a human would: measure an 'i' and a 'W' and see whether they agree.
/// </summary>
public static class MonospaceFonts
{
    private static string[]? s_cache;

    /// <summary>The family used when the requested one isn't installed.</summary>
    public const string FallbackFamilyName = "Consolas";

    /// <summary>
    ///  Returns the names of all installed fixed-pitch families, sorted. Cached after the first call.
    /// </summary>
    public static IReadOnlyList<string> GetInstalledFamilyNames()
        => s_cache ??= Enumerate();

    /// <summary>
    ///  Returns <paramref name="familyName"/> if it is installed, otherwise the best available fallback.
    /// </summary>
    public static string ResolveFamilyName(string? familyName)
    {
        if (!string.IsNullOrWhiteSpace(familyName) && IsInstalled(familyName))
        {
            return familyName;
        }

        if (IsInstalled(FallbackFamilyName))
        {
            return FallbackFamilyName;
        }

        IReadOnlyList<string> names = GetInstalledFamilyNames();

        return names.Count > 0 ? names[0] : FontFamily.GenericMonospace.Name;
    }

    /// <summary>
    ///  Forces re-enumeration, e.g. after a font was installed.
    /// </summary>
    public static void Refresh()
        => s_cache = null;

    private static bool IsInstalled(string familyName)
    {
        try
        {
            using FontFamily family = new(familyName);

            return family.IsStyleAvailable(FontStyle.Regular);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private static string[] Enumerate()
    {
        List<string> names = [];

        using Bitmap probe = new(1, 1);
        using Graphics g = Graphics.FromImage(probe);
        using InstalledFontCollection installed = new();

        foreach (FontFamily family in installed.Families)
        {
            try
            {
                if (!family.IsStyleAvailable(FontStyle.Regular))
                {
                    continue;
                }

                using Font font = new(family, 12f, FontStyle.Regular, GraphicsUnit.Point);
                float narrow = g.MeasureString("i", font, PointF.Empty, StringFormat.GenericTypographic).Width;
                float wide = g.MeasureString("W", font, PointF.Empty, StringFormat.GenericTypographic).Width;

                if (narrow > 0 && Math.Abs(narrow - wide) < 0.5f)
                {
                    names.Add(family.Name);
                }
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                // Some symbol or damaged fonts refuse to be instantiated. Not our problem.
            }
        }

        names.Sort(StringComparer.CurrentCultureIgnoreCase);

        return [.. names];
    }
}

/// <summary>
///  Offers the installed monospace families as a drop-down in the Properties window while
///  still allowing free text.
/// </summary>
public sealed class MonospaceFontNameConverter : StringConverter
{
    /// <inheritdoc/>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        => true;

    /// <inheritdoc/>
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        => false;

    /// <inheritdoc/>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        => new(MonospaceFonts.GetInstalledFamilyNames().ToArray());
}
