using System.Collections.Generic;
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
/// value (all lines until next key are joined with br tag, space is preserved)
/// </code>
/// </remarks>
public class MultilineManagedTextParser
{
    private const string lineBreak = "<br>";
    private readonly HashSet<ManagedTextRecord> records = new();
    private readonly StringBuilder valueBuilder = new();
    private string lastKey = "", lastComment = "";

    /// <summary>
    /// Creates document from specified serialized text string.
    /// </summary>
    public ManagedTextDocument Parse (string text)
    {
        Reset();
        foreach (var line in text.TrimJunk().IterateLines())
            if (line.StartsWithOrdinal(RecordMultilineKeyLiteral))
                ParseKeyLine(line);
            else if (line.StartsWithOrdinal(RecordCommentLiteral))
                ParseCommentLine(line);
            else ParseValueLine(line);
        if (!string.IsNullOrEmpty(lastKey)) AddLastRecord();
        return new ManagedTextDocument(records);
    }

    private void ParseKeyLine (string line)
    {
        if (!string.IsNullOrEmpty(lastKey)) AddLastRecord();
        lastKey = line.GetAfterFirst(RecordMultilineKeyLiteral).Trim();
        valueBuilder.Clear();
        lastComment = "";
    }

    private void ParseCommentLine (string line)
    {
        lastComment = line.GetAfterFirst(RecordCommentLiteral).Trim();
    }

    private void ParseValueLine (string line)
    {
        if (valueBuilder.Length > 0)
            valueBuilder.Append(lineBreak);
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
        lastKey = lastComment = "";
    }
}
