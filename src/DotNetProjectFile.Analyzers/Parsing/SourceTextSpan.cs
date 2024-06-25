using Microsoft.CodeAnalysis.Text;
using System.Text.RegularExpressions;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("{Span} {ToString()}")]
public readonly struct SourceTextSpan(SourceText source, TextSpan span)
{
    public delegate TextSpan? Match(SourceTextSpan sourceTextSpan);

    private static readonly TextSpan? NoMatch = null;

    public readonly SourceText Source = source;
    public readonly TextSpan Span = span;

    [Pure]
    public TextSpan? StartsWith(char ch)
        => !Span.IsEmpty && Source[Span.Start] == ch
        ? new(Span.Start, 1)
        : null;

    [Pure]
    public TextSpan? StartsWith(string str)
    {
        if (Span.Length >= str.Length)
        {
            var pos = Span.Start;

            for (var i = 0; i < str.Length; i++)
            {
                if (Source[pos++] != str[i]) return NoMatch;
            }
            return new(Span.Start, str.Length);
        }
        else return NoMatch;
    }

    [Pure]
    public TextSpan? Matches(Predicate<char> match)
    {
        var length = 0;
        for (var i = Span.Start; i < Span.Length; i++)
        {
            if (match(Source[i]))
            {
                length++;
            }
            else break;
        }

        return length != 0 ? new(Span.Start, length) : NoMatch;
    }

    [Pure]
    public TextSpan? Matches([StringSyntax(StringSyntaxAttribute.Regex)] string regex)
    {
        var pattern = regex[0] == '^' ? regex : '^' + regex;

        var match = Regex.Match(Source.ToString(Span), pattern, Options, Timeout);

        return match.Success
            ? new(Span.Start, match.Length)
            : NoMatch;
    }

    public override string ToString() => Source.ToString(Span);

    private static readonly RegexOptions Options = RegexOptions.CultureInvariant;
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
}
