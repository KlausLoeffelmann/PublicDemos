using WinForms.Audio.Core;
using WinForms.Audio.Synthesis;

namespace SplitFlap.Tests;

public sealed class SynthesisTests
{
    [Theory]
    [InlineData(48_000, 7_000f, 1_400f)]
    [InlineData(44_100, 20_000f, 0f)]
    [InlineData(48_000, 24_000f, 700f)]
    public void Filter_CachedCoefficientsMatchReferenceAndFollowChanges(
        int sampleRate, float lowPass, float highPass)
    {
        OnePoleFilter filter = new(sampleRate) { LowPassHz = lowPass, HighPassHz = highPass };
        ReferenceFilter reference = new(sampleRate);
        NoiseSource noise = new(123);

        for (int i = 0; i < 2_000; i++)
        {
            if (i == 700)
            {
                filter.LowPassHz = lowPass = 12_000;
                filter.HighPassHz = highPass = 2_000;
            }
            else if (i == 1_300)
            {
                filter.LowPassHz = lowPass = sampleRate;
                filter.HighPassHz = highPass = 0;
            }

            float input = noise.Next();
            Assert.Equal(reference.Next(input, lowPass, highPass), filter.Next(input));
        }
    }

    [Fact]
    public void Filter_DefaultCutoffsArePrepared()
    {
        OnePoleFilter filter = new(48_000);
        ReferenceFilter reference = new(48_000);

        for (int i = 0; i < 100; i++)
        {
            float input = i == 0 ? 1f : 0f;
            Assert.Equal(reference.Next(input, 20_000, 0), filter.Next(input));
        }
    }

    [Theory]
    [InlineData(48_000)]
    [InlineData(44_100)]
    public void Oscillator_CachesPitchWithoutChangingPhase(int sampleRate)
    {
        Oscillator oscillator = new(sampleRate);
        double phase = 0;
        double frequency = 440;

        for (int i = 0; i < 5_000; i++)
        {
            if (i == 1_234)
            {
                oscillator.Frequency = frequency = 880;
            }
            else if (i == 3_333)
            {
                oscillator.Frequency = frequency = 55;
            }

            Assert.Equal(frequency, oscillator.Frequency);
            Assert.Equal((float)Math.Sin(phase * Math.Tau), oscillator.Next());
            phase += frequency / sampleRate;
            if (phase >= 1)
            {
                phase -= Math.Floor(phase);
            }
        }

        oscillator.Reset();
        Assert.Equal(0f, oscillator.Next());
    }

    [Theory]
    [InlineData(48_000, 1.5f, 0.85f)]
    [InlineData(48_000, 1.5f, 1.15f)]
    [InlineData(44_100, 1.5f, 1f)]
    [InlineData(48_000, 0f, 1f)]
    public void Clack_SeededOutputMatchesOriginalEquations(
        int sampleRate, float attackMilliseconds, float variance)
    {
        const uint seed = 12_345;
        const float volume = 0.25f;
        TimeSpan delay = TimeSpan.FromMilliseconds(5);
        ClackVoice voice = new(sampleRate, volume, delay, attackMilliseconds, seed, variance);
        ReferenceClack reference = new(sampleRate, volume, delay, attackMilliseconds, seed, variance);

        int samples = 0;
        while (!reference.IsFinished)
        {
            Assert.False(voice.IsFinished);
            Assert.Equal(reference.Next(), voice.Next());
            samples++;
            Assert.True(samples < sampleRate);
        }

        Assert.True(voice.IsFinished);
    }

    [Fact]
    public void Noise_SeedIsRepeatableAndCannotBeZero()
    {
        NoiseSource first = new(42);
        NoiseSource second = new(42);

        for (int i = 0; i < 1_000; i++)
        {
            Assert.Equal(first.Next(), second.Next());
        }

        Assert.Throws<ArgumentOutOfRangeException>(() => new NoiseSource(0));
    }

    // These deliberately retain the original per-sample equations, independently of the
    // optimized implementation. A cache must not change the sound or miss a parameter update.
    private sealed class ReferenceFilter(int sampleRate)
    {
        private float _low;
        private float _high;
        private float _lastInput;

        public float Next(float input, float lowPass, float highPass)
        {
            float hp = input;
            if (highPass > 0)
            {
                _high = Coefficient(highPass) * (_high + input - _lastInput);
                _lastInput = input;
                hp = _high;
            }

            if (lowPass < sampleRate / 2f)
            {
                _low += (1 - Coefficient(lowPass)) * (hp - _low);
                return _low;
            }

            return hp;
        }

        private float Coefficient(float hz)
        {
            float rc = 1f / (MathF.Tau * hz);
            float dt = 1f / sampleRate;
            return rc / (rc + dt);
        }
    }

    private sealed class ReferenceClack(
        int sampleRate,
        float volume,
        TimeSpan delay,
        float attackMilliseconds,
        uint seed,
        float variance) : IVoice
    {
        private readonly ReferenceFilter _filter = new(sampleRate);
        private readonly float _noiseDecay = MathF.Exp(-1f / (6f * variance * sampleRate / 1000f));
        private readonly float _tickDecay = MathF.Exp(-1f / (2.5f * sampleRate / 1000f));
        private readonly int _attackSamples =
            Math.Max(1, (int)Math.Round(Math.Max(0, attackMilliseconds) * sampleRate / 1000f));
        private int _delaySamples = Math.Max(0, (int)Math.Round(delay.TotalSeconds * sampleRate));
        private int _age;
        private uint _noiseState = seed;
        private double _phase;
        private float _noiseLevel = 1f;
        private float _tickLevel = 0.6f;

        public bool IsFinished => _delaySamples <= 0 && _noiseLevel < 0.001f;

        public float Next()
        {
            if (_delaySamples > 0)
            {
                _delaySamples--;
                return 0;
            }

            float attack = MathF.Sin(Math.Min(1f, ++_age / (float)_attackSamples) * MathF.PI / 2f);
            _noiseState ^= _noiseState << 13;
            _noiseState ^= _noiseState >> 17;
            _noiseState ^= _noiseState << 5;
            float input = (_noiseState / (float)uint.MaxValue) * 2f - 1f;
            float noise = _filter.Next(input, 7_000, 1_400 * variance) * _noiseLevel;
            float tick = (float)Math.Sin(_phase * Math.Tau) * _tickLevel;
            double frequency = 2_300 * variance;
            _phase += frequency / sampleRate;
            if (_phase >= 1)
            {
                _phase -= Math.Floor(_phase);
            }

            _noiseLevel *= _noiseDecay;
            _tickLevel *= _tickDecay;
            return (noise + tick) * (volume * variance) * attack;
        }

        public void Release()
        {
        }
    }
}
