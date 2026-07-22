// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  The background environment a preview cell paints its rounded rectangle over, so the same
///  technique can be judged against a dark, a classic (light), and a colorful surface.
/// </summary>
internal enum PreviewBackgroundTheme
{
    Dark,
    Classic,
    Colorful,
}

/// <summary>
///  A single, double-buffered preview cell. It paints its assigned <see cref="Theme"/> background and
///  then renders the shared <see cref="RoundedRectanglePrototypeParameters"/> with its assigned
///  <see cref="Technique"/>. All cells share one parameter instance and re-paint together whenever it
///  changes, so tuning any control updates every technique across every theme at once.
/// </summary>
internal sealed class RoundedRectanglePreviewPanel : Panel
{
    private readonly RoundedRectanglePrototypeParameters _parameters;

    public RoundedRectanglePreviewPanel(
        RoundedRectangleTechnique technique,
        PreviewBackgroundTheme theme,
        RoundedRectanglePrototypeParameters parameters)
    {
        Technique = technique;
        Theme = theme;
        _parameters = parameters;

        DoubleBuffered = true;
        ResizeRedraw = true;
        Margin = new Padding(4);
        Dock = DockStyle.Fill;

        _parameters.Changed += OnParametersChanged;
    }

    public RoundedRectangleTechnique Technique { get; }

    public PreviewBackgroundTheme Theme { get; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics graphics = e.Graphics;
        Rectangle client = ClientRectangle;

        PaintBackgroundTheme(graphics, client);

        float dpiScale = DeviceDpi / 96f;
        float padding = 18f * dpiScale;
        RectangleF area = RectangleF.Inflate(client, -padding, -padding);
        if (area.Width <= 1f || area.Height <= 1f)
        {
            return;
        }

        RoundedRectangleRenderer.Draw(graphics, area, Technique, _parameters, GetBodyFillColor(), dpiScale);
    }

    private void PaintBackgroundTheme(Graphics graphics, Rectangle client)
    {
        switch (Theme)
        {
            case PreviewBackgroundTheme.Dark:
                using (SolidBrush brush = new(Color.FromArgb(32, 32, 32)))
                {
                    graphics.FillRectangle(brush, client);
                }

                break;

            case PreviewBackgroundTheme.Classic:
                using (SolidBrush brush = new(Color.FromArgb(240, 240, 240)))
                {
                    graphics.FillRectangle(brush, client);
                }

                break;

            case PreviewBackgroundTheme.Colorful:
                if (client.Width > 0 && client.Height > 0)
                {
                    using LinearGradientBrush brush = new(
                        client,
                        Color.FromArgb(64, 160, 255),
                        Color.FromArgb(200, 80, 200),
                        LinearGradientMode.ForwardDiagonal);
                    graphics.FillRectangle(brush, client);
                }

                break;
        }
    }

    private Color GetBodyFillColor() => Theme switch
    {
        PreviewBackgroundTheme.Dark => Color.FromArgb(58, 58, 60),
        PreviewBackgroundTheme.Classic => Color.White,
        PreviewBackgroundTheme.Colorful => Color.White,
        _ => Color.White,
    };

    private void OnParametersChanged(object? sender, EventArgs e) => Invalidate();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _parameters.Changed -= OnParametersChanged;
        }

        base.Dispose(disposing);
    }
}
