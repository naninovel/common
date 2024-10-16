namespace Naninovel.Expression;

/// <summary>
/// An operation over single operand.
/// </summary>
public class UnaryOperation (IUnaryOperator op, IExpression operand) : IExpression
{
    public IUnaryOperator Operator { get; } = op;
    public IExpression Operand { get; } = operand;
}
