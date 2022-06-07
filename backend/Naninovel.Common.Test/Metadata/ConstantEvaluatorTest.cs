using Xunit;

namespace Naninovel.Metadata.Test;

public class ConstantEvaluatorTest
{
    private string inspectedScript = "";
    private ConstantEvaluator.GetParamValue getParamValue = (id, idx) => null;

    [Fact]
    public void WhenNullOrEmptyReturnsUnmodified ()
    {
        Assert.Null(Evaluate(null));
        Assert.Empty(Evaluate(""));
    }

    [Fact]
    public void WhenDoesntContainExpressionsReturnsUnmodified ()
    {
        Assert.Equal("foo", Evaluate("foo"));
    }

    [Fact]
    public void CanInjectScriptName ()
    {
        inspectedScript = "foo";
        Assert.Equal("foo", Evaluate("{$Script}"));
    }

    [Fact]
    public void DoesntModifyContentOutsideOfExpression ()
    {
        inspectedScript = "bar";
        Assert.Equal("foo/bar", Evaluate("foo/{$Script}"));
    }

    [Fact]
    public void CanInjectParamValues ()
    {
        getParamValue = (id, idx) => id == "1" ? "foo" : "bar";
        Assert.Equal("foo/bar", Evaluate("{:1}/{:2}"));
    }

    [Fact]
    public void CanParamValueIsNullEmptyReturned ()
    {
        getParamValue = (id, idx) => null;
        Assert.Empty(Evaluate("{:foo}"));
    }

    [Fact]
    public void CanUseNullCoalescing ()
    {
        inspectedScript = "bar";
        getParamValue = (id, idx) => id == "foo" ? "foo" : null;
        Assert.Equal("foobar", Evaluate("{:foo??:bar}{:bar??$Script}"));
    }

    [Fact]
    public void CanGetEnumerableParamValue ()
    {
        inspectedScript = "nya";
        getParamValue = (id, idx) => idx == 0 ? "foo" : idx == 1 ? "bar" : null;
        Assert.Equal("foo/bar/nya", Evaluate("{:foo[0]}/{:bar[1]}/{:nya[2]??$Script}"));
    }

    [Fact]
    public void WhenIncorrectExpressionReturnsEmpty ()
    {
        Assert.Empty(Evaluate("{:foo??bar}"));
    }

    private string Evaluate (string expression)
    {
        return ConstantEvaluator.EvaluateName(expression, inspectedScript, getParamValue);
    }
}
