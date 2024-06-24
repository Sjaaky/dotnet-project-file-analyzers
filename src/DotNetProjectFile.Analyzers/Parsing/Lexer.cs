using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("Tokens: {Tokens.Count}, {Span}, Text = {Source.ToString(Span)}")]
public readonly struct Lexer<TSyntaxKind>
    where TSyntaxKind : struct, Enum
{
    private Lexer(TextSpan span, SourceText source, IReadOnlyCollection<SyntaxToken<TSyntaxKind>> tokens, LexerState state)
    {
        Span = span;
        Source = source;
        Tokens = tokens;
        State = state;
    }

    public IReadOnlyCollection<SyntaxToken<TSyntaxKind>> Tokens { get; }

    public readonly SourceText Source;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public readonly TextSpan Span;

    public LexerState State { get; }

    [Pure]
    public Lexer<TSyntaxKind> NoMatch() => new(Span, Source, Tokens, LexerState.NoMatch);

    [Pure]
    public Lexer<TSyntaxKind> Match(SourceTextSpan.Match match, TSyntaxKind kind)
        => match(new(Source, Span)) is { } span
        ? New(span, kind)
        : NoMatch();

    public static Lexer<TSyntaxKind> operator +(Lexer<TSyntaxKind> lexer, Grammar<TSyntaxKind>.Rule rule) => lexer.State switch
    {
        LexerState.Match => rule(lexer),
        LexerState.Done => lexer.NoMatch(),

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
        var token = new SyntaxToken<TSyntaxKind>(span, kind, Source);
        var lefto = new TextSpan(Span.Start + span.Length, Span.Length - span.Length);
        var state = lefto.IsEmpty ? LexerState.Done : LexerState.Match;
        return new(lefto, Source, [.. Tokens, token], state);
    }

    [Pure]
    public static Lexer<TSyntaxKind> New(SourceText source)
        => new(new(0, source.Length), source, [], source.Length == 0 ? LexerState.Done : LexerState.Match);

    [Pure]
    public static Lexer<TSyntaxKind> Tokenize(SourceText source, Grammar<TSyntaxKind>.Rule rule)
    {
        var lexer = Lexer<TSyntaxKind>.New(source);
        return rule(lexer);
    }
}
