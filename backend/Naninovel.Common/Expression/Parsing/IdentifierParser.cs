using System.Text;
using Naninovel.Parsing;

namespace Naninovel.Expression;

internal class IdentifierParser (ParseContext ctx, Identifiers ids)
{
    private readonly StringBuilder str = new();

    public bool TryParse ()
    {
        if (!IsIdentifierStart()) return false;

        Reset();

        while (IsIdentifierContent())
            str.Append(ctx.Consume());
        var id = str.ToString();

        var exp = default(IExpression);
        if (!TryFunction(id, out exp) && !TryBoolean(id, out exp))
            exp = new Variable(id);

        return true;

        bool IsIdentifierStart () => ctx.Is(char.IsLetter) && ctx.IsTopAndUnquoted;
        bool IsIdentifierContent () => ctx.Is(char.IsLetterOrDigit) || ctx.Is('_');
    }

    private void Reset ()
    {
        str.Clear();
    }

    private bool TryBoolean (string id, out IExpression boolean)
    {
        boolean =
            id.Equals(ids.True, StringComparison.OrdinalIgnoreCase) ? new Boolean(true) :
            id.Equals(ids.False, StringComparison.OrdinalIgnoreCase) ? new Boolean(false) :
            default!;
        return boolean != default!;
    }

    private bool TryFunction (string id, out IExpression fn)
    {
        fn = default!;

        if (!IsFunctionOpen()) return false;

        ctx.Move();

        while (!IsFunctionClose())
            str.Append(ctx.Consume());

        return true;

        bool IsFunctionOpen () => ctx.Is('(') && ctx.IsTopAndUnquoted;
        bool IsFunctionClose () => ctx.Is(')') && ctx.IsTopAndUnquoted;
    }
}
