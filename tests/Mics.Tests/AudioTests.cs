using Mics.Core.Audio;
using Mics.Core.Score;
using Xunit;

namespace Mics.Tests;

public class EnvelopeTests
{
    [Fact]
    public void At_zero_is_zero()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.7, 0.1);
        Assert.Equal(0, Envelope.Evaluate(0, 0.5, env), precision: 6);
    }

    [Fact]
    public void At_sustain_is_constant()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.7, 0.1);
        Assert.Equal(0.7, Envelope.Evaluate(0.2, 0.5, env), precision: 6);
        Assert.Equal(0.7, Envelope.Evaluate(0.3, 0.5, env), precision: 6);
    }

    [Fact]
    public void At_end_is_near_zero()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.7, 0.1);
        Assert.True(Envelope.Evaluate(0.5, 0.5, env) < 0.01);
    }

    [Fact]
    public void Negative_time_returns_zero()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.7, 0.1);
        Assert.Equal(0, Envelope.Evaluate(-1, 0.5, env));
    }
}

public class RendererTests
{
    [Fact]
    public void Empty_composition_yields_no_samples()
    {
        var comp = new Composition(120, Array.Empty<Voice>());
        Assert.Empty(Renderer.Render(comp));
    }

    [Fact]
    public void Single_note_produces_correct_sample_count()
    {
        var note = new Note(440.0, 0, 0.1, 0.7);
        var voice = new Voice("t", Instruments.Sine, new[] { note });
        var comp = new Composition(120, new[] { voice });
        var samples = Renderer.Render(comp, sampleRate: 8000);
        Assert.Equal(800, samples.Length);
    }

    [Fact]
    public void Multiple_voices_are_mixed()
    {
        var note = new Note(440.0, 0, 0.1, 0.5);
        var v1 = new Voice("a", Instruments.Sine, new[] { note });
        var v2 = new Voice("b", Instruments.Triangle, new[] { note });
        var comp = new Composition(120, new[] { v1, v2 });
        var samples = Renderer.Render(comp, sampleRate: 8000);
        Assert.Equal(800, samples.Length);
        foreach (var s in samples)
            Assert.InRange(s, -1.0, 1.0);
    }

    [Fact]
    public void Invalid_notes_are_skipped()
    {
        var n1 = new Note(0, 0, 0.1, 0.7);
        var n2 = new Note(440, 0, -1, 0.7);
        var voice = new Voice("t", Instruments.Sine, new[] { n1, n2 });
        var comp = new Composition(120, new[] { voice });
        Assert.Empty(Renderer.Render(comp, sampleRate: 8000));
    }
}

public class WavWriterTests
{
    [Fact]
    public void Write_produces_riff_wave_header()
    {
        var path = Path.GetTempFileName() + ".wav";
        try
        {
            WavWriter.Write(path, new double[] { 0.0, 0.5, -0.5, 0.0 });
            var bytes = File.ReadAllBytes(path);
            Assert.True(bytes.Length > 44);
            Assert.Equal((byte)'R', bytes[0]);
            Assert.Equal((byte)'I', bytes[1]);
            Assert.Equal((byte)'F', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
            Assert.Equal((byte)'W', bytes[8]);
            Assert.Equal((byte)'A', bytes[9]);
            Assert.Equal((byte)'V', bytes[10]);
            Assert.Equal((byte)'E', bytes[11]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Write_clamps_out_of_range_samples()
    {
        var path = Path.GetTempFileName() + ".wav";
        try
        {
            WavWriter.Write(path, new double[] { 100.0, -100.0 });
            var bytes = File.ReadAllBytes(path);
            Assert.True(bytes.Length >= 48);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
