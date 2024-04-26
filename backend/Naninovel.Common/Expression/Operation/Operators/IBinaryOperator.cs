namespace Naninovel.Expression;

internal interface IBinaryOperator
{
    IOperand Operate (IOperand lhs, IOperand rhs);
}
