namespace Naninovel.Parsing;

/// <summary>
/// Represents script line used for annotations; ignored at playback.
/// </summary>
public class CommentLine (PlainText comment, int indent = 0) : IScriptLine
{
    /// <summary>
    /// The annotation text.
    /// </summary>
    public PlainText Comment { get; } = comment;
    public int Indent { get; } = indent;

    public override string ToString () => $"{Syntax.Default.CommentLine} {Comment}";
}
