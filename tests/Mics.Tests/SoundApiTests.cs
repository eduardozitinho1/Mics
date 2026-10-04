using Mics;
using Xunit;

namespace Mics.Tests;

public class SoundApiTests
{
    [Fact]
    public void Sound_Note_creates_a_track_with_one_note()
    {
        var track = Sound.Note("C4");
        Assert.Single(track.Notes);
        Assert.True(track.DurationSeconds > 0);
    }

    [Fact]
    public void Sound_Melody_creates_sequential_notes()
    {
        var track = Sound.Melody("C4", "E4", "G4");
        Assert.Equal(3, track.Notes.Count);
        Assert.Equal(0.0, track.Notes[0].StartTime, precision: 6);
        Assert.True(track.Notes[1].StartTime > track.Notes[0].StartTime);
        Assert.True(track.Notes[2].StartTime > track.Notes[1].StartTime);
    }

    [Fact]
    public void Sound_Chord_creates_simultaneous_notes()
    {
        var track = Sound.Chord("C4", "E4", "G4");
        Assert.Equal(3, track.Notes.Count);
        foreach (var n in track.Notes)
            Assert.Equal(0.0, n.StartTime, precision: 6);
    }

    [Fact]
    public void Sound_Rest_advances_time_without_notes()
    {
        var track = Sound.Note("C4").Rest(1).Note("E4");
        Assert.Equal(2, track.Notes.Count);
        Assert.True(track.Notes[1].StartTime > track.Notes[0].StartTime);
    }

    [Fact]
    public void Track_Bpm_affects_duration_of_subsequent_notes()
    {
        var fast = new Track().Bpm(240).Note("C4");
        var slow = new Track().Bpm(60).Note("C4");
        Assert.True(slow.DurationSeconds > fast.DurationSeconds);
    }

    [Fact]
    public void Track_Instrument_forces_specific_timbre()
    {
        var track = Sound.Note("C4").Instrument("square");
        var voice = track.ToComposition().Voices[0];
        Assert.Equal("square", voice.Instrument.Name);
    }

    [Fact]
    public void Track_ToWavBytes_produces_valid_header()
    {
        var bytes = Sound.Note("C4").ToWavBytes();
        Assert.True(bytes.Length > 44);
        Assert.Equal((byte)'R', bytes[0]);
        Assert.Equal((byte)'I', bytes[1]);
        Assert.Equal((byte)'F', bytes[2]);
        Assert.Equal((byte)'F', bytes[3]);
    }

    [Fact]
    public void Song_with_two_voices_produces_two_tracks()
    {
        var song = Sound.Song()
            .Voice("lead", t => t.Note("C4").Note("E4"))
            .Voice("bass", "sawtooth", t => t.Note("C2", 2));
        Assert.Equal(2, song.ToComposition().Voices.Count);
    }

    [Fact]
    public void Song_ToWavBytes_produces_non_empty_output()
    {
        var bytes = Sound.Song()
            .Voice("lead", t => t.Note("C4"))
            .Voice("bass", t => t.Note("C2"))
            .ToWavBytes();
        Assert.True(bytes.Length > 44);
    }

    [Fact]
    public void Song_tempo_is_propagated()
    {
        var song = Sound.Song().Tempo(90)
            .Voice("a", t => t.Note("C4"));
        Assert.Equal(90, song.ToComposition().TempoBpm);
    }
}
