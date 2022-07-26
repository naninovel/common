namespace Naninovel.Parsing;

/// <summary>
/// Part of <see cref="GenericLine"/> containing a <see cref="Command"/>
/// to be executed in the midst of printed text.
/// </summary>
public class InlinedCommand : IGenericContent
{
    /// <summary>
    /// The inlined command.
    /// </summary>
    public Command Command { get; }

    public InlinedCommand (Command command)
    {
        Command = command;
    }

    public override string ToString ()
    {
        return $"{Identifiers.InlinedOpen}{Command}{Identifiers.InlinedClose}";
    }
}
