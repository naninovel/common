using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows serializing <see cref="ManagedTextDocument"/> to text string in multiline format.
/// </summary>
public class MultilineManagedTextSerializer
{
    /// <summary>
    /// Record index ranges to join into single line when serializing.
    /// </summary>
    /// <param name="startIndex">First record index to merge.</param>
    /// <param name="endIndex">Last record index to merge (inclusive).</param>
    public readonly struct JoinRange (int startIndex, int endIndex)
    {
        public readonly int StartIndex = startIndex;
        public readonly int EndIndex = endIndex;
    }

    private readonly StringBuilder builder = new();
    private readonly List<JoinRange> ranges = [];
    private readonly List<ManagedTextRecord> bucket = [];
    private readonly int spacing;

    /// <param name="spacing">Line breaks between records.</param>
    public MultilineManagedTextSerializer (int spacing = 1)
    {
        this.spacing = spacing;
    }

    /// <summary>
    /// Serializes specified document into text string.
    /// When join indexes specified, will serialize affected records into single line.
    /// </summary>
    public string Serialize (ManagedTextDocument document, IEnumerable<JoinRange>? join = null)
    {
        Reset(join);
        if (!string.IsNullOrEmpty(document.Header))
            AppendHeader(document.Header);
        AppendRecords(document.Records);
        return builder.ToString();
    }

    private void Reset (IEnumerable<JoinRange>? ranges = null)
    {
        builder.Clear();
        bucket.Clear();
        this.ranges.Clear();
        if (ranges != null) this.ranges.AddRange(ranges.OrderBy(r => r.StartIndex));
    }

    private void AppendHeader (string header)
    {
        builder.Append(RecordCommentLiteral)
            .Append(' ')
            .Append(header.Trim())
            .Append('\n');
    }

    private void AppendRecords (IReadOnlyCollection<ManagedTextRecord> records)
    {
        var index = -1;
        foreach (var record in records)
        {
            bucket.Add(record);
            if (ShouldAppendBucket(++index))
                AppendBucket();
        }
    }

    private void AppendBucket ()
    {
        for (int i = 0; i < spacing; i++)
            builder.Append('\n');
        AppendKey();
        if (bucket.Any(r => !string.IsNullOrWhiteSpace(r.Comment)))
            AppendComment();
        AppendValue();
        bucket.Clear();
    }

    private void AppendKey ()
    {
        builder.Append(RecordMultilineKeyLiteral).Append(' ');
        for (var i = 0; i < bucket.Count; i++)
        {
            if (ShouldAppendJoin(i))
                builder.Append(RecordJoinLiteral);
            builder.Append(bucket[i].Key);
        }
        builder.Append('\n');
    }

    private void AppendComment ()
    {
        builder.Append(RecordCommentLiteral).Append(' ');
        for (var i = 0; i < bucket.Count; i++)
        {
            if (ShouldAppendJoin(i))
                builder.Append(RecordJoinLiteral);
            builder.Append(EscapeJoinLiteral(bucket[i].Comment));
        }
        builder.Append('\n');
    }

    private void AppendValue ()
    {
        for (var i = 0; i < bucket.Count; i++)
        {
            if (ShouldAppendJoin(i))
                builder.Append(RecordJoinLiteral);
            builder.Append(EscapeJoinLiteral(InsertLineBreaksAfterBrTags(bucket[i].Value)));
        }
        builder.Append('\n');
    }

    private string InsertLineBreaksAfterBrTags (string value)
    {
        return value.Replace(LineBreakTag, LineBreakTag + '\n');
    }

    private bool ShouldAppendBucket (int index)
    {
        var insideAny = false;
        foreach (var range in ranges)
        {
            if (range.EndIndex == index)
                return true;
            if (!insideAny && index >= range.StartIndex && index <= range.EndIndex)
                insideAny = true;
        }
        return !insideAny;
    }

    private bool ShouldAppendJoin (int index)
    {
        return index > 0 && index < bucket.Count;
    }

    private string EscapeJoinLiteral (string content)
    {
        return content.Replace(RecordJoinLiteral, $"\\{RecordJoinLiteral}");
    }
}
