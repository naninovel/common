using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal static class Utilities
{
    private static readonly char[] controlChars = {
        ExpressionOpen[0],
        ExpressionClose[0],
        InlinedOpen[0],
        InlinedClose[0],
        '\\'
    };

    public static bool IsControlChar (char @char, char next)
    {
        if (@char == TextIdOpen[0] && next == TextIdOpen[1]) return true;
        for (int i = 0; i < controlChars.Length; i++)
            if (controlChars[i] == @char)
                return true;
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
