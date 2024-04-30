namespace Naninovel.Expression;

internal readonly struct Token (TokenType type, int index, string content = "")
{
    public readonly TokenType Type = type;
    public readonly int Index = index;
    public readonly string Content = content;
}
