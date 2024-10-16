namespace Naninovel.Expression;

internal class GreaterOrEqual : IBinaryOperator
{
    private readonly Equal eq = new();

    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Boolean(ln.Value > rn.Value || ((Boolean)eq.Operate(lhs, rhs)).Value);
        throw new Error($"Can't check if '{lhs.GetType().Name}' is greater or equal to '{rhs.GetType().Name}'.");
    }
}
