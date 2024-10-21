namespace Naninovel.Expression;

/// <summary>
/// Configures <see cref="Evaluator"/> instance.
/// </summary>
public class EvaluateOptions
{
    /// <summary>
    /// Handler for variable references in the evaluated expressions.
    /// </summary>
    public ResolveVariable ResolveVariable { get; set; } = null!;
    /// <summary>
    /// Handler for function references in the evaluated expressions.
    /// </summary>
    public ResolveFunction ResolveFunction { get; set; } = null!;
}

// TODO: Use init props when Unity upgraded .NET runtime.
