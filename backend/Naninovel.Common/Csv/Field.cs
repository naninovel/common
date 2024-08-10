namespace Naninovel.Csv;

internal sealed class Field
{
    public int Start;
    public int End;
    public bool Quoted;
    public int EscapedQuotesCount;
    public int Length => End - Start + 1;

    public Field Reset (int start)
    {
        Start = start;
        End = start - 1;
        Quoted = false;
        EscapedQuotesCount = 0;
        return this;
    }

    public string GetValue (char[] buf)
    {
        if (Quoted)
        {
            var s = Start + 1;
            var lenWithoutQuotes = Length - 2;
            var val = lenWithoutQuotes > 0 ? GetString(buf, s, lenWithoutQuotes) : string.Empty;
            if (EscapedQuotesCount > 0) val = val.Replace("\"\"", "\"");
            return val;
        }
        var len = Length;
        return len > 0 ? GetString(buf, Start, len) : string.Empty;
    }

    private string GetString (char[] buf, int start, int len)
    {
        var bufLen = buf.Length;
        start = start < bufLen ? start : start % bufLen;
        var endIdx = start + len - 1;
        if (endIdx >= bufLen)
        {
            var prefixLen = buf.Length - start;
            var prefix = new string(buf, start, prefixLen);
            var suffix = new string(buf, 0, len - prefixLen);
            return prefix + suffix;
        }
        return new string(buf, start, len);
    }
}
