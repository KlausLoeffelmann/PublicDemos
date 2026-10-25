using WinForms.Audio.Percussion;
using WinForms.Audio.Sequencing;

namespace DrumMachine.Demo;

/// <summary>
///  Provides original demonstration material rather than a transcription or manufacturer preset.
/// </summary>
internal static class DemoScores
{
    /// <summary>
    ///  Gets a sparse two-bar ballad groove with an alternating kick and small percussion responses.
    /// </summary>
    public static PercussionScore OriginalBallad { get; } = new(2,
    [
        new(0, 0, Cr78Instrument.BassDrum, 0.8f),
        new(0, 6, Cr78Instrument.BassDrum, 0.48f),
        new(0, 10, Cr78Instrument.BassDrum, 0.65f),
        new(0, 4, Cr78Instrument.SnareDrum, 0.55f),
        new(0, 12, Cr78Instrument.SnareDrum, 0.65f),
        new(0, 0, Cr78Instrument.HiHat, 0.32f),
        new(0, 2, Cr78Instrument.HiHat, 0.44f),
        new(0, 4, Cr78Instrument.HiHat, 0.28f),
        new(0, 6, Cr78Instrument.HiHat, 0.4f),
        new(0, 8, Cr78Instrument.HiHat, 0.34f),
        new(0, 10, Cr78Instrument.HiHat, 0.5f),
        new(0, 12, Cr78Instrument.HiHat, 0.3f),
        new(0, 14, Cr78Instrument.HiHat, 0.43f),
        new(0, 3, Cr78Instrument.Maracas, 0.32f),
        new(0, 7, Cr78Instrument.Maracas, 0.34f),
        new(0, 11, Cr78Instrument.Maracas, 0.3f),
        new(0, 15, Cr78Instrument.Maracas, 0.38f),
        new(0, 9, Cr78Instrument.Claves, 0.25f),
        new(1, 0, Cr78Instrument.BassDrum, 0.8f),
        new(1, 7, Cr78Instrument.BassDrum, 0.45f),
        new(1, 10, Cr78Instrument.BassDrum, 0.6f),
        new(1, 15, Cr78Instrument.BassDrum, 0.35f),
        new(1, 4, Cr78Instrument.SnareDrum, 0.55f),
        new(1, 12, Cr78Instrument.SnareDrum, 0.7f),
        new(1, 0, Cr78Instrument.HiHat, 0.32f),
        new(1, 2, Cr78Instrument.HiHat, 0.4f),
        new(1, 4, Cr78Instrument.HiHat, 0.3f),
        new(1, 6, Cr78Instrument.HiHat, 0.46f),
        new(1, 8, Cr78Instrument.HiHat, 0.34f),
        new(1, 10, Cr78Instrument.HiHat, 0.42f),
        new(1, 12, Cr78Instrument.HiHat, 0.3f),
        new(1, 14, Cr78Instrument.HiHat, 0.5f),
        new(1, 3, Cr78Instrument.Maracas, 0.3f),
        new(1, 7, Cr78Instrument.Maracas, 0.36f),
        new(1, 11, Cr78Instrument.Maracas, 0.3f),
        new(1, 15, Cr78Instrument.Maracas, 0.4f),
        new(1, 13, Cr78Instrument.HighBongo, 0.22f)
    ]);
}
