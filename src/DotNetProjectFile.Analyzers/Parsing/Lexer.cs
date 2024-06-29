using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("Tokens: {Tokens.Count}, {Span}, Text = {Source.ToString(Span)}")]
public readonly struct Lexer<TSyntaxKind>
    where TSyntaxKind : struct, Enum
{
    private Lexer(SourceSpan span, IReadOnlyCollection<SyntaxToken<TSyntaxKind>> tokens, LexerState state)
    {
        Span = span;
        Tokens = tokens;
        State = state;
    }

    public IReadOnlyCollection<SyntaxToken<TSyntaxKind>> Tokens { get; }

    public readonly SourceSpan Span;

    public LexerState State { get; }

    [Pure]
    public Lexer<TSyntaxKind> NoMatch() => new(Span, Tokens, LexerState.NoMatch);

    [Pure]
    public Lexer<TSyntaxKind> Match(SourceSpan.Match match, TSyntaxKind kind) => match(Span) switch
    {
        null => NoMatch(),
        var span when span.Value.Length == 0 => this,
        var span => New(span.Value, kind),
    };

    public static Lexer<TSyntaxKind> operator +(Lexer<TSyntaxKind> lexer, Grammar<TSyntaxKind>.Rule rule) => lexer.State switch
    {
        LexerState.Match or
        LexerState.Done => rule(lexer),

        // LexerState.NoMatch
        _ => lexer,
    };

    public static Lexer<TSyntaxKind> operator |(Lexer<TSyntaxKind> lexer, Grammar<TSyntaxKind>.Rule rule) => lexer.State switch
    {
        LexerState.NoMatch => rule(lexer),

        // LexerState.Match
        // LexerState.Done
        _ => lexer,
    };

    [Pure]
    private Lexer<TSyntaxKind> New(TextSpan span, TSyntaxKind kind)
    {
        var token = new SyntaxToken<TSyntaxKind>(span, kind, Span.Source);
        var trimm = Span.Trim(span.Length);
        var state = trimm.IsEmpty ? LexerState.Done : LexerState.Match;
        return new(trimm, [.. Tokens, token], state);
    }

    [Pure]
    public static Lexer<TSyntaxKind> New(SourceText source)
        => new(new(source, new(0, source.Length)), [], source.Length == 0 ? LexerState.Done : LexerState.Match);

    [Pure]
    public static Lexer<TSyntaxKind> Tokenize(SourceText source, Grammar<TSyntaxKind>.Rule rule)
    {
        var lexer = Lexer<TSyntaxKind>.New(source);
        return rule(lexer);
    }
}
