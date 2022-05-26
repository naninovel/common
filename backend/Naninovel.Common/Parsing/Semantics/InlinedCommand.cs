namespace Naninovel.Parsing;

public class InlinedCommand : LineContent, IGenericContent
{
    public Command Command { get; } = new();

    public override string ToString () => $"[{Command}]";
}
