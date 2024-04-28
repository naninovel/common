namespace Naninovel.Expression;

internal class Divide : IAssociativeBinaryOperator
{
    public IOperand Operate (IOperand lhs, IOperand rhs)
    {
        if (lhs is Numeric ln && rhs is Numeric rn)
            return new Numeric(ln.Value / rn.Value);
        throw new Error($"Can't divide '{lhs.GetType().Name}' by '{rhs.GetType().Name}'.");
    }
}
