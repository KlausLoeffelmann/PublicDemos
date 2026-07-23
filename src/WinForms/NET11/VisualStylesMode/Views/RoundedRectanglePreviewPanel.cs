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
///  A single, double-buffered preview card. It paints the shared
///  <see cref="RoundedRectanglePrototypeParameters.BackgroundTheme"/> background, a caption, and the
///  rounded rectangle rendered with its assigned <see cref="Technique"/>. The card sizes itself to
///  the shared Width/Height parameters so a <see cref="FlowLayoutPanel"/> can arrange the cards, and
///  all cards re-paint together whenever any parameter changes.
/// </summary>
internal sealed class RoundedRectanglePreviewPanel : Panel
{
    private const int CaptionHeightDip = 24;
    private const int PaddingDip = 16;

    private readonly RoundedRectanglePrototypeParameters _parameters;

    public RoundedRectanglePreviewPanel(
        RoundedRectangleTechnique technique,
        RoundedRectanglePrototypeParameters parameters)
    {
        Technique = technique;
        _parameters = parameters;

        DoubleBuffered = true;
        Margin = new Padding(8);

        _parameters.Changed += OnParametersChanged;
        UpdatePreferredSize();
    }

    public RoundedRectangleTechnique Technique { get; }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdatePreferredSize();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics graphics = e.Graphics;
        Rectangle client = ClientRectangle;
        float dpiScale = DeviceDpi / 96f;

        PaintBackgroundTheme(graphics, client);
        PaintCaption(graphics, client, dpiScale);

        float padding = PaddingDip * dpiScale;
        float captionHeight = CaptionHeightDip * dpiScale;
        float availableX = padding;
        float availableY = captionHeight + padding;
        float availableWidth = client.Width - (2f * padding);
        float availableHeight = client.Height - captionHeight - (2f * padding);
        if (availableWidth <= 1f || availableHeight <= 1f)
        {
            return;
        }

        float rectWidth = Math.Min(_parameters.RectWidth * dpiScale, availableWidth);
        float rectHeight = Math.Min(_parameters.RectHeight * dpiScale, availableHeight);
        RectangleF area = new(
            availableX + ((availableWidth - rectWidth) / 2f),
            availableY + ((availableHeight - rectHeight) / 2f),
            rectWidth,
            rectHeight);

        RoundedRectangleRenderer.Draw(graphics, area, Technique, _parameters, GetBodyFillColor(), dpiScale);
    }

    private void UpdatePreferredSize()
    {
        float dpiScale = DeviceDpi / 96f;
        int width = (int)Math.Ceiling((_parameters.RectWidth + (2 * PaddingDip)) * dpiScale);
        int height = (int)Math.Ceiling((_parameters.RectHeight + CaptionHeightDip + (2 * PaddingDip)) * dpiScale);
        Size = new Size(width, height);
    }

    private void PaintBackgroundTheme(Graphics graphics, Rectangle client)
    {
        switch (_parameters.BackgroundTheme)
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

    private void PaintCaption(Graphics graphics, Rectangle client, float dpiScale)
    {
        int captionHeight = (int)Math.Ceiling(CaptionHeightDip * dpiScale);
        Rectangle captionBounds = new(client.X, client.Y, client.Width, captionHeight);
        TextRenderer.DrawText(
            graphics,
            RoundedRectangleRenderer.GetCaption(Technique),
            Font,
            captionBounds,
            GetCaptionColor(),
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private Color GetCaptionColor() => _parameters.BackgroundTheme switch
    {
        PreviewBackgroundTheme.Dark => Color.Gainsboro,
        PreviewBackgroundTheme.Classic => Color.FromArgb(40, 40, 40),
        PreviewBackgroundTheme.Colorful => Color.White,
        _ => Color.Black,
    };

    private Color GetBodyFillColor() => _parameters.BackgroundTheme switch
    {
        PreviewBackgroundTheme.Dark => Color.FromArgb(58, 58, 60),
        PreviewBackgroundTheme.Classic => Color.White,
        PreviewBackgroundTheme.Colorful => Color.White,
        _ => Color.White,
    };

    private void OnParametersChanged(object? sender, EventArgs e)
    {
        UpdatePreferredSize();
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _parameters.Changed -= OnParametersChanged;
        }

        base.Dispose(disposing);
    }
}
