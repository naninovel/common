namespace Naninovel.Expression;

/// <summary>
/// An assignment statement that was evaluated.
/// </summary>
public readonly struct EvaluatedAssignment (string variable, IOperand result)
{
    /// <summary>
    /// Identifier (name) of the variable being assigned.
    /// </summary>
    public readonly string Variable = variable;
    /// <summary>
    /// Evaluation result assigned to <see cref="Variable"/>.
    /// </summary>
    public readonly IOperand Result = result;
}
