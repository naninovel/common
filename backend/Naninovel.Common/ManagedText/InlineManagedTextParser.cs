using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows creating <see cref="ManagedTextDocument"/> from serialized text in inline format.
/// </summary>
/// <remarks>
/// Inline format spec:
/// <code>
/// ; comment (optional, content after semicolon is trimmed)
/// key: value (single space after colon is required and is not part of the value)
/// </code>
/// </remarks>
public class InlineManagedTextParser
{
    /// <summary>
    /// Exception thrown when managed text parsing fails due to incorrect document format.
    /// </summary>
    public class SyntaxError (string message) : Error(message);

    private readonly HashSet<ManagedTextRecord> records = [];
    private readonly StringBuilder commentBuilder = new();
    private string header = "";

    /// <summary>
    /// Creates document from specified serialized text string.
    /// </summary>
    /// <exception cref="SyntaxError">Parsing failed to incorrect document format.</exception>
    public ManagedTextDocument Parse (string text)
    {
        Reset();
        foreach (var (line, index) in text.TrimJunk().IterateLinesIndexed())
            if (string.IsNullOrWhiteSpace(line)) continue;
            else if (line.StartsWithOrdinal(RecordCommentLiteral))
                ParseCommentLine(line, index);
            else ParseRecordLine(line, index);
        return new ManagedTextDocument(records, header);
    }

    private void Reset ()
    {
        records.Clear();
        commentBuilder.Clear();
        header = "";
    }

    private void ParseCommentLine (string line, int index)
    {
        if (index == 0)
        {
            header = line.GetAfterFirst(RecordCommentLiteral);
            return;
        }

        var comment = line.GetAfterFirst(RecordCommentLiteral);
        if (commentBuilder.Length > 0)
            commentBuilder.Append('\n');
        commentBuilder.Append(comment);
    }

    private void ParseRecordLine (string line, int index)
    {
        var id = line.GetBefore(RecordInlineKeyLiteral);
        if (string.IsNullOrWhiteSpace(id)) throw new SyntaxError($"Incorrect record syntax at line #{index + 1}.");
        var value = line.Substring(id.Length + RecordInlineKeyLiteral.Length);
        records.Add(new ManagedTextRecord(id, value, commentBuilder.ToString()));
        commentBuilder.Clear();
    }
}
