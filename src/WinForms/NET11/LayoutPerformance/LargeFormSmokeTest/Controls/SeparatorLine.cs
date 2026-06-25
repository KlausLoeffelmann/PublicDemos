namespace LargeFormSmokeTest.Controls;

using LargeFormSmokeTest.Theming;

/// <summary>
///  A decorative, chrome-only horizontal rule used to separate field groups inside a dense
///  section. It draws a single subtle 1px line centered vertically, themed for light/dark mode.
/// </summary>
public sealed class SeparatorLine : Control
{
    private readonly ThemeManager _theme = AppServices.Theme;

    /// <summary>Initializes a double-buffered, owner-drawn separator line.</summary>
    public SeparatorLine()
    {
        SetStyle(
            ControlStyles.UserPaint
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw,
            true);

        // The control reserves a little vertical breathing room around the hairline.
        Height = 10;

        _theme.ThemeChanged += OnThemeChanged;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _theme.ThemeChanged -= OnThemeChanged;
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    protected override void OnPaint(PaintEventArgs e)
    {
        Color line = _theme.IsDark ? Color.FromArgb(80, 80, 84) : Color.FromArgb(210, 210, 215);
        int y = Height / 2;

        using Pen pen = new(line);
        e.Graphics.DrawLine(pen, 0, y, Width, y);
    }

    private void OnThemeChanged(object? sender, EventArgs e)
        => Invalidate();
}
