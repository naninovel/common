namespace Naninovel.Expression;

/// <summary>
/// Boolean ternary operation.
/// </summary>
public class TernaryOperation (IExpression condition, IExpression truthy, IExpression falsy) : IExpression
{
    /// <summary>
    /// Boolean expression determining whether <see cref="Truthy"/>
    /// or <see cref="Falsy"/> expression should be evaluated.
    /// </summary>
    public IExpression Condition { get; } = condition;
    /// <summary>
    /// Evaluated when <see cref="Condition"/> is truthy.
    /// </summary>
    public IExpression Truthy { get; } = truthy;
    /// <summary>
    /// Evaluated when <see cref="Condition"/> is falsy.
    /// </summary>
    public IExpression Falsy { get; } = falsy;
}
