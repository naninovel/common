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
    private readonly HashSet<ManagedTextRecord> records = [];
    private readonly StringBuilder commentBuilder = new();
    private string header = "";

    /// <summary>
    /// Creates document from specified serialized text string.
    /// </summary>
    public ManagedTextDocument Parse (string text)
    {
        Reset();
        foreach (var (line, index) in text.TrimJunk().IterateLinesIndexed())
            if (line.StartsWithOrdinal(RecordCommentLiteral))
                ParseCommentLine(line, index);
            else ParseRecordLine(line);
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

    private void ParseRecordLine (string line)
    {
        var id = line.GetBefore(RecordInlineKeyLiteral);
        if (string.IsNullOrWhiteSpace(id)) return;
        var value = line.Substring(id.Length + RecordInlineKeyLiteral.Length);
        records.Add(new ManagedTextRecord(id, value, commentBuilder.ToString()));
        commentBuilder.Clear();
    }
}
