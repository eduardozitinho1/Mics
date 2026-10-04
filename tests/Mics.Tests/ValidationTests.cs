using Mics;
using Mics;
using Mics.Core.Audio;
using Mics.Core.Mapping;
using Xunit;

namespace Mics.Tests;

public class BpmValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-120)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Track_Bpm_rejects_invalid_values(double bpm)
    {
        var track = new Track();
        Assert.Throws<ArgumentOutOfRangeException>(() => track.Bpm(bpm));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(60)]
    [InlineData(120)]
    [InlineData(240.5)]
    public void Track_Bpm_accepts_positive_finite(double bpm)
    {
        var track = new Track().Bpm(bpm);
        Assert.Equal(bpm, track.Tempo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void SongBuilder_Tempo_rejects_invalid_values(double bpm)
    {
        var song = Sound.Song();
        Assert.Throws<ArgumentOutOfRangeException>(() => song.Tempo(bpm));
    }

    [Fact]
    public void SongBuilder_Tempo_accepts_positive()
    {
        var song = Sound.Song().Tempo(90);
        Assert.Equal(90, song.ToComposition().TempoBpm);
    }

    [Fact]
    public void SonifierOptions_rejects_zero_tempo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SonifierOptions { TempoBpm = 0 });
    }

    [Fact]
    public void SonifierOptions_rejects_negative_tempo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SonifierOptions { TempoBpm = -100 });
    }

    [Fact]
    public void SonifierOptions_accepts_valid_tempo()
    {
        var opts = new SonifierOptions { TempoBpm = 140 };
        Assert.Equal(140, opts.TempoBpm);
        Assert.Equal(60.0 / 140, opts.BeatSeconds, precision: 6);
    }
}

public class NoteAndRestValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Note_rejects_invalid_beats(double beats)
    {
        var track = new Track();
        Assert.Throws<ArgumentOutOfRangeException>(() => track.Note("C4", beats));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Chord_rejects_invalid_beats(double beats)
    {
        var track = new Track();
        Assert.Throws<ArgumentOutOfRangeException>(() => track.Chord(beats, "C4", "E4"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Rest_rejects_negative_or_non_finite(double beats)
    {
        var track = new Track();
        Assert.Throws<ArgumentOutOfRangeException>(() => track.Rest(beats));
    }

    [Fact]
    public void Rest_zero_is_allowed()
    {
        var track = new Track().Rest(0);
        Assert.Equal(0, track.DurationSeconds, precision: 6);
    }

    [Fact]
    public void Instrument_rejects_unknown_name()
    {
        var track = new Track();
        Assert.Throws<ArgumentException>(() => track.Instrument("piano"));
    }
}
