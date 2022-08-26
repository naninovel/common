namespace Naninovel.Parsing;

/// <summary>
/// Extensions for lexing and parsing types.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Resolves <see cref="LineType"/> of the specified parsed line instance.
    /// </summary>
    public static LineType GetLineType (this IScriptLine line) => line switch {
        CommentLine => LineType.Comment,
        LabelLine => LineType.Label,
        CommandLine => LineType.Command,
        GenericLine => LineType.Generic,
        _ => throw new Error($"Unknown line type: {line}")
    };
}
