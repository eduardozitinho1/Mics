namespace Mics.Core.Audio;

public static class Envelope
{
    public static double Adsr(
        double t,
        double total,
        double attack = 0.01,
        double decay = 0.05,
        double sustain = 0.7,
        double release = 0.1)
    {
        if (t < 0) return 0;
        if (total <= 0) return 0;
        if (release > total) release = total * 0.4;

        if (t < attack)
            return t / attack;

        if (t < attack + decay)
        {
            var k = (t - attack) / decay;
            return 1 - k * (1 - sustain);
        }

        if (t < total - release)
            return sustain;

        var k2 = Math.Max(0, (total - t) / release);
        return sustain * k2;
    }
}
