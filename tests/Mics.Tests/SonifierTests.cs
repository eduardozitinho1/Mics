using Mics.Core.Mapping;
using Xunit;

namespace Mics.Tests;

public class SonifierTests
{
    [Fact]
    public void Sonify_empty_source_returns_no_notes()
    {
        var notes = Sonifier.SonifySource("");
        Assert.Empty(notes);
    }

    [Fact]
    public void Sonify_class_produces_notes()
    {
        var code = "public class Foo { }";
        var notes = Sonifier.SonifySource(code);
        Assert.NotEmpty(notes);
    }

    [Fact]
    public void Sonify_is_deterministic()
    {
        var code = "public class Foo { public int Bar() { return 42; } }";
        var a = Sonifier.SonifySource(code);
        var b = Sonifier.SonifySource(code);
        Assert.Equal(a.Count, b.Count);
        for (int i = 0; i < a.Count; i++)
        {
            Assert.Equal(a[i].Frequency, b[i].Frequency);
            Assert.Equal(a[i].DurationSeconds, b[i].DurationSeconds);
        }
    }

    [Fact]
    public void Sonify_if_statement_adds_notes()
    {
        var withIf = "public class Foo { public int Bar(int x) { if (x > 0) return 1; return 0; } }";
        var withoutIf = "public class Foo { public int Bar() { return 0; } }";
        var a = Sonifier.SonifySource(withIf);
        var b = Sonifier.SonifySource(withoutIf);
        Assert.True(a.Count > b.Count);
    }

    [Fact]
    public void Sonify_produces_frequencies_in_audible_range()
    {
        var code = "public class Foo { public void Bar() { } }";
        var notes = Sonifier.SonifySource(code);
        Assert.NotEmpty(notes);
        foreach (var note in notes)
        {
            Assert.InRange(note.Frequency, 20.0, 20000.0);
        }
    }

    [Fact]
    public void Scales_c4_is_approximately_261()
    {
        var freq = Scales.Frequency(0, 4);
        Assert.InRange(freq, 261.0, 262.0);
    }

    [Fact]
    public void Scales_c5_is_double_c4()
    {
        var c4 = Scales.Frequency(0, 4);
        var c5 = Scales.Frequency(0, 5);
        Assert.Equal(c4 * 2, c5, precision: 1);
    }

    [Fact]
    public void Scales_handles_negative_index()
    {
        var neg = Scales.Frequency(-1, 4);
        var pos = Scales.Frequency(6, 4);
        Assert.Equal(pos, neg, precision: 6);
    }
}
