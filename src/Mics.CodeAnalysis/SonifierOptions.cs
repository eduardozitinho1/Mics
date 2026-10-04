using Mics.Core.Audio;
using Mics.Core.Mapping;

namespace Mics;

/// <summary>
/// Options that control how C# source is mapped to music.
/// </summary>
public sealed record SonifierOptions
{
    private readonly double _tempoBpm = 120;

    /// <summary>Musical scale used to map syntax nodes to pitches.</summary>
    public Scale Scale { get; init; } = Scales.Major;

    /// <summary>
    /// Tempo in beats per minute. Must be a positive, finite value.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when set to a non-positive or non-finite value.</exception>
    public double TempoBpm
    {
        get => _tempoBpm;
        init
        {
            if (!double.IsFinite(value) || value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Tempo must be a positive, finite number.");
            _tempoBpm = value;
        }
    }

    /// <summary>When set, forces every voice to use this instrument instead of cycling through the default list.</summary>
    public Instrument? Instrument { get; init; }

    /// <summary>Seconds per beat, derived from <see cref="TempoBpm"/>.</summary>
    public double BeatSeconds => 60.0 / _tempoBpm;
}
