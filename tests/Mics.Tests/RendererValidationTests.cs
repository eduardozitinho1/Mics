using Mics.Core.Audio;
using Mics.Core.Score;
using Xunit;

namespace Mics.Tests;

public class RendererValidationTests
{
    private static Composition SingleNote()
    {
        var note = new Note(440, 0, 0.1, 0.7);
        var voice = new Voice("v", Instruments.Sine, new[] { note });
        return new Composition(120, new[] { voice });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(7999)]
    [InlineData(192001)]
    public void Render_rejects_out_of_range_sample_rate(int rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Renderer.Render(SingleNote(), rate));
    }

    [Fact]
    public void Render_rejects_null_composition()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Renderer.Render(null!, 22050));
    }

    [Fact]
    public void Render_accepts_min_sample_rate()
    {
        var samples = Renderer.Render(SingleNote(), Renderer.MinSampleRate);
        Assert.NotEmpty(samples);
    }

    [Fact]
    public void Render_accepts_max_sample_rate()
    {
        var samples = Renderer.Render(SingleNote(), Renderer.MaxSampleRate);
        Assert.NotEmpty(samples);
    }

    [Fact]
    public void Render_skips_notes_with_non_finite_values()
    {
        var bad = new Note(double.NaN, 0, 0.1, 0.7);
        var voice = new Voice("v", Instruments.Sine, new[] { bad });
        var comp = new Composition(120, new[] { voice });
        var samples = Renderer.Render(comp, 8000);
        Assert.All(samples, s => Assert.Equal(0, s));
    }

    [Fact]
    public void Render_skips_notes_with_non_finite_start_time()
    {
        var bad = new Note(440, double.NaN, 0.1, 0.7);
        var voice = new Voice("v", Instruments.Sine, new[] { bad });
        var comp = new Composition(120, new[] { voice });
        var samples = Renderer.Render(comp, 8000);
        Assert.All(samples, s => Assert.Equal(0, s));
    }
}

public class EnvelopeParamsTests
{
    [Fact]
    public void Negative_attack_throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EnvelopeParams(-0.1, 0, 0.5, 0.1));
    }

    [Fact]
    public void Negative_decay_throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EnvelopeParams(0, -0.1, 0.5, 0.1));
    }

    [Fact]
    public void Negative_release_throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EnvelopeParams(0, 0.1, 0.5, -0.1));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    public void Out_of_range_sustain_throws(double sustain)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EnvelopeParams(0, 0.1, sustain, 0.1));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Non_finite_attack_throws(double attack)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new EnvelopeParams(attack, 0, 0.5, 0));
    }

    [Fact]
    public void Valid_params_construct_successfully()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.7, 0.1);
        Assert.Equal(0.01, env.Attack);
        Assert.Equal(0.05, env.Decay);
        Assert.Equal(0.7, env.Sustain);
        Assert.Equal(0.1, env.Release);
    }

    [Fact]
    public void Zero_attack_is_allowed()
    {
        var env = new EnvelopeParams(0, 0.05, 0.7, 0.1);
        Assert.Equal(0, env.Attack);
    }

    [Fact]
    public void Zero_decay_is_allowed()
    {
        var env = new EnvelopeParams(0.01, 0, 0.7, 0.1);
        Assert.Equal(0, env.Decay);
    }

    [Fact]
    public void Zero_release_is_allowed()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.7, 0);
        Assert.Equal(0, env.Release);
    }
}

public class EnvelopeEvaluateTests
{
    [Fact]
    public void Zero_attack_starts_at_sustain_level()
    {
        var env = new EnvelopeParams(0, 0.05, 0.5, 0.1);
        var value = Envelope.Evaluate(0.001, 1.0, env);
        Assert.True(value > 0);
        Assert.True(value <= 1.0);
    }

    [Fact]
    public void Zero_decay_jumps_from_peak_to_sustain()
    {
        var env = new EnvelopeParams(0.01, 0, 0.5, 0.1);
        var afterAttack = Envelope.Evaluate(0.02, 1.0, env);
        Assert.Equal(0.5, afterAttack, precision: 6);
    }

    [Fact]
    public void Zero_release_ends_sharply()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.5, 0);
        var value = Envelope.Evaluate(0.99, 1.0, env);
        Assert.True(value >= 0);
    }

    [Fact]
    public void Past_total_returns_zero()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.5, 0.1);
        Assert.Equal(0, Envelope.Evaluate(1.5, 1.0, env));
    }

    [Fact]
    public void Zero_total_returns_zero()
    {
        var env = new EnvelopeParams(0.01, 0.05, 0.5, 0.1);
        Assert.Equal(0, Envelope.Evaluate(0.5, 0, env));
    }
}
