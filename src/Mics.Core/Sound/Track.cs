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

    internal IReadOnlyList<ScoreNote> Notes => _notes;
    internal double Tempo => _tempo;
    internal Instrument? ExplicitInstrument => _explicitInstrument;

    private double BeatsToSeconds(double beats) => beats * 60.0 / _tempo;

    /// <summary>Sets the tempo in beats per minute.</summary>
    public Track Bpm(double bpm)
    {
        _tempo = bpm;
        return this;
    }

    /// <summary>Forces a specific instrument (sine, triangle, square, sawtooth).</summary>
    public Track Instrument(string name)
    {
        _explicitInstrument = Instruments.ByName(name)
            ?? throw new ArgumentException($"Unknown instrument: '{name}'", nameof(name));
        return this;
    }

    /// <summary>Appends a single note for the given number of beats.</summary>
    public Track Note(string name, double beats = 1.0)
    {
        var note = NoteName.Parse(name);
        var dur = BeatsToSeconds(beats);
        _notes.Add(new ScoreNote(note.Frequency, _time, dur, 0.7));
        _time += dur;
        return this;
    }

    /// <summary>Appends a chord (all notes start at the same time).</summary>
    public Track Chord(params string[] names) => Chord(1.0, names);

    /// <summary>Appends a chord with an explicit duration in beats.</summary>
    public Track Chord(double beats, params string[] names)
    {
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
    public Track Rest(double beats = 1.0)
    {
        _time += BeatsToSeconds(beats);
        return this;
    }

    /// <summary>Total duration of this track in seconds.</summary>
    public double DurationSeconds => _time;

    internal Voice ToVoice(string name, int index)
    {
        var instrument = _explicitInstrument ?? Instruments.All[index % Instruments.All.Count];
        return new Voice(name, instrument, _notes);
    }

    internal Composition ToComposition()
    {
        var voice = ToVoice("track", 0);
        return new Composition(_tempo, new[] { voice });
    }

    /// <summary>Returns the raw PCM samples for this track.</summary>
    public double[] ToSamples(int sampleRate = Renderer.DefaultSampleRate)
        => Renderer.Render(ToComposition(), sampleRate);

    /// <summary>Returns the track encoded as a WAV file in memory.</summary>
    public byte[] ToWavBytes(int sampleRate = Renderer.DefaultSampleRate)
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
}
