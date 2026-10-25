using System.Buffers.Binary;
using WinForms.Audio.Percussion;

namespace WinForms.Audio.Sequencing;

/// <summary>
///  Describes an offline render of a contiguous score range and its current sound/mixer settings.
/// </summary>
public sealed class PercussionWaveExportOptions(PercussionScore score, Tempo tempo)
{
    /// <summary>
    ///  Gets the source score; playback position and the live player's loop switch are independent.
    /// </summary>
    public PercussionScore Score { get; } = score;

    /// <summary>
    ///  Gets the constant tempo used for the entire file.
    /// </summary>
    public Tempo Tempo { get; } = tempo;

    /// <summary>
    ///  Gets the zero-based first bar to render, starting with no preceding sound tails.
    /// </summary>
    public int FirstBar { get; init; }

    /// <summary>
    ///  Gets the number of contiguous bars, or null for all remaining bars.
    /// </summary>
    public int? BarCount { get; init; }

    /// <summary>
    ///  Gets how many times to render the range, retaining sound tails across repetitions.
    /// </summary>
    public int Repetitions { get; init; } = 1;

    /// <summary>
    ///  Gets the output rate; stereo channels contain the same mono mix as live playback.
    /// </summary>
    public int SampleRate { get; init; } = 48_000;

    /// <summary>
    ///  Gets the player-local master level, from zero through one.
    /// </summary>
    public float MasterVolume { get; init; } = 1f;

    /// <summary>
    ///  Gets one channel level per fixed track, in kit rather than enum order.
    /// </summary>
    public IReadOnlyList<float> InstrumentVolumes { get; init; }
        = Array.AsReadOnly(Enumerable.Repeat(1f, Cr78Kit.Instruments.Count).ToArray());

    /// <summary>
    ///  Gets whether hi-hat/cymbal strikes also excite the auxiliary metallic layer.
    /// </summary>
    public bool MetallicEnabled { get; init; } = true;

    /// <summary>
    ///  Gets the auxiliary metallic-layer amount, from zero through one.
    /// </summary>
    public float MetallicLevel { get; init; }

    /// <summary>
    ///  Gets rate-matched overrides; unmapped tracks use stock synthesis.
    /// </summary>
    public IReadOnlyDictionary<Cr78Instrument, Sample>? Samples { get; init; }

    /// <summary>
    ///  Gets whether to append naturally decaying sounds after the exact musical endpoint.
    /// </summary>
    public bool IncludeTail { get; init; }
}

/// <summary>
///  Writes unnormalized stereo PCM16 using the live percussion renderer, without an audio device.
/// </summary>
public static class PercussionWaveExporter
{
    /// <summary>
    ///  Bounds output duration, including any requested final tail, to one hour.
    /// </summary>
    public const int MaximumDurationSeconds = 3_600;

    /// <summary>
    ///  Bounds repetition requests independently of their tempo and bar count.
    /// </summary>
    public const int MaximumRepetitions = 10_000;

    /// <summary>
    ///  Renders a snapshot off the caller thread and atomically publishes the completed WAV.
    ///  Cancellation or failure preserves an existing destination.
    /// </summary>
    public static Task ExportAsync(
        string path, PercussionWaveExportOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Score);
        Cr78Kit.ValidateSampleRate(options.SampleRate);
        PercussionClock.ValidateTempo(options.Tempo);
        ArgumentOutOfRangeException.ThrowIfNegative(options.FirstBar);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(options.FirstBar, options.Score.BarCount);
        int bars = options.BarCount ?? options.Score.BarCount - options.FirstBar;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bars);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bars, options.Score.BarCount - options.FirstBar);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(options.Repetitions);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(options.Repetitions, MaximumRepetitions);
        cancellationToken.ThrowIfCancellationRequested();

        decimal framesPerStep = options.SampleRate * 15m / options.Tempo.BeatsPerMinute;
        decimal exactFrames = framesPerStep * PercussionScore.StepsPerBar * bars * options.Repetitions;
        long maximumFrames = (long)options.SampleRate * MaximumDurationSeconds;
        if (exactFrames > maximumFrames)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "A WAV export cannot exceed one hour.");
        }

        long musicalFrames = PercussionClock.RoundFrame(exactFrames);
        PercussionScore score = new(bars, options.Score.Hits
            .Where(hit => hit.Bar >= options.FirstBar && hit.Bar < options.FirstBar + bars)
            .Select(hit => new PercussionHit(
                hit.Bar - options.FirstBar, hit.Step, hit.Instrument, hit.Velocity, hit.GateSteps)));
        PercussionMailbox mailbox = new(score, options.Tempo);
        mailbox.ApplyConfiguration(score, options.Tempo, options.MasterVolume, options.InstrumentVolumes,
            loop: true, options.MetallicEnabled, options.MetallicLevel);
        mailbox.SetSamples(options.Samples is null
            ? PercussionSampleSet.Stock
            : new PercussionSampleSet(options.Samples, options.SampleRate));
        mailbox.Start();
        string destination = Path.GetFullPath(path);
        return Task.Run(() => Write(
            destination, options, mailbox, musicalFrames, maximumFrames, cancellationToken), cancellationToken);
    }

    private static void Write(
        string destination, PercussionWaveExportOptions options, PercussionMailbox mailbox,
        long musicalFrames, long maximumFrames, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PercussionRenderer renderer = new(options.SampleRate, mailbox, noiseSeed: 42);
        cancellationToken.ThrowIfCancellationRequested();
        string temporary = Path.Combine(Path.GetDirectoryName(destination)!,
            $".{Path.GetFileName(destination)}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (FileStream stream = new(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                byte[] header = WaveFile.ToWavBytes([], options.SampleRate, channels: 2);
                stream.Write(header);
                byte[] buffer = new byte[4_096];
                int buffered = 0;
                long frame = 0;
                while (frame < musicalFrames || options.IncludeTail)
                {
                    if (frame == musicalFrames)
                    {
                        // At this exact wrap, the live renderer stops scheduling but leaves natural tails.
                        mailbox.SetLoop(false);
                        renderer.BeginBlock(frame);
                        if (!renderer.HasActiveSounds)
                        {
                            break;
                        }
                    }
                    else if (frame % 1_024 == 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        renderer.BeginBlock(frame);
                    }

                    if (frame >= maximumFrames)
                    {
                        throw new InvalidOperationException("The exported sounds and final tail exceed one hour.");
                    }

                    float sample = renderer.Next(frame);
                    // Match AudioEngine's unity-master PCM conversion, including truncation.
                    short pcm = (short)Math.Clamp(sample * short.MaxValue, short.MinValue, short.MaxValue);
                    BinaryPrimitives.WriteInt16LittleEndian(buffer.AsSpan(buffered), pcm);
                    BinaryPrimitives.WriteInt16LittleEndian(buffer.AsSpan(buffered + 2), pcm);
                    buffered += 4;
                    frame++;
                    if (buffered == buffer.Length)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        stream.Write(buffer);
                        buffered = 0;
                    }

                    if (frame >= musicalFrames && !renderer.HasActiveSounds)
                    {
                        break;
                    }
                }

                stream.Write(buffer.AsSpan(0, buffered));
                uint dataBytes = checked((uint)(frame * 4));
                BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked(36 + dataBytes));
                BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(40), dataBytes);
                stream.Position = 0;
                stream.Write(header);
                stream.Flush(flushToDisk: true);
            }

            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporary, destination, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }
}
