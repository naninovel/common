namespace Naninovel.Expression;

internal class UnaryOperation (IUnaryOperator op, IExpression operand) : IExpression
{
    public IUnaryOperator Operator { get; } = op;
    public IExpression Operand { get; } = operand;
}
