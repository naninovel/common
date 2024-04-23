using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Persistent identifier component of <see cref="IdentifiedText"/>.
/// </summary>
public class TextIdentifier (PlainText body) : ILineComponent
{
    /// <summary>
    /// Value of the identifier.
    /// </summary>
    public PlainText Body { get; } = body;

    public override string ToString ()
    {
        var builder = new StringBuilder(Identifiers.Default.TextIdOpen);
        builder.Append(Body);
        builder.Append(Identifiers.Default.TextIdClose);
        return builder.ToString();
    }
}
