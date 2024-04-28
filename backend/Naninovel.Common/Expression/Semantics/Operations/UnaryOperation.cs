namespace Naninovel.Expression;

internal class UnaryOperation (IExpression operand, IBinaryOperator op) : IExpression
{
    public IExpression Operand { get; } = operand;
    public IBinaryOperator BinaryOperator { get; } = op;
}
