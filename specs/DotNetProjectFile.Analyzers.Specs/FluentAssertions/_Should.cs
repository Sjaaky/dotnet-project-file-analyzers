using DotNetProjectFile.Parsing;

namespace FluentAssertions;

internal static class DotNetProjectFileAnalyzersFluentAssertionsExtensions
{
    [Pure]
    public static LexerAssertions<TSyntaxKind> Should<TSyntaxKind>(this Lexer<TSyntaxKind> lexer)
        where TSyntaxKind : struct, Enum
        => new(lexer);
}
