using System.Drawing.Drawing2D;

namespace Northwind.App;

internal static class FluentIconImageFactory
{
    private const int IconSize = 40;

    public static Image CreateAdd() => Render(g =>
    {
        using var pen = CreatePen();
        g.DrawLine(pen, 20, 8, 20, 32);
        g.DrawLine(pen, 8, 20, 32, 20);
    });

    public static Image CreateEdit() => Render(g =>
    {
        using var pen = CreatePen();
        g.DrawLine(pen, 10, 28, 28, 10);
        g.DrawLine(pen, 12, 30, 30, 12);
        g.DrawLine(pen, 8, 32, 16, 32);
    });

    public static Image CreateCancel() => Render(g =>
    {
        using var pen = CreatePen();
        g.DrawLine(pen, 10, 10, 30, 30);
        g.DrawLine(pen, 30, 10, 10, 30);
    });

    public static Image CreateSave() => Render(g =>
    {
        using var pen = CreatePen();
        g.DrawRectangle(pen, 9, 9, 22, 22);
        g.DrawLine(pen, 13, 19, 27, 19);
        g.DrawLine(pen, 13, 24, 27, 24);
    });

    private static Pen CreatePen()
    {
        return new Pen(Color.FromArgb(50, 50, 50), 3)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
    }

    private static Image Render(Action<Graphics> draw)
    {
        var bitmap = new Bitmap(IconSize, IconSize);
        using var g = Graphics.FromImage(bitmap);
        g.Clear(Color.Transparent);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        draw(g);
        return bitmap;
    }
}
