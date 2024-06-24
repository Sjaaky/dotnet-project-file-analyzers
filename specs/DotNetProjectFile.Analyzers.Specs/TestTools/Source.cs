using Microsoft.CodeAnalysis.Text;

namespace Specs.TestTools;

internal static class Source
{
    [Pure]
    public static SourceText Text(string str) => SourceText.From(str);
}
