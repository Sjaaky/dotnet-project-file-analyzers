using DotNetProjectFile.Parsing;
using Microsoft.CodeAnalysis.Text;

namespace FluentAssertions;

internal sealed class LexerAssertions<TSyntaxKind>(Lexer<TSyntaxKind> subject) where TSyntaxKind : struct, Enum
{
    public Lexer<TSyntaxKind> Subject { get; } = subject;

    public void HaveTokenized(params LexToken<TSyntaxKind>[] expected)
    {
        Subject.State.Should().Be(LexerState.Done, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.ToArray();
        tokens.Should().BeEquivalentTo(expected);
    }

    public void NotHaveTokenized(params LexToken<TSyntaxKind>[] expected)
    {
        Subject.State.Should().Be(LexerState.NoMatch, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.ToArray();
        tokens.Should().BeEquivalentTo(expected);
    }
}

internal readonly record struct LexToken<TSyntaxKind>(TextSpan Span, string Text, TSyntaxKind Kind) where TSyntaxKind : struct, Enum;

internal static class LexToken
{
    public static LexToken<TSyntaxKind> New<TSyntaxKind>(int start, int length, string text, TSyntaxKind kind) where TSyntaxKind : struct, Enum
        => new(new(start, length), text, kind);
}

