using System.Text;

namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class ExpressionParser
{
    private readonly ParseOptions options;
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
        exp = default!;

        try
        {
            while (!ctx.EndReached)
            {
                if (ops.TryUnary() is { } unary)
                    exp = ConsumeUnary(unary);
                if (exp != null && ops.TryBinary() is { } binary)
                    exp = ConsumeBinary(exp, binary);
                ctx.Move();
            }

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

    private UnaryOperation ConsumeUnary (IUnaryOperator op)
    {

    }

    private UnaryOperation ConsumeBinary (IExpression lhs, IBinaryOperator op)
    {

    }

    private IExpression? TryClosure ()
    {
        if (!IsOpen()) return null;

        while (!IsClose())
            str.Append(ctx.Consume());
        if (!new ExpressionParser(options).TryParse(str.ToString(), out var closure))
            throw new Error("Failed to parse closure.");

        str.Clear();
        return closure;

        bool IsOpen () => ctx.Is('(') && ctx.IsTopAndUnquoted;
        bool IsClose () => ctx.Is(')') && ctx.IsTopAndUnquoted;
    }

    private IExpression? TryString ()
    {
        if (!ctx.IsQuoted) return null;

        while (ctx.IsQuoted)
            str.Append(ctx.Consume());

        str.Clear();
        return new String(str.ToString());
    }
}
