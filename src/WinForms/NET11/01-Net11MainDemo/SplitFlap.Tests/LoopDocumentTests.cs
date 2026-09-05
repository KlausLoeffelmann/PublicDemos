using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using DrumMachine.Demo;
using DrumMachine.Demo.Documents;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Sequencing;

namespace SplitFlap.Tests;

/// <summary>
///  Exercises immutable loop values, strict bounded JSON, and failure-safe file publication without an audio endpoint.
/// </summary>
public sealed class LoopDocumentTests
{
    /// <summary>
    ///  Creates genuinely blank scores, including valid lengths beyond the New dialog's three choices.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(PercussionScore.MaximumBars)]
    public void EmptyDocument_RetainsItsBarsAndCompleteDefaultMixer(int bars)
    {
        LoopDocument document = LoopDocument.CreateEmpty(bars);
        Assert.Equal(bars, document.Score.BarCount);
        Assert.Empty(document.Score.Hits);
        Assert.Equal(92, document.TempoBpm);
        Assert.Equal(65, document.MasterVolumePercent);
        Assert.True(document.Loop);
        Assert.False(document.MetallicEnabled);
        Assert.Equal(0, document.MetallicVolumePercent);
        Assert.Equal(13, document.PercussionVolumes.Count);
        Assert.All(Cr78Kit.Instruments, instrument => Assert.Equal(100, document.PercussionVolumes[instrument]));
    }

    /// <summary>
    ///  Copies caller-owned levels and treats separately allocated score and mixer contents as equal.
    /// </summary>
    [Fact]
    public void Document_IsDefensiveAndStructurallyEqual()
    {
        Dictionary<Cr78Instrument, int> levels = Cr78Kit.Instruments.ToDictionary(instrument => instrument, _ => 71);
        PercussionScore score = new(4, [new(1, 15, Cr78Instrument.Guiro, 0.37f, 33)]);
        LoopDocument document = new(score, percussionVolumes: levels, metallicVolumePercent: 57);
        levels[Cr78Instrument.Guiro] = 0;
        Assert.Equal(71, document.PercussionVolumes[Cr78Instrument.Guiro]);
        Assert.Throws<NotSupportedException>(() =>
            ((IDictionary<Cr78Instrument, int>)document.PercussionVolumes)[Cr78Instrument.Guiro] = 0);

        Dictionary<Cr78Instrument, int> reordered = document.PercussionVolumes.Reverse()
            .ToDictionary(entry => entry.Key, entry => entry.Value);
        LoopDocument equal = new(new PercussionScore(4, score.Hits), percussionVolumes: reordered, metallicVolumePercent: 57);
        Assert.NotSame(document.Score, equal.Score);
        Assert.True(document.ValueEquals(equal));
        Assert.Equal(document, equal);
        Assert.Equal(document.GetHashCode(), equal.GetHashCode());
        Assert.False(document.ValueEquals(null));
        Assert.False(document.Equals(new object()));

        LoopDocument changed = document.WithTempo(137).WithMasterVolume(0)
            .WithInstrumentVolume(Cr78Instrument.Cowbell, 0).WithLoop(false).WithMetallic(false, 83);
        Assert.Same(score, changed.Score);
        Assert.Equal(4, changed.Score.BarCount);
        Assert.Equal(0.37f, Assert.Single(changed.Score.Hits).Velocity);
        Assert.Equal(33, Assert.Single(changed.Score.Hits).GateSteps);
        Assert.Equal(57, document.MetallicVolumePercent);
        Assert.False(changed.MetallicEnabled);
        Assert.Equal(83, changed.MetallicVolumePercent);
        Assert.Equal(71, document.PercussionVolumes[Cr78Instrument.Cowbell]);
    }

    /// <summary>
    ///  Reuses unchanged values and includes every musical field and event parameter in semantic equality.
    /// </summary>
    [Fact]
    public void Document_RecognizesNoOpsAndEveryMusicalDifference()
    {
        LoopDocument document = new(new PercussionScore(4, [new(1, 2, Cr78Instrument.Guiro, 0.37f, 9)]));
        Assert.Same(document, document.WithScore(new PercussionScore(4, document.Score.Hits)));
        Assert.Same(document, document.WithTempo(92));
        Assert.Same(document, document.WithMasterVolume(65));
        Assert.Same(document, document.WithInstrumentVolume(Cr78Instrument.Guiro, 100));
        Assert.Same(document, document.WithLoop(true));
        Assert.Same(document, document.WithMetallic(false, 0));

        LoopDocument[] different =
        [
            document.WithTempo(93),
            document.WithMasterVolume(64),
            document.WithInstrumentVolume(Cr78Instrument.LowBongo, 99),
            document.WithLoop(false),
            document.WithMetallic(true, 0),
            document.WithMetallic(false, 1),
            document.WithScore(new PercussionScore(3, document.Score.Hits)),
            document.WithScore(new PercussionScore(4, [new(1, 2, Cr78Instrument.Guiro, 0.38f, 9)])),
            document.WithScore(new PercussionScore(4, [new(1, 2, Cr78Instrument.Guiro, 0.37f, 10)])),
            document.WithScore(new PercussionScore(4, [new(1, 3, Cr78Instrument.Guiro, 0.37f, 9)]))
        ];
        Assert.All(different, value => Assert.False(document.ValueEquals(value)));
    }

    /// <summary>
    ///  Rejects invalid model values before they can become editor history or serialized state.
    /// </summary>
    [Fact]
    public void Document_RejectsInvalidValuesAndIncompleteMixers()
    {
        LoopDocument document = LoopDocument.CreateEmpty(1);
        Assert.Throws<ArgumentNullException>(() => new LoopDocument(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => LoopDocument.CreateEmpty(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => LoopDocument.CreateEmpty(PercussionScore.MaximumBars + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithTempo(39));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithTempo(241));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithMasterVolume(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithMasterVolume(101));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithMetallic(true, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithMetallic(false, 101));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithInstrumentVolume(Cr78Instrument.HiHat, 101));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithInstrumentVolume(Cr78Instrument.MetallicBeat, 50));
        Assert.Throws<ArgumentOutOfRangeException>(() => document.WithInstrumentVolume((Cr78Instrument)99, 50));
        Assert.Throws<ArgumentException>(() =>
            new LoopDocument(document.Score, percussionVolumes: new Dictionary<Cr78Instrument, int>()));

        Dictionary<Cr78Instrument, int> levels = new(document.PercussionVolumes)
        {
            [Cr78Instrument.MetallicBeat] = 10
        };
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoopDocument(document.Score, percussionVolumes: levels));
        levels.Remove(Cr78Instrument.MetallicBeat);
        levels[Cr78Instrument.HiHat] = -1;
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoopDocument(document.Score, percussionVolumes: levels));
    }

    /// <summary>
    ///  Enforces a document event bound in addition to the reusable score's independent bar bound.
    /// </summary>
    [Fact]
    public void Document_RejectsTooManyOtherwiseValidScoreEvents()
    {
        IEnumerable<PercussionHit> hits = Enumerable.Range(0, LoopDocument.MaximumHits + 1).Select(index =>
            new PercussionHit(index / (16 * 13), index / 13 % 16, Cr78Kit.Instruments[index % 13]));
        PercussionScore large = new(PercussionScore.MaximumBars, hits);
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoopDocument(large));
        Assert.Throws<ArgumentOutOfRangeException>(() => LoopDocument.CreateEmpty(1).WithScore(large));
    }

    /// <summary>
    ///  Round-trips zeros, exact float velocities, cross-bar gates, empty bars, and all mixer entries.
    /// </summary>
    [Theory]
    [InlineData(40, false)]
    [InlineData(240, true)]
    public async Task Store_RoundTripsTheEntireMusicalSnapshot(int tempo, bool metallicEnabled)
    {
        using LoopTestFiles files = new();
        string path = files.File("roundtrip.drumloop.json");
        Dictionary<Cr78Instrument, int> levels = Cr78Kit.Instruments
            .Select((instrument, index) => (instrument, index))
            .ToDictionary(entry => entry.instrument, entry => entry.index * 8);
        PercussionScore score = new(4,
        [
            new(0, 0, Cr78Instrument.HiHat, 0),
            new(1, 15, Cr78Instrument.Guiro, 0.33333334f, 33),
            new(2, 12, Cr78Instrument.Cowbell, float.Epsilon),
            new(2, 15, Cr78Instrument.SnareDrum, 1, PercussionScore.MaximumGateSteps)
        ]);
        LoopDocument expected = new(score, tempo, 0, levels, false, metallicEnabled, 100);

        await LoopDocumentStore.SaveAsync(expected, path, TestContext.Current.CancellationToken);
        LoopDocument actual = await LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
        Assert.Equal(expected.GetHashCode(), actual.GetHashCode());
        Assert.Equal(expected.Score.Hits.ToArray(), actual.Score.Hits.ToArray());
        Assert.Equal(4, actual.Score.BarCount);
        using JsonDocument json = JsonDocument.Parse(
            await System.IO.File.ReadAllTextAsync(path, TestContext.Current.CancellationToken));
        JsonElement root = json.RootElement;
        Assert.Equal(LoopDocumentStore.FormatIdentifier, root.GetProperty("format").GetString());
        Assert.Equal(LoopDocumentStore.KitIdentifier, root.GetProperty("kit").GetString());
        Assert.Equal(13, root.GetProperty("score").GetProperty("tracks").GetArrayLength());
        Assert.Equal(13, root.GetProperty("mixer").GetProperty("percussionVolumes").GetArrayLength());
        Assert.All(root.GetProperty("score").GetProperty("tracks").EnumerateArray(), track =>
            Assert.Equal(JsonValueKind.String, track.GetProperty("instrument").ValueKind));
        Assert.Contains(root.GetProperty("score").GetProperty("tracks").EnumerateArray(),
            track => track.GetProperty("hits").GetArrayLength() == 0);
        Assert.Empty(Directory.EnumerateFiles(files.DirectoryPath, "*.tmp"));
    }

    /// <summary>
    ///  Accepts an ordinary UTF-8 editor BOM and loads the bundled original, rather than a copied song or preset.
    /// </summary>
    [Fact]
    public async Task Store_LoadsBomAndTheOriginalBundledExample()
    {
        using LoopTestFiles files = new();
        string path = files.File("bom.drumloop.json");
        await System.IO.File.WriteAllTextAsync(
            path, CreateValidJson().ToJsonString(), new UTF8Encoding(true), TestContext.Current.CancellationToken);
        LoopDocument actual = await LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken);
        Assert.Equal(4, actual.Score.BarCount);
        Assert.Single(actual.Score.Hits);

        string examplePath = Path.Combine(AppContext.BaseDirectory, "Examples", "OriginalBallad.drumloop.json");
        LoopDocument example = await LoopDocumentStore.LoadAsync(examplePath, TestContext.Current.CancellationToken);
        Assert.Equal(new LoopDocument(DemoScores.OriginalBallad), example);
    }

    /// <summary>
    ///  Rejects unsupported versions, kits, types, numeric instruments, and every score or mixer range violation.
    /// </summary>
    [Theory]
    [InlineData("format", "\"other\"")]
    [InlineData("version", "2")]
    [InlineData("version", "\"1\"")]
    [InlineData("kit", "\"unknown-kit\"")]
    [InlineData("tempoBpm", "39")]
    [InlineData("tempoBpm", "241")]
    [InlineData("tempoBpm", "92.5")]
    [InlineData("tempoBpm", "\"92\"")]
    [InlineData("loop", "1")]
    [InlineData("score.bars", "0")]
    [InlineData("score.bars", "4097")]
    [InlineData("score.stepsPerBar", "32")]
    [InlineData("score.tracks", "null")]
    [InlineData("score.tracks.0.instrument", "0")]
    [InlineData("score.tracks.0.instrument", "\"0\"")]
    [InlineData("score.tracks.0.instrument", "\"BassDrum\"")]
    [InlineData("score.tracks.0.instrument", "\"metallicBeat\"")]
    [InlineData("score.tracks.0.instrument", "\"not-a-drum\"")]
    [InlineData("score.tracks.0.hits", "{}")]
    [InlineData("score.tracks.0.hits.0.bar", "-1")]
    [InlineData("score.tracks.0.hits.0.bar", "4")]
    [InlineData("score.tracks.0.hits.0.step", "-1")]
    [InlineData("score.tracks.0.hits.0.step", "16")]
    [InlineData("score.tracks.0.hits.0.velocity", "-0.1")]
    [InlineData("score.tracks.0.hits.0.velocity", "1.1")]
    [InlineData("score.tracks.0.hits.0.velocity", "\"NaN\"")]
    [InlineData("score.tracks.0.hits.0.velocity", "1e400")]
    [InlineData("score.tracks.0.hits.0.gateSteps", "0")]
    [InlineData("score.tracks.0.hits.0.gateSteps", "65537")]
    [InlineData("mixer.masterVolumePercent", "-1")]
    [InlineData("mixer.masterVolumePercent", "101")]
    [InlineData("mixer.masterVolumePercent", "1e400")]
    [InlineData("mixer.percussionVolumes.0.instrument", "0")]
    [InlineData("mixer.percussionVolumes.0.instrument", "\"0\"")]
    [InlineData("mixer.percussionVolumes.0.instrument", "\"metallicBeat\"")]
    [InlineData("mixer.percussionVolumes.0.volumePercent", "-1")]
    [InlineData("mixer.percussionVolumes.0.volumePercent", "101")]
    [InlineData("mixer.percussionVolumes.0.volumePercent", "\"NaN\"")]
    [InlineData("mixer.percussionVolumes.0.volumePercent", "0.5")]
    [InlineData("mixer.metallicEnabled", "\"false\"")]
    [InlineData("mixer.metallicVolumePercent", "-1")]
    [InlineData("mixer.metallicVolumePercent", "101")]
    public async Task Store_RejectsInvalidFieldValues(string propertyPath, string replacement)
    {
        using LoopTestFiles files = new();
        JsonObject root = CreateValidJson();
        LoopTestFiles.SetJsonValue(root, propertyPath, JsonNode.Parse(replacement));
        string path = files.File("invalid.drumloop.json");
        await System.IO.File.WriteAllTextAsync(path, root.ToJsonString(), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///  Requires every persisted musical field rather than silently substituting empty scores or default gains.
    /// </summary>
    [Theory]
    [InlineData("format")]
    [InlineData("version")]
    [InlineData("kit")]
    [InlineData("tempoBpm")]
    [InlineData("loop")]
    [InlineData("score")]
    [InlineData("score.bars")]
    [InlineData("score.stepsPerBar")]
    [InlineData("score.tracks")]
    [InlineData("score.tracks.0.instrument")]
    [InlineData("score.tracks.0.hits")]
    [InlineData("score.tracks.0.hits.0.bar")]
    [InlineData("score.tracks.0.hits.0.step")]
    [InlineData("score.tracks.0.hits.0.velocity")]
    [InlineData("score.tracks.0.hits.0.gateSteps")]
    [InlineData("mixer")]
    [InlineData("mixer.masterVolumePercent")]
    [InlineData("mixer.percussionVolumes")]
    [InlineData("mixer.percussionVolumes.0.instrument")]
    [InlineData("mixer.percussionVolumes.0.volumePercent")]
    [InlineData("mixer.metallicEnabled")]
    [InlineData("mixer.metallicVolumePercent")]
    public async Task Store_RejectsMissingFields(string propertyPath)
    {
        using LoopTestFiles files = new();
        JsonObject root = CreateValidJson();
        LoopTestFiles.RemoveJsonProperty(root, propertyPath);
        string path = files.File("missing.drumloop.json");
        await System.IO.File.WriteAllTextAsync(path, root.ToJsonString(), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///  Rejects duplicate tracks, mixer entries, and score cells even when the remaining values are valid.
    /// </summary>
    [Theory]
    [InlineData("duplicate-track")]
    [InlineData("duplicate-mixer")]
    [InlineData("duplicate-cell")]
    [InlineData("missing-track")]
    [InlineData("missing-mixer")]
    [InlineData("extra-track")]
    [InlineData("extra-mixer")]
    public async Task Store_RejectsAmbiguousOrIncompleteInstrumentCollections(string problem)
    {
        using LoopTestFiles files = new();
        JsonObject root = CreateValidJson();
        JsonArray tracks = root["score"]!["tracks"]!.AsArray();
        JsonArray mixer = root["mixer"]!["percussionVolumes"]!.AsArray();
        JsonArray hits = tracks[0]!["hits"]!.AsArray();
        switch (problem)
        {
            case "duplicate-track": tracks[1] = tracks[0]!.DeepClone(); break;
            case "duplicate-mixer": mixer[1] = mixer[0]!.DeepClone(); break;
            case "duplicate-cell": hits.Add(hits[0]!.DeepClone()); break;
            case "missing-track": tracks.RemoveAt(12); break;
            case "missing-mixer": mixer.RemoveAt(12); break;
            case "extra-track": tracks.Add(tracks[0]!.DeepClone()); break;
            case "extra-mixer": mixer.Add(mixer[0]!.DeepClone()); break;
        }

        string path = files.File("ambiguous.drumloop.json");
        await System.IO.File.WriteAllTextAsync(path, root.ToJsonString(), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///  Rejects malformed JSON, duplicate object properties, unknown fields, and excessive nesting.
    /// </summary>
    [Theory]
    [InlineData("empty")]
    [InlineData("null")]
    [InlineData("array")]
    [InlineData("malformed")]
    [InlineData("duplicate-root")]
    [InlineData("duplicate-nested")]
    [InlineData("unknown-field")]
    [InlineData("polymorphic-type")]
    [InlineData("trailing-json")]
    [InlineData("comment")]
    [InlineData("trailing-comma")]
    [InlineData("deep")]
    public async Task Store_RejectsMalformedOrUnexpectedJson(string problem)
    {
        using LoopTestFiles files = new();
        string json = CreateValidJson().ToJsonString();
        json = problem switch
        {
            "empty" => "",
            "null" => "null",
            "array" => "[]",
            "malformed" => "{not-json",
            "duplicate-root" => json.Insert(1, "\"version\":1,"),
            "duplicate-nested" => json.Replace("\"bar\":0", "\"bar\":0,\"bar\":1", StringComparison.Ordinal),
            "unknown-field" => json.Insert(1, "\"unexpected\":true,"),
            "polymorphic-type" => json.Insert(1, "\"$type\":\"System.Object\","),
            "trailing-json" => json + "{}",
            "comment" => "/* comment */" + json,
            "trailing-comma" => json.Insert(json.Length - 1, ","),
            "deep" => new string('[', 32) + "0" + new string(']', 32),
            _ => throw new ArgumentException(nameof(problem))
        };
        string path = files.File("malformed.drumloop.json");
        await System.IO.File.WriteAllTextAsync(path, json, TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///  Rejects oversized files before allocating their contents and rejects excessive otherwise valid events.
    /// </summary>
    [Fact]
    public async Task Store_EnforcesByteAndEventResourceBounds()
    {
        using LoopTestFiles files = new();
        string oversizedPath = files.File("oversized.drumloop.json");
        using (FileStream stream = new(oversizedPath, FileMode.CreateNew, FileAccess.Write))
        {
            stream.SetLength(LoopDocumentStore.MaximumFileBytes + 1L);
        }

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(oversizedPath, TestContext.Current.CancellationToken));

        JsonObject root = CreateValidJson();
        root["score"]!["bars"] = PercussionScore.MaximumBars;
        JsonArray tracks = root["score"]!["tracks"]!.AsArray();
        JsonArray firstTrack = tracks[0]!["hits"]!.AsArray();
        firstTrack.Clear();
        for (int index = 0; index < LoopDocument.MaximumHits; index++)
        {
            firstTrack.Add(new JsonObject
            {
                ["bar"] = index / 16,
                ["step"] = index % 16,
                ["velocity"] = 1,
                ["gateSteps"] = 1
            });
        }

        tracks[1]!["hits"]!.AsArray().Add(firstTrack[0]!.DeepClone());
        string excessiveHitsPath = files.File("too-many-hits.drumloop.json");
        await System.IO.File.WriteAllTextAsync(excessiveHitsPath, root.ToJsonString(), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            LoopDocumentStore.LoadAsync(excessiveHitsPath, TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///  Preserves the previous file after a denied replacement and removes only the operation's own staging file.
    /// </summary>
    [Fact]
    public async Task Save_FailurePreservesOldContentsAndUnrelatedStagingFiles()
    {
        using LoopTestFiles files = new();
        string path = files.File("existing.drumloop.json");
        string unrelated = files.File(".drum-json-unrelated.tmp");
        LoopDocument original = LoopDocument.CreateEmpty(4);
        await LoopDocumentStore.SaveAsync(original, path, TestContext.Current.CancellationToken);
        byte[] originalBytes = await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        await System.IO.File.WriteAllTextAsync(unrelated, "not owned by this save", TestContext.Current.CancellationToken);

        using (FileStream heldOpen = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            await Assert.ThrowsAnyAsync<IOException>(() =>
                LoopDocumentStore.SaveAsync(original.WithTempo(120), path, TestContext.Current.CancellationToken));
        }

        Assert.Equal(originalBytes, await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken));
        Assert.Equal("not owned by this save",
            await System.IO.File.ReadAllTextAsync(unrelated, TestContext.Current.CancellationToken));
        Assert.Equal([unrelated], Directory.GetFiles(files.DirectoryPath, "*.tmp"));

        await LoopDocumentStore.SaveAsync(original.WithTempo(120), path, TestContext.Current.CancellationToken);
        Assert.Equal(120, (await LoopDocumentStore.LoadAsync(path, TestContext.Current.CancellationToken)).TempoBpm);
        Assert.Equal([unrelated], Directory.GetFiles(files.DirectoryPath, "*.tmp"));
    }

    /// <summary>
    ///  Keeps the last good file when serialization fails after the staging file has already been created.
    /// </summary>
    [Fact]
    public async Task Save_SerializationFailureCannotPublishPartialJson()
    {
        using LoopTestFiles files = new();
        string path = files.File("existing.drumloop.json");
        await LoopDocumentStore.SaveAsync(LoopDocument.CreateEmpty(2), path, TestContext.Current.CancellationToken);
        byte[] original = await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() => JsonFileStorage.WriteAsync(path, writer =>
        {
            writer.WriteStartObject();
            writer.WriteNumber("version", 1);
            throw new InvalidOperationException("Injected serializer failure.");
        }, LoopDocumentStore.MaximumFileBytes, false, TestContext.Current.CancellationToken));

        Assert.Equal(original, await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken));
        Assert.Empty(Directory.EnumerateFiles(files.DirectoryPath, "*.tmp"));
    }

    /// <summary>
    ///  Allocates independent exclusively owned staging names when two serializations overlap in one directory.
    /// </summary>
    [Fact]
    public async Task Save_OverlappingOperationsUseDistinctStagingFiles()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        using LoopTestFiles files = new();
        using ManualResetEventSlim release = new();
        TaskCompletionSource firstEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource secondEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task first = StartWrite("first.json", firstEntered);
        Task second = StartWrite("second.json", secondEntered);
        try
        {
            await Task.WhenAll(firstEntered.Task, secondEntered.Task)
                .WaitAsync(TimeSpan.FromSeconds(15), cancellationToken);
            string[] stagingFiles = Directory.GetFiles(files.DirectoryPath, ".drum-json-*.tmp");
            Assert.Equal(2, stagingFiles.Length);
            Assert.NotEqual(stagingFiles[0], stagingFiles[1]);
        }
        finally
        {
            release.Set();
            await Task.WhenAll(first, second);
        }

        Assert.Empty(Directory.EnumerateFiles(files.DirectoryPath, "*.tmp"));

        Task StartWrite(string name, TaskCompletionSource entered)
            => JsonFileStorage.WriteAsync(files.File(name), writer =>
            {
                entered.SetResult();
                if (!release.Wait(TimeSpan.FromSeconds(30), cancellationToken))
                {
                    throw new TimeoutException("The test did not release its serializer.");
                }

                writer.WriteStartObject();
                writer.WriteString("name", name);
                writer.WriteEndObject();
            }, 1_024, false, cancellationToken);
    }

    /// <summary>
    ///  Propagates missing files and cancellation rather than returning a blank score or touching a saved file.
    /// </summary>
    [Fact]
    public async Task Store_PropagatesMissingFilesAndCancellation()
    {
        using LoopTestFiles files = new();
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            LoopDocumentStore.LoadAsync(files.File("missing.json"), TestContext.Current.CancellationToken));
        string path = files.File("saved.drumloop.json");
        LoopDocument original = LoopDocument.CreateEmpty(2);
        await LoopDocumentStore.SaveAsync(original, path, TestContext.Current.CancellationToken);
        byte[] originalBytes = await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken);
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => LoopDocumentStore.LoadAsync(path, cancellation.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            LoopDocumentStore.SaveAsync(original.WithTempo(120), path, cancellation.Token));
        Assert.Equal(originalBytes, await System.IO.File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken));
        Assert.Empty(Directory.EnumerateFiles(files.DirectoryPath, "*.tmp"));
    }

    private static JsonObject CreateValidJson()
    {
        JsonArray tracks = [];
        JsonArray levels = [];
        foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
        {
            string name = JsonNamingPolicy.CamelCase.ConvertName(instrument.ToString());
            tracks.Add(new JsonObject { ["instrument"] = name, ["hits"] = new JsonArray() });
            levels.Add(new JsonObject { ["instrument"] = name, ["volumePercent"] = 100 });
        }

        tracks[0]!["hits"]!.AsArray().Add(new JsonObject
        {
            ["bar"] = 0, ["step"] = 0, ["velocity"] = 0.37f, ["gateSteps"] = 7
        });
        return new JsonObject
        {
            ["format"] = "drumloop",
            ["version"] = 1,
            ["kit"] = "cr78-procedural-v1",
            ["tempoBpm"] = 92,
            ["loop"] = true,
            ["score"] = new JsonObject { ["bars"] = 4, ["stepsPerBar"] = 16, ["tracks"] = tracks },
            ["mixer"] = new JsonObject
            {
                ["masterVolumePercent"] = 65,
                ["percussionVolumes"] = levels,
                ["metallicEnabled"] = false,
                ["metallicVolumePercent"] = 0
            }
        };
    }
}

/// <summary>
///  Isolates file tests in an owned working-directory subfolder without touching user preferences or system temp paths.
/// </summary>
internal sealed class LoopTestFiles : IDisposable
{
    /// <summary>
    ///  Creates a unique artifact folder underneath the test process's working directory.
    /// </summary>
    internal LoopTestFiles()
    {
        DirectoryPath = Path.Combine(Environment.CurrentDirectory, $".drum-loop-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(DirectoryPath);
    }

    /// <summary>
    ///  Gets the exclusively owned test artifact directory.
    /// </summary>
    internal string DirectoryPath { get; }

    /// <summary>
    ///  Gets an isolated file or child directory path without creating anything outside the owned folder.
    /// </summary>
    internal string File(string name) => Path.Combine(DirectoryPath, name);

    /// <summary>
    ///  Replaces one existing JSON fixture value addressed by object names and zero-based array indices.
    /// </summary>
    internal static void SetJsonValue(JsonObject root, string propertyPath, JsonNode? value)
    {
        (JsonNode parent, string name) = FindParent(root, propertyPath);
        if (parent is JsonArray array)
        {
            array[int.Parse(name, System.Globalization.CultureInfo.InvariantCulture)] = value;
        }
        else
        {
            parent[name] = value;
        }
    }

    /// <summary>
    ///  Removes one required property from a complete fixture so tests exercise strict missing-field handling.
    /// </summary>
    internal static void RemoveJsonProperty(JsonObject root, string propertyPath)
    {
        (JsonNode parent, string name) = FindParent(root, propertyPath);
        parent.AsObject().Remove(name);
    }

    /// <summary>
    ///  Deletes only this test's uniquely named artifact subtree.
    /// </summary>
    public void Dispose() => Directory.Delete(DirectoryPath, recursive: true);

    private static (JsonNode Parent, string Name) FindParent(JsonObject root, string propertyPath)
    {
        string[] components = propertyPath.Split('.');
        JsonNode parent = root;
        foreach (string component in components[..^1])
        {
            parent = parent is JsonArray array
                ? array[int.Parse(component, System.Globalization.CultureInfo.InvariantCulture)]!
                : parent[component]!;
        }

        return (parent, components[^1]);
    }
}
