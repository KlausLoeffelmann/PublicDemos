namespace WarpClock.Themes.Builtin;

internal readonly record struct NerdSlideSnapshot(
    int Index,
    bool Active,
    float Angle,
    float Speed);

internal readonly record struct NerdSlideRenderState(
    float Angle,
    float TrackRadius,
    float BeamScale,
    int PositionSecond,
    float CheatOpacity);

internal readonly record struct NerdHandRenderState(
    float Angle,
    float HourCheatOpacity,
    float MinuteCheatOpacity);

internal readonly record struct NerdCheatSample(
    float HourOpacity,
    float MinuteOpacity,
    float SledOpacity);

internal static class NerdCheatSequence
{
    private const float SequenceSeconds = 6f;
    private const float CrossFadeSeconds = 0.30f;

    public static NerdCheatSample Sample(DateTime now, bool enabled)
    {
        if (!enabled)
        {
            return default;
        }

        float elapsed = (float)(now.TimeOfDay.TotalSeconds % 30d);
        if (elapsed < 0f || elapsed >= SequenceSeconds)
        {
            return default;
        }

        return new NerdCheatSample(
            HourOpacity: StageOpacity(elapsed, 0f, 2f + (CrossFadeSeconds / 2f), fadeIn: false),
            MinuteOpacity: StageOpacity(
                elapsed,
                2f - (CrossFadeSeconds / 2f),
                4f + (CrossFadeSeconds / 2f),
                fadeIn: true),
            SledOpacity: StageOpacity(
                elapsed,
                4f - (CrossFadeSeconds / 2f),
                SequenceSeconds,
                fadeIn: true));
    }

    private static float StageOpacity(float elapsed, float start, float end, bool fadeIn)
    {
        if (elapsed < start || elapsed >= end)
        {
            return 0f;
        }

        float entering = fadeIn
            ? SmoothStep(Math.Clamp((elapsed - start) / CrossFadeSeconds, 0f, 1f))
            : 1f;
        float leaving = SmoothStep(Math.Clamp((end - elapsed) / CrossFadeSeconds, 0f, 1f));
        return Math.Min(entering, leaving);
    }

    private static float SmoothStep(float value)
        => value * value * (3f - (2f * value));
}

internal sealed class NerdSlideTrackPlanner
{
    private const float DegreesPerSecond = 6f;
    private const float ReleaseGapDegrees =
        NerdThemeGeometry.SledCollisionAngularSpan + NerdThemeGeometry.SledAngularSafetyGap;

    private readonly bool[,] _passing =
        new bool[NerdThemeGeometry.SledTrackCount, NerdThemeGeometry.SledTrackCount];

    public int[] Plan(IReadOnlyList<NerdSlideSnapshot> slides)
    {
        ArgumentNullException.ThrowIfNull(slides);

        UpdatePasses(slides);

        int[] tracks = new int[NerdThemeGeometry.SledTrackCount];
        bool[] resolving = new bool[NerdThemeGeometry.SledTrackCount];
        bool[] resolved = new bool[NerdThemeGeometry.SledTrackCount];
        foreach (NerdSlideSnapshot slide in slides.Where(slide => slide.Active))
        {
            ResolveTrack(slide.Index, slides, tracks, resolving, resolved);
        }

        return tracks;
    }

    public bool IsPassing(int fasterIndex, int slowerIndex)
        => _passing[fasterIndex, slowerIndex];

    public static float FindSafeSpawnAngle(IReadOnlyList<NerdSlideSnapshot> slides)
    {
        float[] angles = slides
            .Where(slide => slide.Active)
            .Select(slide => Normalize(slide.Angle))
            .OrderBy(angle => angle)
            .ToArray();

        if (angles.Length == 0)
        {
            return 0f;
        }

        float largestGap = -1f;
        float gapStart = angles[0];
        for (int index = 0; index < angles.Length; index++)
        {
            float current = angles[index];
            float next = index == angles.Length - 1 ? angles[0] + 360f : angles[index + 1];
            float gap = next - current;
            if (gap > largestGap)
            {
                largestGap = gap;
                gapStart = current;
            }
        }

        return Normalize(gapStart + (largestGap / 2f));
    }

    public static bool SledsOverlap(
        float firstAngle,
        float firstRadius,
        float secondAngle,
        float secondRadius)
        => AngularDistance(firstAngle, secondAngle) < NerdThemeGeometry.SledCollisionAngularSpan
            && MathF.Abs(firstRadius - secondRadius) < NerdThemeGeometry.SledCollisionRadialSpan;

    private void UpdatePasses(IReadOnlyList<NerdSlideSnapshot> slides)
    {
        for (int fasterIndex = 0; fasterIndex < NerdThemeGeometry.SledTrackCount; fasterIndex++)
        {
            NerdSlideSnapshot faster = slides[fasterIndex];
            for (int slowerIndex = 0; slowerIndex < NerdThemeGeometry.SledTrackCount; slowerIndex++)
            {
                if (fasterIndex == slowerIndex)
                {
                    continue;
                }

                NerdSlideSnapshot slower = slides[slowerIndex];
                if (!faster.Active || !slower.Active)
                {
                    _passing[fasterIndex, slowerIndex] = false;
                    continue;
                }

                float signedGap = SignedAngularDelta(faster.Angle, slower.Angle);
                if (_passing[fasterIndex, slowerIndex])
                {
                    bool stillFaster = faster.Speed > slower.Speed;
                    bool safelySeparated = stillFaster
                        ? signedGap <= -ReleaseGapDegrees
                        : MathF.Abs(signedGap) >= ReleaseGapDegrees;
                    if (safelySeparated)
                    {
                        _passing[fasterIndex, slowerIndex] = false;
                    }

                    continue;
                }

                if (faster.Speed <= slower.Speed || _passing[slowerIndex, fasterIndex])
                {
                    continue;
                }

                float relativeDegreesPerSecond = (faster.Speed - slower.Speed) * DegreesPerSecond;
                float entryGap =
                    NerdThemeGeometry.SledCollisionAngularSpan
                    + NerdThemeGeometry.SledAngularSafetyGap
                    + (relativeDegreesPerSecond
                        * NerdThemeGeometry.SledMaximumTrackTransitionSeconds);

                if (signedGap >= 0f
                    && signedGap <= entryGap
                    && !WouldCreateCycle(fasterIndex, slowerIndex))
                {
                    _passing[fasterIndex, slowerIndex] = true;
                }
            }

        }
    }

    private bool WouldCreateCycle(int fasterIndex, int slowerIndex)
    {
        Span<bool> visited = stackalloc bool[NerdThemeGeometry.SledTrackCount];
        return HasPath(slowerIndex, fasterIndex, visited);
    }

    private bool HasPath(int fromIndex, int targetIndex, Span<bool> visited)
    {
        if (fromIndex == targetIndex)
        {
            return true;
        }

        if (visited[fromIndex])
        {
            return false;
        }

        visited[fromIndex] = true;
        for (int nextIndex = 0; nextIndex < NerdThemeGeometry.SledTrackCount; nextIndex++)
        {
            if (_passing[fromIndex, nextIndex]
                && HasPath(nextIndex, targetIndex, visited))
            {
                return true;
            }
        }

        return false;
    }

    private int ResolveTrack(
        int index,
        IReadOnlyList<NerdSlideSnapshot> slides,
        int[] tracks,
        bool[] resolving,
        bool[] resolved)
    {
        if (resolved[index])
        {
            return tracks[index];
        }

        if (resolving[index])
        {
            return tracks[index];
        }

        resolving[index] = true;
        int track = 0;
        for (int otherIndex = 0; otherIndex < NerdThemeGeometry.SledTrackCount; otherIndex++)
        {
            if (!_passing[index, otherIndex] || !slides[otherIndex].Active)
            {
                continue;
            }

            track = Math.Max(
                track,
                ResolveTrack(otherIndex, slides, tracks, resolving, resolved) + 1);
        }

        resolving[index] = false;
        resolved[index] = true;
        tracks[index] = Math.Min(track, NerdThemeGeometry.SledTrackCount - 1);
        return tracks[index];
    }

    private static float SignedAngularDelta(float from, float to)
    {
        float delta = Normalize(to) - Normalize(from);
        if (delta > 180f)
        {
            delta -= 360f;
        }
        else if (delta <= -180f)
        {
            delta += 360f;
        }

        return delta;
    }

    private static float AngularDistance(float first, float second)
        => MathF.Abs(SignedAngularDelta(first, second));

    private static float Normalize(float degrees)
    {
        degrees %= 360f;
        return degrees < 0f ? degrees + 360f : degrees;
    }
}
