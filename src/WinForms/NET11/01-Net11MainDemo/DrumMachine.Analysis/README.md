# Percussion analysis

This offline tool fits the seventeen temporary reference WAVs to compact
modal/colored-noise models. The app requires neither Python nor those WAVs.
Run from `01-Net11MainDemo`.

```powershell
python -m pip install -r .\DrumMachine.Analysis\requirements.txt
python .\DrumMachine.Analysis\analyze.py .\DrumMachine.Demo\Sounds `
  --output .\WinForms.Audio\Percussion\MeasuredModels.json `
  --report .\fit-report.json --render .\fitted-audio
```

The output contains fitted frequencies, exponential decays, chirps, amplitudes,
phases of analytic modes, smoothed noise-spectrum gains, RMS envelopes, and
guiro scrape parameters. It never retains PCM or recorded noise phases.
`--only "Guiro 2"` refits one source and retains other saved model entries.
Full regeneration requires all seventeen named sources.

Kick regeneration preserves only the initial fitted attack, crossfading from
4-12 ms to one fixed 60 Hz carrier fitted to the early body. The kick/accent use
43/37 ms exponential decays and finish at 160/140 ms. Late chirped modes and
random bass residuals are deliberately excluded to prevent wavering tails.
The current body parameters were derived from the retained early modal fits;
the original WAVs are no longer present for another source comparison.

Render the **actual .NET stock synthesis** without opening an audio device:

```powershell
dotnet run --project .\DrumMachine.Analysis -- --render-stock .\actual-audio
python .\DrumMachine.Analysis\analyze.py .\DrumMachine.Demo\Sounds `
  --compare .\actual-audio --output .\WinForms.Audio\Percussion\MeasuredModels.json `
  --report .\comparison.json
```

Comparison does not rewrite models. It reports full-signal spectral centroid,
T90, cosine similarity of 48 smoothed spectral bands and 2 ms RMS envelopes.
Cosine similarity measures spectral/temporal shape, not a percentage of
perceived likeness. The export uses seed 42 for repeatable comparisons; live
playback randomizes its noise. Peaks and
RMS in reports are raw individual-voice levels. These exports intentionally
exclude the player's `PercussionOutputGain = 4f` (+12.04 dB) and peak limiter,
so changing playback calibration does not change spectral/temporal comparisons.
Listen to the renders as a separate perceptual check.

Validate WAVs through the **same decoder used for preset imports**:

```powershell
dotnet run --project .\DrumMachine.Analysis -- --validate-wavs .\DrumMachine.Demo\Sounds
```

Keep generated reports and listening WAVs outside source control. The original
WAV references can be removed after fitting without affecting stock playback.
