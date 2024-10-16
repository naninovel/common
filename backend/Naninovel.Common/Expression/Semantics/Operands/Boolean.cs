namespace Naninovel.Expression;

public class Boolean (bool value) : IOperand
{
    public bool Value { get; } = value;
    public object GetValue () => Value;
}
