namespace Naninovel.Parsing;

/// <summary>
/// Represents a script line consisting of a single <see cref="Command"/>.
/// </summary>
public class CommandLine : IScriptLine
{
    /// <summary>
    /// The command contained in the line.
    /// </summary>
    public Command Command { get; }

    public CommandLine (Command command)
    {
        Command = command;
    }

    public override string ToString () => $"{Identifiers.CommandLine}{Command}";
}
