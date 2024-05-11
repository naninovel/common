namespace Naninovel.Parsing;

internal class Utilities (ISyntax stx)
{
    public bool IsPlainTextControlChar (char ch, char next = default)
    {
        if (ch == '\\') return true;
        if (ch == stx.ExpressionOpen[0] || ch == stx.ExpressionClose[0]) return true;
        if (ch == stx.InlinedOpen[0] || ch == stx.InlinedClose[0]) return true;
        if (ch == stx.TextIdOpen[0] && next == stx.TextIdOpen[1]) return true;
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
