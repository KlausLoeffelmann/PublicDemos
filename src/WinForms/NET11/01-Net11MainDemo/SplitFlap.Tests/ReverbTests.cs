using SplitFlap.Audio.Synthesis;

namespace SplitFlap.Tests;

public sealed class ReverbTests
{
    [Fact]
    public void Off_LeavesDryOutputUntouched()
    {
        Reverb reverb = new(48_000);
        float[] wet = [1, 0, 0, 0];
        float[] dry = [0.25f, -0.25f, 0.5f, 0];

        reverb.Process(ReverbSettings.Off, wet, dry);

        Assert.Equal([0.25f, -0.25f, 0.5f, 0], dry);
    }

    [Fact]
    public void Hall_ContinuesItsTailAfterInputStops()
    {
        Reverb reverb = new(48_000);
        float[] wet = new float[960];
        float[] output = new float[960];
        wet[0] = 1;
        reverb.Process(ReverbSettings.Hall, wet, output);
        Array.Clear(wet);
        Array.Clear(output);

        reverb.Process(ReverbSettings.Hall, wet, output);

        Assert.Contains(output, sample => sample != 0);
        Assert.All(output, sample => Assert.True(float.IsFinite(sample)));
    }

    [Fact]
    public void ChunkBoundaries_DoNotChangeTheReverb()
    {
        float[] wet = new float[9_600];
        float[] whole = new float[wet.Length];
        float[] chunked = new float[wet.Length];
        wet[0] = 1;
        wet[1_777] = -0.25f;
        new Reverb(48_000).Process(ReverbSettings.Hall, wet, whole);
        Reverb reverb = new(48_000);

        for (int offset = 0; offset < wet.Length; offset += 251)
        {
            int length = Math.Min(251, wet.Length - offset);
            reverb.Process(
                ReverbSettings.Hall,
                wet.AsSpan(offset, length),
                chunked.AsSpan(offset, length));
        }

        Assert.Equal(whole, chunked);
    }

    [Theory]
    [InlineData(0.45f)]
    [InlineData(0.8f)]
    public void LongTail_EventuallyReachesExactSilence(float decay)
    {
        Reverb reverb = new(48_000);
        ReverbSettings settings = new(0.35f, decay);
        float[] wet = new float[960];
        float[] output = new float[960];
        wet[0] = 1;
        reverb.Process(settings, wet, output);
        Array.Clear(wet);

        // Simulate a long timetable interval without sleeps or a timing assertion.
        // Before flushing tiny feedback values, these tails kept circulating subnormal floats.
        for (int block = 0; block < 3_200; block++)
        {
            Array.Clear(output);
            reverb.Process(settings, wet, output);
        }

        Assert.All(output, sample => Assert.Equal(0f, sample));
    }
}
