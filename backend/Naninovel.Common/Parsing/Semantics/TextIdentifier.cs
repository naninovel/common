using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Persistent identifier component of <see cref="IdentifiedText"/>.
/// </summary>
public class TextIdentifier : ILineComponent
{
    /// <summary>
    /// Value of the identifier.
    /// </summary>
    public PlainText Body { get; }

    public TextIdentifier (PlainText body)
    {
        Body = body;
    }

    public override string ToString ()
    {
        var builder = new StringBuilder(Identifiers.TextIdDelimiter);
        builder.Append(Body);
        builder.Append(Identifiers.TextIdDelimiter);
        return builder.ToString();
    }
}
