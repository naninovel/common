namespace Naninovel.Expression;

internal class OperatorParser
{
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
    };

    private readonly Dictionary<string, IUnaryOperator> unary = new() {
        ["!"] = new NegateBoolean(),
        ["-"] = new NegateNumeric(),
    };

    public bool TryParseBinary (string text, out IBinaryOperator op)
    {
        return binary.TryGetValue(text, out op);
    }

    public bool TryParseUnary (string text, out IUnaryOperator op)
    {
        return unary.TryGetValue(text, out op);
    }
}
