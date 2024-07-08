using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows creating <see cref="ManagedTextDocument"/> from serialized text in multiline format.
/// </summary>
/// <remarks>
/// Multiline format spec:
/// <code>
/// # key1 (space around key is ignored)
/// ; comment (optional, space around comment is ignored)
/// value (all lines until next key are joined, space is preserved)
/// </code>
/// </remarks>
public class MultilineManagedTextParser
{
    private readonly HashSet<ManagedTextRecord> records = [];
    private readonly List<string> keys = [];
    private readonly List<string> comments = [];
    private readonly List<string> values = [];
    private readonly StringBuilder valueBuilder = new();
    private readonly StringBuilder commentBuilder = new();
    private string header = "";

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
        if (keys.Count > 0) AddLastRecord();
        return new ManagedTextDocument(records, header);
    }

    private void Reset ()
    {
        records.Clear();
        keys.Clear();
        comments.Clear();
        valueBuilder.Clear();
        commentBuilder.Clear();
        values.Clear();
        header = "";
    }

    private void ParseKeyLine (string line)
    {
        if (keys.Count > 0) AddLastRecord();
        keys.Add(line.GetAfterFirst(RecordMultilineKeyLiteral).Trim());
        if (keys.Any(string.IsNullOrWhiteSpace))
            throw new Error($"Managed text key can't be empty: {line}");
    }

    private void ParseCommentLine (string line, int index)
    {
        var comment = line.GetAfterFirst(RecordCommentLiteral);
        if (comment.Length > 0 && comment[0] == ' ')
            comment = comment.Substring(1);
        if (commentBuilder.Length > 0)
            commentBuilder.Append('\n');
        commentBuilder.Append(comment);
        if (index == 0) header = comment;
    }

    private void ParseValueLine (string line)
    {
        valueBuilder.Append(line);
    }

    private void AddLastRecord ()
    {
        values.Add(valueBuilder.ToString());
        comments.Add(commentBuilder.ToString());
        for (int i = 0; i < keys.Count; i++)
            records.Add(new(keys[i], values.ElementAtOrDefault(i), comments.ElementAtOrDefault(i)));
        keys.Clear();
        comments.Clear();
        commentBuilder.Clear();
        values.Clear();
        valueBuilder.Clear();
    }
}
