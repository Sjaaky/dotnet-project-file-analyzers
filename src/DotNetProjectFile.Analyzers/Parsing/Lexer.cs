using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("Tokens: {Tokens.Count}, {Span}, Text = {Source.ToString(Span)}")]
public readonly struct Lexer<TKInd>
    where TKInd : struct, Enum
{
    private Lexer(SourceSpan span, IReadOnlyCollection<SyntaxToken<TKInd>> tokens, LexerState state)
    {
        Span = span;
        Tokens = tokens;
        State = state;
    }

    public IReadOnlyCollection<SyntaxToken<TKInd>> Tokens { get; }

    public readonly SourceSpan Span;

    public LexerState State { get; }

    [Pure]
    public Lexer<TKInd> NoMatch() => new(Span, Tokens, LexerState.NoMatch);

    [Pure]
    public Lexer<TKInd> Match(SourceSpan.Match match, TKInd kind) => match(Span) switch
    {
        null => NoMatch(),
        var span when span.Value.Length == 0 => this,
        var span => New(span.Value, kind),
    };

    public static Lexer<TKInd> operator +(Lexer<TKInd> lexer, Grammar<TKInd>.Rule rule) => lexer.State switch
    {
        LexerState.Match or
        LexerState.Done => rule(lexer),

        // LexerState.NoMatch
        _ => lexer,
    };

    public static Lexer<TKInd> operator |(Lexer<TKInd> lexer, Grammar<TKInd>.Rule rule) => lexer.State switch
    {
        LexerState.NoMatch => rule(lexer),

        // LexerState.Match
        // LexerState.Done
        _ => lexer,
    };

    [Pure]
    private Lexer<TKInd> New(TextSpan span, TKInd kind)
    {
        var token = new SyntaxToken<TKInd>(span, kind, Span.Source);
        var trimm = Span.Trim(span.Length);
        var state = trimm.IsEmpty ? LexerState.Done : LexerState.Match;
        return new(trimm, [.. Tokens, token], state);
    }

    [Pure]
    public static Lexer<TKInd> New(SourceText source)
        => new(new(source, new(0, source.Length)), [], source.Length == 0 ? LexerState.Done : LexerState.Match);

    [Pure]
    public static Lexer<TKInd> Tokenize(SourceText source, Grammar<TKInd>.Rule rule)
    {
        var lexer = Lexer<TKInd>.New(source);
        return rule(lexer);
    }
}
