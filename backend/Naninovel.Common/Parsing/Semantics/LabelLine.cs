namespace Naninovel.Parsing;

/// <summary>
/// Represents script line used to identify playback navigation endpoint.
/// </summary>
public class LabelLine (PlainText label, int indent = 0) : IScriptLine
{
    /// <summary>
    /// The identifier of the label.
    /// </summary>
    public PlainText Label { get; } = label;
    public int Indent { get; } = indent;

    public override string ToString () => $"{Syntax.Default.LabelLine} {Label}";
}
