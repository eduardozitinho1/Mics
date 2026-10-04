using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mics.Core.Audio;
using Mics.Core.Score;

namespace Mics.Core.Mapping;

public sealed class CodeSonifier : CSharpSyntaxWalker
{
    private readonly SonifierOptions _options;
    private readonly List<VoiceBuilder> _allVoices = new();
    private readonly VoiceBuilder _root = new("Program");
    private VoiceBuilder? _current;

    public CodeSonifier(SonifierOptions options)
    {
        _options = options;
        _current = _root;
    }

    public override void VisitCompilationUnit(CompilationUnitSyntax node)
    {
        base.VisitCompilationUnit(node);
        if (_current == _root)
            _current = null;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        PushVoice($"class {node.Identifier.Text}");
        base.VisitClassDeclaration(node);
        PopVoice();
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        PushVoice($"struct {node.Identifier.Text}");
        base.VisitStructDeclaration(node);
        PopVoice();
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        PushVoice($"record {node.Identifier.Text}");
        base.VisitRecordDeclaration(node);
        PopVoice();
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        PushVoice($"interface {node.Identifier.Text}");
        base.VisitInterfaceDeclaration(node);
        PopVoice();
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        PushVoice($"enum {node.Identifier.Text}");
        base.VisitEnumDeclaration(node);
        PopVoice();
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        Emit(2, 0.5, 0.7);
        DepthUp();
        base.VisitMethodDeclaration(node);
        DepthDown();
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        Emit(4, 0.5, 0.6);
        DepthUp();
        base.VisitConstructorDeclaration(node);
        DepthDown();
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
        DepthUp();
        base.VisitIfStatement(node);
        DepthDown();
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
        DepthUp();
        base.VisitSwitchStatement(node);
        DepthDown();
        Emit(0, 0.5, 0.6);
    }

    public override void VisitForStatement(ForStatementSyntax node)
    {
        for (int i = 0; i < 4; i++) Emit(7, 0.125, 0.5);
        DepthUp();
        base.VisitForStatement(node);
        DepthDown();
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        for (int i = 0; i < 4; i++) Emit(7, 0.125, 0.5);
        DepthUp();
        base.VisitForEachStatement(node);
        DepthDown();
    }

    public override void VisitWhileStatement(WhileStatementSyntax node)
    {
        for (int i = 0; i < 4; i++) Emit(6, 0.125, 0.5);
        DepthUp();
        base.VisitWhileStatement(node);
        DepthDown();
    }

    public override void VisitDoStatement(DoStatementSyntax node)
    {
        for (int i = 0; i < 3; i++) Emit(6, 0.125, 0.5);
        DepthUp();
        base.VisitDoStatement(node);
        DepthDown();
    }

    public override void VisitTryStatement(TryStatementSyntax node)
    {
        Emit(2, 0.5, 0.6);
        Emit(4, 0.5, 0.7);
        DepthUp();
        base.VisitTryStatement(node);
        DepthDown();
    }

    public override void VisitCatchClause(CatchClauseSyntax node)
    {
        Emit(6, 0.5, 0.8);
        DepthUp();
        base.VisitCatchClause(node);
        DepthDown();
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
                var idx = (int)(Math.Abs(value) % 7);
                Emit(idx, 0.125, 0.3);
            }
        }
        else if (node.IsKind(SyntaxKind.StringLiteralExpression))
        {
            var len = node.Token.Text.Length;
            Emit(len % 7, 0.125, 0.3);
        }
        base.VisitLiteralExpression(node);
    }

    public Composition Build()
    {
        var voices = new List<Voice>();
        var index = 0;

        if (_root.Notes.Count > 0)
        {
            voices.Add(_root.Build(PickInstrument(index)));
            index++;
        }

        foreach (var v in _allVoices)
        {
            voices.Add(v.Build(PickInstrument(index)));
            index++;
        }

        return new Composition(_options.TempoBpm, voices);
    }

    private Instrument PickInstrument(int index)
        => _options.Instrument ?? Instruments.All[index % Instruments.All.Count];

    private void PushVoice(string name)
    {
        var voice = new VoiceBuilder(name);
        _allVoices.Add(voice);
        _current = voice;
    }

    private void PopVoice()
    {
        _current = null;
    }

    private void DepthUp()
    {
        if (_current is not null) _current.Depth++;
    }

    private void DepthDown()
    {
        if (_current is not null) _current.Depth--;
    }

    private void Emit(int scaleIndex, double beats, double velocity = 0.7)
    {
        if (_current is null) return;

        var octave = Math.Clamp(5 - _current.Depth, 2, 7);
        var freq = Scales.Frequency(_options.Scale, scaleIndex, octave);
        var duration = beats * _options.BeatSeconds;

        _current.Emit(new Note(freq, 0, duration, velocity));
    }

    private sealed class VoiceBuilder
    {
        public string Name { get; }
        public List<Note> Notes { get; } = new();
        public double Time;
        public int Depth;

        public VoiceBuilder(string name)
        {
            Name = name;
        }

        public void Emit(Note note)
        {
            Notes.Add(note with { StartTime = Time });
            Time += note.DurationSeconds;
        }

        public Voice Build(Instrument instrument)
            => new(Name, instrument, Notes);
    }
}
