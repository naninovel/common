namespace Naninovel.Parsing;

/// <summary>
/// Represents plain text content.
/// </summary>
public class PlainText : ILineComponent, IValueComponent
{
    public static readonly PlainText Empty = new("");

    /// <summary>
    /// The text content.
    /// </summary>
    public string Text { get; }

    public PlainText (string text)
    {
        Text = text;
    }

    public override string ToString () => Text;
}
