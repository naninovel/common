namespace Naninovel.Expression;

internal class AddUnary : IUnaryOperator
{
    public IOperand Operate (IOperand operand)
    {
        if (operand is Numeric lo)
            return new Numeric(lo.Value);
        throw new Error($"Can't unary-add '{operand.GetType().Name}'.");
    }
}
