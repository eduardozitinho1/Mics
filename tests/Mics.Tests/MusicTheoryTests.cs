using Mics.Core.Audio;
using Mics.Core.Mapping;
using Xunit;

namespace Mics.Tests;

public class ScalesTests
{
    [Fact]
    public void C4_is_approximately_261_hz()
    {
        var freq = Scales.Frequency(Scales.Major, 0, 4);
        Assert.InRange(freq, 261.0, 262.0);
    }

    [Fact]
    public void C5_is_double_C4()
    {
        var c4 = Scales.Frequency(Scales.Major, 0, 4);
        var c5 = Scales.Frequency(Scales.Major, 0, 5);
        Assert.Equal(c4 * 2, c5, precision: 1);
    }

    [Fact]
    public void Minor_third_is_below_major_third()
    {
        var majorThird = Scales.Frequency(Scales.Major, 2, 4);
        var minorThird = Scales.Frequency(Scales.Minor, 2, 4);
        Assert.True(minorThird < majorThird);
    }

    [Fact]
    public void Index_wraps_to_next_octave()
    {
        var c4 = Scales.Frequency(Scales.Major, 0, 4);
        var c5 = Scales.Frequency(Scales.Major, 7, 4);
        Assert.Equal(c4 * 2, c5, precision: 1);
    }

    [Fact]
    public void ByName_is_case_insensitive()
    {
        Assert.NotNull(Scales.ByName("MAJOR"));
        Assert.NotNull(Scales.ByName("major"));
    }
}

public class InstrumentTests
{
    [Fact]
    public void All_have_unique_names()
    {
        var names = Instruments.All.Select(i => i.Name).ToList();
        Assert.Equal(names.Count, names.Distinct().Count());
    }

    [Fact]
    public void ByName_is_case_insensitive()
    {
        var a = Instruments.ByName("SINE");
        var b = Instruments.ByName("sine");
        Assert.NotNull(a);
        Assert.NotNull(b);
        Assert.Equal(a, b);
    }
}
