namespace Naninovel.Expression;

/// <summary>
/// An assignment statement.
/// </summary>
public readonly struct Assignment (string variable, IExpression expression)
{
    /// <summary>
    /// Identifier (name) of the variable being assigned.
    /// </summary>
    public readonly string Variable = variable;
    /// <summary>
    /// Expression assigned to <see cref="Variable"/>.
    /// </summary>
    public readonly IExpression Expression = expression;
}
