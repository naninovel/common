namespace Naninovel.Parsing;

public class Parameter : LineContent
{
    public LineText Identifier { get; } = new();
    public ParameterValue Value { get; } = new();
    public bool Nameless => Identifier.Empty;

    public override string ToString () => Nameless ? Value : $"{Identifier}{Identifiers.ParameterAssign}{Value}";
}
