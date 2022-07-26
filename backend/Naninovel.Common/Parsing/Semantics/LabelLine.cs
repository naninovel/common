namespace Naninovel.Parsing;

/// <summary>
/// Represents a script line used to identify playback navigation point.
/// </summary>
public class LabelLine : IScriptLine
{
    /// <summary>
    /// The identifier of the label.
    /// </summary>
    public string Label { get; }

    public LabelLine (string label)
    {
        Label = label;
    }

    public override string ToString () => $"{Identifiers.LabelLine} {Label}";
}
