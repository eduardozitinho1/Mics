using Mics.Core.Score;

namespace Mics.Core.Audio;

internal static class Renderer
{
    public const int DefaultSampleRate = 22050;
    public const int MinSampleRate = 8000;
    public const int MaxSampleRate = 192000;

    public static double[] Render(Composition composition, int sampleRate = DefaultSampleRate)
    {
        ArgumentNullException.ThrowIfNull(composition);

        if (sampleRate < MinSampleRate || sampleRate > MaxSampleRate)
            throw new ArgumentOutOfRangeException(
                nameof(sampleRate),
                sampleRate,
                $"Sample rate must be between {MinSampleRate} and {MaxSampleRate}.");

        var duration = composition.DurationSeconds;
        if (duration <= 0) return Array.Empty<double>();

        var totalSamples = (int)(duration * sampleRate);
        if (totalSamples <= 0) return Array.Empty<double>();

        var buffer = new double[totalSamples];

        foreach (var voice in composition.Voices)
            RenderVoice(voice, buffer, sampleRate);

        var max = 0.0;
        for (int i = 0; i < buffer.Length; i++)
        {
            var abs = Math.Abs(buffer[i]);
            if (abs > max) max = abs;
        }

        if (max > 1.0)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] /= max;
        }

        return buffer;
    }

    private static void RenderVoice(Voice voice, double[] buffer, int sampleRate)
    {
        var waveform = voice.Instrument.Waveform;
        var env = voice.Instrument.Envelope;

        foreach (var note in voice.Notes)
        {
            if (note.Frequency <= 0 || note.DurationSeconds <= 0)
                continue;
            if (!double.IsFinite(note.Frequency) || !double.IsFinite(note.StartTime) || !double.IsFinite(note.DurationSeconds))
                continue;

            var startSample = (int)(note.StartTime * sampleRate);
            var count = (int)(note.DurationSeconds * sampleRate);
            var phaseStep = note.Frequency / sampleRate;

            for (int i = 0; i < count; i++)
            {
                var idx = startSample + i;
                if (idx >= buffer.Length) break;
                if (idx < 0) continue;

                var t = (double)i / sampleRate;
                var phase = i * phaseStep;
                var amplitude = Envelope.Evaluate(t, note.DurationSeconds, env);
                buffer[idx] += Oscillator.Sample(waveform, phase) * amplitude * note.Velocity;
            }
        }
    }
}
