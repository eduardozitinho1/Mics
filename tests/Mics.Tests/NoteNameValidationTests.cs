using Mics;
using Xunit;

namespace Mics.Tests;

public class NoteNameMidiValidationTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    [InlineData(128)]
    [InlineData(999999)]
    public void FromMidi_rejects_out_of_range(int midi)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NoteName.FromMidi(midi));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    [InlineData(127)]
    public void FromMidi_accepts_valid_range(int midi)
    {
        var n = NoteName.FromMidi(midi);
        Assert.Equal(midi, n.MidiNumber);
    }

    [Fact]
    public void Constructor_also_validates()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NoteName(200));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(128)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void TryFromMidi_returns_false_out_of_range(int midi)
    {
        Assert.False(NoteName.TryFromMidi(midi, out _));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    [InlineData(127)]
    public void TryFromMidi_returns_true_in_range(int midi)
    {
        Assert.True(NoteName.TryFromMidi(midi, out var n));
        Assert.Equal(midi, n.MidiNumber);
    }
}

public class NoteNameFrequencyValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-440)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void FromFrequency_rejects_invalid(double hz)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NoteName.FromFrequency(hz));
    }

    [Fact]
    public void FromFrequency_rejects_below_audible_range()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => NoteName.FromFrequency(1.0));
    }

    [Fact]
    public void FromFrequency_accepts_a4()
    {
        var n = NoteName.FromFrequency(440.0);
        Assert.Equal("A4", n.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(1.0)]
    public void TryFromFrequency_returns_false_for_invalid(double hz)
    {
        Assert.False(NoteName.TryFromFrequency(hz, out _));
    }

    [Fact]
    public void TryFromFrequency_returns_true_for_valid()
    {
        Assert.True(NoteName.TryFromFrequency(440.0, out var n));
        Assert.Equal("A4", n.ToString());
    }
}

public class NoteNameTryParseSafetyTests
{
    [Theory]
    [InlineData("C999999999999999999999999")]
    [InlineData("C2147483648")]
    [InlineData("C99999999999")]
    [InlineData("C-5")]
    public void TryParse_does_not_throw_for_extreme_input(string input)
    {
        var result = NoteName.TryParse(input, out _);
        Assert.False(result);
    }

    [Fact]
    public void TryParse_does_not_throw_for_overflowing_octave()
    {
        var huge = "C" + new string('9', 100);
        Assert.False(NoteName.TryParse(huge, out _));
    }

    [Fact]
    public void TryParse_accepts_valid_notes()
    {
        Assert.True(NoteName.TryParse("C4", out _));
        Assert.True(NoteName.TryParse("A#3", out _));
        Assert.True(NoteName.TryParse("Bb5", out _));
    }

    [Fact]
    public void TryParse_rejects_notes_out_of_midi_range()
    {
        Assert.False(NoteName.TryParse("C-2", out _));
        Assert.False(NoteName.TryParse("G10", out _));
    }

    [Theory]
    [InlineData(0, "C-1")]
    [InlineData(60, "C4")]
    [InlineData(127, "G9")]
    public void Boundary_midi_values_round_trip(int midi, string expected)
    {
        var n = NoteName.FromMidi(midi);
        Assert.Equal(expected, n.ToString());
    }
}
