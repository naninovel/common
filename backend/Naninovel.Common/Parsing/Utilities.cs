using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal static class Utilities
{
    public static bool IsPlainTextControlChar (char ch, char next = default)
    {
        if (ch == '\\') return true;
        if (ch == ExpressionOpen[0] || ch == ExpressionClose[0]) return true;
        if (ch == InlinedOpen[0] || ch == InlinedClose[0]) return true;
        if (ch == TextIdOpen[0] && next == TextIdOpen[1]) return true;
        if (ch == AuthorAssign[0]) return true;
        return false;
    }

    public static bool IsEscaped (string value, int i)
    {
        return i > 0 && value[i - 1] == '\\' && !IsEscaped(value, i - 1);
    }

    public static string UnescapeCharacter (string content, string character)
    {
        var escaped = "\\" + character;
        return content.Contains(escaped) ?
            content.Replace(escaped, character) : content;
    }
}
