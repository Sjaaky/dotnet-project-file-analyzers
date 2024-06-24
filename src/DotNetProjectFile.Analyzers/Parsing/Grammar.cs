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
        l =>
        {
            var curr = l;
            var repeat = 0;

            while (repeat++ < max && curr.State != LexerState.Done)
            {
                var next = rule(curr);

                if (next.State == LexerState.NoMatch)
                {
                    // we have no match
                    if (repeat < min)
                    {
                        return next;
                    }
                    // we have enough.
                    else
                    {
                        return curr;
                    }
                }
                curr = next;
            }

            if (repeat < min)
            {
                return curr.NoMatch();
            }

            return curr;
        };

    [Pure]
    public static Rule Option(Rule rule) =>
        l => rule(l) is { State: not LexerState.NoMatch } m
            ? m
            : l;

    [Pure]
    public static Rule ch(char c) => ch(c, Resolve.CharKind(c));

    [Pure]
    public static Rule ch(char c, TSyntaxKind kind) =>
        l => l.Match(s => s.StartsWith(c), kind);

    [Pure]
    public static Rule literal(string str, TSyntaxKind kind) =>
        l => l.Match(s => s.StartsWith(str), kind);

    [Pure]
    public static Rule regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TSyntaxKind kind) =>
        l => l.Match(m => m.Matches(pattern), kind);

    [Pure]
    public static Rule whitespace(TSyntaxKind kind) =>
        l => l + regex("^[ \t]+", kind);

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
}
