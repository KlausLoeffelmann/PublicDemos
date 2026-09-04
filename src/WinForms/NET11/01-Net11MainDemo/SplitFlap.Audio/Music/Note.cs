using System.Text.RegularExpressions;

namespace SplitFlap.Audio.Music;

/// <summary>
///  Letter names. International: B, not H.
/// </summary>
public enum NoteName
{
    /// <summary>
    ///  C.
    /// </summary>
    C,

    /// <summary>
    ///  D.
    /// </summary>
    D,

    /// <summary>
    ///  E.
    /// </summary>
    E,

    /// <summary>
    ///  F.
    /// </summary>
    F,

    /// <summary>
    ///  G.
    /// </summary>
    G,

    /// <summary>
    ///  A. A4 is 440 Hz.
    /// </summary>
    A,

    /// <summary>
    ///  B. What German scores call H.
    /// </summary>
    B
}

/// <summary>
///  Raises or lowers a note by a semitone.
/// </summary>
public enum Accidental
{
    /// <summary>
    ///  As written.
    /// </summary>
    Natural,

    /// <summary>
    ///  A semitone up (#).
    /// </summary>
    Sharp,

    /// <summary>
    ///  A semitone down (b).
    /// </summary>
    Flat
}

/// <summary>
///  Note durations relative to a whole note (Notenwerte).
/// </summary>
public enum NoteValue
{
    /// <summary>
    ///  Four beats in 4/4.
    /// </summary>
    Whole = 1,

    /// <summary>
    ///  Two beats.
    /// </summary>
    Half = 2,

    /// <summary>
    ///  One beat.
    /// </summary>
    Quarter = 4,

    /// <summary>
    ///  Half a beat.
    /// </summary>
    Eighth = 8,

    /// <summary>
    ///  A quarter beat.
    /// </summary>
    Sixteenth = 16,

    /// <summary>
    ///  An eighth of a beat.
    /// </summary>
    ThirtySecond = 32
}

/// <summary>
///  How the note is attacked and how much of its duration it actually sounds (Anschlag).
/// </summary>
public enum Articulation
{
    /// <summary>
    ///  Sounds for ~85 % of its value.
    /// </summary>
    Normal,

    /// <summary>
    ///  Sounds for its full value, running into the next note.
    /// </summary>
    Legato,

    /// <summary>
    ///  Short: ~40 % of its value.
    /// </summary>
    Staccato,

    /// <summary>
    ///  Louder and slightly detached.
    /// </summary>
    Accent
}

/// <summary>
///  Decorations played on top of the written note.
/// </summary>
public enum Ornament
{
    /// <summary>
    ///  The note as written.
    /// </summary>
    None,

    /// <summary>
    ///  Rapid alternation with the note a whole step above (Triller).
    /// </summary>
    Trill,

    /// <summary>
    ///  A single quick alternation with the note above, then the main note (Mordent).
    /// </summary>
    Mordent
}

/// <summary>
///  One note or rest, as a musician would write it: name, accidental, octave, value, dots, triplet,
///  articulation, ornament. <see cref="Frequency"/> is derived, A4 = 440 Hz, equal temperament.
/// </summary>
public sealed partial record Note
{
    private static readonly int[] s_semitones = [0, 2, 4, 5, 7, 9, 11];

    /// <summary>
    ///  Letter name.
    /// </summary>
    public NoteName Name { get; init; } = NoteName.C;

    /// <summary>
    ///  Sharp, flat, or natural.
    /// </summary>
    public Accidental Accidental { get; init; } = Accidental.Natural;

    /// <summary>
    ///  Scientific pitch octave. Middle C is C4; A4 is 440 Hz.
    /// </summary>
    public int Octave { get; init; } = 4;

    /// <summary>
    ///  Duration relative to a whole note.
    /// </summary>
    public NoteValue Value { get; init; } = NoteValue.Quarter;

    /// <summary>
    ///  A dot adds half the value.
    /// </summary>
    public bool IsDotted { get; init; }

    /// <summary>
    ///  Three in the time of two (Triole).
    /// </summary>
    public bool IsTriplet { get; init; }

    /// <summary>
    ///  Attack and length within the value.
    /// </summary>
    public Articulation Articulation { get; init; } = Articulation.Normal;

    /// <summary>
    ///  Trill, mordent, or nothing.
    /// </summary>
    public Ornament Ornament { get; init; } = Ornament.None;

    /// <summary>
    ///  A rest: takes time, makes no sound.
    /// </summary>
    public bool IsRest { get; init; }

    /// <summary>
    ///  <see langword="true"/> for a sharp.
    /// </summary>
    public bool IsSharp
        => Accidental is Accidental.Sharp;

    /// <summary>
    ///  <see langword="true"/> for a flat.
    /// </summary>
    public bool IsFlat
        => Accidental is Accidental.Flat;

    /// <summary>
    ///  MIDI note number (C4 = 60, A4 = 69).
    /// </summary>
    public int MidiNumber
        => (Octave + 1) * 12 + s_semitones[(int)Name] + Accidental switch
        {
            Accidental.Sharp => 1,
            Accidental.Flat => -1,
            _ => 0
        };

    /// <summary>
    ///  Pitch in Hz.
    /// </summary>
    public double Frequency
        => IsRest ? 0 : FrequencyOf(MidiNumber);

    /// <summary>
    ///  The value including dot and triplet, as a fraction of a whole note.
    /// </summary>
    public double WholeNoteFraction
    {
        get
        {
            double fraction = 1.0 / (int)Value;

            if (IsDotted)
            {
                fraction *= 1.5;
            }

            if (IsTriplet)
            {
                fraction *= 2.0 / 3.0;
            }

            return fraction;
        }
    }

    /// <summary>
    ///  A rest of a given value.
    /// </summary>
    public static Note Rest(NoteValue value = NoteValue.Quarter)
        => new() { IsRest = true, Value = value };

    /// <summary>
    ///  Frequency for a MIDI note number, A4 = 440.
    /// </summary>
    public static double FrequencyOf(int midiNumber)
        => 440.0 * Math.Pow(2, (midiNumber - 69) / 12.0);

    /// <summary>
    ///  Frequency <paramref name="semitones"/> above (or below) this note.
    /// </summary>
    public double FrequencyTransposed(int semitones)
        => FrequencyOf(MidiNumber + semitones);

    /// <summary>
    ///  Parses IML-style notation: <c>C4-4</c>, <c>F#3-8</c>, <c>Bb5-2.</c> (dotted), <c>E4-8t</c> (triplet),
    ///  <c>R-4</c> (rest). Octave defaults to 4, value to a quarter. Suffix <c>!</c> = accent, <c>'</c> = staccato,
    ///  <c>_</c> = legato, <c>~</c> = trill.
    /// </summary>
    public static Note Parse(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Match match = Syntax().Match(text.Trim());

        if (!match.Success)
        {
            throw new FormatException($"'{text}' is not a note. Try C4-4, F#3-8, Bb5-2. or R-4.");
        }

        char letter = char.ToUpperInvariant(match.Groups["name"].Value[0]);
        string modifiers = match.Groups["mod"].Value;

        return new Note
        {
            IsRest = letter is 'R',
            Name = letter is 'R' ? NoteName.C : Enum.Parse<NoteName>(letter.ToString()),
            Accidental = match.Groups["acc"].Value switch
            {
                "#" => Accidental.Sharp,
                "b" => Accidental.Flat,
                _ => Accidental.Natural
            },
            Octave = match.Groups["oct"].Success ? int.Parse(match.Groups["oct"].Value) : 4,
            Value = match.Groups["val"].Success ? (NoteValue)int.Parse(match.Groups["val"].Value) : NoteValue.Quarter,
            IsDotted = match.Groups["dot"].Success,
            IsTriplet = match.Groups["trip"].Success,
            Articulation = modifiers.Contains('!') ? Articulation.Accent
                : modifiers.Contains('\'') ? Articulation.Staccato
                : modifiers.Contains('_') ? Articulation.Legato
                : Articulation.Normal,
            Ornament = modifiers.Contains('~') ? Ornament.Trill : Ornament.None
        };
    }

    /// <summary>
    ///  Parses a whitespace- or comma-separated sequence of notes.
    /// </summary>
    public static IReadOnlyList<Note> ParseMelody(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return [.. text.Split([' ', ',', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(Parse)];
    }

    /// <inheritdoc/>
    public override string ToString()
        => IsRest
            ? $"R-{(int)Value}{(IsDotted ? "." : "")}{(IsTriplet ? "t" : "")}"
            : $"{Name}{(IsSharp ? "#" : IsFlat ? "b" : "")}{Octave}-{(int)Value}{(IsDotted ? "." : "")}{(IsTriplet ? "t" : "")}";

    [GeneratedRegex(@"^(?<name>[A-Ga-gRr])(?<acc>[#b])?(?<oct>\d)?(?:-(?<val>1|2|4|8|16|32)(?<dot>\.)?(?<trip>t)?)?(?<mod>[!'_~]*)$")]
    private static partial Regex Syntax();
}

/// <summary>
///  Beats per minute, where a beat is a quarter note. With the Italian names, because a score says
///  "Allegro", not "130".
/// </summary>
/// <param name="BeatsPerMinute">Quarter notes per minute.</param>
public readonly record struct Tempo(int BeatsPerMinute = 120)
{
    /// <summary>
    ///  ~50 bpm.
    /// </summary>
    public static Tempo Largo => new(50);

    /// <summary>
    ///  ~70 bpm.
    /// </summary>
    public static Tempo Adagio => new(70);

    /// <summary>
    ///  ~90 bpm.
    /// </summary>
    public static Tempo Andante => new(90);

    /// <summary>
    ///  ~110 bpm.
    /// </summary>
    public static Tempo Moderato => new(110);

    /// <summary>
    ///  ~130 bpm.
    /// </summary>
    public static Tempo Allegro => new(130);

    /// <summary>
    ///  ~160 bpm.
    /// </summary>
    public static Tempo Vivace => new(160);

    /// <summary>
    ///  ~180 bpm.
    /// </summary>
    public static Tempo Presto => new(180);

    /// <summary>
    ///  Duration of one quarter note.
    /// </summary>
    public TimeSpan Beat
        => TimeSpan.FromSeconds(60.0 / Math.Max(1, BeatsPerMinute));

    /// <summary>
    ///  Duration of a note at this tempo, including dot and triplet.
    /// </summary>
    public TimeSpan DurationOf(Note note)
        => TimeSpan.FromSeconds(Beat.TotalSeconds * 4 * note.WholeNoteFraction);
}
