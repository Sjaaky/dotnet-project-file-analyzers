using DotNetProjectFile.Parsing;

namespace FluentAssertions;

internal sealed class LexerAssertions<TSyntaxKind>(Lexer<TSyntaxKind> subject) where TSyntaxKind : struct, Enum
{
    public Lexer<TSyntaxKind> Subject { get; } = subject;

    public void HaveTokenized(params TSyntaxKind[] expected)
    {
        Subject.State.Should().Be(LexerState.Done, because: "Did not fully tokenized.");
        var tokens = Subject.Tokens.Select(t => t.Kind);
        tokens.Should().BeEquivalentTo(expected);

        var input = Subject.Source.ToString();
        var output = string.Concat(Subject.Tokens);

        output.Should().Be(input);
    }

    public void NotHaveTokenized()
    {
        Subject.State.Should().Be(LexerState.NoMatch, because: "Did not fully tokenized.");
    }
}
