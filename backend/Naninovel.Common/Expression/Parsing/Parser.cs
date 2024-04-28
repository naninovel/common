namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class Parser (ParseOptions options)
{
    private readonly Lexer lexer = new();

    /// <summary>
    /// Attempts to parse specified text as expression.
    /// </summary>
    /// <param name="text">Expression text to parse.</param>
    /// <param name="exp">Parsed expression, when successful.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParse (string text, out IExpression exp)
    {
        try
        {
            var tokens = lexer.Lex(text);
            exp = null!;
            return true;
        }
        catch (Error err)
        {
            exp = null!;
            var index = err.Index ?? 0;
            var length = err.Length ?? text.Length - index;
            options.HandleDiagnostic?.Invoke(new(index, length, err.Message));
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse specified text as expression assigned to a variable
    /// or multiple such statements separated with ';'.
    /// </summary>
    /// <param name="text">Assignment statement(s) text to parse.</param>
    /// <param name="assignments">Parsed statements.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParseAssignments (string text, out IReadOnlyList<Assignment> assignments)
    {
        assignments = [];
        return false;
    }
}
