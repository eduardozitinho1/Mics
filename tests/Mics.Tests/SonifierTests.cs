using Mics.Core.Audio;
using Mics.Core.Mapping;
using Xunit;

namespace Mics.Tests;

public class SonifierTests
{
    [Fact]
    public void Empty_source_produces_empty_composition()
    {
        var comp = Sonifier.SonifySource("");
        Assert.Empty(comp.Voices);
    }

    [Fact]
    public void Single_class_produces_one_voice()
    {
        var comp = Sonifier.SonifySource("public class Foo { public void Bar() { } }");
        Assert.Single(comp.Voices);
        Assert.NotEmpty(comp.Voices[0].Notes);
    }

    [Fact]
    public void Two_classes_produce_two_voices()
    {
        var code = "public class A { public void X() { } } public class B { public void Y() { } }";
        var comp = Sonifier.SonifySource(code);
        Assert.Equal(2, comp.Voices.Count);
    }

    [Fact]
    public void Sonify_is_deterministic()
    {
        var code = "public class Foo { public int Bar() { return 42; } }";
        var a = Sonifier.SonifySource(code);
        var b = Sonifier.SonifySource(code);
        Assert.Equal(a.Voices.Count, b.Voices.Count);
        for (int i = 0; i < a.Voices.Count; i++)
            Assert.Equal(a.Voices[i].Notes.Count, b.Voices[i].Notes.Count);
    }

    [Fact]
    public void Notes_are_audible_frequencies()
    {
        var comp = Sonifier.SonifySource("public class Foo { public void Bar() { } }");
        foreach (var v in comp.Voices)
            foreach (var n in v.Notes)
                Assert.InRange(n.Frequency, 20.0, 20000.0);
    }

    [Fact]
    public void Options_are_respected()
    {
        var opts = new SonifierOptions
        {
            Scale = Scales.Minor,
            TempoBpm = 60,
            Instrument = Instruments.Square,
        };
        var comp = Sonifier.SonifySource("public class Foo { public void Bar() { } }", opts);
        Assert.Single(comp.Voices);
        Assert.Equal("square", comp.Voices[0].Instrument.Name);
        Assert.Equal(60, comp.TempoBpm);
    }

    [Fact]
    public void If_statement_adds_notes()
    {
        var withIf = "public class Foo { public int Bar(int x) { if (x > 0) return 1; return 0; } }";
        var withoutIf = "public class Foo { public int Bar() { return 0; } }";
        var a = Sonifier.SonifySource(withIf);
        var b = Sonifier.SonifySource(withoutIf);
        var aNotes = a.Voices.Sum(v => v.Notes.Count);
        var bNotes = b.Voices.Sum(v => v.Notes.Count);
        Assert.True(aNotes > bNotes);
    }
}
