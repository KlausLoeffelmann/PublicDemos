namespace MaterialKeys.Demo;

/// <summary>
///  The functional groups a cash-register key can belong to. Each category is drawn in its own
///  color so the operator can tell number entry, editing, department and totalling keys apart at
///  a glance.
/// </summary>
public enum KeyCategory
{
    /// <summary>Digit entry keys (0–9, 00, decimal point).</summary>
    Number,

    /// <summary>Editing/control keys such as CLEAR, VOID and CORR.</summary>
    Function,

    /// <summary>Product department keys (DEPT 1–4).</summary>
    Department,

    /// <summary>The subtotal/TOTAL key — the visually dominant, default action.</summary>
    Total
}

/// <summary>
///  The three colors that fully describe a <see cref="MaterialKeyButton"/> face: its fill, its
///  caption and its rim.
/// </summary>
/// <remarks>
///  <para>
///   A dedicated struct (rather than three loose <see cref="Color"/> values) keeps a category's
///   colors together and makes the palette tables below read like a small design spec.
///  </para>
/// </remarks>
/// <param name="Back">The key face (fill) color.</param>
/// <param name="Fore">The caption color.</param>
/// <param name="Border">The rim/border color.</param>
public readonly record struct KeyStyle(Color Back, Color Fore, Color Border);

/// <summary>
///  A complete cash-register color set: one <see cref="KeyStyle"/> per <see cref="KeyCategory"/>.
/// </summary>
/// <remarks>
///  <para>
///   Two ready-made sets are provided — <see cref="Classic"/> for light/classic Windows and
///   <see cref="Dark"/> for dark mode — and <see cref="Current"/> picks between them based on
///   <see cref="Application.IsDarkModeEnabled"/>. The colors are chosen explicitly (rather than
///   derived from <see cref="SystemColors"/>) because system color names are remapped in dark
///   mode, which would break a branded, category-based palette.
///  </para>
/// </remarks>
public sealed class CashRegisterPalette
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="CashRegisterPalette"/> class.
    /// </summary>
    /// <param name="number">The style for digit-entry keys.</param>
    /// <param name="function">The style for editing/control keys.</param>
    /// <param name="department">The style for department keys.</param>
    /// <param name="total">The style for the TOTAL key.</param>
    public CashRegisterPalette(KeyStyle number, KeyStyle function, KeyStyle department, KeyStyle total)
    {
        Number = number;
        Function = function;
        Department = department;
        Total = total;
    }

    /// <summary>Gets the style for digit-entry keys.</summary>
    public KeyStyle Number { get; }

    /// <summary>Gets the style for editing/control keys.</summary>
    public KeyStyle Function { get; }

    /// <summary>Gets the style for department keys.</summary>
    public KeyStyle Department { get; }

    /// <summary>Gets the style for the TOTAL key.</summary>
    public KeyStyle Total { get; }

    /// <summary>
    ///  Gets the <see cref="KeyStyle"/> for a given <paramref name="category"/>.
    /// </summary>
    public KeyStyle this[KeyCategory category]
        => category switch
        {
            KeyCategory.Number => Number,
            KeyCategory.Function => Function,
            KeyCategory.Department => Department,
            KeyCategory.Total => Total,
            _ => Number
        };

    // Helper to keep the palette tables terse and self-documenting.
    private static Color Rgb(uint argb) => Color.FromArgb(unchecked((int)argb));

    /// <summary>
    ///  The palette for classic (light) Windows: bright faces with dark captions, in the spirit of
    ///  a hard-plastic mechanical register.
    /// </summary>
    public static CashRegisterPalette Classic { get; } = new(
        number: new KeyStyle(Rgb(0xFFF3F3EE), Rgb(0xFF1E2430), Rgb(0xFFBFC2BA)),
        function: new KeyStyle(Rgb(0xFFFFC24A), Rgb(0xFF3A2A00), Rgb(0xFFC8912A)),
        department: new KeyStyle(Rgb(0xFF2E9E6B), Rgb(0xFFFFFFFF), Rgb(0xFF1B6B47)),
        total: new KeyStyle(Rgb(0xFFC82A2A), Rgb(0xFFFFFFFF), Rgb(0xFF7E1414)));

    /// <summary>
    ///  The palette for dark mode: deep, desaturated faces with light captions that keep the same
    ///  category hues but sit comfortably on a dark form.
    /// </summary>
    public static CashRegisterPalette Dark { get; } = new(
        number: new KeyStyle(Rgb(0xFF3A4048), Rgb(0xFFF2F4F7), Rgb(0xFF5A626C)),
        function: new KeyStyle(Rgb(0xFFB8862A), Rgb(0xFF1A1206), Rgb(0xFF7A5A17)),
        department: new KeyStyle(Rgb(0xFF2C7D57), Rgb(0xFFF2F4F7), Rgb(0xFF184D35)),
        total: new KeyStyle(Rgb(0xFFB33636), Rgb(0xFFFFF2F2), Rgb(0xFF6E1C1C)));

    /// <summary>
    ///  Gets the palette that matches the current Windows color mode.
    /// </summary>
    public static CashRegisterPalette Current
        => Application.IsDarkModeEnabled ? Dark : Classic;

    /// <summary>
    ///  Gets the form/background surface color that suits the current color mode. Used so the
    ///  register body reads as a dark or light chassis behind the keys.
    /// </summary>
    public static Color SurfaceColor
        => Application.IsDarkModeEnabled ? Rgb(0xFF20242A) : Rgb(0xFFE7E7E1);

    /// <summary>
    ///  Gets the caption/label color that reads on <see cref="SurfaceColor"/>.
    /// </summary>
    public static Color OnSurfaceColor
        => Application.IsDarkModeEnabled ? Rgb(0xFFE6E8EC) : Rgb(0xFF2A2E24);
}
