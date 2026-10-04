using Microsoft.CodeAnalysis.CSharp;
using Mics.Core.Score;

namespace Mics.Core.Mapping;

public static class Sonifier
{
    public static IReadOnlyList<Note> SonifySource(string sourceCode)
    {
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetRoot();
        var visitor = new CodeSonifier();
        visitor.Visit(root);
        return visitor.Notes;
    }

    public static IReadOnlyList<Note> SonifyFile(string path)
    {
        var text = File.ReadAllText(path);
        return SonifySource(text);
    }
}
