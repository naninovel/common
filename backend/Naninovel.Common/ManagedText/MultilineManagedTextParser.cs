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
/// # key1 (space around key is ignored)
/// ; comment (optional, space around comment is ignored)
/// value (all lines until next key are joined, space is preserved)
/// </code>
/// Multiple records can be joined into single line with pipes:
/// <code>
/// # key1|key2|key3
/// ; comment1|comment2|co\|ent3 (pipes in comments are escaped)
/// value1|value2|va\|ue3 (pipes in values are escaped)
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
        Split(line.GetAfterFirst(RecordMultilineKeyLiteral).Trim(), keys);
        if (keys.Any(string.IsNullOrWhiteSpace))
            throw new Error($"Managed text key can't be empty: {line}");
    }

    private void ParseCommentLine (string line, int index)
    {
        var comment = line.GetAfterFirst(RecordCommentLiteral);
        if (comment.Length > 0 && comment[0] == ' ')
            comment = comment.Substring(1);
        commentBuilder.Append(comment);
        if (index == 0) header = comment;
    }

    private void ParseValueLine (string line)
    {
        valueBuilder.Append(line);
    }

    private void AddLastRecord ()
    {
        Split(valueBuilder.ToString(), values);
        if (values.Count > keys.Count)
            throw new Error($"Managed text has more values than keys. Last key: {keys.Last()}");
        Split(commentBuilder.ToString(), comments);
        if (comments.Count > keys.Count)
            throw new Error($"Managed text has more comments than keys. Last key: {keys.Last()}");
        for (int i = 0; i < keys.Count; i++)
            records.Add(new(keys[i], values.ElementAtOrDefault(i), comments.ElementAtOrDefault(i)));
        keys.Clear();
        comments.Clear();
        commentBuilder.Clear();
        values.Clear();
        valueBuilder.Clear();
    }

    private void Split (string line, IList<string> collection)
    {
        var startIdx = 0;
        for (int i = 0; i < line.Length; i++)
            if (line[i] == RecordJoinLiteral[0] && line.ElementAtOrDefault(i - 1) != '\\')
            {
                collection.Add(line.Substring(startIdx, i - startIdx)
                    .Replace($"\\{RecordJoinLiteral}", RecordJoinLiteral));
                startIdx = i + 1;
            }
        collection.Add(line.Substring(startIdx, line.Length - startIdx));
    }
}
