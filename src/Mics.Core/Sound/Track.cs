using Mics.Core.Audio;
using Mics.Core.Score;
using ScoreNote = Mics.Core.Score.Note;

namespace Mics;

/// <summary>
/// A fluent single-voice builder. Chain Note, Chord and Rest calls, then save.
/// </summary>
public sealed class Track
{
    private readonly List<ScoreNote> _notes = new();
    private double _time;
    private double _tempo = 120.0;
    private Instrument? _explicitInstrument;

    /// <summary>The notes added so far, in chronological order.</summary>
    public IReadOnlyList<ScoreNote> Notes => _notes;

    /// <summary>The tempo in beats per minute.</summary>
    public double Tempo => _tempo;

    /// <summary>The instrument explicitly set for this track, or null to use the default.</summary>
    public Instrument? ExplicitInstrument => _explicitInstrument;

    private double BeatsToSeconds(double beats) => beats * 60.0 / _tempo;

    /// <summary>Sets the tempo in beats per minute. Must be a positive, finite value.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bpm"/> is not positive or not finite.</exception>
    public Track Bpm(double bpm)
    {
        if (!double.IsFinite(bpm) || bpm <= 0)
            throw new ArgumentOutOfRangeException(nameof(bpm), bpm, "Tempo must be a positive, finite number.");

        _tempo = bpm;
        return this;
    }

    /// <summary>Forces a specific instrument (sine, triangle, square, sawtooth).</summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is not a known instrument.</exception>
    public Track Instrument(string name)
    {
        _explicitInstrument = Instruments.ByName(name)
            ?? throw new ArgumentException($"Unknown instrument: '{name}'", nameof(name));
        return this;
    }

    /// <summary>Appends a single note for the given number of beats.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="beats"/> is not positive or not finite.</exception>
    public Track Note(string name, double beats = 1.0)
    {
        if (!double.IsFinite(beats) || beats <= 0)
            throw new ArgumentOutOfRangeException(nameof(beats), beats, "Beats must be a positive, finite number.");

        var note = NoteName.Parse(name);
        var dur = BeatsToSeconds(beats);
        _notes.Add(new ScoreNote(note.Frequency, _time, dur, 0.7));
        _time += dur;
        return this;
    }

    /// <summary>Appends a chord (all notes start at the same time).</summary>
    public Track Chord(params string[] names) => Chord(1.0, names);

    /// <summary>Appends a chord with an explicit duration in beats.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="beats"/> is not positive or not finite.</exception>
    public Track Chord(double beats, params string[] names)
    {
        if (!double.IsFinite(beats) || beats <= 0)
            throw new ArgumentOutOfRangeException(nameof(beats), beats, "Beats must be a positive, finite number.");

        var dur = BeatsToSeconds(beats);
        foreach (var name in names)
        {
            var note = NoteName.Parse(name);
            _notes.Add(new ScoreNote(note.Frequency, _time, dur, 0.5));
        }
        _time += dur;
        return this;
    }

    /// <summary>Advances time without producing a sound.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="beats"/> is negative or not finite.</exception>
    public Track Rest(double beats = 1.0)
    {
        if (!double.IsFinite(beats) || beats < 0)
            throw new ArgumentOutOfRangeException(nameof(beats), beats, "Beats must be a non-negative, finite number.");

        _time += BeatsToSeconds(beats);
        return this;
    }

    /// <summary>Total duration of this track in seconds.</summary>
    public double DurationSeconds => _time;

    /// <summary>Builds a single-voice composition from this track.</summary>
    public Composition ToComposition()
    {
        var instrument = _explicitInstrument ?? Instruments.All[0];
        var voice = new Voice("track", instrument, _notes);
        return new Composition(_tempo, new[] { voice });
    }

    /// <summary>Returns the raw PCM samples for this track.</summary>
    public double[] ToSamples(int sampleRate = Sound.DefaultSampleRate)
        => Renderer.Render(ToComposition(), sampleRate);

    /// <summary>Returns the track encoded as a WAV file in memory.</summary>
    public byte[] ToWavBytes(int sampleRate = Sound.DefaultSampleRate)
    {
        var samples = ToSamples(sampleRate);
        using var ms = new MemoryStream();
        WavWriter.Write(ms, samples, sampleRate);
        return ms.ToArray();
    }

    /// <summary>Writes the track to a WAV file.</summary>
    public void Save(string path)
    {
        var samples = ToSamples();
        WavWriter.Write(path, samples);
    }

    internal Voice ToVoice(string name, int index)
    {
        var instrument = _explicitInstrument ?? Instruments.All[index % Instruments.All.Count];
        return new Voice(name, instrument, _notes);
    }
}
