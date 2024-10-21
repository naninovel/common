namespace Naninovel.Expression;

internal class Power : IBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Numeric(Math.Pow(ln.Value, rn.Value));
        throw new Error($"Can't raise '{lhs.GetType().Name}' to the power of '{rhs.GetType().Name}'.");
    }
}
