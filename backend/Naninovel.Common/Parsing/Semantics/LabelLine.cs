namespace Naninovel.Parsing;

public class LabelLine : LineContent, IScriptLine
{
    public LineText LabelText { get; } = new();

    public override string ToString () => $"{Identifiers.LabelLine} {LabelText}";
}
