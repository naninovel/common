using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows serializing <see cref="ManagedTextDocument"/> to text string in multiline format.
/// </summary>
public class MultilineManagedTextSerializer
{
    private readonly StringBuilder builder = new();
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
    public string Serialize (ManagedTextDocument document)
    {
        Reset();
        if (!string.IsNullOrEmpty(document.Header))
            AppendHeader(document.Header);
        foreach (var record in document.Records)
            AppendRecord(record);
        return builder.ToString();
    }

    private void Reset ()
    {
        builder.Clear();
    }

    private void AppendHeader (string header)
    {
        builder.Append(RecordCommentLiteral)
            .Append(' ')
            .Append(header.Trim())
            .Append('\n');
    }

    private void AppendRecord (ManagedTextRecord record)
    {
        AppendSpace();
        AppendKey();
        foreach (var line in record.Comment.SplitLines())
            if (!string.IsNullOrWhiteSpace(line))
                AppendCommentLine(line);
        AppendValue();

        void AppendSpace ()
        {
            for (int i = 0; i < spacing; i++)
                builder.Append('\n');
        }

        void AppendKey ()
        {
            builder.Append(RecordMultilineKeyLiteral).Append(' ');
            builder.Append(record.Key);
            builder.Append('\n');
        }

        void AppendCommentLine (string line)
        {
            builder.Append(RecordCommentLiteral).Append(' ');
            builder.Append(line);
            builder.Append('\n');
        }

        void AppendValue ()
        {
            builder.Append(InsertLineBreaksAfterBrTags(record.Value));
            builder.Append('\n');
        }
    }

    private string InsertLineBreaksAfterBrTags (string value)
    {
        return value.Replace(LineBreakTag, LineBreakTag + '\n');
    }
}
