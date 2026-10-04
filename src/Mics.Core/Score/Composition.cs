namespace Mics.Core.Score;

public sealed record Composition(double TempoBpm, IReadOnlyList<Voice> Voices)
{
    public double DurationSeconds
    {
        get
        {
            double max = 0;
            foreach (var v in Voices)
                if (v.DurationSeconds > max) max = v.DurationSeconds;
            return max;
        }
    }
}
