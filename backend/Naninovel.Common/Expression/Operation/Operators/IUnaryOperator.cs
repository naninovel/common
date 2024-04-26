namespace Naninovel.Expression;

internal interface IUnaryOperator
{
    IOperand Operate (IOperand operand);
}
