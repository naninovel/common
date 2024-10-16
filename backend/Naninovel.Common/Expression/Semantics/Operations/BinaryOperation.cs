namespace Naninovel.Expression;

/// <summary>
/// An operation over two operands.
/// </summary>
public class BinaryOperation (IBinaryOperator op, IExpression lhs, IExpression rhs) : IExpression
{
    /// <summary>
    /// Operator to apply over the operands.
    /// </summary>
    public IBinaryOperator Operator { get; } = op;
    /// <summary>
    /// Left-hand side operand.
    /// </summary>
    public IExpression Lhs { get; } = lhs;
    /// <summary>
    /// Right-hand side operand.
    /// </summary>
    public IExpression Rhs { get; } = rhs;
}
