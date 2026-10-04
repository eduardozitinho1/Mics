namespace Mics.Core.Audio;

/// <summary>
/// Parameters for an ADSR envelope: attack, decay, sustain, release.
/// </summary>
/// <remarks>
/// All times are in seconds. <see cref="Sustain"/> is a level between 0 and 1, not a time.
/// All values must be finite; times must be non-negative; sustain must be in [0, 1].
/// </remarks>
public readonly record struct EnvelopeParams
{
    /// <summary>Attack time in seconds. Must be non-negative and finite.</summary>
    public double Attack { get; }

    /// <summary>Decay time in seconds. Must be non-negative and finite.</summary>
    public double Decay { get; }

    /// <summary>Sustain level between 0 and 1.</summary>
    public double Sustain { get; }

    /// <summary>Release time in seconds. Must be non-negative and finite.</summary>
    public double Release { get; }

    /// <summary>Creates a new envelope, validating every parameter.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any parameter is out of range or not finite.</exception>
    public EnvelopeParams(double Attack, double Decay, double Sustain, double Release)
    {
        if (!double.IsFinite(Attack) || Attack < 0)
            throw new ArgumentOutOfRangeException(nameof(Attack), Attack, "Attack must be a non-negative, finite number.");
        if (!double.IsFinite(Decay) || Decay < 0)
            throw new ArgumentOutOfRangeException(nameof(Decay), Decay, "Decay must be a non-negative, finite number.");
        if (!double.IsFinite(Sustain) || Sustain < 0 || Sustain > 1)
            throw new ArgumentOutOfRangeException(nameof(Sustain), Sustain, "Sustain must be a finite number in [0, 1].");
        if (!double.IsFinite(Release) || Release < 0)
            throw new ArgumentOutOfRangeException(nameof(Release), Release, "Release must be a non-negative, finite number.");

        this.Attack = Attack;
        this.Decay = Decay;
        this.Sustain = Sustain;
        this.Release = Release;
    }

    /// <summary>Deconstructs the four parameters.</summary>
    public void Deconstruct(out double Attack, out double Decay, out double Sustain, out double Release)
    {
        Attack = this.Attack;
        Decay = this.Decay;
        Sustain = this.Sustain;
        Release = this.Release;
    }
}
