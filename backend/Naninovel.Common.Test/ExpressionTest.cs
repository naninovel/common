namespace Naninovel.Expression.Test;

public class ExpressionTest
{
    private readonly ExpressionParser parser = new(new());
    private readonly ExpressionEvaluator evaluator = new(new());

    [
        Theory,
        InlineData("1+2", 1.0 + 2.0),
        InlineData("1+2+3", 1.0 + 2.0 + 3.0),
        InlineData("1+2*3", 1.0 + 2.0 * 3.0),
        InlineData("(1+2)*-3", (1.0 + 2.0) * -3.0),
        InlineData("(1+2)*(3+4)", (1.0 + 2.0) * (3.0 + 4.0)),
        InlineData("num_10+num_2-2", 10.0),
    ]
    public void CanEvaluateNumericExpressions (string text, double expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.True(evaluator.TryEvaluate<double>(expression, out var actual));
        Assert.Equal(expected, actual);
    }

    [
        Theory,
        InlineData("(1+2) < (3+4)", true),
        InlineData("(1+2) <= (3+4)", true),
        InlineData("(1+2) == (3+4)", false),
        InlineData("true", true),
        InlineData("false", false),
        InlineData("1 < 2 || 1 > 2", true),
        InlineData("1 < 2 && 1 > 2", false),
        InlineData("1 < 2 | 1 > 2", true),
        InlineData("1 < 2 & 1 > 2", false),
        InlineData("!(1 > 2)", true),
        InlineData("positive", true),
        InlineData("negative", false),
        InlineData("positive & negative", false),
    ]
    public void CanEvaluateBooleanExpressions (string text, bool expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.True(evaluator.TryEvaluate<bool>(expression, out var actual));
        Assert.Equal(expected, actual);
    }

    [
        Theory,
        InlineData("\"foo\"", "foo"),
        InlineData("foo", "foo"),
        InlineData("bar", "bar"),
        InlineData("true ? \"foo\" : \"bar\"", "foo"),
        InlineData("negative ? foo : bar", "bar"),
    ]
    public void CanEvaluateStringExpressions (string text, string expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.True(evaluator.TryEvaluate<string>(expression, out var actual));
        Assert.Equal(expected, actual);
    }

    [
        Theory,
        InlineData("1", 1),
        InlineData("\"1\"", "1"),
        InlineData("\"true\"", true),
    ]
    public void CanEvaluateDynamicExpressions (string text, object expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.True(evaluator.TryEvaluate(expression, expected.GetType(), out var actual));
        Assert.Equal(expected, actual);
    }
}