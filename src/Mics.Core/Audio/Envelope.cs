namespace Mics.Core.Audio;

/// <summary>
/// ADSR envelope evaluation. Given a time <c>t</c> inside a note of total duration <c>total</c>,
/// returns the amplitude multiplier in [0, 1].
/// </summary>
internal static class Envelope
{
    /// <summary>
    /// Evaluates the envelope at time <paramref name="t"/> within a note of duration <paramref name="total"/>.
    /// </summary>
    /// <param name="t">Elapsed time in seconds since note start.</param>
    /// <param name="total">Total duration of the note in seconds.</param>
    /// <param name="env">The envelope parameters.</param>
    /// <returns>Amplitude multiplier in [0, 1].</returns>
    public static double Evaluate(double t, double total, EnvelopeParams env)
    {
        if (t < 0 || total <= 0) return 0;
        if (t >= total) return 0;

        var attack = env.Attack;
        var decay = env.Decay;
        var sustain = env.Sustain;
        var release = env.Release;

        // Clamp release so it never eats more than 40% of the note.
        if (release > total * 0.4)
            release = total * 0.4;

        // Attack phase. If attack is zero, the envelope starts at sustain immediately.
        if (attack > 0 && t < attack)
            return t / attack;

        // Decay phase. If decay is zero, jump straight from peak to sustain.
        if (decay > 0 && t < attack + decay)
        {
            var k = (t - attack) / decay;
            return 1 - k * (1 - sustain);
        }

        // Sustain phase.
        if (t < total - release)
            return sustain;

        // Release phase. If release is zero, drop to zero immediately at the end.
        if (release <= 0)
            return 0;

        var remaining = total - t;
        if (remaining <= 0) return 0;
        return sustain * (remaining / release);
    }
}
