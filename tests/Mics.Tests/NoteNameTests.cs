using Mics;
using Xunit;

namespace Mics.Tests;

public class NoteNameTests
{
    [Theory]
    [InlineData("C4", 60)]
    [InlineData("A4", 69)]
    [InlineData("C5", 72)]
    [InlineData("C3", 48)]
    [InlineData("B3", 59)]
    public void Parse_returns_correct_midi_number(string text, int expectedMidi)
    {
        var n = NoteName.Parse(text);
        Assert.Equal(expectedMidi, n.MidiNumber);
    }

    [Fact]
    public void A4_is_440_hz()
    {
        var n = NoteName.Parse("A4");
        Assert.Equal(440.0, n.Frequency, precision: 4);
    }

    [Fact]
    public void C4_is_approximately_261_hz()
    {
        var n = NoteName.Parse("C4");
        Assert.InRange(n.Frequency, 261.0, 262.0);
    }

    [Theory]
    [InlineData("c4", "C4")]
    [InlineData("A#3", "A#3")]
    [InlineData("Bb3", "A#3")]
    [InlineData("Db5", "C#5")]
    public void Parse_normalizes_enharmonic_spellings(string input, string canonical)
    {
        var n = NoteName.Parse(input);
        Assert.Equal(canonical, n.ToString());
    }

    [Fact]
    public void Octave_is_computed_correctly()
    {
        Assert.Equal(4, NoteName.Parse("C4").Octave);
        Assert.Equal(5, NoteName.Parse("C5").Octave);
        Assert.Equal(3, NoteName.Parse("C3").Octave);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("X4")]
    [InlineData("C")]
    [InlineData("C#")]
    [InlineData("4C")]
    [InlineData("Hello")]
    [InlineData("C999")]
    public void TryParse_returns_false_for_invalid_input(string input)
    {
        Assert.False(NoteName.TryParse(input, out _));
    }

    [Fact]
    public void Parse_throws_for_invalid_input()
    {
        Assert.Throws<FormatException>(() => NoteName.Parse("not-a-note"));
    }

    [Fact]
    public void FromMidi_round_trips_via_ToString()
    {
        var n = NoteName.FromMidi(60);
        Assert.Equal("C4", n.ToString());
    }

    [Fact]
    public void FromFrequency_finds_closest_note()
    {
        var n = NoteName.FromFrequency(440.0);
        Assert.Equal("A4", n.ToString());
    }
}
