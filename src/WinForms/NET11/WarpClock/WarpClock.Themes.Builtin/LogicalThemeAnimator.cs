using System.Drawing;
using System.Globalization;

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

internal readonly record struct LogicalTravelWindow(float Start, float End);

internal enum LogicalLabelSlot
{
    Left,
    Middle,
    Right,
}

internal sealed class LogicalDetachPlan
{
    private readonly LogicalTravelWindow[] _windows;

    private LogicalDetachPlan(
        IReadOnlyList<int> firstWaveNumerals,
        IReadOnlyList<int> secondWaveNumerals,
        IReadOnlyList<int> thirdWaveMovers,
        IReadOnlyList<int> fourthWaveMovers,
        LogicalTravelWindow[] windows)
    {
        FirstWaveNumerals = firstWaveNumerals;
        SecondWaveNumerals = secondWaveNumerals;
        ThirdWaveMovers = thirdWaveMovers;
        FourthWaveMovers = fourthWaveMovers;
        _windows = windows;
    }

    public IReadOnlyList<int> FirstWaveNumerals { get; }

    public IReadOnlyList<int> SecondWaveNumerals { get; }

    public IReadOnlyList<int> ThirdWaveMovers { get; }

    public IReadOnlyList<int> FourthWaveMovers { get; }

    internal static LogicalDetachPlan Create(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        List<int> numerals = Enumerable.Range(1, 12).ToList();
        Shuffle(numerals, random);

        int firstCount = random.Next(1, 4);
        int secondCount = (int)Math.Round((12 - firstCount) / 2f, MidpointRounding.AwayFromZero);

        int[] first = numerals.Take(firstCount).ToArray();
        int[] second = numerals.Skip(firstCount).Take(secondCount).ToArray();
        int[] third =
        [
            .. numerals.Skip(firstCount + secondCount),
            LogicalThemeAnimator.HourHandMoverIndex,
            LogicalThemeAnimator.MinuteHandMoverIndex,
            LogicalThemeAnimator.SecondHandMoverIndex,
            LogicalThemeAnimator.ArbourMoverIndex,
        ];
        int[] fourth =
        [
            LogicalThemeAnimator.FaceMoverIndex,
            LogicalThemeAnimator.CaseMoverIndex,
        ];

        LogicalTravelWindow[] windows = new LogicalTravelWindow[LogicalThemeAnimator.MoverCount];
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i] = new LogicalTravelWindow(0f, 1f);
        }

        AssignWave(windows, first, launchStart: 0.00f, launchEnd: 0.10f, arrivalStart: 0.58f, arrivalEnd: 0.72f);
        AssignWave(windows, second, launchStart: 0.18f, launchEnd: 0.34f, arrivalStart: 0.68f, arrivalEnd: 0.82f);
        AssignWave(windows, third, launchStart: 0.40f, launchEnd: 0.60f, arrivalStart: 0.84f, arrivalEnd: 0.94f);
        AssignWave(windows, fourth, launchStart: 0.72f, launchEnd: 0.86f, arrivalStart: 0.98f, arrivalEnd: 1.00f);

        return new LogicalDetachPlan(first, second, third, fourth, windows);
    }

    internal LogicalTravelWindow GetWindow(int moverIndex)
        => moverIndex >= 0 && moverIndex < _windows.Length
            ? _windows[moverIndex]
            : new LogicalTravelWindow(0f, 1f);

    private static void AssignWave(
        LogicalTravelWindow[] windows,
        IReadOnlyList<int> movers,
        float launchStart,
        float launchEnd,
        float arrivalStart,
        float arrivalEnd)
    {
        for (int i = 0; i < movers.Count; i++)
        {
            float rank = movers.Count <= 1 ? 0.5f : i / (float)(movers.Count - 1);
            float start = Lerp(launchStart, launchEnd, rank);
            float end = Lerp(arrivalStart, arrivalEnd, rank);
            end = MathF.Max(end, start + 0.12f);
            windows[movers[i]] = new LogicalTravelWindow(start, MathF.Min(1f, end));
        }
    }

    private static void Shuffle(IList<int> values, Random random)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int swap = random.Next(i + 1);
            (values[i], values[swap]) = (values[swap], values[i]);
        }
    }

    private static float Lerp(float from, float to, float progress)
        => from + ((to - from) * progress);
}

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
    internal const float EscalationPanLead = 0.24f;
    internal const float EscalationZoomLead = 0.16f;

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
                Lerp(_currentSafeOffset, _sourceStagingOffset, EscalationPanLead * eased),
                Lerp(BaseSceneScale, ZoomedOutSceneScale, EscalationZoomLead * eased),
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
                Lerp(_currentSafeOffset, _sourceStagingOffset, EscalationPanLead + ((1f - EscalationPanLead) * eased)),
                Lerp(BaseSceneScale, ZoomedOutSceneScale, EscalationZoomLead + ((1f - EscalationZoomLead) * eased)),
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
                StormIntensity: 1f - (0.75f * eased),
                FlashIntensity: 1f - (0.80f * eased),
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
}

internal sealed class LogicalThemeAnimator : IThemeAnimator
{
    internal const int FaceMoverIndex = 0;
    internal const int HourHandMoverIndex = 13;
    internal const int MinuteHandMoverIndex = 14;
    internal const int SecondHandMoverIndex = 15;
    internal const int ArbourMoverIndex = 16;
    internal const int CaseMoverIndex = 17;
    internal const int MoverCount = 18;

    private const float MaxNumeralJitter = 6.8f;
    private const float MaxHandJitter = 4.2f;
    private const float MaxArbourJitter = 2.6f;
    private const float MaxNumeralRock = 8.0f;
    private const float MaxHandRock = 4.0f;
    private const float MaxArbourRock = 4.8f;
    private const float MaxNumeralSkew = 4.8f;
    private const float MaxHandSkew = 2.0f;
    private const float MaxArbourSkew = 1.4f;
    private const double LabelReturnDurationSeconds = 2.6d;

    private readonly LogicalThemePalette _palette;
    private readonly LogicalThemeStateMachine _stateMachine;
    private readonly Random _random;
    private double _motionSeconds;
    private double _labelReturnSeconds = LabelReturnDurationSeconds;
    private LogicalDetachPlan _detachPlan;
    private int[] _labelPermutation = [0, 1, 2];
    private readonly PointF[] _labelWorldAnchors = new PointF[3];
    private readonly PointF[] _labelReturnOrigins = new PointF[3];
    private SizeF _lastViewportSize;
    private bool _labelsPinnedToViewport = true;
    private int _observedCompletedCycles;

    public LogicalThemeAnimator(LogicalThemePalette palette)
        : this(palette, new Random(Random.Shared.Next()))
    {
    }

    internal LogicalThemeAnimator(LogicalThemePalette palette, Random random)
    {
        ArgumentNullException.ThrowIfNull(palette);
        ArgumentNullException.ThrowIfNull(random);

        _palette = palette;
        _random = random;
        _stateMachine = new LogicalThemeStateMachine(random);
        _detachPlan = LogicalDetachPlan.Create(random);
    }

    internal LogicalDetachPlan CurrentDetachPlan => _detachPlan;

    internal LogicalThemeSnapshot Snapshot => _stateMachine.Snapshot;

    internal double LabelReturnSeconds => _labelReturnSeconds;

    public void Initialize(IClockTickContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _motionSeconds = 0d;
        _labelPermutation = [0, 1, 2];
        _labelReturnSeconds = LabelReturnDurationSeconds;
        _observedCompletedCycles = 0;
        _labelsPinnedToViewport = true;
        _detachPlan = LogicalDetachPlan.Create(_random);
        SizeF viewport = LogicalThemeStateMachine.NormalizeViewport(context.SurfaceSize);
        _stateMachine.SetViewport(viewport);
        _lastViewportSize = _stateMachine.ViewportSize;
        InitializeLabelWorldAnchors(_stateMachine.Snapshot);
        ApplySnapshot(context, _stateMachine.Snapshot);
    }

    public void OnTick(IClockTickContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _motionSeconds += Math.Max(0d, context.FrameDelta.TotalSeconds);
        SizeF viewport = LogicalThemeStateMachine.NormalizeViewport(context.SurfaceSize);
        _stateMachine.SetViewport(viewport);

        LogicalThemeSnapshot beforeAdvance = _stateMachine.Snapshot;
        LogicalThemeSnapshot snapshot = _stateMachine.Advance(context.FrameDelta);
        if (!SameSize(viewport, _lastViewportSize))
        {
            HandleViewportChanged(snapshot);
            _lastViewportSize = snapshot.ViewportSize;
        }

        if (snapshot.CompletedCycles != _observedCompletedCycles)
        {
            // Use pre-transition staging: after Calm begins, Source/Destination already
            // describe the next cycle. Subtract the completed cycle's physical offset so
            // caption world anchors keep continuous screen positions.
            RebaseLabelWorldAnchorsAfterRecenter(beforeAdvance);
            _observedCompletedCycles = snapshot.CompletedCycles;
            Array.Copy(_labelWorldAnchors, _labelReturnOrigins, _labelWorldAnchors.Length);
            _labelPermutation = PickNextLabelPermutation(_random, _labelPermutation);
            _labelReturnSeconds = 0d;
            _labelsPinnedToViewport = false;
            _detachPlan = LogicalDetachPlan.Create(_random);
        }

        if (snapshot.Phase == LogicalThemePhase.Calm && _labelReturnSeconds < LabelReturnDurationSeconds)
        {
            _labelReturnSeconds = Math.Min(
                LabelReturnDurationSeconds,
                _labelReturnSeconds + Math.Max(0d, context.FrameDelta.TotalSeconds));

            if (_labelReturnSeconds >= LabelReturnDurationSeconds)
            {
                CommitLabelWorldTargets(snapshot);
                _labelsPinnedToViewport = true;
            }
        }

        ApplySnapshot(context, snapshot);
    }

    internal float GetTravelProgress(LogicalThemeSnapshot snapshot, int moverIndex)
    {
        float combined = ComputeCombinedTravelProgress(snapshot);
        LogicalTravelWindow window = _detachPlan.GetWindow(moverIndex);
        float raw = Math.Clamp((combined - window.Start) / MathF.Max(0.0001f, window.End - window.Start), 0f, 1f);
        return EaseInOut(raw);
    }

    internal LogicalLabelSlot GetAssignedLabelSlot(ClockElementId id)
        => (LogicalLabelSlot)_labelPermutation[GetLabelIndex(id)];

    internal PointF GetLabelViewportPosition(LogicalThemeSnapshot snapshot, ClockElementId id)
    {
        PointF worldAnchor = GetLabelSceneAnchor(snapshot, id);
        return Add(GetCameraOffset(snapshot), Scale(worldAnchor, snapshot.SceneScale));
    }

    internal PointF GetLabelSceneAnchor(LogicalThemeSnapshot snapshot, ClockElementId id)
    {
        PointF worldAnchor = _labelWorldAnchors[GetLabelIndex(id)];
        if (snapshot.Phase == LogicalThemePhase.Calm
            && _observedCompletedCycles > 0
            && _labelReturnSeconds < LabelReturnDurationSeconds)
        {
            PointF target = GetTargetLabelWorldAnchor(snapshot, id);
            float progress = EaseInOut((float)(_labelReturnSeconds / LabelReturnDurationSeconds));
            worldAnchor = Lerp(_labelReturnOrigins[GetLabelIndex(id)], target, progress);
            worldAnchor.Y -= MathF.Sin(progress * MathF.PI) * 82f;
        }

        return worldAnchor;
    }

    internal static string FormatWeekdayText(DateTime value)
        => value.ToString("dddd", CultureInfo.InvariantCulture);

    internal static string FormatLongDateText(DateTime value)
        => $"{value.ToString("MMMM", CultureInfo.InvariantCulture)}, {value.Day}{GetOrdinalSuffix(value.Day)}";

    internal static string? ComposeTimeZoneText(ClockAmbientSnapshot ambient)
    {
        string alias = ambient.TimeZoneAlias?.Trim() ?? string.Empty;
        string designation = ambient.TimeZoneDesignation?.Trim() ?? string.Empty;

        return (alias, designation) switch
        {
            ("", "") => null,
            (_, "") => alias,
            ("", _) => designation,
            _ when string.Equals(alias, designation, StringComparison.OrdinalIgnoreCase) => alias,
            _ => $"{alias} · {designation}",
        };
    }

    internal static int[] PickNextLabelPermutation(Random random, IReadOnlyList<int> current)
    {
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(current);

        // Weekday/date alternate exclusively between the two upper corners.
        int weekday = current[0] == (int)LogicalLabelSlot.Left
            ? (int)LogicalLabelSlot.Right
            : (int)LogicalLabelSlot.Left;
        int day = weekday == (int)LogicalLabelSlot.Left
            ? (int)LogicalLabelSlot.Right
            : (int)LogicalLabelSlot.Left;

        // Timezone may use left/center/right. Prefer center; otherwise a free corner.
        // When it shares a corner it is stacked on a second row by GetPaddedScreenTopAnchor.
        int[] timezoneCandidates =
        [
            (int)LogicalLabelSlot.Middle,
            weekday == (int)LogicalLabelSlot.Left ? (int)LogicalLabelSlot.Right : (int)LogicalLabelSlot.Left,
            weekday,
        ];
        int timeZone = timezoneCandidates[random.Next(timezoneCandidates.Length)];
        if (timeZone == current[1] && timezoneCandidates.Length > 1)
        {
            timeZone = timezoneCandidates[(Array.IndexOf(timezoneCandidates, timeZone) + 1) % timezoneCandidates.Length];
        }

        return [weekday, timeZone, day];
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
        PointF home = GetHomeVector(element.Id);
        float scale = snapshot.SceneScale;
        float skewDegrees = 0f;
        float extraRotationDegrees = 0f;
        float opacity = 1f;
        float progress = 0f;
        bool needsRedraw = false;
        bool visible = true;
        ClockNumeralVisibility visibility = ClockNumeralVisibility.Visible;
        string? text = null;
        PointF screenDesign;
        PointF jitter = PointF.Empty;

        int moverIndex = GetMoverIndex(element.Id);
        if (IsLabelElement(element.Id))
        {
            // Layout returns the pixel center; AnchorOffset is the full design-space
            // screen position so DesignScale is applied exactly once by the engine.
            screenDesign = GetLabelViewportPosition(snapshot, element.Id);
            ApplyLabelScenery(
                context,
                element.Id,
                ref opacity,
                ref progress,
                ref visible,
                ref text);
        }
        else if (moverIndex >= 0
            && snapshot.Phase is LogicalThemePhase.FlyingOff or LogicalThemePhase.Reassembling)
        {
            float travel = GetTravelProgress(snapshot, moverIndex);
            screenDesign = SampleFlightScreenPosition(element, snapshot, moverIndex, travel);
            float settle = EaseOut(travel);
            opacity = 0.76f + (0.24f * (0.35f + (0.65f * settle)));
            scale *= 0.94f + (0.06f * settle);
        }
        else
        {
            PointF world = home;
            if (moverIndex >= 0 && snapshot.Phase == LogicalThemePhase.ZoomingIn)
            {
                // Reconstructed dial stays rigid at the destination-relative world offset
                // while only the camera recenters.
                world = Add(GetPhysicalClockOffset(snapshot), home);
            }

            screenDesign = Add(GetCameraOffset(snapshot), Scale(world, scale));
        }

        if (moverIndex >= 0)
        {
            float activity = GetTravelActivity(snapshot, moverIndex);
            float storm = snapshot.StormIntensity * _palette.MotionCeiling * activity;
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
            jitter = new PointF(waveA * jitterMagnitude, waveB * jitterMagnitude * 0.72f);
            extraRotationDegrees = storm * maxRock * MathF.Sin((float)(_motionSeconds * 6.2d + (seed * 0.82d)));
            skewDegrees = storm * maxSkew * MathF.Cos((float)(_motionSeconds * 4.6d + (seed * 1.11d)));
            scale *= 1f + (storm * 0.03f * MathF.Sin((float)(_motionSeconds * 4d + (seed * 0.48d))));

            progress = snapshot.FlashIntensity * (0.28f + (0.72f * activity));
            needsRedraw = snapshot.FlashIntensity > 0.001f
                || snapshot.Phase is LogicalThemePhase.ZoomingOut
                    or LogicalThemePhase.FlyingOff
                    or LogicalThemePhase.Reassembling
                    or LogicalThemePhase.ZoomingIn;
        }

        PointF anchorOffset = IsLabelElement(element.Id)
            ? Add(screenDesign, jitter)
            : Add(Subtract(screenDesign, home), jitter);

        SetParameters(
            parameters,
            visible,
            visibility,
            text,
            anchorOffset,
            scale,
            skewDegrees,
            extraRotationDegrees,
            opacity,
            progress,
            needsRedraw);
    }

    private void ApplyLabelScenery(
        IClockTickContext context,
        ClockElementId id,
        ref float opacity,
        ref float progress,
        ref bool visible,
        ref string? text)
    {
        text = id.Kind switch
        {
            ClockElementKind.Weekday => FormatWeekdayText(context.Time.Now),
            ClockElementKind.Day => FormatLongDateText(context.Time.Now),
            ClockElementKind.TimeZone => ComposeTimeZoneText(context.Ambient),
            _ => null,
        };

        if (string.IsNullOrWhiteSpace(text))
        {
            visible = false;
            opacity = 0f;
            progress = 0f;
            return;
        }

        progress = 0f;
        visible = true;
        opacity = 1f;
    }

    internal static PointF SampleFlightScreenPosition(
        ClockElementDescriptor element,
        LogicalThemeSnapshot snapshot,
        int moverIndex,
        float progress)
    {
        PointF homeVector = GetHomeVector(element.Id);
        // Flight paths are authored in screen/design space while the camera is frozen
        // at SourceStagingOffset. Destination is the reconstructed dial corner.
        PointF sourceScreen = Add(snapshot.SourceStagingOffset, Scale(homeVector, LogicalThemeStateMachine.ZoomedOutSceneScale));
        PointF destinationScreen = Add(snapshot.DestinationOffset, Scale(homeVector, LogicalThemeStateMachine.ZoomedOutSceneScale));
        PointF route = Subtract(destinationScreen, sourceScreen);
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

        PointF control1 = Add(Lerp(sourceScreen, destinationScreen, 0.26f), Scale(perpendicular, bend));
        PointF control2 = Add(Lerp(sourceScreen, destinationScreen, 0.72f), Scale(perpendicular, -bend * 0.58f));
        control1 = ClampWorldAnchor(control1, element, snapshot);
        control2 = ClampWorldAnchor(control2, element, snapshot);

        return CubicBezier(
            sourceScreen,
            control1,
            control2,
            destinationScreen,
            Math.Clamp(progress, 0f, 1f));
    }

    /// <summary>Design-space AnchorOffset for a flight sample (screen − home).</summary>
    internal static PointF SampleFlightAnchorOffset(
        ClockElementDescriptor element,
        LogicalThemeSnapshot snapshot,
        int moverIndex,
        float progress)
        => Subtract(
            SampleFlightScreenPosition(element, snapshot, moverIndex, progress),
            GetHomeVector(element.Id));

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
            ClockElementKind.Case => CaseMoverIndex,
            _ => -1,
        };

    internal static PointF GetHomeVector(ClockElementId id)
        => id.Kind switch
        {
            ClockElementKind.HourMarker => Polar(LogicalTheme.HourMarkerRadius, NormalizeIndex(id.Index, 12) * 30f),
            // Captions are laid out from the pixel center; their design home is origin.
            ClockElementKind.Weekday or ClockElementKind.TimeZone or ClockElementKind.Day => PointF.Empty,
            _ => PointF.Empty,
        };

    internal static PointF GetLabelSlotVector(LogicalLabelSlot slot)
        => slot switch
        {
            LogicalLabelSlot.Left => new PointF(-350f, -470f),
            LogicalLabelSlot.Middle => new PointF(0f, -470f),
            _ => new PointF(350f, -470f),
        };

    internal static PointF GetLabelSlotVector(ClockElementId id, LogicalLabelSlot slot)
        => id.Kind == ClockElementKind.TimeZone
            ? slot switch
            {
                LogicalLabelSlot.Left => new PointF(-350f, -525f),
                LogicalLabelSlot.Middle => new PointF(0f, -525f),
                _ => new PointF(350f, -525f),
            }
            : GetLabelSlotVector(slot);

    internal static float ComputeCombinedTravelProgress(LogicalThemeSnapshot snapshot)
    {
        float flightShare = (float)(
            LogicalThemeStateMachine.FlyOutDuration.TotalSeconds
            / (LogicalThemeStateMachine.FlyOutDuration.TotalSeconds + LogicalThemeStateMachine.ReassemblyDuration.TotalSeconds));

        return snapshot.Phase switch
        {
            LogicalThemePhase.FlyingOff => snapshot.PhaseProgress * flightShare,
            LogicalThemePhase.Reassembling => flightShare + (snapshot.PhaseProgress * (1f - flightShare)),
            LogicalThemePhase.ZoomingIn or LogicalThemePhase.Calm when snapshot.CompletedCycles > 0 => 1f,
            _ => 0f,
        };
    }

    private float GetTravelActivity(LogicalThemeSnapshot snapshot, int moverIndex)
    {
        if (snapshot.Phase is LogicalThemePhase.FlyingOff or LogicalThemePhase.Reassembling)
        {
            float travel = GetTravelProgress(snapshot, moverIndex);
            return 0.04f + (0.96f * (1f - EaseOut(travel)));
        }

        if (snapshot.Phase == LogicalThemePhase.ZoomingIn)
        {
            return 0.04f * (1f - EaseInOut(snapshot.PhaseProgress));
        }

        return 1f;
    }

    private void InitializeLabelWorldAnchors(LogicalThemeSnapshot snapshot)
    {
        foreach (ClockElementId id in LabelIds)
        {
            int index = GetLabelIndex(id);
            _labelWorldAnchors[index] = GetTargetLabelWorldAnchor(snapshot, id);
            _labelReturnOrigins[index] = _labelWorldAnchors[index];
        }

        _labelsPinnedToViewport = true;
    }

    private void CommitLabelWorldTargets(LogicalThemeSnapshot snapshot)
    {
        foreach (ClockElementId id in LabelIds)
        {
            _labelWorldAnchors[GetLabelIndex(id)] = GetTargetLabelWorldAnchor(snapshot, id);
        }
    }

    private void RebaseLabelWorldAnchorsAfterRecenter(LogicalThemeSnapshot completedCycleSnapshot)
    {
        // completedCycleSnapshot must still carry the cycle's Source/Destination (i.e. the
        // pre-Calm-transition snapshot). Subtracting P maps world anchors into the rebased
        // clock origin without depending on the last rendered frame's PhaseProgress.
        PointF physical = GetPhysicalClockOffset(completedCycleSnapshot);

        for (int i = 0; i < _labelWorldAnchors.Length; i++)
        {
            _labelWorldAnchors[i] = Subtract(_labelWorldAnchors[i], physical);
        }
    }

    private void HandleViewportChanged(LogicalThemeSnapshot snapshot)
    {
        if (snapshot.Phase == LogicalThemePhase.Calm
            && (_labelsPinnedToViewport || _observedCompletedCycles == 0)
            && _labelReturnSeconds >= LabelReturnDurationSeconds)
        {
            InitializeLabelWorldAnchors(snapshot);
        }
    }

    /// <summary>
    ///  Camera translation in design units. Independent from the reconstructed clock's
    ///  world offset: during Reassembling the camera stays at the source staging corner
    ///  while parts occupy destination-relative screen positions.
    /// </summary>
    internal static PointF GetCameraOffset(LogicalThemeSnapshot snapshot)
    {
        if (snapshot.Phase is LogicalThemePhase.FlyingOff or LogicalThemePhase.Reassembling)
        {
            return snapshot.SourceStagingOffset;
        }

        if (snapshot.Phase is not LogicalThemePhase.ZoomingIn)
        {
            return snapshot.SceneOffset;
        }

        PointF physicalClockOffset = GetPhysicalClockOffset(snapshot);
        return Subtract(
            snapshot.SceneOffset,
            Scale(physicalClockOffset, snapshot.SceneScale));
    }

    internal static PointF GetPhysicalClockOffset(LogicalThemeSnapshot snapshot)
        => Scale(
            Subtract(snapshot.DestinationOffset, snapshot.SourceStagingOffset),
            1f / LogicalThemeStateMachine.ZoomedOutSceneScale);

    private PointF GetTargetLabelWorldAnchor(LogicalThemeSnapshot snapshot, ClockElementId id)
    {
        PointF screenAnchor = GetPaddedScreenTopAnchor(
            snapshot.ViewportSize,
            id,
            GetAssignedLabelSlot(id),
            _labelPermutation);
        PointF camera = GetCameraOffset(snapshot);
        return Scale(
            Subtract(screenAnchor, camera),
            1f / LogicalThemeStateMachine.BaseSceneScale);
    }

    internal static PointF GetPaddedScreenTopAnchor(
        SizeF viewport,
        ClockElementId id,
        LogicalLabelSlot slot)
        => GetPaddedScreenTopAnchor(viewport, id, slot, permutation: null);

    internal static PointF GetPaddedScreenTopAnchor(
        SizeF viewport,
        ClockElementId id,
        LogicalLabelSlot slot,
        IReadOnlyList<int>? permutation)
    {
        const float padding = 28f;
        const float rowGap = 10f;
        SizeF contentSize = GetLabelContentSize(id);
        float renderedWidth = contentSize.Width * LogicalThemeStateMachine.BaseSceneScale;
        float renderedHeight = contentSize.Height * LogicalThemeStateMachine.BaseSceneScale;
        float x = slot switch
        {
            LogicalLabelSlot.Left => (-viewport.Width / 2f) + padding + (renderedWidth / 2f),
            LogicalLabelSlot.Middle => 0f,
            _ => (viewport.Width / 2f) - padding - (renderedWidth / 2f),
        };

        int row = 0;
        if (id.Kind == ClockElementKind.TimeZone
            && slot is LogicalLabelSlot.Left or LogicalLabelSlot.Right
            && permutation is not null
            && (permutation[0] == (int)slot || permutation[2] == (int)slot))
        {
            // Stack timezone under the weekday/date that already owns this corner.
            row = 1;
        }

        float y = (-viewport.Height / 2f)
            + padding
            + (renderedHeight / 2f)
            + (row * (renderedHeight + rowGap));
        return new PointF(x, y);
    }

    internal static SizeF GetLabelContentSize(ClockElementId id)
        => id.Kind switch
        {
            ClockElementKind.Weekday => new SizeF(276f, 54f),
            ClockElementKind.Day => new SizeF(296f, 54f),
            ClockElementKind.TimeZone => new SizeF(320f, 46f),
            _ => throw new ArgumentOutOfRangeException(nameof(id)),
        };

    private static bool SameSize(SizeF left, SizeF right)
        => MathF.Abs(left.Width - right.Width) <= 0.01f
            && MathF.Abs(left.Height - right.Height) <= 0.01f;

    private static ClockElementId[] LabelIds { get; } =
        [ClockElementId.Weekday, ClockElementId.TimeZone, ClockElementId.Day];

    private static bool IsLabelElement(ClockElementId id)
        => id.Kind is ClockElementKind.Weekday or ClockElementKind.Day or ClockElementKind.TimeZone;

    private static int GetLabelIndex(ClockElementId id)
        => id.Kind switch
        {
            ClockElementKind.Weekday => 0,
            ClockElementKind.TimeZone => 1,
            ClockElementKind.Day => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(id)),
        };

    private static string GetOrdinalSuffix(int day)
        => day % 100 is 11 or 12 or 13
            ? "th"
            : (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };

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
        bool visible,
        ClockNumeralVisibility visibility,
        string? text,
        PointF anchorOffset,
        float scale,
        float skewDegrees,
        float extraRotationDegrees,
        float opacity,
        float progress,
        bool needsRedraw)
    {
        bool redraw = needsRedraw;

        if (parameters.Visible != visible)
        {
            parameters.Visible = visible;
        }

        if (parameters.Visibility != visibility)
        {
            parameters.Visibility = visibility;
        }

        if (!string.Equals(parameters.Text, text, StringComparison.Ordinal))
        {
            parameters.Text = text;
            redraw = true;
        }

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

    private static float EaseOut(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return 1f - MathF.Pow(1f - value, 3f);
    }

    private static float EaseInOut(float value)
    {
        value = Math.Clamp(value, 0f, 1f);
        return value < 0.5f
            ? 4f * value * value * value
            : 1f - (MathF.Pow(-2f * value + 2f, 3f) / 2f);
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
