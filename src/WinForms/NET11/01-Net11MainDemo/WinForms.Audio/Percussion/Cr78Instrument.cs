namespace WinForms.Audio.Percussion;

/// <summary>
///  The thirteen percussion sounds and the separately adjustable cymbal/hi-hat metallic layer.
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
    ///  The nominally 600 Hz bongo.
    /// </summary>
    HighBongo,

    /// <summary>
    ///  The nominally 400 Hz bongo.
    /// </summary>
    LowBongo,

    /// <summary>
    ///  The nominally 208 Hz conga.
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
    ///  The cymbal/hi-hat metallic layer; available for audition, not a fourteenth score track.
    /// </summary>
    MetallicBeat
}
