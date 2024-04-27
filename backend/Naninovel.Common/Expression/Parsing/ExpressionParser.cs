namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class ExpressionParser
{
    private readonly ParseContext ctx = new();
    private readonly OperatorParser ops = new();
    private readonly IdentifierParser ids;

    public ExpressionParser (ParseOptions options)
    {
        ids = new IdentifierParser(ctx, options.Identifiers);
    }

    /// <summary>
    /// Attempts to parse specified text as expression.
    /// </summary>
    /// <param name="text">Expression text to parse.</param>
    /// <param name="exp">Parsed expression, when successful.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParse (string text, out IExpression exp)
    {
        Reset(text);

        while (!ctx.EndReached)
            if (!ids.TryParse())
                ctx.Move();

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

    private void Reset (string text)
    {
        ctx.Reset(text);
    }
}
