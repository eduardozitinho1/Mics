namespace Mics.Core.Audio;

public static class Envelope
{
    public static double Evaluate(double t, double total, EnvelopeParams env)
    {
        if (t < 0 || total <= 0) return 0;

        var attack = env.Attack;
        var decay = env.Decay;
        var sustain = env.Sustain;
        var release = env.Release;

        if (release > total * 0.4) release = total * 0.4;

        if (t < attack)
            return t / attack;

        if (t < attack + decay)
        {
            var k = (t - attack) / decay;
            return 1 - k * (1 - sustain);
        }

        if (t < total - release)
            return sustain;

        var remaining = total - t;
        if (remaining <= 0) return 0;
        return sustain * (remaining / release);
    }
}
