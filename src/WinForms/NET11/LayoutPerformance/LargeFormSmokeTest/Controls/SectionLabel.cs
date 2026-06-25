namespace LargeFormSmokeTest.Controls;

using System.Drawing.Drawing2D;
using LargeFormSmokeTest.Theming;

/// <summary>
///  A decorative, chrome-only section header. It paints a full-width rectangle filled with a
///  left-to-right gradient and draws its caption in bold text two points larger than the parent's
///  font. The colors follow the active theme:
///  <list type="bullet">
///   <item><description>Dark: black (#000000) to 70%-black gray (#4D4D4D), white text.</description></item>
///   <item><description>Light: white (#FFFFFF) to 30% gray (#B3B3B3), black text.</description></item>
///  </list>
///  It carries no input and is skipped by read-only handling.
/// </summary>
public sealed class SectionLabel : Control
{
    private readonly ThemeManager _theme = AppServices.Theme;
    private Font? _bannerFont;

    /// <summary>Initializes a double-buffered, owner-drawn section banner.</summary>
    public SectionLabel()
    {
        SetStyle(
            ControlStyles.UserPaint
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw,
            true);

        // A comfortable default band height; refined once the parent font is known.
        Height = 32;

        // Repaint whenever the theme flips so the gradient and text color stay correct.
        _theme.ThemeChanged += OnThemeChanged;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _theme.ThemeChanged -= OnThemeChanged;
            _bannerFont?.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);

        // Make the banner font bold and two points larger than the host form's base font. The
        // previous owned font (if any) is disposed before replacing it.
        if (Parent is not null)
        {
            _bannerFont?.Dispose();
            _bannerFont = new Font(Parent.Font.FontFamily, Parent.Font.Size + 2f, FontStyle.Bold);
            Font = _bannerFont;
            Height = Font.Height + 12;
        }
    }

    /// <inheritdoc/>
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        Rectangle bounds = ClientRectangle;

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        bool dark = _theme.IsDark;
        Color from = dark ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
        Color to = dark ? Color.FromArgb(77, 77, 77) : Color.FromArgb(179, 179, 179);
        Color textColor = dark ? Color.White : Color.Black;

        using (LinearGradientBrush brush = new(bounds, from, to, LinearGradientMode.Horizontal))
        {
            e.Graphics.FillRectangle(brush, bounds);
        }

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            Rectangle.Inflate(bounds, -12, 0),
            textColor,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private void OnThemeChanged(object? sender, EventArgs e)
        => Invalidate();
}
