using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("Tokens: {Tokens.Count}, {Span.Span}, Text = {Span}")]
public readonly struct Lexer<TKInd> where TKInd : struct, Enum
{
    private Lexer(SourceSpan span, IReadOnlyCollection<SyntaxToken<TKInd>> tokens, LexerState state)
    {
        Span = span;
        Tokens = tokens;
        State = state;
    }

    public readonly IReadOnlyCollection<SyntaxToken<TKInd>> Tokens;
    public readonly SourceSpan Span;
    public readonly LexerState State;

    [Pure]
    public Lexer<TKInd> NoMatch() => new(Span, Tokens, LexerState.NoMatch);

    [Pure]
    public Lexer<TKInd> Match(SourceSpan.Match match, TKInd kind) => match(Span) switch
    {
        null => NoMatch(),
        var span when span.Value.Length == 0 => this,
        var span => New(span.Value, kind),
    };

    public static Lexer<TKInd> operator +(Lexer<TKInd> lexer, Grammar<TKInd>.Rule rule)
        => lexer.State == LexerState.NoMatch
        ? lexer
        : rule(lexer);

    public static Lexer<TKInd> operator |(Lexer<TKInd> lexer, Grammar<TKInd>.Rule rule)
        => lexer.State != LexerState.NoMatch
        ? lexer
        : rule(lexer);

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
        => rule(Lexer<TKInd>.New(source));
}
