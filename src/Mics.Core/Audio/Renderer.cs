using Mics.Core.Score;

namespace Mics.Core.Audio;

public static class Renderer
{
    public const int DefaultSampleRate = 22050;

    public static double[] Render(IEnumerable<Note> notes, int sampleRate = DefaultSampleRate)
    {
        var samples = new List<double>();
        foreach (var note in notes)
        {
            if (note.Frequency <= 0 || note.DurationSeconds <= 0)
                continue;

            var count = (int)(note.DurationSeconds * sampleRate);
            var phaseStep = note.Frequency / sampleRate;

            for (int i = 0; i < count; i++)
            {
                var t = (double)i / sampleRate;
                var phase = i * phaseStep;
                var env = Envelope.Adsr(t, note.DurationSeconds);
                var value = Oscillator.Sine(phase) * env * note.Velocity;
                samples.Add(value);
            }
        }
        return samples.ToArray();
    }
}
