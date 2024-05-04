namespace Naninovel.Expression;

internal class Remainder : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Numeric(ln.Value % rn.Value);
        throw new Error($"Can't divide '{rhs.GetType().Name}' by '{lhs.GetType().Name}' to get a remainder.");
    }
}
