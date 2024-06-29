using Microsoft.CodeAnalysis.Text;
using System.Text.RegularExpressions;

namespace DotNetProjectFile.Parsing;

[DebuggerDisplay("{Span} {ToString()}")]
public readonly record struct SourceSpan(SourceText Source, TextSpan Span)
{
    public delegate TextSpan? Match(SourceSpan sourceTextSpan);

    private static readonly TextSpan? NoMatch = null;

    public string Text => Source.ToString(Span);

    public int Start => Span.Start;

    public int Length => Span.Length;

    public bool IsEmpty => Span.IsEmpty;

    [Pure]
    public SourceSpan Trim(int left) => new(Source, new(Start + left, Length - left));

    [Pure]
    public TextSpan Line()
    {
        var len = -1;
        var i = Span.Start;

        while (++len < Span.Length)
        {
            if (Source[i++] == '\n')
            {
                return len != 0 && Source[i - 2] == '\r'
                    ? new(Span.Start, len - 1)
                    : new(Span.Start, len);
            }
        }
        return Span;
    }

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
        var line = Line();
        var match = Regex.Match(Source.ToString(line), pattern, Options, Timeout);

        return match.Success
            ? new(Span.Start, match.Length)
            : NoMatch;
    }

    [Pure]
    public override string ToString() => Text;

    private static readonly RegexOptions Options = RegexOptions.CultureInvariant;
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
}
