namespace Naninovel.Expression;

/// <summary>
/// Allows evaluating expression text into result.
/// </summary>
public class ExpressionEvaluator (EvaluateOptions options)
{
    private readonly ExpressionParser parser = new(options.ParseOptions);

    public static bool TryEvaluate<TResult> (Expression expression, out TResult result)
    {
        result = default!;
        try { result = (TResult)Evaluate(expression, typeof(TResult)); }
        catch { return false; }
        return true;
    }

    public static object Evaluate (Expression expression, Type resultType)
    {
        throw new ExpressionError("");
    }
}
