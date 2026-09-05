namespace WinForms.Audio.Sequencing;

/// <summary>
///  Describes whether the score is stopped, advancing, or holding its musical position.
/// </summary>
public enum DrumTransportState
{
    /// <summary>
    ///  No score events are scheduled; the next Start begins at the first bar.
    /// </summary>
    Stopped,

    /// <summary>
    ///  The score advances against the engine's sample clock.
    /// </summary>
    Playing,

    /// <summary>
    ///  The score holds its position while the engine clock and auditions continue.
    /// </summary>
    Paused
}
