using DotNetProjectFile.Parsing;

using Lex = DotNetProjectFile.Parsing.Lexer<DotNetProjectFile.Ini.SyntaxKind>;

namespace DotNetProjectFile.Ini;

public sealed class Grammar : Grammar<Grammar, SyntaxKind>
{
    public static Lex file(Lex l) => l +
        Repeat(section);

    public static Lex section(Lex l) => l +
        Option(header) + Repeat(line, 1);

    public static Lex header(Lex l) => l +
        ch('[') + header_name + ch(']') + eol;

    public static Lex header_name(Lex l) => l +
        regex(@"[^\]\n]+", SyntaxKind.HeaderToken);

    public static Lex line(Lex l) => l +
        Option(kvp) + Option(comment) + eol;

    public static Lex kvp(Lex l) => l +
        space + key + space + assign + space + value;

    public static Lex key(Lex l) => l +
        regex(@"[^\s]+", SyntaxKind.ValueToken);

    public static Lex assign(Lex l) => l +
        ch('=') | ch(':');

    public static Lex value(Lex l) => l +
        regex(@"[^\s]+", SyntaxKind.ValueToken);

    public static Lex comment(Lex l) => l +
        space + comment_start + comment_text;

    public static Lex comment_start(Lex l) => l +
        ch('#') | ch(':');

    public static Lex comment_text(Lex l) => l +
        regex(".*", SyntaxKind.CommentToken);

    public static Lex space(Lex l) => l +
        Option(whitespace(SyntaxKind.WhitespaceToken));
}
