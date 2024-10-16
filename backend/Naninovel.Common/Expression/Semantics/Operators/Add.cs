namespace Naninovel.Expression;

internal class Add : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Numeric(ln.Value + rn.Value);
        if (lhs is String ls && rhs is String rs)
            return new String(ls.Value + rs.Value);
        throw new Error($"Can't add '{rhs.GetType().Name}' to '{lhs.GetType().Name}'.");
    }
}
