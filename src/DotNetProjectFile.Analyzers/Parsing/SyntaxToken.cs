using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("{Span} {Text}, {Kind}")]
public readonly struct SyntaxToken<TSyntaxKind>(TextSpan span, TSyntaxKind kind, SourceText source)
{
    public readonly TextSpan Span = span;

    public readonly TSyntaxKind Kind = kind;

    public string Text => Source?.ToString(Span) ?? string.Empty;

    [Pure]
    public override string ToString() => Text;

    private readonly SourceText? Source = source;
}
