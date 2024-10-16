namespace Naninovel.Parsing;

/// <summary>
/// A plain text content with associated persistent identifier.
/// </summary>
public class IdentifiedText (PlainText text, TextIdentifier id) : IValueComponent
{
    /// <summary>
    /// The identified text content.
    /// </summary>
    public PlainText Text { get; } = text;
    /// <summary>
    /// The identifier of the text content.
    /// </summary>
    public TextIdentifier Id { get; } = id;

    public override string ToString () => Text + Id;
}
