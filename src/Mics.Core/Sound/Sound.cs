using Mics.Core.Audio;
using Mics.Core.Score;

namespace Mics;

/// <summary>
/// The main entry point of the Mics sound library.
/// </summary>
/// <remarks>
/// Use the static methods to create tracks from note names. Each returns a
/// <see cref="Track"/> you can chain and save.
///
/// <code>
/// Sound.Note("C4").Save("note.wav");
/// Sound.Melody("C4", "E4", "G4").Save("melody.wav");
/// Sound.Chord("C4", "E4", "G4").Save("chord.wav");
/// </code>
/// </remarks>
public static class Sound
{
    /// <summary>Default sample rate used when none is specified.</summary>
    public const int DefaultSampleRate = 22050;

    /// <summary>Starts a new track with a single note.</summary>
    public static Track Note(string name, double beats = 1.0)
        => new Track().Note(name, beats);

    /// <summary>Starts a new track with a melody, one beat per note.</summary>
    public static Track Melody(params string[] names)
        => Melody(1.0, names);

    /// <summary>Starts a new track with a melody at a fixed duration per note.</summary>
    public static Track Melody(double beatsPerNote, params string[] names)
    {
        var track = new Track();
        foreach (var name in names)
            track.Note(name, beatsPerNote);
        return track;
    }

    /// <summary>Starts a new track with a chord.</summary>
    public static Track Chord(params string[] names)
        => new Track().Chord(names);

    /// <summary>Starts a new track with a rest (silence).</summary>
    public static Track Rest(double beats = 1.0)
        => new Track().Rest(beats);

    /// <summary>Starts a new multi-voice song.</summary>
    public static SongBuilder Song() => new();

    /// <summary>Renders a composition to raw PCM samples.</summary>
    public static double[] ToSamples(Composition composition, int sampleRate = DefaultSampleRate)
        => Renderer.Render(composition, sampleRate);

    /// <summary>Renders a composition to a WAV byte array in memory.</summary>
    public static byte[] ToWavBytes(Composition composition, int sampleRate = DefaultSampleRate)
    {
        var samples = ToSamples(composition, sampleRate);
        using var ms = new MemoryStream();
        WavWriter.Write(ms, samples, sampleRate);
        return ms.ToArray();
    }

    /// <summary>Renders a composition and writes it to a WAV file.</summary>
    public static void Save(Composition composition, string path, int sampleRate = DefaultSampleRate)
    {
        var samples = ToSamples(composition, sampleRate);
        WavWriter.Write(path, samples, sampleRate);
    }
}
