namespace Naninovel.Expression;

/// <summary>
/// A malformed or otherwise invalid part of expression.
/// </summary>
public class Invalid (string message) : IExpression
{
    /// <summary>
    /// Message associated with the error.
    /// </summary>
    public string Message { get; } = message;
}
