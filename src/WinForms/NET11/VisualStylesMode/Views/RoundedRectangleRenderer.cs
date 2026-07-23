// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace VisualStylesModeDemo.Views;

/// <summary>
///  The rounded-rectangle rendering technique used by a single preview cell.
/// </summary>
internal enum RoundedRectangleTechnique
{
    /// <summary>
    ///  The current framework way: the built-in <see cref="Graphics.DrawRoundedRectangle(Pen, RectangleF, SizeF)"/>
    ///  and <see cref="Graphics.FillRoundedRectangle(Brush, RectangleF, SizeF)"/> APIs. This is what
    ///  most code uses today and exhibits the reported corner-arc anti-aliasing artifact.
    /// </summary>
    BuiltInFramework,

    /// <summary>
    ///  The common "naive" recipe: an arc-based <see cref="GraphicsPath"/> stroked with a centered
    ///  pen under <see cref="SmoothingMode.AntiAlias"/> but the default
    ///  <see cref="PixelOffsetMode"/>. Reproduces the same artifact by hand so it can be compared to
    ///  the framework method and to the fixes.
    /// </summary>
    NaiveBaseline,

    /// <summary>
    ///  Identical geometry to <see cref="NaiveBaseline"/> but with
    ///  <see cref="PixelOffsetMode.HighQuality"/>. This single change realigns the arc anti-aliasing
    ///  with the straight edges and removes most of the artifact.
    /// </summary>
    PixelOffsetHighQuality,

    /// <summary>
    ///  <see cref="PixelOffsetMode.HighQuality"/> plus a half-pen inset so the entire stroke sits
    ///  inside the body on a consistent sub-pixel grid, yielding a crisper, more uniform border.
    /// </summary>
    HighQualityInsetStroke,

    /// <summary>
    ///  Converts the stroke into a filled outline region via <see cref="GraphicsPath.Widen"/> and
    ///  fills it. Because the border becomes a filled shape rather than a stroked path, the arc/line
    ///  join is seam-free and the anti-aliasing is perfectly uniform.
    /// </summary>
    WidenedOutline,

    /// <summary>
    ///  Supersampled anti-aliasing (SSAA): the rounded rectangle is rendered into an offscreen 32bpp
    ///  bitmap at 2-4x scale and drawn back down with <see cref="InterpolationMode.HighQualityBicubic"/>.
    ///  The most robust result across every color and background.
    /// </summary>
    Supersampled,
}

/// <summary>
///  Conceptual prototype renderer that draws the same rounded rectangle with several GDI+ techniques
///  so their anti-aliasing quality can be compared side by side. Not a shipping control - it exists
///  purely to explore how to make corner arcs blend seamlessly with the straight edges.
/// </summary>
internal static class RoundedRectangleRenderer
{
    /// <summary>
    ///  Draws <paramref name="parameters"/> into <paramref name="area"/> (panel-relative, in pixels)
    ///  using the requested <paramref name="technique"/>. <paramref name="fillColor"/> is the
    ///  theme-appropriate body color; the stroke color comes from the parameters.
    /// </summary>
    public static void Draw(
        Graphics graphics,
        RectangleF area,
        RoundedRectangleTechnique technique,
        RoundedRectanglePrototypeParameters parameters,
        Color fillColor,
        float dpiScale)
    {
        if (area.Width <= 1f || area.Height <= 1f)
        {
            return;
        }

        float radius = parameters.CornerRadius * dpiScale;
        float penWidth = Math.Max(1f, parameters.BorderThickness * dpiScale);

        radius = Math.Clamp(radius, 0f, Math.Min(area.Width, area.Height) / 2f);

        switch (technique)
        {
            case RoundedRectangleTechnique.BuiltInFramework:
                DrawBuiltIn(graphics, area, radius, penWidth, parameters, fillColor);
                break;

            case RoundedRectangleTechnique.NaiveBaseline:
                DrawStroked(graphics, area, radius, penWidth, parameters, fillColor, PixelOffsetMode.Default, strokeInset: 0f);
                break;

            case RoundedRectangleTechnique.PixelOffsetHighQuality:
                DrawStroked(graphics, area, radius, penWidth, parameters, fillColor, PixelOffsetMode.HighQuality, strokeInset: 0f);
                break;

            case RoundedRectangleTechnique.HighQualityInsetStroke:
                DrawStroked(graphics, area, radius, penWidth, parameters, fillColor, PixelOffsetMode.HighQuality, strokeInset: penWidth / 2f);
                break;

            case RoundedRectangleTechnique.WidenedOutline:
                DrawWidenedOutline(graphics, area, radius, penWidth, parameters, fillColor);
                break;

            case RoundedRectangleTechnique.Supersampled:
                DrawSupersampled(graphics, area, radius, penWidth, parameters, fillColor);
                break;
        }
    }

    /// <summary>Human-readable caption for a technique (used as a card header).</summary>
    public static string GetCaption(RoundedRectangleTechnique technique) => technique switch
    {
        RoundedRectangleTechnique.BuiltInFramework => "Graphics.DrawRoundedRectangle (current)",
        RoundedRectangleTechnique.NaiveBaseline => "Manual arc path (naive)",
        RoundedRectangleTechnique.PixelOffsetHighQuality => "+ PixelOffset HQ",
        RoundedRectangleTechnique.HighQualityInsetStroke => "HQ + inset stroke",
        RoundedRectangleTechnique.WidenedOutline => "Widened outline",
        RoundedRectangleTechnique.Supersampled => "Supersampled (SSAA)",
        _ => technique.ToString(),
    };

    /// <summary>
    ///  True for the techniques that represent how a rounded rectangle is drawn today (the built-in
    ///  framework method and the equivalent hand-rolled arc path), as opposed to the improved
    ///  variants that fix the corner-arc artifact.
    /// </summary>
    public static bool IsCurrentTechnique(RoundedRectangleTechnique technique)
        => technique is RoundedRectangleTechnique.BuiltInFramework or RoundedRectangleTechnique.NaiveBaseline;

    private static void DrawBuiltIn(
        Graphics graphics,
        RectangleF area,
        float radius,
        float penWidth,
        RoundedRectanglePrototypeParameters parameters,
        Color fillColor)
    {
        SmoothingMode previousSmoothing = graphics.SmoothingMode;

        try
        {
            // Leave PixelOffsetMode at its default: this cell demonstrates the framework method as it
            // is typically called, which is exactly where the corner-arc artifact shows up.
            graphics.SmoothingMode = ResolveSmoothing(parameters);

            SizeF cornerRadius = new(radius, radius);
            if (parameters.FillEnabled)
            {
                using SolidBrush brush = new(Color.FromArgb(parameters.FillAlpha, fillColor));
                graphics.FillRoundedRectangle(brush, area, cornerRadius);
            }

            using Pen pen = new(parameters.StrokeColor, penWidth);
            graphics.DrawRoundedRectangle(pen, area, cornerRadius);
        }
        finally
        {
            graphics.SmoothingMode = previousSmoothing;
        }
    }

    private static SmoothingMode ResolveSmoothing(RoundedRectanglePrototypeParameters parameters)
        => parameters.AntiAliasEnabled ? SmoothingMode.AntiAlias : SmoothingMode.None;

    private static void DrawStroked(
        Graphics graphics,
        RectangleF area,
        float radius,
        float penWidth,
        RoundedRectanglePrototypeParameters parameters,
        Color fillColor,
        PixelOffsetMode pixelOffsetMode,
        float strokeInset)
    {
        SmoothingMode previousSmoothing = graphics.SmoothingMode;
        PixelOffsetMode previousOffset = graphics.PixelOffsetMode;

        try
        {
            graphics.SmoothingMode = ResolveSmoothing(parameters);
            graphics.PixelOffsetMode = pixelOffsetMode;

            if (parameters.FillEnabled)
            {
                using GraphicsPath body = CreateRoundedPath(area, radius);
                using SolidBrush brush = new(Color.FromArgb(parameters.FillAlpha, fillColor));
                graphics.FillPath(brush, body);
            }

            RectangleF strokeRect = RectangleF.Inflate(area, -strokeInset, -strokeInset);
            float strokeRadius = Math.Max(0f, radius - strokeInset);
            using GraphicsPath strokePath = CreateRoundedPath(strokeRect, strokeRadius);
            using Pen pen = new(parameters.StrokeColor, penWidth)
            {
                Alignment = PenAlignment.Center,
                LineJoin = LineJoin.Round,
            };
            graphics.DrawPath(pen, strokePath);
        }
        finally
        {
            graphics.SmoothingMode = previousSmoothing;
            graphics.PixelOffsetMode = previousOffset;
        }
    }

    private static void DrawWidenedOutline(
        Graphics graphics,
        RectangleF area,
        float radius,
        float penWidth,
        RoundedRectanglePrototypeParameters parameters,
        Color fillColor)
    {
        SmoothingMode previousSmoothing = graphics.SmoothingMode;
        PixelOffsetMode previousOffset = graphics.PixelOffsetMode;

        try
        {
            graphics.SmoothingMode = ResolveSmoothing(parameters);
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            if (parameters.FillEnabled)
            {
                using GraphicsPath body = CreateRoundedPath(area, radius);
                using SolidBrush brush = new(Color.FromArgb(parameters.FillAlpha, fillColor));
                graphics.FillPath(brush, body);
            }

            // Keep the widened outline fully inside the body so both halves of the border share the
            // same uniform anti-aliasing.
            RectangleF outlineRect = RectangleF.Inflate(area, -penWidth / 2f, -penWidth / 2f);
            float outlineRadius = Math.Max(0f, radius - penWidth / 2f);
            using GraphicsPath outline = CreateRoundedPath(outlineRect, outlineRadius);

            using (Pen pen = new(parameters.StrokeColor, penWidth) { LineJoin = LineJoin.Round })
            {
                outline.Widen(pen);
            }

            using SolidBrush strokeBrush = new(parameters.StrokeColor);
            graphics.FillPath(strokeBrush, outline);
        }
        finally
        {
            graphics.SmoothingMode = previousSmoothing;
            graphics.PixelOffsetMode = previousOffset;
        }
    }

    private static void DrawSupersampled(
        Graphics graphics,
        RectangleF area,
        float radius,
        float penWidth,
        RoundedRectanglePrototypeParameters parameters,
        Color fillColor)
    {
        int factor = Math.Clamp(parameters.SupersamplingFactor, 2, 4);

        // Inflate by the pen width so the outward half of a centered stroke is not clipped, then work
        // in the high-resolution bitmap's coordinate space.
        float margin = penWidth;
        RectangleF hiArea = RectangleF.Inflate(area, margin, margin);
        int width = (int)Math.Ceiling(hiArea.Width * factor);
        int height = (int)Math.Ceiling(hiArea.Height * factor);

        if (width <= 0 || height <= 0)
        {
            return;
        }

        using Bitmap bitmap = new(width, height, PixelFormat.Format32bppPArgb);

        using (Graphics hi = Graphics.FromImage(bitmap))
        {
            hi.SmoothingMode = ResolveSmoothing(parameters);
            hi.PixelOffsetMode = PixelOffsetMode.HighQuality;
            hi.Clear(Color.Transparent);

            RectangleF shape = new(
                (area.X - hiArea.X) * factor,
                (area.Y - hiArea.Y) * factor,
                area.Width * factor,
                area.Height * factor);
            float shapeRadius = radius * factor;
            float shapePen = penWidth * factor;

            using GraphicsPath path = CreateRoundedPath(shape, shapeRadius);

            if (parameters.FillEnabled)
            {
                using SolidBrush brush = new(Color.FromArgb(parameters.FillAlpha, fillColor));
                hi.FillPath(brush, path);
            }

            using Pen pen = new(parameters.StrokeColor, shapePen)
            {
                Alignment = PenAlignment.Center,
                LineJoin = LineJoin.Round,
            };

            hi.DrawPath(pen, path);
        }

        InterpolationMode previousInterpolation = graphics.InterpolationMode;
        PixelOffsetMode previousOffset = graphics.PixelOffsetMode;

        try
        {
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.DrawImage(bitmap, hiArea);
        }
        finally
        {
            graphics.InterpolationMode = previousInterpolation;
            graphics.PixelOffsetMode = previousOffset;
        }
    }

    private static GraphicsPath CreateRoundedPath(RectangleF rect, float radius)
    {
        GraphicsPath path = new();
        radius = Math.Max(0f, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2f));

        if (radius < 0.5f || rect.Width < 1f || rect.Height < 1f)
        {
            path.AddRectangle(rect);
            return path;
        }

        float diameter = radius * 2f;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180f, 90f);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270f, 90f);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0f, 90f);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90f, 90f);
        path.CloseFigure();

        return path;
    }
}
