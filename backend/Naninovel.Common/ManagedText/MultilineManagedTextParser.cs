using System.Text;
using Naninovel.Utilities;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows creating <see cref="ManagedTextDocument"/> from serialized text in multiline format.
/// </summary>
/// <remarks>
/// Multiline format spec:
/// <code>
/// # key (space around key is ignored)
/// ; comment (optional, space around comment is ignored)
/// value (all lines until next key are joined, space is preserved)
/// </code>
/// </remarks>
public class MultilineManagedTextParser
{
    private readonly HashSet<ManagedTextRecord> records = [];
    private readonly StringBuilder valueBuilder = new();
    private string header = "", lastKey = "", lastComment = "";

    /// <summary>
    /// Creates document from specified serialized text string.
    /// </summary>
    public ManagedTextDocument Parse (string text)
    {
        Reset();
        foreach (var (line, index) in text.TrimJunk().IterateLinesIndexed())
            if (line.StartsWithOrdinal(RecordMultilineKeyLiteral))
                ParseKeyLine(line);
            else if (line.StartsWithOrdinal(RecordCommentLiteral))
                ParseCommentLine(line, index);
            else ParseValueLine(line);
        if (!string.IsNullOrEmpty(lastKey)) AddLastRecord();
        return new ManagedTextDocument(records, header);
    }

    private void ParseKeyLine (string line)
    {
        if (!string.IsNullOrEmpty(lastKey)) AddLastRecord();
        lastKey = line.GetAfterFirst(RecordMultilineKeyLiteral).Trim();
        valueBuilder.Clear();
        lastComment = "";
    }

    private void ParseCommentLine (string line, int index)
    {
        lastComment = line.GetAfterFirst(RecordCommentLiteral);
        if (lastComment.Length > 0 && lastComment[0] == ' ')
            lastComment = lastComment.Substring(1);
        if (index == 0) header = lastComment;
    }

    private void ParseValueLine (string line)
    {
        valueBuilder.Append(line);
    }

    private void AddLastRecord ()
    {
        records.Add(new(lastKey, valueBuilder.ToString(), lastComment));
        valueBuilder.Clear();
        lastKey = lastComment = "";
    }

    private void Reset ()
    {
        records.Clear();
        valueBuilder.Clear();
        header = lastKey = lastComment = "";
    }
}
