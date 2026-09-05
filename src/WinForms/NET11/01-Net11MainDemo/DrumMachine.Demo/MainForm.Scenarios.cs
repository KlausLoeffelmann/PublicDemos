using System.Diagnostics;
using DrumMachine.Demo.Documents;
using SplitFlap.Audio.Analysis;
using SplitFlap.Audio.Core;
using SplitFlap.Audio.Percussion;
using SplitFlap.Audio.Playback;
using SplitFlap.Audio.Sequencing;

namespace DrumMachine.Demo;

partial class MainForm
{
    private async Task RunScenarioAsync(DemoScenario scenario, CancellationToken cancellation)
    {
        AudioEngine engine = _engine ?? throw new InvalidOperationException("The audio engine is unavailable.");
        DrumMachinePlayer player = _player ?? throw new InvalidOperationException("The drum player is unavailable.");
        AppLogger.Information("Harness", $"Running {scenario}.");

        if (scenario is DemoScenario.Kit or DemoScenario.All)
        {
            foreach (Cr78Instrument instrument in Cr78Kit.Instruments.Append(Cr78Instrument.MetallicBeat))
            {
                _statusLabel.Text = $"Audition: {Cr78Kit.GetDisplayName(instrument)}";
                await engine.Play(Cr78Kit.CreateVoice(engine.SampleRate, instrument, 0.75f))
                    .WaitAsync(TimeSpan.FromSeconds(5), cancellation);
                await Task.Delay(50, cancellation);
            }
        }

        if (scenario is DemoScenario.Score or DemoScenario.All)
        {
            player.Loop = false;
            player.Start();
            await WaitUntilAsync(() => player.IsPlaying, TimeSpan.FromSeconds(5), cancellation);
            await WaitUntilAsync(() => !player.IsPlaying, TimeSpan.FromSeconds(20), cancellation);
        }

        if (scenario is DemoScenario.Spectrum or DemoScenario.All)
        {
            const double frequency = 1_000;
            VoiceChannel channel = engine.CreateChannel(VoicePatch.Default with { Volume = 0.25f });
            Task tone = channel.PlaySoundAsync(frequency, TimeSpan.FromMilliseconds(900), cancellation);
            await WaitUntilAsync(() => SpectrumContains(frequency), TimeSpan.FromSeconds(3), cancellation);
            await tone.WaitAsync(TimeSpan.FromSeconds(3), cancellation);
        }

        if (scenario is DemoScenario.Document or DemoScenario.All)
        {
            await RunDocumentScenarioAsync(cancellation);
        }
    }

    private async Task RunDocumentScenarioAsync(CancellationToken cancellation)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"DrumMachine-{Guid.NewGuid():N}");
        string path = Path.Combine(directory, "roundtrip.drumloop.json");
        Directory.CreateDirectory(directory);
        LoopDocument original = _session.Current;
        int oldView = _settings.BarsPerView;
        try
        {
            LoopDocument edited = original.WithMasterVolume(45)
                .WithInstrumentVolume(Cr78Instrument.SnareDrum, 35).WithMetallic(true, 30).WithTempo(120);
            ReplaceDocument(edited, null);
            await LoopDocumentStore.SaveAsync(edited, path, cancellation);
            LoopDocument loaded = await LoopDocumentStore.LoadAsync(path, cancellation);
            if (!loaded.ValueEquals(edited))
            {
                throw new InvalidOperationException("Loop file round-trip changed score or mixer values.");
            }
            ReplaceDocument(loaded, path);
            ChangeView(2);
            ApplyEdit(loaded.WithMasterVolume(55), "Scenario volume edit");
            Undo_Click(this, EventArgs.Empty);
            if (!_session.Current.ValueEquals(loaded))
            {
                throw new InvalidOperationException("Undo did not restore the saved loop.");
            }
            Redo_Click(this, EventArgs.Empty);
            if (_session.Current.MasterVolumePercent != 55)
            {
                throw new InvalidOperationException("Redo did not restore the volume edit.");
            }

            DrumMachinePlayer player = _player ?? throw new InvalidOperationException("The player is unavailable.");
            player.Start();
            await Task.Delay(200, cancellation);
            player.Pause();
            await WaitUntilAsync(() => player.GetPlaybackSnapshot().IsPaused, TimeSpan.FromSeconds(3), cancellation);
            player.Start();
            await WaitUntilAsync(() => player.GetPlaybackSnapshot().IsPlaying, TimeSpan.FromSeconds(3), cancellation);
            player.Stop();
            await WaitUntilAsync(
                () => player.GetPlaybackSnapshot() is { State: DrumTransportState.Stopped, Bar: 0, Step: 0 },
                TimeSpan.FromSeconds(3), cancellation);
        }
        finally
        {
            if (!_closing)
            {
                _settings = _settings with { BarsPerView = oldView };
                ReplaceDocument(original, null);
            }
            File.Delete(path);
            Directory.Delete(directory);
        }
    }

    private bool SpectrumContains(double frequency)
    {
        if (_spectrum is null || !_spectrum.TryCopySpectrum(_spectrumReadback, out AudioSpectrumFrame frame))
        {
            return false;
        }

        double binWidth = frame.SampleRate / (double)frame.FftSize;
        // PeakLevel is dBFS, not linear amplitude: a healthy tone normally stays below zero.
        return frame.IsPlaybackSynchronized && frame.PeakLevel > -60f
            && Math.Abs(frame.PeakFrequency - frequency) <= binWidth * 2;
    }

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout, CancellationToken cancellation)
    {
        Stopwatch clock = Stopwatch.StartNew();
        while (!condition())
        {
            cancellation.ThrowIfCancellationRequested();
            if (clock.Elapsed >= timeout)
            {
                throw new TimeoutException("The audio scenario did not reach its expected state.");
            }
            await Task.Delay(20, cancellation);
        }
    }
}
