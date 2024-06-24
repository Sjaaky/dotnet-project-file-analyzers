namespace DotNetProjectFile.Parsing;

[Inheritable]
public class Grammar<TSyntaxKind>
    where TSyntaxKind : struct, Enum
{
    public delegate Lexer<TSyntaxKind> Rule(Lexer<TSyntaxKind> lexar);

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
    public static Rule ch(char c) =>
        l => l.Match(m => m.StartsWith(c), FromCh(c));

    [Pure]
    public static Rule ch(char c, TSyntaxKind kind) =>
        l => l.Match(m => m.StartsWith(c), kind);

    [Pure]
    public static Rule keyword(string keyword) =>
        l => l.Match(m => m.StartsWith(keyword), Enum.TryParse<TSyntaxKind>($"{keyword}Keyword", true, out var kind) ? kind : default);

    [Pure]
    public static Rule regex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern, TSyntaxKind kind) =>
        l => l.Match(m => m.Matches(pattern), kind);

    [Pure]
    public static Rule whitespace(TSyntaxKind kind) =>
        l => l + regex("^[ \t]+", kind);

    [Pure]
    public static Lexer<TSyntaxKind> eol(Lexer<TSyntaxKind> l)
        => l.Match(m => m.StartsWith('\n') ?? m.StartsWith("\r\n"), Enum.TryParse<TSyntaxKind>("EndOfLine", out var kind) ? kind : default);

    [Pure]
    private static TSyntaxKind FromCh(char c)
    {
        int obj = c;
        return (TSyntaxKind)(object)obj;
    }
}
