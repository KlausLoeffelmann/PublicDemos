using System.Text;
using DrumMachine.Demo.Documents;
using WinForms.Audio.Music;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;
using WinForms.Audio.Sequencing;

namespace DrumMachine.Demo;

partial class MainForm
{
    private async void ExportWav_Click(object? sender, EventArgs e)
        => await RunDocumentCommandAsync(ExportWavAsync);

    private async Task ExportWavAsync()
    {
        LoopDocument document = _session.Current;
        int currentBar = Math.Clamp(CurrentBar, 0, document.Score.BarCount - 1);
        using ExportWaveDialog dialog = new(currentBar, document.Score.BarCount, document.TempoBpm);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        int bars = dialog.BarCount ?? document.Score.BarCount - dialog.FirstBar;
        if (bars <= 0 || ExportWaveDialog.MusicalLimitExceeded(document.TempoBpm, bars, dialog.Repetitions))
        {
            ReportExportRejected("A WAV export cannot exceed one hour.");
            return;
        }

        using SaveFileDialog save = new()
        {
            Title = "Export WAV",
            Filter = "WAV files (*.wav)|*.wav",
            DefaultExt = "wav",
            AddExtension = true,
            OverwritePrompt = true,
            CheckPathExists = true,
            InitialDirectory = FirstExistingDirectory(_settings.RecordingsFolder, _settings.DefaultFolder),
            FileName = BuildSuggestedWavName(_session.FilePath, dialog.Scope, dialog.FirstBar, dialog.Repetitions)
        };
        if (save.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string path = Path.GetFullPath(save.FileName);
        if (!string.Equals(Path.GetExtension(path), ".wav", StringComparison.OrdinalIgnoreCase))
        {
            ReportExportRejected("Choose a .wav file. Export will not replace the loop document.");
            return;
        }

        if (IsProtectedExportDestination(path))
        {
            ReportExportRejected("Choose a new WAV file. Export will not overwrite the loop document or an imported sound.");
            return;
        }

        int sampleRate = _engine?.SampleRate ?? 48_000;
        _statusLabel.Text = _soundPreset.WaveFiles.Count == 0
            ? "Exporting WAV..."
            : "Preparing sounds for WAV export...";
        IReadOnlyDictionary<Cr78Instrument, Sample>? samples = await ResolveExportSamplesAsync(sampleRate);
        PercussionWaveExportOptions options = new(document.Score, new Tempo(document.TempoBpm))
        {
            FirstBar = dialog.FirstBar,
            BarCount = dialog.BarCount,
            Repetitions = dialog.Repetitions,
            SampleRate = sampleRate,
            MasterVolume = document.MasterVolumePercent / 100f,
            InstrumentVolumes = Cr78Kit.Instruments.Select(instrument => document.PercussionVolumes[instrument] / 100f).ToArray(),
            MetallicEnabled = document.MetallicEnabled,
            MetallicLevel = document.MetallicVolumePercent / 100f,
            Samples = samples,
            IncludeTail = dialog.IncludeTail
        };

        _statusLabel.Text = "Exporting WAV...";
        await PercussionWaveExporter.ExportAsync(path, options, _lifetime.Token);
        string musical = ExportWaveDialog.FormatMusicalDuration(document.TempoBpm, bars, dialog.Repetitions);
        _statusLabel.Text = $"Exported {Path.GetFileName(path)} ({musical}).";
        AppLogger.Information("Export", $"Exported WAV '{path}' ({musical}, tail {dialog.IncludeTail}).");
    }

    private async Task<IReadOnlyDictionary<Cr78Instrument, Sample>?> ResolveExportSamplesAsync(int sampleRate)
    {
        if (_soundPreset.WaveFiles.Count == 0)
        {
            return null;
        }

        if (_preparedSamples is not null && _preparedRate == sampleRate && ExportSamplesMatch(_preparedSamples, sampleRate))
        {
            return _preparedSamples;
        }

        IReadOnlyDictionary<Cr78Instrument, Sample> prepared =
            await _soundPreset.PrepareAsync(sampleRate, _lifetime.Token);
        if (!ExportSamplesMatch(prepared, sampleRate))
        {
            throw new InvalidOperationException(
                "Imported sounds could not be prepared at the export rate. Export will not fall back to stock sounds.");
        }

        return prepared;
    }

    private bool ExportSamplesMatch(IReadOnlyDictionary<Cr78Instrument, Sample> samples, int sampleRate)
    {
        foreach (Cr78Instrument instrument in _soundPreset.WaveFiles.Keys)
        {
            if (!samples.TryGetValue(instrument, out Sample? sample) || sample.SampleRate != sampleRate)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsProtectedExportDestination(string destination)
    {
        if (PathsEqual(destination, _session.FilePath))
        {
            return true;
        }

        foreach (string source in _soundPreset.WaveFiles.Values)
        {
            if (PathsEqual(destination, source) ||
                string.Equals(destination, source, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private void ReportExportRejected(string message)
    {
        _statusLabel.Text = message;
        AppLogger.Warning("Export", message);
        if (!Automated && !_closing)
        {
            MessageBox.Show(this, message, "Export WAV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private static string BuildSuggestedWavName(string? sessionPath, WaveExportScope scope, int firstBar, int repetitions)
    {
        string baseName = SanitizeExportBaseName(sessionPath);
        string suffix = scope switch
        {
            WaveExportScope.CurrentBar => $"-bar-{firstBar + 1}",
            WaveExportScope.RepeatedLoop => $"-x{repetitions}",
            _ => ""
        };
        return baseName + suffix + ".wav";
    }

    private static string SanitizeExportBaseName(string? filePath)
    {
        string name = "Untitled";
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            name = Path.GetFileNameWithoutExtension(filePath);
            if (name.EndsWith(".drumloop", StringComparison.OrdinalIgnoreCase))
            {
                name = name[..^".drumloop".Length];
            }
        }

        char[] invalid = Path.GetInvalidFileNameChars();
        StringBuilder builder = new(name.Length);
        foreach (char ch in name.Trim())
        {
            builder.Append(Array.IndexOf(invalid, ch) >= 0 || char.IsControl(ch) ? '_' : ch);
        }

        string sanitized = builder.ToString().Trim().Trim('.');
        if (sanitized.Length > 80)
        {
            sanitized = sanitized[..80].Trim().Trim('.');
        }

        return string.IsNullOrWhiteSpace(sanitized) || IsReservedDeviceName(sanitized)
            ? "Untitled"
            : sanitized;
    }

    private static bool IsReservedDeviceName(string name)
    {
        string stem = name.Split('.')[0];
        return stem.Equals("CON", StringComparison.OrdinalIgnoreCase)
            || stem.Equals("PRN", StringComparison.OrdinalIgnoreCase)
            || stem.Equals("AUX", StringComparison.OrdinalIgnoreCase)
            || stem.Equals("NUL", StringComparison.OrdinalIgnoreCase)
            || stem.Length == 4
                && (stem.StartsWith("COM", StringComparison.OrdinalIgnoreCase)
                    || stem.StartsWith("LPT", StringComparison.OrdinalIgnoreCase))
                && char.IsDigit(stem[3]);
    }

    private static bool PathsEqual(string left, string? right)
    {
        if (string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        try
        {
            return string.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return false;
        }
    }
}
