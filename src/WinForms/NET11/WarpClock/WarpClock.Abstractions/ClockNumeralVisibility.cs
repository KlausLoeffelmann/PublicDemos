namespace WarpClock.Abstractions;

/// <summary>
///  How an hour numeral participates in the clock, used by the engine's
///  <c>MagneticNumerals</c> aiming. The three states deliberately separate "drawn"
///  from "targetable":
/// </summary>
/// <remarks>
///  <list type="bullet">
///   <item><see cref="Visible"/> — drawn and a valid magnetic target.</item>
///   <item><see cref="Transparent"/> — not drawn, but still placed on the canvas and
///    still a valid magnetic target (the hands take it into account).</item>
///   <item><see cref="Invisible"/> — not drawn and <b>not</b> a target: a hand that
///    would otherwise step onto it skips it and stays where it is.</item>
///  </list>
/// </remarks>
public enum ClockNumeralVisibility
{
    /// <summary>Drawn and targetable.</summary>
    Visible,

    /// <summary>Not drawn but still placed and targetable.</summary>
    Transparent,

    /// <summary>Not drawn and skipped by magnetic aiming (the hand stays put).</summary>
    Invisible,
}
