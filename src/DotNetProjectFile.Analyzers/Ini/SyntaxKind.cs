namespace DotNetProjectFile.Ini;

public enum SyntaxKind
{
    None,
    HeaderToken,
    WhitespaceToken,
    CommentToken,
    ValueToken,
    EndOfLine,
}
