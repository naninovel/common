namespace Naninovel.Expression;

internal class And : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Boolean lb && rhs is Boolean rb)
            return new Boolean(lb.Value && rb.Value);
        throw new Error($"Can't perform boolean conjunction of '{lhs.GetType().Name}' and '{rhs.GetType().Name}'.");
    }
}
