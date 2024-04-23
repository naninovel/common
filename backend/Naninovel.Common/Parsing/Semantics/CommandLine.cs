namespace Naninovel.Parsing;

/// <summary>
/// Represents script line consisting of a single <see cref="Command"/>.
/// </summary>
public class CommandLine (Command command, int indent = 0) : IScriptLine
{
    /// <summary>
    /// The command body contained in the line.
    /// </summary>
    public Command Command { get; } = command;
    public int Indent { get; } = indent;

    public override string ToString () => $"{Identifiers.Default.CommandLine}{Command}";
}
