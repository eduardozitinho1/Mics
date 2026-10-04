namespace Mics.Core.Mapping;

public static class Scales
{
    public static readonly Scale Major = new("major", new[] { 0, 2, 4, 5, 7, 9, 11 });
    public static readonly Scale Minor = new("minor", new[] { 0, 2, 3, 5, 7, 8, 10 });
    public static readonly Scale Dorian = new("dorian", new[] { 0, 2, 3, 5, 7, 9, 10 });
    public static readonly Scale Mixolydian = new("mixolydian", new[] { 0, 2, 4, 5, 7, 9, 10 });
    public static readonly Scale Pentatonic = new("pentatonic", new[] { 0, 2, 4, 7, 9 });
    public static readonly Scale Blues = new("blues", new[] { 0, 3, 5, 6, 7, 10 });

    private static readonly Scale[] AllScales =
    {
        Major, Minor, Dorian, Mixolydian, Pentatonic, Blues,
    };

    public static IReadOnlyList<Scale> All => AllScales;

    public static Scale? ByName(string name)
    {
        foreach (var s in AllScales)
            if (string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase))
                return s;
        return null;
    }

    public static double Frequency(Scale scale, int scaleIndex, int octave)
    {
        var len = scale.Semitones.Length;
        var normalized = ((scaleIndex % len) + len) % len;
        var octaveShift = (int)Math.Floor((double)scaleIndex / len);
        var semitone = scale.Semitones[normalized] + (octave + octaveShift - 4) * 12;
        return 261.63 * Math.Pow(2, semitone / 12.0);
    }
}
