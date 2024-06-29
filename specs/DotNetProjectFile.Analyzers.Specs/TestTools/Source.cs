using DotNetProjectFile.Parsing;
using Microsoft.CodeAnalysis.Text;

namespace Specs.TestTools;

internal static class Source
{
    [Pure]
    public static SourceText Text(string str) => SourceText.From(str);

    [Pure]
    public static SourceSpan Span(string str) => new(SourceText.From(str), new(0, str.Length));
}
