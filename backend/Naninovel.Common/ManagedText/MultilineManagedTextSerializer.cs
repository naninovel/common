using System;
using System.Text;
using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows serializing <see cref="ManagedTextDocument"/> to text string in multiline format.
/// </summary>
public class MultilineManagedTextSerializer
{
    private static readonly string[] breaks = { MultilineValueLineBreak };
    private readonly StringBuilder builder = new();
    private readonly int indent;

    /// <param name="indent">Line breaks between records.</param>
    public MultilineManagedTextSerializer (int indent = 1)
    {
        this.indent = indent;
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
        for (int i = 0; i < indent; i++)
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

        foreach (var value in record.Value.Split(breaks, StringSplitOptions.None))
            builder.Append(value).Append('\n');
    }
}
