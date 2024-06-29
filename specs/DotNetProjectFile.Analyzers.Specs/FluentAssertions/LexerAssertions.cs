using DotNetProjectFile.Parsing;
using Microsoft.CodeAnalysis.Text;

namespace FluentAssertions;

internal sealed class LexerAssertions<TKInd>(Lexer<TKInd> subject) where TKInd : struct, Enum
{
    public Lexer<TKInd> Subject { get; } = subject;

    public void HaveTokenized(params LexToken<TKInd>[] expected)
    {
        Subject.State.Should().Be(LexerState.Done, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.ToArray();
        tokens.Should().BeEquivalentTo(expected);
    }

    public void NotHaveTokenized(params LexToken<TKInd>[] expected)
    {
        Subject.State.Should().Be(LexerState.NoMatch, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.ToArray();
        tokens.Should().BeEquivalentTo(expected);
    }
}

internal readonly record struct LexToken<TKInd>(TextSpan Span, string Text, TKInd Kind) where TKInd : struct, Enum;

internal static class LexToken
{
    public static LexToken<TKInd> New<TKInd>(int start, int length, string text, TKInd kind) where TKInd : struct, Enum
        => new(new(start, length), text, kind);
}

