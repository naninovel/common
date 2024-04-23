namespace Naninovel.Parsing;

/// <summary>
/// Part of <see cref="GenericLine"/> containing a <see cref="Command"/>
/// to be executed in the midst of printed text.
/// </summary>
public class InlinedCommand (Command command) : ILineComponent, IGenericContent
{
    /// <summary>
    /// The inlined command body.
    /// </summary>
    public Command Command { get; } = command;

    public override string ToString ()
    {
        return $"{Identifiers.Default.InlinedOpen}{Command}{Identifiers.Default.InlinedClose}";
    }
}
