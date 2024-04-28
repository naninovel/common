namespace Naninovel.Expression;

internal class Boolean (bool value) : IOperand
{
    public bool Value { get; } = value;
}
