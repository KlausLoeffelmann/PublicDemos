using System.Reflection;
using SplitFlap.Audio.Core;
using SplitFlap.Demo;
using SplitFlap.Visuals;

namespace SplitFlap.Tests;

public sealed class BoardSoundTests
{
    [Fact]
    public void Sound_HandlesFlapEventsButNotJamEvents()
    {
        using ManualResetEventSlim stopped = new();
        using SplitFlapAnimator animator = SplitFlapAnimator.Create();
        using BoardSound sound = new(animator, new WaitingSink(stopped));

        // Check the actual subscription, not the handler's name: jamming must not admit
        // a second sound, while ordinary flap clacks must remain connected.
        Assert.Null(GetEventHandlers(animator, nameof(SplitFlapAnimator.Jammed)));
        Delegate flaps = Assert.IsAssignableFrom<Delegate>(
            GetEventHandlers(animator, nameof(SplitFlapAnimator.FlapFell)));
        Assert.Same(sound, Assert.Single(flaps.GetInvocationList()).Target);

        sound.Dispose();
        Assert.Null(GetEventHandlers(animator, nameof(SplitFlapAnimator.FlapFell)));
    }

    private static object? GetEventHandlers(SplitFlapAnimator animator, string eventName)
    {
        FieldInfo? field = typeof(SplitFlapAnimator).GetField(
            eventName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field);
        return field.GetValue(animator);
    }

    private sealed class WaitingSink(ManualResetEventSlim stopped) : IAudioSink
    {
        public AudioFormat Format => AudioFormat.Default;
        public int FramesPerBuffer => 64;

        public void Write(ReadOnlySpan<short> pcm)
        {
            stopped.Wait();
            throw new ObjectDisposedException(nameof(WaitingSink));
        }

        public void Dispose() => stopped.Set();
    }
}
