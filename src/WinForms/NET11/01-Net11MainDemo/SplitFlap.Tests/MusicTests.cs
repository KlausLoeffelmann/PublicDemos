using SplitFlap.Audio.Music;

namespace SplitFlap.Tests;

public sealed class MusicTests
{
    [Theory]
    [InlineData("A4-4", 69, 440d)]
    [InlineData("C4-8", 60, 261.625565d)]
    [InlineData("Bb3-2.", 58, 233.081881d)]
    public void Parse_ProducesExpectedPitch(string text, int midi, double frequency)
    {
        Note note = Note.Parse(text);

        Assert.Equal(midi, note.MidiNumber);
        Assert.Equal(frequency, note.Frequency, precision: 4);
    }

    [Fact]
    public void ParseMelody_HandlesWhitespaceAndCommas()
    {
        IReadOnlyList<Note> notes = Note.ParseMelody("C4-4, E4-4\tG4-2");

        Assert.Equal(3, notes.Count);
        Assert.Equal(NoteName.G, notes[2].Name);
    }

    [Fact]
    public void Parse_InvalidNotationThrowsUsefulError()
    {
        FormatException exception = Assert.Throws<FormatException>(() => Note.Parse("H4-3"));

        Assert.Contains("Try C4-4", exception.Message);
    }

    [Fact]
    public void Tempo_DurationIncludesDotsAndTriplets()
    {
        Tempo tempo = new(120);
        Note dotted = new() { Value = NoteValue.Quarter, IsDotted = true };
        Note triplet = new() { Value = NoteValue.Quarter, IsTriplet = true };

        Assert.Equal(TimeSpan.FromMilliseconds(750), tempo.DurationOf(dotted));
        Assert.Equal(TimeSpan.FromMilliseconds(1000d / 3d), tempo.DurationOf(triplet));
    }
}
