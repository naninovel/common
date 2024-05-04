namespace Naninovel.Expression;

internal class NegateBoolean : IUnaryOperator
{
    public IOperand Operate (IOperand operand)
    {
        if (operand is Boolean b)
            return new Boolean(!b.Value);
        throw new Error($"Can't perform boolean negation of '{operand.GetType().Name}'.");
    }
}
