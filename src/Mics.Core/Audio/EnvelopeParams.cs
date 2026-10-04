namespace Mics.Core.Audio;

public readonly record struct EnvelopeParams(
    double Attack,
    double Decay,
    double Sustain,
    double Release);
