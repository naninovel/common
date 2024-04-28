namespace Naninovel.Expression;

internal class Greater : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Boolean(ln.Value > rn.Value);
        throw new Error($"Can't check if '{lhs.GetType().Name}' is greater than '{rhs.GetType().Name}'.");
    }
}
