namespace Naninovel.Parsing;

public class CommentLine : LineContent, IScriptLine
{
    public LineText CommentText { get; } = new();

    public override string ToString () => $"{Identifiers.CommentLine} {CommentText}";
}
