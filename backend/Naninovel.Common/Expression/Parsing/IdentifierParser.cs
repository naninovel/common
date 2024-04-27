using System.Text;
using Naninovel.Parsing;

namespace Naninovel.Expression;

internal class IdentifierParser (ParseContext ctx, ParseOptions options)
{
    private readonly StringBuilder str = new();
    private readonly Identifiers ids = options.Identifiers;

    public IExpression? TryParse ()
    {
        if (!IsIdentifierStart()) return null;

        Reset();

        while (IsIdentifierContent())
            str.Append(ctx.Consume());
        var id = str.ToString();

        return TryFunction(id) ?? TryBoolean(id) ?? new Variable(id);

        bool IsIdentifierStart () => ctx.Is(char.IsLetter) && ctx.IsTopAndUnquoted;
        bool IsIdentifierContent () => ctx.Is(char.IsLetterOrDigit) || ctx.Is('_');
    }

    private void Reset ()
    {
        str.Clear();
    }

    private IExpression? TryBoolean (string id)
    {
        return
            id.Equals(ids.True, StringComparison.OrdinalIgnoreCase) ? new Boolean(true) :
            id.Equals(ids.False, StringComparison.OrdinalIgnoreCase) ? new Boolean(false) :
            null;
    }

    private IExpression? TryFunction (string id)
    {
        if (!IsFunctionOpen()) return null;

        ctx.Move();

        while (!IsFunctionClose())
            str.Append(ctx.Consume());
        if (!new ExpressionParser(options).TryParse(str.ToString(), out var fn))
            throw new Error("Failed to parse function parameters.");

        return fn;

        bool IsFunctionOpen () => ctx.Is('(') && ctx.IsTopAndUnquoted;
        bool IsFunctionClose () => ctx.Is(')') && ctx.IsTopAndUnquoted;
    }
}
