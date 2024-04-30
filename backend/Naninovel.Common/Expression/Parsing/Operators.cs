namespace Naninovel.Expression;

internal static class Operators
{
    public static readonly IReadOnlyDictionary<string, IBinaryOperator> Binary =
        new Dictionary<string, IBinaryOperator> {
            ["+"] = new Add(),
            ["-"] = new Subtract(),
            ["*"] = new Multiply(),
            ["/"] = new Divide(),
            ["%"] = new Remainder(),
            ["^"] = new Power(),
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

    public static readonly IReadOnlyDictionary<string, IUnaryOperator> Unary =
        new Dictionary<string, IUnaryOperator> {
            ["!"] = new NegateBoolean(),
            ["-"] = new NegateNumeric(),
            ["+"] = new AddUnary()
        };

    public static readonly IReadOnlyDictionary<string, bool> Control =
        new Dictionary<string, bool> {
            [","] = default,
            ["("] = default,
            [")"] = default,
            [":"] = default,
            ["?"] = default
        };

    public static bool IsOperator (char c1, char? c2, out string op)
    {
        foreach (var key in Binary.Keys)
            if (CompareKey(c1, c2, key))
            {
                op = key;
                return true;
            }

        foreach (var key in Unary.Keys)
            if (CompareKey(c1, c2, key))
            {
                op = key;
                return true;
            }

        foreach (var key in Control.Keys)
            if (CompareKey(c1, c2, key))
            {
                op = key;
                return true;
            }

        op = null!;
        return false;
    }

    private static bool CompareKey (char c1, char? c2, string key)
    {
        if (!c2.HasValue) return key.Length == 1 && key[0] == c1;
        return key.Length == 2 && key[0] == c1 && key[1] == c2;
    }
}
