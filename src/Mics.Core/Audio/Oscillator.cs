namespace Mics.Core.Audio;

public static class Oscillator
{
    public static double Sine(double phase)
        => Math.Sin(2 * Math.PI * phase);

    public static double Square(double phase)
        => Math.Sin(2 * Math.PI * phase) >= 0 ? 1.0 : -1.0;

    public static double Triangle(double phase)
    {
        var x = phase - Math.Floor(phase + 0.5);
        return 4 * Math.Abs(x) - 1;
    }

    public static double Sawtooth(double phase)
        => 2 * (phase - Math.Floor(phase + 0.5));
}
