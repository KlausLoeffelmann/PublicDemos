using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed class NerdAnimator : IThemeAnimator
{
    private const float DegreesPerSecond = 6f;
    private const float BeamDurationSeconds = 1.4f;
    private const float MinimumSpacingDegrees = 40f;

    private sealed class SlideState
    {
        public bool Active;
        public float PhaseOffset;
        public float BeamProgress;
    }

    private readonly NerdSlideMotion _motion;
    private readonly float _normalSeconds;
    private readonly float _fastSeconds;
    private readonly float _spawnIntervalSeconds;
    private readonly float _soloRecoverySeconds;
    private readonly int _maximumSlides;
    private readonly float _minimumFastMultiplier;
    private readonly float _maximumFastMultiplier;
    private readonly Random _random = new(0x4E455244);
    private readonly SlideState[] _slides = Enumerable.Range(0, 4).Select(_ => new SlideState()).ToArray();
    private float _spawnElapsed;
    private float _soloElapsed;
    private double _speedElapsed;
    private float _boostAngle;
    private float _boostTickAccumulator;
    private float _fastMultiplier;
    private bool _retiring;
    private bool _soloRecovery;

    public NerdAnimator(
        NerdSlideMotion motion,
        int speedUpAfterMin,
        int fastDurationMin,
        int addSlideEveryMin,
        int soloRecoveryMin,
        int maximumSlides,
        float minimumFastMultiplier,
        float maximumFastMultiplier)
    {
        _motion = motion;
        _normalSeconds = Math.Max(1, speedUpAfterMin) * 60f;
        _fastSeconds = Math.Max(1, fastDurationMin) * 60f;
        _spawnIntervalSeconds = Math.Max(1, addSlideEveryMin) * 60f;
        _soloRecoverySeconds = Math.Max(1, soloRecoveryMin) * 60f;
        _maximumSlides = Math.Clamp(maximumSlides, 1, 4);
        _minimumFastMultiplier = Math.Clamp(minimumFastMultiplier, 1.5f, 5f);
        _maximumFastMultiplier = Math.Clamp(maximumFastMultiplier, _minimumFastMultiplier, 5f);
    }

    public void Initialize(IClockTickContext context)
    {
        SlideState primary = _slides[0];
        primary.Active = true;
        primary.PhaseOffset = 0f;
        primary.BeamProgress = 1f;
        _fastMultiplier = NextFastMultiplier();
        Apply(context);
    }

    public void OnTick(IClockTickContext context)
    {
        float dt = (float)Math.Max(context.FrameDelta.TotalSeconds, 0d);
        UpdatePopulation(dt);
        UpdateSpeed(dt);

        foreach (SlideState slide in _slides.Where(slide => slide.Active))
        {
            UpdateBeam(slide, dt);
        }

        Apply(context);
    }

    private void UpdatePopulation(float dt)
    {
        if (_retiring)
        {
            if (_slides.Skip(1).All(slide => !slide.Active))
            {
                _retiring = false;
                _soloRecovery = true;
                _soloElapsed = 0f;
                _spawnElapsed = 0f;
            }

            return;
        }

        if (_soloRecovery)
        {
            _soloElapsed += dt;
            if (_soloElapsed >= _soloRecoverySeconds)
            {
                _soloRecovery = false;
                _spawnElapsed = 0f;
            }

            return;
        }

        if (_maximumSlides <= 1)
        {
            return;
        }

        _spawnElapsed += dt;
        if (_spawnElapsed < _spawnIntervalSeconds)
        {
            return;
        }

        _spawnElapsed = 0f;
        int activeCount = _slides.Count(slide => slide.Active);
        if (activeCount >= _maximumSlides)
        {
            _retiring = true;
            foreach (SlideState slide in _slides.Skip(1).Where(slide => slide.Active))
            {
                slide.BeamProgress = -0.001f;
            }

            return;
        }

        SpawnSlide();
    }

    private void SpawnSlide()
    {
        SlideState? slide = _slides.Skip(1).FirstOrDefault(candidate => !candidate.Active);
        if (slide is null)
        {
            return;
        }

        float phaseOffset = FindWidestGapMidpoint();
        if (_slides.Where(candidate => candidate.Active)
            .Any(candidate => AngularDistance(candidate.PhaseOffset, phaseOffset) < MinimumSpacingDegrees))
        {
            return;
        }

        slide.Active = true;
        slide.PhaseOffset = phaseOffset;
        slide.BeamProgress = 0.001f;
    }

    private void UpdateSpeed(float dt)
    {
        float cycleDuration = _normalSeconds + _fastSeconds;
        _speedElapsed += dt;
        if (_speedElapsed >= cycleDuration)
        {
            _speedElapsed %= cycleDuration;
            _fastMultiplier = NextFastMultiplier();
        }

        if (_speedElapsed < _normalSeconds)
        {
            return;
        }

        float additionalSpeed = _fastMultiplier - 1f;
        if (_motion == NerdSlideMotion.Glide)
        {
            _boostAngle = ClockMathLikeNormalize(
                _boostAngle + (DegreesPerSecond * additionalSpeed * dt));
            return;
        }

        _boostTickAccumulator += additionalSpeed * dt;
        int boostTicks = (int)MathF.Floor(_boostTickAccumulator);
        if (boostTicks > 0)
        {
            _boostTickAccumulator -= boostTicks;
            _boostAngle = ClockMathLikeNormalize(_boostAngle + (DegreesPerSecond * boostTicks));
        }
    }

    private void UpdateBeam(SlideState slide, float dt)
    {
        if (slide.BeamProgress < 0f)
        {
            slide.BeamProgress -= dt / BeamDurationSeconds;
            if (slide.BeamProgress <= -1f)
            {
                slide.Active = false;
                slide.BeamProgress = 0f;
            }
        }
        else if (slide.BeamProgress < 1f)
        {
            slide.BeamProgress = Math.Min(1f, slide.BeamProgress + (dt / BeamDurationSeconds));
        }
    }

    private void Apply(IClockTickContext context)
    {
        float baseAngle = _motion == NerdSlideMotion.Glide
            ? context.Time.SecondAngle
            : MathF.Floor(context.Time.SecondAngle / DegreesPerSecond) * DegreesPerSecond;

        for (int i = 0; i < _slides.Length; i++)
        {
            SlideState state = _slides[i];
            ClockElementParameters parameters = context.GetParameters(ClockElementId.CustomElement(i));
            parameters.Visible = state.Active;
            parameters.ExtraRotationDegrees = ClockMathLikeNormalize(
                baseAngle + _boostAngle + state.PhaseOffset);

            float beam = state.BeamProgress < 0f ? 1f + state.BeamProgress : state.BeamProgress;
            parameters.Opacity = Math.Clamp(beam, 0f, 1f);
            parameters.Scale = 0.72f + (0.28f * Math.Clamp(beam, 0f, 1f));
        }
    }

    private float NextFastMultiplier()
        => _minimumFastMultiplier
            + ((float)_random.NextDouble() * (_maximumFastMultiplier - _minimumFastMultiplier));

    private float FindWidestGapMidpoint()
    {
        float[] offsets = _slides
            .Where(slide => slide.Active)
            .Select(slide => ClockMathLikeNormalize(slide.PhaseOffset))
            .Order()
            .ToArray();

        float widestGap = -1f;
        float midpoint = 180f;
        for (int i = 0; i < offsets.Length; i++)
        {
            float start = offsets[i];
            float end = i == offsets.Length - 1 ? offsets[0] + 360f : offsets[i + 1];
            float gap = end - start;
            if (gap > widestGap)
            {
                widestGap = gap;
                midpoint = ClockMathLikeNormalize(start + (gap / 2f));
            }
        }

        return midpoint;
    }

    private static float AngularDistance(float left, float right)
    {
        float distance = MathF.Abs(ClockMathLikeNormalize(left - right));
        return MathF.Min(distance, 360f - distance);
    }

    private static float ClockMathLikeNormalize(float degrees)
    {
        degrees %= 360f;
        return degrees < 0f ? degrees + 360f : degrees;
    }
}
