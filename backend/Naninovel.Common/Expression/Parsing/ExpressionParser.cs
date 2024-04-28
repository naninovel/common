using System.Text;

namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class ExpressionParser
{
    private readonly ParseOptions options;
    private readonly ExpressionBuilder builder = new();
    private readonly ParseContext ctx = new();
    private readonly StringBuilder str = new();
    private readonly IdentifierParser ids;
    private readonly OperatorParser ops;

    public ExpressionParser (ParseOptions options)
    {
        this.options = options;
        ids = new IdentifierParser(ctx, options);
        ops = new OperatorParser(ctx);
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
            while (!ctx.EndReached)
                if (!ids.TryParse() && !ops.TryUnary() && !ops.TryBinary() &&
                    !TryClosure() && !TryString() && !TryNumber())
                    ctx.Move();
            exp = builder.Build(ctx.Parsed);
            return true;
        }
        catch (Error err)
        {
            exp = default!;
            options.HandleDiagnostic?.Invoke(new(0, text.Length, err.Message));
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

    private bool TryClosure ()
    {
        if (!IsOpen()) return false;

        while (!IsClose())
            str.Append(ctx.Consume());

        if (!new ExpressionParser(options).TryParse(str.ToString(), out var closure))
            throw new Error("Failed to parse closure.");

        ctx.AddParsed(closure);
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

        ctx.AddParsed(new String(str.ToString()));
        str.Clear();
        return true;
    }

    private bool TryNumber ()
    {
        while (ctx.Is(char.IsNumber))
            str.Append(ctx.Consume());

        var text = str.ToString();
        if (!double.TryParse(text, out var num))
            throw new Error($"Failed to parse '{text}' as number.");

        ctx.AddParsed(new Numeric(num));
        str.Clear();
        return true;
    }
}
