namespace Mics.Core.Mapping;

/// <summary>
/// Musical scales used to map syntax nodes to pitches.
/// </summary>
/// <remarks>
/// Every frequency is computed through <see cref="NoteName"/>, so the equal-temperament
/// formula lives in exactly one place. A scale is defined by the semitone offsets of its
/// notes relative to the tonic (C).
/// </remarks>
public static class Scales
{
    /// <summary>Major scale: whole-whole-half-whole-whole-whole-half.</summary>
    public static readonly Scale Major = new("major", new[] { 0, 2, 4, 5, 7, 9, 11 });

    /// <summary>Natural minor scale.</summary>
    public static readonly Scale Minor = new("minor", new[] { 0, 2, 3, 5, 7, 8, 10 });

    /// <summary>Dorian mode.</summary>
    public static readonly Scale Dorian = new("dorian", new[] { 0, 2, 3, 5, 7, 9, 10 });

    /// <summary>Mixolydian mode.</summary>
    public static readonly Scale Mixolydian = new("mixolydian", new[] { 0, 2, 4, 5, 7, 9, 10 });

    /// <summary>Major pentatonic scale.</summary>
    public static readonly Scale Pentatonic = new("pentatonic", new[] { 0, 2, 4, 7, 9 });

    /// <summary>Blues scale.</summary>
    public static readonly Scale Blues = new("blues", new[] { 0, 3, 5, 6, 7, 10 });

    private static readonly Scale[] AllScales =
    {
        Major, Minor, Dorian, Mixolydian, Pentatonic, Blues,
    };

    /// <summary>All built-in scales.</summary>
    public static IReadOnlyList<Scale> All => AllScales;

    /// <summary>Looks up a scale by name (case-insensitive), or returns null.</summary>
    public static Scale? ByName(string name)
    {
        foreach (var s in AllScales)
            if (string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase))
                return s;
        return null;
    }

    /// <summary>
    /// Computes the frequency of a scale degree.
    /// </summary>
    /// <param name="scale">The scale to use.</param>
    /// <param name="scaleIndex">Zero-based index within the scale. Values outside [0, len) wrap into neighbouring octaves.</param>
    /// <param name="octave">Scientific pitch octave (C4 = 261.63 Hz in equal temperament).</param>
    /// <returns>Frequency in hertz, using A4 = 440 Hz.</returns>
    public static double Frequency(Scale scale, int scaleIndex, int octave)
    {
        var len = scale.Semitones.Length;
        var normalized = ((scaleIndex % len) + len) % len;
        var octaveShift = (int)Math.Floor((double)scaleIndex / len);
        var semitone = scale.Semitones[normalized];

        // MIDI note number for this pitch: C in the given octave has MIDI (octave + 1) * 12.
        var midi = (octave + 1 + octaveShift) * 12 + semitone;
        return NoteName.FromMidi(midi).Frequency;
    }
}
