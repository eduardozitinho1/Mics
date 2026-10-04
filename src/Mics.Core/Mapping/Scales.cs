namespace Mics.Core.Mapping;

public static class Scales
{
    private static readonly int[] MajorSemitones = { 0, 2, 4, 5, 7, 9, 11 };

    public static double Frequency(int scaleIndex, int octave)
    {
        var normalized = ((scaleIndex % MajorSemitones.Length) + MajorSemitones.Length) % MajorSemitones.Length;
        var semitone = MajorSemitones[normalized] + (octave - 4) * 12;
        return 261.63 * Math.Pow(2, semitone / 12.0);
    }

    public static int Length => MajorSemitones.Length;
}
