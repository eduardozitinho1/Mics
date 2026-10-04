using Mics.Core.Audio;
using Mics.Core.Score;

namespace Mics;

/// <summary>
/// Fluent builder for multi-voice songs. Each voice is a separate Track playing in parallel.
/// </summary>
public sealed class SongBuilder
{
    private readonly List<(string Name, Track Track)> _voices = new();
    private double _tempo = 120.0;

    /// <summary>
    /// Sets the global tempo in beats per minute. Must be a positive, finite value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bpm"/> is not positive or not finite.</exception>
    public SongBuilder Tempo(double bpm)
    {
        if (!double.IsFinite(bpm) || bpm <= 0)
            throw new ArgumentOutOfRangeException(nameof(bpm), bpm, "Tempo must be a positive, finite number.");

        _tempo = bpm;
        return this;
    }

    /// <summary>Adds a voice using the next available instrument automatically.</summary>
    public SongBuilder Voice(string name, Func<Track, Track> configure)
        => Voice(name, instrument: null, configure);

    /// <summary>Adds a voice with a specific instrument.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="configure"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the instrument name is not known.</exception>
    public SongBuilder Voice(string name, string? instrument, Func<Track, Track> configure)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(configure);

        var track = new Track().Bpm(_tempo);
        if (instrument is not null)
            track.Instrument(instrument);

        var configured = configure(track) ?? track;
        _voices.Add((name, configured));
        return this;
    }

    /// <summary>Builds the underlying composition model.</summary>
    public Composition ToComposition()
    {
        var list = new List<Voice>();
        for (int i = 0; i < _voices.Count; i++)
        {
            var (name, track) = _voices[i];
            list.Add(track.ToVoice(name, i));
        }
        return new Composition(_tempo, list);
    }

    /// <summary>Returns the mixed PCM samples for the whole song.</summary>
    public double[] ToSamples(int sampleRate = Renderer.DefaultSampleRate)
        => Renderer.Render(ToComposition(), sampleRate);

    /// <summary>Returns the song encoded as a WAV file in memory.</summary>
    public byte[] ToWavBytes(int sampleRate = Renderer.DefaultSampleRate)
    {
        var samples = ToSamples(sampleRate);
        using var ms = new MemoryStream();
        WavWriter.Write(ms, samples, sampleRate);
        return ms.ToArray();
    }

    /// <summary>Writes the song to a WAV file.</summary>
    public void Save(string path)
    {
        var samples = ToSamples();
        WavWriter.Write(path, samples);
    }
}
