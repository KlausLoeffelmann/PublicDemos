using System.Diagnostics;
using System.Drawing;

using WarpToolkit.WinForms.DirectX.D2D;

namespace WarpClock.Engine;

/// <summary>
///  Draws an engine-owned fallback time-zone headline in the upper-left corner when the
///  active theme does not materialize its own time-zone visual.
/// </summary>
internal sealed class TimeZoneHeadlineOverlay : IDisposable
{
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private Font? _font;
    private float _fontSize;

    public void Render(ID2DGraphics graphics, Size client, string text, bool nightMode)
    {
        if (client.Width < 8 || client.Height < 8 || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        EnsureFont(client);

        if (_font is null)
        {
            return;
        }

        float fade = ComputeFade((float)_clock.Elapsed.TotalSeconds);
        Color fill = ApplyAlpha(GetBaseColor(nightMode), fade);
        Color shadow = ApplyAlpha(nightMode ? Color.FromArgb(10, 16, 30) : Color.FromArgb(20, 28, 40), fade * 0.6f);
        float marginX = MathF.Max(12f, client.Width * 0.018f);
        float marginY = MathF.Max(10f, client.Height * 0.018f);
        float shadowOffset = MathF.Max(1f, _fontSize * 0.05f);

        graphics.DrawString(text, _font, shadow, marginX + shadowOffset, marginY + shadowOffset);
        graphics.DrawString(text, _font, fill, marginX, marginY);
    }

    internal static bool ShouldRender(
        bool enabled,
        string? text,
        IReadOnlyList<WarpClock.Abstractions.ClockElementDescriptor> descriptors)
        => enabled
            && !string.IsNullOrWhiteSpace(text)
            && !descriptors.Any(descriptor => descriptor.Id.Kind == WarpClock.Abstractions.ClockElementKind.TimeZone);

    internal static float ComputeFade(float elapsedSeconds)
        => 0.42f + ((MathF.Sin(elapsedSeconds * 0.8f) + 1f) * 0.16f);

    internal static Color GetBaseColor(bool nightMode)
        => nightMode
            ? Color.FromArgb(48, 66, 104)
            : Color.FromArgb(182, 210, 238);

    private void EnsureFont(Size client)
    {
        float requestedSize = Math.Clamp(client.Height * 0.028f, 12f, 28f);
        if (_font is not null && MathF.Abs(_fontSize - requestedSize) < 0.1f)
        {
            return;
        }

        _font?.Dispose();
        _fontSize = requestedSize;
        _font = new Font("Segoe UI", requestedSize, FontStyle.Bold, GraphicsUnit.Pixel);
    }

    private static Color ApplyAlpha(Color color, float opacity)
        => Color.FromArgb((int)MathF.Round(Math.Clamp(opacity, 0f, 1f) * 255f), color.R, color.G, color.B);

    public void Dispose()
    {
        _font?.Dispose();
        _font = null;
    }
}
