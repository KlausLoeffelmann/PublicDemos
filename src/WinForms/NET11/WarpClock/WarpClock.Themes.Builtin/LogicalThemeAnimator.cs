using System.Drawing;

using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal enum LogicalThemePhase
{
    Calm,
    Escalating,
    ZoomingOut,
    FlyingOff,
    Reassembling,
    ZoomingIn,
}

internal readonly record struct LogicalThemeSnapshot(
    LogicalThemePhase Phase,
    TimeSpan PhaseDuration,
    TimeSpan PhaseElapsed,
    float PhaseProgress,
    PointF CurrentSafeOffset,
    PointF TargetSafeOffset,
    PointF SourceStagingOffset,
    PointF DestinationOffset,
    SizeF ViewportSize,
    PointF SceneOffset,
    float SceneScale,
    float StormIntensity,
    float FlashIntensity,
    int CompletedCycles);

internal sealed class LogicalThemeStateMachine
{
    internal static readonly TimeSpan CalmMinDuration = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan CalmMaxDuration = TimeSpan.FromSeconds(120);
    internal static readonly TimeSpan EscalationMinDuration = TimeSpan.FromSeconds(5);
    internal static readonly TimeSpan EscalationMaxDuration = TimeSpan.FromSeconds(15);
    internal static readonly TimeSpan ZoomOutDuration = TimeSpan.FromSeconds(2.50);
    internal static readonly TimeSpan FlyOutDuration = TimeSpan.FromSeconds(5.20);
    internal static readonly TimeSpan ReassemblyDuration = TimeSpan.FromSeconds(1.80);
    internal static readonly TimeSpan ZoomInDuration = TimeSpan.FromSeconds(10);

    internal const float BaseSceneScale = 0.88f;
    internal const float ZoomedOutSceneScale = 0.50f;
    internal const float ViewportMargin = 32f;
    internal const float MinimumSafeMoveDistance = 36f;

    internal static IReadOnlyList<PointF> SafeOffsets { get; } = Array.AsReadOnly(
    [
        new PointF(0f, -64f),
        new PointF(66f, -48f),
        new PointF(82f, 0f),
        new PointF(68f, 50f),
        new PointF(0f, 68f),
        new PointF(-64f, 48f),
        new PointF(-78f, 0f),
        new PointF(-60f, -52f),
    ]);

    private readonly Random _random;
    private double _phaseElapsedSeconds;
    private PointF _currentSafeOffset;
    private PointF _targetSafeOffset;
    private PointF _sourceStagingOffset;
    private PointF _destinationOffset;
    private SizeF _viewportSize = new(1000f, 1000f);

    public LogicalThemeStateMachine(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        _random = random;
        _currentSafeOffset = PointF.Empty;
        _targetSafeOffset = PickNextSafeOffset(_currentSafeOffset, _random);
        ComputeStagingOffsets();

        EnterPhase(LogicalThemePhase.Calm, SampleDuration(_random, CalmMinDuration, CalmMaxDuration));
    }

    public LogicalThemePhase Phase { get; private set; }

    public TimeSpan CurrentPhaseDuration { get; private set; }

    public PointF CurrentSafeOffset => _currentSafeOffset;

    public PointF TargetSafeOffset => _targetSafeOffset;

    public PointF SourceStagingOffset => _sourceStagingOffset;

    public PointF DestinationOffset => _destinationOffset;

    public SizeF ViewportSize => _viewportSize;

    public int CompletedCycles { get; private set; }

    public LogicalThemeSnapshot Snapshot => CreateSnapshot();

    public LogicalThemeSnapshot Advance(TimeSpan delta)
    {
        double remainingSeconds = Math.Max(0d, delta.TotalSeconds);

        while (remainingSeconds > 0d)
        {
            double phaseDurationSeconds = CurrentPhaseDuration.TotalSeconds;
            double phaseRemainingSeconds = Math.Max(0d, phaseDurationSeconds - _phaseElapsedSeconds);

            if (phaseRemainingSeconds <= 1e-9)
            {
                TransitionToNextPhase();
                continue;
            }

            double consumed = Math.Min(remainingSeconds, phaseRemainingSeconds);
            _phaseElapsedSeconds += consumed;
            remainingSeconds -= consumed;

            if (_phaseElapsedSeconds >= phaseDurationSeconds - 1e-9)
            {
                TransitionToNextPhase();
            }
        }

        return CreateSnapshot();
    }

    public void SetViewport(SizeF viewportSize)
    {
        SizeF normalized = NormalizeViewport(viewportSize);
        if (MathF.Abs(normalized.Width - _viewportSize.Width) <= 0.01f
            && MathF.Abs(normalized.Height - _viewportSize.Height) <= 0.01f)
        {
            return;
        }

        _viewportSize = normalized;
        ComputeStagingOffsets();
    }

    internal static PointF PickNextSafeOffset(PointF current, Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        PointF[] eligible = new PointF[SafeOffsets.Count];
        int count = 0;

        for (int i = 0; i < SafeOffsets.Count; i++)
        {
            PointF candidate = SafeOffsets[i];
            if (Distance(current, candidate) >= MinimumSafeMoveDistance)
            {
                eligible[count++] = candidate;
            }
        }

        if (count == 0)
        {
            return SafeOffsets[random.Next(SafeOffsets.Count)];
        }

        return eligible[random.Next(count)];
    }

    internal static (PointF Source, PointF Destination) ComputeStagingOffsets(
        SizeF viewportSize,
        PointF targetSafeOffset)
    {
        SizeF normalized = NormalizeViewport(viewportSize);
        float clockRadius = ClockGeometryDesignRadius * ZoomedOutSceneScale;
        float x = MathF.Max(0f, normalized.Width / 2f - clockRadius - ViewportMargin);
        float y = MathF.Max(0f, normalized.Height / 2f - clockRadius - ViewportMargin);
        float destinationXSign = targetSafeOffset.X < 0f ? -1f : 1f;
        float destinationYSign = targetSafeOffset.Y < 0f ? -1f : 1f;
        PointF destination = new(destinationXSign * x, destinationYSign * y);
        return (Scale(destination, -1f), destination);
    }

    private const float ClockGeometryDesignRadius = 500f;

    private void ComputeStagingOffsets()
        => (_sourceStagingOffset, _destinationOffset) =
            ComputeStagingOffsets(_viewportSize, _targetSafeOffset);

    private LogicalThemeSnapshot CreateSnapshot()
    {
        float progress = CurrentPhaseDuration <= TimeSpan.Zero
            ? 1f
            : Math.Clamp((float)(_phaseElapsedSeconds / CurrentPhaseDuration.TotalSeconds), 0f, 1f);

        float eased = EaseInOut(progress);

        return Phase switch
        {
            LogicalThemePhase.Calm => new LogicalThemeSnapshot(
                Phase,
                CurrentPhaseDuration,
                TimeSpan.FromSeconds(_phaseElapsedSeconds),
                progress,
                _currentSafeOffset,
                _targetSafeOffset,
                _sourceStagingOffset,
                _destinationOffset,
                _viewportSize,
                _currentSafeOffset,
                BaseSceneScale,
                StormIntensity: 0f,
                FlashIntensity: 0f,
                CompletedCycles),

            LogicalThemePhase.Escalating => new LogicalThemeSnapshot(
                Phase,
                CurrentPhaseDuration,
                TimeSpan.FromSeconds(_phaseElapsedSeconds),
                progress,
                _currentSafeOffset,
                _targetSafeOffset,
                _sourceStagingOffset,
                _destinationOffset,
                _viewportSize,
                _currentSafeOffset,
                BaseSceneScale,
                StormIntensity: eased,
                FlashIntensity: eased,
                CompletedCycles),

            LogicalThemePhase.ZoomingOut => new LogicalThemeSnapshot(
                Phase,
                CurrentPhaseDuration,
                TimeSpan.FromSeconds(_phaseElapsedSeconds),
                progress,
                _currentSafeOffset,
                _targetSafeOffset,
                _sourceStagingOffset,
                _destinationOffset,
                _viewportSize,
                Lerp(_currentSafeOffset, _sourceStagingOffset, eased),
                Lerp(BaseSceneScale, ZoomedOutSceneScale, eased),
                StormIntensity: 1f,
                FlashIntensity: 1f,
                CompletedCycles),

            LogicalThemePhase.FlyingOff => new LogicalThemeSnapshot(
                Phase,
                CurrentPhaseDuration,
                TimeSpan.FromSeconds(_phaseElapsedSeconds),
                progress,
                _currentSafeOffset,
                _targetSafeOffset,
                _sourceStagingOffset,
                _destinationOffset,
                _viewportSize,
                _sourceStagingOffset,
                ZoomedOutSceneScale,
                StormIntensity: 1f,
                FlashIntensity: 1f,
                CompletedCycles),

            LogicalThemePhase.Reassembling => new LogicalThemeSnapshot(
                Phase,
                CurrentPhaseDuration,
                TimeSpan.FromSeconds(_phaseElapsedSeconds),
                progress,
                _currentSafeOffset,
                _targetSafeOffset,
                _sourceStagingOffset,
                _destinationOffset,
                _viewportSize,
                _destinationOffset,
                ZoomedOutSceneScale,
                StormIntensity: 0.85f - (0.20f * progress),
                FlashIntensity: 0.70f - (0.35f * progress),
                CompletedCycles),

            _ => new LogicalThemeSnapshot(
                Phase,
                CurrentPhaseDuration,
                TimeSpan.FromSeconds(_phaseElapsedSeconds),
                progress,
                _currentSafeOffset,
                _targetSafeOffset,
                _sourceStagingOffset,
                _destinationOffset,
                _viewportSize,
                Lerp(_destinationOffset, _targetSafeOffset, eased),
                Lerp(ZoomedOutSceneScale, BaseSceneScale, eased),
                StormIntensity: 0.25f * (1f - eased),
                FlashIntensity: 0.20f * (1f - eased),
                CompletedCycles),
        };
    }

    private void TransitionToNextPhase()
    {
        switch (Phase)
        {
            case LogicalThemePhase.Calm:
                EnterPhase(LogicalThemePhase.Escalating, SampleDuration(_random, EscalationMinDuration, EscalationMaxDuration));
                break;

            case LogicalThemePhase.Escalating:
                EnterPhase(LogicalThemePhase.ZoomingOut, ZoomOutDuration);
                break;

            case LogicalThemePhase.ZoomingOut:
                EnterPhase(LogicalThemePhase.FlyingOff, FlyOutDuration);
                break;

            case LogicalThemePhase.FlyingOff:
                EnterPhase(LogicalThemePhase.Reassembling, ReassemblyDuration);
                break;

            case LogicalThemePhase.Reassembling:
                EnterPhase(LogicalThemePhase.ZoomingIn, ZoomInDuration);
                break;

            default:
                _currentSafeOffset = _targetSafeOffset;
                CompletedCycles++;
                _targetSafeOffset = PickNextSafeOffset(_currentSafeOffset, _random);
                ComputeStagingOffsets();
                EnterPhase(LogicalThemePhase.Calm, SampleDuration(_random, CalmMinDuration, CalmMaxDuration));
                break;
        }
    }

    private void EnterPhase(LogicalThemePhase phase, TimeSpan duration)
    {
        Phase = phase;
        CurrentPhaseDuration = duration;
        _phaseElapsedSeconds = 0d;
    }

    private static TimeSpan SampleDuration(Random random, TimeSpan min, TimeSpan max)
    {
        long deltaTicks = max.Ticks - min.Ticks;
        long sampledTicks = min.Ticks + (long)Math.Round(random.NextDouble() * deltaTicks);
        return TimeSpan.FromTicks(sampledTicks);
    }

    private static float Distance(PointF a, PointF b)
        => MathF.Sqrt(DistanceSquared(a, b));

    private static float DistanceSquared(PointF a, PointF b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return (dx * dx) + (dy * dy);
    }

    private static PointF Lerp(PointF from, PointF to, float progress)
        => new(
            from.X + ((to.X - from.X) * progress),
            from.Y + ((to.Y - from.Y) * progress));

    private static float Lerp(float from, float to, float progress)
        => from + ((to - from) * progress);

    private static PointF Scale(PointF point, float factor)
        => new(point.X * factor, point.Y * factor);

    internal static SizeF NormalizeViewport(SizeF surfaceSize)
    {
        float width = MathF.Max(1f, surfaceSize.Width);
        float height = MathF.Max(1f, surfaceSize.Height);
        float designScale = 1000f / MathF.Min(width, height);
        return new SizeF(width * designScale, height * designScale);
    }

    private static float EaseInOut(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return value < 0.5f
            ? 4f * value * value * value
            : 1f - (MathF.Pow(-2f * value + 2f, 3f) / 2f);
    }

    private static float EaseOut(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return 1f - MathF.Pow(1f - value, 3f);
    }
}

internal sealed class LogicalThemeAnimator : IThemeAnimator
{
    private const int FaceMoverIndex = 0;
    private const int HourHandMoverIndex = 13;
    private const int MinuteHandMoverIndex = 14;
    private const int SecondHandMoverIndex = 15;
    private const int ArbourMoverIndex = 16;
    private const int MoverCount = 17;

    private const float MaxNumeralJitter = 6.8f;
    private const float MaxHandJitter = 4.2f;
    private const float MaxArbourJitter = 2.6f;
    private const float MaxNumeralRock = 8.0f;
    private const float MaxHandRock = 4.0f;
    private const float MaxArbourRock = 4.8f;
    private const float MaxNumeralSkew = 4.8f;
    private const float MaxHandSkew = 2.0f;
    private const float MaxArbourSkew = 1.4f;

    private readonly LogicalThemePalette _palette;
    private readonly LogicalThemeStateMachine _stateMachine;
    private double _motionSeconds;

    public LogicalThemeAnimator(LogicalThemePalette palette)
        : this(palette, new Random(Random.Shared.Next()))
    {
    }

    internal LogicalThemeAnimator(LogicalThemePalette palette, Random random)
    {
        ArgumentNullException.ThrowIfNull(palette);
        ArgumentNullException.ThrowIfNull(random);

        _palette = palette;
        _stateMachine = new LogicalThemeStateMachine(random);
    }

    public void Initialize(IClockTickContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _motionSeconds = 0d;
        _stateMachine.SetViewport(LogicalThemeStateMachine.NormalizeViewport(context.SurfaceSize));
        ApplySnapshot(context, _stateMachine.Snapshot);
    }

    public void OnTick(IClockTickContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _motionSeconds += Math.Max(0d, context.FrameDelta.TotalSeconds);
        _stateMachine.SetViewport(LogicalThemeStateMachine.NormalizeViewport(context.SurfaceSize));
        ApplySnapshot(context, _stateMachine.Advance(context.FrameDelta));
    }

    private void ApplySnapshot(IClockTickContext context, LogicalThemeSnapshot snapshot)
    {
        context.FaceRotationDegrees = 0f;

        foreach (ClockElementDescriptor element in context.Elements)
        {
            ApplyElement(context, snapshot, element, context.GetParameters(element.Id));
        }
    }

    private void ApplyElement(
        IClockTickContext context,
        LogicalThemeSnapshot snapshot,
        ClockElementDescriptor element,
        ClockElementParameters parameters)
    {
        PointF homeVector = GetHomeVector(element.Id);
        PointF anchorOffset = Add(snapshot.SceneOffset, Scale(homeVector, snapshot.SceneScale - 1f));
        float scale = snapshot.SceneScale;
        float skewDegrees = 0f;
        float extraRotationDegrees = 0f;
        float opacity = 1f;
        float progress = 0f;
        bool needsRedraw = false;

        int moverIndex = GetMoverIndex(element.Id);
        if (moverIndex >= 0)
        {
            float storm = snapshot.StormIntensity * _palette.MotionCeiling;
            float seed = moverIndex + 1;
            float waveA = MathF.Sin((float)(_motionSeconds * 7.4d + (seed * 0.91d)));
            float waveB = MathF.Cos((float)(_motionSeconds * 5.9d + (seed * 1.37d)));
            float pulse = 0.55f + (0.45f * MathF.Sin((float)(_motionSeconds * 9.7d + (seed * 0.63d))));

            (float maxJitter, float maxRock, float maxSkew) = element.Id.Kind switch
            {
                ClockElementKind.Arbour => (MaxArbourJitter, MaxArbourRock, MaxArbourSkew),
                _ when element.IsHand => (MaxHandJitter, MaxHandRock, MaxHandSkew),
                _ => (MaxNumeralJitter, MaxNumeralRock, MaxNumeralSkew),
            };

            float jitterMagnitude = storm * maxJitter * (0.35f + (0.65f * pulse));
            anchorOffset = Add(anchorOffset, new PointF(waveA * jitterMagnitude, waveB * jitterMagnitude * 0.72f));
            extraRotationDegrees = storm * maxRock * MathF.Sin((float)(_motionSeconds * 6.2d + (seed * 0.82d)));
            skewDegrees = storm * maxSkew * MathF.Cos((float)(_motionSeconds * 4.6d + (seed * 1.11d)));
            scale *= 1f + (storm * 0.03f * MathF.Sin((float)(_motionSeconds * 4d + (seed * 0.48d))));

            progress = snapshot.FlashIntensity;
            needsRedraw = snapshot.FlashIntensity > 0.001f
                || snapshot.Phase is LogicalThemePhase.ZoomingOut or LogicalThemePhase.FlyingOff or LogicalThemePhase.Reassembling or LogicalThemePhase.ZoomingIn;

            ApplyTravelMotion(snapshot, moverIndex, element, ref anchorOffset, ref opacity, ref scale);
        }

        parameters.Visible = true;
        parameters.Visibility = ClockNumeralVisibility.Visible;
        SetParameters(parameters, anchorOffset, scale, skewDegrees, extraRotationDegrees, opacity, progress, needsRedraw);
    }

    private void ApplyTravelMotion(
        LogicalThemeSnapshot snapshot,
        int moverIndex,
        ClockElementDescriptor element,
        ref PointF anchorOffset,
        ref float opacity,
        ref float scale)
    {
        if (snapshot.Phase is not LogicalThemePhase.FlyingOff and not LogicalThemePhase.Reassembling)
        {
            return;
        }

        if (snapshot.Phase == LogicalThemePhase.Reassembling)
        {
            float settle = EaseOut(snapshot.PhaseProgress);
            opacity = 0.80f + (0.20f * settle);
            scale *= 0.96f + (0.04f * settle);
        }
        else
        {
            float orderedProgress = ComputeOrderedProgress(snapshot.PhaseProgress, moverIndex);
            float travel = EaseIn(orderedProgress);
            PointF homeVector = GetHomeVector(element.Id);
            PointF sourceOffset = Add(
                snapshot.SourceStagingOffset,
                Scale(homeVector, snapshot.SceneScale - 1f));
            PointF jitter = Subtract(anchorOffset, sourceOffset);
            anchorOffset = Add(
                SampleFlightAnchorOffset(element, snapshot, moverIndex, travel),
                jitter);
            opacity = 0.78f + (0.22f * MathF.Abs((2f * travel) - 1f));
            scale *= 1f - (0.06f * MathF.Sin(travel * MathF.PI));
        }
    }

    private static float ComputeOrderedProgress(float phaseProgress, int moverIndex)
    {
        const float delaySpan = 0.52f;
        float delay = MoverCount <= 1
            ? 0f
            : (moverIndex / (float)(MoverCount - 1)) * delaySpan;

        return Math.Clamp((phaseProgress - delay) / (1f - delay), 0f, 1f);
    }

    internal static PointF SampleFlightAnchorOffset(
        ClockElementDescriptor element,
        LogicalThemeSnapshot snapshot,
        int moverIndex,
        float progress)
    {
        PointF homeVector = GetHomeVector(element.Id);
        PointF sourceWorld = Add(snapshot.SourceStagingOffset, Scale(homeVector, snapshot.SceneScale));
        PointF destinationWorld = Add(snapshot.DestinationOffset, Scale(homeVector, snapshot.SceneScale));
        PointF route = Subtract(destinationWorld, sourceWorld);
        float routeLength = MathF.Sqrt(LengthSquared(route));
        PointF perpendicular = routeLength <= 0.001f
            ? PointF.Empty
            : new PointF(-route.Y / routeLength, route.X / routeLength);

        uint hash = unchecked(
            ((uint)(moverIndex + 1) * 2654435761u)
            ^ ((uint)(snapshot.CompletedCycles + 1) * 2246822519u));
        float variation = 0.70f + (((hash >> 8) & 0xff) / 255f * 0.30f);
        float direction = (hash & 1) == 0 ? -1f : 1f;
        float bend = MathF.Min(routeLength * 0.22f, 180f) * variation * direction;

        PointF control1 = Add(Lerp(sourceWorld, destinationWorld, 0.26f), Scale(perpendicular, bend));
        PointF control2 = Add(Lerp(sourceWorld, destinationWorld, 0.72f), Scale(perpendicular, -bend * 0.58f));
        control1 = ClampWorldAnchor(control1, element, snapshot);
        control2 = ClampWorldAnchor(control2, element, snapshot);

        PointF worldAnchor = CubicBezier(
            sourceWorld,
            control1,
            control2,
            destinationWorld,
            Math.Clamp(progress, 0f, 1f));

        return Subtract(worldAnchor, homeVector);
    }

    private static PointF ClampWorldAnchor(
        PointF point,
        ClockElementDescriptor element,
        LogicalThemeSnapshot snapshot)
    {
        (float horizontalExtent, float verticalExtent) = GetSafeElementExtents(element, snapshot.SceneScale);
        float halfWidth = snapshot.ViewportSize.Width / 2f;
        float halfHeight = snapshot.ViewportSize.Height / 2f;
        return new PointF(
            Math.Clamp(point.X, -halfWidth + horizontalExtent, halfWidth - horizontalExtent),
            Math.Clamp(point.Y, -halfHeight + verticalExtent, halfHeight - verticalExtent));
    }

    internal static (float Horizontal, float Vertical) GetSafeElementExtents(
        ClockElementDescriptor element,
        float scale)
    {
        const float motionPadding = 12f;
        float left = element.Pivot.X;
        float right = element.ContentSize.Width - element.Pivot.X;
        float top = element.Pivot.Y;
        float bottom = element.ContentSize.Height - element.Pivot.Y;

        if (element.IsHand)
        {
            float maxHorizontal = MathF.Max(left, right);
            float maxVertical = MathF.Max(top, bottom);
            float radius = MathF.Sqrt(
                (maxHorizontal * maxHorizontal) + (maxVertical * maxVertical));
            float extent = radius * scale + motionPadding;
            return (extent, extent);
        }

        if (element.Id.Kind == ClockElementKind.HourMarker)
        {
            float radius = MathF.Sqrt(
                MathF.Max(left, right) * MathF.Max(left, right)
                + MathF.Max(top, bottom) * MathF.Max(top, bottom));
            float extent = radius * scale + motionPadding;
            return (extent, extent);
        }

        return (
            MathF.Max(left, right) * scale + motionPadding,
            MathF.Max(top, bottom) * scale + motionPadding);
    }

    private static PointF CubicBezier(
        PointF start,
        PointF control1,
        PointF control2,
        PointF end,
        float progress)
    {
        float remaining = 1f - progress;
        float startWeight = remaining * remaining * remaining;
        float control1Weight = 3f * remaining * remaining * progress;
        float control2Weight = 3f * remaining * progress * progress;
        float endWeight = progress * progress * progress;
        return new PointF(
            start.X * startWeight
                + control1.X * control1Weight
                + control2.X * control2Weight
                + end.X * endWeight,
            start.Y * startWeight
                + control1.Y * control1Weight
                + control2.Y * control2Weight
                + end.Y * endWeight);
    }

    internal static int GetMoverIndex(ClockElementId id)
        => id.Kind switch
        {
            ClockElementKind.Face => FaceMoverIndex,
            ClockElementKind.HourMarker => NormalizeIndex(id.Index, 12) + 1,
            ClockElementKind.HourHand => HourHandMoverIndex,
            ClockElementKind.MinuteHand => MinuteHandMoverIndex,
            ClockElementKind.SecondHand => SecondHandMoverIndex,
            ClockElementKind.Arbour => ArbourMoverIndex,
            _ => -1,
        };

    internal static PointF GetHomeVector(ClockElementId id)
        => id.Kind == ClockElementKind.HourMarker
            ? Polar(LogicalTheme.HourMarkerRadius, NormalizeIndex(id.Index, 12) * 30f)
            : PointF.Empty;

    private static int NormalizeIndex(int index, int count)
    {
        index %= count;
        return index < 0 ? index + count : index;
    }

    private static PointF Polar(float radius, float angleDegrees)
    {
        float radians = angleDegrees * (MathF.PI / 180f);
        return new PointF(MathF.Sin(radians) * radius, -MathF.Cos(radians) * radius);
    }

    private static void SetParameters(
        ClockElementParameters parameters,
        PointF anchorOffset,
        float scale,
        float skewDegrees,
        float extraRotationDegrees,
        float opacity,
        float progress,
        bool needsRedraw)
    {
        bool redraw = needsRedraw;

        if (!NearlyEqual(parameters.AnchorOffset, anchorOffset))
        {
            parameters.AnchorOffset = anchorOffset;
        }

        scale = Math.Max(0.10f, scale);
        if (!NearlyEqual(parameters.Scale, scale))
        {
            parameters.Scale = scale;
        }

        if (!NearlyEqual(parameters.SkewDegrees, skewDegrees))
        {
            parameters.SkewDegrees = skewDegrees;
        }

        if (!NearlyEqual(parameters.ExtraRotationDegrees, extraRotationDegrees))
        {
            parameters.ExtraRotationDegrees = extraRotationDegrees;
        }

        opacity = Math.Clamp(opacity, 0f, 1f);
        if (!NearlyEqual(parameters.Opacity, opacity))
        {
            parameters.Opacity = opacity;
            redraw = true;
        }

        progress = Math.Clamp(progress, 0f, 1f);
        if (!NearlyEqual(parameters.Progress, progress))
        {
            parameters.Progress = progress;
            redraw = true;
        }

        parameters.RedrawRequested = redraw;
    }

    private static float EaseIn(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return value * value * value;
    }

    private static float EaseOut(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return 1f - MathF.Pow(1f - value, 3f);
    }

    private static bool NearlyEqual(float left, float right)
        => MathF.Abs(left - right) <= 0.001f;

    private static bool NearlyEqual(PointF left, PointF right)
        => MathF.Abs(left.X - right.X) <= 0.001f && MathF.Abs(left.Y - right.Y) <= 0.001f;

    private static PointF Add(PointF left, PointF right)
        => new(left.X + right.X, left.Y + right.Y);

    private static PointF Lerp(PointF from, PointF to, float progress)
        => new(
            from.X + ((to.X - from.X) * progress),
            from.Y + ((to.Y - from.Y) * progress));

    private static PointF Subtract(PointF left, PointF right)
        => new(left.X - right.X, left.Y - right.Y);

    private static PointF Scale(PointF point, float factor)
        => new(point.X * factor, point.Y * factor);

    private static float LengthSquared(PointF point)
        => (point.X * point.X) + (point.Y * point.Y);
}
