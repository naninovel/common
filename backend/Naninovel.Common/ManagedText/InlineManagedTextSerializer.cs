using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows serializing <see cref="ManagedTextDocument"/> to text string in inline format.
/// </summary>
public class InlineManagedTextSerializer
{
    private readonly StringBuilder builder = new();
    private readonly int spacing;

    /// <param name="spacing">Line breaks between records.</param>
    public InlineManagedTextSerializer (int spacing = 1)
    {
        this.spacing = spacing;
    }

    /// <summary>
    /// Serializes specified document into text string.
    /// </summary>
    public string Serialize (ManagedTextDocument document)
    {
        builder.Clear();
        if (!string.IsNullOrEmpty(document.Header))
            AppendHeader(document.Header);
        else builder.Append('\n');
        foreach (var record in document.Records)
            AppendRecord(record);
        return builder.ToString();
    }

    private void AppendHeader (string header)
    {
        builder.Append(RecordCommentLiteral)
            .Append(header)
            .Append('\n');
    }

    private void AppendRecord (ManagedTextRecord record)
    {
        for (int i = 0; i < spacing; i++)
            builder.Append('\n');

        if (!string.IsNullOrEmpty(record.Comment))
            foreach (var line in record.Comment.SplitLines())
                builder.Append(RecordCommentLiteral)
                    .Append(line)
                    .Append('\n');

        builder.Append(record.Key)
            .Append(RecordInlineKeyLiteral)
            .Append(record.Value)
            .Append('\n');
    }
}
