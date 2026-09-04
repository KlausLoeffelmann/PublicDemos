using SplitFlap.Audio.Core;
using SplitFlap.Audio.Playback;

namespace SplitFlap.Tests;

public sealed class AudioFormatTests
{
    [Fact]
    public void Default_UsesSupportedMono48KhzFormat()
    {
        AudioFormat format = AudioFormat.Default;

        Assert.Equal(48_000, format.SampleRate);
        Assert.Equal(1, format.Channels);
        Assert.Equal(2, format.BlockAlign);
        Assert.Equal(96_000, format.BytesPerSecond);
    }

    [Fact]
    public async Task Engine_WritesPcmAndCompletesVoice()
    {
        using MemorySink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);
        VoiceChannel channel = engine.CreateChannel(VoicePatch.Default);

        await channel.PlaySoundAsync(
                440,
                TimeSpan.FromMilliseconds(10),
                TestContext.Current.CancellationToken)
            .WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        Assert.True(sink.WriteCount > 0);
        Assert.True(sink.SawNonZeroSample);
    }

    [Fact]
    public async Task Engine_PropagatesSinkFailureToPlayback()
    {
        using FailingSink sink = new();
        using AudioEngine engine = AudioEngine.Create(sink);

        Task playback = engine.CreateChannel().PlaySoundAsync(
            440,
            TimeSpan.FromMilliseconds(100),
            TestContext.Current.CancellationToken);

        InvalidOperationException exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => playback.WaitAsync(
                    TimeSpan.FromSeconds(2),
                    TestContext.Current.CancellationToken));

        Assert.Contains("sink failure", exception.Message);
    }

    private sealed class MemorySink : IAudioSink
    {
        private readonly Lock _sync = new();

        public AudioFormat Format
            => AudioFormat.Default;

        public int FramesPerBuffer
            => 128;

        public int WriteCount { get; private set; }

        public short[] LastBuffer { get; private set; } = [];

        public bool SawNonZeroSample { get; private set; }

        public void Write(ReadOnlySpan<short> pcm)
        {
            lock (_sync)
            {
                LastBuffer = pcm.ToArray();
                SawNonZeroSample |= pcm.ContainsAnyExcept((short)0);
                WriteCount++;
            }

            Thread.Sleep(1);
        }

        public void Dispose()
        {
        }
    }

    private sealed class FailingSink : IAudioSink
    {
        public AudioFormat Format
            => AudioFormat.Default;

        public int FramesPerBuffer
            => 64;

        public void Write(ReadOnlySpan<short> pcm)
            => throw new InvalidOperationException("sink failure");

        public void Dispose()
        {
        }
    }
}
