namespace Naninovel.Expression.Test;

public class ExpressionTest
{
    private readonly Parser parser = new(new());
    private readonly Evaluator evaluator = new(new());

    [
        Theory,
        InlineData("1+2", 1.0 + 2.0),
        InlineData("1+2+3", 1.0 + 2.0 + 3.0),
        InlineData("1+2*3", 1.0 + 2.0 * 3.0),
        InlineData("(1+2)*-3", (1.0 + 2.0) * -3.0),
        InlineData("(1+2)*(3+4)", (1.0 + 2.0) * (3.0 + 4.0)),
        InlineData("-1", -1.0),
        InlineData("- 1.0", -1.0),
        InlineData("-(1+2) * -(3+4)", -(1.0 + 2.0) * -(3.0 + 4.0)),
        InlineData(" - ( 1 + 2 ) * - ( 3 + 4 ) ", -(1.0 + 2.0) * -(3.0 + 4.0)),
        InlineData("-((1+2) / (3+4))", -((1.0 + 2.0) / (3.0 + 4.0))),
        InlineData("-(-(1+2) * (3+4))", -(-(1.0 + 2.0) * (3.0 + 4.0))),
        InlineData("-(-(1+2) / -(3+4))", -(-(1.0 + 2.0) / -(3.0 + 4.0))),
        InlineData("1 /2-1+2*-(3-1.5)/ 2+-1", 1.0 / 2.0 - 1.0 + 2.0 * -(3.0 - 1.5) / 2.0 + -1.0),
        InlineData("num_10+num_2-2", 10.0),
    ]
    public void CanEvaluateNumericExpressions (string text, double expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.Equal(expected, evaluator.Evaluate<double>(expression));
    }

    [
        Theory,
        InlineData("1 < 2", true),
        InlineData("1 > 2", false),
        InlineData("1 = 1", true),
        InlineData("1 = 2", false),
        InlineData("1 != 1", false),
        InlineData("1 != 2", true),
        InlineData("true", true),
        InlineData("false", false),
        InlineData("!true", false),
        InlineData("!false", true),
        InlineData("true = true", true),
        InlineData("true != true", false),
        InlineData("true = false", false),
        InlineData("true != false", true),
        InlineData("false = false", true),
        InlineData("false != false", false),
        InlineData("\"foo\" = \"foo\"", true),
        InlineData("\"foo\" != \"foo\"", false),
        InlineData("\"foo\" = \"bar\"", false),
        InlineData("\"foo\" != \"bar\"", true),
        InlineData("1 <= 2", true),
        InlineData("1 >= 2", false),
        InlineData("1 >= 1", true),
        InlineData("1 <= 1", true),
        InlineData("1 < 2 | 1 > 2", true),
        InlineData("1 < 2 & 1 > 2", false),
        InlineData("1 < 2 || 1 > 2", true),
        InlineData("1 < 2 && 1 > 2", false),
        InlineData("!(1 > 2)", true),
        InlineData("(1+2) < (3+4)", true),
        InlineData("(1+2) > (3+4)", false),
        InlineData("(3+4) > (1+2)", true),
        InlineData("(3+4) < (1+2)", true),
        InlineData("(1+2) <= (3+4)", true),
        InlineData("(3+4) >= (1+2)", false),
        InlineData("(1+2) >= (3+4)", true),
        InlineData("(3+4) <= (1+2)", false),
        InlineData("(1+2) == (3+4)", false),
        InlineData("(1+2) == (2+1)", true),
        InlineData("(1+2) >= (2+1)", true),
        InlineData("(1+2) <= (2+1)", true),
        InlineData("positive", true),
        InlineData("negative", false),
        InlineData("!positive", false),
        InlineData("!negative", true),
        InlineData("positive & negative", false),
    ]
    public void CanEvaluateBooleanExpressions (string text, bool expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.Equal(expected, evaluator.Evaluate<bool>(expression));
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
        Assert.Equal(expected, evaluator.Evaluate<string>(expression));
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
        Assert.Equal(expected, evaluator.Evaluate(expression));
    }
}
