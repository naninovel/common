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
}
