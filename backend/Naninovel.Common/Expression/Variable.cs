namespace Naninovel.Expression;

internal class Variable (string name) : IExpression
{
    /// <summary>
    /// Identifier of the variable.
    /// </summary>
    public string Name { get; } = name;
}
