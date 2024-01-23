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
    /// </summary>
    public string Serialize (ManagedTextDocument document)
    {
        builder.Clear();
        if (!string.IsNullOrEmpty(document.Header))
            AppendHeader(document.Header);
        foreach (var record in document.Records)
            AppendRecord(record);
        return builder.ToString();
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
        for (int i = 0; i < spacing; i++)
            builder.Append('\n');

        builder.Append(RecordMultilineKeyLiteral)
            .Append(' ')
            .Append(record.Key)
            .Append('\n');

        if (!string.IsNullOrWhiteSpace(record.Comment))
            builder.Append(RecordCommentLiteral)
                .Append(' ')
                .Append(record.Comment)
                .Append('\n');

        var value = InsertLineBreaksAfterBrTags(record.Value);
        builder.Append(value).Append('\n');
    }

    private string InsertLineBreaksAfterBrTags (string value)
    {
        return value.Replace(LineBreakTag, LineBreakTag + '\n');
    }
}
