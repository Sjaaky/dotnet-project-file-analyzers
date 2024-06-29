using Microsoft.CodeAnalysis.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Parsing.Source_span_specs;

public class Line
{
    [TestCase(0, "Hello\nWorld!", "Hello")]
    [TestCase(0, "Hello\r\nWorld!", "Hello")]
    [TestCase(6, "Hello\nWorld!", "World!")]
    [TestCase(12, "Hello\nWorld\n!", "!")]
    public void trims_until_new_line(int left, string text, string line)
    {
        var span = Source.Span(text).Trim(left);
        span.Source.ToString(span.Line()).Should().Be(line);
    }
}

public class Matches
{
    [Test]
    public void from_left_only()
    {
        Source.Span("Hello").Matches("el+").Should().BeNull();
    }

    [TestCase("^Hel+")]
    [TestCase("Hel+")]
    public void from_left_only(string regex)
    {
        Source.Span("Hello").Matches(regex).Should().Be(new TextSpan(0, 4));
    }

    [Test]
    public void line_only()
    {
        Source.Span("Hello\r\n").Matches(".+").Should().Be(new TextSpan(0, 5));
    }

    [TestCase(0, "Hello")]
    [TestCase(2, "Hello")]
    [TestCase(5, "Hello")]
    public void empty_pattern(int left, string text)
    {
        var span = Source.Span(text).Trim(left);
        span.Matches("Z*").Should().Be(new TextSpan(left, 0));
    }

    [TestCase(0, "Hello")]
    [TestCase(2, "Hello")]
    [TestCase(5, "Hello")]
    public void returns_null_on_no_match(int left, string text)
    {
        var span = Source.Span(text).Trim(left);
        span.Matches("Z+").Should().BeNull();
    }
}

public class Starts_with
{
    public class Char
    {
        [TestCase(0, "Hello", 'A')]
        [TestCase(1, "Hello", 'H')]
        [TestCase(5, "Hello", '0')]
        public void returns_null_if_no_match(int left, string text, char first)
        {
            var span = Source.Span(text).Trim(left);
            span.StartsWith(first).Should().BeNull();
        }

        [TestCase("hello", 'H')]
        [TestCase("Hello", 'h')]
        public void is_case_sensitive(string text, char first)
        {
            var span = Source.Span(text);
            span.StartsWith(first).Should().BeNull();
        }

        [TestCase(0, "hello")]
        [TestCase(1, " hell")]
        public void returns_length_1(int left, string text)
        {
            var span = Source.Span(text).Trim(left);
            span.StartsWith('h').Should().Be(new TextSpan(left, 1));
        }
    }

    public class String
    {
        [TestCase(0, "Hello", "HA")]
        [TestCase(1, "Hello", "be")]
        [TestCase(4, "Hello", "o ")]
        [TestCase(5, "Hello", "ox")]
        public void returns_null_if_no_match(int left, string text, string first)
        {
            var span = Source.Span(text).Trim(left);
            span.StartsWith(first).Should().BeNull();
        }

        [TestCase("hello", "He")]
        [TestCase("Hello", "he")]
        [TestCase("Hello", "HE")]
        public void is_case_sensitive(string text, string first)
        {
            var span = Source.Span(text);
            span.StartsWith(first).Should().BeNull();
        }

        [TestCase(0, "hello", "hell")]
        [TestCase(1, " hell", "hel")]
        public void returns_length_of_left_with(int left, string text, string first)
        {
            var span = Source.Span(text).Trim(left);
            span.StartsWith(first).Should().Be(new TextSpan(left, first.Length));
        }
    }
}
