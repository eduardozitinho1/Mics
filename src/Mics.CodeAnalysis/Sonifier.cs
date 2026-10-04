using Microsoft.CodeAnalysis.CSharp;
using Mics.Core.Score;

namespace Mics;

public static class Sonifier
{
    public static Composition SonifySource(string sourceCode, SonifierOptions? options = null)
    {
        var opts = options ?? new SonifierOptions();
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetRoot();
        var visitor = new CodeSonifier(opts);
        visitor.Visit(root);
        return visitor.Build();
    }

    public static Composition SonifyFile(string path, SonifierOptions? options = null)
    {
        var text = File.ReadAllText(path);
        return SonifySource(text, options);
    }
}
