using System.Text.Json;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;

namespace DrumMachine.Demo.Documents;

/// <summary>
///  Publishes portable kits only after their copied WAV assets have been completely validated.
/// </summary>
internal static class PercussionSoundPresetStore
{
    private const int MaximumManifestBytes = 64 * 1024;

    /// <summary> The extension of a portable percussion sound preset manifest. </summary>
    public const string FileExtension = ".drumkit.json";

    /// <summary>
    ///  Reads a portable manifest and validates every referenced WAV before returning a replacement.
    /// </summary>
    public static Task<PercussionSoundPreset> LoadAsync(string path, CancellationToken cancellationToken = default)
        => Task.Run(async () =>
        {
            string manifest = Path.GetFullPath(path);
            byte[] bytes = await JsonFileStorage.ReadAsync(manifest, MaximumManifestBytes, cancellationToken)
                .ConfigureAwait(false);
            using JsonDocument json = StrictJson.Parse(bytes);
            JsonElement root = json.RootElement;
            StrictJson.RequireProperties(root, "format", "version", "name", "sounds");
            if (StrictJson.String(root.GetProperty("format"), "format") != "drumkit")
            {
                throw new InvalidDataException("This file is not a percussion sound preset.");
            }

            StrictJson.Integer(root.GetProperty("version"), "version", 1, 1);
            string name = StrictJson.String(root.GetProperty("name"), "name");
            JsonElement sounds = root.GetProperty("sounds");
            StrictJson.RequireArray(sounds, "sounds");
            if (sounds.GetArrayLength() > Cr78Kit.Instruments.Count)
            {
                throw new InvalidDataException("A sound preset cannot add percussion tracks.");
            }

            string directory = Path.GetDirectoryName(manifest)!;
            Dictionary<Cr78Instrument, string> files = [];
            foreach (JsonElement sound in sounds.EnumerateArray())
            {
                StrictJson.RequireProperties(sound, "instrument", "file");
                string token = StrictJson.String(sound.GetProperty("instrument"), "instrument");
                if (!Enum.TryParse(token, out Cr78Instrument instrument) || instrument.ToString() != token ||
                    !Cr78Kit.Instruments.Contains(instrument))
                {
                    throw new InvalidDataException($"Unknown fixed percussion track '{token}'.");
                }

                string relative = StrictJson.String(sound.GetProperty("file"), "file");
                string fullPath = ResolveAsset(directory, relative);
                if (!files.TryAdd(instrument, fullPath))
                {
                    throw new InvalidDataException($"Duplicate sound mapping '{token}'.");
                }
            }

            PercussionSoundPreset preset = new(name, files);
            await preset.PrepareAsync(48_000, cancellationToken).ConfigureAwait(false);
            return preset;
        }, cancellationToken);

    /// <summary>
    ///  Copies the original WAV bytes into an exclusively owned adjacent asset folder, then atomically
    ///  publishes the manifest. A failed or cancelled save never damages an existing preset.
    /// </summary>
    public static Task SaveAsync(
        PercussionSoundPreset preset, string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(preset);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return Task.Run(async () =>
        {
            string manifest = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(manifest)!;
            Directory.CreateDirectory(directory);
            string assetName = $"{Path.GetFileNameWithoutExtension(manifest)}.assets-{Guid.NewGuid():N}";
            string assets = Path.Combine(directory, assetName);
            List<string> ownedFiles = [];
            bool ownsDirectory = false;
            bool published = false;
            try
            {
                Dictionary<Cr78Instrument, string> relativePaths = [];
                foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
                {
                    if (!preset.WaveFiles.TryGetValue(instrument, out string? source))
                    {
                        continue;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    if (!ownsDirectory)
                    {
                        Directory.CreateDirectory(assets);
                        ownsDirectory = true;
                    }

                    string filename = $"{instrument}.wav";
                    string destination = Path.Combine(assets, filename);
                    await using (FileStream input = new(source, FileMode.Open, FileAccess.Read, FileShare.Read,
                        65_536, FileOptions.Asynchronous | FileOptions.SequentialScan))
                    {
                        if (input.Length > Sample.MaximumWaveFileBytes)
                        {
                            throw new InvalidDataException("The WAV file exceeds the supported import size.");
                        }

                        await using FileStream output = new(destination, FileMode.CreateNew, FileAccess.Write,
                            FileShare.None, 65_536, FileOptions.Asynchronous);
                        ownedFiles.Add(destination);
                        byte[] buffer = new byte[65_536];
                        int count;
                        long total = 0;
                        while ((count = await input.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
                        {
                            total += count;
                            if (total > Sample.MaximumWaveFileBytes)
                            {
                                throw new InvalidDataException("The WAV file grew beyond the supported import size.");
                            }

                            await output.WriteAsync(buffer.AsMemory(0, count), cancellationToken).ConfigureAwait(false);
                        }

                        await output.FlushAsync(cancellationToken).ConfigureAwait(false);
                        output.Flush(flushToDisk: true);
                    }

                    Sample sample = await Sample.FromWaveFileAsync(destination, 48_000, cancellationToken)
                        .ConfigureAwait(false);
                    if (sample.Duration > TimeSpan.FromSeconds(30))
                    {
                        throw new InvalidDataException($"'{Path.GetFileName(source)}' exceeds the thirty-second percussion limit.");
                    }

                    relativePaths.Add(instrument, Path.Combine(assetName, filename));
                }

                await JsonFileStorage.WriteAsync(manifest, writer =>
                {
                    writer.WriteStartObject();
                    writer.WriteString("format", "drumkit");
                    writer.WriteNumber("version", 1);
                    writer.WriteString("name", preset.Name);
                    writer.WriteStartArray("sounds");
                    foreach ((Cr78Instrument instrument, string relative) in relativePaths)
                    {
                        writer.WriteStartObject();
                        writer.WriteString("instrument", instrument.ToString());
                        writer.WriteString("file", relative);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }, MaximumManifestBytes, createDirectory: false, cancellationToken).ConfigureAwait(false);
                published = true;
            }
            finally
            {
                if (!published && ownsDirectory)
                {
                    try
                    {
                        foreach (string owned in ownedFiles)
                        {
                            File.Delete(owned);
                        }

                        Directory.Delete(assets, recursive: false);
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        AppLogger.Warning("Presets", $"Could not remove unpublished preset assets '{assets}'.", ex);
                    }
                }
            }
        }, cancellationToken);
    }

    private static string ResolveAsset(string directory, string relative)
    {
        if (string.IsNullOrWhiteSpace(relative) || Path.IsPathRooted(relative) ||
            !string.Equals(Path.GetExtension(relative), ".wav", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("Preset WAV paths must be relative to the preset folder.");
        }

        string fullPath = Path.GetFullPath(Path.Combine(directory, relative));
        string root = Path.TrimEndingDirectorySeparator(directory) + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) ||
            relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(part => part is "." or ".." || part.Contains(':')))
        {
            throw new InvalidDataException("A preset WAV path cannot escape its preset folder.");
        }

        return fullPath;
    }
}
