namespace Naninovel.Expression;

internal class Equal : IBinaryOperator
{
    private const double tolerance = 0.00001;

    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Boolean lb && rhs is Boolean rb)
            return new Boolean(lb.Value == rb.Value);
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Boolean(Math.Abs(ln.Value - rn.Value) < tolerance);
        if (lhs is String ls && rhs is String rs)
            return new Boolean(ls.Value == rs.Value);
        throw new Error($"Can't check equality of '{lhs.GetType().Name}' and '{rhs.GetType().Name}'.");
    }
}