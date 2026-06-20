using System.Drawing;

namespace WarpClock.Engine;

/// <summary>
///  The dial geometry for a given surface size: where the center is and the
///  reference radius used to place markers and size content.
/// </summary>
public readonly record struct ClockGeometry
{
    /// <summary>The surface size in pixels.</summary>
    public required SizeF Surface { get; init; }

    /// <summary>The dial center in pixels.</summary>
    public required PointF Center { get; init; }

    /// <summary>The reference radius in pixels (half the smaller surface dimension).</summary>
    public required float Radius { get; init; }

    /// <summary>The design-space reference radius all themes author against.</summary>
    public const float DesignRadius = 500f;

    /// <summary>Creates the geometry for a surface, fitting the largest centered dial.</summary>
    public static ClockGeometry ForSurface(SizeF surface)
    {
        float radius = MathF.Min(surface.Width, surface.Height) / 2f;
        return new ClockGeometry
        {
            Surface = surface,
            Center = new PointF(surface.Width / 2f, surface.Height / 2f),
            Radius = radius,
        };
    }

    /// <summary>The scale factor from design units (radius 500) to pixels.</summary>
    public float DesignScale => Radius / DesignRadius;
}
