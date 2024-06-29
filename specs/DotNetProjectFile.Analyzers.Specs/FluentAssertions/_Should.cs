using DotNetProjectFile.Parsing;

namespace FluentAssertions;

internal static class DotNetProjectFileAnalyzersFluentAssertionsExtensions
{
    [Pure]
    public static LexerAssertions<TKInd> Should<TKInd>(this Lexer<TKInd> lexer)
        where TKInd : struct, Enum
        => new(lexer);
}
