namespace DotNetProjectFile.Parsing;

[Inheritable]
public class Grammar<TKInd>
    where TKInd : struct, Enum
{
    public delegate Lexer<TKInd> Rule(Lexer<TKInd> lexar);
}

public class Grammar<TGrammar, TKInd> : Grammar<TKInd>
    where TGrammar : Grammar<TGrammar, TKInd>, new()
    where TKInd : struct, Enum
{
    protected static readonly TGrammar Resolve = new();

    [Pure]
    public static Lexer<TKInd> eol(Lexer<TKInd> l) => l +
        eof | ch('\n', Resolve.EndOfLineKind) | literal("\r\n", Resolve.EndOfLineKind);

    [Pure]
    public static Lexer<TKInd> eof(Lexer<TKInd> l)
        => l.State == LexerState.Done ? l : l.NoMatch();

    [Pure]
    public static Rule keyword(string keyword) => literal(keyword, Resolve.KeywordKind(keyword));

    [Pure]
    public static Rule Repeat(Rule rule, int min = 0, int max = int.MaxValue) =>
        l => Rules.Repeat(l, rule, min, max);

    [Pure]
    public static Rule Not(Rule rule) =>
        l => Rules.Not(l, rule);

    [Pure]
    public static Rule Option(Rule rule) =>
        l => Rules.Option(l, rule);

    [Pure]
    public static Rule ch(char c) => ch(c, Resolve.CharKind(c));

    [Pure]
    public static Rule ch(char c, TKInd kind) =>
        l => Rules.ch(l, c, kind);

    [Pure]
    public static Rule literal(string str, TKInd kind) =>
        l => Rules.literal(l, str, kind);

    [Pure]
    public static Rule regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TKInd kind) =>
        l => Rules.regex(l, pattern, kind);

    [Pure]
    public static Rule whitespace(TKInd kind) =>
        l => Rules.whitespace(l, kind);

    protected virtual TKInd EndOfLineKind => Enum.TryParse<TKInd>("EndOfLine", out var kind) ? kind : default;

    [Pure]
    protected virtual TKInd CharKind(char c)
    {
        int obj = c;
        return (TKInd)(object)obj;
    }

    [Pure]
    protected virtual TKInd KeywordKind(string keyword)
        => Enum.TryParse<TKInd>($"{keyword}Keyword", true, out var kind) ? kind : default;

#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members
    /// <remarks>
    /// This indirection only exists to make debugging easier.
    /// </remarks>
    private static class Rules
    {
        public static Lexer<TKInd> Not(Lexer<TKInd> l, Rule rule)
        {
            var next = rule(l);

            return next.State == LexerState.Match
                ? l.NoMatch()
                : l;
        }

        public static Lexer<TKInd> Option(Lexer<TKInd> l, Rule rule)
        {
            var next = rule(l);
            if (next.State != LexerState.NoMatch)
            {
                return next;
            }
            else
            {
                return l;
            }
        }

        public static Lexer<TKInd> Repeat(Lexer<TKInd> l, Rule rule, int min, int max)
        {
            var curr = l;
            var repeat = 0;

            while (repeat < max)
            {
                var next = rule(curr);

                switch (next.State)
                {
                    case LexerState.Match:
                        repeat++;
                        curr = next;
                        break;

                    case LexerState.Done:
                        repeat++;
                        return repeat >= min ? next : l.NoMatch();

                    case LexerState.NoMatch:
                    default:
                        return repeat >= min ? next : l.NoMatch();
                }
            }
            return l.NoMatch();
        }

        [Pure]
        public static Lexer<TKInd> whitespace(Lexer<TKInd> l, TKInd kind)
            => l.Match(s => s.Matches(c => c == ' ' || c == '\t'), kind);

        [Pure]
        public static Lexer<TKInd> ch(Lexer<TKInd> l, char c, TKInd kind)
           => l.Match(s => s.StartsWith(c), kind);

        [Pure]
        public static Lexer<TKInd> literal(Lexer<TKInd> l, string str, TKInd kind)
            => l.Match(s => s.StartsWith(str), kind);

        [Pure]
        public static Lexer<TKInd> regex(Lexer<TKInd> l, string pattern, TKInd kind)
            => l.Match(m => m.Matches(pattern), kind);
    }
}
