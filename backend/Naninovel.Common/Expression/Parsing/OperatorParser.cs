using System.Text;

namespace Naninovel.Expression;

internal class OperatorParser (ParseContext ctx)
{
    private readonly StringBuilder str = new();

    private readonly Dictionary<string, IBinaryOperator> binary = new() {
        ["+"] = new Add(),
        ["-"] = new Subtract(),
        ["*"] = new Multiply(),
        ["/"] = new Divide(),
        ["&"] = new And(),
        ["&&"] = new And(),
        ["|"] = new Or(),
        ["||"] = new Or(),
        ["="] = new Equal(),
        ["=="] = new Equal(),
        ["!="] = new NotEqual(),
        [">"] = new Greater(),
        ["<"] = new Less(),
        [">="] = new GreaterOrEqual(),
        ["<="] = new LessOrEqual()
    };

    private readonly Dictionary<string, IUnaryOperator> unary = new() {
        ["!"] = new NegateBoolean(),
        ["-"] = new NegateNumeric()
    };

    public IBinaryOperator? TryBinary ()
    {
        if (!IsBinary()) return null;

        Reset();

        while (IsBinary())
            str.Append(ctx.Consume());

        var key = str.ToString();
        if (!binary.TryGetValue(key, out var op))
            throw new Error($"Unknown binary operator: {key}");

        return op;
    }

    public IUnaryOperator? TryUnary ()
    {
        if (!IsUnary()) return null;

        Reset();

        while (IsUnary())
            str.Append(ctx.Consume());

        var key = str.ToString();
        if (!unary.TryGetValue(key, out var op))
            throw new Error($"Unknown unary operator: {key}");

        return op;
    }

    private void Reset ()
    {
        str.Clear();
    }

    private bool IsBinary ()
    {
        if (!ctx.IsTopAndUnquoted) return false;
        return ctx.Is(c => ContainsCharInKey(c, binary.Keys));
    }

    private bool IsUnary ()
    {
        if (!ctx.IsTopAndUnquoted) return false;
        return ctx.Is(c => ContainsCharInKey(c, unary.Keys));
    }

    private bool ContainsCharInKey (char ch, IEnumerable<string> keys)
    {
        foreach (var key in keys)
        foreach (var c in key)
            if (c == ch)
                return true;
        return false;
    }
}
