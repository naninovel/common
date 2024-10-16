namespace Naninovel.Expression;

/// <summary>
/// A range inside expression string associated with an expression morpheme.
/// </summary>
public readonly struct ExpressionRange (IExpression exp, int index, int length)
{
    /// <summary>
    /// The expression morpheme this range is associated with.
    /// </summary>
    public readonly IExpression Expression = exp;
    /// <summary>
    /// Position of the first character in the string, 0-based.
    /// </summary>
    public readonly int Index = index;
    /// <summary>
    /// Length (in characters) of the range.
    /// </summary>
    public readonly int Length = length;
}
