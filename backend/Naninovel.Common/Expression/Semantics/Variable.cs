namespace Naninovel.Expression;

/// <summary>
/// Script variable.
/// </summary>
public class Variable (string name) : IExpression
{
    /// <summary>
    /// Identifier of the variable.
    /// </summary>
    public string Name { get; } = name;
}
