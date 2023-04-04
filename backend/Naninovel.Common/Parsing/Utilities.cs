using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal static class Utilities
{
    private static readonly char[] plainTextControlChars = {
        ExpressionOpen[0],
        ExpressionClose[0],
        InlinedOpen[0],
        InlinedClose[0],
        TextIdDelimiter[0],
        '\\'
    };

    public static bool IsPlainTextControlChar (char @char)
    {
        for (int i = 0; i < plainTextControlChars.Length; i++)
            if (plainTextControlChars[i] == @char)
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
