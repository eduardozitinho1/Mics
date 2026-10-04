namespace Mics.Core.Audio;

internal static class Oscillator
{
    public static double Sample(Waveform waveform, double phase)
    {
        var cycle = phase - Math.Floor(phase);
        return waveform switch
        {
            Waveform.Sine => Math.Sin(2 * Math.PI * cycle),
            Waveform.Triangle => 4 * Math.Abs(cycle - 0.5) - 1,
            Waveform.Square => cycle < 0.5 ? 1.0 : -1.0,
            Waveform.Sawtooth => 2 * cycle - 1,
            _ => 0,
        };
    }
}
