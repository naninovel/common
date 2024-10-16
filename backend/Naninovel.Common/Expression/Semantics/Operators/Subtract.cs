namespace Naninovel.Expression;

internal class Subtract : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Numeric(ln.Value - rn.Value);
        throw new Error($"Can't subtract '{rhs.GetType().Name}' from '{lhs.GetType().Name}'.");
    }
}
