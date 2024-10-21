namespace Naninovel.Csv;

/// <summary>
/// Parses text in comma-separated values (CSV) format.
/// </summary>
public sealed class Reader
{
    /// <summary>
    /// Configures reading behaviour.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Size of the circular buffer; limits max length of the CSV line that can be processed (32KB by defaut).
        /// </summary>
        public int BufferSize { get; set; } = 32768;
        /// <summary>
        /// Whether to trim leading and trailing whitespace in fields, except when wrapped in quotes.
        /// </summary>
        public bool TrimFields { get; set; } = true;
    }

    /// <summary>
    /// Number of fields in the last read row.
    /// </summary>
    public int FieldsCount { get; private set; }

    private const char delimiter = ',';

    private readonly TextReader reader;
    private readonly Options options;
    private readonly List<Field> fields = [];
    private readonly char[] buffer;
    private readonly int bufferLength;
    private readonly int bufferThreshold;
    private int lineStartPos;
    private int actualBufferLen;
    private int linesRead;

    public string this [int idx] => fields[idx].GetValue(buffer);

    public Reader (TextReader reader, Options? options = null)
    {
        this.reader = reader;
        this.options = options ?? new();
        bufferThreshold = Math.Min(this.options.BufferSize, 8192);
        bufferLength = this.options.BufferSize + bufferThreshold;
        buffer = new char[bufferLength];
    }

    public bool ReadRow ()
    {
        FieldsCount = 0;
        var eof = FillBuffer();
        if (actualBufferLen <= 0) return false;
        linesRead++;

        var maxPos = lineStartPos + actualBufferLen;
        var charPos = lineStartPos;
        var currentField = GetOrAddField(charPos);
        var ignoreQuote = false;

        while (FieldsCount == 1 && fields[0].Length == 0)
        {
            ReadNext();
            actualBufferLen -= charPos - lineStartPos;
            lineStartPos = charPos % bufferLength;
        }

        return true;

        void ReadNext ()
        {
            for (; charPos < maxPos; charPos++)
            {
                var charBufIdx = charPos < bufferLength ? charPos : charPos % bufferLength;
                var ch = buffer[charBufIdx];
                switch (ch)
                {
                    case '\"':
                        if (ignoreQuote) currentField.End = charPos;
                        else if (currentField.Quoted || currentField.Length > 0)
                        {
                            currentField.End = charPos;
                            currentField.Quoted = false;
                            ignoreQuote = true;
                        }
                        else
                        {
                            var endQuotePos = ReadQuotedFieldToEnd(charPos + 1, maxPos, eof, ref currentField.EscapedQuotesCount);
                            currentField.Start = charPos;
                            currentField.End = endQuotePos;
                            currentField.Quoted = true;
                            charPos = endQuotePos;
                        }
                        break;
                    case '\r':
                        if ((charPos + 1) < maxPos && buffer[(charPos + 1) % bufferLength] == '\n') charPos++;
                        charPos++;
                        return;
                    case '\n':
                        charPos++;
                        return;
                    default:
                        if (ch == delimiter)
                        {
                            currentField = GetOrAddField(charPos + 1);
                            ignoreQuote = false;
                            continue;
                        }
                        if (ch == ' ' && options.TrimFields) continue;
                        if (currentField.Length == 0) currentField.Start = charPos;
                        if (currentField.Quoted)
                        {
                            currentField.Quoted = false;
                            ignoreQuote = true;
                        }
                        currentField.End = charPos;
                        break;
                }
            }
            if (!eof) throw new InvalidDataException(GetLineTooLongMsg());
        }
    }

    private int ReadBlockAndCheckEof (char[] buffer, int start, int len, ref bool eof)
    {
        var read = reader.ReadBlock(buffer, start, len);
        if (read < len) eof = true;
        return read;
    }

    private bool FillBuffer ()
    {
        var eof = false;
        var toRead = bufferLength - actualBufferLen;
        if (toRead >= bufferThreshold)
        {
            var start = (lineStartPos + actualBufferLen) % buffer.Length;
            if (start >= lineStartPos)
            {
                actualBufferLen += ReadBlockAndCheckEof(buffer, start, buffer.Length - start, ref eof);
                if (lineStartPos > 0)
                    actualBufferLen += ReadBlockAndCheckEof(buffer, 0, lineStartPos, ref eof);
            }
            else actualBufferLen += ReadBlockAndCheckEof(buffer, start, toRead, ref eof);
        }
        return eof;
    }

    private string GetLineTooLongMsg ()
    {
        return $"CSV line #{linesRead} length exceeds buffer size ({options.BufferSize}).";
    }

    private int ReadQuotedFieldToEnd (int start, int maxPos, bool eof, ref int escapedQuotesCount)
    {
        var pos = start;
        for (; pos < maxPos; pos++)
        {
            var chIdx = pos < bufferLength ? pos : pos % bufferLength;
            var ch = buffer[chIdx];
            if (ch != '\"') continue;
            var hasNextCh = (pos + 1) < maxPos;
            if (hasNextCh && buffer[(pos + 1) % bufferLength] == '\"')
            {
                pos++;
                escapedQuotesCount++;
            }
            else return pos;
        }
        if (eof) return pos - 1;
        throw new InvalidDataException(GetLineTooLongMsg());
    }

    private Field GetOrAddField (int startIdx)
    {
        FieldsCount++;
        while (FieldsCount > fields.Count)
            fields.Add(new Field());
        var field = fields[FieldsCount - 1];
        field.Reset(startIdx);
        return field;
    }
}
