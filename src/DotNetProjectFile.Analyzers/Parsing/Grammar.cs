namespace DotNetProjectFile.Parsing;

[Inheritable]
public class Grammar<TSyntaxKind>
    where TSyntaxKind : struct, Enum
{
    public delegate Lexer<TSyntaxKind> Rule(Lexer<TSyntaxKind> lexar);
}

public class Grammar<TGrammar, TSyntaxKind> : Grammar<TSyntaxKind>
    where TGrammar : Grammar<TGrammar, TSyntaxKind>, new()
    where TSyntaxKind : struct, Enum
{
    protected static readonly TGrammar Resolve = new();

    [Pure]
    public static Lexer<TSyntaxKind> eol(Lexer<TSyntaxKind> l) => l +
        eof | ch('\n', Resolve.EndOfLineKind) | literal("\r\n", Resolve.EndOfLineKind);

    [Pure]
    public static Lexer<TSyntaxKind> eof(Lexer<TSyntaxKind> l)
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
    public static Rule ch(char c, TSyntaxKind kind) =>
        l => Rules.ch(l, c, kind);

    [Pure]
    public static Rule literal(string str, TSyntaxKind kind) =>
        l => Rules.literal(l, str, kind);

    [Pure]
    public static Rule regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TSyntaxKind kind) =>
        l => Rules.regex(l, pattern, kind);

    [Pure]
    public static Rule whitespace(TSyntaxKind kind) =>
        l => Rules.whitespace(l, kind);

    protected virtual TSyntaxKind EndOfLineKind => Enum.TryParse<TSyntaxKind>("EndOfLine", out var kind) ? kind : default;

    [Pure]
    protected virtual TSyntaxKind CharKind(char c)
    {
        int obj = c;
        return (TSyntaxKind)(object)obj;
    }

    [Pure]
    protected virtual TSyntaxKind KeywordKind(string keyword)
        => Enum.TryParse<TSyntaxKind>($"{keyword}Keyword", true, out var kind) ? kind : default;

    private static class Rules
    {
        public static Lexer<TSyntaxKind> Not(Lexer<TSyntaxKind> l, Rule rule)
        {
            var next = rule(l);

            return next.State == LexerState.Match
                ? l.NoMatch()
                : l;
        }

        public static Lexer<TSyntaxKind> Option(Lexer<TSyntaxKind> l, Rule rule)
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

        public static Lexer<TSyntaxKind> Repeat(Lexer<TSyntaxKind> l, Rule rule, int min, int max)
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
        public static Lexer<TSyntaxKind> whitespace(Lexer<TSyntaxKind> l, TSyntaxKind kind)
            => l.Match(s => s.Matches(c => c == ' ' || c == '\t'), kind);

        [Pure]
        public static Lexer<TSyntaxKind> ch(Lexer<TSyntaxKind> l, char c, TSyntaxKind kind)
           => l.Match(s => s.StartsWith(c), kind);

        [Pure]
        public static Lexer<TSyntaxKind> literal(Lexer<TSyntaxKind> l, string str, TSyntaxKind kind)
            => l.Match(s => s.StartsWith(str), kind);

        [Pure]
        public static Lexer<TSyntaxKind> regex(Lexer<TSyntaxKind> l, string pattern, TSyntaxKind kind)
            => l.Match(m => m.Matches(pattern), kind);
    }
}
