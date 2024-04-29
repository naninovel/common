namespace Naninovel.Expression;

/// <summary>
/// Evaluates <see cref="IExpression"/> into result.
/// </summary>
public class Evaluator (EvaluateOptions options)
{
    private readonly ResolveVariable resolveVar = options.ResolveVariable;
    private readonly ResolveFunction resolveFn = options.ResolveFunction;

    /// <summary>
    /// Evaluates specified expression into result of specified type.
    /// </summary>
    /// <param name="exp">Expression to evaluate.</param>
    /// <typeparam name="TResult">Expected type of the result.</typeparam>
    /// <returns>Result of the evaluated expression.</returns>
    /// <exception cref="Error">Thrown when expression evaluation fails.</exception>
    public TResult Evaluate<TResult> (IExpression exp) => Evaluate(exp).GetValue<TResult>();

    /// <summary>
    /// Evaluates result of specified expression.
    /// </summary>
    /// <param name="exp">Expression to evaluate.</param>
    /// <returns>Result of the evaluated expression.</returns>
    /// <exception cref="Error">Thrown when expression evaluation fails.</exception>
    public IOperand Evaluate (IExpression exp) => exp switch {
        String str => str,
        Numeric num => num,
        Boolean @bool => @bool,
        Variable var => resolveVar(var.Name),
        Function fn => resolveFn(fn.Name, fn.Parameters.Select(Evaluate).ToArray()),
        UnaryOperation unary => unary.Operator.Operate(Evaluate(unary.Operand)),
        BinaryOperation binary => binary.Operator.Operate(Evaluate(binary.Lhs), Evaluate(binary.Rhs)),
        TernaryOperation ternary => Evaluate(ternary.Condition).GetValue<bool>()
            ? Evaluate(ternary.Truthy)
            : Evaluate(ternary.Falsy),
        _ => throw new Error($"Unknown expression type: {exp.GetType().Name}")
    };
}
