namespace Naninovel.Parsing;

/// <summary>
/// Text interpreted as-is at runtime.
/// </summary>
/// <remarks>
/// The type is used to distinct text parts of composite values
/// from dynamically-evaluated <see cref="Expression"/>.
/// </remarks>
public class PlainText : IMixedValue
{
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
