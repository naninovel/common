namespace Naninovel.Parsing;

/// <summary>
/// Represents a script line consisting of a single <see cref="Command"/>.
/// </summary>
public class CommandLine (Command command) : IScriptLine
{
    /// <summary>
    /// The command body contained in the line.
    /// </summary>
    public Command Command { get; } = command;

    public override string ToString () => $"{Identifiers.CommandLine}{Command}";
}
