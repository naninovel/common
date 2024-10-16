namespace Naninovel.Expression;

internal class Less : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Boolean(ln.Value < rn.Value);
        throw new Error($"Can't check if '{lhs.GetType().Name}' is less than '{rhs.GetType().Name}'.");
    }
}
