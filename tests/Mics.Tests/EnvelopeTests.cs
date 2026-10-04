using Mics.Core.Audio;
using Xunit;

namespace Mics.Tests;

public class EnvelopeTests
{
    [Fact]
    public void Adsr_at_zero_is_zero()
    {
        var value = Envelope.Adsr(0, 0.5);
        Assert.Equal(0, value, precision: 6);
    }

    [Fact]
    public void Adsr_peaks_at_end_of_attack()
    {
        var value = Envelope.Adsr(0.01, 0.5, attack: 0.01);
        Assert.Equal(1, value, precision: 6);
    }

    [Fact]
    public void Adsr_reaches_sustain_after_decay()
    {
        var value = Envelope.Adsr(0.2, 0.5);
        Assert.Equal(0.7, value, precision: 6);
    }

    [Fact]
    public void Adsr_ends_at_zero()
    {
        var value = Envelope.Adsr(0.5, 0.5);
        Assert.Equal(0, value, precision: 6);
    }

    [Fact]
    public void Adsr_negative_time_returns_zero()
    {
        var value = Envelope.Adsr(-1, 0.5);
        Assert.Equal(0, value);
    }
}
