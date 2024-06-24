using DotNetProjectFile.Parsing;
using Microsoft.CodeAnalysis.Text;

namespace Parsing.Source_text_match_specs;

public class Matches
{
    [TestCase(1, 'e')]
    [TestCase(2, 'l')]
    [TestCase(4, 'o')]
    [TestCase(7, 'W')]
    public void starts_with_char(int start, char ch)
    {
        var source = new SourceTextSpan(Source.Text("Hello, World!"), new(start, 3));
        source.StartsWith(ch).Should().Be(new TextSpan(start, 1));
    }

    [TestCase(1, 4, "ello")]
    [TestCase(2, 2, "ll")]
    [TestCase(4, 7, "o, Worl")]
    [TestCase(7, 1, "W")]
    public void starts_with_str(int start, int length, string str)
    {
        var source = new SourceTextSpan(Source.Text("Hello, World!"), new(start, 13 - start));
        source.StartsWith(str).Should().Be(new TextSpan(start, length));
    }

    [TestCase(0, 6, "[^ ]+")]
    [TestCase(1, 5, "[^ ]+")]
    [TestCase(2, 2, "l+")]
    [TestCase(7, 1, "W")]
    public void regex(int start, int length, string regex)
    {
        var source = new SourceTextSpan(Source.Text("Hello, World!"), new(start, 13 - start));
        source.Matches(regex).Should().Be(new TextSpan(start, length));
    }
}

public class Does_not_match
{
    [Test]
    public void not_matching_char()
        => new SourceTextSpan(Source.Text("a"), new(0, 1)).StartsWith('A').Should().BeNull();

    [Test]
    public void char_on_empty_source_text()
        => new SourceTextSpan(Source.Text("a"), new(0, 0)).StartsWith('a').Should().BeNull();

    [Test]
    public void not_matching_string()
        => new SourceTextSpan(Source.Text("abc"), new(0, 3)).StartsWith("ABc").Should().BeNull();

    [Test]
    public void string_longer_then_source_text()
        => new SourceTextSpan(Source.Text("abc"), new(0, 2)).StartsWith("ABC").Should().BeNull();

    [Test]
    public void regex()
        => new SourceTextSpan(Source.Text("abc"), new(0, 2)).Matches("[0-9]+").Should().BeNull();
}

public class Regexes
{
    [Test]
    public void match_from_start_only()
    {
        var source = new SourceTextSpan(Source.Text("abcde"), new(0, 5));
        source.Matches("b.+").Should().BeNull();
    }

    [Test]
    public void do_not_prefix_if_specified()
    {
        var source = new SourceTextSpan(Source.Text("abcde"), new(0, 5));
        source.Matches("^a+").Should().Be(new TextSpan(0, 1));
    }
}
