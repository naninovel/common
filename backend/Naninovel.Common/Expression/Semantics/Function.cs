namespace Naninovel.Expression;

/// <summary>
/// Expression function.
/// </summary>
public class Function (string name, IReadOnlyList<IExpression> @params) : IExpression
{
    /// <summary>
    /// Identifier of the function.
    /// </summary>
    public string Name { get; } = name;
    /// <summary>
    /// Argument values of the function, in signature order.
    /// </summary>
    public IReadOnlyList<IExpression> Parameters { get; } = @params;
}
