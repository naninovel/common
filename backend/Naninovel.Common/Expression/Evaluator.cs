namespace Naninovel.Expression;

/// <summary>
/// Evaluates <see cref="IExpression"/> into result.
/// </summary>
public class Evaluator
{
    /// <summary>
    /// Attempts to evaluate result of the specified expression and result type.
    /// </summary>
    /// <param name="exp">Expression to evaluate.</param>
    /// <param name="result">Result of the evaluated expression, when successful.</param>
    /// <typeparam name="TResult">Expected type of the result.</typeparam>
    /// <returns>Whether expression was evaluated successfully and result is of the specified type.</returns>
    public bool TryEvaluate<TResult> (IExpression exp, out TResult result)
    {
        result = default!;
        if (!TryEvaluate(exp, typeof(TResult), out var obj)) return false;
        result = (TResult)obj;
        return true;
    }

    /// <inheritdoc cref="TryEvaluate{TResult}"/>
    /// <param name="resultType">Expected type of the result.</param>
    public bool TryEvaluate (IExpression exp, Type resultType, out object result)
    {
        throw new NotImplementedException();
    }
}
