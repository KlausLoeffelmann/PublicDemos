using System.Collections.ObjectModel;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;

namespace DrumMachine.Demo.Documents;

/// <summary>
///  Maps the fixed percussion tracks to recordings without changing the score or track count.
///  An absent mapping always selects that track's procedural stock sound.
/// </summary>
internal sealed class PercussionSoundPreset
{
    /// <summary>
    ///  Gets the stock algorithmic palette, independent of temporary analysis recordings.
    /// </summary>
    public static PercussionSoundPreset Stock { get; } = new("Stock percussion");

    /// <summary>
    ///  Copies a named mapping with fully qualified source WAV paths.
    /// </summary>
    public PercussionSoundPreset(string name, IReadOnlyDictionary<Cr78Instrument, string>? waveFiles = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        name = name.Trim();
        if (name.Length > 80 || name.Any(char.IsControl))
        {
            throw new ArgumentException("A sound preset name must contain at most eighty printable characters.", nameof(name));
        }

        Dictionary<Cr78Instrument, string> files = [];
        if (waveFiles is not null)
        {
            foreach ((Cr78Instrument instrument, string path) in waveFiles)
            {
                ValidateTrack(instrument);
                ArgumentException.ThrowIfNullOrWhiteSpace(path);
                if (!Path.IsPathFullyQualified(path) ||
                    !string.Equals(Path.GetExtension(path), ".wav", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Sound mappings must name fully qualified WAV files.", nameof(waveFiles));
                }

                files.Add(instrument, Path.GetFullPath(path));
            }
        }

        Name = name;
        WaveFiles = new ReadOnlyDictionary<Cr78Instrument, string>(files);
    }

    /// <summary> Gets the human-readable sound preset name. </summary>
    public string Name { get; }

    /// <summary> Gets an immutable snapshot of source paths for imported tracks only. </summary>
    public IReadOnlyDictionary<Cr78Instrument, string> WaveFiles { get; }

    /// <summary> Replaces a single fixed track's recording. </summary>
    public PercussionSoundPreset WithWaveFile(Cr78Instrument instrument, string path)
    {
        Dictionary<Cr78Instrument, string> files = new(WaveFiles) { [instrument] = path };
        return new(Name, files);
    }

    /// <summary> Restores a single fixed track's algorithmic stock sound. </summary>
    public PercussionSoundPreset WithoutWaveFile(Cr78Instrument instrument)
    {
        ValidateTrack(instrument);
        Dictionary<Cr78Instrument, string> files = new(WaveFiles);
        files.Remove(instrument);
        return new(Name, files);
    }

    /// <summary> Renames a preset without modifying its sound mappings. </summary>
    public PercussionSoundPreset WithName(string name) => new(name, WaveFiles);

    /// <summary>
    ///  Decodes and resamples the entire replacement off the UI/audio threads before publishing it.
    /// </summary>
    public Task<IReadOnlyDictionary<Cr78Instrument, Sample>> PrepareAsync(
        int sampleRate, CancellationToken cancellationToken = default)
        => Task.Run(async () =>
        {
            Dictionary<Cr78Instrument, Sample> samples = [];
            foreach ((Cr78Instrument instrument, string path) in WaveFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Sample sample = await Sample.FromWaveFileAsync(path, sampleRate, cancellationToken).ConfigureAwait(false);
                if (sample.Duration > TimeSpan.FromSeconds(30))
                {
                    throw new InvalidDataException($"'{Path.GetFileName(path)}' exceeds the thirty-second percussion limit.");
                }

                samples.Add(instrument, sample);
            }

            return (IReadOnlyDictionary<Cr78Instrument, Sample>)new ReadOnlyDictionary<Cr78Instrument, Sample>(samples);
        }, cancellationToken);

    private static void ValidateTrack(Cr78Instrument instrument)
    {
        if (!Cr78Kit.Instruments.Contains(instrument))
        {
            throw new ArgumentOutOfRangeException(nameof(instrument), "Only the fixed percussion tracks can be mapped.");
        }
    }
}
