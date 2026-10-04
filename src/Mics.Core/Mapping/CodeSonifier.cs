using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mics.Core.Score;

namespace Mics.Core.Mapping;

public sealed class CodeSonifier : CSharpSyntaxWalker
{
    private readonly List<Note> _notes = new();
    private int _depth;
    private const double BeatSeconds = 0.5;

    public IReadOnlyList<Note> Notes => _notes;

    private void Emit(int scaleIndex, double beats, double velocity = 0.7)
    {
        var octave = Math.Clamp(5 - _depth, 2, 7);
        var freq = Scales.Frequency(scaleIndex, octave);
        var duration = beats * BeatSeconds;
        _notes.Add(new Note(freq, duration, velocity));
    }

    public override void VisitCompilationUnit(CompilationUnitSyntax node)
    {
        var hasContent = node.Members.Count > 0;
        if (!hasContent)
            return;

        Emit(0, 2.0, 0.4);
        base.VisitCompilationUnit(node);
        Emit(0, 2.0, 0.4);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        Emit(0, 1.0, 0.8);
        _depth++;
        base.VisitClassDeclaration(node);
        _depth--;
        Emit(0, 1.0, 0.8);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        Emit(2, 1.0, 0.8);
        _depth++;
        base.VisitStructDeclaration(node);
        _depth--;
        Emit(2, 1.0, 0.8);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        Emit(4, 1.0, 0.7);
        _depth++;
        base.VisitInterfaceDeclaration(node);
        _depth--;
        Emit(4, 1.0, 0.7);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        Emit(6, 1.0, 0.8);
        _depth++;
        base.VisitRecordDeclaration(node);
        _depth--;
        Emit(6, 1.0, 0.8);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        Emit(1, 0.5, 0.7);
        _depth++;
        base.VisitEnumDeclaration(node);
        _depth--;
        Emit(1, 0.5, 0.7);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        Emit(2, 0.5, 0.7);
        _depth++;
        base.VisitMethodDeclaration(node);
        _depth--;
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        Emit(4, 0.5, 0.6);
        _depth++;
        base.VisitConstructorDeclaration(node);
        _depth--;
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        Emit(5, 0.25, 0.5);
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        Emit(1, 0.25, 0.4);
        base.VisitFieldDeclaration(node);
    }

    public override void VisitIfStatement(IfStatementSyntax node)
    {
        Emit(4, 0.5, 0.8);
        _depth++;
        base.VisitIfStatement(node);
        _depth--;
        Emit(0, 0.5, 0.6);
    }

    public override void VisitElseClause(ElseClauseSyntax node)
    {
        Emit(6, 0.25, 0.5);
        base.VisitElseClause(node);
    }

    public override void VisitSwitchStatement(SwitchStatementSyntax node)
    {
        for (int i = 0; i < 3; i++) Emit(2 + i, 0.25, 0.6);
        _depth++;
        base.VisitSwitchStatement(node);
        _depth--;
        Emit(0, 0.5, 0.6);
    }

    public override void VisitForStatement(ForStatementSyntax node)
    {
        for (int i = 0; i < 4; i++) Emit(7, 0.125, 0.5);
        _depth++;
        base.VisitForStatement(node);
        _depth--;
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        for (int i = 0; i < 4; i++) Emit(7, 0.125, 0.5);
        _depth++;
        base.VisitForEachStatement(node);
        _depth--;
    }

    public override void VisitWhileStatement(WhileStatementSyntax node)
    {
        for (int i = 0; i < 4; i++) Emit(6, 0.125, 0.5);
        _depth++;
        base.VisitWhileStatement(node);
        _depth--;
    }

    public override void VisitDoStatement(DoStatementSyntax node)
    {
        for (int i = 0; i < 3; i++) Emit(6, 0.125, 0.5);
        _depth++;
        base.VisitDoStatement(node);
        _depth--;
    }

    public override void VisitTryStatement(TryStatementSyntax node)
    {
        Emit(2, 0.5, 0.6);
        Emit(4, 0.5, 0.7);
        _depth++;
        base.VisitTryStatement(node);
        _depth--;
    }

    public override void VisitCatchClause(CatchClauseSyntax node)
    {
        Emit(6, 0.5, 0.8);
        _depth++;
        base.VisitCatchClause(node);
        _depth--;
        Emit(7, 0.25, 0.5);
    }

    public override void VisitReturnStatement(ReturnStatementSyntax node)
    {
        Emit(0, 0.5, 0.9);
        base.VisitReturnStatement(node);
    }

    public override void VisitThrowStatement(ThrowStatementSyntax node)
    {
        Emit(7, 0.25, 1.0);
        Emit(6, 0.25, 1.0);
        base.VisitThrowStatement(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        Emit(2, 0.125, 0.4);
        Emit(4, 0.125, 0.4);
        Emit(6, 0.125, 0.4);
        base.VisitInvocationExpression(node);
    }

    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        Emit(1, 0.25, 0.3);
        base.VisitVariableDeclarator(node);
    }

    public override void VisitLiteralExpression(LiteralExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            var text = node.Token.Text;
            if (long.TryParse(text.Replace("_", "").TrimEnd('L', 'l', 'u', 'U'), out var value))
            {
                var idx = (int)(Math.Abs(value) % Scales.Length);
                Emit(idx, 0.125, 0.3);
            }
        }
        else if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            var len = node.Token.Text.Length;
            Emit(len % Scales.Length, 0.125, 0.3);
        }
        base.VisitLiteralExpression(node);
    }
}
