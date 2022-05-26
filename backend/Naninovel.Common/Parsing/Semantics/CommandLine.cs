namespace Naninovel.Parsing;

public class CommandLine : LineContent, IScriptLine
{
    public Command Command { get; } = new();

    public override string ToString () => $"{Identifiers.CommandLine}{Command}";
}
