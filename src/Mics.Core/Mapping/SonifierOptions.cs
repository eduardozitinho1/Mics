using Mics.Core.Audio;

namespace Mics.Core.Mapping;

public sealed record SonifierOptions
{
    public Scale Scale { get; init; } = Scales.Major;
    public double TempoBpm { get; init; } = 120;
    public Instrument? Instrument { get; init; }

    public double BeatSeconds => 60.0 / TempoBpm;
}
