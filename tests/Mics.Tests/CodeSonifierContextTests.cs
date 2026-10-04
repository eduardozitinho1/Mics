using Mics;
using Mics.Core.Mapping;
using Xunit;

namespace Mics.Tests;

public class CodeSonifierContextTests
{
    [Fact]
    public void Nested_class_events_go_to_inner_voice()
    {
        var code = @"
public class Outer
{
    public void OuterMethod() { }

    public class Inner
    {
        public void InnerMethod() { }
    }
}";
        var comp = Sonifier.SonifySource(code);

        // Should have two type-voices (Outer, Inner), no Program voice since root has no direct notes.
        Assert.True(comp.Voices.Count >= 2);

        var outer = comp.Voices.FirstOrDefault(v => v.Name == "class Outer");
        var inner = comp.Voices.FirstOrDefault(v => v.Name == "class Inner");

        Assert.NotNull(outer);
        Assert.NotNull(inner);
        Assert.NotEmpty(outer.Notes);
        Assert.NotEmpty(inner.Notes);
    }

    [Fact]
    public void Events_after_nested_class_go_to_outer()
    {
        var code = @"
public class Outer
{
    public void Before() { }

    public class Inner
    {
        public void Inside() { }
    }

    public void After() { }
}";
        var comp = Sonifier.SonifySource(code);

        var outer = comp.Voices.First(v => v.Name == "class Outer");
        var inner = comp.Voices.First(v => v.Name == "class Inner");

        // Both outer methods must land in the outer voice; the inner method in the inner voice.
        Assert.True(outer.Notes.Count > inner.Notes.Count);
    }

    [Fact]
    public void Multiple_sibling_classes_each_get_their_own_voice()
    {
        var code = @"
public class A { public void X() { } }
public class B { public void Y() { } }
public class C { public void Z() { } }";
        var comp = Sonifier.SonifySource(code);

        Assert.Equal(3, comp.Voices.Count);
        Assert.Contains(comp.Voices, v => v.Name == "class A");
        Assert.Contains(comp.Voices, v => v.Name == "class B");
        Assert.Contains(comp.Voices, v => v.Name == "class C");
    }

    [Fact]
    public void Deeply_nested_types_all_produce_voices()
    {
        var code = @"
public class A
{
    public class B
    {
        public class C
        {
            public void Deep() { }
        }
    }
}";
        var comp = Sonifier.SonifySource(code);

        Assert.Contains(comp.Voices, v => v.Name == "class A");
        Assert.Contains(comp.Voices, v => v.Name == "class B");
        Assert.Contains(comp.Voices, v => v.Name == "class C");
    }

    [Fact]
    public void Struct_record_interface_enum_each_produce_a_voice()
    {
        var code = @"
public class C { public void M() { } }
public struct S { public void M() { } }
public record R(int X);
public interface I { void M(); }
public enum E { A, B }";
        var comp = Sonifier.SonifySource(code);

        Assert.Contains(comp.Voices, v => v.Name.StartsWith("class "));
        Assert.Contains(comp.Voices, v => v.Name.StartsWith("struct "));
        Assert.Contains(comp.Voices, v => v.Name.StartsWith("record "));
        Assert.Contains(comp.Voices, v => v.Name.StartsWith("interface "));
        Assert.Contains(comp.Voices, v => v.Name.StartsWith("enum "));
    }

    [Fact]
    public void Empty_source_produces_no_voices()
    {
        var comp = Sonifier.SonifySource("");
        Assert.Empty(comp.Voices);
    }
}
