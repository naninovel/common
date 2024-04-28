using System.Text;
using Naninovel.Parsing;

namespace Naninovel.Expression;

internal class IdentifierParser (ParseContext ctx, ParseOptions options)
{
    private readonly StringBuilder str = new();
    private readonly Identifiers ids = options.Identifiers;

    public bool TryParse ()
    {
        if (!IsIdentifierStart()) return false;

        Reset();

        while (IsIdentifierContent())
            str.Append(ctx.Consume());

        var id = str.ToString();
        var exp = TryFunction(id) ?? TryBoolean(id) ?? new Variable(id);
        ctx.AddParsed(exp);

        return true;

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

        var @params = new List<IExpression>();

        while (!IsFunctionClose())
            if (IsParamSeparator())
            {
                @params.Add(ParseParameter(str.ToString()));
                str.Clear();
                ctx.Move();
            }
            else str.Append(ctx.Consume());

        return new Function(id, @params);

        bool IsFunctionOpen () => ctx.Is('(') && ctx.IsTopAndUnquoted;
        bool IsFunctionClose () => ctx.Is(')') && ctx.IsTopAndUnquoted;
        bool IsParamSeparator () => ctx.Is(',') && ctx.IsTopAndUnquoted;
    }

    private IExpression ParseParameter (string text)
    {
        if (!new ExpressionParser(options).TryParse(text, out var exp))
            throw new Error($"Failed to parse '{text}' as function parameter.");
        return exp;
    }
}
