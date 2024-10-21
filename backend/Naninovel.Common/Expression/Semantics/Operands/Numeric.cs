namespace Naninovel.Expression;

public class Numeric (double value) : IOperand
{
    public double Value { get; } = value;
    public object GetValue () => Value;
}
