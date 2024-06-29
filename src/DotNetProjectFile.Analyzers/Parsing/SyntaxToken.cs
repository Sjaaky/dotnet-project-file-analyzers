using Microsoft.CodeAnalysis.Text;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("{Span} {Text}, {Kind}")]
public readonly struct SyntaxToken<TKInd>(TextSpan span, TKInd kind, SourceText source)
{
    public readonly TextSpan Span = span;

    public readonly TKInd Kind = kind;

    public string Text => Source?.ToString(Span) ?? string.Empty;

    [Pure]
    public override string ToString() => Text;

    private readonly SourceText? Source = source;
}
