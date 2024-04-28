namespace Naninovel.Expression;

internal class String (string value) : IOperand
{
    public string Value { get; } = value;
}
