using System.Drawing;

namespace WarpClock.Engine;

/// <summary>
///  The dial geometry for a given surface size: where the center is and the
///  reference radius used to place markers and size content.
/// </summary>
public readonly record struct ClockGeometry
{
    /// <summary>The logical scene surface size in pixels.</summary>
    public required SizeF Surface { get; init; }

    /// <summary>The top-left of the logical scene within the host surface, in pixels.</summary>
    public required PointF Origin { get; init; }

    /// <summary>The dial center in host pixels.</summary>
    public required PointF Center { get; init; }

    /// <summary>The reference radius in pixels (half the smaller surface dimension).</summary>
    public required float Radius { get; init; }

    /// <summary>The design-space reference radius all themes author against.</summary>
    public const float DesignRadius = 500f;

    /// <summary>The scene bounds in host pixels.</summary>
    public RectangleF Bounds => new(Origin, Surface);

    /// <summary>Creates the geometry for a surface, fitting the largest centered dial.</summary>
    public static ClockGeometry ForSurface(SizeF surface)
        => ForSurface(surface, OledSceneTransform.Identity);

    internal static ClockGeometry ForSurface(SizeF hostSurface, OledSceneTransform transform)
    {
        float scale = Math.Clamp(transform.Scale, 0.85f, 1f);
        SizeF surface = new(
            MathF.Max(1f, hostSurface.Width * scale),
            MathF.Max(1f, hostSurface.Height * scale));

        float maxOriginX = MathF.Max(0f, hostSurface.Width - surface.Width);
        float maxOriginY = MathF.Max(0f, hostSurface.Height - surface.Height);
        PointF origin = new(
            Math.Clamp((hostSurface.Width - surface.Width) / 2f + transform.Offset.X, 0f, maxOriginX),
            Math.Clamp((hostSurface.Height - surface.Height) / 2f + transform.Offset.Y, 0f, maxOriginY));

        float radius = MathF.Min(surface.Width, surface.Height) / 2f;
        return new ClockGeometry
        {
            Surface = surface,
            Origin = origin,
            Center = new PointF(origin.X + surface.Width / 2f, origin.Y + surface.Height / 2f),
            Radius = radius,
        };
    }

    /// <summary>The scale factor from design units (radius 500) to pixels.</summary>
    public float DesignScale => Radius / DesignRadius;
}
