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

    private static readonly IReadOnlyDictionary<char, string> singleChar =
        BuildSingleChars(Binary.Keys, Unary.Keys, [",", "(", ")", ":", "?"]);
    private static readonly IReadOnlyDictionary<(char c1, char c2), string> doubleChar =
        BuildDoubleChars(Binary.Keys, Unary.Keys);

    public static bool IsOperator (char c1, char c2, out string op)
    {
        return ((op = doubleChar.TryGetValue((c1, c2), out var op2) ? op2 : null!) ??
                (op = singleChar.TryGetValue(c1, out var op1) ? op1 : null!)) != null;
    }

    private static Dictionary<char, string> BuildSingleChars (params IEnumerable<string>[] collections)
    {
        var map = new Dictionary<char, string>();
        foreach (var collection in collections)
        foreach (var str in collection)
            if (str.Length == 1)
                map[str[0]] = str;
        return map;
    }

    private static Dictionary<(char c1, char c2), string> BuildDoubleChars (params IEnumerable<string>[] collections)
    {
        var map = new Dictionary<(char c1, char c2), string>();
        foreach (var collection in collections)
        foreach (var str in collection)
            if (str.Length == 2)
                map[(str[0], str[1])] = str;
        return map;
    }
}
