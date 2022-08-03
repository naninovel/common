namespace Naninovel.Parsing;

/// <summary>
/// Represents a script line used for annotations; ignored at playback.
/// </summary>
public class CommentLine : IScriptLine
{
    /// <summary>
    /// The annotation text.
    /// </summary>
    public PlainText Comment { get; }

    public CommentLine (PlainText comment)
    {
        Comment = comment;
    }

    public override string ToString () => $"{Identifiers.CommentLine} {Comment}";
}
