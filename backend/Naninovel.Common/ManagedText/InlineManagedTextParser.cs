using System.Collections.Generic;
using Naninovel.Utilities;
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
    private readonly HashSet<ManagedTextRecord> records = new();
    private string header = "", lastComment = "";

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
        header = lastComment = "";
    }

    private void ParseCommentLine (string line, int index)
    {
        lastComment = line.GetAfterFirst(RecordCommentLiteral).Trim();
        if (index == 0) header = lastComment;
    }

    private void ParseRecordLine (string line)
    {
        var id = line.GetBefore(RecordInlineKeyLiteral);
        if (string.IsNullOrWhiteSpace(id)) return;
        var value = line.Substring(id.Length + RecordInlineKeyLiteral.Length);
        records.Add(new ManagedTextRecord(id, value, lastComment));
        lastComment = "";
    }
}
