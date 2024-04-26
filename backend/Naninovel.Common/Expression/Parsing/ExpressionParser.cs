namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class ExpressionParser (ParseOptions options)
{
    private readonly OperatorParser operatorParser = new();

    /// <summary>
    /// Attempts to parse specified text as expression.
    /// </summary>
    /// <param name="text">Expression text to parse.</param>
    /// <param name="exp">Parsed expression, when successful.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParse (string text, out IExpression exp)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Attempts to parse specified text as expression assigned to a variable.
    /// </summary>
    /// <param name="text">Expression assignment text to parse.</param>
    /// <param name="exp">Assigned expression, when successful.</param>
    /// <param name="var">Variable name to assign, when successful.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParseAssigned (string text, out IExpression exp, out string var)
    {
        throw new NotImplementedException();
    }
}
