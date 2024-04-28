namespace Naninovel.Expression;

internal class Numeric (double value) : IOperand
{
    public double Value { get; } = value;
}
