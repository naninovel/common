using System.Text;

namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class ExpressionParser
{
    private readonly ParseOptions options;
    private readonly ParseContext ctx = new();
    private readonly OperatorParser ops = new();
    private readonly StringBuilder str = new();
    private readonly IdentifierParser ids;

    public ExpressionParser (ParseOptions options)
    {
        this.options = options;
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

        try
        {
            exp = ParseNext();
            return true;
        }
        catch (Error err)
        {
            exp = default!;
            var length = text.Length - ctx.Index;
            options.HandleDiagnostic?.Invoke(new(ctx.Index, length, err.Message));
            return false;
        }
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
        str.Clear();
    }

    private IExpression ParseNext ()
    {
        while (!ctx.EndReached)
            if (!ids.TryParse() && !TryClosure() && !TryString())
                ctx.Move();
    }

    private bool TryClosure ()
    {
        if (!IsOpen()) return false;

        while (!IsClose())
            str.Append(ctx.Consume());
        var closure = str.ToString();

        str.Clear();
        return true;

        bool IsOpen () => ctx.Is('(') && ctx.IsTopAndUnquoted;
        bool IsClose () => ctx.Is(')') && ctx.IsTopAndUnquoted;
    }

    private bool TryString ()
    {
        if (!ctx.IsQuoted) return false;

        while (ctx.IsQuoted)
            str.Append(ctx.Consume());
        var @string = new String(str.ToString());

        str.Clear();
        return true;
    }
}
