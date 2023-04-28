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
    private string lastComment = "";

    /// <summary>
    /// Creates document from specified serialized text string.
    /// </summary>
    public ManagedTextDocument Parse (string text)
    {
        Reset();
        foreach (var line in text.TrimJunk().IterateLines())
            if (line.StartsWithOrdinal(RecordCommentLiteral))
                ParseCommentLine(line);
            else ParseRecordLine(line);
        return new ManagedTextDocument(records);
    }

    private void Reset ()
    {
        records.Clear();
        lastComment = "";
    }

    private void ParseCommentLine (string line)
    {
        lastComment = line.GetAfterFirst(RecordCommentLiteral).Trim();
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
