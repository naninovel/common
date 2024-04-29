namespace Naninovel.Expression;

internal class UnaryOperation (IUnaryOperator op, IExpression operand) : IExpression
{
    public IUnaryOperator BinaryOperator { get; } = op;
    public IExpression Operand { get; } = operand;
}
