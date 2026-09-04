using System.Diagnostics;

namespace SplitFlap.Audio.Playback;

/// <summary>
///  A channel is where you play things: it carries the instrument (<see cref="Patch"/>), a volume,
///  a reverb send, and the Play* methods. Channels are polyphonic; every call starts its own voice.
/// </summary>
/// <remarks>
///  <para>
///   All Play* tasks complete when the sound has fully died away (release included). Cancelling the
///   token doesn't throw; it releases the sound early and the task completes normally once it's gone.
///   A cancelled sequence stops after the current note.
///  </para>
///  <para>
///   Sequencing uses the wall clock (<see cref="Stopwatch"/>) with drift correction, so a melody
///   stays in time, but individual note starts jitter by the thread scheduler's granularity (~1-15 ms).
///   Good enough for a tune; a sample-accurate sequencer inside the engine is the next step if it isn't.
///  </para>
/// </remarks>
public sealed class VoiceChannel
{
    private readonly AudioEngine _engine;

    internal VoiceChannel(AudioEngine engine, VoicePatch patch)
    {
        _engine = engine;
        Patch = patch;
    }

    /// <summary>The instrument.</summary>
    public VoicePatch Patch { get; set; }

    /// <summary>Channel gain, 0..1.</summary>
    public float Volume { get; set; } = 1f;

    /// <summary>How much of this channel goes to the reverb bus, 0..1.</summary>
    public float ReverbSend { get; set; } = 0.2f;

    /// <summary>
    ///  Plays a frequency until the token is cancelled.
    /// </summary>
    public Task PlaySoundAsync(double frequency, CancellationToken cancellationToken)
        => Start(new ToneVoice(_engine.SampleRate, Patch, frequency, gate: null, Volume), cancellationToken);

    /// <summary>
    ///  Plays a frequency for a length.
    /// </summary>
    public Task PlaySoundAsync(double frequency, TimeSpan length, CancellationToken cancellationToken = default)
        => Start(new ToneVoice(_engine.SampleRate, Patch, frequency, length, Volume), cancellationToken);

    /// <summary>
    ///  Plays a <see cref="Sound"/>.
    /// </summary>
    public Task PlaySoundAsync(Sound sound, CancellationToken cancellationToken = default)
        => PlaySoundAsync(sound.Frequency, sound.Length, cancellationToken);

    /// <summary>
    ///  Plays one note at a tempo, honoring articulation and ornament.
    /// </summary>
    public async Task PlayNoteAsync(Note note, Tempo tempo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(note);

        TimeSpan duration = tempo.DurationOf(note);

        if (note.IsRest)
        {
            await DelayQuietly(duration, cancellationToken).ConfigureAwait(false);

            return;
        }

        (float gateFactor, float velocity) = note.Articulation switch
        {
            Articulation.Legato => (1f, 1f),
            Articulation.Staccato => (0.4f, 1f),
            Articulation.Accent => (0.8f, 1.3f),
            _ => (0.85f, 1f)
        };

        TimeSpan gate = duration * gateFactor;

        switch (note.Ornament)
        {
            case Ornament.Trill:
                await PlayTrillAsync(note, gate, velocity, cancellationToken).ConfigureAwait(false);
                break;

            case Ornament.Mordent:
                TimeSpan grace = TimeSpan.FromMilliseconds(Math.Min(60, gate.TotalMilliseconds / 4));
                await PlayToneAsync(note.Frequency, grace, velocity, cancellationToken, waitFor: grace).ConfigureAwait(false);
                await PlayToneAsync(note.FrequencyTransposed(2), grace, velocity, cancellationToken, waitFor: grace).ConfigureAwait(false);
                await PlayToneAsync(note.Frequency, gate - grace * 2, velocity, cancellationToken).ConfigureAwait(false);
                break;

            default:
                await PlayToneAsync(note.Frequency, gate, velocity, cancellationToken).ConfigureAwait(false);
                break;
        }
    }

    /// <summary>
    ///  Plays notes one after another at a tempo. Releases overlap the next note, as they should.
    /// </summary>
    public async Task PlayNotesAsync(IEnumerable<Note> notes, Tempo tempo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notes);

        Stopwatch clock = Stopwatch.StartNew();
        TimeSpan scheduled = TimeSpan.Zero;
        List<Task> tails = [];

        foreach (Note note in notes)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            TimeSpan duration = tempo.DurationOf(note);
            tails.Add(PlayNoteAsync(note, tempo, cancellationToken));
            scheduled += duration;

            // Wait for the note's slot to end, measured against the wall clock so jitter doesn't accumulate.
            await DelayQuietly(scheduled - clock.Elapsed, cancellationToken).ConfigureAwait(false);
        }

        await Task.WhenAll(tails).ConfigureAwait(false);
    }

    /// <summary>
    ///  Plays a melody in IML notation: <c>"C4-4 E4-4 G4-2"</c>.
    /// </summary>
    public Task PlayNotesAsync(string melody, Tempo tempo, CancellationToken cancellationToken = default)
        => PlayNotesAsync(Note.ParseMelody(melody), tempo, cancellationToken);

    /// <summary>
    ///  Plays a sample once.
    /// </summary>
    public Task PlaySampleAsync(Sample sample, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sample);

        return Start(new SampleVoice(sample, Volume), cancellationToken);
    }

    /// <summary>
    ///  Plays samples back to back.
    /// </summary>
    public async Task PlaySamplesAsync(IEnumerable<Sample> samples, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(samples);

        foreach (Sample sample in samples)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await PlaySampleAsync(sample, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///  Fires any voice and forgets it. For percussion and clacks, where nobody awaits anything.
    /// </summary>
    public void Trigger(IVoice voice)
        => _ = _engine.Play(voice, ReverbSend);

    private Task Start(IVoice voice, CancellationToken cancellationToken)
    {
        Task completion = _engine.Play(voice, ReverbSend);

        if (cancellationToken.CanBeCanceled)
        {
            CancellationTokenRegistration registration = cancellationToken.Register(voice.Release);
            _ = completion.ContinueWith(_ => registration.Dispose(), TaskScheduler.Default);
        }

        return completion;
    }

    private Task PlayToneAsync(double frequency, TimeSpan gate, float velocity, CancellationToken cancellationToken, TimeSpan? waitFor = null)
    {
        Task tone = Start(new ToneVoice(_engine.SampleRate, Patch, frequency, gate, Volume * velocity), cancellationToken);

        return waitFor is { } wait ? DelayQuietly(wait, cancellationToken) : tone;
    }

    private async Task PlayTrillAsync(Note note, TimeSpan gate, float velocity, CancellationToken cancellationToken)
    {
        TimeSpan step = TimeSpan.FromMilliseconds(Math.Clamp(gate.TotalMilliseconds / 8, 40, 90));
        ToneVoice voice = new(_engine.SampleRate, Patch, note.Frequency, gate, Volume * velocity);
        Task completion = Start(voice, cancellationToken);
        Stopwatch clock = Stopwatch.StartNew();
        bool upper = false;

        while (clock.Elapsed < gate && !cancellationToken.IsCancellationRequested)
        {
            await DelayQuietly(step, cancellationToken).ConfigureAwait(false);
            upper = !upper;
            voice.Oscillator.Frequency = upper ? note.FrequencyTransposed(2) : note.Frequency;
        }

        await completion.ConfigureAwait(false);
    }

    private static async Task DelayQuietly(TimeSpan delay, CancellationToken cancellationToken)
    {
        if (delay <= TimeSpan.Zero)
        {
            return;
        }

        try
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Cancellation means "stop playing", not "something went wrong".
        }
    }
}
