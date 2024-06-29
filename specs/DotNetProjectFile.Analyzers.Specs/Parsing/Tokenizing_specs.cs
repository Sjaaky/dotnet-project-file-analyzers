using DotNetProjectFile.Parsing;
using Lex = DotNetProjectFile.Parsing.Lexer<Parsing.Tokenizing_specs.SyntaxKind>;

namespace Parsing.Tokenizing_specs;

public static class Not
{
    public class Matches
    {
        [Test]
        public void tokens_afterwards()
        {
            var lex = Lex.Tokenize(Source.Text("ABCD"),
                l => l + Grammar.Not(Grammar.regex("[0-9]+", SyntaxKind.Number)) + Grammar.regex(".+", SyntaxKind.Name));

            lex.Should().HaveTokenized(
                LexToken.New(0, 4, "ABCD", SyntaxKind.Name));
        }
    }

    public class Does_not_match
    {
        [Test]
        public void any_tokens_after_match()
        {
            var lex = Lex.Tokenize(Source.Text("class17ABCD"),
                l => l + Grammar.keyword("class") + Grammar.Not(Grammar.regex("[0-9]+", SyntaxKind.Number)) + Grammar.regex(".+", SyntaxKind.Name));

            lex.Should().NotHaveTokenized(
                LexToken.New(0, 5, "class", SyntaxKind.ClassKeyword));
        }
    }
}

public static class Regular_expressions
{
    public class Match
    {
        [Test]
        public void zero_length_match()
        {
            var lex = Lex.Tokenize(Source.Text("BBB"),
                l => l + Grammar.regex("A*", SyntaxKind.Word) + Grammar.regex("B*", SyntaxKind.Word));

            lex.Should()
                .HaveTokenized(LexToken.New(0, 3, "BBB",SyntaxKind.Word));
        }
    }

    public class Do_not_match
    {
        [Test]
        public void new_line_with_dot_star()
        {
            var lex = Lex.Tokenize(Source.Text("Hello\nWorld"),
                l => l + Grammar.regex(".*", SyntaxKind.Word) + Grammar.eol + Grammar.regex(".*", SyntaxKind.Word));

            lex.Should().HaveTokenized(
                LexToken.New(0, 5, "Hello", SyntaxKind.Word),
                LexToken.New(5, 1, "\n", SyntaxKind.EndOfLine),
                LexToken.New(6, 5, "World", SyntaxKind.Word));
        }
    }

}

public class Matches
{
    //[Test]
    //public void keywords()
    //{
    //    var lex = Lex.Tokenize(Source.Text("class"), Grammar.some_keyword);
    //    lex.Should()
    //        .HaveTokenized(SyntaxKind.ClassKeyword);
    //}

    //[Test]
    //public void and_sequence()
    //{
    //    var lex = Lex.Tokenize(Source.Text("hello  world"), Grammar.multiple_words);
    //    lex.Should()
    //        .HaveTokenized(SyntaxKind.Word, SyntaxKind.Whitespace, SyntaxKind.Word);
    //}


    //[Test]
    //public void repeat()
    //{
    //    var lex = Lex.Tokenize(
    //        Source.Text("hello\nworld"),
    //        Grammar.all);

    //    lex.Should()
    //        .HaveTokenized(SyntaxKind.Word, SyntaxKind.EndOfLine, SyntaxKind.Word);
    //}

    //[Test]
    //public void single_char()
    //{
    //    var lex = Lex.Tokenize(
    //        Source.Text("(hello)"),
    //        Grammar.brackets);

    //    lex.Should()
    //        .HaveTokenized(SyntaxKind.BracketOpen, SyntaxKind.Word, SyntaxKind.BracketClose);
    //}
}

public class Does_not_match
{
    [Test]
    public void required_match_on_eof()
    {
        var lex = Lex.Tokenize(Source.Text("hello "), Grammar.multiple_words);

        lex.Should()
            .NotHaveTokenized();
    }
}

internal sealed class Grammar : Grammar<Grammar, SyntaxKind>
{
    public static Lex multiple_words(Lex l) => l +
        word + space + word;

    public static Lex some_keyword(Lex l) => l +
        keyword("class");

    public static Lex all(Lex l) => l +
        _(some_line, 0..);

    public static Lex brackets(Lex l) => l + 
        ch('(') + word + ch(')');

    public static Lex declaration(Lex l) => l +
        keyword("const") + regex("[a-z][a-z0-9_]+", SyntaxKind.Name);

    public static Lex word(Lex l) => l +
        regex("[a-z][a-z0-9_]+", SyntaxKind.Word);

    public static Lex some_line(Lex l) => l +
        word + eol;

    public static Lex space(Lex l) => whitespace(SyntaxKind.Whitespace)(l);

    public static Rule _(Rule rule, Range repeat) 
        => repeat.End.IsFromEnd
        ? Repeat(rule, repeat.Start.Value)
        : Repeat(rule, repeat.Start.Value, repeat.End.Value);
}


internal enum SyntaxKind
{
    Unknown = 0,
    Word = 1,
    BracketOpen = '(',
    BracketClose = ')',
    Name,
    EndOfLine,
    ClassKeyword,
    Whitespace,
    Number,
}
