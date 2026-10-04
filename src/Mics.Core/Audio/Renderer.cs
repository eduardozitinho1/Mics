using Mics.Core.Score;

namespace Mics.Core.Audio;

/// <summary>
/// Mixes every voice of a <see cref="Composition"/> into a single PCM buffer.
/// </summary>
/// <remarks>
/// The output is a mono signal at the requested sample rate, normalized so the peak
/// amplitude stays within [-1, 1]. If the raw mix already fits inside that range, no
/// scaling is applied.
/// </remarks>
public static class Renderer
{
    /// <summary>Default sample rate used when none is specified.</summary>
    public const int DefaultSampleRate = 22050;

    /// <summary>Minimum allowed sample rate.</summary>
    public const int MinSampleRate = 8000;

    /// <summary>Maximum allowed sample rate.</summary>
    public const int MaxSampleRate = 192000;

    /// <summary>
    /// Renders the composition into PCM samples.
    /// </summary>
    /// <param name="composition">The composition to render.</param>
    /// <param name="sampleRate">Samples per second. Must be between <see cref="MinSampleRate"/> and <see cref="MaxSampleRate"/>.</param>
    /// <returns>An array of PCM samples in [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="composition"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="sampleRate"/> is outside the supported range.</exception>
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
