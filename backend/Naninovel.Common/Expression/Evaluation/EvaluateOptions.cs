namespace Naninovel.Expression;

/// <summary>
/// Configures <see cref="ExpressionEvaluator"/> instance.
/// </summary>
public class EvaluateOptions
{
    public ParseOptions ParseOptions { get; set; } = new();
}
