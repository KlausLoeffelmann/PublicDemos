using System.Buffers.Binary;
using SplitFlap.Audio.Playback;

namespace SplitFlap.Tests;

public sealed class WaveFileTests
{
    [Fact]
    public async Task WavRoundTrip_PreservesMonoPcm()
    {
        short[] pcm = [short.MinValue, -1000, 0, 1000, short.MaxValue];
        byte[] wav = WaveFile.ToWavBytes(pcm, 48_000);
        await using MemoryStream stream = new(wav);

        Sample sample = await Sample.FromWaveStreamAsync(
            stream,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(pcm.Length, sample.Data.Length);
        Assert.Equal(-1f, sample.Data[0], precision: 3);
        Assert.Equal(0f, sample.Data[2], precision: 3);
        Assert.Equal(1f, sample.Data[4], precision: 3);
    }

    [Fact]
    public void ToWavBytes_WritesConsistentHeader()
    {
        byte[] wav = WaveFile.ToWavBytes([1, 2, 3, 4], 22_050, channels: 2);

        Assert.Equal("RIFF", System.Text.Encoding.ASCII.GetString(wav, 0, 4));
        Assert.Equal(2, BinaryPrimitives.ReadInt16LittleEndian(wav.AsSpan(22, 2)));
        Assert.Equal(22_050, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(24, 4)));
        Assert.Equal(8, BinaryPrimitives.ReadInt32LittleEndian(wav.AsSpan(40, 4)));
    }

    [Fact]
    public void FromPcm_FoldsStereoAndResamples()
    {
        Sample sample = Sample.FromPcm(
            [short.MaxValue, short.MaxValue, 0, 0],
            sourceRate: 24_000,
            channels: 2,
            targetRate: 48_000);

        Assert.Equal(4, sample.Data.Length);
        Assert.InRange(sample.Data[0], 0.99f, 1.01f);
    }
}
