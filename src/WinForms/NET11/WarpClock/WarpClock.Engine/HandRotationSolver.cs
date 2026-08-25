using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Engine;

/// <summary>
///  Everything the engine needs to aim one hand for one frame. The hand's displayed
///  rotation is derived exclusively from these engine-owned inputs, so a theme can
///  move anchors and pick a target mode but can never state an angle.
/// </summary>
internal readonly record struct HandRotationRequest
{
    /// <summary>The hand being aimed.</summary>
    public required ClockHandKind Hand { get; init; }

    /// <summary>The hand's pivot anchor in host pixels.</summary>
    public required PointF Pivot { get; init; }

    /// <summary>The authoritative time for this frame.</summary>
    public required ClockTimeSnapshot Time { get; init; }

    /// <summary>The target mode the theme requested for this hand.</summary>
    public required ClockHandTargetMode RequestedTargetMode { get; init; }

    /// <summary>The host-selected motion style for this hand.</summary>
    public required ClockHandMotion Motion { get; init; }

    /// <summary>Whether the active theme declares a free-floating layout.</summary>
    public required bool ThemeSupportsFreeFloating { get; init; }

    /// <summary>Whether radially aimed hands rotate with the theme's face rotation.</summary>
    public required bool HandsFollowFaceRotation { get; init; }

    /// <summary>Whether the host's global magnetic-numerals mode is on.</summary>
    public required bool MagneticNumeralsEnabled { get; init; }

    /// <summary>Resolves the live anchor of any element in host pixels.</summary>
    public required Func<ClockElementId, PointF> AnchorOf { get; init; }

    /// <summary>Returns an hour numeral's visibility, or <see langword="null"/> when absent.</summary>
    public required Func<int, ClockNumeralVisibility?> NumeralVisibilityOf { get; init; }

    /// <summary>The theme's current face rotation in degrees.</summary>
    public float FaceRotationDegrees { get; init; }

    /// <summary>The theme's requested extra rotation; clamped to a small wobble.</summary>
    public float ExtraRotationDegrees { get; init; }

    /// <summary>The grace catch-up window in seconds.</summary>
    public float GraceSeconds { get; init; }

    /// <summary>The ease-in-out glide duration in seconds.</summary>
    public float GlideDurationSeconds { get; init; }

    /// <summary>Elapsed seconds since the previous frame.</summary>
    public float DeltaSeconds { get; init; }
}

/// <summary>
///  The engine's hand-aiming pipeline: resolves the effective
///  <see cref="ClockHandTargetMode"/> once, then derives the displayed rotation with
///  the matching solver (magnetic numeral stepping, free-floating anchor pointing, or
///  radial dial pointing) plus the grace catch-up and the clamped theme wobble.
/// </summary>
/// <remarks>
///  <see cref="WarpClockControl"/> owns exactly one of these so every hand travels the
///  same code path, and so the mode decision cannot drift between magnetic and
///  non-magnetic branches.
/// </remarks>
internal sealed class HandRotationSolver
{
    /// <summary>Theme wobble is clamped so a theme can never misrepresent the time.</summary>
    private const float MaxWobbleDegrees = 5f;

    private readonly HandPointingSolver _pointing = new();
    private readonly MagneticNumeralSolver _magnetic = new();

    /// <summary>Clears all per-hand state (call on theme change or mode switch).</summary>
    public void Reset()
    {
        _pointing.Reset();
        _magnetic.Reset();
    }

    /// <summary>Resolves the effective target mode for a hand without aiming it.</summary>
    public static ClockHandTargetMode ResolveTargetMode(in HandRotationRequest request)
        => HandTargetModeResolver.Resolve(
            request.Hand,
            request.RequestedTargetMode,
            request.ThemeSupportsFreeFloating,
            request.MagneticNumeralsEnabled);

    /// <summary>Computes the hand's displayed rotation for this frame.</summary>
    public float Solve(in HandRotationRequest request)
    {
        ClockHandTargetMode targetMode = ResolveTargetMode(request);
        float wobble = Math.Clamp(request.ExtraRotationDegrees, -MaxWobbleDegrees, MaxWobbleDegrees);

        if (targetMode == ClockHandTargetMode.MagneticNumerals)
        {
            Func<ClockElementId, PointF> anchorOf = request.AnchorOf;
            float? magnetic = _magnetic.Solve(
                request.Hand,
                request.Pivot,
                request.Time,
                request.NumeralVisibilityOf,
                index => anchorOf(ClockElementId.HourMarker(index)));

            if (magnetic.HasValue)
            {
                return magnetic.Value + wobble;
            }

            // A theme may expose no targetable numerals. Preserve authoritative time
            // with radial aiming instead of parking the hand at 12.
            targetMode = ClockHandTargetMode.Radial;
        }

        float target;

        if (targetMode == ClockHandTargetMode.FreeFloating)
        {
            target = HandPointingSolver.FreeFloatingTargetAngle(
                request.Hand,
                request.Pivot,
                request.Time,
                request.Motion,
                request.GlideDurationSeconds,
                request.AnchorOf);
        }
        else
        {
            target = HandPointingSolver.RadialTargetAngle(
                request.Time,
                request.Hand,
                request.Motion,
                request.GlideDurationSeconds);

            if (request.HandsFollowFaceRotation)
            {
                target += request.FaceRotationDegrees;
            }
        }

        bool smoothFollow = targetMode == ClockHandTargetMode.FreeFloating
            && request.Motion != ClockHandMotion.Crawling;

        float displayed = _pointing.Solve(
            request.Hand,
            target,
            request.GraceSeconds,
            smoothFollow,
            request.DeltaSeconds);

        return displayed + wobble;
    }
}
