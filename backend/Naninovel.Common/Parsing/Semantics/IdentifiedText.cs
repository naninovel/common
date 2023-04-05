namespace Naninovel.Parsing;

/// <summary>
/// A plain text content with associated persistent identifier.
/// </summary>
public class IdentifiedText : ILineComponent, IValueComponent
{
    /// <summary>
    /// The identified text content.
    /// </summary>
    public PlainText Text { get; }
    /// <summary>
    /// The identifier of the text content.
    /// </summary>
    public TextIdentifier Id { get; }

    public IdentifiedText (PlainText text, TextIdentifier id)
    {
        Id = id;
        Text = text;
    }

    public override string ToString () => Text + Id;
}
