using System.Text.Json;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Sequencing;

namespace DrumMachine.Demo.Documents;

/// <summary>
///  Reads and atomically writes complete, versioned loop documents without audio or UI objects.
/// </summary>
internal static class LoopDocumentStore
{
    /// <summary>
    ///  The compound extension used by loop-file dialogs and the bundled original example.
    /// </summary>
    public const string FileExtension = ".drumloop.json";

    /// <summary>
    ///  The known document identity, distinct from application preferences or arbitrary JSON files.
    /// </summary>
    public const string FormatIdentifier = "drumloop";

    /// <summary>
    ///  The supported version of this explicit score and mixer schema.
    /// </summary>
    public const int CurrentVersion = 1;

    /// <summary>
    ///  The stable procedural kit identity; documents never name executable types or sample paths.
    /// </summary>
    public const string KitIdentifier = "cr78-procedural-v1";

    /// <summary>
    ///  Bounds JSON allocation and parsing, in addition to the score's bar and document event limits.
    /// </summary>
    public const int MaximumFileBytes = 16 * 1024 * 1024;

    /// <summary>
    ///  Loads and validates a complete document off the caller's thread, propagating file and format failures.
    /// </summary>
    public static Task<LoopDocument> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Task.Run(async () =>
        {
            byte[] contents = await JsonFileStorage.ReadAsync(path, MaximumFileBytes, cancellationToken)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using JsonDocument json = StrictJson.Parse(contents);
                return ReadDocument(json.RootElement, cancellationToken);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("The loop file is not valid JSON in the supported format.", ex);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidDataException($"The loop score or mixer is invalid: {ex.Message}", ex);
            }
        }, cancellationToken);
    }

    /// <summary>
    ///  Saves the supplied immutable editor snapshot without truncating or replacing an old file on failure.
    /// </summary>
    public static Task SaveAsync(
        LoopDocument document,
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        return JsonFileStorage.WriteAsync(
            path,
            writer => WriteDocument(writer, document, cancellationToken),
            MaximumFileBytes,
            createDirectory: false,
            cancellationToken);
    }

    private static LoopDocument ReadDocument(JsonElement root, CancellationToken cancellationToken)
    {
        StrictJson.RequireProperties(root, "format", "version", "kit", "score", "tempoBpm", "loop", "mixer");
        if (StrictJson.String(root.GetProperty("format"), "format") != FormatIdentifier)
        {
            throw new InvalidDataException("This file is not a drum-loop document.");
        }

        StrictJson.Integer(root.GetProperty("version"), "version", CurrentVersion, CurrentVersion);
        if (StrictJson.String(root.GetProperty("kit"), "kit") != KitIdentifier)
        {
            throw new InvalidDataException("This loop uses an unsupported percussion kit.");
        }

        int tempo = StrictJson.Integer(root.GetProperty("tempoBpm"), "tempoBpm", 40, 240);
        bool loop = StrictJson.Boolean(root.GetProperty("loop"), "loop");
        PercussionScore score = ReadScore(root.GetProperty("score"), cancellationToken);

        JsonElement mixer = root.GetProperty("mixer");
        StrictJson.RequireProperties(mixer,
            "masterVolumePercent", "percussionVolumes", "metallicEnabled", "metallicVolumePercent");
        int masterVolume = StrictJson.Integer(mixer.GetProperty("masterVolumePercent"), "masterVolumePercent", 0, 100);
        bool metallicEnabled = StrictJson.Boolean(mixer.GetProperty("metallicEnabled"), "metallicEnabled");
        int metallicVolume = StrictJson.Integer(
            mixer.GetProperty("metallicVolumePercent"), "metallicVolumePercent", 0, 100);
        Dictionary<Cr78Instrument, int> levels = ReadLevels(mixer.GetProperty("percussionVolumes"));
        return new LoopDocument(score, tempo, masterVolume, levels, loop, metallicEnabled, metallicVolume);
    }

    private static PercussionScore ReadScore(JsonElement score, CancellationToken cancellationToken)
    {
        StrictJson.RequireProperties(score, "bars", "stepsPerBar", "tracks");
        int bars = StrictJson.Integer(score.GetProperty("bars"), "bars", 1, PercussionScore.MaximumBars);
        StrictJson.Integer(score.GetProperty("stepsPerBar"), "stepsPerBar",
            PercussionScore.StepsPerBar, PercussionScore.StepsPerBar);
        JsonElement tracks = score.GetProperty("tracks");
        RequireAllInstruments(tracks, "tracks");

        HashSet<Cr78Instrument> instruments = [];
        List<PercussionHit> hits = [];
        foreach (JsonElement track in tracks.EnumerateArray())
        {
            cancellationToken.ThrowIfCancellationRequested();
            StrictJson.RequireProperties(track, "instrument", "hits");
            Cr78Instrument instrument = ReadInstrument(track.GetProperty("instrument"));
            if (!instruments.Add(instrument))
            {
                throw new InvalidDataException($"Duplicate score track '{InstrumentIdentifier(instrument)}'.");
            }

            JsonElement events = track.GetProperty("hits");
            StrictJson.RequireArray(events, "hits");
            int count = events.GetArrayLength();
            if (count > bars * PercussionScore.StepsPerBar || hits.Count + count > LoopDocument.MaximumHits)
            {
                throw new InvalidDataException("The loop exceeds the supported number of score events.");
            }

            foreach (JsonElement hit in events.EnumerateArray())
            {
                StrictJson.RequireProperties(hit, "bar", "step", "velocity", "gateSteps");
                int bar = StrictJson.Integer(hit.GetProperty("bar"), "bar", 0, bars - 1);
                int step = StrictJson.Integer(hit.GetProperty("step"), "step", 0, PercussionScore.StepsPerBar - 1);
                int gate = StrictJson.Integer(hit.GetProperty("gateSteps"), "gateSteps", 1, PercussionScore.MaximumGateSteps);
                JsonElement velocityValue = hit.GetProperty("velocity");
                if (velocityValue.ValueKind != JsonValueKind.Number ||
                    !velocityValue.TryGetDouble(out double velocity) ||
                    !double.IsFinite(velocity) || velocity is < 0 or > 1)
                {
                    throw new InvalidDataException("'velocity' must be a finite number from zero through one.");
                }

                hits.Add(new PercussionHit(bar, step, instrument, (float)velocity, gate));
            }
        }

        // The existing score constructor also rejects duplicate cells and validates the runtime model.
        return new PercussionScore(bars, hits);
    }

    private static Dictionary<Cr78Instrument, int> ReadLevels(JsonElement entries)
    {
        RequireAllInstruments(entries, "percussionVolumes");
        Dictionary<Cr78Instrument, int> levels = [];
        foreach (JsonElement entry in entries.EnumerateArray())
        {
            StrictJson.RequireProperties(entry, "instrument", "volumePercent");
            Cr78Instrument instrument = ReadInstrument(entry.GetProperty("instrument"));
            int percent = StrictJson.Integer(entry.GetProperty("volumePercent"), "volumePercent", 0, 100);
            if (!levels.TryAdd(instrument, percent))
            {
                throw new InvalidDataException($"Duplicate mixer instrument '{InstrumentIdentifier(instrument)}'.");
            }
        }

        return levels;
    }

    private static void RequireAllInstruments(JsonElement entries, string name)
    {
        StrictJson.RequireArray(entries, name);
        if (entries.GetArrayLength() != Cr78Kit.Instruments.Count)
        {
            throw new InvalidDataException($"'{name}' must contain all thirteen primary percussion instruments exactly once.");
        }
    }

    private static void WriteDocument(
        Utf8JsonWriter writer,
        LoopDocument document,
        CancellationToken cancellationToken)
    {
        writer.WriteStartObject();
        writer.WriteString("format", FormatIdentifier);
        writer.WriteNumber("version", CurrentVersion);
        writer.WriteString("kit", KitIdentifier);
        writer.WriteNumber("tempoBpm", document.TempoBpm);
        writer.WriteBoolean("loop", document.Loop);
        writer.WriteStartObject("score");
        writer.WriteNumber("bars", document.Score.BarCount);
        writer.WriteNumber("stepsPerBar", PercussionScore.StepsPerBar);
        writer.WriteStartArray("tracks");
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            cancellationToken.ThrowIfCancellationRequested();
            writer.WriteStartObject();
            writer.WriteString("instrument", InstrumentIdentifier(instrument));
            writer.WriteStartArray("hits");
            foreach (PercussionHit hit in document.Score.Hits)
            {
                if (hit.Instrument != instrument)
                {
                    continue;
                }

                writer.WriteStartObject();
                writer.WriteNumber("bar", hit.Bar);
                writer.WriteNumber("step", hit.Step);
                writer.WriteNumber("velocity", hit.Velocity);
                writer.WriteNumber("gateSteps", hit.GateSteps);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.WriteStartObject("mixer");
        writer.WriteNumber("masterVolumePercent", document.MasterVolumePercent);
        writer.WriteStartArray("percussionVolumes");
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            writer.WriteStartObject();
            writer.WriteString("instrument", InstrumentIdentifier(instrument));
            writer.WriteNumber("volumePercent", document.PercussionVolumes[instrument]);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteBoolean("metallicEnabled", document.MetallicEnabled);
        writer.WriteNumber("metallicVolumePercent", document.MetallicVolumePercent);
        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private static string InstrumentIdentifier(Cr78Instrument instrument)
        => instrument switch
        {
            Cr78Instrument.BassDrum => "bassDrum",
            Cr78Instrument.SnareDrum => "snareDrum",
            Cr78Instrument.RimShot => "rimShot",
            Cr78Instrument.HiHat => "hiHat",
            Cr78Instrument.Cymbal => "cymbal",
            Cr78Instrument.Maracas => "maracas",
            Cr78Instrument.Claves => "claves",
            Cr78Instrument.Cowbell => "cowbell",
            Cr78Instrument.HighBongo => "highBongo",
            Cr78Instrument.LowBongo => "lowBongo",
            Cr78Instrument.LowConga => "lowConga",
            Cr78Instrument.Tambourine => "tambourine",
            Cr78Instrument.Guiro => "guiro",
            _ => throw new ArgumentOutOfRangeException(nameof(instrument))
        };

    private static Cr78Instrument ReadInstrument(JsonElement value)
        => StrictJson.String(value, "instrument") switch
        {
            "bassDrum" => Cr78Instrument.BassDrum,
            "snareDrum" => Cr78Instrument.SnareDrum,
            "rimShot" => Cr78Instrument.RimShot,
            "hiHat" => Cr78Instrument.HiHat,
            "cymbal" => Cr78Instrument.Cymbal,
            "maracas" => Cr78Instrument.Maracas,
            "claves" => Cr78Instrument.Claves,
            "cowbell" => Cr78Instrument.Cowbell,
            "highBongo" => Cr78Instrument.HighBongo,
            "lowBongo" => Cr78Instrument.LowBongo,
            "lowConga" => Cr78Instrument.LowConga,
            "tambourine" => Cr78Instrument.Tambourine,
            "guiro" => Cr78Instrument.Guiro,
            _ => throw new InvalidDataException("The loop contains an unknown or non-score percussion instrument.")
        };
}
