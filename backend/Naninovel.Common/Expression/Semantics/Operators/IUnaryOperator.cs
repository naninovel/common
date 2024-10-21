namespace Naninovel.Expression;

public interface IUnaryOperator
{
    IOperand Operate (IOperand operand);
}
