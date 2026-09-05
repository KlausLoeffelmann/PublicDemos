namespace WinForms.Audio.Synthesis;

/// <summary>
///  Two knobs. That's the "simplest approach" you asked for.
/// </summary>
/// <param name="Mix">How much of the wet signal is added to the output, 0..1.</param>
/// <param name="Decay">How long the tail rings, 0..1.</param>
public readonly record struct ReverbSettings(float Mix = 0.25f, float Decay = 0.5f)
{
    /// <summary>
    ///  No reverb at all.
    /// </summary>
    public static ReverbSettings Off
        => new(0, 0);

    /// <summary>
    ///  A modest room.
    /// </summary>
    public static ReverbSettings Room
        => new(0.2f, 0.45f);

    /// <summary>
    ///  A departure hall. Long, big, echoey.
    /// </summary>
    public static ReverbSettings Hall
        => new(0.35f, 0.8f);
}

/// <summary>
///  Schroeder reverb: four parallel comb filters with damping into two serial all-pass filters.
///  Freeverb's grandfather, with the delay lengths scaled from 44.1 kHz to whatever we run at.
/// </summary>
public sealed class Reverb
{
    private const float SilenceThreshold = 1e-20f;
    private static readonly int[] s_combLengths = [1116, 1188, 1277, 1356];
    private static readonly int[] s_allPassLengths = [556, 441];

    private readonly Comb[] _combs;
    private readonly AllPass[] _allPasses;

    /// <summary>
    ///  Allocates the delay lines for a sample rate.
    /// </summary>
    public Reverb(int sampleRate)
    {
        double scale = sampleRate / 44_100.0;
        _combs = [.. s_combLengths.Select(len => new Comb((int)(len * scale)))];
        _allPasses = [.. s_allPassLengths.Select(len => new AllPass((int)(len * scale)))];
    }

    /// <summary>
    ///  Runs the wet bus through the reverb and adds the result to the output.
    /// </summary>
    public void Process(ReverbSettings settings, ReadOnlySpan<float> wet, Span<float> output)
    {
        if (settings.Mix <= 0f)
        {
            return;
        }

        float feedback = 0.7f + Math.Clamp(settings.Decay, 0f, 1f) * 0.28f;
        float mix = Math.Clamp(settings.Mix, 0f, 1f);

        for (int i = 0; i < wet.Length; i++)
        {
            float input = wet[i] * 0.4f;
            float acc = 0;

            foreach (Comb comb in _combs)
            {
                acc += comb.Next(input, feedback);
            }

            acc /= _combs.Length;

            foreach (AllPass allPass in _allPasses)
            {
                acc = allPass.Next(acc);
            }

            output[i] += acc * mix;
        }
    }

    // Feedback can retain tiny "subnormal" floats long after the tail is inaudible; some
    // processors handle those much more slowly. This floor is far below one 16-bit PCM step.
    // Clear only that residue, never a live tail just because there are no active voices.
    private static float FlushInaudible(float value)
        => MathF.Abs(value) < SilenceThreshold ? 0f : value;

    private sealed class Comb(int length)
    {
        private readonly float[] _buffer = new float[Math.Max(1, length)];
        private float _filterStore;
        private int _index;

        public float Next(float input, float feedback)
        {
            float output = _buffer[_index];
            _filterStore = FlushInaudible(output * 0.8f + _filterStore * 0.2f);
            _buffer[_index] = FlushInaudible(input + _filterStore * feedback);

            if (++_index >= _buffer.Length)
            {
                _index = 0;
            }

            return output;
        }
    }

    private sealed class AllPass(int length)
    {
        private readonly float[] _buffer = new float[Math.Max(1, length)];
        private int _index;

        public float Next(float input)
        {
            float buffered = _buffer[_index];
            float output = -input + buffered;
            _buffer[_index] = FlushInaudible(input + buffered * 0.5f);

            if (++_index >= _buffer.Length)
            {
                _index = 0;
            }

            return output;
        }
    }
}
