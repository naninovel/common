namespace Naninovel.Expression;

internal class NegateNumeric : IUnaryOperator
{
    public IOperand Operate (IOperand operand)
    {
        if (operand is Numeric n)
            return new Numeric(-n.Value);
        throw new Error($"Can't perform numeric negation of '{operand.GetType().Name}'.");
    }
}
