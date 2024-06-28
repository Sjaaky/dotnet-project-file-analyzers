using DotNetProjectFile.Parsing;
using Microsoft.CodeAnalysis.Text;

namespace Ini.Parse_specs;

public class Parses
{
    [Test]
    public void File()
    {
        using var stream = new FileStream("../../../../../.editorconfig", FileMode.Open, FileAccess.Read);
        var source = SourceText.From(stream);

        var lexer = Lexer<DotNetProjectFile.Ini.SyntaxKind>.Tokenize(source, DotNetProjectFile.Ini.Grammar.file);

        var tokens = lexer.Tokens.ToArray();

        tokens.Should().NotBeEmpty();
    }
}
