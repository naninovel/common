namespace Naninovel.Expression;

public interface IBinaryOperator
{
    IOperand Operate (IOperand lhs, IOperand rhs);
}
