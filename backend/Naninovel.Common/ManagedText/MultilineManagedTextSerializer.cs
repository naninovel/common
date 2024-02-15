using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows serializing <see cref="ManagedTextDocument"/> to text string in multiline format.
/// </summary>
public class MultilineManagedTextSerializer
{
    private readonly ManagedTextRecord[] singleRecord = new ManagedTextRecord[1];
    private readonly StringBuilder builder = new();
    private readonly HashSet<object> buckets = [];
    private readonly Dictionary<string, List<ManagedTextRecord>> keyToBuckets = [];
    private readonly int spacing;

    /// <param name="spacing">Line breaks between records.</param>
    public MultilineManagedTextSerializer (int spacing = 1)
    {
        this.spacing = spacing;
    }

    /// <summary>
    /// Serializes specified document into text string.
    /// When join keys specified, will join associated records into single lines.
    /// </summary>
    public string Serialize (ManagedTextDocument document, IEnumerable<IEnumerable<string>>? join = null)
    {
        Reset();
        BuildBuckets(document.Records, join ?? []);
        if (!string.IsNullOrEmpty(document.Header))
            AppendHeader(document.Header);
        foreach (var bucket in buckets)
            AppendBucket(bucket);
        return builder.ToString();
    }

    private void Reset ()
    {
        builder.Clear();
        buckets.Clear();
        keyToBuckets.Clear();
    }

    private void BuildBuckets (IReadOnlyCollection<ManagedTextRecord> records, IEnumerable<IEnumerable<string>> join)
    {
        foreach (var joinedKeys in join)
        {
            var bucket = new List<ManagedTextRecord>();
            foreach (var joinedKey in joinedKeys)
                keyToBuckets[joinedKey] = bucket;
        }

        foreach (var record in records)
            if (keyToBuckets.TryGetValue(record.Key, out var bucket))
            {
                bucket.Add(record);
                buckets.Add(bucket);
            }
            else buckets.Add(record);
    }

    private void AppendHeader (string header)
    {
        builder.Append(RecordCommentLiteral)
            .Append(' ')
            .Append(header.Trim())
            .Append('\n');
    }

    private void AppendBucket (object bucket)
    {
        IReadOnlyList<ManagedTextRecord> records = null!;
        if (bucket is ManagedTextRecord record)
        {
            singleRecord[0] = record;
            records = singleRecord;
        }
        else records = (List<ManagedTextRecord>)bucket;

        for (int i = 0; i < spacing; i++)
            builder.Append('\n');
        AppendKey();
        if (records.Any(r => !string.IsNullOrWhiteSpace(r.Comment)))
            AppendComment();
        AppendValue();

        void AppendKey ()
        {
            builder.Append(RecordMultilineKeyLiteral).Append(' ');
            for (var i = 0; i < records.Count; i++)
            {
                if (ShouldAppendJoin(i))
                    builder.Append(RecordJoinLiteral);
                builder.Append(records[i].Key);
            }
            builder.Append('\n');
        }

        void AppendComment ()
        {
            builder.Append(RecordCommentLiteral).Append(' ');
            for (var i = 0; i < records.Count; i++)
            {
                if (ShouldAppendJoin(i))
                    builder.Append(RecordJoinLiteral);
                builder.Append(EscapeJoinLiteral(records[i].Comment));
            }
            builder.Append('\n');
        }

        void AppendValue ()
        {
            for (var i = 0; i < records.Count; i++)
            {
                if (ShouldAppendJoin(i))
                    builder.Append(RecordJoinLiteral);
                builder.Append(EscapeJoinLiteral(InsertLineBreaksAfterBrTags(records[i].Value)));
            }
            builder.Append('\n');
        }

        bool ShouldAppendJoin (int index)
        {
            return index > 0 && index < records.Count;
        }
    }

    private string InsertLineBreaksAfterBrTags (string value)
    {
        return value.Replace(LineBreakTag, LineBreakTag + '\n');
    }

    private string EscapeJoinLiteral (string content)
    {
        return content.Replace(RecordJoinLiteral, $"\\{RecordJoinLiteral}");
    }
}
