namespace Naninovel.Expression;

public class String (string value) : IOperand
{
    public string Value { get; } = value;
    public object GetValue () => Value;
}
