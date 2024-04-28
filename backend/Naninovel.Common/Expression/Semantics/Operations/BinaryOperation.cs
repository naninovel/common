namespace Naninovel.Expression;

internal class BinaryOperation (IExpression lhs, IExpression rhs, IBinaryOperator op) : IExpression
{
    /// <summary>
    /// Left-hand side operand.
    /// </summary>
    public IExpression Lhs { get; } = lhs;
    /// <summary>
    /// Right-hand side operand.
    /// </summary>
    public IExpression Rhs { get; } = rhs;
    /// <summary>
    /// Operator to apply over the operands.
    /// </summary>
    public IBinaryOperator BinaryOperator { get; } = op;
}
