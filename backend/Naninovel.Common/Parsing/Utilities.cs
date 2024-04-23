namespace Naninovel.Parsing;

internal class Utilities (Identifiers ids)
{
    public bool IsPlainTextControlChar (char ch, char next = default)
    {
        if (ch == '\\') return true;
        if (ch == ids.ExpressionOpen[0] || ch == ids.ExpressionClose[0]) return true;
        if (ch == ids.InlinedOpen[0] || ch == ids.InlinedClose[0]) return true;
        if (ch == ids.TextIdOpen[0] && next == ids.TextIdOpen[1]) return true;
        return false;
    }

    public bool IsEscaped (string value, int i)
    {
        return i > 0 && value[i - 1] == '\\' && !IsEscaped(value, i - 1);
    }

    public string UnescapeCharacter (string content, string character)
    {
        var escaped = "\\" + character;
        return content.Contains(escaped) ?
            content.Replace(escaped, character) : content;
    }
}
