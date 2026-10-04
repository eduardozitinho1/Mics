namespace Mics.Core.Audio;

public sealed record Instrument(string Name, Waveform Waveform, EnvelopeParams Envelope);
