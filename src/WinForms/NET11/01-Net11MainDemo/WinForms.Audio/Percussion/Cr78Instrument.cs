namespace WinForms.Audio.Percussion;

/// <summary>
///  Stable sound identifiers for the fixed stock palette, legacy voices and auxiliary metallic layer.
/// </summary>
public enum Cr78Instrument
{
    /// <summary>
    ///  The low, damped bass-drum membrane.
    /// </summary>
    BassDrum,

    /// <summary>
    ///  A tuned drum body with a burst of filtered snare noise.
    /// </summary>
    SnareDrum,

    /// <summary>
    ///  A very short, hard rim resonance.
    /// </summary>
    RimShot,

    /// <summary>
    ///  A short high-passed noise burst.
    /// </summary>
    HiHat,

    /// <summary>
    ///  A longer, broader cymbal-noise decay.
    /// </summary>
    Cymbal,

    /// <summary>
    ///  A compact shaker-noise burst.
    /// </summary>
    Maracas,

    /// <summary>
    ///  A bright, damped wooden resonance.
    /// </summary>
    Claves,

    /// <summary>
    ///  Two non-harmonically related, band-limited bell tones.
    /// </summary>
    Cowbell,

    /// <summary>
    ///  The measured high bongo, with its principal resonance near 590 Hz.
    /// </summary>
    HighBongo,

    /// <summary>
    ///  The measured low bongo, with its principal resonance near 389 Hz.
    /// </summary>
    LowBongo,

    /// <summary>
    ///  The measured low conga, with its principal resonance near 196 Hz.
    /// </summary>
    LowConga,

    /// <summary>
    ///  A metallic and noisy jingle decay.
    /// </summary>
    Tambourine,

    /// <summary>
    ///  A gated, periodically excited scraping sound.
    /// </summary>
    Guiro,

    /// <summary>
    ///  The auxiliary cymbal/hi-hat metallic layer; available for audition, not a score track.
    /// </summary>
    MetallicBeat,

    /// <summary> An independently modeled accented kick drum. </summary>
    BassDrumAccent,

    /// <summary> An independently modeled accented snare drum. </summary>
    SnareDrumAccent,

    /// <summary> An independently modeled accented hi-hat. </summary>
    HiHatAccent,

    /// <summary> A metallic hi-hat strike, independent of the auxiliary metallic layer. </summary>
    MetallicHiHat,

    /// <summary> The second tambourine articulation. </summary>
    Tambourine2,

    /// <summary> A longer guiro articulation with a faster final scrape section. </summary>
    Guiro2
}
