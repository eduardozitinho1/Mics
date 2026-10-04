namespace Mics.Core.Audio;

public static class Instruments
{
    public static readonly Instrument Sine = new(
        "sine",
        Waveform.Sine,
        new EnvelopeParams(0.01, 0.05, 0.7, 0.1));

    public static readonly Instrument Triangle = new(
        "triangle",
        Waveform.Triangle,
        new EnvelopeParams(0.02, 0.08, 0.6, 0.15));

    public static readonly Instrument Square = new(
        "square",
        Waveform.Square,
        new EnvelopeParams(0.005, 0.03, 0.4, 0.05));

    public static readonly Instrument Sawtooth = new(
        "sawtooth",
        Waveform.Sawtooth,
        new EnvelopeParams(0.005, 0.05, 0.3, 0.08));

    private static readonly Instrument[] AllInstruments =
    {
        Sine, Triangle, Square, Sawtooth,
    };

    public static IReadOnlyList<Instrument> All => AllInstruments;

    public static Instrument? ByName(string name)
    {
        foreach (var i in AllInstruments)
            if (string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase))
                return i;
        return null;
    }
}
