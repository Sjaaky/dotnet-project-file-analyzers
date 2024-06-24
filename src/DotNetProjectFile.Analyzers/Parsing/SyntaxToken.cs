using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("{DebuggerDisplay}")]
public readonly struct SyntaxToken<TSyntaxKind>(TextSpan span, TSyntaxKind kind, SourceText source)
{
    public readonly TextSpan Span = span;

    public readonly TSyntaxKind Kind = kind;

    [Pure]
    public override string ToString() => Source?.ToString(Span) ?? string.Empty;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[{Span}] {{{ToString()}}}, {Kind}";

    private readonly SourceText? Source = source;
}
