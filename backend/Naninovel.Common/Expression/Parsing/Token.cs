namespace Naninovel.Expression;

internal readonly struct Token (TokenType type, string content = "")
{
    public readonly TokenType Type = type;
    public readonly string Content = content;
}
