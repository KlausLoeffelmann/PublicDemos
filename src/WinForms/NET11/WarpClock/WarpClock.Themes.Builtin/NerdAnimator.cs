using WarpClock.Abstractions;

namespace WarpClock.Themes.Builtin;

internal sealed class NerdAnimator : IThemeAnimator
{
    private const float DegreesPerSecond = 6f;
    private const float BeamDurationSeconds = 1.4f;
    private const float BackgroundCycleSeconds = 42f;

    private sealed class SlideState
    {
        public bool Active;
        public float Angle;
        public float NormalSpeed;
        public float FastSpeed;
        public float BeamProgress;
    }

    private readonly ClockHandMotion _secondHandMotion;
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
    private double _backgroundElapsed;
    private bool _fastPhase;
    private bool _retiring;
    private bool _soloRecovery;

    public NerdAnimator(
        ClockHandMotion secondHandMotion,
        int speedUpAfterMin,
        int fastDurationMin,
        int addSlideEveryMin,
        int soloRecoveryMin,
        int maximumSlides,
        float minimumFastMultiplier,
        float maximumFastMultiplier)
    {
        _secondHandMotion = secondHandMotion;
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
        primary.Angle = context.Time.SecondAngle;
        primary.NormalSpeed = 1f;
        primary.FastSpeed = NextDistinctFastSpeed(primary);
        primary.BeamProgress = 1f;
        Apply(context);
    }

    public void OnTick(IClockTickContext context)
    {
        float dt = (float)Math.Max(context.FrameDelta.TotalSeconds, 0d);
        UpdatePopulation(dt);
        UpdateSpeed(dt);
        _backgroundElapsed = (_backgroundElapsed + dt) % BackgroundCycleSeconds;

        foreach (SlideState slide in _slides.Where(slide => slide.Active))
        {
            float speed = _fastPhase ? slide.FastSpeed : slide.NormalSpeed;
            slide.Angle = ClockMathLikeNormalize(slide.Angle + (DegreesPerSecond * speed * dt));
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

        slide.Active = true;
        slide.Angle = (float)_random.NextDouble() * 360f;
        slide.NormalSpeed = NextDistinctNormalSpeed(slide);
        slide.FastSpeed = NextDistinctFastSpeed(slide);
        slide.BeamProgress = 0.001f;
    }

    private void UpdateSpeed(float dt)
    {
        double cycleDuration = _normalSeconds + _fastSeconds;
        bool wasFast = _fastPhase;
        _speedElapsed += dt;
        if (_speedElapsed >= cycleDuration)
        {
            _speedElapsed %= cycleDuration;
        }

        _fastPhase = _speedElapsed >= _normalSeconds;
        if (_fastPhase && !wasFast)
        {
            foreach (SlideState slide in _slides.Where(slide => slide.Active))
            {
                slide.FastSpeed = NextDistinctFastSpeed(slide);
            }
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
        context.GetParameters(ClockElementId.SecondHand).HandMotion = _secondHandMotion;
        context.GetParameters(ClockElementId.Face).Progress = (float)_backgroundElapsed;

        for (int i = 0; i < _slides.Length; i++)
        {
            SlideState state = _slides[i];
            ClockElementParameters parameters = context.GetParameters(ClockElementId.CustomElement(i));
            parameters.Visible = state.Active;
            parameters.ExtraRotationDegrees = state.Angle;

            float beam = state.BeamProgress < 0f ? 1f + state.BeamProgress : state.BeamProgress;
            parameters.Opacity = Math.Clamp(beam, 0f, 1f);
            parameters.Scale = 0.72f + (0.28f * Math.Clamp(beam, 0f, 1f));
        }
    }

    private float NextDistinctNormalSpeed(SlideState target)
        => NextDistinctSpeed(target, 0.82f, 1.24f);

    private float NextDistinctFastSpeed(SlideState target)
        => NextDistinctSpeed(target, _minimumFastMultiplier, _maximumFastMultiplier);

    private float NextDistinctSpeed(SlideState target, float minimum, float maximum)
    {
        float speed = minimum;
        for (int attempt = 0; attempt < 12; attempt++)
        {
            speed = minimum + ((float)_random.NextDouble() * (maximum - minimum));
            if (_slides.Where(slide => slide.Active && slide != target)
                .All(slide => MathF.Abs(
                    speed - (minimum >= 1.5f ? slide.FastSpeed : slide.NormalSpeed)) >= 0.08f))
            {
                break;
            }
        }

        return speed;
    }

    private static float ClockMathLikeNormalize(float degrees)
    {
        degrees %= 360f;
        return degrees < 0f ? degrees + 360f : degrees;
    }
}
