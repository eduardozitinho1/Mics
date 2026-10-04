namespace Mics.Core.Score;

public readonly record struct Note(
    double Frequency,
    double StartTime,
    double DurationSeconds,
    double Velocity);
