namespace Naninovel.Expression;

/// <summary>
/// Allows evaluating expression text into result.
/// </summary>
public class ExpressionEvaluator (EvaluateOptions options)
{
    private readonly ExpressionParser parser = new(options.ParseOptions);

    public static bool TryEvaluate<TResult> (string text, out TResult result)
    {
        result = default!;
        var obj = Evaluate(text, typeof(TResult));
        if (obj is not TResult specific) return false;
        result = specific;
        return true;
    }

    public static object Evaluate (string text, Type resultType)
    {
        throw new ExpressionError("");
    }
}
