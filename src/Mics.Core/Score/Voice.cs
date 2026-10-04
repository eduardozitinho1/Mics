using Mics.Core.Audio;

namespace Mics.Core.Score;

public sealed record Voice(string Name, Instrument Instrument, IReadOnlyList<Note> Notes)
{
    public double DurationSeconds
    {
        get
        {
            double max = 0;
            foreach (var n in Notes)
            {
                if (n.Frequency <= 0 || n.DurationSeconds <= 0)
                    continue;

                var end = n.StartTime + n.DurationSeconds;
                if (end > max) max = end;
            }
            return max;
        }
    }
}
