namespace Naninovel.Parsing;

/// <summary>
/// Represents single script line.
/// Can be either <see cref="CommentLine"/>, <see cref="LabelLine"/>,
/// <see cref="CommandLine"/> or <see cref="GenericLine"/>.
/// </summary>
public interface IScriptLine
{
    /// <summary>
    /// Indent level of the line.
    /// Single indent is constituted by 4 continuous spaces, tabs don't count.
    /// </summary>
    int Indent { get; }
}
